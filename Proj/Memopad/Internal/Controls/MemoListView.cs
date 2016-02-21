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
using Mkamo.Memopad.Core;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Common.Forms.Input;
using Mkamo.Model.Memo;
using Mkamo.Common.Forms.Mouse;
using Mkamo.Common.Forms.Themes;
using Mkamo.Memopad.Internal.Utils;
using Mkamo.Common.Core;
using Mkamo.Control.Core;
using Mkamo.Common.Forms.KeyMap;

namespace Mkamo.Memopad.Internal.Controls {
    [ToolboxItem(false)]
    internal partial class MemoListView: UserControl {

        // ========================================
        // field
        // ========================================
        private MemopadApplication _app;

        private KeyMap<MemoListView> _keyMap;

        private MemoListTargetKind _targetKind;
        private WorkspaceView _workspaceView;

        // --- theme ---
        private ITheme _theme;

        // ========================================
        // constructor
        // ========================================
        public MemoListView() {
            InitializeComponent();

            _app = MemopadApplication.Instance;

            _memoListBox.Sorted = true;
            _memoListBox.DragOver += HandleMemoListBoxDragOver;
            _memoListBox.DragDrop += HandleMemoListBoxDragDrop;


        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            _keyMap = new KeyMap<MemoListView>();
            if (!DesignMode) {
                _app.KeySchema.MemoListViewKeyBinder.Bind(_keyMap);
            }
        }

