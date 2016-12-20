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
using System.Drawing;
using Mkamo.Control.TreeNodeEx;
using Mkamo.Model.Memo;
using Mkamo.Common.Forms.TreeView;
using Mkamo.Memopad.Core;
using System.Collections;
using Mkamo.Common.Core;
using Mkamo.Common.Forms.Mouse;
using Mkamo.Model.Core;
using System.ComponentModel;
using Mkamo.Common.Event;
using Mkamo.Memopad.Internal.Utils;

namespace Mkamo.Memopad.Internal.Controls
{
    [ToolboxItem(false)]
    internal class WorkspaceTree : TreeView
    {
        // ========================================
        // static field
        // ========================================
        private static readonly object SmartFolderCategoryObj = new object();
        private static readonly object TagCategoryObj = new object();
        private static readonly object FolderCategoryObj = new object();
        private static readonly object TrashBoxObj = new object();
        private static readonly object AllMemosObj = new object();
        private static readonly object OpenMemosObj = new object();

        private static readonly object[] SpecialObjects = new[] {
            AllMemosObj,
            SmartFolderCategoryObj,
            FolderCategoryObj,
            TagCategoryObj,
            TrashBoxObj,
            OpenMemosObj,
        };

        // ========================================
        // field
        // ========================================
        private MemopadApplication _facade;

        private ImageList _imageList;
        private IContainer components;

        private bool _showAllMemos;
        private bool _showSmartFolder;
        private bool _showFolder;
        private bool _showTrashBox;
        private bool _showOpenMemos;
        private bool _allowEdit;

        private Workspace _workspace;
        private TreeNodeEx _folderCategoryNode;
        private TreeNodeEx _smartFolderCategoryNode;
        private TreeNodeEx _tagCategoryNode;
        private TreeNodeEx _trashBoxNode;
        private TreeNodeEx _openMemosNode;
        private TreeNodeEx _allMemosNode;

        private ContextMenuStrip _commonContextMenuStrip;
        private ContextMenuStrip _folderContextMenuStrip;
        private ContextMenuStrip _smartFolderContextMenuStrip;
        private ContextMenuStrip _tagContextMenuStrip;

        private SmartFolderTreePresenter _smartFolderTreePresenter;
        private TagTreePresenter _tagTreePresenter;
        private FolderTreePresenter _folderTreePresenter;
        private TrashBoxPresenter _trashBoxPresenter;
        private AllMemosPresenter _allMemosPresenter;
        private OpenMemosPresenter _openMemosPresenter;

        // --- conversation state ---
        private bool _isInProcessingAfterLabelEdit; /// label edit
        private bool _isRightMouseDown; /// context menu

        private bool _isInDragOver; /// dnd
        private object _selectedObjBeforeDragOver; /// dnd

        // --- dnd ---
        private bool _isDragPrepared;
        private Point _dragStartPoint;
        private Rectangle _dragStartRect;

