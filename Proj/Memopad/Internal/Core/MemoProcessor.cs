/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Model.Memo;
using Mkamo.Memopad.Internal.Utils;
using Mkamo.Editor.Core;
using System.Windows.Forms;
using Mkamo.Control.Progress;
using System.ComponentModel;
using Mkamo.Memopad.Core;

namespace Mkamo.Memopad.Internal.Core {
    internal class MemoProcessor {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // ========================================
        // method
        // ========================================
        public void Process(Form owner, string caption, Action<EditorCanvas, Memo, MemoInfo> action) {
            if (action == null) {
                return;
            }

            var app = MemopadApplication.Instance;
            var dialog = new ProgressDialog();
            dialog.Text = caption;
            dialog.SupportCancel = true;
            dialog.Font = app.Theme.CaptionFont;

            dialog.BackgroundWorker.DoWork += DoProccessAsync;
            dialog.BackgroundWorker.RunWorkerCompleted += (sender, e) => {
                if (e.Error != null) {
                    MessageBox.Show(owner, "処理に失敗しました。", "処理エラー");
                } else if (e.Cancelled) {
                    MessageBox.Show(owner, "処理をキャンセルしました。", "処理のキャンセル");
                } else {
                    MessageBox.Show(owner, "処理を完了しました。", "処理の完了");
                }

                dialog.Close();
                dialog.Dispose();
            };
            dialog.Run(owner, action);
        }

        private void DoProccessAsync(object sender, DoWorkEventArgs dwe) {
            var app = MemopadApplication.Instance;
            lock (app._Lock) {
                var worker = (BackgroundWorker) sender;

                try {
                    var action = (Action<EditorCanvas, Memo, MemoInfo>) dwe.Argument;

                    var infos = app.MemoInfos;
                    var count = app.MemoInfoCount;
                    var i = 0;
                    foreach (var info in infos) {
                        if (worker.CancellationPending) {
                            dwe.Cancel = true;
                            break;
                        }

                        var memo = app.Container.Find<Memo>(info.MemoId);
                        if (memo == null) {
                            continue;
                        }

                        Action a = () => {
                            using (var canvas = new EditorCanvas()) {
                                Application.DoEvents();
                                MemoSerializeUtil.LoadEditor(canvas, info.MementoId);
                                Application.DoEvents();
                                action(canvas, memo, info);
                                Application.DoEvents();
                            }
                        };
                        app.MainForm.Invoke(a);

                        worker.ReportProgress(i * 100 / count);
                        ++i;
                    }
                } catch (Exception e) {
                    Logger.Warn("Export failed", e);
                    throw;
                }
            }
        }
    }
}
