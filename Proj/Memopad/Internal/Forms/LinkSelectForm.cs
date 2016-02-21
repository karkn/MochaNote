/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Memopad.Core;
using Mkamo.Memopad.Internal.Utils;
using Mkamo.Common.String;
using Mkamo.Common.Core;
using Mkamo.Common.Forms.Utils;

namespace Mkamo.Memopad.Internal.Forms {
    internal partial class LinkSelectForm: Form {
        // ========================================
        // field
        // ========================================
        private MemopadApplication _facade;

        private bool _inited;
        private string _defaultUri;

        // ========================================
        // constructor
        // ========================================
        public LinkSelectForm() {
            _facade = MemopadApplication.Instance;

            InitializeComponent();

            _workspaceView.PanelPadding = new Padding(0);

            var theme = _facade.Theme;
            _workspaceView.Theme = theme;
            _memoListView.Theme = theme;

            Font = theme.CaptionFont;
            _okButton.Font = theme.CaptionFont;
            _cancelButton.Font = theme.CaptionFont;
            _linkTargetLabel.Font = theme.CaptionFont;
            _titleTextLabel.Font = theme.CaptionFont;
            _addressLabel.Font = theme.CaptionFont;

            _linkTargetListBox.Font = theme.InputFont;
            _titleTextTextBox.Font = theme.InputFont;
            _addressTextBox.Font = theme.InputFont;

            _inited = false;


            _memoLinkPanel.Paint += (s, e) => {
                ControlPaintUtil.DrawBorder(
                    e.Graphics,
                    _workspaceView.Bounds,
                    SystemColors.ControlDark,
                    SystemColors.Window,
                    1
                );
                ControlPaintUtil.DrawBorder(
                    e.Graphics,
                    _memoListView.Bounds,
                    SystemColors.ControlDark,
                    SystemColors.Window,
                    1
                );
            };
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            _workspaceView.WorkspaceTree.ShowAllMemos = true;
            _workspaceView.WorkspaceTree.ShowTrashBox = false;
            _workspaceView.WorkspaceTree.AllowEdit = false;
            _workspaceView.ContextMenusEnabled = false;
            _memoListView.MemoListBox.SelectionMode = SelectionMode.One;
            _memoListView.MemoListBox.DisplayItems = new[] { MemoListBoxDisplayItem.Title };

            _workspaceView.WorkspaceTree.AfterSelect += HandleWorkspaceTreeAfterSelect;
            _memoListView.MemoListBox.SelectedIndexChanged += HandleMemoListBoxSelectedIndex;

            _workspaceView.WorkspaceTree.RebuildTree();

            _memoLinkPanel.Visible = true;
            _webLinkPanel.Visible = false;

            _okButton.Enabled = false;

            Uri = _defaultUri;
        }

        // ========================================
        // property
        // ========================================
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Uri {
            get {
                if (_linkTargetListBox.SelectedIndex == 0) {
                    var info = _memoListView.MemoListBox.SelectedItem as MemoInfo;
                    if (info == null) {
                        return null;
                    } else {
                        return "memo:///" + info.MemoId;
                    }
                } else if (_linkTargetListBox.SelectedIndex == 1) {
                    return _addressTextBox.Text;
                }
                return null;
            }
            set {
                if (UriUtil.IsHttpUri(value) || UriUtil.IsFileUri(value)) {
                    _linkTargetListBox.SelectedIndex = 1;
                    _addressTextBox.Text = value;

                } else {
                    _linkTargetListBox.SelectedIndex = 0;
                    if (value == null) {
                        return;
                    }
                    var info = UriUtil.GetMemoInfo(value);
                    if (info == null) {
                        return;
                    }
                    _workspaceView.WorkspaceTree.SelectedNode = _workspaceView.WorkspaceTree.AllMemosNode;
                    UpdateMemoListBox();
                    var listBox = _memoListView.MemoListBox;
                    var index = listBox.Items.IndexOf(info);
                    if (index > -1) {
                        _memoListView.MemoListBox.SetSelected(index, true);
                    }
                }
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string TitleText {
            get { return _titleTextTextBox.Text; }
            set { _titleTextTextBox.Text = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool TitleTextTextBoxEnabled {
            get { return _titleTextTextBox.Enabled; }
            set { _titleTextTextBox.Enabled = value; }
        }

        // ========================================
        // method
        // ========================================
        public DialogResult ShowDialog(Form owner, string defaultUri, string title) {
            _defaultUri = defaultUri;
            TitleText = title;
            return ShowDialog(owner);
        }

        public void UpdateMemoListBox() {
            var oldCursor = Cursor.Current;
            try {
                Cursor.Current = Cursors.WaitCursor;

                var provider = _workspaceView.WorkspaceTree.CurrentMemoInfoListProvider;
                if (provider == null) {
                    _memoListView.MemoListBox.UpdateList(new MemoInfo[0], false, false);
                } else {
                    if (!_workspaceView.WorkspaceTree.IsInDragOver) {
                        _memoListView.MemoListBox.UpdateList(provider.MemoInfos, false, false);
                    }
                }

            } finally {
                Cursor.Current = oldCursor;
            }
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);

            ControlPaintUtil.DrawBorder(
                e.Graphics,
                _linkTargetListBox.Bounds,
                SystemColors.ControlDark,
                SystemColors.Window,
                1
            );
        }

        // ------------------------------
        // private
        // ------------------------------
        private void UpdateOkButtonEnabled() {
            if (StringUtil.IsNullOrWhitespace(_titleTextTextBox.Text)) {
                _okButton.Enabled = false;
            } else if (_linkTargetListBox.SelectedIndex == 0) {
                _okButton.Enabled = _memoListView.MemoListBox.SelectedItems.Count > 0;
            } else if (_linkTargetListBox.SelectedIndex == 1) {
                _okButton.Enabled = UriUtil.IsHttpUri(_addressTextBox.Text) || UriUtil.IsFileUri(_addressTextBox.Text);
            }
        }


        // --- handler ---
        private void HandleWorkspaceTreeAfterSelect(object sender, TreeViewEventArgs e) {
            UpdateMemoListBox();
            if (!_inited) {
                /// ShowDialog()内で必ずこのメソッドが呼ばれるための措置
                /// これをやっておかないと初期状態でノートを選択した状態にならない
                if (UriUtil.IsMemoUri(_defaultUri)) {
                    Uri = _defaultUri;
                }
                _inited = true;
            }
            UpdateOkButtonEnabled();
        }

        private void HandleMemoListBoxSelectedIndex(object sender, EventArgs e) {
            UpdateOkButtonEnabled();
        }

        // --- generated handler ---
        private void _linkTargetListBox_SelectedIndexChanged(object sender, EventArgs e) {
            if (_linkTargetListBox.SelectedIndex == 0) {
                _memoLinkPanel.Visible = true;
                _webLinkPanel.Visible = false;

            } else if (_linkTargetListBox.SelectedIndex == 1) {
                _memoLinkPanel.Visible = false;
                _webLinkPanel.Visible = true;
            }
            UpdateOkButtonEnabled();
        }

        private void _addressTextBox_TextChanged(object sender, EventArgs e) {
            UpdateOkButtonEnabled();
        }

        private void _titleTextTextBox_TextChanged(object sender, EventArgs e) {
            UpdateOkButtonEnabled();
        }
    }
}
