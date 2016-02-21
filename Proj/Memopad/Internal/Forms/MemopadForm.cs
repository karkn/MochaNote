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
using Mkamo.Editor.Tools;
using Mkamo.Editor.Core;
using Mkamo.Memopad.Internal.Controllers;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.StyledText.Core;
using Mkamo.Editor.Focuses;
using Mkamo.Figure.Figures;
using Mkamo.Common.DataType;
using Mkamo.Figure.Figures.EdgeDecorations;
using Mkamo.Common.Win32.User32;
using Mkamo.Model.Memo;
using Mkamo.Model.Core;
using Mkamo.Container.Core;
using Mkamo.Common.Win32;
using System.Windows.Forms;
using Mkamo.Control.SelectorDropDown;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Mkamo.Common.Externalize;
using Mkamo.Model.Uml;
using Mkamo.Memopad.Core;
using Mkamo.Common.IO;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Memopad.Internal.Controls;
using Mkamo.Memopad.Properties;
using Mkamo.Common.Diagnostics;
using Mkamo.Editor.Requests;
using Mkamo.Editor.Commands;
using Mkamo.Figure.Core;
using System.Drawing.Text;
using Mkamo.Memopad.Internal.Utils;
using ComponentFactory.Krypton.Toolkit;
using Mkamo.Control.TabControlEx;
using Mkamo.Common.Forms.Themes;
using Mkamo.Common.Command;
using System.Runtime.Serialization;
using System.Reflection;
using Mkamo.Common.Serialize;
using System.Drawing.Imaging;
using Mkamo.Memopad.Internal.Helpers;
using Mkamo.Common.Forms.Input;
using Mkamo.Common.Win32.Core;
using Mkamo.Control.HotKey;
using Mkamo.Common.Core;
using System.Text.RegularExpressions;
using System.Threading;
using Mkamo.Common.Forms.KeyMap;

namespace Mkamo.Memopad.Internal.Forms {
    internal partial class MemopadForm: MemopadFormBase {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // ========================================
        // field
        // ========================================
        private MemopadApplication _app;
        private MemopadFormMediator _mediator;

        /// <summary>
        /// 起動処理を短くするために自前でインスタンス管理
        /// </summary>
        private Lazy<MemoQueryBuilderView> _memoQueryBuilderView;

        private EditorCanvas _currentEditorCanvas;

        private bool _isShown;

        /// <summary>
        /// 折りたたみ表示のため
        /// </summary>
        private int _finderPaneWidth;
        private int _memoListPaneWidth;


        private Point _normalWindowLocation;
        private Size _normalWindowSize;

        private Point _compactWindowLocation;
        private Size _compactWindowSize;

        // --- keymap ---
        private KeyMap<TextBox> _conditionTextBoxKeyMap;

        // --- theme ---
        private ITheme _theme;

        private System.Windows.Forms.Timer _hideMessageTimer; /// lazy

        //private Lazy<BackgroundWorker> _worker;

        // ========================================
        // constructor
        // ========================================
        public MemopadForm() {
            _app = MemopadApplication.Instance;

            InitializeComponent();

            _memoQueryBuilderView = new Lazy<MemoQueryBuilderView>(() => CreateMemoQueryBuilderView());

            DoubleBuffered = true;
            Icon = Resources.confidante;
 
            _tabControl.Multiline = true;
            _tabControl.CloseTabImage = Resources.cross_button;

            _isShown = false;

            //_worker = new Lazy<BackgroundWorker>(
            //    () => {
            //        var ret = new BackgroundWorker();
            //        ret.DoWork += HandleWorkerDoWork;
            //        ret.RunWorkerCompleted += HandleWorkerRunWorkerCompleted;
            //        return ret;
            //    }
            //);
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            //if (_app.UserInfo.IsLicensed) {
                _adPanel.Visible = false;
            //} else {
            //    _adPanel.Paint += (se, ev) => {
            //        ev.Graphics.DrawLine(Pens.Silver, new Point(5, 0), new Point(_adPanel.Width - 5, 0));
            //    };
            //    _adPanel.Resize += (se, ev) => {
            //        _adLinkLabel.Left = (_adPanel.Width - _adLinkLabel.Width) / 2;
            //        _adLinkLabel.Top = (_adPanel.Height - _adLinkLabel.Height) / 2;
            //        _adPanel.Invalidate();
            //    };
            //    _adLinkLabel.Left = (_adPanel.Width - _adLinkLabel.Width) / 2;
            //    _adLinkLabel.Top = (_adPanel.Height - _adLinkLabel.Height) / 2;
            //}

            /// 設定値のロード
            var windowSettings = _app.WindowSettings;

            /// window bounds
            _normalWindowLocation = windowSettings.WindowLocation;
            _normalWindowSize = windowSettings.WindowSize;
            _compactWindowLocation = windowSettings.CompactWindowLocation;
            _compactWindowSize = windowSettings.CompactWindowSize;
            Location = _normalWindowLocation;
            Size = _normalWindowSize;
            if (windowSettings.WindowTopMost) {
                TopMost = true;
            }
            if (windowSettings.WindowMaximized) {
                WindowState = FormWindowState.Maximized;
            }

            /// load workspace pane settings
            _finderSplitContainer.Panel1Collapsed = false;
            if (windowSettings.WorkspaceSplitterCollapsed) {
                CollapseWorkspacePane();
                _finderPaneWidth = windowSettings.WorkspaceSplitterDistance; /// CollapseWorkspacePane()の後でやらないと初期値に戻される
            } else {
                _finderSplitContainer.SplitterDistance = windowSettings.WorkspaceSplitterDistance;
                ExpandFinderPane();
            }
            ShowWorkspaceView();

            /// load memo list pane settings
            _memoListSplitContainer.Panel1Collapsed = false;
            if (windowSettings.MemoListSplitterCollapsed) {
                CollapseMemoListPane();
                _memoListPaneWidth = windowSettings.MemoListSplitterDistance; /// CollapseMemoListPane()の後でやらないと初期値に戻される
            } else {
                _memoListSplitContainer.SplitterDistance = windowSettings.MemoListSplitterDistance;
                ExpandMemoListPane();
            }

            _memoListView.MemoListBox.DisplayItems = windowSettings.MemoListBoxDisplayItems;
            _memoListView.MemoListBox.SortKey = windowSettings.MemoListBoxSortKey;
            _memoListView.MemoListBox.SortsAscendingOrder = windowSettings.MemoListBoxSortsAscendingOrder;
            _memoListView.MemoListBox.SortsImportanceOrder = windowSettings.MemoListBoxSortsImportanceOrder;

            /// スタートページの表示
            /// OnLoad()だと起動時にタスクトレイに格納されていて自動インポートしたときに，
            /// OnShown()だと起動時にタスクトレイに格納されていてクリップしたときに，
            /// ページ順がおかしくなるが，
            /// サイズなどの設定後でないとListBoxのサイズがおかしくなるのでここでやる
            if (windowSettings.ShowStartPageOnStart) {
                ShowStartPage();
            }

            /// 一旦disabledに
            UpdateToolStrip();
            UpdateMemoToolStrip();
            UpdateCanvasSizeDisplay();
        }

