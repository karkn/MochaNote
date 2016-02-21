/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Memopad.Core;
using System.Windows.Forms;
using Mkamo.Control.Progress;
using System.IO;
using log4net.Config;
using System.ComponentModel;

namespace Mkamo.Memopad.Internal.Core {
    internal class DataFolderExporter {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // ========================================
        // field
        // ========================================
        private MemopadApplication _app;

        // ========================================
        // constructor
        // ========================================
        public DataFolderExporter() {
            _app = MemopadApplication.Instance;
        }

        // ========================================
        // method
        // ========================================
        public bool ExportTo(string dirPath) {
            if (MemoDataFolderSync.ExistsLockFile(dirPath)) {
                MessageBox.Show(
                    _app.MainForm,
                    "他のMochaNoteが使用中のノート格納フォルダにはエクスポートできません。",
                    "エクスポートエラー"
                );
                return false;
            }
            if (MemoDataFolderSync.ContainsNoMemo(dirPath)) {
                MessageBox.Show(
                    _app.MainForm,
                    "ノートデータ以外のフォルダやファイルが格納されているフォルダにはエクスポートできません。" + Environment.NewLine +
                    "空のフォルダか以前にノートをエクスポートしたフォルダを選択してください。",
                    "エクスポートエラー"
                );
                return false;
            }

            var ret = MessageBox.Show(
                _app.MainForm,
                "エクスポートを実行すると\"" + dirPath + "\"に格納されているノートデータ以外のファイルやフォルダは削除されます。" + Environment.NewLine +
                "エクスポートを実行してもよろしいですか。",
                "エクスポートの確認",
                MessageBoxButtons.YesNo
            );
            if (ret == DialogResult.No) {
                MessageBox.Show(
                    _app.MainForm,
                    "エクスポートを中止しました。",
                    "エクスポートの中止"
                );
                return false;
            }

            _app.SaveAllMemos();
            _app.SaveRecentIds();
            _app.SaveFusenFormIds();

            _app.MainForm.SaveFormSettings(_app._WindowSettings);
            MemopadSettings.SaveSettings(_app._Settings);
            MemopadWindowSettings.SaveWindowSettings(_app._WindowSettings);

            _app.CloseConnections();


            var dialog = new ProgressDialog();
            dialog.Text = "エクスポートの進捗";
            dialog.SupportCancel = false;
            dialog.Font = _app.Theme.CaptionFont;
            dialog.BackgroundWorker.DoWork += DoExportAsync;
            dialog.BackgroundWorker.RunWorkerCompleted += (sender, e) => {
                if (e.Error != null) {
                    MessageBox.Show(_app.MainForm, "エクスポートに失敗しました。", "エクスポートエラー");
                } else if (e.Cancelled) {
                    MessageBox.Show(_app.MainForm, "エクスポートをキャンセルしました。", "エクスポートのキャンセル");
                }
                dialog.Close();
                dialog.Dispose();

                _app.OpenConnections();
            };
            dialog.Run(_app.MainForm, dirPath);
    
            _app.BootstrapSettings.LastExportDirectory = dirPath;

            return true;
        }

