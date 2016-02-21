/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mkamo.Control.TreeNodeEx;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Model.Memo;
using Mkamo.Common.Diagnostics;
using Mkamo.Memopad.Core;
using Mkamo.Common.Forms.TreeView;
using Mkamo.Memopad.Internal.Utils;

namespace Mkamo.Memopad.Internal.Controls {
    internal class FolderTreePresenter: IMemoInfoListProvider {
        // ========================================
        // field
        // ========================================
        private MemopadApplication _facade;

        private TreeView _tree;
        private TreeNodeEx _root;

        private int _folderImageIndex;
        private int _activeFolderImageIndex;

        // ========================================
        // constructor
        // ========================================
        public FolderTreePresenter(TreeView tree, TreeNodeEx root) {
            Contract.Requires(tree != null);
            Contract.Requires(root != null);

            _facade = MemopadApplication.Instance;
            _tree = tree;
            _root = root;
        }

        // ========================================
        // event
        // ========================================
        public event EventHandler Updated;

        // ========================================
        // property
        // ========================================
        public IEnumerable<MemoInfo> MemoInfos {
            get {
                var ret = new List<MemoInfo>();

                var selectedFolder = SelectedFolder;
                if (selectedFolder != null) {
                    foreach (var memo in selectedFolder.ContainingMemos) {
                        var info = _facade.FindMemoInfo(memo);
                        if (info != null) {
                            /// ごみ箱にあるノートの場合null
                            ret.Add(info);
                        }
                    }
                }

                return ret;
            }
        }

        public bool IsFolderSelected {
            get { return _tree.SelectedNode != null && _tree.SelectedNode.Tag is MemoFolder; }
        }

        public MemoFolder SelectedFolder {
            get { return _tree.SelectedNode == null? null: _tree.SelectedNode.Tag as MemoFolder; }
        }

        public int FolderImageIndex {
            get { return _folderImageIndex; }
            set { _folderImageIndex = value; }
        }

        public int ActiveFolderImageIndex {
            get { return _activeFolderImageIndex; }
            set { _activeFolderImageIndex = value; }
        }


        // ========================================
        // method
        // ========================================
        public void RebuildTree() {
            var folders = _facade.Workspace.RootFolders;

            _root.Nodes.Clear();
            foreach (var folder in folders) {
                var node = new TreeNodeEx(folder.Name);
                node.Tag = folder;
                if (folder == _facade.ActiveFolder) {
                    node.ImageIndex = _activeFolderImageIndex;
                    node.SelectedImageIndex = _activeFolderImageIndex;
                } else {
                    node.ImageIndex = _folderImageIndex;
                    node.SelectedImageIndex = _folderImageIndex;
                }
                _root.Nodes.Add(node);
            }

            OnUpdated();
        }

        public TreeNode GetNode(object obj) {
            var folder = obj as MemoFolder;
            if (folder == null) {
                return null;

            } else {
                foreach (var node in new TreeNodeIterator(_tree)) {
                    if (node.Tag == folder) {
                        return node;
                    }
                }

                return null;
            }
        }

        public TreeNode SelectNode(object obj) {
            return _tree.SelectedNode = GetNode(obj);
        }

        public void OpenAllMemos() {
            if (_tree.SelectedNode == null) {
                return;
            }
            var selected = _tree.SelectedNode.Tag as MemoFolder;
            if (selected != null) {
                var memos = selected.ContainingMemos;
                var first = true;
                foreach (var memo in memos) {
                    var info = _facade.FindMemoInfo(memo);
                    if (first) {
                        first = false;
                        _facade.LoadMemo(info, false);
                    } else {
                        _facade.LoadMemo(info, true);
                    }
                }
            }
        }

        public MemoInfo CreateMemo() {
            if (_tree.SelectedNode == null) {
                return null;
            }
            var selected = _tree.SelectedNode.Tag as MemoFolder;
            if (selected == null) {
                return null;
            }
            var memoInfo = _facade.CreateMemo();
            if (memoInfo != null) {
                var memo = _facade.Container.Find<Memo>(memoInfo.MemoId);
                memo.AddContainedFolder(selected);
            }
            return memoInfo;
        }

