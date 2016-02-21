/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Memopad.Internal.Controls;
using Mkamo.Memopad.Internal.Forms;
using Mkamo.Common.Diagnostics;
using Mkamo.Memopad.Core;
using System.Windows.Forms;
using Mkamo.Model.Core;
using Mkamo.Common.Event;
using Mkamo.Model.Memo;
using Mkamo.Common.Core;

namespace Mkamo.Memopad.Internal.Core {
    internal class MemopadFormMediator {
        // ========================================
        // field
        // ========================================
        private MemopadApplication _facade;
        private MemoFinder _finder;

        private MemopadForm _mainForm;
        private WorkspaceView _workspaceView;
        private MemoQueryBuilderView _memoQueryBuilderView;
        private MemoListView _memoListView;

        // ========================================
        // constructor
        // ========================================
        public MemopadFormMediator(
            MemopadForm mainForm,
            WorkspaceView workspaceView,
//            MemoQueryBuilderView memoQueryBuilderView,
            MemoListView memoListView
        ) {
            Contract.Requires(workspaceView != null);
//            Contract.Requires(memoQueryBuilderView != null);
            Contract.Requires(memoListView != null);

            _facade = MemopadApplication.Instance;
            _finder = new MemoFinder();

            _mainForm = mainForm;
            _workspaceView = workspaceView;
//            _memoQueryBuilderView = memoQueryBuilderView;
            _memoListView = memoListView;

            _memoListView.MemoListBox.CanDragStart = true;

            InitHandlers();
        }

        private void InitHandlers() {
            _facade.MemoInfoAdded += HandleMemoInfoAdded;
            _facade.MemoInfoRemoved += HandleMemoInfoChanged;
            _facade.MemoInfoRemovedCompletely += HandleMemoInfoChanged;
            _facade.MemoInfoRecovered += HandleMemoInfoChanged;
            _facade.MemoInfoChanged += HandleMemoInfoChanged;

            _facade.Workspace.MemoTagAdded += HandleMemoTagAdded;
            _facade.Workspace.MemoTagRemoving += HandleMemoTagRemoving;
            _facade.Workspace.MemoTagChanged += HandleMemoTagChanged;

            _facade.Workspace.MemoSmartFolderAdded += HandleMemoSmartFolderAdded;
            _facade.Workspace.MemoSmartFolderRemoving += HandleMemoSmartFolderRemoving;
            _facade.Workspace.MemoSmartFolderChanged += HandleMemoSmartFolderChanged;

            _facade.ActiveFolderChanged += HandleActiveFolderChanged;
            _facade.Workspace.MemoFolderChanged += HandleMemoFolderChanged;

            _workspaceView.WorkspaceTree.AfterSelect += HandleWorkspaceTreeAfterSelect;
            //_memoQueryBuilderView.QueryUpdated += HandleMemoQueryBuilderViewQueryUpdated;
            _memoListView.MemoListBox.MouseDoubleClick += HandleMemoListBoxMouseDoubleClick;

            _mainForm._TabControl.ControlAdded += HandleMainFormTabControlAdded;
            _mainForm._TabControl.ControlRemoved += HandleMainFormTabControlRemoved;
            _facade.FusenManager.Registered += HandleFusenManagerRegisteredOrUnregistered;
            _facade.FusenManager.Unregistered += HandleFusenManagerRegisteredOrUnregistered;
        }

        // ========================================
        // destructor
        // ========================================



        // ========================================
        // property
        // ========================================
        public bool IsWorkspaceViewShown {
            get { return _workspaceView != null && _workspaceView.Visible; }
        }

        public bool IsMemoQueryBuilderViewShown {
            get { return _memoQueryBuilderView != null && _memoQueryBuilderView.Visible; }
        }

        internal MemoQueryBuilderView MemoQueryBuilderView {
            set {
                if (value == _memoQueryBuilderView) {
                    return;
                }
                _memoQueryBuilderView = value;
                if (_memoQueryBuilderView != null) {
                    _memoQueryBuilderView.QueryUpdated += HandleMemoQueryBuilderViewQueryUpdated;
                }
            }
        }


