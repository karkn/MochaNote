/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Deployment.Application;
using System.Configuration;
using System.Drawing;
using Mkamo.Common.IO;
using System.Runtime.Serialization;
using Mkamo.Memopad.Core;

namespace Mkamo.DataConverter.Core {
    public static class MemopadConstsV1 {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static readonly string MemoRoot;
        public static readonly string LogRootName = "log";
        public static readonly string LogRoot;
        public static readonly string LockFileName;
        public static readonly string LockFilePath;
        public static readonly BootstrapSettings BootstrapSettings;


        internal static readonly string ModelRootName = "model";
        internal static readonly string MementoRootName = "memento";
        internal static readonly string EmbeddedRootName = "embedded";
        internal static readonly string ModelRoot;
        internal static readonly string MementoRoot;
        internal static readonly string EmbeddedRoot;

        internal static readonly string EmbeddedFileRoot;
        internal static readonly string ProxyRoot;

        internal static readonly string BootstrapSettingsFilePath;
        internal static readonly string SettingsFilePath;
        internal static readonly string WindowSettingsFilePath;

        internal static readonly string MemoInfosFileName = "memoinfos.xml";
        internal static readonly string RemovedMemoInfosFileName = "removed.xml";
        internal static readonly string RemovedEmbeddedFileIdsFileName = "removedembedded.xml";
        internal static readonly string MemoInfosFilePath;
        internal static readonly string RemovedMemoInfosFilePath;
        internal static readonly string RemovedEmbeddedFileIdsFilePath;

        internal static readonly string RecentlyClosedMemoIdsFilePath;
        internal static readonly string RecentlyCreatedMemoIdsFilePath;
        internal static readonly string RecentlyModifiedMemoIdsFilePath;

        internal static readonly string StartUpShortcutFilePath;

        internal const string MemoFileExtension = "mem";

        internal static readonly string ProxyAssemblyName = "Mkamo.Model.Proxies";
        internal static readonly string ProxyAssemblyFilePath;

        //internal static readonly string TutorialPath = Path.Combine(Application.StartupPath, @"Help\Tutorial\start_tutorial.html");

//#if DEBUG
//        internal static readonly string LatestUrl = "http://www.confidante.jp/file/latest_debug.txt";
//#else
//        internal static readonly string LatestUrl = "http://www.confidante.jp/file/latest.txt";
//#endif

        /// MemoTextの図形のLocationと一番最初の文字の左上の点との差
        /// MemoTextのPaddingとBlockのMargin/Paddingで値を足したものの符号をマイナスにしたもの
        /// BlockのLineSpaceはひかない?
        //internal static readonly Size MemoTextFirstCharDelta = new Size(-4, -4);

        //internal static readonly int BlockLineSpace = 2;

        /// <summary>
        /// caret初期位置．
        /// </summary>
        //internal static readonly Point DefaultCaretPosition = new Point(16, 16);

        /// <summary>
        /// caret移動量
        /// </summary>
        //internal const int CaretMoveDelta = 16;


        // ========================================
        // static method
        // ========================================
        static MemopadConstsV1() {
            var localAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appDataRoot = Path.Combine(localAppDataFolder, @"mkamo\Confidante\1.0");

            BootstrapSettingsFilePath = Path.Combine(appDataRoot, @"bootstrap.xml");
            BootstrapSettings = BootstrapSettings.LoadBootstrapSettings(BootstrapSettingsFilePath);


            /// MemoRootの初期化
            //var clArgs = System.Environment.GetCommandLineArgs();
            //if (clArgs != null && clArgs.Length > 1) {
            //    /// コマンドライン引数からMemoRoot取得
            //    MemoRoot = clArgs[1];
            //    UseCommandLineMemoRoot = true;

            //} else {
                /// bootstrap.xmlからMemoRoot取得
                if (string.IsNullOrEmpty(BootstrapSettings.MemoRoot)) {
                    BootstrapSettings.MemoRoot = Path.Combine(appDataRoot, @"memo");
                }
                MemoRoot = BootstrapSettings.MemoRoot;
                //UseCommandLineMemoRoot = false;
            //}

            //try {
            //    PathUtil.EnsureDirectoryExists(MemoRoot);
            //} catch (Exception e) {
            //    Logger.Error("Can't create MemoRoot folder", e);
            //    try {
            //        /// デフォルトの設定でやり直す
            //        BootstrapSettings.MemoRoot = Path.Combine(appDataRoot, @"memo");
            //        MemoRoot = BootstrapSettings.MemoRoot;
            //        //UseCommandLineMemoRoot = false;
            //        PathUtil.EnsureDirectoryExists(MemoRoot);
            //    } catch (Exception ex) {
            //        Logger.Error("Can't create Default MemoRoot folder", ex);
            //        throw;
            //    }
            //}

            LockFileName = "lock";
            LockFilePath = Path.Combine(MemoRoot, LockFileName);

            ModelRoot = Path.Combine(MemoRoot, ModelRootName);
            MementoRoot = Path.Combine(MemoRoot, MementoRootName);
            LogRoot = Path.Combine(MemoRoot, LogRootName);
            EmbeddedRoot = Path.Combine(MemoRoot, EmbeddedRootName);
            EmbeddedFileRoot = Path.Combine(EmbeddedRoot, "file");

            SettingsFilePath = Path.Combine(MemoRoot, "settings.xml");
            WindowSettingsFilePath = Path.Combine(MemoRoot, "window." + Environment.MachineName + ".xml");
            MemoInfosFilePath = Path.Combine(MemoRoot, MemoInfosFileName);
            RemovedMemoInfosFilePath = Path.Combine(MemoRoot, RemovedMemoInfosFileName);
            RemovedEmbeddedFileIdsFilePath = Path.Combine(MemoRoot, RemovedEmbeddedFileIdsFileName);
            RecentlyClosedMemoIdsFilePath = Path.Combine(MemoRoot, "recentlyclosed.xml");
            RecentlyCreatedMemoIdsFilePath = Path.Combine(MemoRoot, "recentlycreated.xml");
            RecentlyModifiedMemoIdsFilePath = Path.Combine(MemoRoot, "recentlymodified.xml");

            ProxyRoot = Path.Combine(appDataRoot, "proxy");
            ProxyAssemblyFilePath = Path.Combine(ProxyRoot, ProxyAssemblyName + ".dll");

            //BackupRoot = Path.Combine(appDataRoot, "backup");

            StartUpShortcutFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Startup),
                "Confidante.lnk"
            );
        }

    }
}
