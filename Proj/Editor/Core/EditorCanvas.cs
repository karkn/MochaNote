/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using Mkamo.Figure.Figures;
using Mkamo.Figure.Layouts;
using System.ComponentModel;
using Mkamo.Editor.Controllers;
using Mkamo.Editor.Internal.Core;
using Mkamo.Editor.Tools;
using System.Windows.Forms;
using Mkamo.Common.Externalize;
using Mkamo.Editor.Internal.Editors;
using Mkamo.Common.Forms.Input;
using Mkamo.Common.Win32.Imm32;
using Mkamo.Common.Win32;
using System.Drawing;
using Mkamo.Common.Command;
using ICommandExecutor = Mkamo.Common.Command.ICommandExecutor;
using System.Runtime.InteropServices;
using Mkamo.Common.Forms.Drawing;
using Mkamo.Common.DataType;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Editor.Requests;
using System.Drawing.Imaging;
using System.IO;
using Mkamo.Common.Win32.User32;
using Mkamo.Editor.Focuses;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Mkamo.Common.Win32.Gdi32;
using Mkamo.Common.Forms.Themes;
using Mkamo.Common.Win32.Core;
using Mkamo.Common.Forms.Clipboard;
using Mkamo.Common.Core;
using System.Drawing.Printing;
using Mkamo.Common.String;
using Mkamo.Common.Forms.KeyMap;

namespace Mkamo.Editor.Core {
    public class EditorCanvas: Canvas {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly Size BitmapMargin = new Size(4, 4);

        // ========================================
        // field
        // ========================================
        private IFigure _primaryLayer;
        private Layer _showOnPointHandleLayer;
        private Layer _handleLayer;
        private Layer _feedbackLayer;
        private Layer _focusLayer;

        private RootEditor _rootEditor;
        private IEditorSite _site;

        private ITool _tool;

        private IControllerFactory _controllerFactory;
        private ICommandExecutor _commandExecutor;

        private ISelectionManager _selectionManager;
        private IFocusManager _focusManager;

        //private IEditorDataAggregatorManager _aggregatorManager;
        private IEditorCopyExtenderManager _copyExtenderManager;

        private IContextMenuProvider _contextMenuProvider;

        private ContextMenuStrip _contextMenu;
        private ToolStripDropDown _miniToolBar;
        private Color _miniToolBarBorderColor;

        private Caret _caret;
        private IntPtr _immHandle;

        private IGridService _gridService;

        private ITheme _theme;

        private IKeyMap<EditorCanvas> _noSelectionKeyMap;

        private string _imageCopyRightString = null;

        private Lazy<IDictionary<string, object>> _transientData; /// lazy

        private Lazy<AbbrevWordProvider> _abbrevWordProvider;

        private bool _isInImeComposition = false;
        private Keys _immVirtualKey = Keys.None;

        // ========================================
        // constructor
        // ========================================
        public EditorCanvas(): base() {
            RootFigure.Layout = new StackLayout();

            SetStyle(ControlStyles.Selectable, true);

            FigureContent = new Layer();
            FigureContent.Layout = new StackLayout();

            FigureContent.Children.Add(_primaryLayer = new Layer());
            FigureContent.Children.Add(_showOnPointHandleLayer = new BuriedLayer());
            FigureContent.Children.Add(_handleLayer = new Layer());
            FigureContent.Children.Add(_focusLayer = new Layer());
            FigureContent.Children.Add(_feedbackLayer = new MirageLayer());

            AutoAdjustExcepter = fig => {
                if (fig == _handleLayer || fig == _showOnPointHandleLayer) {
                    return AutoAdjustExceptionKind.ExceptThisAndChildren;
                } else if (fig is ILayer) {
                    /// MemoなどEditorのContentになるfigure
                    return AutoAdjustExceptionKind.ExceptThis;
                } else {
                    return AutoAdjustExceptionKind.None;
                }
            };

            _gridService = new DefaultGridService();

            _controllerFactory = EditorConsts.NullControllerFactory;
            _commandExecutor = new CommandExecutor();

            _site = new EditorSite(this);

            _rootEditor = new RootEditor(this);
            _rootEditor.Enable();
            _selectionManager = new SelectionManager(_rootEditor);
            _focusManager = new FocusManager(_rootEditor);

            //_aggregatorManager = new EditorDataAggregatorManager();
            _copyExtenderManager = new EditorCopyExtenderManager();

            _miniToolBarBorderColor = SystemColors.ControlDark;

            _transientData = new Lazy<IDictionary<string, object>>(() => new Dictionary<string, object>());

            ConfigureEditorCanvas();

            if (_tool == null) {
                _tool = new SelectTool();
            }
            if (_contextMenuProvider == null) {
                _contextMenuProvider = new DefaultContextMenuProvider(this);
            }

            _abbrevWordProvider = new Lazy<AbbrevWordProvider>(() => new AbbrevWordProvider(this));

            GotFocus += HandleGotFocus;
        }


        // ========================================
        // destructor
        // ========================================
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (_DirtManager != null) {
                    _DirtManager.IsEnabled = false;
                    _site.SuppressUpdateHandleLayer = true;
                    using (_DirtManager.BeginDirty()) {
                        _rootEditor.Accept(
                            editor => {
                                /// editor.Disable()だといらないFigure.IsVisible = falseを呼んでしまう    
                                editor.Disable(true);
                                return false;
                            }
                        );
                    }
                }
    
                if (_caret != null) {
                    _caret.Dispose();
                }
                if (_contextMenuProvider != null) {
                    _contextMenuProvider.Dispose();
                }
                if (_contextMenu != null) {
                    _contextMenu.Dispose();
                }
                if (_miniToolBar != null) {
                    _miniToolBar.Dispose();
                }
            }