        // ========================================
        // method
        // ========================================
        public void UpdateMemoListBox(bool keepTopIndex) {
            var oldCursor = Cursor.Current;
            try {
                Cursor.Current = Cursors.WaitCursor;

                if (IsWorkspaceViewShown) {
                    if (!_workspaceView.WorkspaceTree.IsInDragOver) {
                        var provider = _workspaceView.WorkspaceTree.CurrentMemoInfoListProvider;
                        if (provider == null) {
                            _memoListView.MemoListBox.UpdateList(new MemoInfo[0], false, keepTopIndex);
                        } else {
                            _memoListView.MemoListBox.UpdateList(provider.MemoInfos, false, keepTopIndex);
                        }
                    }
                } else if (IsMemoQueryBuilderViewShown) {
                    _memoListView.MemoListBox.UpdateList(_finder.Search(_memoQueryBuilderView.GetQuery()), false, keepTopIndex);
                }
            } finally {
                Cursor.Current = oldCursor;
            }
        }


        public void UpdateMemoImportance(IEnumerable<MemoInfo> infos, MemoImportanceKind importance) {
            if (infos == null || !infos.Any()) {
                return;
            }

            var memos = infos.Select(i => _facade.Container.Find<Memo>(i.MemoId));
            memos.ForEach(memo => memo.Importance = importance);

            if (_facade.IsMainFormLoaded) {
                _mainForm.UpdateToolStrip();
                var listBox = _mainForm._MemoListView.MemoListBox;
                if (_mainForm._MemoListView.MemoListBox.SortsImportanceOrder) {
                    if (infos.Any(i => listBox.Items.Contains(i))) {
                        listBox.Sorted = false;
                        listBox.Sorted = true;
                    }
                }
                listBox.InvalidateList(infos);
            }

        }


        // --- handler ---
        private void HandleWorkspaceTreeAfterSelect(object sender, TreeViewEventArgs e) {
            var tree = _workspaceView.WorkspaceTree;
            if (tree.IsSmartFolderSelected) {
                _memoListView.TargetKind = MemoListTargetKind.SmartFolder;
            } else if (tree.IsTagSelected || tree.IsUntaggedSelected) {
                _memoListView.TargetKind = MemoListTargetKind.Tag;
            } else if (tree.IsFolderSelected) {
                _memoListView.TargetKind = MemoListTargetKind.Folder;
            } else if (tree.IsTrashBoxSelected) {
                _memoListView.TargetKind = MemoListTargetKind.TrashBox;
            } else if (tree.IsOpenMemosSelected) {
                _memoListView.TargetKind = MemoListTargetKind.OpenMemos;
            }

            UpdateMemoListBox(false);
        }

        private void HandleMemoInfoAdded(object sender, MemoInfoEventArgs e) {
            var provider = _workspaceView.WorkspaceTree.CurrentMemoInfoListProvider;
            if (provider == null) {
                _memoListView.MemoListBox.UpdateList(new MemoInfo[0], false, false);
            } else {
                var newInfos = provider.MemoInfos;
                if (newInfos.Contains(e.MemoInfo)) {
                    _memoListView.MemoListBox.UpdateList(newInfos, false, true);
                }
            }
        }

        private void HandleMemoInfoChanged(object sender, MemoInfoEventArgs e) {
            var infos = _memoListView.MemoListBox.MemoInfos;
            if (infos.Contains(e.MemoInfo)) {
                UpdateMemoListBox(true);
            }
        }

        private void HandleMemoQueryBuilderViewQueryUpdated(object sender, EventArgs e) {
            if (IsMemoQueryBuilderViewShown) {
                UpdateMemoListBox(false);
            }
        }

        private void HandleActiveFolderChanged(object sender, EventArgs e) {
            UpdateMemoListBox(true);
        }

        private void HandleMainFormTabControlAdded(object sender, EventArgs e) {
            if (_workspaceView.WorkspaceTree.IsOpenMemosSelected) {
                UpdateMemoListBox(true);
            }
        }

        private void HandleMainFormTabControlRemoved(object sender, EventArgs e) {
            if (_workspaceView.WorkspaceTree.IsOpenMemosSelected) {
                _workspaceView.WorkspaceTree._OpenMemosPresenter.IsForControlRemoved = true;
                try {
                    UpdateMemoListBox(true);
                } finally {
                    _workspaceView.WorkspaceTree._OpenMemosPresenter.IsForControlRemoved = false;
                }
            }
        }