        private void UpdateMemoToolStrip(){
            _showAllFusenToolStripButton.Enabled = _app.FusenManager.RegisteredForms.Any();
        }
        
        private void HandleFusenManagerChanged(object sender, EventArgs e) {
            UpdateMemoToolStrip();
        }

        protected override void OnShown(EventArgs e) {
            base.OnShown(e);

            Cursor = Cursors.WaitCursor;

            try {
                _memoListView.SetTargetViews(_workspaceView);
    
                // --- key bind ---
                _conditionTextBoxKeyMap = new KeyMap<TextBox>();
                _app.KeySchema.TextBoxKeyBinder.Bind(_conditionTextBoxKeyMap);
    
                // --- event ---
                _memoListSplitContainer.Panel2.DoubleClick += HandleMemoPanelDoubleClick;

                _tabControl.ControlAdded += HandleTabControlControlAdded;
                _tabControl.Selecting += HandleTabControlSelecting;
                _tabControl.Selected += HandleTabControlSelected;
                _tabControl.MouseDown += HandleTabControlMouseDown;
                _tabControl.MouseDoubleClick += HandleTabControlMouseDoubleClick;
                _tabControl.DragStart += HandleTabControlDragStart;
                _tabControl.CloseButtonPressed += HandleTabControlCloseButtonPressed;
    
                _app.ActiveFolderChanged += HandleAppActiveFolderChanged;
                _app.ActiveSmartFilterChanged += HandleAppActiveSmartFilterChanged;
    
                _memoListHeaderGroup.Resize += (sender, ex) => {
                    _memoListView.MemoListBox.Invalidate();
                };

                _app.FusenManager.Registered += HandleFusenManagerChanged;
                _app.FusenManager.Unregistered += HandleFusenManagerChanged;

                InitToolStripHandlers();

                // --- prepare ui ---
                _workspaceView.InitUI();

                InitDropDowns();
                InitMemoMarkToolStripSplitButton();
                Application.DoEvents(); /// これがないとマークツールボタンの右の表示がおかしい

                _mediator = new MemopadFormMediator(this, _workspaceView, _memoListView);
                if (_memoQueryBuilderView.IsValueCreated) {
                    _mediator.MemoQueryBuilderView = _memoQueryBuilderView.Value;
                }
    
                /// menu
                switch (_app.Settings.KeyScheme) {
                    case KeySchemeKind.Default:
                        _createMemoFileToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.N;
                        _createMemoFromClipboardFileToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Alt |     Keys.V;
                        break;
                    case KeySchemeKind.Emacs:
                        _createMemoFileToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Alt | Keys.M;
                        _createMemoFromClipboardFileToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Alt |     Keys.V;
                        break;
                }
    
                /// toolstrip
                UpdateToolStrip();
    
                var windowSettings = _app.WindowSettings;
                _workspaceView.WorkspaceTree.TagTreePresenter.FindMemoWithDescendants = windowSettings.ShowDescendantTagsMemo;


                // --- status bar ---
                _app.MemoInfoAdded += HandleMemoInfoAdded;
                _app.MemoInfoRemoved += HandleMemoInfoRemoved;
                _app.MemoInfoRecovered += HandleMemoInfoRecovered;
                UpdateMemoCountLabel();
                UpdateSmartFilterLabel();

                // --- win7 taskbar ---
                //if (EnvironmentUtil.IsWin7()) {
                //    InitWin7Taskbar();
                //}


                //if (_app.Settings.CheckLatestOnStart) {
                //    _app.CheckForUpdatesAsync();
                //}

            } finally {
                Cursor = Cursors.Default;
                _isShown = true;
            }
        }

        //private void InitWin7Taskbar() {
        //    try {
        //        if (TaskbarManager.IsPlatformSupported) {
        //            var jlist = JumpList.CreateJumpList();
        //            jlist.ClearAllUserTasks();

        //            var cmd = Path.Combine(
        //                Path.GetDirectoryName(Application.ExecutablePath),
        //                "MochaNoteClientW.exe"
        //            );

        //            var task = new JumpListLink(cmd, "ノートを作成");
        //            task.Arguments = "create \"新しいノート\" -a -s=30";
        //            task.ShowCommand = Microsoft.WindowsAPICodePack.Shell.WindowShowCommand.Hide;
        //            jlist.AddUserTasks(task);

        //            jlist.Refresh();
        //        }
        //    } catch (Exception e) {
        //        Logger.Warn("Failed to init win 7 taskbar.", e);
        //    }
        //}

        private MemoQueryBuilderView CreateMemoQueryBuilderView() {
            var ret = new MemoQueryBuilderView();

            ret.SearchTextBox = _conditionTextBox;
            ret.BackColor = System.Drawing.Color.WhiteSmoke;
            ret.Dock = System.Windows.Forms.DockStyle.Fill;
            ret.Location = new System.Drawing.Point(0, 0);
            ret.Margin = new System.Windows.Forms.Padding(0);
            ret.Name = "_memoQueryBuilderView";
            ret.Size = new System.Drawing.Size(198, 533);
            ret.TabIndex = 4;

            _workspaceViewPanel.Controls.Add(ret);
            if (_mediator != null) {
                _mediator.MemoQueryBuilderView = ret;
            }

            return ret;
        }

        private void InitDropDowns() {
            _addFigureToolStripDropDownButton.DropDown = _ToolSelectorDropDown;
            _addFigureToolStripDropDownButton.DropDownOpening += (s, e) => {
                _ToolSelectorDropDown.Prepare();
            };

            _setNodeStyleToolStripDropDownButton.DropDown = _NodeStyleSelectorDropDown;
            _setNodeStyleToolStripDropDownButton.DropDownOpening += (s, e) => {
                _NodeStyleSelectorDropDown.Prepare();
            };

            _setLineStyleToolStripDropDownButton.DropDown = _LineStyleSelectorDropDown;
            _setLineStyleToolStripDropDownButton.DropDownOpening += (s, e) => {
                _LineStyleSelectorDropDown.Prepare();
            };

            _addFreehandToolStripDropDownButton.DropDown = _FreehandSelectorDropDown;
            _addFreehandToolStripDropDownButton.DropDownOpening += (s, e) => {
                _FreehandSelectorDropDown.Prepare();
            };

        }

        private void InitMemoMarkToolStripSplitButton() {
            _memoMarkToolStripSplitButton.Image = Resources.star;
            _memoMarkToolStripSplitButton.Tag = MemoMarkKind.Important;
        }

