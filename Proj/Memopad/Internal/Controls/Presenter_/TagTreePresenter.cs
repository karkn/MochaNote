/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Control.TreeNodeEx;
using System.Windows.Forms;
using Mkamo.Memopad.Internal.Utils;
using Mkamo.Common.Core;
using Mkamo.Model.Memo;
using Mkamo.Common.Forms.TreeView;
using Mkamo.Common.Collection;
using Mkamo.Memopad.Core;
using Mkamo.Common.Diagnostics;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Common.String;
using System.Xml.XPath;
using System.IO;

namespace Mkamo.Memopad.Internal.Controls {
    internal class TagTreePresenter: IMemoInfoListProvider {
        // ========================================
        // static field
        // ========================================
        private const string NewTagText = "新しいタグ";
        private const string UntaggedText = "未整理";

        public static readonly object AllTagsObj = new object();
        public static readonly object UntaggedObj = new object();

        // ========================================
        // field
        // ========================================
        private MemopadApplication _facade;

        private TreeView _tree;
        private TreeNodeEx _root;

        //private bool _isAllTagsEnabled;
        private bool _isUntaggedEnabled;

        private TreeNode _untaggedNode;

        private int _untaggedImageIndex;
        private int _tagImageIndex;

        private bool _findMemoWithDescendants;

        // ========================================
        // constructor
        // ========================================
        public TagTreePresenter(TreeView tree, TreeNodeEx root, bool isUntaggedEnabled) {
            Contract.Requires(tree != null);
            Contract.Requires(root != null);

            _facade = MemopadApplication.Instance;
            _tree = tree;
            _root = root;

            _isUntaggedEnabled = isUntaggedEnabled;
            if (_isUntaggedEnabled) {
                _untaggedNode = new TreeNodeEx(UntaggedText);
                _untaggedNode.Tag = UntaggedObj;
            }

            _findMemoWithDescendants = false;
        }

        // ========================================
        // property
        // ========================================
        public IEnumerable<MemoInfo> MemoInfos {
            get {
                var ret = _facade.MemoInfos;
                var selectedTags = GetSelectedTagsAndDescendants();

                if (ret != null && ret.Any()) {
                    if (selectedTags == null) {
                        /// すべてのタグを追加なので何もしない

                    } else if (selectedTags.Any()) {
                        /// 選択されたタグのノートを追加
                        /// 選択されたタグ以外をフィルタ
                        /// このやり方はすべてのMemoをロードしてしまうので遅い
                        //Func<MemoInfo, bool> tagFilter = info => {
                        //    var memo = _facade.Container.Find<Memo>(info.MemoId);
                        //    return memo == null ? false : selectedTags.ContainsAny(memo.Tags);
                        //};
                        //ret = ret.Where(tagFilter).ToArray();

                        var set = new HashSet<MemoInfo>();
                        foreach (var tag in selectedTags) {
                            foreach (var memo in tag.Memos) {
                                var info = _facade.FindMemoInfo(memo);
                                if (info != null) {
                                    /// ごみ箱内のノートは返さないように
                                    set.Add(info);
                                }
                            }
                        }
                        ret = set;

                    } else {
                        /// タグのないノートを追加 => タグのあるノートをフィルタ
                        /// すべてのMemoをロードするので遅い
                        //Func<MemoInfo, bool> tagFilter = info => {
                        //    var memo = _facade.Container.Find<Memo>(info.MemoId);
                        //    return memo == null ? false : !memo.Tags.Any();
                        //};
                        //ret = ret.Where(tagFilter).ToArray();

                        var list = new List<MemoInfo>();
                        var ids = _facade.Container.GetIdsWhereWithEntityAndStoreRawData<Memo>(
                            memo => !memo.Tags.Any(),
                            raw => {
                                if (StringUtil.IsNullOrWhitespace(raw)) {
                                    return false;
                                }
                                return raw.IndexOf("<Tags />") > -1;
                            }
                        );
                        //var ids = _facade.Container.GetIdsLike<Memo>(
                        //    memo => !memo.Tags.Any(),
                        //    "<Tags />"
                        //);
                        foreach (var id in ids) {
                            var info = _facade.FindMemoInfoByMemoId(id);
                            if (info != null) {
                                list.Add(info);
                            }
                        }
                        ret = list;
                    }
                }
                return ret;
            }
        }

        public bool FindMemoWithDescendants {
            get { return _findMemoWithDescendants; }
            set { _findMemoWithDescendants = value; }
        }

        public bool IsUntaggedEnabled {
            get { return _isUntaggedEnabled; }
        }

        public bool IsUntaggedSelected {
            get {
                /// RebulidTree()前は_untaggedNode == nullになっている
                return _untaggedNode != null && _tree.SelectedNode == _untaggedNode;
            }
        }

        public bool IsTagSelected {
            get { return SelectedTag != null; }
        }

        public MemoTag SelectedTag {
            get {
                var selectedItem = _tree.SelectedNode;
                if (selectedItem == null) {
                    return null;
                }
                return selectedItem.Tag as MemoTag;
            }
        }

        public TreeNode UntaggedNode {
            get { return _untaggedNode; }
        }

        public int UntaggedImageIndex {
            get { return _untaggedImageIndex; }
            set {
                _untaggedImageIndex = value;
                _untaggedNode.ImageIndex = _untaggedImageIndex;
                _untaggedNode.SelectedImageIndex = _untaggedImageIndex;
            }
        }

        public int TagImageIndex {
            get { return _tagImageIndex; }
            set { _tagImageIndex = value; }
        }