        public bool ImportFrom(string dirPath) {
            if (!MemoDataFolderSync.ContainsMemo(dirPath)) {
                MessageBox.Show(
                    _app.MainForm,
                    "ノートデータが格納されていないフォルダからはインポートできません。",
                    "インポートエラー"
                );
                return false;
            }
            if (MemoDataFolderSync.ExistsLockFile(dirPath)) {
                MessageBox.Show(
                    _app.MainForm,
                    "他のMochaNoteが使用中のノート格納フォルダからはインポートできません。",
                    "インポートエラー"
                );
                return false;
            }

            var ret = MessageBox.Show(
                _app.MainForm,
                "\"" + dirPath + "\"からのインポートを実行してもよろしいですか。",
                "インポートの確認",
                MessageBoxButtons.YesNo
            );
            if (ret == DialogResult.No) {
                MessageBox.Show(
                    _app.MainForm,
                    "インポートを中止しました。",
                    "インポートの中止"
                );
                return false;
            }

            _app.SaveAllMemos();

            _app.SaveRecentIds();
            _app.SaveFusenFormIds();

            _app.MainForm.SaveFormSettings(_app._WindowSettings);
            MemopadSettings.SaveSettings(_app._Settings);
            MemopadWindowSettings.SaveWindowSettings(_app._WindowSettings);

            _app.BootstrapSettings.LastImportDirectory = dirPath;
            _app.SaveBootstrapSettings();

            _app.CloseConnections();
            log4net.LogManager.Shutdown();

            var dialog = new ProgressDialog();
            dialog.Text = "インポートの進捗";
            dialog.SupportCancel = false;
            dialog.Font = _app.Theme.CaptionFont;
            dialog.BackgroundWorker.DoWork += DoImportAsync;
            dialog.BackgroundWorker.RunWorkerCompleted += (sender, e) => {
                if (e.Error != null) {
                    MessageBox.Show(_app.MainForm, "インポートに失敗しました。", "インポートエラー");
                } else if (e.Cancelled) {
                    MessageBox.Show(_app.MainForm, "インポートをキャンセルしました。", "インポートのキャンセル");
                }
                dialog.Close();
                dialog.Dispose();

                /// インポート前のmemoinfosなどが上書きされないように
                /// 終了時のSaveAll()を防ぐ
                _app._PreventSaveAll = true;
                Application.Restart();
            };
            dialog.Run(_app.MainForm, dirPath);

            return true;
        }

#if false
        public void BackupWithUI(string fileName) {
            /// SaveAllMemos()はUIスレッドで呼び出す必要がある
            _app.SaveAllMemos();

            _app.SaveRecentIds();
            _app.SaveFusenFormIds();

            _app.MainForm.SaveFormSettings(_app._WindowSettings);
            MemopadSettings.SaveSettings(_app._Settings);
            MemopadWindowSettings.SaveWindowSettings(_app._WindowSettings);

            var dialog = new ProgressDialog();
            dialog.Text = "バックアップの進捗";
            dialog.SupportCancel = true;
            dialog.Font = _app.Theme.CaptionFont;
            dialog.BackgroundWorker.DoWork += DoBackupAsync;
            dialog.BackgroundWorker.RunWorkerCompleted += (sender, e) => {
                if (e.Error != null) {
                    MessageBox.Show(_app.MainForm, "バックアップに失敗しました。", "バックアップエラー");
                } else if (e.Cancelled) {
                    MessageBox.Show(_app.MainForm, "バックアップをキャンセルしました。", "バックアップのキャンセル");
                }
                dialog.Close();
                dialog.Dispose();
            };
            dialog.Run(_app.MainForm, fileName);
        }

        public void RestoreWithUI(string fileName) {
            /// SaveAllMemos()はUIスレッドで呼び出す必要がある
            _app.SaveAllMemos();

            var dialog = new ProgressDialog();
            dialog.Text = "復元の進捗";
            dialog.Font = _app.Theme.CaptionFont;
            dialog.BackgroundWorker.DoWork += DoRestoreAsync;
            dialog.BackgroundWorker.RunWorkerCompleted += (sender, e) => {
                if (e.Error != null) {
                    MessageBox.Show(_app.MainForm, "復元に失敗しました。", "復元エラー");
                }
                dialog.Close();
                dialog.Dispose();

                /// 復元前のmemoinfosなどが上書きされないように
                /// 終了時のSaveAll()を防ぐ
                _app._PreventSaveAll = true;
                Application.Restart();
            };
            dialog.Run(_app.MainForm, fileName);
        }
#endif
        
        private void DoExportAsync(object sender, DoWorkEventArgs dwe) {
            lock (_app._Lock) {
                var worker = (BackgroundWorker) sender;

                try {
                    var dirPath = dwe.Argument as string;

                    var sync = new MemoDataFolderSync(
                        (progress) => {
                            worker.ReportProgress(progress);
                        }
                    );
                    sync.SyncMemoDataFoldersTo(dirPath);
                    MemoDataFolderSync.MakeMemoDataFolderIcon(dirPath);

                } catch (Exception e) {
                    Logger.Warn("Export failed", e);
                    throw;
                }
            }
        }