            base.Dispose(disposing);
            GC.SuppressFinalize(this);
        }

        // ========================================
        // event
        // ========================================
        public event EventHandler ToolChanged;

        public event EventHandler ImeStartComposition;
        public event EventHandler ImeEndComposition;
        public event EventHandler ImeComposition;


        // ========================================
        // property
        // ========================================
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ITheme Theme {
            get { return _theme; }
            set {
                if (value == _theme) {
                    return;
                }
                _theme = value;

                var menuFont = value.MenuFont;
                _contextMenu.Font = menuFont;
                _miniToolBar.Font = _theme.CaptionFont;
            }
        }

        [Browsable(true)]
        public bool MultiSelect {
            get { return _selectionManager.MultiSelect; }
            set { _selectionManager.MultiSelect = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IRootEditor RootEditor {
            get { return _rootEditor; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object EditorContent {
            get { return _rootEditor.Content == null? null: _rootEditor.Content.Model; }
            set {
                if (value == EditorContent) {
                    return;
                }
                if (value != null) {
                    var editor = EditorFactory.CreateEditor(value, _controllerFactory);
                    //_owner._EditorSite.SuppressUpdateHandleLayer = false;
                    _rootEditor.Content = editor;
                    //_owner._EditorSite.SuppressUpdateHandleLayer = true;
                    editor.Enable();
                }
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IControllerFactory ControllerFactory {
            get { return _controllerFactory; }
            set {
                if (value == _controllerFactory) {
                    return;
                }
                _controllerFactory = value?? EditorConsts.NullControllerFactory;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ICommandExecutor CommandExecutor {
            get { return _commandExecutor; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ITool Tool {
            get { return _tool; }
            set {
                if (value == _tool) {
                    return;
                }
                if (_tool != null) {
                    _tool.Uninstalled(this);
                }
                _tool = value ?? new SelectTool();
                if (_tool != null) {
                    _tool.Installed(this);
                }
                OnToolChanged();
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IContextMenuProvider ContextMenuProvider {
            get { return _contextMenuProvider; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IFocusManager FocusManager {
            get { return _focusManager; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ISelectionManager SelectionManager {
            get { return _selectionManager; }
        }

        //[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //public IEditorDataAggregatorManager EditorDataAggregatorManager {
        //    get { return _aggregatorManager; }
        //}
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEditorCopyExtenderManager EditorCopyExtenderManager {
            get { return _copyExtenderManager; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Caret Caret {
            get {
                if (_caret == null) {
                    _caret = new Caret(this, (pos => TranslateToControlPoint(pos)));
                    _caret.Hide();
                    _caret.CaretMoved += HandleCaretMoved;
                }
                return _caret;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IGridService GridService {
            get { return _gridService; }
            set { _gridService = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color MiniToolBarBorderColor {
            get { return _miniToolBarBorderColor; }
            set { _miniToolBarBorderColor = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IKeyMap<EditorCanvas> NoSelectionKeyMap {
            get { return _noSelectionKeyMap; }
            set { _noSelectionKeyMap = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ImageCopyRightString {
            get { return _imageCopyRightString; }
            set { _imageCopyRightString = value; }
        }

        public IDictionary<string, object> TransientData {
            get { return _transientData.Value; }
        }

        public bool IsInImeComposition {
            get { return _isInImeComposition; }
        }

        public EventDispatcher EventDispatcher {
            get { return _EventDispatcher; }
        }

        public IAbbrevWordProvider AbbrevWordProvider {
            get { return _abbrevWordProvider.Value; }
        }

        // ------------------------------
        // internal
        // ------------------------------
        protected internal IFigure _PrimaryLayer {
            get { return _primaryLayer; }
        }

        protected internal IFigure _HandleLayer {
            get { return _handleLayer; }
        }

        protected internal IFigure _ShowOnPointHandleLayer {
            get { return _showOnPointHandleLayer; }
        }

        protected internal IFigure _FeedbackLayer {
            get { return _feedbackLayer; }
        }

        protected internal IFigure _FocusLayer {
            get { return _focusLayer; }
        }

        protected internal ISelectionManager _SelectionManager {
            get { return _selectionManager; }
        }

        protected internal IFocusManager _FocusManager {
            get { return _focusManager; }
        }

        protected internal IEditorSite _EditorSite {
            get { return _site; }
        }

        internal EditorEventDispatcher _EditorEventDispatcher {
            get { return _EventDispatcher as EditorEventDispatcher; }
            set { _EventDispatcher = value; }
        }

        internal Keys _ImmVirtualKey {
            get { return _immVirtualKey; }
        }

        // ========================================
        // method
        // ========================================
        // === Control ==========
        // ------------------------------
        // protected
        // ------------------------------
        protected override bool IsInputChar(char charCode) {
            /// mnemonicキーとして処理させない
            return true;
            //return base.IsInputChar(charCode);
        }

        protected override bool IsInputKey(Keys keyData) {
            switch (keyData & Keys.KeyCode) {
                case Keys.Tab:
                case Keys.Left:
                case Keys.Up:
                case Keys.Right:
                case Keys.Down: {
                    return true;
                }

                default: {
                    return base.IsInputKey(keyData);
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            Focus();
            base.OnMouseDown(e);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (keyData == Keys.ProcessKey) {
                _immVirtualKey = (Keys) Imm32PI.ImmGetVirtualKey(Handle);
            }

            var e = new ShortcutKeyProcessEventArgs(keyData);
            _EditorEventDispatcher.HandleShortcutKeyProcess(e);
            if (e.Handled) {
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnKeyPress(KeyPressEventArgs e) {
 			base.OnKeyPress(e);
            _EditorEventDispatcher.HandleKeyPress(e);
			e.Handled = true; /// これがないとIME ON時に二重にKeyPressが発火する
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);
            _EditorEventDispatcher.HandleKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e) {
            base.OnKeyUp(e);
            _EditorEventDispatcher.HandleKeyUp(e);
        }

        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e) {
            base.OnPreviewKeyDown(e);
            _EditorEventDispatcher.HandlePreviewKeyDown(e);
        }
        
        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                //case (int) Imm32WindowMessage.WM_IME_SETCONTEXT: {
                //    var ret = Imm32PI.ImmAssociateContextEx(
                //        Handle,
                //        _immHandle,
                //        ImmAssociateContextExFlag.IACE_DEFAULT
                //    );
                //    if (ret) {
                //        DefWndProc(ref m);
                //    } else {
                //        base.WndProc(ref m);
                //    }
                //    break;
                //}

                case (int) Imm32WindowMessage.WM_IME_STARTCOMPOSITION: {
                    OnImeStartComposition();
                    base.WndProc(ref m);
                    break;
                }

                case (int) Imm32WindowMessage.WM_IME_ENDCOMPOSITION: {
                    OnImeEndComposition();
                    base.WndProc(ref m);
                    break;
                }

                case (int) Imm32WindowMessage.WM_IME_COMPOSITION: {
                    OnImeComposition();
                    base.WndProc(ref m);
                    break;
                }

                default: {
                    base.WndProc(ref m);
                    break;
                }
            }
        }

        // === Canvas ==========
        protected override void Configure() {
            base.Configure();

            _contextMenu = new ContextMenuStrip();

            _miniToolBar = new ToolStripDropDown();
            _miniToolBar.Margin = Padding.Empty;
            _miniToolBar.Padding = new Padding(2, 1, 0, 0);
            _miniToolBar.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            //_miniToolBar.Renderer = new MiniToolBarRenderer(this);
            _miniToolBar.BackColor = Color.White;

            _EditorEventDispatcher = new EditorEventDispatcher(this, _contextMenu, _miniToolBar);
        }

        public ToolStripRenderer MiniToolBarRenderer {
            set { _miniToolBar.Renderer = value; }
        }

        protected override Rectangle CalcPreferredContentsBounds() {
            var ret = base.CalcPreferredContentsBounds();
            if (_caret != null && _caret.IsVisible) {
                var caretRect = _caret.Bounds;
                return Rectangle.Union(ret, caretRect);
            } else {
                return ret;
            }
        }
        
        // === EditorCanvas ==========
        // ------------------------------
        // public
        // ------------------------------
        public IMemento SaveContent(IModelSerializer modelSerializer) {
            var externalizer = new Externalizer();
            externalizer.ExtendedData[EditorConsts.ModelSerializerKey] = modelSerializer;
            externalizer.ExtendedData[EditorConsts.EditorSiteKey] = _site;

            return externalizer.Save(_rootEditor.Content);
        }

        public void LoadContent(IMemento contentMemento, IModelSerializer modelSerializer) {
            var externalizer = new Externalizer();
            externalizer.ExtendedData[EditorConsts.ModelSerializerKey] = modelSerializer;
            externalizer.ExtendedData[EditorConsts.EditorSiteKey] = _site;

            _site.SuppressUpdateHandleLayer = true;

            using (_DirtManager.BeginDirty()) {
                _rootEditor.Content = externalizer.Load(contentMemento) as IEditor;

                var context = new RefreshContext(EditorRefreshKind.Loaded);

                _rootEditor.Accept(
                    editor => {
                        editor.Refresh(context);
                        editor.Enable();
                        return false;
                    }
                );
            }

            _site.SuppressUpdateHandleLayer = false;
        }

        public IEnumerable<IEditor> CloneEditors(IEnumerable<IEditor> editors) {
            var dataObj = EditorFactory.CreateDataObject(editors);
            return EditorFactory.RestoreDataObject(dataObj, _controllerFactory);
        }

        public void EnsureVisible(IEditor editor) {
            var scope = Viewport;
            var bounds = editor.Figure.Bounds;
            if (!scope.Contains(bounds)) {
                EnsurePointShown(new Point(bounds.Right - 1, bounds.Bottom - 1 + 16));
                EnsurePointShown(bounds.Location - new Size(0, 16));
            }
        }

        /// <summary>
        /// Canvasのすべての内容をEMF形式とBitmap形式でクリップボードにコピーする．
        /// </summary>
        public void CopyAsImage() {
            using (var bmp = new Bitmap(1, 1))
            using (var bmpg = Graphics.FromImage(bmp)) {
                var hdc = bmpg.GetHdc();

                using (var mem = new MemoryStream())
                using (var mf = new Metafile(mem, hdc)) {
                    bmpg.ReleaseHdc(hdc);
                    AbstractNode.PreventPaintingLineBreakAndBlockBreak = true;
                    using (var g = Graphics.FromImage(mf)) {
                        _primaryLayer.Paint(g);
                        DrawImageCopyRight(g, GetPrimaryLayersBounds());
                    }
                    AbstractNode.PreventPaintingLineBreakAndBlockBreak = false;

                    var hEmf = mf.GetHenhmetafile();
                    if (User32PI.OpenClipboard(Handle)) {
                        try {
                            if (User32PI.EmptyClipboard()) {
                                var hEmf2 = User32PI.CopyEnhMetaFile(hEmf, IntPtr.Zero);
                                var result = User32PI.SetClipboardData(ClipboardFormat.CF_ENHMETAFILE, hEmf2);
                                if (result == IntPtr.Zero) {
                                    var err = Marshal.GetLastWin32Error();
                                }

                                using (var b = CreateBitmap(1f, BitmapMargin)) {
                                    var p = GetCompatibleBitmap(b);
                                    User32PI.SetClipboardData(ClipboardFormat.CF_BITMAP, p);
                                }
                            }
                        } finally {
                            User32PI.CloseClipboard();
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Canvasの内容を書き込んだBitmapを作成して返す。
        /// </summary>
        /// <returns></returns>
        public Bitmap CreateBitmap(float ratio, Size margin) {
            var rect = GetPrimaryLayersBounds();

            if (ratio != 1f) {
                rect = new Rectangle(
                    rect.Location,
                    new Size((int) (rect.Width * ratio), (int) (rect.Height * ratio))
                );
            }

            var trans = new SizeF(
                -rect.Left + margin.Width,
                -rect.Top + margin.Height
            );
            
            rect.Inflate(margin);

            var ret = new Bitmap(rect.Width, rect.Height);
            using (var g = Graphics.FromImage(ret)) {
                GraphicsUtil.SetupGraphics(g, GraphicQuality.MaxQuality);

                if (ratio != 1f) {
                    g.ScaleTransform(ratio, ratio);
                }

                g.TranslateTransform(trans.Width, trans.Height);

                AbstractNode.PreventPaintingLineBreakAndBlockBreak = true;
                UseGdiPlus = true;

                try {
                    g.Clear(Color.White);
                    _primaryLayer.Paint(g);

                    DrawImageCopyRight(g, rect);

                } finally {
                    AbstractNode.PreventPaintingLineBreakAndBlockBreak = false;
                    UseGdiPlus = false;
                }
            }

            return ret;
        }

        /// <summary>
        /// Canvasのすべての内容をEMF形式で保存する．
        /// </summary>
        public void SaveAsEmf(string fileName) {
            using (var bmp = new Bitmap(1, 1))
            using (var bmpg = Graphics.FromImage(bmp)) {
                var hdc = bmpg.GetHdc();
                using (var mf = new Metafile(fileName, hdc)) {
                    bmpg.ReleaseHdc(hdc);
                    AbstractNode.PreventPaintingLineBreakAndBlockBreak = true;
                    using (var g = Graphics.FromImage(mf)) {
                        _primaryLayer.Paint(g);
                        DrawImageCopyRight(g, GetPrimaryLayersBounds());
                    }
                    AbstractNode.PreventPaintingLineBreakAndBlockBreak = false;
                }
            }
        }

        /// <summary>
        /// Canvasのすべての内容をPNG形式で保存する．
        /// </summary>
        public void SaveAsPng(string fileName) {
            using (var bmp = CreateBitmap(1f, BitmapMargin)) {
                bmp.Save(fileName, ImageFormat.Png);
            }
        }

        public void SaveAsJpeg(string fileName) {
            using (var bmp = CreateBitmap(1f, BitmapMargin)) {
                using (var para = ImageUtil.CreateEncoderParameters(100L)) {
                    var codec = ImageUtil.GetCodec(ImageFormat.Jpeg);
                    bmp.Save(fileName, codec, para);
                }
            }
        }

        public int CalcPrintPages(Graphics g, Rectangle printBounds, int overlap, PrintFitKind fitKind) {
            _CurrentGraphics = g;
            UseGdiPlus = true;
            try {
                /// StyledTextの描画方法が変わるので
                /// キャッシュをクリアさせる
                _primaryLayer.Accept(
                    fig => {
                        var node = fig as AbstractNode;
                        if (node != null) {
                            using (node.BeginUpdateStyledText()) {
                            }
                        }
                        return false;
                    }
                );
                _rootEditor.Accept(
                    editor => {
                        /// 画面と印刷では文字列の描画サイズが大きく変わってしまうので
                        /// MemoTextのように文字列のサイズから図形を求めるようなeditorの場合は
                        /// AdjustSize()しておかないと正しく矩形のサイズをとれない
                        if (editor.Controller.NeedAdjustSizeOnPrint) {
                            var node = editor.Figure as INode;
                            if (node != null) {
                                node.AdjustSize();
                            }
                        }
                        return false;
                    }
                );


                var xy = CalcPrintPageXAndY(g, printBounds, overlap, fitKind);
                return xy.Item1 * xy.Item2;


            } finally {
                _CurrentGraphics = null;

                /// StyledTextの描画方法が変わるので
                /// キャッシュをクリアさせる
                UseGdiPlus = false;
                _primaryLayer.Accept(
                    fig => {
                        var node = fig as AbstractNode;
                        if (node != null) {
                            using (node.BeginUpdateStyledText()) {
                            }
                        }
                        return false;
                    }
                );
                _rootEditor.Accept(
                    editor => {
                        if (editor.Controller.NeedAdjustSizeOnPrint) {
                            var node = editor.Figure as INode;
                            if (node != null) {
                                node.AdjustSize();
                            }
                        }
                        return false;
                    }
                );
            }
        }

        public void PrintPages(Graphics g, Rectangle printBounds, int page, int overlap, PrintFitKind fitKind) {
            _CurrentGraphics = g;
            UseGdiPlus = true;
            AbstractNode.PreventPaintingLineBreakAndBlockBreak = true;
            try {
                /// StyledTextの描画方法が変わるので
                /// キャッシュをクリアさせる
                _primaryLayer.Accept(
                    fig => {
                        var node = fig as AbstractNode;
                        if (node != null) {
                            using (node.BeginUpdateStyledText()) {
                            }
                        }
                        return false;
                    }
                );
                _rootEditor.Accept(
                    editor => {
                        /// 画面と印刷では文字列の描画サイズが大きく変わってしまうので
                        /// MemoTextのように文字列のサイズから図形を求めるようなeditorの場合は
                        /// AdjustSize()しておかないと正しく矩形のサイズをとれない
                        if (editor.Controller.NeedAdjustSizeOnPrint) {
                            var node = editor.Figure as INode;
                            if (node != null) {
                                node.AdjustSize();
                            }
                        }
                        return false;
                    }
                );

                
                //g.Clear(Color.White);

                var figBounds = GetAllFiguresBounds();

                var xyPages = CalcPrintPageXAndY(g, printBounds, overlap, fitKind);
                var xPages = xyPages.Item1;
                var yPages = xyPages.Item2;

                var xIndex = page % xPages;
                var yIndex = page / xPages;

                var ratio = CalcPrintRatio(printBounds, overlap, fitKind);

                /// 原点合わせ
                var x = -((int) Math.Truncate(figBounds.Left * ratio)) + printBounds.Left - (xIndex * (printBounds.Width - overlap));
                var y = -((int) Math.Truncate(figBounds.Top * ratio)) + printBounds.Top - (yIndex * (printBounds.Height - overlap));
                g.TranslateTransform(x, y);

                var xClip = xIndex * (printBounds.Width - overlap);
                var yClip = yIndex * (printBounds.Height - overlap);
                g.SetClip(new Rectangle(
                    xClip,
                    yClip,
                    printBounds.Width + 2,
                    printBounds.Height + 2
                    //(int) Math.Ceiling(printBounds.Width / ratio),
                    //(int) Math.Ceiling(printBounds.Height / ratio)
                ));

                if (ratio < 1) {
                    g.ScaleTransform(ratio, ratio);
                }


                _primaryLayer.Paint(g);
            } finally {
                _CurrentGraphics = null;

                /// StyledTextの描画方法が変わるので
                /// キャッシュをクリアさせる
                UseGdiPlus = false;
                AbstractNode.PreventPaintingLineBreakAndBlockBreak = false;
                _primaryLayer.Accept(
                    fig => {
                        var node = fig as AbstractNode;
                        if (node != null) {
                            using (node.BeginUpdateStyledText()) {
                            }
                        }
                        return false;
                    }
                );
                _rootEditor.Accept(
                    editor => {
                        if (editor.Controller.NeedAdjustSizeOnPrint) {
                            var node = editor.Figure as INode;
                            if (node != null) {
                                node.AdjustSize();
                            }
                        }
                        return false;
                    }
                );
            }
        }


        /// <summary>
        /// Canvasの選択された要素をEMF形式とBitmap形式でクリップボードにコピーする．
        /// </summary>
        public void CopySelectedAsImage() {
            using (var bmp = new Bitmap(1, 1))
            using (var bmpg = Graphics.FromImage(bmp)) {
                var hdc = bmpg.GetHdc();

                using (var mem = new MemoryStream())
                using (var mf = new Metafile(mem, hdc)) {
                    bmpg.ReleaseHdc(hdc);
                    AbstractNode.PreventPaintingLineBreakAndBlockBreak = true;
                    using (var g = Graphics.FromImage(mf)) {
                        _rootEditor.Accept(
                            editor => {
                                if (editor.IsSelected) {
                                    editor.Figure.Paint(g);
                                }
                                return false;
                            }
                        );
                        DrawImageCopyRight(g, GetPrimaryLayersSelectedBounds());
                    }
                    AbstractNode.PreventPaintingLineBreakAndBlockBreak = false;

                    var hEmf = mf.GetHenhmetafile();
                    if (User32PI.OpenClipboard(Handle)) {
                        try {
                            if (User32PI.EmptyClipboard()) {
                                var hEmf2 = User32PI.CopyEnhMetaFile(hEmf, IntPtr.Zero);
                                var result = User32PI.SetClipboardData(ClipboardFormat.CF_ENHMETAFILE, hEmf2);
                                if (result == IntPtr.Zero) {
                                    var err = Marshal.GetLastWin32Error();
                                }

                                using (var b = CreateBitmapForSelected(BitmapMargin)) {
                                    var p = GetCompatibleBitmap(b);
                                    User32PI.SetClipboardData(ClipboardFormat.CF_BITMAP, p);
                                }

                            }
                        } finally {
                            User32PI.CloseClipboard();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Canvasの選択された要素をEMF形式で保存する．
        /// </summary>
        public void SaveSelectedAsEmf(string fileName) {
            using (var bmp = new Bitmap(1, 1))
            using (var bmpg = Graphics.FromImage(bmp)) {
                var hdc = bmpg.GetHdc();
                using (var mf = new Metafile(fileName, hdc)) {
                    bmpg.ReleaseHdc(hdc);
                    AbstractNode.PreventPaintingLineBreakAndBlockBreak = true;
                    using (var g = Graphics.FromImage(mf)) {
                        _rootEditor.Accept(
                            editor => {
                                if (editor.IsSelected) {
                                    editor.Figure.Paint(g);
                                }
                                return false;
                            }
                        );
                        DrawImageCopyRight(g, GetPrimaryLayersSelectedBounds());
                    }
                    AbstractNode.PreventPaintingLineBreakAndBlockBreak = false;
                }
            }
        }

        /// <summary>
        /// Canvasの選択された要素をPNG形式で保存する．
        /// </summary>
        public void SaveSelectedAsPng(string fileName) {
            using (var bmp = CreateBitmapForSelected(BitmapMargin)) {
                bmp.Save(fileName, ImageFormat.Png);
            }
        }

        public string GetFullText() {
            var buf = new StringBuilder();
            RootEditor.Accept(
                editor => {
                    var text = editor.Controller.GetText();
                    if (!string.IsNullOrEmpty(text)) {
                        buf.AppendLine(text);
                    }
                    return false;
                }
            );
            return buf.ToString();
        }

        // --- ime ---
        public bool IsImeOpen() {
            _immHandle = Imm32PI.ImmGetContext(Handle);
            try {
                return Imm32PI.ImmGetOpenStatus(_immHandle);
            } finally {
                Imm32PI.ImmReleaseContext(Handle, _immHandle);
            }
        }

        public string GetImeString() {
            _immHandle = Imm32PI.ImmGetContext(Handle);
            try {
                var len = Imm32PI.ImmGetCompositionString(
                    _immHandle, ImmGetCompositionStringInfoType.GCS_COMPSTR, null, 0
                );
                if (len == 0) {
                    return "";
                }

                var buf = new byte[len];
                Imm32PI.ImmGetCompositionString(_immHandle, ImmGetCompositionStringInfoType.GCS_COMPSTR, buf, len);
                return Encoding.Default.GetString(buf);
            } finally {
                Imm32PI.ImmReleaseContext(Handle, _immHandle);
            }
        }

        public Point GetImeWindowLocation() {
            _immHandle = Imm32PI.ImmGetContext(Handle);
            try {
                var cf = new COMPOSITIONFORM();
                var ret = Imm32PI.ImmGetCompositionWindow(_immHandle, ref cf);
                return TranslateToRootFigurePoint(new Point(cf.ptCurrentPos.x, cf.ptCurrentPos.y));

            } finally {
                Imm32PI.ImmReleaseContext(Handle, _immHandle);
            }
        }

        public void SetImePosition(Point pt) {
            _immHandle = Imm32PI.ImmGetContext(Handle);
            try {
                var point = new POINT() {
                    x = pt.X,
                    y = pt.Y
                };
                var cf = new COMPOSITIONFORM() {
                    dwStyle = CompositionFormStyle.CFS_POINT,
                    ptCurrentPos = point,
                };
                Imm32PI.ImmSetCompositionWindow(_immHandle, ref cf);
            } finally {
                Imm32PI.ImmReleaseContext(Handle, _immHandle);
            }
        }


        // --- scroll ---
        public void ScrollRecenter() {
            if (CanvasBackgroundImage != null) {
                Invalidate();
            }
            if (_caret != null && _caret.IsVisible) {
                var loc = _caret.Position;
                var y = loc.Y - (ClientSize.Height / 2);
                AutoScrollPosition = new Point(-AutoScrollPosition.X, y);

                /// スクロールするがCaretは移動しないままでOnScroll()も呼ばれないので
                SetImePosition(TranslateToControlPoint(_caret.Position));
            }
        }

        public void ScrollUp() {
            if (CanvasBackgroundImage != null) {
                Invalidate();
            }
            var delta = -((int) (ClientSize.Height * 0.9));
            var y = -AutoScrollPosition.Y + delta;
            AutoScrollPosition = new Point(-AutoScrollPosition.X, y);
            if (_caret.IsVisible) {
                var cpos = _caret.Position;
                _caret.Position = new Point(cpos.X, cpos.Y + delta);
            }
        }

        public void ScrollDown() {
            if (CanvasBackgroundImage != null) {
                Invalidate();
            }
            var delta = (int) (ClientSize.Height * 0.9);
            var y = -AutoScrollPosition.Y + delta;
            AutoScrollPosition = new Point(-AutoScrollPosition.X, y);
            if (_caret.IsVisible) {
                var cpos = _caret.Position;
                _caret.Position = new Point(cpos.X, cpos.Y + delta);
            }
        }

        // --- text format ---
        public bool CanModifyFontName() {
            return GetFontStyleUIState(FontModificationKinds.Name);
        }

        public bool CanModifyFontSize() {
            return GetFontStyleUIState(FontModificationKinds.Size);
        }

        public bool CanModifyFontStyle() {
            return GetFontStyleUIState(FontModificationKinds.Style);
        }

        public bool CanModifyHorizontalAlignment() {
            return GetTextAlignmentUIState(AlignmentModificationKinds.Horizontal);
        }

        public bool CanModifyVerticalAlignment() {
            return GetTextAlignmentUIState(AlignmentModificationKinds.Vertical);
        }

        public bool CanModifyListKind() {
            if (_focusManager.IsEditorFocused) {
                var editor = _focusManager.FocusedEditor;
                var sreq = new SetStyledTextListKindRequest();
                if (editor.CanUnderstand(sreq)) {
                    return true;
                }
            }

            var selecteds = _selectionManager.SelectedEditors;
            {
                if (selecteds == null || !selecteds.Any()) {
                    return false;
                }
                var sreq = new SetStyledTextListKindRequest();
                foreach (var editor in selecteds) {
                    if (editor.CanUnderstand(sreq)) {
                        return true;
                    }
                }

                return false;
            }
        }

        public bool CanModifyTextColor() {
            if (_focusManager.IsEditorFocused) {
                var editor = _focusManager.FocusedEditor;
                var sreq = new SetStyledTextColorRequest(Color.Black);
                if (editor.CanUnderstand(sreq)) {
                    return true;
                }
                
                //var preq = new SetPlainTextFontRequest() {
                //    Kinds = kinds,
                //};
                //if (editor.CanUnderstand(preq)) {
                //    return true;
                //}

                return false;
            }

            var selecteds = _selectionManager.SelectedEditors;
            if (selecteds == null || !selecteds.Any()) {
                return false;
            } else {
                var sreq = new SetStyledTextColorRequest(Color.Black);
                foreach (var editor in selecteds) {
                    if (editor.CanUnderstand(sreq)) {
                        return true;
                    }

                }

                return false;
            }
        }

        // --- undo redo ---
        public bool CanUndo() {
            if (!Enabled) {
                return false;
            }

            var isEditorFocused = Enabled && FocusManager.IsEditorFocused;
            var focus = FocusManager.Focus as StyledTextFocus;

            return
                (isEditorFocused && focus.CommandExecutor.CanUndo) ||
                (!isEditorFocused && CommandExecutor.CanUndo);
        }

        public bool CanRedo() {
            if (!Enabled) {
                return false;
            }

            var isEditorFocused = Enabled && FocusManager.IsEditorFocused;
            var focus = FocusManager.Focus as StyledTextFocus;

            return
                (isEditorFocused && focus.CommandExecutor.CanRedo) ||
                (!isEditorFocused && CommandExecutor.CanRedo);
        }

        // --- clipboard operation state ---
        public bool CanCut() {
            if (_focusManager.IsEditorFocused) {
                var focus = _focusManager.Focus as StyledTextFocus;
                if (focus == null) {
                    return false;
                }
                return !focus.Selection.IsEmpty;

            } else {
                var selecteds = _selectionManager.SelectedEditors;
                var list = new EditorBundle(selecteds);
                if (!list.CanUnderstandAll(new CopyRequest(selecteds))) {
                    return false;
                }
                return list.CanUnderstandAll(new RemoveRequest());
            }
        }

        public bool CanCopy() {
            if (_focusManager.IsEditorFocused) {
                var focus = _focusManager.Focus as StyledTextFocus;
                if (focus == null) {
                    return false;
                }
                return !focus.Selection.IsEmpty;

            } else {
                var selecteds = _selectionManager.SelectedEditors;
                var list = new EditorBundle(selecteds);
                return list.CanUnderstandAll(new CopyRequest(selecteds));
            }
        }

        public bool CanPaste() {
            if (_focusManager.IsEditorFocused) {
                var focus = _focusManager.Focus as StyledTextFocus;
                if (focus == null) {
                    return false;
                }
                //todo: StyledText対応
                // StyledTextが入っているときはかならずTextも入っているのでとりあえずこれでも問題ない
                return ClipboardUtil.ContainsText();

            } else {
                var selecteds = _selectionManager.SelectedEditors;
                var list = new EditorBundle(selecteds);
                return list.CanUnderstandAll(new PasteRequest());
            }
        }

        public bool CanDelete() {
            if (_focusManager.IsEditorFocused) {
                var focus = _focusManager.Focus as StyledTextFocus;
                if (focus == null) {
                    return false;
                }
                return !focus.Selection.IsEmpty;

            } else {
                var selecteds = _selectionManager.SelectedEditors;
                var list = new EditorBundle(selecteds);
                return list.CanUnderstandAll(new RemoveRequest());
            }
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected override void OnScroll(ScrollEventArgs se) {
            base.OnScroll(se);
            SetImePosition(TranslateToControlPoint(_caret.Position));
        }

        protected virtual void ConfigureEditorCanvas() {
        }

        protected virtual void OnToolChanged() {
            var handler = ToolChanged;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnImeStartComposition() {
            _isInImeComposition = true;

            if (_focusManager.IsEditorFocused) {
                using (var font = _focusManager.Focus.GetNextInputFont().CreateFont()) {
                    SetImeCompositionFont(font);
                }
            } else {
                SetImeCompositionFont(Font);
            }

            var handler = ImeStartComposition;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnImeEndComposition() {
            _isInImeComposition = false;

            var handler = ImeEndComposition;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnImeComposition() {
            var handler = ImeComposition;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }


        // ------------------------------
        // private
        // ------------------------------
        // --- for print ---
        private Rectangle GetAllFiguresBounds() {
            var rect = Rectangle.Empty;
            {
                var first = true;
                _primaryLayer.Accept(
                    fig => {
                        if (fig is ILayer) {
                            return false;
                        }
                        if (first) {
                            first = false;
                            rect = fig.Bounds;
                        } else {
                            rect = Rectangle.Union(rect, fig.Bounds);
                        }
                        return false;
                    }
                );
            }

            return rect;
        }

        private float CalcPrintRatio(Rectangle printBounds, int overlap, PrintFitKind fitKind) {
            switch (fitKind) {
                case PrintFitKind.Horizontal: {
                    var figBounds = GetAllFiguresBounds();
                    var w = (float) printBounds.Width / figBounds.Width;
                    return w < 1 ? w : 1;
                }
                case PrintFitKind.Vertical: {
                    var figBounds = GetAllFiguresBounds();
                    var h = (float) printBounds.Height / figBounds.Height;
                    return h < 1 ? h : 1;
                }
                case PrintFitKind.Both: {
                    var figBounds = GetAllFiguresBounds();
                    var w = (float) printBounds.Width / figBounds.Width;
                    var h = (float) printBounds.Height / figBounds.Height;
                    var r = Math.Min(w, h);
                    return r < 1 ? r : 1;
                }
                case PrintFitKind.None:
                default:
                    return 1;
            }
        }

        private Tuple<int, int> CalcPrintPageXAndY(Graphics g, Rectangle printBounds, int overlap, PrintFitKind fitKind) {
                switch (fitKind) {
                    case PrintFitKind.None: {
                        var figBounds = GetAllFiguresBounds();
                    
                        var xPages = (int) ((figBounds.Width - overlap) / (printBounds.Width - overlap)) + 1;
                        var yPages = (int) ((figBounds.Height - overlap) / (printBounds.Height - overlap)) + 1;
                        
                        return Tuple.Create(xPages, yPages);
                    }
    
                    case PrintFitKind.Horizontal: {
                        var figBounds = GetAllFiguresBounds();
    
                        var w = (float) printBounds.Width / figBounds.Width;
                        if (w < 1) {
                            var fitted = new Rectangle(
                                figBounds.Left,
                                figBounds.Top,
                                (int) Math.Ceiling(figBounds.Width * w),
                                (int) Math.Ceiling(figBounds.Height * w)
                            );
                            var yPages = (int) ((fitted.Height - overlap) / (printBounds.Height - overlap)) + 1;
                            return Tuple.Create(1, yPages);
    
                        } else {
                            var xPages = (int) ((figBounds.Width - overlap) / (printBounds.Width - overlap)) + 1;
                            var yPages = (int) ((figBounds.Height - overlap) / (printBounds.Height - overlap)) + 1;
    
                            return Tuple.Create(xPages, yPages);
                        }
                    }
    
                    case PrintFitKind.Vertical: {
                        var figBounds = GetAllFiguresBounds();
    
                        var h = (float) printBounds.Height / figBounds.Height;
                        if (h < 1) {
                            var fitted = new Rectangle(
                                figBounds.Left,
                                figBounds.Top,
                                (int) Math.Ceiling(figBounds.Width * h),
                                (int) Math.Ceiling(figBounds.Height * h)
                            );
                            var xPages = (int) ((fitted.Width - overlap) / (printBounds.Width - overlap)) + 1;
                            return Tuple.Create(xPages, 1);
    
                        } else {
                            var xPages = (int) ((figBounds.Width - overlap) / (printBounds.Width - overlap)) + 1;
                            var yPages = (int) ((figBounds.Height - overlap) / (printBounds.Height - overlap)) + 1;
    
                            return Tuple.Create(xPages, yPages);
                        }
                    }
    
                    case PrintFitKind.Both:
                    default:
                        return Tuple.Create(1, 1);
                }
        
        }


        
        /// <summary>
        /// 現在のフォーカス状態や選択状態から，
        /// Font名，フォントスタイル，Horizontal/VerticalAlignmentの設定可否を返す．
        /// </summary>
        private bool GetFontStyleUIState(FontModificationKinds kinds) {
            if (_focusManager.IsEditorFocused) {
                var editor = _focusManager.FocusedEditor;
                var sreq = new SetStyledTextFontRequest() {
                    Kinds = kinds,
                };
                if (editor.CanUnderstand(sreq)) {
                    return true;
                }
                
                //var preq = new SetPlainTextFontRequest() {
                //    Kinds = kinds,
                //};
                //if (editor.CanUnderstand(preq)) {
                //    return true;
                //}

                return false;
            }

            var selecteds = _selectionManager.SelectedEditors;
            {
                if (selecteds == null || !selecteds.Any()) {
                    return false;
                }

                var sreq = new SetStyledTextFontRequest() {
                    Kinds = kinds,
                };
                var preq = new SetPlainTextFontRequest() {
                    Kinds = kinds,
                };
                foreach (var editor in selecteds) {
                    if (editor.CanUnderstand(sreq) || editor.CanUnderstand(preq)) {
                        return true;
                    }

                }

                return false;
            }
        }

        private bool GetTextAlignmentUIState(AlignmentModificationKinds kinds) {
            if (_focusManager.IsEditorFocused) {
                var editor = _focusManager.FocusedEditor;
                var sreq = new SetStyledTextAlignmentRequest() {
                    Kinds = kinds,
                };
                if (editor.CanUnderstand(sreq)) {
                    return true;
                }
                
            //    //var preq = new SetPlainTextFontRequest() {
            //    //    Kinds = kinds,
            //    //};
            //    //if (editor.CanUnderstand(preq)) {
            //    //    return true;
            //    //}

            //    return false;
            }

            var selecteds = _selectionManager.SelectedEditors;
            {
                if (selecteds == null || !selecteds.Any()) {
                    return false;
                }
                var sreq = new SetStyledTextAlignmentRequest() {
                    Kinds = kinds,
                };
                //var preq = new SetPlainTextAlignmentRequest() {
                //    Kinds = kinds,
                //};
                foreach (var editor in selecteds) {
                    //if (!(editor.CanUnderstand(sreq) || editor.CanUnderstand(preq))) {
                    if (editor.CanUnderstand(sreq)) {
                        return true;
                    }

                }

                return false;
            }
        }

        // --- ime ---
        private void SetImeArea(Point pt, Rectangle area) {
            _immHandle = Imm32PI.ImmGetContext(Handle);
            try {
                var p = new POINT() {
                    x = pt.X,
                    y = pt.Y,
                };
                var r = new RECT() {
                    left = area.Left,
                    top = area.Top,
                    right = area.Right,
                    bottom = area.Bottom,
                };
                //var r= new RECT() {
                //    //left = pt.X,
                //    //top = pt.Y,
                //    left = 0,
                //    top = 0,
                //    right = pt.X + 100,
                //    bottom = Screen.PrimaryScreen.Bounds.Height,
                //};
                var cf = new COMPOSITIONFORM() {
                    dwStyle = CompositionFormStyle.CFS_RECT,
                    ptCurrentPos = p,
                    rcArea = r,
                };
                Imm32PI.ImmSetCompositionWindow(_immHandle, ref cf);
            } finally {
                Imm32PI.ImmReleaseContext(Handle, _immHandle);
            }
        }

        private void SetImeCompositionFont(Font font) {
            _immHandle = Imm32PI.ImmGetContext(Handle);
            try {
                /// LOGFONT型で宣言するとToLogFont()に渡すときにboxingされるので値が返ってこない
                object lf = new LOGFONT(); /// ここでboxing
                font.ToLogFont(lf); /// lfがそのまま渡るのでちゃんとlfに値が入る
                var logfont = (LOGFONT) lf; /// ここでunboxing

                Imm32PI.ImmSetCompositionFont(_immHandle, ref logfont);

            } finally {
                Imm32PI.ImmReleaseContext(Handle, _immHandle);
            }
        }

        private Bitmap CreateBitmapForSelected(Size margin) {
            var first = true;
            var rect = Rectangle.Empty;
            _rootEditor.Accept(
                editor => {
                    if (editor.IsSelected) {
                        if (first) {
                            first = false;
                            rect = editor.Figure.Bounds;
                        } else {
                            rect = Rectangle.Union(rect, editor.Figure.Bounds);
                        }
                    }
                    return false;
                }
            );

            rect.Inflate(margin);
            var ret = new Bitmap(rect.Width, rect.Height);
            using (var g = Graphics.FromImage(ret)) {
                GraphicsUtil.SetupGraphics(g, GraphicQuality.MaxQuality);

                g.TranslateTransform(-rect.Left, -rect.Top);

                AbstractNode.PreventPaintingLineBreakAndBlockBreak = true;
                UseGdiPlus = true;
                try {
                    g.Clear(Color.White);
                    _rootEditor.Accept(
                        editor => {
                            if (editor.IsSelected) {
                                editor.Figure.Paint(g);
                            }
                            return false;
                        }
                    );

                    DrawImageCopyRight(g, rect);

                    return ret;
                } finally {
                    AbstractNode.PreventPaintingLineBreakAndBlockBreak = false;
                    UseGdiPlus = false;
                }
            }
        }

        /// <summary>
        /// GDI+のビットマップの形式からクリップボードのビットマップの形式に変換してそのハンドルを返す．
        /// </summary>
        private IntPtr GetCompatibleBitmap(Bitmap bmp) { 
            var hBitmap = bmp.GetHbitmap();
 
            // Get the screen DC. 
            //
            var hDC = User32PI.GetDC(IntPtr.Zero); 

            // Create a compatible DC to render the source bitmap.
            //
            var dcSrc = Gdi32PI.CreateCompatibleDC(hDC);
            var srcOld = Gdi32PI.SelectObject(dcSrc, hBitmap);
 
            // Create a compatible DC and a new compatible bitmap. 
            //
            var dcDest = Gdi32PI.CreateCompatibleDC(hDC);
            var hBitmapNew = Gdi32PI.CreateCompatibleBitmap(hDC, bmp.Size.Width, bmp.Size.Height);

            // Select the new bitmap into a compatible DC and render the blt the original bitmap.
            // 
            var destOld = Gdi32PI.SelectObject(dcDest, hBitmapNew);
            Gdi32PI.BitBlt(dcDest, 0, 0, bmp.Size.Width, bmp.Size.Height, dcSrc, 0, 0, TernaryRasterOperations.SRCCOPY); 
 
            // Clear the source and destination compatible DCs.
            // 
            Gdi32PI.SelectObject(dcSrc, srcOld);
            Gdi32PI.SelectObject(dcDest, destOld);

            Gdi32PI.DeleteDC(dcSrc); 
            Gdi32PI.DeleteDC(dcDest);
            User32PI.ReleaseDC(IntPtr.Zero, hDC);

            Gdi32PI.DeleteObject(hBitmap);
 
            return hBitmapNew;
        }

        // --- caret ---
        private void EnsurePointShown(Point pt) {
            var scope = Viewport;
            if (!scope.Contains(pt)) {
                var pos = new Point(-AutoScrollPosition.X, -AutoScrollPosition.Y);

                var dir = RectUtil.GetOuterDirection(scope, pt);
                if (EnumUtil.HasAllFlags((int) dir, (int) Directions.Left)) {
                    pos.X = pt.X - 8;
                } else if (EnumUtil.HasAllFlags((int) dir, (int) Directions.Right)) {
                    pos.X = pt.X - ClientSize.Width + 8;
                }

                if (EnumUtil.HasAllFlags((int) dir, (int) Directions.Up)) {
                    pos.Y = pt.Y - 8;
                } else if (EnumUtil.HasAllFlags((int) dir, (int) Directions.Down)) {
                    pos.Y = pt.Y - ClientSize.Height + 8;
                }

                if (CanvasBackgroundImage != null) {
                    Invalidate();
                }
                AutoScrollPosition = pos;
            }
        }

        private void HandleCaretMoved(object sender, EventArgs e) {
            AdjustRootFigureBounds();

            EnsurePointShown(_caret.Position);
            EnsurePointShown(_caret.Position + _caret.Size);

            SetImePosition(TranslateToControlPoint(_caret.Position));
        }

        private void HandleGotFocus(object sender, EventArgs e) {
            /// GotFocus時にImeウィンドウの位置を設定しておかないと
            /// なぜかEditorCanvas外にImeウィンドウが表示されてしまう
            SetImePosition(TranslateToControlPoint(_caret.Position));
        }

        private Rectangle GetPrimaryLayersBounds() {
            var first = true;
            var rect = Rectangle.Empty;
            _primaryLayer.Accept(
                fig => {
                    if (fig is ILayer) {
                        return false;
                    }
                    if (first) {
                        first = false;
                        rect = fig.Bounds;
                    } else {
                        rect = Rectangle.Union(rect, fig.Bounds);
                    }
                    return false;
                }
            );
            return rect;
        }

        private Rectangle GetPrimaryLayersSelectedBounds() {
            var first = true;
            var rect = Rectangle.Empty;
            _rootEditor.Accept(
                editor => {
                    if (editor.IsSelected) {
                        if (first) {
                            first = false;
                            rect = editor.Figure.Bounds;
                        } else {
                            rect = Rectangle.Union(rect, editor.Figure.Bounds);
                        }
                    }
                    return false;
                }
            );
            return rect;
        }

        private void DrawImageCopyRight(Graphics g, Rectangle rect) {
            if (!StringUtil.IsNullOrWhitespace(_imageCopyRightString)) {
                var fmt = StringFormat.GenericTypographic;
                var size = Size.Ceiling(g.MeasureString(_imageCopyRightString, Font));
                var loc = new Point(
                    rect.Left + rect.Width - size.Width - 2,
                    rect.Top + rect.Height - size.Height - 2
                );
                g.DrawString(_imageCopyRightString, Font, Brushes.Gray, loc);
            }
        }

        // ========================================
        // class
        // ========================================
        //private class MiniToolBarRenderer: ToolStripProfessionalRenderer {
        //    private EditorCanvas _owner;
        //    public MiniToolBarRenderer(EditorCanvas owner) {
        //        _owner = owner;
        //    }

        //    protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) {
        //        //base.OnRenderToolStripBorder(e);
                
        //        using (var pen = new Pen(_owner.MiniToolBarBorderColor)) {
        //            var bounds = e.AffectedBounds;
        //            var r = new Rectangle(bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1);
        //            e.Graphics.DrawRectangle(pen, r);
        //        }
        //    }
        //}

    }
}
