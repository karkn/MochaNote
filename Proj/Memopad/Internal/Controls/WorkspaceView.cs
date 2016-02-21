/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mkamo.Common.Forms.Utils;
using Mkamo.Memopad.Internal.Utils;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Model.Memo;
using Mkamo.Control.TreeNodeEx;
using Mkamo.Common.Forms.TreeView;
using Mkamo.Common.Forms.Input;
using Mkamo.Memopad.Internal.Helpers;
using Mkamo.Common.Forms.Themes;
using Mkamo.Model.Core;
using Mkamo.Memopad.Internal.Forms;
using Mkamo.Common.Forms.KeyMap;

namespace Mkamo.Memopad.Internal.Controls {
    [ToolboxItem(false)]
    internal partial class WorkspaceView: UserControl {
        // ========================================
        // field
        // ========================================
        private MemopadApplication _app;

        private Workspace _workspace;

        private KeyMap<TreeView> _folderTreeViewKeyMap;

        private bool _contextMenusEnabled;

        // --- theme ---
        private ITheme _theme;

        // ========================================
        // constructor
        // ========================================
        public WorkspaceView() {
            InitializeComponent();

            _app = MemopadApplication.Instance;
            _workspace = _app.Workspace;

            _contextMenusEnabled = true;
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            _folderTreeViewKeyMap = new KeyMap<TreeView>();
            if (!DesignMode) {
                _app.KeySchema.TreeViewKeyBinder.Bind(_folderTreeViewKeyMap);
            }
        }

        public void InitUI() {
            UpdateContextMenuEnabled(_contextMenusEnabled);
            _workspaceTree.RebuildTree();
        }


        // ========================================
        // property
        // ========================================
        public WorkspaceTree WorkspaceTree {
            get { return _workspaceTree; }
        }

        public SmartFolderTreePresenter SmartFolderTreePresenter {
            get { return _workspaceTree.SmartFolderTreePresenter; }
        }

        public FolderTreePresenter FolderTreePresenter {
            get { return _workspaceTree.FolderTreePresenter; }
        }