        // ========================================
        // property
        // ========================================
        public MemoListBox MemoListBox {
            get { return _memoListBox; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MemoListTargetKind TargetKind {
            get { return _targetKind; }
            set {
                _targetKind = value;
                switch (_targetKind) {
                    case MemoListTargetKind.None: {
                        ContextMenuStrip = null;
                        break;
                    }
                    case MemoListTargetKind.QueryBuilder:
                    case MemoListTargetKind.SmartFolder:
                    case MemoListTargetKind.OpenMemos:
                        ContextMenuStrip = _commonContextMenuStrip;
                        break;
                    case MemoListTargetKind.Tag:
                        ContextMenuStrip = _tagContextMenuStrip;
                        break;
                    case MemoListTargetKind.Folder:
                        ContextMenuStrip = _folderContextMenuStrip;
                        break;
                    case MemoListTargetKind.TrashBox:
                        ContextMenuStrip = _trashBoxContextMenuStrip;
                        break;
                }
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ITheme Theme {
            get { return _theme; }
            set {
                if (value == _theme) {
                    return;
                }
                _theme = value;

                var captionFont = value.CaptionFont;
                _memoListBox.CaptionFont = captionFont;

                var descriptionFont = value.DescriptionFont;
                _memoListBox.DescriptionFont = descriptionFont;

                var menuFont = value.MenuFont;
                _commonContextMenuStrip.Font = menuFont;
                _folderContextMenuStrip.Font = menuFont;
                _trashBoxContextMenuStrip.Font = menuFont;
            }
        }

        private IEnumerable<MemoInfo> _SelectedMemoInfos {
            get {
                var ret = new MemoInfo[_memoListBox.SelectedItems.Count];
                if (ret.Length > 0) {
                    for (int i = 0, len = ret.Length; i < len; ++i) {
                        ret[i] = (MemoInfo) _memoListBox.SelectedItems[i];
                    }
                }
                return ret;
            }
        }


        // ========================================
        // method
        // ========================================
        public void SetTargetViews(WorkspaceView workspaceView) {
            _workspaceView = workspaceView;
        }

        public void SelectAll() {
            _memoListBox.BeginUpdate();
            for (int i = 0, len = _memoListBox.Items.Count; i < len; ++i) {
                _memoListBox.SelectedIndices.Add(i);
            }
            _memoListBox.EndUpdate();
        }

        public void SelectNextItem() {
            ListBoxUtil.SelectNextItem(_memoListBox);
        }

        public void SelectPreviousItem() {
            ListBoxUtil.SelectPreviousItem(_memoListBox);
        }

        public void LoadSelectedMemos() {
            var infos = new List<MemoInfo>();
            foreach (MemoInfo item in _memoListBox.SelectedItems) {
                infos.Add(item);
            }
            foreach (var info in infos) {
                if (_targetKind == MemoListTargetKind.TrashBox) {
                    _app.LoadRemovedMemo(info, false);
                } else {
                    _app.LoadMemo(info);
                }
            }
        }

        public void LoadSelectedMemosAsFusen() {
            if (_targetKind == MemoListTargetKind.TrashBox) {
                return;
            }
            var infos = new List<MemoInfo>();
            foreach (MemoInfo item in _memoListBox.SelectedItems) {
                infos.Add(item);
            }
            foreach (var info in infos) {
                _app.LoadMemoAsFusen(info, false);
            }
        }

        public void RemoveSelectedMemos() {
            if (_memoListBox.SelectedItems.Count == 0) {
                return;
            }

            var selecteds = new MemoInfo[_memoListBox.SelectedItems.Count];
            if (selecteds.Length > 0) {
                for (int i = 0, len = selecteds.Length; i < len; ++i) {
                    selecteds[i] = (MemoInfo) _memoListBox.SelectedItems[i];
                }
            }

            if (!MessageUtil.ConfirmMemoRemoval(selecteds)) {
                return;
            }

            _memoListBox.BeginUpdate();
            foreach (var item in selecteds) {
                var info = (MemoInfo) item;
                _app.RemoveMemo(info);
            }
            _memoListBox.EndUpdate();
        }

        public void RemoveSelectedMemosFromFolder() {
            var folder = _workspaceView.WorkspaceTree.SelectedFolder;
            if (folder != null) {
                var selecteds = new MemoInfo[_memoListBox.SelectedItems.Count];
                if (selecteds.Length > 0) {
                    for (int i = 0, len = selecteds.Length; i < len; ++i) {
                        selecteds[i] = (MemoInfo) _memoListBox.SelectedItems[i];
                    }
                }
                foreach (var item in selecteds) {
                    var info = (MemoInfo) item;
                    var memo = _app.Container.Find<Memo>(info.MemoId);
                    if (memo != null) {
                        folder.RemoveContainingMemo(memo);
                    }
                }
            }
        }

        public void RemoveSelectedMemosCompletely() {
            if (_targetKind != MemoListTargetKind.TrashBox) {
                return;
            }

            var selecteds = new MemoInfo[_memoListBox.SelectedItems.Count];
            if (selecteds.Length == 0) {
                return;
            }

            if (selecteds.Length > 0) {
                for (int i = 0, len = selecteds.Length; i < len; ++i) {
                    selecteds[i] = (MemoInfo) _memoListBox.SelectedItems[i];
                }
            }

            if (!MessageUtil.ConfirmMemoRemovalCompletely(selecteds)) {
                return;
            }

            _memoListBox.BeginUpdate();
            foreach (var item in selecteds) {
                var info = (MemoInfo) item;
                _app.RemoveCompletelyMemoFromTrashBox(info);
            }
            _memoListBox.EndUpdate();
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (_memoListBox.Focused) {
                if (_keyMap.IsDefined(keyData)) {
                    var action = _keyMap.GetAction(keyData);
                    if (action != null) {
                        if (action(this)) {
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
        private void LoadAllMemos() {
            _memoListBox.BeginUpdate();

            var infos = new List<MemoInfo>();
            foreach (MemoInfo item in _memoListBox.Items) {
                infos.Add(item);
            }
            var first = true;
            foreach (var info in infos) {
                if (first) {
                    first = false;
                    _app.LoadMemo(info, false);
                } else {
                    _app.LoadMemo(info, true);
                }
            }

            _memoListBox.EndUpdate();
        }

        private void RecoverSelectedMemos() {
            if (_targetKind != MemoListTargetKind.TrashBox) {
                return;
            }

            foreach (var item in _SelectedMemoInfos) {
                var info = (MemoInfo) item;
                _app.RecoverMemoFromTrashBox(info);
            }
        }

        private void EmptyMemos() {
            var result = MessageBox.Show(
                "ごみ箱内のノートを削除します。よろしいですか。",
                "ごみ箱内のノートの削除の確認",
                MessageBoxButtons.YesNo
            );
            if (result == DialogResult.Yes) {
                _memoListBox.BeginUpdate();
                _app.EmptyMemosFromTrashBox();
                _memoListBox.EndUpdate();
            }
        }



        private void CreateMemoForMemoFinder() {
            _memoListBox.BeginUpdate();

            var memoInfo = _app.CreateMemo();
            //var memo = _app.Container.Find<Memo>(memoInfo.MemoId);
            //if (_memoFinderView.IsTagSelected) {
            //    var tag = _memoFinderView.SelectedTag;
            //    if (tag != null) {
            //        tag.AddMemo(memo);
            //    }
            //}

            _memoListBox.EndUpdate();
        }

        private void CreateMemoForFolder() {
            _memoListBox.BeginUpdate();
            var info = _workspaceView.FolderTreePresenter.CreateMemo();
            if (info != null) {
                _memoListBox.UpdateList(_workspaceView.FolderTreePresenter.MemoInfos, false, false);
                var i = _memoListBox.Items.IndexOf(info);
                if (i > -1) {
                    _memoListBox.TopIndex = i;
                    _memoListBox.SelectedIndex = i;
                }
            }
            _memoListBox.EndUpdate();
        }

        private void CreateMemoForTag() {
            _memoListBox.BeginUpdate();
            _workspaceView.TagTreePresenter.CreateMemo();
            _memoListBox.EndUpdate();
        }

        private void ClearMemosForWorkspace() {
            _memoListBox.BeginUpdate();
            _workspaceView.FolderTreePresenter.ClearMemos();
            // update memo liset
            _memoListBox.EndUpdate();
        }

        private void AddAllOpenMemosForWorkspace() {
            _memoListBox.BeginUpdate();
            _workspaceView.FolderTreePresenter.AddAllOpenMemos();
            // update memo liset
            _memoListBox.EndUpdate();
        }

        private void SetMemoImportance(MemoImportanceKind importance) {
            var infos = _SelectedMemoInfos;
            if (!infos.Any()) {
                return;
            }

            if (_app.IsMainFormLoaded) {
                _app.MainForm.Mediator.UpdateMemoImportance(infos, importance);
            }
        }


        // --- handler ---
        private void HandleMemoListBoxDragOver(object sender, DragEventArgs e) {
            e.Effect = DragDropEffects.None;
            if (e.Data.GetDataPresent(typeof(MemoInfo[]))) {
                if (_targetKind == MemoListTargetKind.Folder) {
                    if (DragDropUtil.IsCopyAllowed(e)) {
                        var targetFolder = _workspaceView.WorkspaceTree.SelectedFolder;
                        if (targetFolder != null) {
                            e.Effect = DragDropEffects.Copy;
                        }
                    }

                } else if (_targetKind == MemoListTargetKind.Tag) {
                    if (DragDropUtil.IsCopyAllowed(e)) {
                        var targetTag = _workspaceView.WorkspaceTree.SelectedTag;
                        if (targetTag != null) {
                            e.Effect = DragDropEffects.Copy;
                        }
                    }

                } else if (_targetKind == MemoListTargetKind.TrashBox) {
                    e.Effect = DragDropEffects.Move;
                }
            }
        }


        private void HandleMemoListBoxDragDrop(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(typeof(MemoInfo[]))) {
                if (_targetKind == MemoListTargetKind.Folder) {
                    if (DragDropUtil.IsCopy(e)) {
                        var targetFolder = _workspaceView.WorkspaceTree.SelectedFolder;
                        if (targetFolder != null) {
                            var memoInfos = (MemoInfo[]) e.Data.GetData(typeof(MemoInfo[]));

                            var existsNotContained = false;
                            foreach (var memoInfo in memoInfos) {
                                var memo = _app.Container.Find<Memo>(memoInfo.MemoId);
                                if (!targetFolder.ContainingMemos.Contains(memo)) {
                                    existsNotContained = true;
                                    break;
                                }
                            }
                            if (existsNotContained) {
                                foreach (var memoInfo in memoInfos) {
                                    var memo = _app.Container.Find<Memo>(memoInfo.MemoId);
                                    if (!targetFolder.ContainingMemos.Contains(memo)) {
                                        targetFolder.AddContainingMemo(memo);
                                    }
                                }
                            }
                        }
                    }
                } else if (_targetKind == MemoListTargetKind.Tag) {
                    if (DragDropUtil.IsCopyAllowed(e)) {
                        var targetTag = _workspaceView.WorkspaceTree.SelectedTag;
                        if (targetTag != null) {
                            var memoInfos = (MemoInfo[]) e.Data.GetData(typeof(MemoInfo[]));

                            var existsNotContained = false;
                            foreach (var memoInfo in memoInfos) {
                                var memo = _app.Container.Find<Memo>(memoInfo.MemoId);
                                if (!targetTag.Memos.Contains(memo)) {
                                    existsNotContained = true;
                                    break;
                                }
                            }
                            if (existsNotContained) {
                                foreach (var memoInfo in memoInfos) {
                                    var memo = _app.Container.Find<Memo>(memoInfo.MemoId);
                                    if (!targetTag.Memos.Contains(memo)) {
                                        targetTag.AddMemo(memo);
                                    }
                                }
                            }
                        }
                    }

                } else if (_targetKind == MemoListTargetKind.TrashBox) {
                    if (DragDropUtil.IsMove(e)) {
                        var memoInfos = (MemoInfo[]) e.Data.GetData(typeof(MemoInfo[]));
                        if (MessageUtil.ConfirmMemoRemoval(memoInfos)) {
                            foreach (var info in memoInfos) {
                                _app.RemoveMemo(info);
                            }
                        }
                    }
                }
            }
        }

        
        // --- common ---
        private void _commonContextMenuStrip_Opening(object sender, CancelEventArgs e) {
            var any = _memoListBox.Items.Count > 0;
            var selected = _memoListBox.SelectedIndices.Count > 0;
            _commonOpenMemoToolStripMenuItem.Enabled = selected;
            _commonOpenMemoAsFusenToolStripMenuItem.Enabled = selected;
            _commonOpenAllMemosToolStripMenuItem.Enabled = any;
            _commonRemoveMemoToolStripMenuItem.Enabled = selected;
            _commonSelectAllToolStripMenuItem.Enabled = any;
            
            _commonImportanceToolStripMenuItem.Enabled = selected;
            _commonImportanceHighToolStripMenuItem.Enabled = selected;
            _commonImportanceNormalToolStripMenuItem.Enabled = selected;
            _commonImportanceLowToolStripMenuItem.Enabled = selected;
        }

        private void _commonOpenMemoToolStripMenuItem_Click(object sender, EventArgs e) {
            LoadSelectedMemos();
        }

        private void _commonOpenMemoAsFusenToolStripMenuItem_Click(object sender, EventArgs e) {
            LoadSelectedMemosAsFusen();
        }

        private void _commonOpenAllMemosToolStripMenuItem_Click(object sender, EventArgs e) {
            LoadAllMemos();
        }

        private void _commonRemoveMemoToolStripMenuItem_Click(object sender, EventArgs e) {
            RemoveSelectedMemos();
        }

        private void _commonSelectAllToolStripMenuItem_Click(object sender, EventArgs e) {
            SelectAll();
        }

        private void _commonImportanceToolStripMenuItem_DropDownOpening(object sender, EventArgs e) {
            var infos = _SelectedMemoInfos;
            var memos = infos.Select(i => _app.Container.Find<Memo>(i.MemoId));
            _commonImportanceHighToolStripMenuItem.Checked = memos.All(memo => memo.Importance == MemoImportanceKind.High);
            _commonImportanceNormalToolStripMenuItem.Checked = memos.All(memo => memo.Importance == MemoImportanceKind.Normal);
            _commonImportanceLowToolStripMenuItem.Checked = memos.All(memo => memo.Importance == MemoImportanceKind.Low);
        }

        private void _commonImportanceHighToolStripMenuItem_Click(object sender, EventArgs e) {
            SetMemoImportance(MemoImportanceKind.High);
        }

        private void _commonImportanceNormalToolStripMenuItem_Click(object sender, EventArgs e) {
            SetMemoImportance(MemoImportanceKind.Normal);
        }

        private void _commonImportanceLowToolStripMenuItem_Click(object sender, EventArgs e) {
            SetMemoImportance(MemoImportanceKind.Low);
        }

        // --- folder ---
        private void _folderContextMenuStrip_Opening(object sender, CancelEventArgs e) {
            var enabled = _workspaceView.WorkspaceTree.IsFolderSelected;
            var any = enabled && _memoListBox.Items.Count > 0;
            var selected = enabled && _memoListBox.SelectedIndices.Count > 0;

            _folderOpenAllMemosToolStripMenuItem.Enabled = any;
            _folderOpenMemoToolStripMenuItem.Enabled = selected;
            _folderOpenMemoAsFusenToolStripMenuItem.Enabled = selected;
            _folderCreateToolStripMenuItem.Enabled = enabled;
            _folderRemoveMemoFromFolderToolStripMenuItem.Enabled = selected;
            _folderRemoveMemoToolStripMenuItem.Enabled = selected;
            _folderClearMemosToolStripMenuItem.Enabled = any;
            _folderSelectAllToolStripMenuItem.Enabled = any;
            _folderAddOpenMemosToolStripMenuItem.Enabled = enabled && _app.MainForm.OpenMemoInfos.Count() > 0;

            _folderImportanceToolStripMenuItem.Enabled = selected;
            _folderImportanceHighToolStripMenuItem.Enabled = selected;
            _folderImportanceNormalToolStripMenuItem.Enabled = selected;
            _folderImportanceLowToolStripMenuItem.Enabled = selected;
        }

        private void _folderOpenAllMemosToolStripMenuItem_Click(object sender, EventArgs e) {
            LoadAllMemos();
        }

        private void _folderOpenMemoToolStripMenuItem_Click(object sender, EventArgs e) {
            LoadSelectedMemos();
        }

        private void _folderOpenMemoAsFusenToolStripMenuItem_Click(object sender, EventArgs e) {
            LoadSelectedMemosAsFusen();
        }

        private void _folderRemoveMemoToolStripMenuItem_Click(object sender, EventArgs e) {
            RemoveSelectedMemos();
        }

        private void _folderRemoveMemoFromFolderToolStripMenuItem_Click(object sender, EventArgs e) {
            RemoveSelectedMemosFromFolder();
        }

        private void _folderClearMemosToolStripMenuItem_Click(object sender, EventArgs e) {
            ClearMemosForWorkspace();
        }

        private void _folderSelectAllToolStripMenuItem_Click(object sender, EventArgs e) {
            SelectAll();
        }

        private void _folderAddOpenMemosToolStripMenuItem_Click(object sender, EventArgs e) {
            AddAllOpenMemosForWorkspace();
        }

        private void _folderCreateMemoToolStripMenuItem_Click(object sender, EventArgs e) {
            CreateMemoForFolder();
        }

        private void _folderImportanceToolStripMenuItem_DropDownOpening(object sender, EventArgs e) {
            var infos = _SelectedMemoInfos;
            var memos = infos.Select(i => _app.Container.Find<Memo>(i.MemoId));
            _folderImportanceHighToolStripMenuItem.Checked = memos.All(memo => memo.Importance == MemoImportanceKind.High);
            _folderImportanceNormalToolStripMenuItem.Checked = memos.All(memo => memo.Importance == MemoImportanceKind.Normal);
            _folderImportanceLowToolStripMenuItem.Checked = memos.All(memo => memo.Importance == MemoImportanceKind.Low);
        }

        private void _folderImportanceHighToolStripMenuItem_Click(object sender, EventArgs e) {
            SetMemoImportance(MemoImportanceKind.High);
        }

        private void _folderImportanceNormalToolStripMenuItem_Click(object sender, EventArgs e) {
            SetMemoImportance(MemoImportanceKind.Normal);
        }

        private void _folderImportanceLowToolStripMenuItem_Click(object sender, EventArgs e) {
            SetMemoImportance(MemoImportanceKind.Low);
        }


        // --- trash box ---
        private void _trashBoxContextMenuStrip_Opening(object sender, CancelEventArgs e) {
            var enabled = _workspaceView.WorkspaceTree.IsTrashBoxSelected;
            var any = enabled && _memoListBox.Items.Count > 0;
            var selected = enabled && _memoListBox.SelectedIndices.Count > 0;

            _trashBoxOpenMemoToolStripMenuItem.Enabled = selected;
            _trashBoxRecoverMemoToolStripMenuItem.Enabled = selected;
            _trashBoxRemoveMemoToolStripMenuItem.Enabled = selected;
            _trashBoxEmptyMemosToolStripMenuItem.Enabled = any;
        }

        private void _trashBoxOpenMemoToolStripMenuItem_Click(object sender, EventArgs e) {
            LoadSelectedMemos();
        }

        private void _trashBoxRecoverMemoToolStripMenuItem_Click(object sender, EventArgs e) {
            RecoverSelectedMemos();
        }

        private void _trashBoxRemoveMemoToolStripMenuItem_Click(object sender, EventArgs e) {
            RemoveSelectedMemosCompletely();
        }

        private void _trashBoxEmptyMemosToolStripMenuItem_Click(object sender, EventArgs e) {
            EmptyMemos();
        }

        // --- tag ---
        private void _tagContextMenuStrip_Opening(object sender, CancelEventArgs e) {
            var enabled = _workspaceView.WorkspaceTree.IsTagSelected || _workspaceView.WorkspaceTree.IsUntaggedSelected;
            var any = enabled && _memoListBox.Items.Count > 0;
            var selected = enabled && _memoListBox.SelectedIndices.Count > 0;

            _tagOpenMemoToolStripMenuItem.Enabled = selected;
            _tagOpenMemoAsFusenToolStripMenuItem.Enabled = selected;
            _tagOpenAllMemosToolStripMenuItem.Enabled = any;
            _tagCreateMemoToolStripMenuItem.Enabled = enabled;
            _tagRemoveMemoToolStripMenuItem.Enabled = selected;
            _tagSelectAllToolStripMenuItem.Enabled = any;

            _tagImportanceToolStripMenuItem.Enabled = selected;
            _tagImportanceHighToolStripMenuItem.Enabled = selected;
            _tagImportanceNormalToolStripMenuItem.Enabled = selected;
            _tagImportanceLowToolStripMenuItem.Enabled = selected;
        }

        private void _tagOpenMemoToolStripMenuItem_Click(object sender, EventArgs e) {
            LoadSelectedMemos();
        }

        private void _tagOpenMemoAsFusenToolStripMenuItem_Click(object sender, EventArgs e) {
            LoadSelectedMemosAsFusen();
        }

        private void _tagOpenAllMemosToolStripMenuItem_Click(object sender, EventArgs e) {
            LoadAllMemos();
        }

        private void _tagCreateMemoToolStripMenuItem_Click(object sender, EventArgs e) {
            CreateMemoForTag();
        }

        private void _tagRemoveMemoToolStripMenuItem_Click(object sender, EventArgs e) {
            RemoveSelectedMemos();
        }

        private void _tagSelectAllToolStripMenuItem_Click(object sender, EventArgs e) {
            SelectAll();
        }

        private void _tagImportanceToolStripMenuItem_DropDownOpening(object sender, EventArgs e) {
            var infos = _SelectedMemoInfos;
            var memos = infos.Select(i => _app.Container.Find<Memo>(i.MemoId));
            _tagImportanceHighToolStripMenuItem.Checked = memos.All(memo => memo.Importance == MemoImportanceKind.High);
            _tagImportanceNormalToolStripMenuItem.Checked = memos.All(memo => memo.Importance == MemoImportanceKind.Normal);
            _tagImportanceLowToolStripMenuItem.Checked = memos.All(memo => memo.Importance == MemoImportanceKind.Low);
        }

        private void _tagImportanceHighToolStripMenuItem_Click(object sender, EventArgs e) {
            SetMemoImportance(MemoImportanceKind.High);
        }

        private void _tagImportanceNormalToolStripMenuItem_Click(object sender, EventArgs e) {
            SetMemoImportance(MemoImportanceKind.Normal);
        }

        private void _tagImportanceLowToolStripMenuItem_Click(object sender, EventArgs e) {
            SetMemoImportance(MemoImportanceKind.Low);
        }

    }
}
