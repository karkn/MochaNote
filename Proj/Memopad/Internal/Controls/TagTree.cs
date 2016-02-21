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
using Mkamo.Model.Memo;
using Mkamo.Memopad.Internal.Utils;
using Mkamo.Common.Forms.TreeView;
using System.Drawing;
using System.ComponentModel;
using System.Collections;
using Mkamo.Common.Collection;
using Mkamo.Memopad.Core;
using Mkamo.Common.Forms.Mouse;
using Mkamo.Control.TreeNodeEx;

namespace Mkamo.Memopad.Internal.Controls {
    [ToolboxItem(false)]
    internal class TagTree: TreeView {
        // ========================================
        // static field
        // ========================================
        private const string NewTagText = "新しいタグ";
        private const string AllTagsText = "すべて";
        private const string UntaggedText = "未整理";
        private const string TagRootText = "タグ";

        private static readonly object AllTags = new object();
        private static readonly object Untagged = new object();
        private static readonly object TagRoot = new object();

        // ========================================
        // field
        // ========================================
        private ImageList _imageList;
        private IContainer components;

        private MemopadApplication _facade;

        private bool _isAllTagsEnabled;
        private bool _isUntaggedEnabled;

        private TreeNode _tagRootNode;
        private TreeNode _allTagsNode;
        private TreeNode _untaggedNode;

        // --- label edit state ---
        private bool _isInProcessingAfterLabelEdit;

        // --- dnd state ---
        private bool _isInDragOver;
        private object _selectedObjBeforeDragOver;