        public TagTreePresenter TagTreePresenter {
            get { return _workspaceTree.TagTreePresenter; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ITheme Theme {
            get { return _theme; }
            set {
                if (value == _theme) {
                    return;
                }
                _theme = value;

                _workspaceTree.BackColor = _theme.DarkBackColor;
                _workspaceTreePanel.BackColor = _theme.DarkBackColor;

                var captionFont = value.CaptionFont;
                _workspaceTree.Font = captionFont;

                var menuFont = value.MenuFont;
                _commonContextMenuStrip.Font = menuFont;
                _folderContextMenuStrip.Font = menuFont;
                _smartFolderContextMenuStrip.Font = menuFont;
                _tagContextMenuStrip.Font = menuFont;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ContextMenusEnabled {
            get { return _contextMenusEnabled; }
            set {
                if (value == _contextMenusEnabled) {
                    return;
                }
                _contextMenusEnabled = value;
                UpdateContextMenuEnabled(_contextMenusEnabled);
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Padding PanelPadding {
            get { return _workspaceTreePanel.Padding; }
            set { _workspaceTreePanel.Padding = value; }
        }

        // ========================================
        // method
        // ========================================
        // ------------------------------
        // protected
        // ------------------------------
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (_workspaceTree.Focused) {
                if (_folderTreeViewKeyMap.IsDefined(keyData)) {
                    var action = _folderTreeViewKeyMap.GetAction(keyData);
                    if (action != null) {
                        if (action(_workspaceTree)) {
                            return true;
                        }
                    }
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        // ------------------------------
        // private
        // ------------------------------
        private void UpdateContextMenuEnabled(bool enabled) {
            _workspaceTree.CommonContextMenuStrip = enabled? _commonContextMenuStrip: null;
            _workspaceTree.FolderContextMenuStrip = enabled? _folderContextMenuStrip: null;
            _workspaceTree.SmartFolderContextMenuStrip = enabled? _smartFolderContextMenuStrip: null;
            _workspaceTree.TagContextMenuStrip = enabled? _tagContextMenuStrip: null;
        }

        // --- common ---
        private void _commonContextMenuStrip_Opening(object sender, CancelEventArgs e) {
            _commonSmartFolderCreateSmartFolderToolStripMenuItem.Visible = _workspaceTree.IsSmartFolderCategorySelected;
            _commonFolderCreateFolderToolStripMenuItem.Visible = _workspaceTree.IsFolderCategorySelected;
            _commonCreateTagToolStripMenuItem.Visible = _workspaceTree.IsTagCategorySelected;
            _commonEmptyTrashBoxToolStripMenuItem.Visible = _workspaceTree.IsTrashBoxSelected;
        }

        private void _workspaceCreateFolderToolStripMenuItem_Click(object sender, EventArgs e) {
            _workspaceTree.FolderTreePresenter.CreateFolder();
        }

        private void _commonEmptyTrashBoxToolStripMenuItem_Click(object sender, EventArgs e) {
            if (MessageUtil.ConfirmEmptyTrashBox()) {
                _app.EmptyMemosFromTrashBox();
            }
        }

        private void _commonCreateTagToolStripMenuItem_Click(object sender, EventArgs e) {
            _workspaceTree.TagTreePresenter.CreateTag();
        }

        private void _commonSmartFolderCreateSmartFolderToolStripMenuItem_Click(object sender, EventArgs e) {
            _workspaceTree.SmartFolderTreePresenter.CreateSmartFolder();
        }

        // --- memo folder ---
        private void _memoFolderContextMenuStrip_Opening(object sender, CancelEventArgs e) {
            if (_workspaceTree.SelectedFolder != null && _workspaceTree.SelectedFolder == _app.ActiveFolder) {
                _activateFolderToolStripMenuItem.Visible = false;
                _deactivateFolderToolStripMenuItem.Visible = true;
            } else {
                _activateFolderToolStripMenuItem.Visible = true;
                _deactivateFolderToolStripMenuItem.Visible = false;
            }
        }

        private void _removeFolderToolStripMenuItem_Click(object sender, EventArgs e) {
            _workspaceTree.FolderTreePresenter.RemoveFolder();
        }

        private void _renameToolStripMenuItem_Click(object sender, EventArgs e) {
            _workspaceTree.FolderTreePresenter.RenameFolder();
        }

        private void _openAllMemosToolStripMenuItem_Click(object sender, EventArgs e) {
            _workspaceTree.FolderTreePresenter.OpenAllMemos();
        }

        private void _addAllOpenMemoAToolStripMenuItem_Click(object sender, EventArgs e) {
            _workspaceTree.FolderTreePresenter.AddAllOpenMemos();
        }

        private void _clearMemosToolStripMenuItem_Click(object sender, EventArgs e) {
            _workspaceTree.FolderTreePresenter.ClearMemos();
        }

        private void _createFolderToolStripMenuItem_Click(object sender, EventArgs e) {
            _workspaceTree.FolderTreePresenter.CreateFolder();
        }

        private void _createAndAddMemoToolStripMenuItem_Click(object sender, EventArgs e) {
            _workspaceTree.FolderTreePresenter.CreateMemo();
        }

        private void _activateFolderToolStripMenuItem_Click(object sender, EventArgs e) {
            _workspaceTree.FolderTreePresenter.ActivateFolder();
        }

        private void _deactivateFolderToolStripMenuItem_Click(object sender, EventArgs e) {
            _workspaceTree.FolderTreePresenter.DeactivateFolder();
        }

        // --- tag ---
        private void _tagContextMenuStrip_Opening(object sender, CancelEventArgs e) {
            _tagCreateTagToolStripMenuItem.Enabled = _workspaceTree.IsTagSelected;
            _tagCreateMemoToolStripMenuItem.Enabled = _workspaceTree.IsTagSelected || _workspaceTree.IsUntaggedSelected;
            _tagRemoveToolStripMenuItem.Enabled = _workspaceTree.IsTagSelected;
            _tagRenameToolStripMenuItem.Enabled = _workspaceTree.IsTagSelected;
        }

        private void _tagCreateTagToolStripMenuItem_Click(object sender, EventArgs e) {
            _workspaceTree.TagTreePresenter.CreateTag();
        }

        private void _tagCreateMemoToolStripMenuItem_Click(object sender, EventArgs e) {
            _workspaceTree.TagTreePresenter.CreateMemo();
        }

        private void _tagRemoveToolStripMenuItem_Click(object sender, EventArgs e) {
            _workspaceTree.TagTreePresenter.RemoveTag();
        }

        private void _tagRenameToolStripMenuItem_Click(object sender, EventArgs e) {
            _workspaceTree.TagTreePresenter.RenameTag();
        }

        // --- smart folder ---
        private void _smartFolderRemoveToolStripMenuItem_Click(object sender, EventArgs e) {
            _workspaceTree.SmartFolderTreePresenter.RemoveSmartFolder();
        }

        private void _smartFolderRenameToolStripMenuItem_Click(object sender, EventArgs e) {
            _workspaceTree.SmartFolderTreePresenter.RenameSmartFolder();
        }

        private void _editSmartFolderToolStripMenuItem_Click(object sender, EventArgs e) {
            var smartFolder = _workspaceTree.SmartFolderTreePresenter.SelectedSmartFolder;
            if (smartFolder != null) {
                using (var dialog = new QueryHolderEditForm()) {
                    dialog.QueryHolder = smartFolder;
                    if (dialog.ShowDialog(_app.MainForm) == DialogResult.OK) {
                        _app.MainForm.Mediator.UpdateMemoListBox(false);
                    }
                }
            }
        }

    }
}
