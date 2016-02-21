/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Memopad.Internal.Core;
using System.Windows.Forms;
using Mkamo.Control.TreeNodeEx;
using Mkamo.Common.Diagnostics;
using Mkamo.Memopad.Core;
using Mkamo.Model.Memo;
using Mkamo.Common.Forms.TreeView;
using Mkamo.Memopad.Internal.Utils;
using Mkamo.Memopad.Internal.Forms;
using Mkamo.Model.Core;

namespace Mkamo.Memopad.Internal.Controls {
    internal class SmartFolderTreePresenter: IMemoInfoListProvider {
        // ========================================
        // field
        // ========================================
        private MemopadApplication _facade;
        private MemoFinder _finder;

        private TreeView _tree;
        private TreeNodeEx _root;

        private int _smartFolderImageIndex;

        // ========================================
        // constructor
        // ========================================
        public SmartFolderTreePresenter(TreeView tree, TreeNodeEx root) {
            Contract.Requires(tree != null);
            Contract.Requires(root != null);

            _facade = MemopadApplication.Instance;
            _finder = new MemoFinder();
            _tree = tree;
            _root = root;
        }

        // ========================================
        // property
        // ========================================
        public IEnumerable<MemoInfo> MemoInfos {
            get {
                var selectedFolder = SelectedSmartFolder;
                if (selectedFolder == null) {
                    return new MemoInfo[0];
                } else {
                    return _finder.Search(selectedFolder.Query);
                }
            }
        }

        public int SmartFolderImageIndex {
            get { return _smartFolderImageIndex; }
            set { _smartFolderImageIndex = value; }
        }

        public bool IsSmartFolderSelected {
            get { return _tree.SelectedNode != null && _tree.SelectedNode.Tag is MemoSmartFolder; }
        }

        public MemoSmartFolder SelectedSmartFolder {
            get { return _tree.SelectedNode == null? null: _tree.SelectedNode.Tag as MemoSmartFolder; }
        }

        public TreeNode Root {
            get { return _root; }
        }

        // ========================================
        // method
        // ========================================
        public void RebuildTree() {
            var smartFolders = _facade.Workspace.SmartFolders;

            _root.Nodes.Clear();
            foreach (var smartFolder in smartFolders) {
                var node = new TreeNodeEx(smartFolder.Name);
                node.Tag = smartFolder;
                node.ImageIndex = _smartFolderImageIndex;
                node.SelectedImageIndex = _smartFolderImageIndex;
                _root.Nodes.Add(node);
            }
        }

        public void UpdateAdded(MemoSmartFolder smartFolder) {
            var node = new TreeNodeEx(smartFolder.Name);
            node.Tag = smartFolder;
            node.ImageIndex = _smartFolderImageIndex;
            node.SelectedImageIndex = _smartFolderImageIndex;

            _tree.BeginUpdate();
            _root.Nodes.Add(node);
            _tree.EndUpdate();
        }

        public void UpdateRemoving(MemoSmartFolder smartFolder) {
            var removed = default(TreeNode);
            foreach (var node in new TreeNodeIterator(_root)) {
                if (node.Tag == smartFolder) {
                    removed = node;
                }
            }

            _tree.BeginUpdate();
            if (removed != null) {
                removed.Remove();
            }
            _tree.EndUpdate();
        }

        public void UpdateChanged(MemoSmartFolder smartFolder) {
            _tree.BeginUpdate();
            foreach (var node in new TreeNodeIterator(_root)) {
                if (node.Tag == smartFolder) {
                    if (node.Text != smartFolder.Name) {
                        node.Text = smartFolder.Name;
                        _tree.Sort();
                    }
                }
            }
            _tree.EndUpdate();
        }

        public TreeNode GetNode(object obj) {
            var smartFolder = obj as MemoSmartFolder;
            if (smartFolder == null) {
                return null;

            } else {
                foreach (var node in new TreeNodeIterator(_tree)) {
                    if (node.Tag == smartFolder) {
                        return node;
                    }
                }

                return null;
            }
        }

        public TreeNode SelectNode(object obj) {
            return _tree.SelectedNode = GetNode(obj);
        }

        public void CreateSmartFolder() {
            using (var dialog = new QueryHolderEditForm()) {
                var smartFolder = MemoFactory.CreateTransientSmartFolder();
                smartFolder.Query = MemoFactory.CreateTransientQuery();
                smartFolder.Name = "新しいスマートフォルダ";
                smartFolder.Query = smartFolder.Query;
                dialog.QueryHolder = smartFolder;
                if (dialog.ShowDialog(_facade.MainForm) == DialogResult.OK) {
                    /// これをやっておかないと一度もRootがExpand()されていない場合，
                    /// 2つノードが作成されてしまう。
                    /// 理由は未調査。
                    _root.Expand();
                    //_facade.MainForm.WorkspaceView.WorkspaceTree.SmartFolderTreePresenter.Root.Expand();

                    _facade.Container.Persist(dialog.QueryHolder.Query);
                    _facade.Container.Persist(dialog.QueryHolder);

                    SelectNode(dialog.QueryHolder);
                }
            }
        }

        public void RemoveSmartFolder() {
            var smartFolder = SelectedSmartFolder;
            if (smartFolder != null) {
                if (!MessageUtil.ConfirmSmartFolderRemoval(smartFolder)) {
                    return;
                }
                _tree.BeginUpdate();
                _facade.Workspace.RemoveSmartFolder(smartFolder);
                _tree.EndUpdate();
            }
        }

        public void RenameSmartFolder() {
            var smartFolder = SelectedSmartFolder;
            if (smartFolder != null) {
                _tree.SelectedNode.BeginEdit();
            };
        }
    }
}