        // ========================================
        // constructor
        // ========================================
        public WorkspaceTree()
        {
            _facade = MemopadApplication.Instance;
            _workspace = _facade.Workspace;

            InitializeComponent();

            _showAllMemos = true;// false;
            _showSmartFolder = false;
            _showFolder = false;
            _showTrashBox = true;
            _showOpenMemos = true;
            _allowEdit = true;

            _isInProcessingAfterLabelEdit = false;
            _isRightMouseDown = false;
            _isInDragOver = false;
            _selectedObjBeforeDragOver = null;

            ItemHeight = (int)(ItemHeight * 1.2);
            AllowDrop = true;
            HideSelection = false;
            LabelEdit = true;
            Sorted = true;
            ShowRootLines = true;
            FullRowSelect = true;
            ShowLines = false;
            TreeViewNodeSorter = new WorkspaceTreeNodeComparer(this);

            //_facade.MemoInfoAdded += HandleFacadeMemoInfoAdded;
            //_facade.MemoInfoRemoving += HandleFacadeMemoInfoRemoving;
            //_facade.MemoInfoChanged += HandleFacadeMemoInfoChanged;

            _facade.ActiveFolderChanging += HandleFacadeActiveFolderChanging;
            _facade.ActiveFolderChanged += HandleFacadeActiveFolderChanged;

            //_facade.Workspace.MemoFolderChanged += HandleMemoFolderChanged;
            //_facade.Workspace.MemoTagAdded += HandleMemoTagAdded;
            //_facade.Workspace.MemoTagRemoving += HandleMemoTagRemoving;


        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WorkspaceTree));
            this._imageList = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // _imageList
            // 
            this._imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_imageList.ImageStream")));
            this._imageList.TransparentColor = System.Drawing.Color.Magenta;
            this._imageList.Images.SetKeyName(0, "desktop.png");
            this._imageList.Images.SetKeyName(1, "clear_folder_horizontal.png");
            this._imageList.Images.SetKeyName(2, "clear_folder_horizontal_open.png");
            this._imageList.Images.SetKeyName(3, "document-search-result.png");
            this._imageList.Images.SetKeyName(4, "inbox.png");
            this._imageList.Images.SetKeyName(5, "tag-label.png");
            this._imageList.Images.SetKeyName(6, "tags-label.png");
            this._imageList.Images.SetKeyName(7, "bin.png");
            this._imageList.Images.SetKeyName(8, "folder-search-result.png");
            this._imageList.Images.SetKeyName(9, "sticky-notes.png");
            this._imageList.Images.SetKeyName(10, "sticky_note_pencil.png");
            // 
            // WorkspaceTree
            // 
            this.ImageIndex = 0;
            this.ImageList = this._imageList;
            this.SelectedImageIndex = 0;
            this.ResumeLayout(false);

        }

        // ========================================
        // destructor
        // ========================================
        protected override void Dispose(bool disposing)
        {
            CleanUp();
            base.Dispose(disposing);
        }

        private void CleanUp()
        {
            //_facade.MemoInfoAdded -= HandleFacadeMemoInfoAdded;
            //_facade.MemoInfoRemoving -= HandleFacadeMemoInfoRemoving;
            //_facade.MemoInfoChanged -= HandleFacadeMemoInfoChanged;

            _facade.ActiveFolderChanging -= HandleFacadeActiveFolderChanging;
            _facade.ActiveFolderChanged -= HandleFacadeActiveFolderChanged;

            //_facade.Workspace.MemoFolderChanged -= HandleMemoFolderChanged;
            //_facade.Workspace.MemoTagAdded -= HandleMemoTagAdded;
            //_facade.Workspace.MemoTagRemoving -= HandleMemoTagRemoving;

            //_facade.Workspace.MemoTagChanged -= HandleMemoTagChanged;
            //_facade.Workspace.MemoTagAdded -= HandleMemoTagAdded;
            //_facade.Workspace.MemoTagRemoving -= HandleMemoTagRemoving;
        }

        // ========================================
        // property
        // ========================================
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ContextMenuStrip CommonContextMenuStrip
        {
            get { return _commonContextMenuStrip; }
            set { _commonContextMenuStrip = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ContextMenuStrip FolderContextMenuStrip
        {
            get { return _folderContextMenuStrip; }
            set { _folderContextMenuStrip = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ContextMenuStrip SmartFolderContextMenuStrip
        {
            get { return _smartFolderContextMenuStrip; }
            set { _smartFolderContextMenuStrip = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ContextMenuStrip TagContextMenuStrip
        {
            get { return _tagContextMenuStrip; }
            set { _tagContextMenuStrip = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowAllMemos
        {
            get { return _showAllMemos; }
            set { _showAllMemos = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowSmartFolder
        {
            get { return _showSmartFolder; }
            set { _showSmartFolder = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowFolder
        {
            get { return _showFolder; }
            set { _showFolder = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowTrashBox
        {
            get { return _showTrashBox; }
            set { _showTrashBox = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowOpenMemos
        {
            get { return _showOpenMemos; }
            set { _showOpenMemos = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool AllowEdit
        {
            get { return _allowEdit; }
            set { _allowEdit = value; }
        }

        public IMemoInfoListProvider CurrentMemoInfoListProvider
        {
            get
            {
                if (IsSmartFolderSelected)
                {
                    return _smartFolderTreePresenter;
                }
                else if (IsTagSelected || IsUntaggedSelected)
                {
                    return _tagTreePresenter;
                }
                else if (IsFolderSelected)
                {
                    return _folderTreePresenter;
                }
                else if (IsTrashBoxSelected)
                {
                    return _trashBoxPresenter;
                }
                else if (IsAllMemosSelected)
                {
                    return _allMemosPresenter;
                }
                else if (IsOpenMemosSelected)
                {
                    return _openMemosPresenter;
                }

                return null;
            }
        }

        public SmartFolderTreePresenter SmartFolderTreePresenter
        {
            get { return _smartFolderTreePresenter; }
        }

        public TagTreePresenter TagTreePresenter
        {
            get { return _tagTreePresenter; }
        }

        public FolderTreePresenter FolderTreePresenter
        {
            get { return _folderTreePresenter; }
        }

        public bool IsFolderSelected
        {
            get
            {
                if (SelectedNode == null)
                {
                    return false;
                }
                return SelectedNode.Tag is MemoFolder;
            }
        }

        public bool IsSmartFolderSelected
        {
            get
            {
                if (SelectedNode == null)
                {
                    return false;
                }
                return SelectedNode.Tag is MemoSmartFolder;
            }
        }

        public bool IsTagSelected
        {
            get
            {
                if (SelectedNode == null)
                {
                    return false;
                }
                return SelectedNode.Tag is MemoTag;
            }
        }

        public bool IsUntaggedSelected
        {
            get { return _tagTreePresenter.IsUntaggedSelected; }
        }

        public bool IsCategorySelected
        {
            get { return IsFolderCategorySelected || IsSmartFolderCategorySelected || IsTagCategorySelected; }
        }

        public bool IsFolderCategorySelected
        {
            get { return SelectedNode == _folderCategoryNode; }
        }

        public bool IsSmartFolderCategorySelected
        {
            get { return SelectedNode == _smartFolderCategoryNode; }
        }

        public bool IsTagCategorySelected
        {
            get { return SelectedNode == _tagCategoryNode; }
        }

        public bool IsTrashBoxSelected
        {
            get { return SelectedNode == _trashBoxNode; }
        }

        public bool IsAllMemosSelected
        {
            get { return _showAllMemos && SelectedNode == _allMemosNode; }
        }

        public bool IsOpenMemosSelected
        {
            get { return SelectedNode == _openMemosNode; }
        }

        public MemoFolder SelectedFolder
        {
            get { return SelectedNode == null ? null : SelectedNode.Tag as MemoFolder; }
        }

        public MemoTag SelectedTag
        {
            get { return SelectedNode == null ? null : SelectedNode.Tag as MemoTag; }
        }

        public TreeNode AllMemosNode
        {
            get { return _allMemosNode; }
        }

        public bool IsInDragOver
        {
            get { return _isInDragOver; }
        }

        // ------------------------------
        // internal
        // ------------------------------
        internal OpenMemosPresenter _OpenMemosPresenter
        {
            get { return _openMemosPresenter; }
        }

        internal TreeNodeEx _OpenMemosNode
        {
            get { return _openMemosNode; }
        }

        internal TreeNodeEx _TagCategoryNode
        {
            get { return _tagCategoryNode; }
        }

        // ========================================
        // method
        // ========================================
        public void RebuildTree()
        {
            BeginUpdate();

            if (_showAllMemos && _allMemosNode == null)
            {
                _allMemosNode = new TreeNodeEx("すべてのノート");
                _allMemosNode.Tag = AllMemosObj;
                _allMemosNode.ImageIndex = 9;
                _allMemosNode.SelectedImageIndex = 9;
                Nodes.Add(_allMemosNode);

                _allMemosPresenter = new AllMemosPresenter();
            }

            if (_smartFolderCategoryNode == null)
            {
                _smartFolderCategoryNode = new TreeNodeEx("スマートフォルダ");
                _smartFolderCategoryNode.Tag = SmartFolderCategoryObj;
                _smartFolderCategoryNode.ImageIndex = 8;
                _smartFolderCategoryNode.SelectedImageIndex = 8;
                if (_showSmartFolder)
                {
                    Nodes.Add(_smartFolderCategoryNode);
                }

                _smartFolderTreePresenter = new SmartFolderTreePresenter(this, _smartFolderCategoryNode);
                _smartFolderTreePresenter.SmartFolderImageIndex = 8;
            }

            if (_folderCategoryNode == null)
            {
                _folderCategoryNode = new TreeNodeEx("クリアファイル");
                _folderCategoryNode.Tag = FolderCategoryObj;
                _folderCategoryNode.ImageIndex = 1;
                _folderCategoryNode.SelectedImageIndex = 1;
                if (_showFolder)
                {
                    Nodes.Add(_folderCategoryNode);
                }

                _folderTreePresenter = new FolderTreePresenter(this, _folderCategoryNode);
                _folderTreePresenter.FolderImageIndex = 1;
                _folderTreePresenter.ActiveFolderImageIndex = 2;
            }

            if (_tagCategoryNode == null)
            {
                _tagCategoryNode = new TreeNodeEx("タグ");
                _tagCategoryNode.Tag = TagCategoryObj;
                _tagCategoryNode.ImageIndex = 6;
                _tagCategoryNode.SelectedImageIndex = 6;
                Nodes.Add(_tagCategoryNode);

                _tagTreePresenter = new TagTreePresenter(this, _tagCategoryNode, true);
                _tagTreePresenter.UntaggedImageIndex = 4;
                _tagTreePresenter.TagImageIndex = 5;
            }

            if (_showTrashBox && _trashBoxNode == null)
            {
                _trashBoxNode = new TreeNodeEx("ごみ箱");
                _trashBoxNode.Tag = TrashBoxObj;
                _trashBoxNode.ImageIndex = 7;
                _trashBoxNode.SelectedImageIndex = 7;
                Nodes.Add(_trashBoxNode);

                _trashBoxPresenter = new TrashBoxPresenter();
            }

            if (_showOpenMemos && _openMemosNode == null)
            {
                _openMemosNode = new TreeNodeEx("開いているノート");
                _openMemosNode.Tag = OpenMemosObj;
                _openMemosNode.ImageIndex = 10;
                _openMemosNode.SelectedImageIndex = 10;
                Nodes.Add(_openMemosNode);

                _openMemosPresenter = new OpenMemosPresenter();
            }

            /// +アイコンを出すためのダミー
            _smartFolderCategoryNode.Nodes.Add(new TreeNodeEx("_dummy"));
            _folderCategoryNode.Nodes.Add(new TreeNodeEx("_dummy"));
            _tagCategoryNode.Nodes.Add(new TreeNodeEx("_dummy"));

            _tagCategoryNode.Expand();

            EndUpdate();
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
        {
            base.OnBeforeExpand(e);

            var node = (TreeNodeEx)e.Node;
            BeginUpdate();
            UpdateSubNodes(node);
            EndUpdate();
        }

        protected override void OnBeforeLabelEdit(NodeLabelEditEventArgs e)
        {
            base.OnBeforeLabelEdit(e);

            if (!_allowEdit)
            {
                e.CancelEdit = true;
                return;
            }
            if (_isInProcessingAfterLabelEdit)
            {
                e.CancelEdit = true;
                return;
            }
            if (
                e.Node == _folderCategoryNode || e.Node == _smartFolderCategoryNode ||
                e.Node == _tagCategoryNode || e.Node == _tagTreePresenter.UntaggedNode ||
                e.Node == _trashBoxNode || e.Node == _allMemosNode || e.Node == _openMemosNode
            )
            {
                e.CancelEdit = true;
                return;
            }
        }

        protected override void OnAfterLabelEdit(NodeLabelEditEventArgs e)
        {
            base.OnAfterLabelEdit(e);

            if (!_allowEdit)
            {
                e.CancelEdit = true;
                return;
            }
            if (string.IsNullOrEmpty(e.Label))
            {
                e.CancelEdit = true;
                return;
            }

            if (e.Node.Tag is MemoTag)
            {
                var tag = e.Node.Tag as MemoTag;
                if (tag != null)
                {
                    /// _folderTreeNodeのNodeを変更するような処理をする場合，
                    /// BeginInvoke()を通さないと変更時点でまたBegin/AfterLabelEditされてしまう
                    BeginInvoke(
                        (Action)(() =>
                        {
                            _isInProcessingAfterLabelEdit = true;
                            try
                            {
                                tag.Name = e.Label;
                                Sort();
                                if (tag != null)
                                {
                                    SelectNode(tag);
                                }
                            }
                            finally
                            {
                                _isInProcessingAfterLabelEdit = false;
                            }
                        })
                    );
                }
            }
            if (e.Node.Tag is MemoFolder)
            {
                var folder = e.Node.Tag as MemoFolder;
                if (folder != null)
                {
                    /// _folderTreeNodeのNodeを変更するような処理をする場合，
                    /// BeginInvoke()を通さないと変更時点でまたBegin/AfterLabelEditされてしまう
                    BeginInvoke(
                        (Action)(() =>
                        {
                            _isInProcessingAfterLabelEdit = true;
                            try
                            {
                                folder.Name = e.Label;
                                Sort();
                                if (folder != null)
                                {
                                    SelectNode(folder);
                                }
                            }
                            finally
                            {
                                _isInProcessingAfterLabelEdit = false;
                            }
                        })
                    );
                }
            }
            if (e.Node.Tag is MemoSmartFolder)
            {
                var smartFolder = e.Node.Tag as MemoSmartFolder;
                if (smartFolder != null)
                {
                    /// _folderTreeNodeのNodeを変更するような処理をする場合，
                    /// BeginInvoke()を通さないと変更時点でまたBegin/AfterLabelEditされてしまう
                    BeginInvoke(
                        (Action)(() =>
                        {
                            _isInProcessingAfterLabelEdit = true;
                            try
                            {
                                smartFolder.Name = e.Label;
                                Sort();
                                if (smartFolder != null)
                                {
                                    SelectNode(smartFolder);
                                }
                            }
                            finally
                            {
                                _isInProcessingAfterLabelEdit = false;
                            }
                        })
                    );
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                var node = GetNodeAt(e.X, e.Y);
                if (node != null)
                {
                    SetUpDragState(new Point(e.X, e.Y));
                }
                else
                {
                    ClearDragState();
                }
            }
            else
            {
                if (e.Button == MouseButtons.Right)
                {
                    SelectedNode = GetNodeAt(e.X, e.Y);
                    _isRightMouseDown = true;
                }
                else
                {
                    _isRightMouseDown = false;
                }
                ClearDragState();
            }
        }


        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (_isRightMouseDown)
            {
                if (
                    IsFolderCategorySelected ||
                    IsSmartFolderCategorySelected ||
                    IsTagCategorySelected ||
                    IsTrashBoxSelected
                // IsOpenMemosSelected 表示するものがない
                )
                {
                    if (_commonContextMenuStrip != null)
                    {
                        _commonContextMenuStrip.Show(this, new Point(e.X, e.Y));
                    }
                }
                else
                {
                    if (IsSmartFolderSelected)
                    {
                        if (_smartFolderContextMenuStrip != null)
                        {
                            _smartFolderContextMenuStrip.Show(this, new Point(e.X, e.Y));
                        }
                    }
                    else if (IsTagSelected || IsUntaggedSelected)
                    {
                        if (_tagContextMenuStrip != null)
                        {
                            _tagContextMenuStrip.Show(this, new Point(e.X, e.Y));
                        }
                    }
                    else if (IsFolderSelected)
                    {
                        if (_folderContextMenuStrip != null)
                        {
                            _folderContextMenuStrip.Show(this, new Point(e.X, e.Y));
                        }
                    }
                }

                _isRightMouseDown = false;
            }
            ClearDragState();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_isDragPrepared)
            {
                if (!_dragStartRect.Contains(e.X, e.Y))
                {
                    var node = GetNodeAt(_dragStartPoint);
                    if (node == null)
                    {
                        return;
                    }

                    if (node.Tag is MemoFolder)
                    {
                        var effects = DoDragDrop(
                            node,
                            DragDropEffects.Move | DragDropEffects.Copy
                        );
                        ClearDragState();

                    }
                    else if (node.Tag is MemoTag)
                    {
                        var effects = DoDragDrop(
                            node,
                            DragDropEffects.Move | DragDropEffects.Copy
                        );
                        ClearDragState();

                        if ((effects & DragDropEffects.Move) == DragDropEffects.Move)
                        {
                            node.Remove();
                        }

                    }
                    else
                    {
                        ClearDragState();
                    }
                }
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            /* if (e.Button == MouseButtons.Left) {
                /// Nodeクリックでサブツリーを開閉する
                var test = HitTest(e.Location);
                var node = test.Node;
                if (node != null) {
                    if (test.Location != TreeViewHitTestLocations.PlusMinus) {
                        if (node.IsExpanded) {
                            var selected = SelectedNode;
                            node.Collapse();

                            /// これがないと適当なクリアファイルをクリック，「クリアファイル」ノードをクリックで
                            /// AfterSelect()が呼ばれない
                            /// (親子関係のノードでCollapseされると呼ばれない?)
                            if (selected != null) {
                                var parent = selected.Parent;
                                var found = false;
                                while (parent != null) {
                                    if (parent == node) {
                                        found = true;
                                        break;
                                    }
                                    parent = parent.Parent;
                                }
                                if (found) {
                                    OnAfterSelect(new TreeViewEventArgs(node, TreeViewAction.Collapse));
                                }
                            }

                        } else {
                            node.Expand();

                            /// Expand()時はいらない?
                            ///OnAfterSelect(new TreeViewEventArgs(node, TreeViewAction.Expand));
                        }
                    }
                }
            }
            */
            base.OnMouseClick(e);
        }

        //protected override void OnMouseDoubleClick(MouseEventArgs e) {
        //    base.OnMouseDoubleClick(e);

        //    //var node = GetNodeAt(e.Location);
        //    //if (node == _searchResultNode) {
        //    //    _facade.ShowMemoFinderView();
        //    //}
        //}

        // --- dnd ---
        protected override void OnQueryContinueDrag(QueryContinueDragEventArgs e)
        {
            base.OnQueryContinueDrag(e);

            /// マウスの右ボタンが押されていればドラッグをキャンセル
            if (EnumUtil.HasAllFlags((int)e.KeyState, (int)DragEventKeyStates.RightButton))
            {
                e.Action = DragAction.Cancel;
            }
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);

            e.Effect = DragDropEffects.None;

            if (e.Data.GetDataPresent(typeof(MemoInfo[])))
            {
                if (DragDropUtil.IsMoveAllowed(e) || DragDropUtil.IsCopyAllowed(e))
                {
                    var target = GetNodeAt(PointToClient(new Point(e.X, e.Y)));
                    var source = (TreeNode)e.Data.GetData(typeof(TreeNode));

                    if (!_isInDragOver)
                    {
                        _isInDragOver = true;
                        _selectedObjBeforeDragOver = SelectedNode == null ? null : SelectedNode.Tag;
                    }

                    /// 表示のための処理
                    if (target != null)
                    {
                        SelectedNode = target;
                        if (target.PrevVisibleNode != null)
                        {
                            target.PrevVisibleNode.EnsureVisible();
                        }
                        if (target.NextVisibleNode != null)
                        {
                            target.NextVisibleNode.EnsureVisible();
                        }
                    }

                    /// targetがドロップ先として適切か調べる
                    if (target != null)
                    {
                        if (target.Tag is MemoFolder)
                        {
                            /// MemoFolderにドロップ
                            if (DragDropUtil.IsControlPressed(e))
                            {
                                e.Effect = DragDropEffects.Copy;
                            }
                            else
                            {
                                e.Effect = DragDropEffects.Move;
                            }

                        }
                        else if (target.Tag is MemoTag)
                        {
                            /// MemoTagにドロップ
                            if (DragDropUtil.IsControlPressed(e))
                            {
                                e.Effect = DragDropEffects.Copy;
                            }
                            else
                            {
                                e.Effect = DragDropEffects.Move;
                            }

                        }
                        else
                        {
                            /// ドロップ先として不適切
                            e.Effect = DragDropEffects.None;
                        }
                    }
                    else
                    {
                        e.Effect = DragDropEffects.None;
                    }
                }

            }
            else if (e.Data.GetDataPresent(typeof(TreeNodeEx)))
            {
                if (DragDropUtil.IsMoveAllowed(e) || DragDropUtil.IsCopyAllowed(e))
                {
                    var target = GetNodeAt(PointToClient(new Point(e.X, e.Y)));
                    var source = (TreeNodeEx)e.Data.GetData(typeof(TreeNodeEx));

                    if (!_isInDragOver)
                    {
                        _isInDragOver = true;
                        _selectedObjBeforeDragOver = SelectedNode == null ? null : SelectedNode.Tag;
                    }

                    /// 表示のための処理
                    if (target != null)
                    {
                        SelectedNode = target;
                        if (target.PrevVisibleNode != null)
                        {
                            target.PrevVisibleNode.EnsureVisible();
                        }
                        if (target.NextVisibleNode != null)
                        {
                            target.NextVisibleNode.EnsureVisible();
                        }

                    }

                    ///マウス下のNodeがドロップ先として適切か調べる
                    if (
                        target != null &&
                        target != source &&
                        !IsChildNode(source, target)
                    )
                    {
                        if (
                            source.Tag is MemoFolder &&
                            (target.Tag is MemoFolder || target.Tag == FolderCategoryObj) &&
                            (target != source.Parent || DragDropUtil.IsCopyAllowed(e))
                        )
                        {
                            /// MemoFolderのドロップ
                            if (
                                DragDropUtil.IsCopyAllowed(e) &&
                                DragDropUtil.IsControlPressed(e)
                            )
                            {
                                e.Effect = DragDropEffects.Copy;
                            }
                            else
                            {
                                e.Effect = DragDropEffects.Move;
                            }
                        }
                        else if (
                          source.Tag is MemoTag &&
                          (target.Tag is MemoTag || target.Tag == TagCategoryObj) &&
                          target != source.Parent
                      )
                        {
                            /// MemoTagのドロップ
                            e.Effect = DragDropEffects.Move;

                        }
                        else
                        {
                            e.Effect = DragDropEffects.None;
                        }
                    }
                }
            }
        }

        protected override void OnDragDrop(DragEventArgs e)
        {
            base.OnDragDrop(e);

            if (e.Data.GetDataPresent(typeof(MemoInfo[])))
            {
                if (DragDropUtil.IsMove(e) || DragDropUtil.IsCopy(e))
                {
                    var target = GetNodeAt(PointToClient(new Point(e.X, e.Y)));
                    var memoInfos = (MemoInfo[])e.Data.GetData(typeof(MemoInfo[]));

                    if (target != null)
                    {
                        if (target.Tag is MemoFolder)
                        {
                            /// クリアファイルにドロップ
                            var targetFolder = target.Tag as MemoFolder;

                            var existsNotContained = false;
                            foreach (var memoInfo in memoInfos)
                            {
                                if (!targetFolder.ContainingMemos.Contains(_facade.Container.Find<Memo>(memoInfo.MemoId)))
                                {
                                    existsNotContained = true;
                                    break;
                                }
                            }
                            if (existsNotContained)
                            {
                                foreach (var memoInfo in memoInfos)
                                {
                                    var memo = _workspace.Container.Find<Memo>(memoInfo.MemoId);
                                    if (!targetFolder.ContainingMemos.Contains(memo))
                                    {
                                        if (EnumUtil.HasAllFlags((int)e.Effect, (int)DragDropEffects.Move))
                                        {
                                            memo.ClearContainedFolders();
                                        }
                                        targetFolder.AddContainingMemo(memo);
                                    }
                                }
                                _isInDragOver = false;
                                if (SelectedNode.Tag != _selectedObjBeforeDragOver)
                                {
                                    SelectedNode = SelectNode(_selectedObjBeforeDragOver);
                                }
                                _selectedObjBeforeDragOver = null;

                                return; /// end of drop success
                            }
                        }
                        else if (target.Tag is MemoTag)
                        {
                            /// タグにドロップ
                            _isInDragOver = false;
                            SelectNode(_selectedObjBeforeDragOver);
                            _selectedObjBeforeDragOver = null;

                            var addingTag = target.Tag as MemoTag;
                            foreach (var memoInfo in memoInfos)
                            {
                                var memo = _facade.Container.Find<Memo>(memoInfo.MemoId);

                                if (DragDropUtil.IsMove(e))
                                {
                                    memo.ClearTags();
                                }
                                if (!memo.Tags.Contains(addingTag))
                                {
                                    memo.AddTag(addingTag);
                                }
                            }
                            return; /// end of drop success
                        }
                    }
                }
            }
            else if (e.Data.GetDataPresent(typeof(TreeNodeEx)))
            {
                if (DragDropUtil.IsMove(e) || DragDropUtil.IsCopy(e))
                {
                    var target = GetNodeAt(PointToClient(new Point(e.X, e.Y)));
                    var source = (TreeNodeEx)e.Data.GetData(typeof(TreeNodeEx));
                    var sourceFolder = source.Tag as MemoFolder;

                    if (
                        target != null &&
                        target != source &&
                        !IsChildNode(source, target)
                    )
                    {
                        if (
                            source.Tag is MemoFolder &&
                            (target.Tag is MemoFolder || target.Tag == FolderCategoryObj) &&
                            (target != source.Parent || DragDropUtil.IsCopyAllowed(e))
                        )
                        {
                            if (target == _folderCategoryNode)
                            {
                                if (DragDropUtil.IsMove(e))
                                {
                                    sourceFolder.ParentFolder = null;
                                    source.Remove();
                                    target.Nodes.Add(source);
                                    _isInDragOver = false;
                                    _selectedObjBeforeDragOver = null;
                                    SelectedNode = source;
                                    return; /// end of drop success
                                }
                                else if (DragDropUtil.IsCopy(e))
                                {
                                    var created = _workspace.CreateFolder();
                                    created.Name = sourceFolder.Name;
                                    foreach (var memo in sourceFolder.ContainingMemos)
                                    {
                                        created.AddContainingMemo(memo);
                                    }
                                    var createdNode = new TreeNodeEx(created.Name);
                                    createdNode.Tag = created;
                                    createdNode.ImageIndex = 1;
                                    createdNode.SelectedImageIndex = 1;
                                    target.Nodes.Add(createdNode);
                                    _isInDragOver = false;
                                    _selectedObjBeforeDragOver = null;
                                    SelectedNode = createdNode;
                                    return; /// end of drop success
                                }

                            }
                            else if (target.Tag is MemoFolder)
                            {
                                var targetFolder = (MemoFolder)target.Tag;
                                if (DragDropUtil.IsMove(e))
                                {
                                    sourceFolder.ParentFolder = targetFolder;
                                    source.Remove();
                                    target.Nodes.Add(source);
                                    _isInDragOver = false;
                                    _selectedObjBeforeDragOver = null;
                                    SelectedNode = source;
                                    return; /// end of success drop
                                }
                                else if (DragDropUtil.IsCopy(e))
                                {
                                    var created = _workspace.CreateFolder();
                                    created.ParentFolder = targetFolder;
                                    created.Name = sourceFolder.Name;
                                    foreach (var memo in sourceFolder.ContainingMemos)
                                    {
                                        created.AddContainingMemo(memo);
                                    }
                                    var createdNode = new TreeNodeEx(created.Name);
                                    createdNode.Tag = created;
                                    createdNode.ImageIndex = 1;
                                    createdNode.SelectedImageIndex = 1;
                                    target.Nodes.Add(createdNode);
                                    _isInDragOver = false;
                                    _selectedObjBeforeDragOver = null;
                                    SelectedNode = createdNode;
                                    return; /// end of drop success
                                }
                            }
                        }
                        else if (
                          source.Tag is MemoTag &&
                          DragDropUtil.IsMove(e) || DragDropUtil.IsCopy(e) &&
                          target != source.Parent
                      )
                        {
                            /// ドロップされたNodeのコピーを作成
                            var clone = (TreeNode)source.Clone();
                            target.Nodes.Add(clone);
                            target.Expand();

                            var draggedTag = (MemoTag)clone.Tag;
                            if (target == _tagCategoryNode)
                            {
                                draggedTag.SuperTag = null;
                            }
                            else
                            {
                                var super = target.Tag as MemoTag;
                                if (super != null)
                                {
                                    draggedTag.SuperTag = super;
                                }
                            }
                            _isInDragOver = false;
                            _selectedObjBeforeDragOver = null;

                            SelectedNode = clone;
                            return; /// end of drop success
                        }

                    }
                }
            }

            e.Effect = DragDropEffects.None;
            _isInDragOver = false;
            SelectNode(_selectedObjBeforeDragOver);
            _selectedObjBeforeDragOver = null;
        }

        protected override void OnDragLeave(EventArgs e)
        {
            base.OnDragLeave(e);
            if (_isInDragOver)
            {
                _isInDragOver = false;
                SelectNode(_selectedObjBeforeDragOver);
                _selectedObjBeforeDragOver = null;
            }
        }

        //protected override void OnItemDrag(ItemDragEventArgs e) {
        //    base.OnItemDrag(e);

        //    SelectedNode = (TreeNode) e.Item;
        //    Focus();

        //    var effects = DoDragDrop(e.Item, DragDropEffects.Move);
        //    if ((effects & DragDropEffects.Move) == DragDropEffects.Move) {
        //        var dragged = (TreeNode) e.Item;
        //        Nodes.Remove(dragged);
        //    }
        //}


        // ------------------------------
        // private
        // ------------------------------
        private TreeNode GetNode(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            else if (obj == SmartFolderCategoryObj)
            {
                return _smartFolderCategoryNode;
            }
            else if (obj == FolderCategoryObj)
            {
                return _folderCategoryNode;
            }
            else if (obj == TagCategoryObj)
            {
                return _tagCategoryNode;
            }
            else if (obj == AllMemosObj)
            {
                return _allMemosNode;
            }
            else if (obj == TagTreePresenter.UntaggedObj)
            {
                return _tagTreePresenter.UntaggedNode;
            }
            else if (obj is MemoSmartFolder)
            {
                return _smartFolderTreePresenter.GetNode(obj);
            }
            else if (obj is MemoFolder)
            {
                return _folderTreePresenter.GetNode(obj);
            }
            else if (obj is MemoTag)
            {
                return _tagTreePresenter.GetNode(obj);
            }
            else
            {
                return null;
            }
        }

        private TreeNode SelectNode(object obj)
        {
            return SelectedNode = GetNode(obj);
        }

        private TreeNode SelectNode(object obj, TreeNode root)
        {
            if (obj == null || obj == _folderCategoryNode)
            {
                return SelectedNode = _folderCategoryNode;
            }
            else
            {
                foreach (var node in new TreeNodeIterator(root))
                {
                    if (node.Tag == obj)
                    {
                        SelectedNode = node;
                        return node;
                    }
                }
                return SelectedNode = _folderCategoryNode;
            }
        }

        private void UpdateSubNodes(TreeNodeEx node)
        {
            if (node == _smartFolderCategoryNode)
            {
                if (!node.IsSubNodesLoaded)
                {
                    try
                    {
                        Cursor = Cursors.WaitCursor;
                        _smartFolderTreePresenter.RebuildTree();
                        node.IsSubNodesLoaded = true;
                    }
                    finally
                    {
                        Cursor = Cursors.Default;
                    }
                }
            }
            else if (node == _folderCategoryNode)
            {
                if (!node.IsSubNodesLoaded)
                {
                    try
                    {
                        Cursor = Cursors.WaitCursor;
                        _folderTreePresenter.RebuildTree();
                        node.IsSubNodesLoaded = true;

                        foreach (TreeNodeEx child in node.Nodes)
                        {
                            if (!child.IsSubNodesLoaded)
                            {
                                UpdateNode(child);
                            }
                            child.IsSubNodesLoaded = true;
                        }
                    }
                    finally
                    {
                        Cursor = Cursors.Default;
                    }
                }
            }
            else if (node == _tagCategoryNode)
            {
                if (!node.IsSubNodesLoaded)
                {
                    try
                    {
                        Cursor = Cursors.WaitCursor;
                        _tagTreePresenter.RebuildTree();
                        node.IsSubNodesLoaded = true;
                    }
                    finally
                    {
                        Cursor = Cursors.Default;
                    }
                }
            }
            else
            {
                foreach (TreeNodeEx child in node.Nodes)
                {
                    if (!child.IsSubNodesLoaded)
                    {
                        UpdateNode(child);
                    }
                    child.IsSubNodesLoaded = true;
                }
            }
        }


        private void UpdateNode(TreeNodeEx node)
        {
            var folder = node.Tag as MemoFolder;
            if (folder != null)
            {
                foreach (var sub in folder.SubFolders)
                {
                    var created = new TreeNodeEx(sub.Name);
                    created.Tag = sub;
                    if (sub == _facade.ActiveFolder)
                    {
                        created.ImageIndex = 2;
                        created.SelectedImageIndex = 2;
                    }
                    else
                    {
                        created.ImageIndex = 1;
                        created.SelectedImageIndex = 1;
                    }
                    node.Nodes.Add(created);
                }
            }
        }

        private void ClearDragState()
        {
            _isDragPrepared = false;
            _dragStartPoint = Point.Empty;
            _dragStartRect = Rectangle.Empty;
        }

        private void SetUpDragState(Point pt)
        {
            if (_allowEdit)
            {
                _isDragPrepared = true;
                _dragStartPoint = pt;
                _dragStartRect = new Rectangle(
                    _dragStartPoint.X - SystemInformation.DragSize.Width / 2,
                    _dragStartPoint.Y - SystemInformation.DragSize.Height / 2,
                    SystemInformation.DragSize.Width,
                    SystemInformation.DragSize.Height
                );
            }
            else
            {
                ClearDragState();
            }
        }

        private static bool IsChildNode(TreeNode parent, TreeNode child)
        {
            if (child.Parent == null)
            {
                return false;
            }
            else if (child.Parent == parent)
            {
                return true;
            }
            else
            {
                return IsChildNode(parent, child.Parent);
            }
        }

        // --- event handler ---

        //private void HandleFacadeMemoInfoAdded(object sender, MemoInfoEventArgs e) {
        //}

        //private void HandleFacadeMemoInfoRemoved(object sender, MemoInfoEventArgs e) {
        //    var memoInfo = e.MemoInfo;
        //    var memo = _facade.Container.Find<Memo>(memoInfo.MemoId);

        //    if (memo != null) {
        //        var removings = new List<TreeNode>();
        //        foreach (var node in new TreeNodeIterator(this)) {
        //            if (node.Tag == memo) {
        //                removings.Add(node);
        //            }
        //        }

        //        if (removings.Count > 0) {
        //            BeginUpdate();
        //            foreach (var removing in removings) {
        //                removing.Remove();
        //            }
        //            EndUpdate();
        //        }
        //    }
        //}

        //private void HandleFacadeMemoInfoChanged(object sender, MemoInfoEventArgs e) {
        //    var memoInfo = e.MemoInfo;
        //    var memo = _facade.Container.Find<Memo>(memoInfo.MemoId);

        //    if (memo != null) {
        //        BeginUpdate();
        //        foreach (var node in new TreeNodeIterator(this)) {
        //            if (node.Tag == memo) {
        //                node.Text = memo.Title;
        //            }
        //        }
        //        EndUpdate();
        //    }
        //}


        //private void UpdateMemoListBoxForSelectedNode(MemoInfoEventArgs e) {
        //    if (SelectedNode == null) {
        //        return;
        //    }

        //    var memoInfo = e.MemoInfo;
        //    if (SelectedNode == _searchResultNode) {
        //        if (_memoFinderView != null) {
        //            _memoFinderView.UpdateMemoListBox(_memoListBox, false);
        //        }
        //    } else {
        //        var selectedFolder = SelectedNode.Tag as MemoFolder;
        //        if (selectedFolder != null) {
        //            var memo = _facade.Container.Find<Memo>(memoInfo.MemoId);
        //            if (memo != null) {
        //                if (memo.ContainedFolders.Contains(selectedFolder)) {
        //                    UpdateMemoListBox(_memoListBox);
        //                }
        //            }
        //        }
        //    }
        //}

        //private void HandleFacadeMemoInfoAdded(object sender, MemoInfoEventArgs e) {
        //    UpdateMemoListBoxForSelectedNode(e);
        //}

        //private void HandleFacadeMemoInfoRemoving(object sender, MemoInfoEventArgs e) {
        //    UpdateMemoListBoxForSelectedNode(e);
        //}

        //private void HandleFacadeMemoInfoChanged(object sender, MemoInfoEventArgs e) {
        //    UpdateMemoListBoxForSelectedNode(e);
        //}

        //private void HandleMemoFolderChanged(object sender, MemoFolderChangedEventArgs e) {
        //    if (_facade.ActiveFolder != null && e.Cause.PropertyName == "ContainingMemos") {
        //        /// ノートをクリアファイルから削除したときにタブを閉じる
        //        if (e.Cause.Kind == PropertyChangeKind.Remove) {
        //            var openInfos = _facade.OpenMemoInfos.ToArray();
        //            foreach (var info in openInfos) {
        //                var memo = _facade.Container.Find<Memo>(info.MemoId);
        //                if (!e.Folder.ContainingMemos.Contains(memo)) {
        //                    _facade.CloseMemo(info);
        //                }
        //            }
        //        } else if (e.Cause.Kind == PropertyChangeKind.Clear) {
        //            _facade.CloseAllMemos();
        //        }
        //    }

        //    //if (e.Folder == SelectedFolder || _facade.ActiveFolder != null) {
        //    //    UpdateMemoListBox(_memoListBox);
        //    //}
        //}

        //private void HandleMemoFolderAdded(object sender, MemoFolderEventArgs e) {
        //    UpdateMemoListBox(_memoListBox);
        //}

        //private void HandleMemoFolderRemoving(object sender, MemoFolderEventArgs e) {
        //    UpdateMemoListBox(_memoListBox);
        //}

        //private void HandleMemoTagChanged(object sender, MemoTagChangedEventArgs e) {
        //    UpdateMemoListBox(_memoListBox);
        //}

        //private void HandleMemoTagAdded(object sender, MemoTagEventArgs e) {
        //    UpdateMemoListBox(_memoListBox);
        //}

        //private void HandleMemoTagRemoving(object sender, MemoTagEventArgs e) {
        //    UpdateMemoListBox(_memoListBox);
        //}

        private void HandleFacadeActiveFolderChanging(object sender, EventArgs e)
        {
            if (_facade.ActiveFolder != null)
            {
                var node = GetNode(_facade.ActiveFolder);
                if (node != null)
                {
                    node.ImageIndex = 1;
                    node.SelectedImageIndex = 1;
                }
            }
        }

        private void HandleFacadeActiveFolderChanged(object sender, EventArgs e)
        {
            if (_facade.ActiveFolder != null)
            {
                var node = GetNode(_facade.ActiveFolder);
                if (node != null)
                {
                    node.ImageIndex = 2;
                    node.SelectedImageIndex = 2;
                }
            }
            //UpdateMemoListBox(_memoListBox);
        }


        // ========================================
        // class
        // ========================================
        /// <summary>
        /// 未整理，すべてを最後に，それ以外は辞書順にソートするcomparer．
        /// </summary>
        private class WorkspaceTreeNodeComparer : IComparer
        {
            private WorkspaceTree _tree;

            public WorkspaceTreeNodeComparer(WorkspaceTree tree)
            {
                _tree = tree;
            }

            public int Compare(object x, object y)
            {
                var xNode = (TreeNode)x;
                var yNode = (TreeNode)y;

                var xObj = xNode.Tag;
                var yObj = yNode.Tag;

                if (xObj == yObj)
                {
                    return 0;
                }

                if (Array.IndexOf(SpecialObjects, xObj) > -1)
                {
                    var arrComparer = new ArrayOrderingComparer<object>(SpecialObjects);
                    return arrComparer.Compare(xObj, yObj);
                }


                if (xObj is MemoFolder && yObj is MemoFolder)
                {
                    return xNode.Text.CompareTo(yNode.Text);
                }
                if (xObj is MemoSmartFolder && yObj is MemoSmartFolder)
                {
                    return xNode.Text.CompareTo(yNode.Text);
                }
                if (xObj == _tree.TagTreePresenter.UntaggedNode.Tag)
                {
                    if (yObj is MemoTag)
                    {
                        return -1;
                    }
                }
                if (xObj is MemoTag)
                {
                    if (yObj == _tree.TagTreePresenter.UntaggedNode.Tag)
                    {
                        return 1;
                    }
                    else if (yObj is MemoTag)
                    {
                        return xNode.Text.CompareTo(yNode.Text);
                    }
                }

                return 0;
            }
        }

    }
}