        private void HandleFusenManagerRegisteredOrUnregistered(object sender, EventArgs e) {
            if (_workspaceView.WorkspaceTree.IsOpenMemosSelected) {
                UpdateMemoListBox(true);
            }
        }

        // --- tag ---
        private void HandleMemoTagChanged(object sender, MemoTagChangedEventArgs e) {
            if (e.Cause.PropertyName != "Memos") {
                _workspaceView.WorkspaceTree.TagTreePresenter.UpdateChanged(e.Tag);
            }

            UpdateMemoListBox(true);
        }

        private void HandleMemoTagAdded(object sender, MemoTagEventArgs e) {
            _workspaceView.WorkspaceTree.TagTreePresenter.UpdateAdded(e.Tag);
            UpdateMemoListBox(false);
        }

        private void HandleMemoTagRemoving(object sender, MemoTagEventArgs e) {
            _workspaceView.WorkspaceTree.TagTreePresenter.UpdateRemoving(e.Tag);
            UpdateMemoListBox(false);
        }

        // --- smart folder ---
        private void HandleMemoSmartFolderChanged(object sender, MemoSmartFolderChangedEventArgs e) {
            _workspaceView.WorkspaceTree.SmartFolderTreePresenter.UpdateChanged(e.SmartFolder);
            if (e.SmartFolder == _workspaceView.WorkspaceTree.SmartFolderTreePresenter.SelectedSmartFolder) {
                UpdateMemoListBox(true);
            }
        }

        private void HandleMemoSmartFolderAdded(object sender, MemoSmartFolderEventArgs e) {
            _workspaceView.WorkspaceTree.SmartFolderTreePresenter.UpdateAdded(e.SmartFolder);
        }

        private void HandleMemoSmartFolderRemoving(object sender, MemoSmartFolderEventArgs e) {
            _workspaceView.WorkspaceTree.SmartFolderTreePresenter.UpdateRemoving(e.SmartFolder);
        }

        // --- folder ---
        private void HandleMemoFolderChanged(object sender, MemoFolderChangedEventArgs e) {
            if (_facade.ActiveFolder != null && e.Cause.PropertyName == "ContainingMemos") {
                if (e.Cause.Kind == PropertyChangeKind.Remove) {
                    /// ノートをクリアファイルから削除したときにタブを閉じる
                    var openInfos = _facade.MainForm.OpenMemoInfos.ToArray();
                    foreach (var info in openInfos) {
                        var memo = _facade.Container.Find<Memo>(info.MemoId);
                        if (!e.Folder.ContainingMemos.Contains(memo)) {
                            _facade.CloseMemo(info);
                        }
                    }

                } else if (e.Cause.Kind == PropertyChangeKind.Clear) {
                    /// ノートをクリアファイルからクリアしたらすべてのタブを閉じる
                    _facade.CloseAllMemos();

                } else if (e.Cause.Kind == PropertyChangeKind.Add) {
                    /// ノートをクリアファイルに追加したときにタブを開く
                    var openInfos = _facade.MainForm.OpenMemoInfos.ToArray();
                    foreach (var memo in e.Folder.ContainingMemos) {
                        var info = _facade.FindMemoInfo(memo);
                        if (!openInfos.Contains(info)) {
                            _facade.LoadMemo(info, false);
                        }
                    }
                }
            }

            if (e.Folder == _workspaceView.WorkspaceTree.SelectedFolder || _facade.ActiveFolder != null) {
                UpdateMemoListBox(true);
            }
        }

        // --- memo list box ---
        private void HandleMemoListBoxMouseDoubleClick(object sender, MouseEventArgs e) {
            var listBox = _memoListView.MemoListBox;
            var index = listBox.IndexFromPoint(e.Location);
            if (index > -1) {
                var rect = listBox.GetItemRectangle(index);
                if (rect.Contains(e.Location)) {
                    /// 最後のitemより下の余白のマウス操作でない

                    _memoListView.LoadSelectedMemos();
                }
            }
        }

    }
}