        // ========================================
        // constructor
        // ========================================
        public TagTree() {
            _facade = MemopadApplication.Instance;

            InitializeComponent();

            AllowDrop = true;
            LabelEdit = true;
            TreeViewNodeSorter = new TagTreeNodeComparer();
        }

        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TagTree));
            this._imageList = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // _imageList
            // 
            this._imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer) (resources.GetObject("_imageList.ImageStream")));
            this._imageList.TransparentColor = System.Drawing.Color.Transparent;
            this._imageList.Images.SetKeyName(0, "tag-label.png");
            this._imageList.Images.SetKeyName(1, "tags-label.png");
            this._imageList.Images.SetKeyName(2, "inbox.png");
            // 
            // TagTree
            // 
            this.ImageIndex = 0;
            this.ImageList = this._imageList;
            this.SelectedImageIndex = 0;
            this.ResumeLayout(false);

        }

        // ========================================
        // event
        // ========================================
        public event EventHandler Updated;


        // ========================================
        // property
        // ========================================
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsAllTagsEnabled {
            get { return _isAllTagsEnabled; }
            set { _isAllTagsEnabled = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsUntaggedEnabled {
            get { return _isUntaggedEnabled; }
            set { _isUntaggedEnabled = value; }
        }


        public bool IsTagRootSelected {
            get { return SelectedNode == _tagRootNode; }
        }

        public bool IsAllTagsSelected {
            get { return SelectedNode == _allTagsNode; }
        }

        public bool IsUntaggedSelected {
            get { return SelectedNode == _untaggedNode; }
        }

        public bool IsTagSelected {
            get { return SelectedTag != null; }
        }

        public MemoTag SelectedTag {
            get {
                var selectedItem = SelectedNode;
                if (selectedItem == null) {
                    return null;
                }
                return selectedItem.Tag as MemoTag;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsUntaggedChecked {
            get { return _untaggedNode == null? false: _untaggedNode.Checked; }
            set {
                if (_untaggedNode == null) {
                    return;
                }
                _untaggedNode.Checked = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerable<MemoTag> CheckedTags {
            get {
                var ret = new List<MemoTag>();
                if (_tagRootNode == null) {
                    return ret;
                }

                var ite = new TreeNodeIterator(_tagRootNode);
                foreach (var node in ite) {
                    if (node.Checked) {
                        var tag = node.Tag as MemoTag;
                        if (tag != null) {
                            ret.Add(tag);
                        }
                    }
                }

                return ret;
            }
            set {
                if (value == null) {
                    return;
                }
                foreach (var tag in value) {
                    SetNodeChecked(tag, true);
                }
            }
        }

        public TreeNode UntaggedNode {
            get { return _untaggedNode; }
        }

        public TreeNode AllTagsNode {
            get { return _allTagsNode; }
        }

        public TreeNode TagRootNode {
            get { return _tagRootNode; }
        }

        public bool IsInDragOver {
            get { return _isInDragOver; }
        }

        // ========================================
        // method
        // ========================================
        // ------------------------------
        // public
        // ------------------------------
        public void UpdateTags() {
            var selectedNode = SelectedNode;
            var selectedObj = selectedNode == null? null: selectedNode.Tag;

            BeginUpdate();
            Nodes.Clear();

            if (_isAllTagsEnabled) {
                _allTagsNode = Nodes.Add(AllTagsText);
                _allTagsNode.Tag = AllTags;
                _allTagsNode.ImageIndex = 1;
                _allTagsNode.SelectedImageIndex = 1;
            }

            if (_isUntaggedEnabled) {
                _untaggedNode = Nodes.Add(UntaggedText);
                _untaggedNode.Tag = Untagged;
                _untaggedNode.ImageIndex = 2;
                _untaggedNode.SelectedImageIndex = 2;
            }

            _tagRootNode = Nodes.Add(TagRootText);
            _tagRootNode.Tag = TagRoot;
            _tagRootNode.ImageIndex = 1;
            _tagRootNode.SelectedImageIndex = 1;

            TreeViewUtil.UpdateTagTreeView(_tagRootNode, _facade.Workspace);

            _tagRootNode.Expand();

            Sort();
            EndUpdate();

            SelectNode(selectedObj);

            OnUpdated();
        }

        public TreeNode SelectNode(object obj) {
            if (obj == null) {
                return SelectedNode = null;

            } else if (obj == Untagged) {
                return SelectedNode = _untaggedNode;

            } else if (obj == AllTags) {
                return SelectedNode = _allTagsNode;

            } else if (obj == TagRoot) {
                return SelectedNode = _tagRootNode;

            } else {
                var tag = obj as MemoTag;
                foreach (var node in new TreeNodeIterator(this)) {
                    if (node.Tag == tag) {
                        SelectedNode = node;
                        return node;
                    }
                }

                return SelectedNode = _untaggedNode;
            }
        }

        public TreeNode SetNodeChecked(object obj, bool value) {
            if (obj == null) {
                return null;

            } else if (obj == Untagged) {
                _untaggedNode.Checked = value;
                return _untaggedNode;

            } else if (obj == AllTags) {
                _allTagsNode.Checked = value;
                return _allTagsNode;

            } else if (obj == TagRoot) {
                _tagRootNode.Checked = value;
                return _tagRootNode;

            } else {
                var tag = obj as MemoTag;
                foreach (var node in new TreeNodeIterator(this)) {
                    if (node.Tag == tag) {
                        node.Checked = value;
                        return node;
                    }
                }

                return null;
            }
        }


        //private TreeNode FindTreeNode(MemoTag tag) {
        //    var ite = new TreeNodeIterator(_tagTreeView);
        //    foreach (var node in ite) {
        //        if (node.Tag == tag) {
        //            return node;
        //        }
        //    }
        //    return null;
        //}

        public HashSet<MemoTag> GetSelectedTagsAndDescendants() {
            var ret = new HashSet<MemoTag>();

            var selected = SelectedNode;
            if (selected != null) {
                var obj = selected.Tag;

                if (obj == AllTags || obj == TagRoot) {
                    return null;

                } else if (obj == Untagged) {
                    return ret;

                } else {
                    var tag = obj as MemoTag;
                    var ite = new Iterator<MemoTag>(tag, t => t.SubTags);
                    foreach (var destag in ite) {
                        ret.Add(destag);
                    }
                }
            }

            return ret;
        }

        public void CreateTag() {
            var name = NewTagText;
            
            var createdTag = _facade.Workspace.CreateTag();
            createdTag.Name = name;

            var selectedNode = SelectedNode;
            if (selectedNode != null && selectedNode != _tagRootNode) {
                var super = (MemoTag) selectedNode.Tag;
                if (super != null && super != createdTag) {
                    createdTag.SuperTag = super;
                }
            }

            UpdateTags();
            var createdNode = SelectMemoTag(createdTag);
            if (createdNode != null) {
                createdNode.BeginEdit();
            }
        }

        public void RemoveTag() {
            var node = SelectedNode;
            if (node == null || node == _tagRootNode) {
                return;
            }
            var parentTag = node.Parent.Tag as MemoTag;

            var tag = node.Tag as MemoTag;
            if (tag == null) {
                return;
            }

            if (!MessageUtil.ConfirmTagRemoval(tag)) {
                return;
            }

            _facade.Workspace.RemoveTag(tag);

            UpdateTags();
            if (parentTag == null) {
                SelectedNode = _tagRootNode;
            } else {
                SelectMemoTag(parentTag);
            }
        }

        public void EditTag() {
            var selectedNode = SelectedNode;
            var selectedTag = selectedNode == null? null: selectedNode.Tag as MemoTag;
            
            if (selectedTag != null) {
                selectedNode.BeginEdit();
            }
        }


        // ------------------------------
        // protected
        // ------------------------------
        protected override void OnBeforeLabelEdit(NodeLabelEditEventArgs e) {
            base.OnBeforeLabelEdit(e);

            if (_isInProcessingAfterLabelEdit) {
                e.CancelEdit = true;
            }
            if (e.Node == _tagRootNode || e.Node == _allTagsNode || e.Node == _untaggedNode) {
                e.CancelEdit = true;
            }
        }

        protected override void OnAfterLabelEdit(NodeLabelEditEventArgs e) {
            base.OnAfterLabelEdit(e);

            if (e.Label == null) {
                return;
            }

            var tag = e.Node.Tag as MemoTag;
            if (tag != null) {
                /// _tagTreeNodeのNodeを変更するような処理をする場合，
                /// BeginInvoke()を通さないと変更時点でまたBegin/AfterLabelEditされてしまう
                BeginInvoke(
                    (Action) (() => {
                        _isInProcessingAfterLabelEdit = true;
                        try {
                            tag.Name = e.Label;
                            UpdateTags();
                            if (tag != null) {
                                SelectMemoTag(tag);
                            }
                        } finally {
                            _isInProcessingAfterLabelEdit = false;
                        }
                    })
                );
            }
        }

        protected override void OnItemDrag(ItemDragEventArgs e) {
            base.OnItemDrag(e);

            SelectedNode = (TreeNode) e.Item;
            Focus();

            var effects = DoDragDrop(e.Item, DragDropEffects.Move);
            if ((effects & DragDropEffects.Move) == DragDropEffects.Move) {
                var dragged = (TreeNode) e.Item;
                Nodes.Remove(dragged);
            }
        }

        protected override void OnDragOver(DragEventArgs e) {
            base.OnDragOver(e);

            e.Effect = DragDropEffects.None;

            /// まずdataとeffectで判断
            if (e.Data.GetDataPresent(typeof(TreeNode)) || e.Data.GetDataPresent(typeof(TreeNodeEx))) {
                if (DragDropUtil.IsMoveAllowed(e)) {
                    var target = GetNodeAt(PointToClient(new Point(e.X, e.Y)));
                    var source = (TreeNode) e.Data.GetData(typeof(TreeNode));
                    if (source == null) {
                        source = (TreeNode) e.Data.GetData(typeof(TreeNodeEx));
                    }
    
                    if (!_isInDragOver) {
                        _isInDragOver = true;
                        _selectedObjBeforeDragOver = SelectedNode == null? null: SelectedNode.Tag;
                    }

                    /// 表示のための処理
                    if (target != null) {
                        SelectedNode = target;
                        if (target.PrevVisibleNode != null) {
                            target.PrevVisibleNode.EnsureVisible();
                        }
                        if (target.NextVisibleNode != null) {
                            target.NextVisibleNode.EnsureVisible();
                        }
                    }

                    ///マウス下のNodeがドロップ先として適切か調べる
                    if (
                        target != null &&
                        target != source &&
                        target != source.Parent &&
                        !IsChildNode(source, target)
                    ) {
                        e.Effect = DragDropEffects.Move;
                    }
                }

            } else if (e.Data.GetDataPresent(typeof(MemoInfo[]))) {
                if (DragDropUtil.IsMoveAllowed(e) || DragDropUtil.IsCopyAllowed(e)) {
                    var target = GetNodeAt(PointToClient(new Point(e.X, e.Y)));

                    if (!_isInDragOver) {
                        _isInDragOver = true;
                        _selectedObjBeforeDragOver = SelectedNode == null? null: SelectedNode.Tag;
                    }

                    /// 表示のための処理
                    if (target != null) {
                        SelectedNode = target;
                        if (target.PrevVisibleNode != null) {
                            target.PrevVisibleNode.EnsureVisible();
                        }
                        if (target.NextVisibleNode != null) {
                            target.NextVisibleNode.EnsureVisible();
                        }
                    }

                    ///マウス下のNodeがドロップ先として適切か調べる
                    if (
                        target != null &&
                        target != _allTagsNode &&
                        target != _tagRootNode &&
                        target != _untaggedNode
                    ) {
                        if (DragDropUtil.IsControlPressed(e)) {
                            e.Effect = DragDropEffects.Copy;
                        } else {
                            e.Effect = DragDropEffects.Move;
                        }
                    }
                }
            }

            if (e.Effect == DragDropEffects.None) {
                SelectNode(_selectedObjBeforeDragOver);
                _isInDragOver = false;
            }
        }

        protected override void OnDragDrop(DragEventArgs e) {
            base.OnDragDrop(e);

            var effect = DragDropEffects.None;

            if (e.Data.GetDataPresent(typeof(TreeNode)) || e.Data.GetDataPresent(typeof(TreeNodeEx))) {
                var target = GetNodeAt(PointToClient(new Point(e.X, e.Y)));
                var source = (TreeNode) e.Data.GetData(typeof(TreeNode));
                if (source == null) {
                    source = (TreeNode) e.Data.GetData(typeof(TreeNodeEx));
                }

                if (
                    DragDropUtil.IsMove(e) &&
                    target != null &&
                    target != source &&
                    target != source.Parent &&
                    !IsChildNode(source, target)
                ) {
                    /// ドロップされたNodeのコピーを作成
                    var clone = (TreeNode) source.Clone();
                    target.Nodes.Add(clone);
                    target.Expand();
                    SelectedNode = clone;

                    var draggedTag = (MemoTag) clone.Tag;
                    draggedTag.SuperTag = null;
                    if (target != _tagRootNode) {
                        var super = target.Tag as MemoTag;
                        if (super != null) {
                            draggedTag.SuperTag = super;
                        }
                    }
                    _isInDragOver = false;
                    _selectedObjBeforeDragOver = null;
                }

            } else if (e.Data.GetDataPresent(typeof(MemoInfo[]))) {
                if (DragDropUtil.IsMove(e) || DragDropUtil.IsCopy(e)) {

                    _isInDragOver = false;
                    SelectNode(_selectedObjBeforeDragOver);
                    _selectedObjBeforeDragOver = null;

                    var target = GetNodeAt(PointToClient(new Point(e.X, e.Y)));
                    var addingTag = target.Tag as MemoTag;
                    var memoInfos = (MemoInfo[]) e.Data.GetData(typeof(MemoInfo[]));
                    foreach (var memoInfo in memoInfos) {
                        var memo = _facade.Container.Find<Memo>(memoInfo.MemoId);

                        if (DragDropUtil.IsMove(e)) {
                            memo.ClearTags();
                        }
                        if (!memo.Tags.Contains(addingTag)) {
                            memo.AddTag(addingTag);
                        }
                    }

                }
            }

            if (effect == DragDropEffects.None) {
                _isInDragOver = false;
                SelectNode(_selectedObjBeforeDragOver);
                _selectedObjBeforeDragOver = null;
            }
        }

        protected override void OnDragLeave(EventArgs e) {
            base.OnDragLeave(e);

            SelectNode(_selectedObjBeforeDragOver);
            _isInDragOver = false;
            _selectedObjBeforeDragOver = null;
        }

        protected virtual void OnUpdated() {
            var handler = Updated;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        // ------------------------------
        // private
        // ------------------------------
        private TreeNode SelectMemoTag(MemoTag tag) {
            foreach (var node in new TreeNodeIterator(this)) {
                if (node.Tag == tag) {
                    SelectedNode = node;
                    return node;
                }
            }
            return null;
        }


        private static bool IsChildNode(TreeNode parent, TreeNode child) {
            if (child.Parent == parent) {
                return true;
            } else if (child.Parent != null) {
                return IsChildNode(parent, child.Parent);
            } else {
                return false;
            }
        }

        // ========================================
        // class
        // ========================================
        /// <summary>
        /// 未整理，すべてを最後に，それ以外は辞書順にソートするcomparer．
        /// </summary>
        private class TagTreeNodeComparer: IComparer {
            public int Compare(object x, object y) {
                var xNode = (TreeNode) x;
                var yNode = (TreeNode) y;

                var xObj = xNode.Tag;
                var yObj = yNode.Tag;

                if (xObj == AllTags) {
                    return -1;
                } else if (xObj == Untagged) {
                    if (yObj == AllTags) {
                        return 1;
                    } else if (yObj == Untagged) {
                        return 0;
                    } else {
                        return -1;
                    }
                } else {
                    if (yObj == AllTags || yObj == Untagged) {
                        return 1;
                    }
                    return xNode.Text.CompareTo(yNode.Text);
                }

            }
        }

    }
}