        public void CreateFolder() {
            if (_tree.SelectedNode == null) {
                return;
            }
            CreateFolder(_tree.SelectedNode);
        }

        public void CreateFolder(TreeNode parent) {
            var created = default(MemoFolder);

            _root.Expand();

            if (parent == _root) {
                created = _facade.Workspace.CreateFolder();
                created.Name = "新しいクリアファイル";

            } else if (parent.Tag is MemoFolder) {
                var parentFolder = (MemoFolder) parent.Tag;

                created = _facade.Workspace.CreateFolder();
                created.Name = "新しいクリアファイル";
                created.ParentFolder = parentFolder;

            } else if (parent.Tag is Memo) {
                return;
            }

            _tree.BeginUpdate();
            var createdNode = new TreeNodeEx(created.Name);
            createdNode.Tag = created;
            createdNode.ImageIndex = _folderImageIndex;
            createdNode.SelectedImageIndex = _folderImageIndex;
            parent.Nodes.Add(createdNode);
            _tree.SelectedNode = createdNode;
            _tree.EndUpdate();
            createdNode.BeginEdit();
        }

        public void ClearMemos() {
            if (_tree.SelectedNode == null) {
                return;
            }
            var selected = _tree.SelectedNode.Tag as MemoFolder;
            if (selected != null) {
                selected.ClearContainingMemos();
            }
        }

        public void RemoveFolder() {
            if (_tree.SelectedNode == null) {
                return;
            }
            var selectedNode = _tree.SelectedNode;
            if (selectedNode != _root) {
                var folder = selectedNode.Tag as MemoFolder;
                if (folder != null) {
                    if (!MessageUtil.ConfirmFolderRemoval(folder)) {
                        return;
                    }
                    _tree.BeginUpdate();
                    var parentNode = selectedNode.Parent;
                    selectedNode.Remove();
                    _facade.Workspace.RemoveFolder(folder);
                    _tree.SelectedNode = parentNode;
                    _tree.EndUpdate();
                }
            }
        }

        public void RemoveMemo(bool completely) {
            var selectedNode = _tree.SelectedNode;
            if (selectedNode == null) {
                return;
            }

            if (selectedNode != _root) {
                var selected = selectedNode.Tag as Memo;
                if (selected != null) {
                    /// viewから消してContaineingFolderから削除
                    _tree.BeginUpdate();
                    var parentNode = selectedNode.Parent;
                    var parentFolder = (MemoFolder) parentNode.Tag;
                    selectedNode.Remove();
                    parentFolder.RemoveContainingMemo(selected);
                    if (completely) {
                        _facade.RemoveMemo(_facade.FindMemoInfo(selected));
                    }
                    _tree.SelectedNode = parentNode;
                    _tree.EndUpdate();
                }
            }
        }

        public void RenameFolder() {
            if (_tree.SelectedNode == null) {
                return;
            }
            var selectedNode = _tree.SelectedNode;
            if (selectedNode != _root) {
                selectedNode.BeginEdit();
            };
        }

        public void AddAllOpenMemos() {
            if (_tree.SelectedNode == null) {
                return;
            }
            var selected = _tree.SelectedNode.Tag as MemoFolder;
            if (selected != null) {
                foreach (var info in _facade.MainForm.OpenMemoInfos) {
                    var memo = _facade.Container.Find<Memo>(info.MemoId);
                    selected.AddContainingMemo(memo);
                }
            }
        }

        public void ActivateFolder(MemoFolder folder) {
            if (folder == null) {
                return;
            }

            if (folder != null) {
                _facade.ActiveFolder = folder;
            }
        }

        public void ActivateFolder() {
            ActivateFolder(SelectedFolder);
        }

        public void DeactivateFolder() {
            _facade.ActiveFolder = null;
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected virtual void OnUpdated() {
            var handler = Updated;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

    }
}