        private void DoImportAsync(object sender, DoWorkEventArgs dwe) {
            lock (_app._Lock) {
                var worker = (BackgroundWorker) sender;

                try {
                    var dirPath = dwe.Argument as string;

                    var sync = new MemoDataFolderSync(
                        (progress) => {
                            worker.ReportProgress(progress);
                        }
                    );
                    sync.SyncMemoDataFoldersFrom(dirPath);

                } catch (Exception e) {
                    Logger.Warn("Export failed", e);
                    throw;
                }
            }
        }

#if false
        private void DoBackupAsync(object sender, DoWorkEventArgs dwe) {
            lock (_app._Lock) {
                var worker = (BackgroundWorker) sender;

                try {
                    /// SaveAllMemos()をUIスレッド以外から呼び出すと内部的にエラーになってしまう
                    ///SaveAllMemos();

                    worker.ReportProgress(20);
                    if (worker.CancellationPending) {
                        dwe.Cancel = true;
                        return;
                    }
    
                    using (var zip = new ZipFile(Encoding.GetEncoding("shift_jis"))) {
                        worker.ReportProgress(40);
                        if (worker.CancellationPending) {
                            dwe.Cancel = true;
                            return;
                        }
    
                        zip.Comment = "Created by MochaNote";
                        zip.CompressionLevel = CompressionLevel.BestSpeed;
                        var dir = MemopadConsts.MemoRoot;
                        zip.AddDirectory(dir, Path.GetFileName(dir));

                        /// lockファイルを削除
                        zip.RemoveEntry(Path.Combine(Path.GetFileName(dir), "lock"));

                        worker.ReportProgress(60);
                        if (worker.CancellationPending) {
                            dwe.Cancel = true;
                            return;
                        }

                        var fileName = (string) dwe.Argument;
                        zip.Save(fileName);
                        worker.ReportProgress(100);
                        if (worker.CancellationPending) {
                            dwe.Cancel = true;
                            if (File.Exists(fileName)) {
                                File.Delete(fileName);
                            }
                            return;
                        }
                    }
                } catch (Exception e) {
                    Logger.Warn("Backup failed", e);
                    throw;
                }
            }
        }

        private void DoRestoreAsync(object sender, DoWorkEventArgs dwe) {
            lock (_app._Lock) {
                var worker = (BackgroundWorker) sender;

                /// SaveAllMemos()をUIスレッド以外から呼び出すと内部的にエラーになってしまう
                ///SaveAllMemos();
                worker.ReportProgress(20);

                log4net.LogManager.Shutdown();

                var root = MemopadConsts.MemoRoot;
                var rootBak = root + "_restore_bak";
                if (Directory.Exists(rootBak)) {
                    Directory.Delete(rootBak, true);
                }
                if (Directory.Exists(root)) {
                    Directory.Move(root, rootBak);
                }
                worker.ReportProgress(40);

                var fileName = (string) dwe.Argument;

                var tmpFolderName = default(string);
                var tmpFolderPath = default(string);
                try {
                    /// テンポラリフォルダパスの作成
                    tmpFolderName = "_mochanote_restore_" + Guid.NewGuid().ToString();
                    tmpFolderPath = Path.Combine(Path.GetDirectoryName(root), tmpFolderName);

                    var count = 1;
                    while (File.Exists(tmpFolderPath) || Directory.Exists(tmpFolderPath)) {
                        tmpFolderName = "_mochanote_restore_" + Guid.NewGuid().ToString();
                        tmpFolderPath = Path.Combine(Path.GetDirectoryName(root), tmpFolderName);
                        ++count;
                        if (count > 10) {
                            throw new Exception("Cannot create temporary restore folder");
                        }
                    }

                    using (var zip = ZipFile.Read(fileName, Encoding.GetEncoding("shift_jis"))) {
                        worker.ReportProgress(60);

                        if (zip.Comment != "Created by MochaNote") {
                            throw new Exception("ファイル形式が正しくありません。");
                        }

                        /// 一旦temporaryなフォルダに展開する
                        zip.ExtractExistingFile = ExtractExistingFileAction.OverwriteSilently;
                        zip.ExtractAll(tmpFolderPath);

                        var dirs = Directory.GetDirectories(tmpFolderPath);
                        if (dirs.Length != 1) {
                            throw new Exception("Restored folder must be one");
                        }

                        var restoredDir = Path.Combine(tmpFolderPath, dirs[0]);
                        Directory.Move(restoredDir, root);

                        worker.ReportProgress(90);
                    }


                } catch (Exception e) {
                    if (Directory.Exists(rootBak)) {
                        if (Directory.Exists(root)) {
                            Directory.Delete(root, true);
                        }
                        if (Directory.Exists(rootBak)) {
                            Directory.Move(rootBak, root);
                        }
                    }

                    XmlConfigurator.Configure();
                    Logger.Warn("Restore failed", e);

                    throw;

                } finally {
                    if (Directory.Exists(tmpFolderPath)) {
                        Directory.Delete(tmpFolderPath, true);
                    }
                }

                worker.ReportProgress(100);
            }
        }
#endif
    }
}
