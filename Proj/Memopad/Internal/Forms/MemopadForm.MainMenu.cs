/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mkamo.Editor.Focuses;
using Mkamo.Editor.Core;
using Mkamo.Editor.Requests;
using Mkamo.Memopad.Core;
using Mkamo.Memopad.Internal.Controls;
using Mkamo.Model.Memo;
using Mkamo.Common.Forms.ScreenCapture;
using System.Threading;
using System.Drawing.Printing;
using System.Drawing;
using Mkamo.Control.Progress;
using System.ComponentModel;
using Mkamo.Common.Forms.DetailSettings;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using Mkamo.Memopad.Internal.Helpers;
using Mkamo.Memopad.Internal.Core;
using System.Net.Mail;
using Mkamo.Memopad.Internal.Utils;
using Mkamo.Common.IO;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Net;
using Mkamo.Common.Crypto;
using Mkamo.Common.Core;

namespace Mkamo.Memopad.Internal.Forms {
    partial class MemopadForm {
        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        // --- file menu ---
        private void _fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e) {
            _saveAsToolStripMenuItem.Enabled = _currentEditorCanvas != null;
            _saveAsEmfToolStripMenuItem.Enabled = _currentEditorCanvas != null;
            _saveAsPngToolStripMenuItem.Enabled = _currentEditorCanvas != null;
            _saveAsJpegToolStripMenuItem.Enabled = _currentEditorCanvas != null;
            _saveAsHtmlToolStripMenuItem.Enabled = _currentEditorCanvas != null;
            _saveAsTextToolStripMenuItem.Enabled = _currentEditorCanvas != null;

            _sendToolStripMenuItem.Enabled = _currentEditorCanvas != null;
            _sendMailToolStripMenuItem.Enabled = _currentEditorCanvas != null;

            _printToolStripMenuItem.Enabled = _currentEditorCanvas != null;
        }

        private void _newMemoToolStripMenuItem_Click(object sender, EventArgs e) {
            _app.ShowMainForm();
            _app.ActivateMainForm();
            _app.CreateMemo();
        }

        private void _newFusenMemoToolStripMenuItem_Click(object sender, EventArgs e) {
            _app.CreateMemo(true);
        }

        private void _createMemoFromClipboardMenuToolStripMenuItem_Click(object sender, EventArgs e) {
            _app.ShowMainForm();
            _app.ActivateMainForm();
            _app.CreateMemo();

            if (_currentEditorCanvas != null) {
                MemoEditorHelper.Paste(_currentEditorCanvas.RootEditor.Children.First(), false);
            }
        }

        private void _exitToolStripMenuItem_Click(object sender, EventArgs e) {
            _app.Exit();
        }

        private void _saveAsEmfToolStripMenuItem_Click(object sender, EventArgs e) {
            if (_currentEditorCanvas == null) {
                return;
            }

            using (var dialog = new SaveFileDialog()) {
                dialog.RestoreDirectory = true;
                dialog.ShowHelp = true;
                dialog.Filter = "EMF Files(*.emf)|*.emf";
                if (dialog.ShowDialog(this) == DialogResult.OK) {
                    _currentEditorCanvas.SaveAsEmf(dialog.FileName);
                }
                dialog.Dispose();
            }

        }

        private void _saveAsPngToolStripMenuItem_Click(object sender, EventArgs e) {
            if (_currentEditorCanvas == null) {
                return;
            }

            using (var dialog = new SaveFileDialog()) {
                dialog.RestoreDirectory = true;
                dialog.ShowHelp = true;
                dialog.Filter = "PNG Files(*.png)|*.png";
                if (dialog.ShowDialog(this) == DialogResult.OK) {
                    _currentEditorCanvas.SaveAsPng(dialog.FileName);
                }
            }

        }

        private void _saveAsJpegToolStripMenuItem_Click(object sender, EventArgs e) {
            if (_currentEditorCanvas == null) {
                return;
            }

            using (var dialog = new SaveFileDialog()) {
                dialog.RestoreDirectory = true;
                dialog.ShowHelp = true;
                dialog.Filter = "JPEG Files(*.jpg)|*.jpg";
                if (dialog.ShowDialog(this) == DialogResult.OK) {
                    _currentEditorCanvas.SaveAsJpeg(dialog.FileName);
                }
            }
        }

