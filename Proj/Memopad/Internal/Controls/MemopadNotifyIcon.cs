/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Common.Win32.User32;
using System.Threading;
using Mkamo.Memopad.Core;

namespace Mkamo.Memopad.Internal.Controls {
    internal class MemopadNotifyIcon: IDisposable {
        // ========================================
        // field
        // ========================================
        private MemopadApplication _app;
        private NotifyIcon _notifyIcon;
        private ContextMenuStrip _notifyIconContextMenu;

        // ========================================
        // constructor
        // ========================================
        public MemopadNotifyIcon() {
            _app = MemopadApplication.Instance;

            _notifyIcon = new NotifyIcon();
            _notifyIcon.Text = "MochaNote";
            _notifyIcon.Icon = Properties.Resources.confidante;
            _notifyIcon.Visible = true;

            _notifyIconContextMenu = CreateNotifyIconContextMenu();
            _notifyIcon.ContextMenuStrip = _notifyIconContextMenu;
            _notifyIcon.MouseClick += HandleNotifyIconMouseClick;
            _notifyIcon.MouseDoubleClick += HandleNotifyIconMouseDoubleClick;
        }

        // ========================================
        // destructor
        // ========================================
        public void Dispose() {
            if (_notifyIcon != null) {
                _notifyIcon.Dispose();
            }
            if (_notifyIconContextMenu != null) {
                _notifyIconContextMenu.Dispose();
            }
        }

        // ========================================
        // method
        // ========================================
        private ContextMenuStrip CreateNotifyIconContextMenu() {
            var items = new List<ToolStripItem>(16);

            var newMemo = new ToolStripMenuItem();
            newMemo.Text = "ノートを作成(&N)";
            newMemo.Click += (sender, e) => {
                CreateMemo();
            };
            items.Add(newMemo);

            var newFusenMemo = new ToolStripMenuItem();
            newFusenMemo.Text = "付箋ノートを作成(&F)";
            newFusenMemo.Click += (sender, e) => {
                CreateFusenMemo();
            };
            items.Add(newFusenMemo);

            var clipMemo = new ToolStripMenuItem();
            clipMemo.Text = "ノートにクリップ(&C)";
            clipMemo.Click += (sender, e) => {
                var mainForm = _app.MainForm;
                var hWnd = User32PI.GetWindow(
                    mainForm.Handle,
                    GetWindowCmd.GW_HWNDFIRST
                );
                hWnd = User32Util.GetNextWindow(
                    hWnd,
                    handle => {
                        if (User32PI.IsWindowVisible(handle) && !User32Util.IsOwnedWindow(handle)) {
                            var cname = User32Util.GetWindowClassName(handle);
                            return
                                !string.Equals(cname, "Shell_TrayWnd", StringComparison.OrdinalIgnoreCase) &&
                                !string.Equals(cname, "NotifyIconOverflowWindow", StringComparison.OrdinalIgnoreCase);

                        } else {
                            return false;
                        }
                    }
                );

                var oldTopMost = mainForm.TopMost;
                mainForm.TopMost = false;
                if (hWnd != IntPtr.Zero) {
                    User32Util.ActivateWindow(hWnd);
                    Thread.Sleep(1000);
                }
                _app.ClipAndCreateMemo();
                mainForm.TopMost = oldTopMost;
            };
            items.Add(clipMemo);

            var capture = new ToolStripMenuItem();
            capture.Text = "画面を取り込み(&D)";
            capture.Click += (sender, e) => {
                Thread.Sleep(500);
                _app.CaptureAndCreateMemo();
            };
            items.Add(capture);

            items.Add(new ToolStripSeparator());

            var searchMemo = new ToolStripMenuItem();
            searchMemo.Text = "ノートを検索(&S)";
            searchMemo.Click += (sender, e) => {
                _app.ShowMainForm();
                _app.ActivateMainForm();
                var form = _app.MainForm;
                if (form != null) {
                    form.FocusConditionTextBox();
                }
            };
            items.Add(searchMemo);

            var activate = new ToolStripMenuItem();
            activate.Text = "アクティブにする(&A)";
            activate.Click += (sender, e) => {
                ShowMainForm();
            };
            items.Add(activate);

            var showAllFusens = new ToolStripMenuItem();
            showAllFusens.Text = "付箋ノートを表示(&F)";
            showAllFusens.Click += (sender, e) => {
                ShowFusenForms(false);
            };
            items.Add(showAllFusens);

            items.Add(new ToolStripSeparator());

            var action = new ToolStripMenuItem();
            action.Text = "アイコンの動作(&A)";
            items.Add(action);

            {
                var click = new ToolStripMenuItem();
                click.Text = "クリック(&C)";
                action.DropDown.Items.Add(click);
                InitClickSetting(click);
            }
            {
                var dclick = new ToolStripMenuItem();
                dclick.Text = "ダブルクリック(&D)";
                action.DropDown.Items.Add(dclick);

                InitDoubleClickSetting(dclick);
            }

            items.Add(new ToolStripSeparator());

            var exitItem = new ToolStripMenuItem();
            exitItem.Text = "終了(&X)";
            exitItem.Click += (sender, e) => {
                _app.Exit();
            };
            items.Add(exitItem);
        
            var ret = new ContextMenuStrip();
            ret.SuspendLayout();
            ret.Items.AddRange(items.ToArray());
            ret.ResumeLayout();
            return ret;
        }