        // ========================================
        // method
        // ========================================
        public void RebuildTree() {
            _root.Nodes.Clear();
            _root.Nodes.Add(_untaggedNode);
            TreeViewUtil.BuildTagTree(_root, _facade.Workspace, _tagImageIndex);
        }

        public void UpdateAdded(MemoTag tag) {
            _tree.BeginUpdate();

            var node = new TreeNodeEx(tag.Name);
            node.Tag = tag;
            node.ImageIndex = _tagImageIndex;
            node.SelectedImageIndex = _tagImageIndex;

            if (tag.SuperTag == null) {
                _root.Nodes.Add(node);
            } else {
                var parentNode = GetNode(tag.SuperTag);
                if (parentNode != null) {
                    parentNode.Nodes.Add(node);
                }
            }

            _tree.EndUpdate();
        }

        public void UpdateRemoving(MemoTag tag) {
            _tree.BeginUpdate();

            var removed = default(TreeNode);
            foreach (var node in new TreeNodeIterator(_root)) {
                if (node.Tag == tag) {
                    removed = node;
                }
            }
            if (removed != null) {
                removed.Remove();
            }

            _tree.EndUpdate();
        }

        public void UpdateChanged(MemoTag tag) {
            _tree.BeginUpdate();

            foreach (var node in new TreeNodeIterator(_root)) {
                if (node.Tag == tag) {
                    if (node.Text != tag.Name) {
                        node.Text = tag.Name;
                        _tree.Sort();
                    }
                    if (
                        node.Parent.Tag != tag.SuperTag &&
                        !(node.Parent == _root && tag.SuperTag == null)
                    ) {
                        node.Remove();
                        if (tag.SuperTag == null) {
                            _root.Nodes.Add(node);
                        } else {
                            var parentNode = GetNode(tag.SuperTag);
                            if (parentNode != null) {
                                parentNode.Nodes.Add(node);
                            }
                        }
                    }
                }
            }

            _tree.EndUpdate();
        }

        public TreeNode GetNode(object obj) {
            if (obj == null) {
                return null;

            } else if (obj == UntaggedObj) {
                return _untaggedNode;

            //} else if (obj == TagRoot) {
            //    return _tagRootNode;

            } else {
                var tag = obj as MemoTag;
                foreach (var node in new TreeNodeIterator(_tree)) {
                    if (node.Tag == tag) {
                        return node;
                    }
                }

                return null;
            }
        }

        public TreeNode SelectNode(object obj) {
            return _tree.SelectedNode = GetNode(obj);
        }

        /// <summary>
        /// すべてのタグならnull，タグ無しなら空のHashSet，それ以外はタグが格納されたHashSetを返す。
        /// </summary>
        public HashSet<MemoTag> GetSelectedTagsAndDescendants() {
            var ret = new HashSet<MemoTag>();

            var selected = _tree.SelectedNode;
            if (selected != null) {
                var obj = selected.Tag;

                if (obj == AllTagsObj) {
                    return null;

                } else if (obj == UntaggedObj) {
                    return ret;

                } else {
                    var tag = obj as MemoTag;
                    if (_findMemoWithDescendants) {
                        var ite = new Iterator<MemoTag>(tag, t => t.SubTags);
                        foreach (var destag in ite) {
                            ret.Add(destag);
                        }
                    } else {
                        ret.Add(tag);
                    }
                }
            }

            return ret;
        }

        public void CreateMemo() {
            if (_tree.SelectedNode == null) {
                return;
            }
            var memoInfo = default(MemoInfo);
            if (IsUntaggedSelected) {
                memoInfo = _facade.CreateMemo();
            } else {
                var selected = SelectedTag;
                if (selected != null) {
                    memoInfo = _facade.CreateMemo();
                    if (memoInfo != null) {
                        var memo = _facade.Container.Find<Memo>(memoInfo.MemoId);
                        selected.AddMemo(memo);
                    }
                }
            }
        }

        public void CreateTag(string name) {
            //if (_untaggedNode == null) {
            //    RebuildTree();
                _root.Expand();
            //}

            var selectedNode = _tree.SelectedNode;

            var createdTag = _facade.Workspace.CreateTag();
            if (createdTag == null) {
                return;
            }

            createdTag.Name = name;

            if (selectedNode != null && selectedNode.Tag is MemoTag) {
                var super = (MemoTag) selectedNode.Tag;
                if (super != null && super != createdTag) {
                    createdTag.SuperTag = super;
                }
            }

            //RebuildTree();
            var createdNode = SelectNode(createdTag);
            if (createdNode != null) {
                createdNode.BeginEdit();
            }
        }

        public void CreateTag() {
            CreateTag(NewTagText);
        }

        public void RemoveTag() {
            if (!IsTagSelected) {
                return;
            }
            var node = _tree.SelectedNode;
            var parentTag = node.Parent.Tag as MemoTag;

            var tag = node.Tag as MemoTag;
            if (tag == null) {
                return;
            }

            if (!MessageUtil.ConfirmTagRemoval(tag)) {
                return;
            }

            _facade.Workspace.RemoveTag(tag);
            node.Remove();
            //RebuildTree();

            if (parentTag == null) {
                _tree.SelectedNode = _root;
            } else {
                SelectNode(parentTag);
            }
        }

        public void RenameTag() {
            var selectedNode = _tree.SelectedNode;
            var selectedTag = selectedNode == null? null: selectedNode.Tag as MemoTag;
            
            if (selectedTag != null) {
                selectedNode.BeginEdit();
            }
        }

        public static bool IsChildNode(TreeNode parent, TreeNode child) {
            if (child.Parent == parent) {
                return true;
            } else if (child.Parent != null) {
                return IsChildNode(parent, child.Parent);
            } else {
                return false;
            }
        }
    }
}