        private void _saveAsHtmlToolStripMenuItem_Click(object sender, EventArgs e) {
            if (_currentEditorCanvas == null) {
                return;
            }

            using (var dialog = new FolderBrowserDialog()) {
                dialog.Description = "保存先のフォルダを選択してください。";
                dialog.RootFolder = Environment.SpecialFolder.Desktop;
                dialog.ShowNewFolderButton = true;

                if (dialog.ShowDialog(this) == DialogResult.OK) {
                    EnsureFocusCommited();
                    var exporter = new HtmlExporter();
                    exporter.Export(dialog.SelectedPath, _EditorCanvas);
                }
            }
        }

        private void _saveAsTextToolStripMenuItem_Click(object sender, EventArgs e) {
            if (_currentEditorCanvas == null) {
                return;
            }

            using (var dialog = new SaveFileDialog()) {
                dialog.RestoreDirectory = true;
                dialog.ShowHelp = true;
                dialog.Filter = "Text Files(*.txt)|*.txt";
                if (dialog.ShowDialog(this) == DialogResult.OK) {
                    EnsureFocusCommited();
                    var exporter = new PlainTextExporter();
                    exporter.Export(dialog.FileName, _EditorCanvas);
                }
            }
        }

        private void _exportTextToolStripMenuItem_Click(object sender, EventArgs e) {
            using (var dialog = new FolderBrowserDialog()) {
                dialog.Description = "エクスポート先のフォルダを選択してください。";
                dialog.RootFolder = Environment.SpecialFolder.Desktop;
                dialog.ShowNewFolderButton = true;

                if (dialog.ShowDialog(this) == DialogResult.OK) {
                    EnsureFocusCommited();

                    var exporter = new PlainTextExporter();
                    Action<EditorCanvas, Memo, MemoInfo> action = (canvas, memo, info) => {
                        var tags = memo.Tags;
                        if (memo.Tags.Count() == 0) {
                            ExportText(canvas, exporter, dialog.SelectedPath, "未整理", memo.Title);
                        } else {
                            foreach (var tag in tags) {
                                ExportText(canvas, exporter, dialog.SelectedPath, tag.FullName, memo.Title);
                            }
                        }
                    };
                    var proc = new MemoProcessor();
                    proc.Process(this, "テキストのエクスポート", action);
                }
            }
        }

        private void ExportText(EditorCanvas canvas, PlainTextExporter exporter, string rootPath, string tagFullName, string memoTitle) {
            var dirPath = Path.Combine(rootPath, PathUtil.GetValidRelativePath(tagFullName, "_"));
            PathUtil.EnsureDirectoryExists(dirPath);

            var filePath = Path.Combine(dirPath, PathUtil.GetValidFilename(memoTitle, "_") + ".txt");
            filePath = Path.GetFullPath(filePath);
            filePath = PathUtil.GetUniqueFilePathByCounting(filePath);
            exporter.Export(filePath, canvas);
        }

        private void _exportHtmlToolStripMenuItem_Click(object sender, EventArgs e) {
            using (var dialog = new FolderBrowserDialog()) {
                dialog.Description = "エクスポート先のフォルダを選択してください。";
                dialog.RootFolder = Environment.SpecialFolder.Desktop;
                dialog.ShowNewFolderButton = true;

                if (dialog.ShowDialog(this) == DialogResult.OK) {
                    EnsureFocusCommited();

                    var exporter = new HtmlExporter();
                    Action<EditorCanvas, Memo, MemoInfo> action = (canvas, memo, info) => {
                        var tags = memo.Tags;
                        if (memo.Tags.Count() == 0) {
                            ExportHtml(canvas, exporter, dialog.SelectedPath, "未整理", memo.Title);

                        } else {
                            foreach (var tag in tags) {
                                ExportHtml(canvas, exporter, dialog.SelectedPath, tag.FullName, memo.Title);
                            }
                        }
                    };
                    var proc = new MemoProcessor();
                    proc.Process(this, "HTMLのエクスポート", action);
                }
            }
        }