        private void InitClickSetting(ToolStripMenuItem item) {
            item.DropDown.SuspendLayout();

            var createMemo = new ToolStripMenuItem("ノートを作成(&N)");
            createMemo.Click += (se, ev) => {
                _app.Settings.NotifyIconClickAction = NotifyIconActionKind.CreateMemo;
            };
            item.DropDown.Items.Add(createMemo);

            var createFusen = new ToolStripMenuItem("付箋ノートを作成(&F)");
            createFusen.Click += (se, ev) => {
                _app.Settings.NotifyIconClickAction = NotifyIconActionKind.CreateFusenMemo;
            };
            item.DropDown.Items.Add(createFusen);

            var activate = new ToolStripMenuItem("アクティブにする(&A)");
            activate.Click += (se, ev) => {
                _app.Settings.NotifyIconClickAction = NotifyIconActionKind.ShowMainForm;
            };
            item.DropDown.Items.Add(activate);

            var showFusenForms = new ToolStripMenuItem("付箋ノートを表示(&F)");
            showFusenForms.Click += (se, ev) => {
                _app.Settings.NotifyIconClickAction = NotifyIconActionKind.ShowFusenForms;
            };
            item.DropDown.Items.Add(showFusenForms);

            item.DropDownOpening += (se, ev) => {
                var kind = _app.Settings.NotifyIconClickAction;
                createMemo.Checked = kind == NotifyIconActionKind.CreateMemo;
                createFusen.Checked = kind == NotifyIconActionKind.CreateFusenMemo;
                activate.Checked = kind == NotifyIconActionKind.ShowMainForm;
                showFusenForms.Checked = kind == NotifyIconActionKind.ShowFusenForms;
            };

            item.DropDown.ResumeLayout();
        }

        private void InitDoubleClickSetting(ToolStripMenuItem item) {
            item.DropDown.SuspendLayout();

            var createMemo = new ToolStripMenuItem("ノートを作成(&N)");
            createMemo.Click += (se, ev) => {
                _app.Settings.NotifyIconDoubleClickAction = NotifyIconActionKind.CreateMemo;
            };
            item.DropDown.Items.Add(createMemo);

            var createFusen = new ToolStripMenuItem("付箋ノートを作成(&F)");
            createFusen.Click += (se, ev) => {
                _app.Settings.NotifyIconDoubleClickAction = NotifyIconActionKind.CreateFusenMemo;
            };
            item.DropDown.Items.Add(createFusen);

            var activate = new ToolStripMenuItem("アクティブにする(&A)");
            activate.Click += (se, ev) => {
                _app.Settings.NotifyIconDoubleClickAction = NotifyIconActionKind.ShowMainForm;
            };
            item.DropDown.Items.Add(activate);

            var showFusenForms = new ToolStripMenuItem("付箋ノートを表示(&F)");
            showFusenForms.Click += (se, ev) => {
                _app.Settings.NotifyIconDoubleClickAction = NotifyIconActionKind.ShowFusenForms;
            };
            item.DropDown.Items.Add(showFusenForms);

            item.DropDownOpening += (se, ev) => {
                var kind = _app.Settings.NotifyIconDoubleClickAction;
                createMemo.Checked = kind == NotifyIconActionKind.CreateMemo;
                createFusen.Checked = kind == NotifyIconActionKind.CreateFusenMemo;
                activate.Checked = kind == NotifyIconActionKind.ShowMainForm;
                showFusenForms.Checked = kind == NotifyIconActionKind.ShowFusenForms;
            };

            item.DropDown.ResumeLayout();
        }

        // --- handler ---
        private void HandleNotifyIconMouseClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                DoAction(_app.Settings.NotifyIconClickAction);
            }
        }

        private void HandleNotifyIconMouseDoubleClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                DoAction(_app.Settings.NotifyIconDoubleClickAction);
            }
        }

        private void DoAction(NotifyIconActionKind kind) {
            switch (kind) {
                case NotifyIconActionKind.ShowMainForm:
                    ShowMainForm();
                    break;
                case NotifyIconActionKind.ShowFusenForms:
                    ShowFusenForms(true);
                    break;
                case NotifyIconActionKind.CreateMemo:
                    CreateMemo();
                    break;
                case NotifyIconActionKind.CreateFusenMemo:
                    CreateFusenMemo();
                    break;
            }
        }

        private void CreateMemo() {
            _app.ShowMainForm();
            _app.ActivateMainForm();
            _app.CreateMemo();
        }

        private void CreateFusenMemo() {
            _app.CreateMemo(true);
        }

        private void ShowMainForm() {
            _app.ShowMainForm();
            _app.ActivateMainForm();
        }

        private void ShowFusenForms(bool useDummy) {
            _app.ShowFusenForms(useDummy);
        }
    }
}