        // ========================================
        // destructor
        // ========================================
        private void CleanUp() {
            if (_hideMessageTimer != null) {
                _hideMessageTimer.Dispose();
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e) {
            base.OnFormClosed(e);
            
            /// ごみ箱内のノートのpreviewが表示されているままだと
            /// previewでoutline.objが開かれている状態なので削除できずエラーになる
            if (_memoListView != null) {
                _memoListView.MemoListBox.ClosePreviewPopup();
            }

            _app.ExitInternal();
        }


        // ========================================
        // property
        // ========================================
        public EditorCanvas CurrentEditorCanvas {
            get { return _currentEditorCanvas; }
        }

        public PageContent CurrentPageContent {
            get {
                if (_tabControl.SelectedTab != null) {
                    return GetPageContent(_tabControl.SelectedTab);
                }
                return null;
            }
        }

        public WorkspaceView WorkspaceView {
            get { return _workspaceView; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ITheme Theme {
            get { return _theme; }
            set {
                if (value == _theme) {
                    return;
                }

                SuspendLayout();

                _theme = value;

                _workspaceView.Theme = value;
                _memoListView.Theme = value;

                _conditionPanel.BackColor = _theme.DarkBackColor;
                _adPanel.BackColor = _theme.DarkBackColor;

                var captionFont = value.CaptionFont;
                Font = captionFont;
                _tabControl.Font = captionFont;
                _finderHeaderGroup.StateCommon.HeaderPrimary.Content.ShortText.Font = captionFont;
                _memoListHeaderGroup.StateCommon.HeaderPrimary.Content.ShortText.Font = captionFont;

                /// 入れるとなぜかMSゴシック系フォントが使われる
                ///_toolSelectorDropDown.Font = captionFont;
                ///_styleSelectorDropDown.Font = captionFont;
                ///_freehandSelectorDropDown.Font = captionFont;

                //var inputFont = value.InputFont;

                var menuFont = value.MenuFont;
                _mainMenuStrip.Font = menuFont;
                _tabControlContextMenuStrip.Font = menuFont;
                _memoListViewSortContextMenuStrip.Font = menuFont;

                var tabBackColor = KryptonManager.CurrentGlobalPalette.GetBackColor1(
                    PaletteBackStyle.PanelClient,
                    PaletteState.Normal
                );
                var tabBorderColor = KryptonManager.CurrentGlobalPalette.GetBorderColor1(
                    PaletteBorderStyle.ControlClient,
                    PaletteState.Normal
                );
                _tabControl.BackColor = tabBackColor;
                _tabControl.BorderColor = tabBorderColor;

                ResumeLayout(false);
            }
        }

        public IEnumerable<MemoInfo> OpenMemoInfos {
            get {
                foreach (TabPage tabPage in _tabControl.TabPages) {
                    var content = tabPage.Tag as PageContent;
                    if (content != null) {
                        yield return content.MemoInfo;
                    }
                }
            }
        }

        // ------------------------------
        // protected internal
        // ------------------------------
        protected internal override string _GlobalHighlight {
            get { return base._GlobalHighlight; }
            set {
                base._GlobalHighlight = value;
                foreach (var content in MemoTabPageContents) {
                    var canvas = content.EditorCanvas;
                    var hls = Highlight.CreateHighlights(value);
                    canvas.HighlightRegistry.GlobalHighlights = hls;
                    canvas.DirtyAllVisualLines();
                }
            }
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected override EditorCanvas _EditorCanvas {
            get { return _currentEditorCanvas; }
        }

        protected override PageContent _PageContent {
            get { return CurrentPageContent; }
        }

        protected override bool _EnableBackgroundImage {
            get { return true; }
        }

        protected override ComboBox _ParagraphKindToolStripComboBox {
            get { return _paragraphKindToolStripComboBox.ComboBox; }
        }

        protected override ComboBox _FontNameToolStripComboBox {
            get { return _fontNameToolStripComboBox.ComboBox; }
        }

        protected override ComboBox _FontSizeToolStripComboBox {
            get { return _fontSizeToolStripComboBox.ComboBox; }
        }

        // ------------------------------
        // internal
        // ------------------------------
        internal MemopadFormMediator Mediator {
            get { return _mediator; }
        }

        internal IEnumerable<TabPage> MemoTabPages {
            get {
                foreach (TabPage page in _tabControl.TabPages) {
                    if (page.Tag is PageContent) {
                        yield return page;
                    }
                }
            }
        }

        internal IEnumerable<PageContent> MemoTabPageContents {
            get {
                foreach (TabPage page in _tabControl.TabPages) {
                    var content = page.Tag as PageContent;
                    if (content != null) {
                        yield return content;
                    }
                }
            }
        }

        internal TabPage StartPage {
            get {
                foreach (TabPage page in _tabControl.TabPages) {
                    if (IsStartPageTabPage(page)) {
                        return page;
                    }
                }
                return null;
            }
        }

        internal bool IsStartPageOpen {
            get {
                foreach (TabPage page in _tabControl.TabPages) {
                    if (IsStartPageTabPage(page)) {
                        return true;
                    }
                }
                return false;
            }
        }

        internal bool IsShown {
            get { return _isShown; }
        }

        internal MemoListView _MemoListView {
            get { return _memoListView; }
        }

        internal TabControl _TabControl {
            get { return _tabControl; }
        }

        /// <summary>
        /// TabControl.ControlRemovedのタイミングではTabPagesが更新されていないが，
        /// Controlsは更新されているのでそちらを参照する。
        /// 普段はTabPagesの方を使った方がよさそう。
        /// </summary>
        public IEnumerable<MemoInfo> _OpenMemoInfosForControlRemoved {
            get {
                foreach (TabPage tabPage in _tabControl.Controls) {
                    var content = tabPage.Tag as PageContent;
                    if (content != null) {
                        yield return content.MemoInfo;
                    }
                }
            }
        }

        // ------------------------------
        // private
        // ------------------------------
        private bool IsFinderPaneCollapsed {
            get { return _finderSplitContainer.IsSplitterFixed; }
        }

        private bool IsMemoListPaneCollapsed {
            get { return _memoListSplitContainer.IsSplitterFixed; }
        }


        private EditorCanvas _CurrentEditorCanvas {
            get { return _currentEditorCanvas; }
            set {
                if (value == _currentEditorCanvas) {
                    return;
                }

                if (_currentEditorCanvas != null) {
                    _currentEditorCanvas.ToolChanged -= HandleCurrentEditorCanvasToolChanged;
                }
                _currentEditorCanvas = value;
                if (_currentEditorCanvas != null) {
                    _currentEditorCanvas.ToolChanged += HandleCurrentEditorCanvasToolChanged;
                }
            }
        }

        private System.Windows.Forms.Timer _HideMessageTimer {
            get {
                if (_hideMessageTimer == null) {
                    _hideMessageTimer = new System.Windows.Forms.Timer();
                    _hideMessageTimer.Enabled = false;
                    _hideMessageTimer.Interval = 5000;
                    _hideMessageTimer.Tick += (sender, e) => {
                        _messageToolStripStatusLabel.Text = "";
                    };
                }
                return _hideMessageTimer;
            }
        }

        // ========================================
        // method
        // ========================================
        public TabPage ShowStartPage() {
            if (IsStartPageOpen) {
                var startPage = StartPage;
                _tabControl.SelectedTab = startPage;
                return startPage;

            } else {
                var startPage = CreateStartPageTabPage();
                _tabControl.TabPages.Add(startPage);
                _tabControl.SelectTab(startPage);
                return startPage;
            }
        }

        public TabPage NewPage(MemoInfo info, Memo memo) {
            var pageContent = CreateMemoPageContent(info);
            pageContent.IsModified = true;

            var canvas = pageContent.EditorCanvas;
            canvas.EditorContent = memo;
            //pageContent.Memo = memo;

            /// tabPageをAdd()する前にやらないとHandleTabControlControlAdded()で困る
            _CurrentEditorCanvas = canvas;

            var tabPage = CreateMemoTabPage(info.ToString(), pageContent);
            _tabControl.TabPages.Add(tabPage);
            _tabControl.SelectTab(tabPage);

            var caret = canvas.Caret;
            caret.Position = MemopadConsts.DefaultCaretPosition;
            caret.Show();
            canvas.RootEditor.Content.RequestSelect(SelectKind.True, true);

            pageContent.Focus();

            return tabPage;
        }

        public TabPage LoadPage(MemoInfo info, bool background, bool enabled) {
            /// すでに開かれているノートならばそのタブを選択して返す
            foreach (var tp in MemoTabPages) {
                var pc = (PageContent) tp.Tag;
                if (pc.MemoInfo == info) {
                    if (!background) {
                        _tabControl.SelectTab(tp);
                        pc.EditorCanvas.Select();
                    }
                    return tp;
                }
            }

            var pageContent = CreateMemoPageContent(info);
            var canvas = pageContent.EditorCanvas;
            //MemoSerializeUtil.LoadEditor(canvas, info.MementoId);
            pageContent.Memo.AccessedDate = DateTime.Now;

            pageContent.Enabled = enabled;

            _SuppressToolStripUpdate = true;
            if (!background) {
                /// tabPageをAdd()する前にやらないとHandleTabControlControlAdded()で困る
                _CurrentEditorCanvas = canvas;
            }

            var tabPage = CreateMemoTabPage(info.ToString(), pageContent);
            _tabControl.TabPages.Add(tabPage);

            if (!background) {
                _tabControl.SelectTab(tabPage);
            }
            _tabControl.Refresh();

            //canvas.InLoading = true;
            //var ctx = canvas.DirtManager.BeginDirty();
            //_worker.Value.RunWorkerAsync(Tuple.Create(canvas, info.MementoId, ctx));
            MemoSerializeUtil.LoadEditor(canvas, info.MementoId);
            _SuppressToolStripUpdate = false;

            var caret = canvas.Caret;
            caret.Position = MemopadConsts.DefaultCaretPosition;
            caret.Show();
            canvas.RootEditor.Content.RequestSelect(SelectKind.True, true);

            canvas.Select();

            return tabPage;
        }

        //private void HandleWorkerDoWork(object sender, DoWorkEventArgs e) {
        //    var arg = (Tuple<EditorCanvas, string, DirtyingContext>) e.Argument;
        //    MemoSerializeUtil.LoadEditor(arg.Item1, arg.Item2);
        //    e.Result = Tuple.Create(arg.Item1, arg.Item3);
        //}

        //private void HandleWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
        //    var result = (Tuple<EditorCanvas, DirtyingContext>) e.Result;
        //    var canvas = result.Item1;
        //    canvas.InLoading = false;
        //    result.Item2.Dispose();

        //    _SuppressToolStripUpdate = false;

        //    var caret = canvas.Caret;
        //    caret.Position = MemopadConsts.DefaultCaretPosition;
        //    caret.Show();
        //    canvas.Select();
        //}

        public void CloseStartPage() {
            if (IsStartPageOpen) {
                var page = StartPage;
                _tabControl.TabPages.Remove(page);
                page.Dispose();
            }
        }

        public MemoInfo CloseMemoPage(TabPage page) {
            var ret = default(MemoInfo);

            if (page != null && IsMemoTabPage(page)) {
                var pageIndex = _tabControl.TabPages.IndexOf(page);
                Contract.Requires(pageIndex > -1);

                var pageContent = (PageContent) page.Tag;
                ret = pageContent.MemoInfo;
                DisposeMemoPageContent(pageContent);

                if (page == _tabControl.SelectedTab) {
                    var pageCount = _tabControl.TabCount;
                    if (pageCount > 1) {
                        if (pageIndex < pageCount - 1) {
                            _tabControl.SelectTab(pageIndex + 1);
                        } else {
                            _tabControl.SelectTab(pageIndex - 1);
                        }
                    } else {
                        _CurrentEditorCanvas = null;
                    }
                }
                _tabControl.TabPages.Remove(page);

                pageContent.Dispose();
                page.Dispose();
            }

            return ret;
        }

        /// <summary>
        /// pageがスタートページならnullを返す。
        /// </summary>
        public MemoInfo ClosePage(TabPage page) {
            var ret = default(MemoInfo);

            if (IsMemoTabPage(page)) {
                ret = CloseMemoPage(page);
            } else if (IsStartPageTabPage(page)) {
                CloseStartPage();
                ret = null;
            }

            return ret;
        }


        /// <summary>
        /// すべてのページを閉じる．
        /// </summary>
        public IEnumerable<MemoInfo> CloseAllPages() {
            var ret = new List<MemoInfo>();

            if (_tabControl.TabCount > 0) {
                var infos = CloseOtherPages();
                ret.AddRange(infos);
                var page = _tabControl.TabPages[0];
                var info = ClosePage(page);
                if (info != null) {
                    ret.Add(info);
                }
            }

            return ret;
        }

        /// <summary>
        /// 選択されていないすべてのページを閉じる．
        /// </summary>
        public IEnumerable<MemoInfo> CloseOtherPages() {
            var ret = new List<MemoInfo>();

            if (_tabControl.TabCount > 0) {
                _tabControl.SuspendLayout();
                for (int i = 0, len = _tabControl.TabCount; i < len; ++i) {
                    var page = default(TabPage);
                    if (_tabControl.SelectedIndex == 0) {
                        if (_tabControl.TabCount > 1) {
                            page = _tabControl.TabPages[1];
                        } else {
                            page = null;
                        }
                    } else {
                        page = _tabControl.TabPages[0];
                    }

                    if (page != null) {
                        var info = ClosePage(page);
                        if (info != null) {
                            ret.Add(info);
                        }
                    }
                }
                _tabControl.ResumeLayout();
            }

            return ret;
        }

        public void SavePage(TabPage page) {
            if (!IsMemoTabPage(page)) {
                return;
            }

            var pageContent = (PageContent) page.Tag;
            SavePageContent(pageContent);
        }

        public TabPage FindMemoTabPage(Predicate<TabPage> pred) {
            foreach (var page in MemoTabPages) {
                if (pred(page)) {
                    return page;
                }
            }

            return null;
        }

        public EditorCanvas GetEditorCanvas(TabPage page) {
            var content = page.Tag as PageContent;
            if (content != null) {
                return content.EditorCanvas;
            }
            return null;
        }

        public PageContent GetPageContent(TabPage page) {
            return page.Tag as PageContent;
        }

        public MemoInfo GetMemoInfo(TabPage page) {
            var content = page.Tag as PageContent;
            if (content != null) {
                return content.MemoInfo;
            }
            return null;
        }

        public bool IsMemoOpened(MemoInfo info) {
            var found = FindMemoTabPage(
                page => {
                    var pageContent = (PageContent) page.Tag;
                    return info == pageContent.MemoInfo;
                }
            );
            return found != null;
        }

        public PageContent FindPageContent(MemoInfo info) {
            var found = FindMemoTabPage(
                page => {
                    var pageContent = (PageContent) page.Tag;
                    return info == pageContent.MemoInfo;
                }
            );
            return found.Tag as PageContent;
        }

        public void EnsureFocusCommited(TabPage page) {
            if (page == null || !IsMemoTabPage(page)) {
                return;
            }
            var pageContent = (PageContent) page.Tag;
            var canvas = pageContent.EditorCanvas;
            EnsureFocusCommited(canvas);
        }

        public void EnsureFocusCommited() {
            foreach (var tabPage in MemoTabPages) {
                EnsureFocusCommited(tabPage);
            }
        }

        public void ShowWorkspaceView() {
            _workspaceView.Show();
            if (_memoQueryBuilderView.IsValueCreated) {
                _memoQueryBuilderView.Value.Hide();
            }
        }

        public void ShowMemoQueryBuilderView() {
            if (_memoQueryBuilderView.IsValueCreated) {
                _memoQueryBuilderView.Value.Show();
                _memoListView.TargetKind = MemoListTargetKind.QueryBuilder;
            }
            _workspaceView.Hide();
        }

        public void SaveFormSettings(MemopadWindowSettings windowSettings) {
            if (!_isShown) {
                /// 一度も表示されていない場合意味がないのでなにもしない
                return;
            }

            if (_IsCompact) {
                windowSettings.WindowLocation = _normalWindowLocation;
                windowSettings.WindowSize = _normalWindowSize;
                if (WindowState == FormWindowState.Normal) {
                    windowSettings.CompactWindowLocation = Location;
                    windowSettings.CompactWindowSize = Size;
                    windowSettings.WindowTopMost = TopMost;
                } else {
                    windowSettings.CompactWindowLocation = RestoreBounds.Location;
                    windowSettings.CompactWindowSize = RestoreBounds.Size;
                }
            } else {
                windowSettings.CompactWindowLocation = _compactWindowLocation;
                windowSettings.CompactWindowSize = _compactWindowSize;
                if (WindowState == FormWindowState.Normal) {
                    windowSettings.WindowLocation = Location;
                    windowSettings.WindowSize = Size;
                    windowSettings.WindowTopMost = TopMost;
                } else {
                    windowSettings.WindowLocation = RestoreBounds.Location;
                    windowSettings.WindowSize = RestoreBounds.Size;
                }
            }
            windowSettings.WindowMaximized = (WindowState == FormWindowState.Maximized);

            if (Visible && !_IsCompact && WindowState != FormWindowState.Minimized) {
                /// 非表示時は正しい値が取れないので保存をあきらめる

                /// save finder pane settings
                if (IsFinderPaneCollapsed) {
                    windowSettings.WorkspaceSplitterCollapsed = true;
                    windowSettings.WorkspaceSplitterDistance = _finderPaneWidth;
                } else {
                    windowSettings.WorkspaceSplitterCollapsed = false;
                    windowSettings.WorkspaceSplitterDistance = _finderSplitContainer.SplitterDistance;
                }
                windowSettings.ShowDescendantTagsMemo =
                    _workspaceView.WorkspaceTree.TagTreePresenter.FindMemoWithDescendants;
    
                /// save memo list pane settings
                if (IsMemoListPaneCollapsed) {
                    windowSettings.MemoListSplitterCollapsed = true;
                    windowSettings.MemoListSplitterDistance = _memoListPaneWidth;
                } else {
                    windowSettings.MemoListSplitterCollapsed = false;
                    windowSettings.MemoListSplitterDistance = _memoListSplitContainer.SplitterDistance;
                }

                windowSettings.MemoListBoxDisplayItems = _memoListView.MemoListBox.DisplayItems;
                windowSettings.MemoListBoxSortKey = _memoListView.MemoListBox.SortKey;
                windowSettings.MemoListBoxSortsAscendingOrder = _memoListView.MemoListBox.SortsAscendingOrder;
                windowSettings.MemoListBoxSortsImportanceOrder = _memoListView.MemoListBox.SortsImportanceOrder;
            }
        }

        public void FocusConditionTextBox() {
            if (IsFinderPaneCollapsed) {
                ExpandFinderPane();
            }
            _conditionTextBox.Focus();
        }

        public void InvalidateMemoListBox(IEnumerable<MemoInfo> infos) {
            _memoListView.MemoListBox.InvalidateList(infos);
        }

        public void ShowStatusMessage(string msg) {
            _messageToolStripStatusLabel.Text = msg;
            var timer = _HideMessageTimer;
            timer.Stop();
            timer.Start();
        }

        // ------------------------------
        // protected
        // ------------------------------
        //protected override void OnClientSizeChanged(EventArgs e) {
        //    base.OnClientSizeChanged(e);
        protected override void OnResize(EventArgs e) {
            base.OnResize(e);

            if (WindowState == FormWindowState.Minimized) {
                if (_app.WindowSettings.MinimizeToTaskTray) {
                    ShowInTaskbar = false;
                    Hide();
                }
            }
        }

        //protected override void OnActivated(EventArgs e) {
        //    base.OnActivated(e);

        //    /// アクティブにしてもたまにメモリストペインが再描画されないときがあるので
        //    if (_memoListView != null && _memoListView.MemoListBox != null) {
        //        _memoListView.MemoListBox.InvalidateWithoutEraceBackground();
        //    }
        //}

        //protected override void WndProc(ref Message m) {
        //    if (m.Msg == (int) WindowMessage.SYSCOMMAND && m.WParam == (IntPtr) 0xF020) {
        //        if (_app.WindowSettings.MinimizeToTaskTray) {
        //            ShowInTaskbar = false;
        //            Hide();
        //        }
        //    }
        //    base.WndProc(ref m);
        //}

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (_conditionTextBox.Focused) {
                if (_conditionTextBoxKeyMap != null && _conditionTextBoxKeyMap.IsDefined(keyData)) {
                    var action = _conditionTextBoxKeyMap.GetAction(keyData);
                    if (action != null) {
                        if (action(_conditionTextBox.TextBox)) {
                            return true;
                        }
                    }
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override PageContent CreateMemoPageContent(MemoInfo info) {
            if (_fontNameToolStripComboBox.Items.Count == 0) {
                _UILogic.InitFontNameToolStripComboBox(_fontNameToolStripComboBox.ComboBox);
                _UILogic.InitFontSizeToolStripComboBox(_fontSizeToolStripComboBox.ComboBox);
            }

            var ret = base.CreateMemoPageContent(info);
            ret.TitleChanged += HandleMemoTitleChanged;
            return ret;
        }

        protected override void DisposeMemoPageContent(PageContent pageContent) {
            pageContent.TitleChanged -= HandleMemoTitleChanged;
            base.DisposeMemoPageContent(pageContent);
        }

        protected override void FocusEditorCanvas() {
            if (_currentEditorCanvas != null) {
                _currentEditorCanvas.Select();
            }
        }

        // ------------------------------
        // internal
        // ------------------------------
        internal void CancelSearch() {
            ShowWorkspaceView();
            _memoQueryBuilderView.Value.ClearConditions();
            _memoQueryBuilderView.Value.UpdateQuery();

            /// ハイライト解除
            _GlobalHighlight = null;
        }

        // ------------------------------
        // private
        // ------------------------------
        private TabPage CreateStartPageTabPage() {
            var page = new TabPage("スタートページ");
            var color = KryptonManager.CurrentGlobalPalette.GetBackColor1(
                PaletteBackStyle.PanelClient,
                PaletteState.Normal
            );
            page.BackColor = color;
            var content = new StartPageContent();
            page.Padding = new Padding(2);
            page.Tag = content;
            content.Dock = DockStyle.Fill;
            page.Controls.Add(content);
            
            return page;
        }

        private TabPage CreateMemoTabPage(string title, PageContent pageContent) {
            var page = new TabPage(title);
            page.Size = _tabControl.ClientSize;
            var color = KryptonManager.CurrentGlobalPalette.GetBackColor1(
                PaletteBackStyle.PanelClient,
                PaletteState.Normal
            );
            page.BackColor = color;
            page.Padding = new Padding(2);
            page.Tag = pageContent;
            pageContent.Size = page.ClientSize;
            page.Controls.Add(pageContent);
            return page;
        }

        protected internal override void UpdateToolStrip() {
            if (_SuppressToolStripUpdate) {
                return;
            }

            if (_currentEditorCanvas == null || !_currentEditorCanvas.Enabled) {
                _showAsFusenToolStripButton.Enabled = false;

                _importantToolStripButton.Enabled = false;
                _unimportantToolStripButton.Enabled = false;
                _memoMarkToolStripSplitButton.Enabled = false;

                _cutToolStripButton.Enabled = false;
                _copyToolStripButton.Enabled = false;
                _pasteToolStripButton.Enabled = false;
                _undoToolStripButton.Enabled = false;
                _redoToolStripButton.Enabled = false;
                _searchInMemoToolStripButton.Enabled = false;

                DisableParagraphKindComboBox();

                if (_fontNameToolStripComboBox.Enabled) {
                    _fontNameToolStripComboBox.Enabled = false;
                }
                if (_fontSizeToolStripComboBox.Enabled) {
                    _fontSizeToolStripComboBox.Enabled = false;
                }
                _fontBoldToolStripButton.Enabled = false;
                _fontItalicToolStripButton.Enabled = false;
                _fontUnderlineToolStripButton.Enabled = false;
                _fontStrikeoutToolStripButton.Enabled = false;

                SetTextColorButtonToolStripItemEnabled(_textColorButtonToolStripItem, false);

                _leftHorizontalAlignmentToolStripButton.Enabled = false;
                _centerHorizontalAlignmentToolStripButton.Enabled = false;
                _rightHorizontalAlignmentToolStripButton.Enabled = false;
                _verticalAlignToolStripDropDownButton.Enabled = false;

                _orderedListToolStripButton.Enabled = false;
                _unorderedListToolStripButton.Enabled = false;
                _specialListToolStripButton.Enabled = false;
                _selectSpecialListToolStripDropDownButton.Enabled = false;

                _selectToolToolStripButton.Enabled = false;
                _handToolToolStripButton.Enabled = false;
                _adjustSpaceToolToolStripButton.Enabled = false;
                _addFreehandToolStripDropDownButton.Enabled = false;
                _addFigureToolStripDropDownButton.Enabled = false;
                _addImageToolStripButton.Enabled = false;
                _addFileToolStripDropDownButton.Enabled = false;
                _addTableToolStripButton.Enabled = false;

                _setNodeStyleToolStripDropDownButton.Enabled = false;
                _setLineStyleToolStripDropDownButton.Enabled = false;
                SetShapeColorButtonToolStripItemEnabled(_shapeColorButtonToolStripItem, false);

                _addCommentToolStripButton.Enabled = false;

                DisableParagraphPropUI();
                return;
            }

            /// Enabledの設定
            var canCut = _currentEditorCanvas.CanCut();
            var canCopy = _currentEditorCanvas.CanCopy();
            var canPaste = _currentEditorCanvas.CanPaste();
            var canUndo = _currentEditorCanvas.CanUndo();
            var canRedo = _currentEditorCanvas.CanRedo();

            var canModFontName = _currentEditorCanvas.CanModifyFontName();
            var canModFontSize = _currentEditorCanvas.CanModifyFontSize();
            var canModFontStyle = _currentEditorCanvas.CanModifyFontStyle();
            var canModifyHAlign = _currentEditorCanvas.CanModifyHorizontalAlignment();
            var canModifyVAlign = _currentEditorCanvas.CanModifyVerticalAlignment();
            var canModifyListKind = _currentEditorCanvas.CanModifyListKind();

            _showAsFusenToolStripButton.Enabled = true;

            _importantToolStripButton.Enabled = true;
            _unimportantToolStripButton.Enabled = true;
            _memoMarkToolStripSplitButton.Enabled = true;

            _cutToolStripButton.Enabled = canCut;
            _copyToolStripButton.Enabled = canCopy;
            _pasteToolStripButton.Enabled = canPaste;
            _undoToolStripButton.Enabled = canUndo;
            _redoToolStripButton.Enabled = canRedo;
            _searchInMemoToolStripButton.Enabled = true;

            _fontNameToolStripComboBox.Enabled = canModFontName;
            _fontSizeToolStripComboBox.Enabled = canModFontSize;
            _fontBoldToolStripButton.Enabled = canModFontStyle;
            _fontItalicToolStripButton.Enabled = canModFontStyle;
            _fontUnderlineToolStripButton.Enabled = canModFontStyle;
            _fontStrikeoutToolStripButton.Enabled = canModFontStyle;

            SetTextColorButtonToolStripItemEnabled(_textColorButtonToolStripItem, _currentEditorCanvas.CanModifyTextColor());

            _leftHorizontalAlignmentToolStripButton.Enabled = canModifyHAlign;
            _centerHorizontalAlignmentToolStripButton.Enabled = canModifyHAlign;
            _rightHorizontalAlignmentToolStripButton.Enabled = canModifyHAlign;
            _verticalAlignToolStripDropDownButton.Enabled = canModifyVAlign;

            _orderedListToolStripButton.Enabled = canModifyListKind;
            _unorderedListToolStripButton.Enabled = canModifyListKind;
            _specialListToolStripButton.Enabled = canModifyListKind;
            _selectSpecialListToolStripDropDownButton.Enabled = canModifyListKind;

            _selectToolToolStripButton.Enabled = true;
            _handToolToolStripButton.Enabled = true;
            _adjustSpaceToolToolStripButton.Enabled = true;
            _addFreehandToolStripDropDownButton.Enabled = true;
            _addFigureToolStripDropDownButton.Enabled = true;
            _addImageToolStripButton.Enabled = true;
            _addFileToolStripDropDownButton.Enabled = true;
            _addTableToolStripButton.Enabled = true;

            _setNodeStyleToolStripDropDownButton.Enabled =
                !_currentEditorCanvas.FocusManager.IsEditorFocused &&
                _currentEditorCanvas.SelectionManager.SelectedEditors.Any(e => e.Model is MemoShape);
                //_currentEditorCanvas.SelectionManager.SelectedEditors.Any(e => e.Model is MemoShape || e.Model is MemoTableCell);
            _setLineStyleToolStripDropDownButton.Enabled =
                !_currentEditorCanvas.FocusManager.IsEditorFocused &&
                _currentEditorCanvas.SelectionManager.SelectedEditors.Any(e => e.Model is MemoEdge);
                //_currentEditorCanvas.SelectionManager.SelectedEditors.Any(e => e.Model is MemoShape || e.Model is MemoTableCell);
            SetShapeColorButtonToolStripItemEnabled(
                _shapeColorButtonToolStripItem,
                !_currentEditorCanvas.FocusManager.IsEditorFocused &&
                _currentEditorCanvas.SelectionManager.SelectedEditors.Any(
                    e => e.Model is MemoShape || e.Model is MemoTableCell
                )
            );


            //_memoMarkToolStripSplitButton.Enabled =
                //_currentEditorCanvas.SelectionManager.SelectedEditors.Any(
                //    editor => {
                //        if (editor != null && editor.IsEnabled) {
                //            var content = editor.Model as MemoContent;
                //            return content != null && content.IsMarkable;
                //        }
                //        return false;
                //    }
                //);

            /// check
            var memo = _currentEditorCanvas.EditorContent as Memo;
            _importantToolStripButton.Checked = memo.Importance == MemoImportanceKind.High;
            _unimportantToolStripButton.Checked = memo.Importance == MemoImportanceKind.Low;

            _selectToolToolStripButton.Checked = _currentEditorCanvas.Tool is SelectTool;
            _handToolToolStripButton.Checked = _currentEditorCanvas.Tool is HandTool;            
            _adjustSpaceToolToolStripButton.Checked = _currentEditorCanvas.Tool is AdjustSpaceTool;

            if (_currentEditorCanvas.FocusManager.IsEditorFocused) {
                /// フォーカスあり
                var focus = _currentEditorCanvas.FocusManager.Focus as StyledTextFocus;
                var font = focus.GetNextInputFont();

                SetFontComboBoxTextWithoutEventHandling(
                    canModFontName? font.Name: "",
                    canModFontSize? font.Size.ToString(): ""
                );
                _fontBoldToolStripButton.Checked = canModFontStyle? font.IsBold: false;
                _fontItalicToolStripButton.Checked = canModFontStyle? font.IsItalic: false;
                _fontUnderlineToolStripButton.Checked = canModFontStyle? font.IsUnderline: false;
                _fontStrikeoutToolStripButton.Checked = canModFontStyle? font.IsStrikeout: false;

                var model = _currentEditorCanvas.FocusManager.FocusedEditor.Model;
                if (model is MemoText || model is MemoShape || model is MemoTableCell) {
                    /// MemoTextとMemoShapeとMemoTableCellだけ有効にしておく
                    var para = focus.GetBlockAtCaretIndex() as Paragraph;
                    if (para != null) {
                        EnableParagraphKindComboBox();
                        var paraKind = focus.GetParagraphKind();
                        var paraKindText = paraKind == null ? "" : GetStringFromParagraphKind(paraKind.Value);
                        SetParagraphKindComboBoxTextWithoutEventHandling(paraKindText);

                        _unorderedListToolStripButton.Enabled = true;
                        _orderedListToolStripButton.Enabled = true;
                        _specialListToolStripButton.Enabled = true;
                        _selectSpecialListToolStripDropDownButton.Enabled = true;
                        _indentToolStripButton.Enabled = para.ListLevel < 10;
                        _outdentToolStripButton.Enabled = para.ListLevel > 0;

                        _unorderedListToolStripButton.Checked = para.ListKind == ListKind.Unordered;
                        _orderedListToolStripButton.Checked = para.ListKind == ListKind.Ordered;
                        _specialListToolStripButton.Checked =
                             para.ListKind == ListKind.CheckBox ||
                             para.ListKind == ListKind.TriStateCheckBox ||
                             para.ListKind == ListKind.Star ||
                             para.ListKind == ListKind.LeftArrow ||
                             para.ListKind == ListKind.RightArrow;

                        _addCommentToolStripButton.Enabled = model is MemoText;
                    } else {
                        DisableParagraphPropUI();
                        _addCommentToolStripButton.Enabled = false;
                    }
                } else {
                    DisableParagraphPropUI();
                    _addCommentToolStripButton.Enabled = false;
                }

            } else {
                /// フォーカスなし
                SetFontComboBoxTextWithoutEventHandling("", "");
                _fontBoldToolStripButton.Checked = false;
                _fontItalicToolStripButton.Checked = false;
                _fontUnderlineToolStripButton.Checked = false;
                _fontStrikeoutToolStripButton.Checked = false;

                _unorderedListToolStripButton.Checked = false;
                _orderedListToolStripButton.Checked = false;
                _specialListToolStripButton.Checked = false;

                DisableParagraphPropUI();
                _addCommentToolStripButton.Enabled = false;
            }
        }

        //protected internal override void UpdateSpecialListImage() {
        //    if (_EditorCanvas == null) {
        //        return;
        //    }

        //    if (_EditorCanvas.FocusManager.IsEditorFocused) {
        //        var focus = _EditorCanvas.FocusManager.Focus as StyledTextFocus;
        //        var kind = focus.CurrentSpecialListKind;
        //        var image = _specialListToolStripButton.Image;

        //        if (kind == ListKind.Star) {
        //            _specialListToolStripButton.Image = Resources.list_special;
        //        } else if (kind == ListKind.LeftArrow) {
        //            _specialListToolStripButton.Image = Resources.list_left_arrow;
        //        } else if (kind == ListKind.RightArrow) {
        //            _specialListToolStripButton.Image = Resources.list_right_arrow;
        //        }
        //    }
        //}

        private void DisableParagraphPropUI() {
            DisableParagraphKindComboBox();
            _indentToolStripButton.Enabled = false;
            _outdentToolStripButton.Enabled = false;
        }

        private void SetFontComboBoxTextWithoutEventHandling(string fontName, string fontSize) {
            _SuppressFontChangeEventHandler = true;
            try {
                if (_fontNameToolStripComboBox.Text != fontName) {
                    _fontNameToolStripComboBox.Text = fontName;
                }
                if (_fontSizeToolStripComboBox.Text != fontSize) {
                    _fontSizeToolStripComboBox.Text = fontSize;
                }
            } finally {
                _SuppressFontChangeEventHandler = false;
            }
        }

        private void ExpandFinderPane() {
            if (_finderSplitContainer.IsSplitterFixed) {
                SuspendLayout();

                _finderSplitContainer.IsSplitterFixed = false;

                _finderHeaderGroup.ButtonSpecs[0].Visible = true;
                _finderHeaderGroup.ButtonSpecs[1].Type = PaletteButtonSpecStyle.ArrowLeft;

                _finderHeaderGroup.Collapsed = false;
                _finderSplitContainer.Panel1MinSize = 100;
                _finderSplitContainer.SplitterDistance = _finderPaneWidth;

                _finderHeaderGroup.StateCommon.HeaderPrimary.Content.ShortText.TextH = PaletteRelativeAlign.Near;
                _finderHeaderGroup.HeaderPositionPrimary = VisualOrientation.Top;

                ResumeLayout();
            }
        }

        private void CollapseWorkspacePane() {
            if (!_finderSplitContainer.IsSplitterFixed) {
                SuspendLayout();

                _finderSplitContainer.IsSplitterFixed = true;

                _finderPaneWidth = _finderHeaderGroup.Width;

                _finderHeaderGroup.ButtonSpecs[0].Visible = false;
                _finderHeaderGroup.ButtonSpecs[1].Type = PaletteButtonSpecStyle.ArrowRight;

                _finderHeaderGroup.Collapsed = true;
                var newWidth = _finderHeaderGroup.PreferredSize.Height;
                _finderSplitContainer.Panel1MinSize = newWidth;
                _finderSplitContainer.SplitterDistance = newWidth + 2;

                _finderHeaderGroup.StateCommon.HeaderPrimary.Content.ShortText.TextH = PaletteRelativeAlign.Center;
                _finderHeaderGroup.HeaderPositionPrimary = VisualOrientation.Left;

                ResumeLayout();
            }
        }

        private void ExpandMemoListPane() {
            if (_memoListSplitContainer.IsSplitterFixed) {
                SuspendLayout();

                _memoListSplitContainer.IsSplitterFixed = false;

                _memoListHeaderGroup.ButtonSpecs[0].Visible = true;
                _memoListHeaderGroup.ButtonSpecs[1].Visible = true;
                _memoListHeaderGroup.ButtonSpecs[2].Visible = true;
                _memoListHeaderGroup.ButtonSpecs[3].Type = PaletteButtonSpecStyle.ArrowLeft;

                _memoListHeaderGroup.Collapsed = false;
                _memoListSplitContainer.Panel1MinSize = 100;
                _memoListSplitContainer.SplitterDistance = _memoListPaneWidth;

                _memoListHeaderGroup.StateCommon.HeaderPrimary.Content.ShortText.TextH = PaletteRelativeAlign.Near;
                _memoListHeaderGroup.HeaderPositionPrimary = VisualOrientation.Top;

                ResumeLayout();
            }
        }

        private void CollapseMemoListPane() {
            if (!_memoListSplitContainer.IsSplitterFixed) {
                SuspendLayout();

                _memoListSplitContainer.IsSplitterFixed = true;

                _memoListPaneWidth = _memoListHeaderGroup.Width;

                _memoListHeaderGroup.ButtonSpecs[0].Visible = false;
                _memoListHeaderGroup.ButtonSpecs[1].Visible = false;
                _memoListHeaderGroup.ButtonSpecs[2].Visible = false;
                _memoListHeaderGroup.ButtonSpecs[3].Type = PaletteButtonSpecStyle.ArrowRight;

                _memoListHeaderGroup.Collapsed = true;
                var newWidth = _memoListHeaderGroup.PreferredSize.Height;
                _memoListSplitContainer.Panel1MinSize = newWidth;
                _memoListSplitContainer.SplitterDistance = newWidth + 2;

                _memoListHeaderGroup.StateCommon.HeaderPrimary.Content.ShortText.TextH = PaletteRelativeAlign.Center;
                _memoListHeaderGroup.HeaderPositionPrimary = VisualOrientation.Left;

                ResumeLayout();
            }
        }


        private void SetCompact(bool compact) {
            if (compact == _IsCompact) {
                return;
            }

            _IsCompact = compact;
            if (_IsCompact) {
                _finderSplitContainer.Panel1Collapsed = true;
                _memoListSplitContainer.Panel1Collapsed = true;
                _normalWindowLocation = Location;
                _normalWindowSize = Size;
                Bounds = new Rectangle(_compactWindowLocation, _compactWindowSize);
            } else {
                _compactWindowLocation = Location;
                _compactWindowSize = Size;
                Bounds = new Rectangle(_normalWindowLocation, _normalWindowSize);
                _finderSplitContainer.Panel1Collapsed = false;
                _memoListSplitContainer.Panel1Collapsed = false;
            }

            foreach (TabPage page in MemoTabPages) {
                var content = (PageContent) page.Tag;
                content.SetCompact(_IsCompact);
            }
        }

        private bool IsMemoTabPage(TabPage page) {
            return page != null && page.Tag is PageContent;
        }

        private bool IsStartPageTabPage(TabPage page) {
            return page != null && page.Tag is StartPageContent;
        }

        private void ToggleEditorSizeMaximized() {
            if (_IsCompact) {
                /// コンパクトウィンドウ時は何もしない
                return;
            }

            if (IsFinderPaneCollapsed && IsMemoListPaneCollapsed) {
                ExpandFinderPane();
                ExpandMemoListPane();
            } else {
                CollapseWorkspacePane();
                CollapseMemoListPane();
            }
        }

    }
}