        private void ExportHtml(EditorCanvas canvas, HtmlExporter exporter, string rootPath, string tagFullName, string memoTitle) {
            var dirPath = Path.Combine(rootPath, PathUtil.GetValidRelativePath(tagFullName, "_"));
            dirPath = Path.Combine(dirPath, PathUtil.GetValidFilename(memoTitle, "_"));
            dirPath = Path.GetFullPath(dirPath);
            dirPath = PathUtil.GetUniqueDirectoryPathByCounting(dirPath);
            PathUtil.EnsureDirectoryExists(dirPath);
            exporter.Export(dirPath, canvas);
        }

        //private void _saveSelectedAsEmfToolStripMenuItem_Click(object sender, EventArgs e) {
        //    if (_currentEditorCanvas == null) {
        //        return;
        //    }

        //    var dialog = new SaveFileDialog();
        //    dialog.RestoreDirectory = true;
        //    dialog.ShowHelp = true;
        //    dialog.Filter = "EMF Files(*.emf)|*.emf";
        //    if (dialog.ShowDialog(this) == DialogResult.OK) {
        //        _currentEditorCanvas.SaveSelectedAsEmf(dialog.FileName);
        //    }
        //    dialog.Dispose();

        //}

        //private void _saveSelectedAsPngToolStripMenuItem_Click(object sender, EventArgs e) {
        //    if (_currentEditorCanvas == null) {
        //        return;
        //    }

        //    var dialog = new SaveFileDialog();
        //    dialog.RestoreDirectory = true;
        //    dialog.ShowHelp = true;
        //    dialog.Filter = "PNG Files(*.png)|*.png";
        //    if (dialog.ShowDialog(this) == DialogResult.OK) {
        //        _currentEditorCanvas.SaveSelectedAsPng(dialog.FileName);
        //    }
        //    dialog.Dispose();

        //}

