/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using Mkamo.Memopad.Internal.Core;
using System.IO;
using System.Diagnostics;
using Mkamo.Common.IO;
using Mkamo.Memopad.Core;
using Mkamo.Common.Win32.User32;
using Mkamo.Memopad.Internal.Forms;
using log4net.Config;
using Mkamo.Common.Crypto;
using System.Text;

namespace Mkamo.Confidante {
    static class Program {
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(global::System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        //private static Form _splash;
        private static FileStream lockFileStream;

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main() {

            var args = Environment.GetCommandLineArgs();
            var memoRoot = args != null && args.Length > 1? args[1]: "";
            var mutexNamePostfix = memoRoot.Replace('\\', '_');
            var mutexName = "Mkamo.Confidante_" + mutexNamePostfix;

            var isCreatedNew = false;
            using (var mutex = new Mutex(true, mutexName, out isCreatedNew)) {
                if (isCreatedNew) {
                    Application.ThreadException += HandleApplicationThreadException;
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    //_splash = new SplashScreenForm();
                    //_splash.Show();
                    //Application.Idle += HandleApplicationIdle;

                    try {
                        var root = MemopadConsts.MemoRoot;
                    } catch (Exception e) {
                        MessageBox.Show(
                            "起動処理時にエラーが発生しました。" + Environment.NewLine +
                            e.Message,
                            "起動エラー",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }

                    try {
                        PathUtil.EnsureDirectoryExists(MemopadConsts.MemoRoot);
                    } catch (Exception e) {
                        MessageBox.Show(
                            "Confidanteが使用するフォルダを作成できません。" + Environment.NewLine +
                            e.Message,
                            "起動エラー",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return;
                    }

                    ConfigureLog();

                    var msg = Lock();
                    if (msg != null) {
                        MessageBox.Show(msg, "起動エラー");
                        return;
                    }

                    try {
                        var appContext = CreateAppContext();
                        if (appContext != null) {
                            Application.Run(appContext);
                        }
                    } catch (ArgumentException e) {
                        Logger.Error("Error in Main ", e);
                        Application.Exit();
                    } catch (Exception e) {
                        Logger.Error("Error in Main ", e);
                        MessageBox.Show(
                            "エラーによりアプリケーションを終了します。" + Environment.NewLine +
                            e.Message,
                            "エラー"
                        );
                        Application.Exit();
                    } finally {
                        Unlock();
                    }

                } else {
                    /// すでに起動されている場合はWindowを前面に
                    //User32Util.ActivateWindow(
                    //    null,
                    //    "Confidante",
                    //    Process.GetCurrentProcess().MainModule.FileName
                    //);
                    MessageBox.Show(
                        "Confidanteはすでに起動しています。",
                        "起動エラー",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        private static string Lock() {
            try {
                lockFileStream = new FileStream(
                    MemopadConsts.LockFilePath,
                    FileMode.CreateNew,
                    FileAccess.ReadWrite
                );
            
            } catch (Exception e) {
                Logger.Warn("Can't create lock file.", e);

                var machineName = string.Empty;
                try {
                    lockFileStream = new FileStream(
                        MemopadConsts.LockFilePath,
                        FileMode.Open,
                        FileAccess.Read
                    );
                    var reader = new StreamReader(lockFileStream);
                    machineName = reader.ReadToEnd();

                } catch (Exception e2) {
                    Logger.Warn("Can't read lock file", e2);
                    return "他のConfidanteが同じノートデータを使用している可能性があります。";

                } finally {
                    if (lockFileStream != null) {
                        lockFileStream.Close();
                    }
                }

                if (machineName == Environment.MachineName) {
                    try {
                        File.Delete(MemopadConsts.LockFilePath);
                    } catch (Exception e2) {
                        Logger.Warn("Can't delete lock file", e2);
                        return "他のConfidanteが同じノートデータを使用している可能性があります。";
                    }

                    return Lock();

                } else {
                    return
                        machineName + "で起動しているConfidanteが同じノートデータを使用しています。\r\n" +
                        machineName + "で起動しているConfidanteを終了してください。";
                }
            }

            var writer = default(StreamWriter);
            try {
                writer = new StreamWriter(lockFileStream);
                writer.Write(Environment.MachineName);

                return null;

            } catch (Exception e) {
                Logger.Warn("Can't write lock file", e);
                return "ロックファイルの書き込みに失敗しました。";

            } finally {
                if (writer != null) {
                    writer.Close();
                }
                if (lockFileStream != null) {
                    lockFileStream.Close();
                }
            }
        }

        private static void Unlock() {
            if (lockFileStream != null) {
                lockFileStream.Close();
                lockFileStream = null;
            }
            if (File.Exists(MemopadConsts.LockFilePath)) {
                File.Delete(MemopadConsts.LockFilePath);
            }
        }

        private static void ConfigureLog() {
            var logRoot = MemopadConsts.LogRoot;
            try {
                PathUtil.EnsureDirectoryExists(logRoot);
            } catch (Exception e) {
                MessageBox.Show(
                    "Confidanteが使用するフォルダを作成できません。" + Environment.NewLine +
                    e.Message,
                    "起動エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            Environment.SetEnvironmentVariable("LogRoot", logRoot);
            XmlConfigurator.Configure();
        }

        //private static void HandleApplicationIdle(object sender, EventArgs e) {
        //    if (_splash != null && !_splash.IsDisposed) {
        //        _splash.Close();
        //        _splash.Dispose();
        //        _splash = null;
        //        Application.Idle -= HandleApplicationIdle;
        //    }
        //}

        private static void HandleApplicationThreadException(object sender, ThreadExceptionEventArgs e) {
            Logger.Error("Unhandled Exception", e.Exception);
            //Application.Exit();
        }

        private static MemopadAppContext CreateAppContext() {
            return new MochaNoteAppContext();
        }

    }
}