        private void _sendMailToolStripMenuItem_Click(object sender, EventArgs e) {
            try {
                using (var dialog = new SendMailForm()) {
                    dialog.Font = _theme.CaptionFont;

                    dialog.Subject = _PageContent.MemoInfo.Title;
                    dialog.From = _app.WindowSettings.MailFrom;
                    dialog.To = _app.WindowSettings.MailTo;

                    dialog.SmtpServer = _app.WindowSettings.SmtpServer;
                    dialog.SmtpPort = _app.WindowSettings.SmtpPort;
                    dialog.SmtpEnableAuth = _app.WindowSettings.SmtpEnableAuth;
                    dialog.SmtpUserName = _app.WindowSettings.SmtpUserName;
                    dialog.SmtpPassword = CryptoUtil.DecryptSmtpPassword(_app.WindowSettings.SmtpPassword);
                    dialog.SmtpEnableSsl = _app.WindowSettings.SmtpEnableSsl;

                    if (dialog.ShowDialog(this) == DialogResult.OK) {

                        var client = new SmtpClient(dialog.SmtpServer, dialog.SmtpPort);
                        if (dialog.SmtpEnableAuth) {
                            client.UseDefaultCredentials = false;
                            client.Credentials = new NetworkCredential(dialog.SmtpUserName, dialog.SmtpPassword);
                        } else {
                            client.UseDefaultCredentials = true;
                        }
                        client.EnableSsl = dialog.SmtpEnableSsl;
                        client.Timeout = 100000;
                        client.SendCompleted += HandleSmtpClientSendComplete;

                        var msg = new MailMessage(dialog.From, dialog.To); /// HandleSmtpClientSendCompleteでDispose()される
                        msg.BodyEncoding = Encoding.UTF8;
                        msg.Subject = dialog.Subject;
                        switch (dialog.Body) {
                            case SendMailForm.BodyKind.Text:
                                msg.Body = MemoTextUtil.GetText(_currentEditorCanvas, null, false);
                                client.SendAsync(msg, msg);
                                break;
                            case SendMailForm.BodyKind.Image:
                                using (var img = _currentEditorCanvas.CreateBitmap(1f, Size.Empty)) {
                                    var conv = new ImageConverter();
                                    var bytes = (byte[]) conv.ConvertTo(img, typeof(byte[]));
                                    var mem = new MemoryStream(bytes);
                                    var args = new DisposableBundle(new IDisposable[] { msg, mem, });
                                    msg.Attachments.Add(new Attachment(mem, "note.png"));
                                    client.SendAsync(msg, args);
                                }
                                break;
                            case SendMailForm.BodyKind.TextAndImage:
                                msg.Body = MemoTextUtil.GetText(_currentEditorCanvas, null, false);
                                using (var img = _currentEditorCanvas.CreateBitmap(1f, Size.Empty)) {
                                    var conv = new ImageConverter();
                                    var bytes = (byte[]) conv.ConvertTo(img, typeof(byte[]));
                                    var mem = new MemoryStream(bytes);
                                    var args = new DisposableBundle(new IDisposable[] { msg, mem, });
                                    msg.Attachments.Add(new Attachment(mem, "note.png"));
                                    client.SendAsync(msg, args);
                                }
                                break;
                        }

                        _app.WindowSettings.MailFrom = dialog.From;
                        _app.WindowSettings.MailTo = dialog.To;

                        _app.WindowSettings.SmtpServer = dialog.SmtpServer;
                        _app.WindowSettings.SmtpPort = dialog.SmtpPort;
                        _app.WindowSettings.SmtpEnableAuth = dialog.SmtpEnableAuth;
                        _app.WindowSettings.SmtpUserName = dialog.SmtpUserName;
                        _app.WindowSettings.SmtpPassword = CryptoUtil.EncryptSmtpPassword(dialog.SmtpPassword);
                        _app.WindowSettings.SmtpEnableSsl = dialog.SmtpEnableSsl;
                    }
                }
            } catch (Exception ex) {
                Logger.Warn("Can't send mail.", ex);
                MessageBox.Show(
                    this,
                    "メールの送信に失敗しました。" + Environment.NewLine + ex.Message,
                    "メールの送信エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void HandleSmtpClientSendComplete(object sender, AsyncCompletedEventArgs e) {
            if (e.Cancelled) {
                ShowStatusMessage("メールの送信をキャンセルしました。");
            } else if (e.Error != null) {
                MessageBox.Show(this, "メールの送信に失敗しました。" + Environment.NewLine + e.Error.ToString(), "メール送信エラー");
            } else {
                ShowStatusMessage("メールの送信を完了しました。");
            }

            var disp = e.UserState as IDisposable;
            if (disp != null) {
                disp.Dispose();
            }
        }

        private void _printToolStripMenuItem_Click(object sender, EventArgs e) {
            if (_currentEditorCanvas == null) {
                return;
            }

            using (var dialog = new PrintForm(this, _CurrentEditorCanvas)) {
                dialog.Font = _theme.CaptionFont;
                dialog.ShowDialog(this);
            }
        }

        //private void _backupToolStripMenuItem_Click(object sender, EventArgs e) {
        //    using (var dialog = new SaveFileDialog()) {
        //        dialog.RestoreDirectory = true;
        //        dialog.ShowHelp = true;
        //        dialog.Filter = "Confidante Backup File(*.cnfbak)|*.cnfbak";
        //        if (dialog.ShowDialog(this) == DialogResult.OK) {
        //            EnsureFocusCommited();
        //            var exporter = new Exporter();
        //            exporter.BackupWithUI(dialog.FileName);
        //        }
        //    }
        //}


        //private void _restoreToolStripMenuItem_Click(object sender, EventArgs e) {
        //    var result = MessageBox.Show(
        //        this,
        //        "復元すると現在のノートはすべて消去されます。実行してもよろしいですか。",
        //        "復元の確認",
        //        MessageBoxButtons.YesNo
        //    );
        //    if (result == DialogResult.No) {
        //        return;
        //    }

        //    using (var dialog = new OpenFileDialog()) {
        //        dialog.RestoreDirectory = true;
        //        dialog.ShowHelp = true;
        //        dialog.Filter = "Confidante Backup File(*.cnfbak)|*.cnfbak";
        //        if (dialog.ShowDialog(this) == DialogResult.OK) {
        //            EnsureFocusCommited();
        //            var exporter = new Exporter();
        //            exporter.RestoreWithUI(dialog.FileName);
        //        }
        //    }
        //}

        private void _exportToFolderToolStripMenuItem_Click(object sender, EventArgs e) {
            using (var dialog = new FolderBrowserDialog()) {
                dialog.Description = "エクスポート先のフォルダを選択してください。";
                dialog.RootFolder = Environment.SpecialFolder.Desktop;
                dialog.ShowNewFolderButton = true;

                var lastDir = _app.BootstrapSettings.LastExportDirectory;
                if (Directory.Exists(lastDir)) {
                    dialog.SelectedPath = lastDir;
                }

                if (dialog.ShowDialog(this) == DialogResult.OK) {
                    EnsureFocusCommited();
                    var exporter = new DataFolderExporter();
                    lock (MemopadConsts.DataLock) {
                        exporter.ExportTo(dialog.SelectedPath);
                    }
                }
            }
        }

        private void _importFromFolderToolStripMenuItem_Click(object sender, EventArgs e) {
            using (var dialog = new FolderBrowserDialog()) {
                dialog.Description = "インポート元のフォルダを選択してください。";
                dialog.RootFolder = Environment.SpecialFolder.Desktop;
                dialog.ShowNewFolderButton = true;

                var lastDir = _app.BootstrapSettings.LastImportDirectory;
                if (Directory.Exists(lastDir)) {
                    dialog.SelectedPath = lastDir;
                }

                if (dialog.ShowDialog(this) == DialogResult.OK) {
                    EnsureFocusCommited();
                    var exporter = new DataFolderExporter();
                    lock (MemopadConsts.DataLock) {
                        exporter.ImportFrom(dialog.SelectedPath);
                    }
                }
            }
        }

        // --- edit menu ---
        private void _editToolStripMenuItem_DropDownOpening(object sender, EventArgs e) {
            var isCanvasEnabled = _currentEditorCanvas != null && _currentEditorCanvas.Enabled;

            var isEditorFocused =
                isCanvasEnabled &&
                _currentEditorCanvas.FocusManager.IsEditorFocused;

            var focus =
                _currentEditorCanvas == null?
                    null:
                    _currentEditorCanvas.FocusManager.Focus as StyledTextFocus;

            isEditorFocused = isEditorFocused && (focus != null); /// focusされていてそれがStyledTextFocusである

            _undoToolStripMenuItem.Enabled = isCanvasEnabled && _currentEditorCanvas.CanUndo();
            _redoToolStripMenuItem.Enabled = isCanvasEnabled && _currentEditorCanvas.CanRedo();

            _cutToolStripMenuItem.Enabled =
                isCanvasEnabled &&
                ( 
                    (isEditorFocused && focus.CanCut) ||
                    (!isEditorFocused && _currentEditorCanvas.CanCut())
                );
            _copyToolStripMenuItem.Enabled =
                isCanvasEnabled &&
                ( 
                    (isEditorFocused && focus.CanCopy) ||
                    (!isEditorFocused && _currentEditorCanvas.CanCopy())
                );
            _pasteToolStripMenuItem.Enabled =
                isCanvasEnabled &&
                ( 
                    (isEditorFocused && focus.CanPaste) |
                    (!isEditorFocused && _currentEditorCanvas.CanPaste())
                );

            _deleteToolStripMenuItem.Enabled =
                isCanvasEnabled &&
                (
                    isEditorFocused ||
                    (!isEditorFocused && _currentEditorCanvas.CanDelete())
                );

            _selectAllToolStripMenuItem.Enabled = isCanvasEnabled;
            _findToolStripMenuItem.Enabled = isCanvasEnabled;
        }

        private void _undoToolStripMenuItem_Click(object sender, EventArgs e) {
            _UILogic.DoUndo(_currentEditorCanvas);
        }

        private void _redoToolStripMenuItem_Click(object sender, EventArgs e) {
            _UILogic.DoRedo(_currentEditorCanvas);
        }

        private void _selectAllToolStripMenuItem_Click(object sender, EventArgs e) {
            _UILogic.DoSelectAll(_currentEditorCanvas);
        }

        private void _cutToolStripMenuItem_Click(object sender, EventArgs e) {
            _UILogic.DoCut(_currentEditorCanvas);
        }

        private void _copyToolStripMenuItem_Click(object sender, EventArgs e) {
            _UILogic.DoCopy(_currentEditorCanvas);
        }

        private void _pasteToolStripMenuItem_Click(object sender, EventArgs e) {
            _UILogic.DoPaste(_currentEditorCanvas);
        }

        private void _deleteToolStripMenuItem_Click(object sender, EventArgs e) {
            _UILogic.DoDelete(_currentEditorCanvas);
        }

        private void _findToolStripMenuItem_Click(object sender, EventArgs e) {
            _UILogic.DoSearchInMemo(_currentEditorCanvas, CurrentPageContent);
        }

        // --- view menu ---
        private void _viewToolStripMenuItem_DropDownOpening(object sender, EventArgs e) {
            _focusConditionTextBoxToolStripMenuItem.Enabled = !_IsCompact;
            _showWorkspaceToolStripMenuItem.Enabled = !_IsCompact;
            _showMemoListToolStripMenuItem.Enabled = !_IsCompact;

            _compactModeToolStripMenuItem.Checked = _IsCompact;
            _displayTopMostToolStripMenuItem.Checked = TopMost;
        }

        private void _focusConditionTextBoxToolStripMenuItem_Click(object sender, EventArgs e) {
            FocusConditionTextBox();
        }

        private void _showWorkspaceToolStripMenuItem_Click(object sender, EventArgs e) {
            if (IsFinderPaneCollapsed) {
                ExpandFinderPane();
            }
            ShowWorkspaceView();
            _workspaceView.Focus();
        }

        private void _showMemoListToolStripMenuItem_Click(object sender, EventArgs e) {
            if (IsMemoListPaneCollapsed) {
                ExpandMemoListPane();
            }
            _memoListView.Focus();
        }
        
        private void _showStartPageToolStripMenuItem_Click(object sender, EventArgs e) {
            ShowStartPage();
        }

        private void _showFusenMemosToolStripMenuItem_Click(object sender, EventArgs e) {
            _app.ShowFusenForms(false);
        }

        private void _compactModeToolStripMenuItem_Click(object sender, EventArgs e) {
            SetCompact(!_IsCompact);
        }

        private void _displayTopMostToolStripMenuItem_Click(object sender, EventArgs e) {
            TopMost = !TopMost;
        }


        // --- recent menu ---
        private void _recentToolStripMenuItem_DropDownOpening(object sender, EventArgs e) {
            _recentToolStripMenuItem.DropDownItems.Clear();

            var created = new ToolStripMenuItem("最近作成したノート(&C)");
            var count = 1;
            foreach (var recent in _app.RecentlyCreatedMemoInfos.Reverse().ToArray()) {
                var item = new ToolStripMenuItem(recent.Title + "(&" + count % 10 + ")");
                item.Tag = recent;
                item.Click += HandleRecentToolStripMenuItemClick;
                created.DropDownItems.Add(item);
                ++count;
            }
            created.Enabled = created.DropDownItems.Count > 0;
            _recentToolStripMenuItem.DropDownItems.Add(created);

            var modified = new ToolStripMenuItem("最近更新したノート(&M)");
            count = 1;
            foreach (var recent in _app.RecentlyModifiedMemoInfos.Reverse().ToArray()) {
                var item = new ToolStripMenuItem(recent.Title + "(&" + count % 10 + ")");
                item.Tag = recent;
                item.Click += HandleRecentToolStripMenuItemClick;
                modified.DropDownItems.Add(item);
                ++count;
            }
            modified.Enabled = modified.DropDownItems.Count > 0;
            _recentToolStripMenuItem.DropDownItems.Add(modified);

            _recentToolStripMenuItem.DropDownItems.Add(new ToolStripSeparator());

            count = 1;
            foreach (var recent in _app.RecentlyClosedMemoInfos.Reverse().ToArray()) {
                var item = new ToolStripMenuItem(recent.Title + "(&" + count % 10 + ")");
                item.Tag = recent;
                item.Click += HandleRecentToolStripMenuItemClick;
                _recentToolStripMenuItem.DropDownItems.Add(item);
                ++count;
            }
        }

        private void HandleRecentToolStripMenuItemClick(object sender, EventArgs e) {
            var item = (ToolStripMenuItem) sender;
            var info = (MemoInfo) item.Tag;
            _app.LoadMemo(info);
        }

        // --- tool menu ---
        private void _manageTagToolStripMenuItem_Click(object sender, EventArgs e) {
            using (var form = new TagManageForm()) {
                form.Font = _theme.CaptionFont;
                form.ShowDialog(this);
            }
        }

        private void _manageSmartFilterToolStripMenuItem_Click(object sender, EventArgs e) {
            using (var form = new SmartFilterManageForm()) {
                form.Font = _theme.CaptionFont;
                form.ShowDialog(this);
            }
        }

        private void _optionToolStripMenuItem_Click(object sender, EventArgs e) {
            var form = new DetailSettingsForm();
            try {
                form.Text = "オプション";
                form.Size = new Size(500, 400);
                form.Theme = _theme;

                var basic = new BasicSettingsDetailPage(
                    _app.Settings, _app.WindowSettings, _app.MainForm.ToolRegistry
                );
                form.RegisterPage("基本", basic);

                var background = new BackgroundSettingsDetailPage(_app.Settings, _app.WindowSettings);
                form.RegisterPage("背景", background);

                var confirm = new ConfirmSettingsDetailPage(_app.Settings);
                form.RegisterPage("確認", confirm);

                var folder = new FolderSettingsDetailPage(_app.BootstrapSettings, _app.WindowSettings);
                form.RegisterPage("フォルダ", folder);

                var hotkey = new HotKeySettingsDetailPage(_app.HotKey, _app.WindowSettings);
                form.RegisterPage("ホットキー", hotkey);

                var abbrev = new AbbrevSettingDetailPage(_app._AbbrevWordPersister);
                form.RegisterPage("単語補完", abbrev);

                var misc = new MiscSettingsDetailPage(_app.Settings, _app.WindowSettings);
                form.RegisterPage("その他", misc);

                if (form.ShowDialog(this) == DialogResult.OK) {
                    var cmd = form.GetUpdateCommand();
                    if (cmd != null) {
                        cmd.Execute();
                        MessageBox.Show(this, "一部の設定はアプリケーションを再起動するまで反映されません。", "設定の変更");
                    }
                }
            } finally {
                form.Dispose();
            }
        }

        // --- help menu ---
        private void _aboutBoxToolStripMenuItem_Click(object sender, EventArgs e) {
            using (var aboutBox = new AboutBox()) {
                aboutBox.Font = Font;
                aboutBox.ShowDialog(this);
            }
        }

        private void _showHelpToolStripMenuItem_Click(object sender, EventArgs e) {
            var dir = Path.GetDirectoryName(Application.ExecutablePath);
            var helpDir = Path.Combine(dir, "Help");
            var helpPath = Path.Combine(helpDir, @"Introduction\welcome.html");
            if (File.Exists(helpPath)) {
                Process.Start(helpPath);
            } else {
                MessageBox.Show(this, "ヘルプが見つかりませんでした。", "ヘルプ表示エラー");
            }
        }

        private void _checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e) {
            // todo: show dialog
            // _app.CheckForUpdates();
        }

    }
}
