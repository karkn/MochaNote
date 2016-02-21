/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Internal.Core;
using System.Windows.Forms;
using Mkamo.Common.Event;
using System.Drawing;
using Mkamo.Common.Forms.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using Mkamo.Common.Forms.Mouse;
using Mkamo.Figure.Figures;
using System.ComponentModel;
using Mkamo.Figure.Layouts;
using System.Collections.ObjectModel;
using Mkamo.Common.Win32;
using System.IO;
using System.Reflection;
using Mkamo.Common.Win32.Core;

namespace Mkamo.Figure.Core {
    /// event dispatch
    /// invalidation management
    /// figure management
    public class Canvas: ScrollableControl {
        ///UserControlを継承すると，以下の状態のときにIMEを有効にできなくなる．
        ///  - 「詳細なテキスト サービスをオフにする」がチェックオフ
        ///  - 「詳細なテキスト サービスのサポートをプログラムのすべてに拡張する」がチェックオン

        private static readonly Size RootFigureBoundsMargin = new Size(32, 32);

        // ========================================
        // field
        // ========================================
        private RootFigure _rootFigure;
        private IFigure _content;
        private IDirtManager _dirtManager;
        private EventDispatcher _dispatcher;
        private MouseDnDEventRaiser _dndEventProducer;

        private HighlightRegistry _highlightRegistry;

        //private Insets _canvasPadding;
        private bool _enableAutoAdjustRootFigureSize;
        private Func<IFigure, AutoAdjustExceptionKind> _autoAdjustExcepter;

        private bool _enableAutoScroller;
        private AutoScroller _autoScroller;
        private MouseEventArgs _lastDragMoveEventArgs;

        private bool _useGdiPlusText;

        private Image _canvasBackgroundImage;

        private Graphics _currentGraphics = null;

        private Size _reservedMinSize;

        private bool _useClearType = false;

        // ========================================
        // constructor
        // ========================================
        public Canvas(): base() {
            //BackgroundImage = Image.FromFile(@"D:\share\img\WarrenLouw\43a19614516e6315cd8804e3604d84f8.jpg");
            DoubleBuffered = true;
            AutoScroll = true;
            AllowDrop = true;
            Size = new Size(10, 10);

            BackColor = Color.White;
            ForeColor = Color.Black;

            _useGdiPlusText = false;

            //_canvasPadding = new Insets(2);
            _enableAutoAdjustRootFigureSize = true;

            _enableAutoScroller = true;
            _autoScroller = new AutoScroller(this);
            _autoScroller.AutoScrolling += HandleAutoScrollerAutoScrolling;
            _autoScroller.AutoScrolled += HandleAutoScrollerAutoScrolled;

            _dndEventProducer = new MouseDnDEventRaiser(this);

            _highlightRegistry = new HighlightRegistry();

            _rootFigure = new RootFigure();
            _rootFigure.Canvas = this;
            //_rootFigure.Size = ClientSize - _canvasPadding.Size;
            _rootFigure.Size = ClientSize;
            //_rootFigure.DescendantChanged += HandleDescendantChanged;
            _rootFigure.BoundsChanged += HandleRootFigureBoundsChanged;
            _rootFigure.Layout = new StackLayout();

            /// 初期設定．サブクラスでoverrideされることを想定．
            Configure();

            /// DirtManagerやEventDispatcherがConfigureされていなければデフォルト実装を使う．
            if (_dirtManager == null) {
                _DirtManager = new DirtManager(this);
            }
            if (_dispatcher == null) {
                _EventDispatcher = new EventDispatcher(this);
            }

            InitDndEventProducer();
        }

        // ========================================
        // destructor
        // ========================================
        // === IDisposable ==========
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (_canvasBackgroundImage != null) {
                    _canvasBackgroundImage.Dispose();
                }
                RootFigure.Dispose();
                if (_dispatcher != null) {
                    _dispatcher.Dispose();
                }
            }

            base.Dispose(disposing);
            GC.SuppressFinalize(this);
        }

        // ========================================
        // event
        // ========================================
        public event EventHandler AutoScrollMinSizeChanged;

        // ========================================
        // property
        // ========================================
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EnableAutoAdjustRootFigureSize {
            get { return _enableAutoAdjustRootFigureSize; }
            set { _enableAutoAdjustRootFigureSize = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<IFigure, AutoAdjustExceptionKind> AutoAdjustExcepter {
            get { return _autoAdjustExcepter; }
            set { _autoAdjustExcepter = value; }
        }

        //[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //public Insets CanvasPadding {
        //    get { return _canvasPadding; }
        //    set {
        //        if (value == _canvasPadding) {
        //            return;
        //        }
        //        _canvasPadding = value;
        //        AutoScrollMinSize = _rootFigure.Size + _canvasPadding.Size;
        //        Invalidate();
        //    }
        //}

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EnableAutoScroller {
            get { return _enableAutoScroller; }
            set { _enableAutoScroller = value; }
        }

        /// <summary>
        /// RootFigureの直下のfigure．
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IFigure FigureContent {
            get { return _content; }
            set {
                if (value == _content) {
                    return;
                }
                if (_content != null) {
                    _rootFigure.Children.Remove(_content);
                }
                _content = value;
                if (_content != null) {
                    _rootFigure.Children.Add(_content);
                }
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RootFigure RootFigure {
            get { return _rootFigure; }
        }

        /// <summary>
        /// 現在表示しているRootFigure上の矩形．
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Rectangle Viewport {
            get {
                return new Rectangle(
                    TranslateToRootFigurePoint(
                        //new Point(CanvasPadding.Left, CanvasPadding.Top)
                        Point.Empty
                    ),
                    new Size(
                        //ClientRectangle.Width - CanvasPadding.Width,
                        //ClientRectangle.Height - CanvasPadding.Height
                        ClientRectangle.Width,
                        ClientRectangle.Height
                    )
                );
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool UseGdiPlus {
            get { return _useGdiPlusText; }
            set { _useGdiPlusText = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image CanvasBackgroundImage {
            get { return _canvasBackgroundImage; }
            set {
                if (value == _canvasBackgroundImage) {
                    return;
                }
                _canvasBackgroundImage = value;
                Invalidate();
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public HighlightRegistry HighlightRegistry {
            get { return _highlightRegistry; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new virtual Size AutoScrollMinSize {
            get { return base.AutoScrollMinSize; }
            set {
                if (value == base.AutoScrollMinSize) {
                    return;
                }
                base.AutoScrollMinSize = value;
                OnAutoScrollMinSizeChanged();
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool UseClearType {
            get { return _useClearType; }
            set {
                _useClearType = value;
                Invalidate();
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Size ReservedMinSize {
            get { return _reservedMinSize; }
            set {
                _reservedMinSize = value;
                AdjustRootFigureBounds();
            }
        }

        [Browsable(false)]
        public IDirtManager DirtManager {
            get { return _DirtManager; }
        }

        // ------------------------------
        // protected internal
        // ------------------------------
        protected internal Graphics _CurrentGraphics {
            get { return _currentGraphics; }
            set { _currentGraphics = value; }
        }

        // ------------------------------
        // protected
        // ------------------------------
        [Browsable(false)]
        protected IDirtManager _DirtManager {
            get { return _dirtManager; }
            set {
                _dirtManager = value;
                _rootFigure._DirtManager = _dirtManager;
            }
        }

        [Browsable(false)]
        protected EventDispatcher _EventDispatcher {
            get { return _dispatcher; }
            set {
                _dispatcher = value;
                _rootFigure._EventDispatcher = _dispatcher;
            }
        }

        // ========================================
        // method
        // ========================================
        // === Canvas ==========
        /// Controlの左上を原点とするPointからRootFigureの(0, 0)を原点とするPointへの変換
        public Point TranslateToRootFigurePoint(Point controlPoint) {
            return new Point(
                //controlPoint.X - AutoScrollPosition.X + _rootFigure.Left - _canvasPadding.Left,
                //controlPoint.Y - AutoScrollPosition.Y + _rootFigure.Top - _canvasPadding.Top
                controlPoint.X - AutoScrollPosition.X + _rootFigure.Left,
                controlPoint.Y - AutoScrollPosition.Y + _rootFigure.Top
            );
        }

        /// RootFigureの(0, 0)を原点とするPointからControlの左上を原点とするPointへの変換
        public Point TranslateToControlPoint(Point rootFigurePoint) {
            return new Point(
                //rootFigurePoint.X + AutoScrollPosition.X - _rootFigure.Left + _canvasPadding.Left,
                //rootFigurePoint.Y + AutoScrollPosition.Y - _rootFigure.Top + _canvasPadding.Top
                rootFigurePoint.X + AutoScrollPosition.X - _rootFigure.Left,
                rootFigurePoint.Y + AutoScrollPosition.Y - _rootFigure.Top
            );
        }

        public void DirtyAllVisualLines() {
            RootFigure.Accept(
                fig => {
                    var node = fig as INode;
                    if (node != null) {
                        node.DirtyAllVisLines();
                        node.InvalidatePaint();
                    }
                    return false;
                }
            );
        }

        public void MakeTransparent(bool enable) {
            SetStyle(ControlStyles.SupportsTransparentBackColor, enable);
            //SetStyle(ControlStyles.UserPaint, true);
            if (enable) {
                BackColor = Color.Transparent;
            } else {
                BackColor = Color.White;
            }
        }

        // ------------------------------
        // protected internal
        // ------------------------------
        protected internal GraphicsUsingContext GetGraphicsUsingContext() {
            return new GraphicsUsingContext(this);
        }

        // ------------------------------
        // protected
        // ------------------------------
        // === Control ==========
        protected override void OnMouseWheel(MouseEventArgs e) {
            if (_canvasBackgroundImage != null) {
                Invalidate();
            }
            base.OnMouseWheel(e);
        }

        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                case (int) WindowMessage.HSCROLL:
                case (int) WindowMessage.VSCROLL:
                    if (_canvasBackgroundImage != null) {
                        Invalidate();
                    }
                    break;
            }
            base.WndProc(ref m);
        }

        // for background loading and animation
        //private Timer _loadingTimer;
        //private bool _inLoading = false;
        //public bool InLoading {
        //    get { return _inLoading; }
        //    set {
        //        if (value == _inLoading) {
        //            return;
        //        }

        //        _inLoading = value;
        //        if (_inLoading) {
        //            _loadingTimer = new Timer();
        //            _loadingTimer.Interval = 100;
        //            _loadingTimer.Tick += (s, e) => Invalidate();
        //            _loadingTimer.Start();
        //        } else {
        //            _loadingTimer.Stop();
        //            _loadingTimer.Dispose();
        //            _loadingTimer = null;
        //        }
        //    }
        //}

        //private int animcount = 0;
        //private void PaintLoading(PaintEventArgs e) {
        //    // ８個の「点」を円周上に描画する
        //    ++animcount;
        //    if (animcount == 8) animcount = 0;

        //    Graphics g = e.Graphics;
        //    SetupGraphics(g);
        //    //g.SmoothingMode = SmoothingMode.AntiAlias;

        //    //SolidBrush bgbrush = new SolidBrush(Color.FromArgb(128, Color.White));
        //    //g.FillRectangle(bgbrush, ClientRectangle);
        //    //bgbrush.Dispose();

        //    SolidBrush brush = new SolidBrush(Color.Black);

        //    float x = (float)(Width - 32) / 2.0f;
        //    float y = (float)(Height - 32) / 2.0f;
        //    for (int i = 0; i < 8; i++) {
        //        // だんだん濃く、大きく
        //        float r = 3.0f * ((float)i * (8.0f / 70.0f) + 0.2f);

        //        Matrix m = new Matrix();
        //        m.Translate(32 / 3, 0, MatrixOrder.Append);
        //        m.Rotate(45 * (i + animcount), MatrixOrder.Append);
        //        m.Translate(32 / 2 + x, 32 / 2 + y, MatrixOrder.Append);

        //        g.Transform = m;
        //        g.FillEllipse(brush, -r, -r, r * 2, r * 2);
        //    }

        //    g.ResetTransform();
        //    brush.Dispose();
        //}

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);

            //if (InLoading) {
            //    PaintLoading(e);
            //    return;
            //}

            var g = e.Graphics;
            _currentGraphics = g;

            //g.ScaleTransform(0.8f, 0.8f);
            SetupGraphics(g);
            _rootFigure.Paint(g);

            _currentGraphics = null;
        }

        // このやり方ではスクロール時に少しだけ画像が移動してしまうことがある
        // AutoScrollをやめて自前でスクロール処理する必要があるかも
        protected override void OnPaintBackground(PaintEventArgs e) {
            base.OnPaintBackground(e);

            if (_canvasBackgroundImage != null) {
                var loc = new Point(Width - _canvasBackgroundImage.Width, Height - _canvasBackgroundImage.Height);
                e.Graphics.DrawImage(_canvasBackgroundImage, new Rectangle(loc, _canvasBackgroundImage.Size));
            }
            //var img = Image.FromFile(@"D:\share\img\WarrenLouw\43a19614516e6315cd8804e3604d84f8.jpg");
            //e.Graphics.DrawImage(img, new Rectangle(Point.Empty, img.Size));
        }

        //protected override void OnScroll(ScrollEventArgs se) {
        //    base.OnScroll(se);
        //    Invalidate();
        //}

        protected override void OnResize(EventArgs e) {
            if (_rootFigure != null) {
                //if (_rootFigure.Width < ClientSize.Width - _canvasPadding.Width) {
                //    _rootFigure.Width = ClientSize.Width - _canvasPadding.Width;
                //}
                //if (_rootFigure.Height < ClientSize.Height - _canvasPadding.Height) {
                //    _rootFigure.Height = ClientSize.Height - _canvasPadding.Height;
                //}

                /// AdjustRootFigureBounds()の後にAutoScroll = trueしないと
                /// ScrollBarの表示・非表示が反映されないことがある．
                /// SuspendLayout()しないとAutoScroll = falseにしたタイミングで
                /// AutoScrollPositionが原点に戻されてしまう．
                /// base.OnResize(e)もはさんでおかないとサイズが小さくなったときに
                /// 一瞬スクロールバーが表示されてしまう
                SuspendLayout();
                AutoScroll = false;

                base.OnResize(e);
                AdjustRootFigureBounds();

                AutoScroll = true;
                ResumeLayout();

            } else {
                base.OnResize(e);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            _dndEventProducer.HandleMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);
            _dndEventProducer.HandleMouseUp(e);
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);
            _dndEventProducer.HandleMouseMove(e);
        }

        protected override void OnMouseClick(MouseEventArgs e) {
            base.OnMouseClick(e);
            _dndEventProducer.HandleMouseClick(e);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e) {
            base.OnMouseDoubleClick(e);
            _dndEventProducer.HandleMouseDoubleClick(e);
        }

        protected override void OnMouseEnter(EventArgs e) {
            base.OnMouseEnter(e);
            _dispatcher.HandleMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);
            if (_enableAutoScroller) {
                _autoScroller.StopAutoScroll();
            }
            _dispatcher.HandleMouseLeave(e);
        }

        protected override void OnMouseHover(EventArgs e) {
            base.OnMouseHover(e);
            _dispatcher.HandleMouseHover(e);
        }

        protected override void OnDragOver(DragEventArgs e) {
            base.OnDragOver(e);
            var translated = TranslateDragEventArgs(e);
            _dispatcher.HandleDragOver(translated);
            e.Effect = translated.Effect;
        }

        protected override void OnDragDrop(DragEventArgs e) {
            base.OnDragDrop(e);
            var translated = TranslateDragEventArgs(e);
            _dispatcher.HandleDragDrop(translated);
            e.Effect = translated.Effect;
        }

        protected override void OnDragEnter(DragEventArgs e) {
            base.OnDragEnter(e);
            var translated = TranslateDragEventArgs(e);
            _dispatcher.HandleDragEnter(translated);
            e.Effect = translated.Effect;
        }

        protected override void OnDragLeave(EventArgs e) {
            base.OnDragLeave(e);
            _dispatcher.HandleDragLeave(e);
        }

        protected override void OnQueryContinueDrag(QueryContinueDragEventArgs e) {
            base.OnQueryContinueDrag(e);
            _dispatcher.HandleQueryContinueDrag(e);
        }

        protected override void OnLostFocus(EventArgs e) {
            base.OnLostFocus(e);
            _dndEventProducer.CancelDrag();
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            _dndEventProducer.HandleKeyDown(e);
            base.OnKeyDown(e);
        }
    
        // === Canvas ==========
        /// <summary>
        /// 初期設定を行う．
        /// サブクラスでoverrideされることを想定．
        /// </summary>
        protected virtual void Configure() {
        }

        protected void SetupGraphics(Graphics g) {
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighSpeed;
            g.TextRenderingHint = _useClearType ? TextRenderingHint.ClearTypeGridFit : TextRenderingHint.SystemDefault;

            //g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit; /// SystemDefaultの方がいい
            //g.CompositingQuality = CompositingQuality.HighQuality; /// 遅い

            /// スクロール位置やpaddingなどの分，座標移動
            var controlPoint = TranslateToControlPoint(Point.Empty);
            g.TranslateTransform(controlPoint.X, controlPoint.Y);
        }

        protected virtual Rectangle CalcPreferredContentsBounds() {
            var first = true;
            var rect = Rectangle.Empty;

            _rootFigure.Accept(
                fig => {
                    var exception = _autoAdjustExcepter == null? AutoAdjustExceptionKind.None: _autoAdjustExcepter(fig);

                    if (fig == _rootFigure || !fig.IsVisible || exception == AutoAdjustExceptionKind.ExceptThis) {
                        /// RootFigureそのものやIsVisibleでないものや除外対象であれば計算に入れない
                        return false;
                    }

                    if (exception == AutoAdjustExceptionKind.ExceptThisAndChildren) {
                        /// 子も含めて計算に入れない
                        return true;
                    }

                    if (first) {
                        rect = fig.Bounds;
                        first = false;
                    } else {
                        rect = Rectangle.Union(rect, fig.Bounds);
                    }
                    return false;
                }
            );

            return rect;
        }

        /// <summary>
        /// 子FigureからRootFigureの適切な矩形を返す．
        /// </summary>
        protected virtual Rectangle CalcPreferredRootFigureBounds() {
            var rect = CalcPreferredContentsBounds();
            rect = new Rectangle(rect.Location, rect.Size + RootFigureBoundsMargin);

            /// RootFigureを縮められるようにControlのSizeとスクロールバー幅とPoint.Emptyを使う
            /// ClientSizeはScrollbarの表示，非表示でサイズが変わってしまうので使えない

            var contSize = Size;
            contSize = new Size(
                Math.Max(contSize.Width, _reservedMinSize.Width),
                Math.Max(contSize.Height, _reservedMinSize.Height)
            );
            var contRect = new Rectangle(Point.Empty, contSize);

            if (contRect.Contains(rect)) {
                return contRect;

            } else {
                var scrollBarWidth = SystemInformation.VerticalScrollBarWidth;
                var cliSize = Size - new Size(scrollBarWidth, scrollBarWidth);
                var cliRect = new Rectangle(Point.Empty, cliSize);

                if (rect.X < 0 || rect.Right > contRect.Right) {
                    /// 横にはみ出ているので横スクロールバー確定

                    /// スクロールバー幅分大きさを引いたcliRectと比較
                    if (rect.Y < 0 || rect.Bottom > cliRect.Bottom) {
                        /// 縦にはみ出ているので縦スクロールバー確定
                        /// 縦横スクロールバー表示
                        var r = new Rectangle(Point.Empty, cliSize);
                        return Rectangle.Union(rect, r);

                    } else {
                        /// 横スクロールバーのみ表示
                        var r = new Rectangle(Point.Empty, new Size(contSize.Width, cliSize.Height));
                        return Rectangle.Union(rect, r);
                    }

                } else {
                    /// 縦にはみ出ているので縦スクロールバー確定

                    /// スクロールバー幅分大きさを引いたcliRectと比較
                    if (rect.X < 0 || rect.Right > cliRect.Right) {
                        /// 横にはみ出ているので縦スクロールバー確定
                        /// 縦横スクロールバー表示
                        var r = new Rectangle(Point.Empty, cliSize);
                        return Rectangle.Union(rect, r);

                    } else {
                        /// 縦スクロールバーのみ表示
                        var r = new Rectangle(Point.Empty, new Size(cliSize.Width, contSize.Height));
                        return Rectangle.Union(rect, r);
                    }
                }
                }
        }

        // ------------------------------
        // protected internal
        // ------------------------------
        /// <summary>
        /// 子FigureからRootFigureを適切な矩形に調節する．
        /// </summary>
        protected internal virtual void AdjustRootFigureBounds() {
            _rootFigure.Bounds = CalcPreferredRootFigureBounds();
        }

        // ------------------------------
        // private
        // ------------------------------
        // --- event handler ---
        private void InitDndEventProducer() {
            _dndEventProducer.MouseClick += (sender, e) => _dispatcher.HandleMouseClick(TranslateMouseEventArgs(e));
            _dndEventProducer.MouseDoubleClick += (sender, e) => _dispatcher.HandleMouseDoubleClick(TranslateMouseEventArgs(e));

            _dndEventProducer.MouseDown += (sender, e) => _dispatcher.HandleMouseDown(TranslateMouseEventArgs(e));
            _dndEventProducer.MouseUp += (sender, e) => _dispatcher.HandleMouseUp(TranslateMouseEventArgs(e));
            _dndEventProducer.MouseMove += (sender, e) => _dispatcher.HandleMouseMove(TranslateMouseEventArgs(e));

            _dndEventProducer.DragStart += (sender, e) => {
                var translated = TranslateMouseEventArgs(e);
                _dispatcher.HandleDragStart(translated);
                ((CancelMouseEventArgs) e).IsCanceled = translated.IsCanceled;

            };
            _dndEventProducer.DragMove+= (sender, e) => {
                if (_enableAutoScroller) {
                    _autoScroller.StartAutoScroll(_autoScroller.GetPreferredScrollDirection(e.Location));
                }
                _dispatcher.HandleDragMove(TranslateMouseEventArgs(e));
                _lastDragMoveEventArgs = e;
            };
            _dndEventProducer.DragFinish += (sender, e) => {
                if (_enableAutoScroller) {
                    _autoScroller.StopAutoScroll();
                }
                _dispatcher.HandleDragFinish(TranslateMouseEventArgs(e));
            };
            _dndEventProducer.DragCancel += (sender, e) => {
                if (_enableAutoScroller) {
                    _autoScroller.StopAutoScroll();
                }
                _dispatcher.HandleDragCancel(e);
            };
        }

        private CancelMouseEventArgs TranslateMouseEventArgs(MouseEventArgs e) {
            Point rootFigPoint = TranslateToRootFigurePoint(e.Location);
            return new CancelMouseEventArgs(
                e.Button,
                e.Clicks,
                rootFigPoint.X, //rootFigPoint.X - (rootFigPoint.X % _gridSize.Width),
                rootFigPoint.Y, //rootFigPoint.Y - (rootFigPoint.Y % _gridSize.Height),
                e.Delta
            );
        }

        private DragEventArgs TranslateDragEventArgs(DragEventArgs e) {
            Point rootFigPoint = TranslateToRootFigurePoint(PointToClient(new Point(e.X, e.Y)));
            return new DragEventArgs(
                e.Data,
                e.KeyState,
                rootFigPoint.X, // rootViewPoint.X - (rootViewPoint.X % _gridSize.Width),
                rootFigPoint.Y, // rootViewPoint.Y - (rootViewPoint.Y % _gridSize.Height),
                e.AllowedEffect,
                e.Effect
            );
        }

        /// <summary>
        /// figureをCanvasに登録する．
        /// 登録されたfigureのBounds変更時にRootFigureのサイズを調整するようにする．
        /// </summary>
        //private void RegisterFigure(IFigure figure) {
        //    if (figure == null) {
        //        return;
        //    }
        //    figure.Accept(
        //        fig => {
        //            fig.BoundsChanged += HandleRegisteredFigureBoundsChanged;
        //            return false;
        //        }
        //    );

        //    if (_enableAutoAdjustRootFigureSize) {
        //        var first = true;
        //        var rect = Rectangle.Empty;
        //        figure.Accept(
        //            fig => {
        //                var exception = _autoAdjustExcepter == null? AutoAdjustExceptionKind.None : _autoAdjustExcepter(fig);
        //                if (!fig.IsVisible || exception == AutoAdjustExceptionKind.ExceptThis) {
        //                    /// RootFigureそのものやIsVisibleでないものや除外対象であれば計算に入れない
        //                    return false;
        //                }

        //                if (exception == AutoAdjustExceptionKind.ExceptThisAndChildren) {
        //                    /// 子も含めて計算に入れない
        //                    return true;
        //                }

        //                if (first) {
        //                    rect = fig.Bounds;
        //                    first = false;
        //                } else {
        //                    rect = Rectangle.Union(rect, fig.Bounds);
        //                }
        //                return false;
        //            }
        //        );
        //        if (!first && !_rootFigure.Bounds.Contains(rect)) {
        //            AdjustRootFigureBounds();
        //        }
        //    }
        //}

        //private void UnregisterFigure(IFigure figure) {
        //    figure.Accept(
        //        fig => {
        //            fig.BoundsChanged -= HandleRegisteredFigureBoundsChanged;
        //            return false;
        //        }
        //    );
        //}


        /// <summary>
        /// 子孫Figureが追加・削除・変更されたときに登録・解除する．
        /// </summary>
        //private void HandleDescendantChanged(object sender, DetailedPropertyChangedEventArgs e) {
        //    switch (e.Kind) {
        //        case PropertyChangeKind.Add: {
        //            var fig = e.NewValue as IFigure;
        //            RegisterFigure(fig);
        //            break;
        //        }
        //        case PropertyChangeKind.Remove: {
        //            var fig = e.OldValue as IFigure;
        //            UnregisterFigure(fig);
        //            break;
        //        }
        //        case PropertyChangeKind.Clear: {
        //            var figs = e.OldValue as IFigure[];
        //            foreach (var fig in figs) {
        //                UnregisterFigure(fig);
        //            }
        //            break;
        //        }
        //        case PropertyChangeKind.Set: {
        //            var newFig = e.NewValue as IFigure;
        //            var oldFig = e.OldValue as IFigure;
        //            UnregisterFigure(oldFig);
        //            RegisterFigure(newFig);
        //            break;
        //        }
        //        default: {
        //            throw new ArgumentException("Invalid event kind");
        //        }
        //    }
        //}

        protected virtual void OnAutoScrollMinSizeChanged() {
            var handler = AutoScrollMinSizeChanged;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }
        
        /// <summary>
        /// スクロール後にFeedbackが正しく表示されるようにMouseMove/DragMoveイベントが起こるようにする
        /// </summary>
        private void HandleAutoScrollerAutoScrolled(object sender, AutoScrollEventArgs e) {
            OnMouseMove(_lastDragMoveEventArgs);
        }

        private void HandleRootFigureBoundsChanged(object sender, BoundsChangedEventArgs e) {
            //AutoScrollMinSize = e.NewBounds.Size + _canvasPadding.Size;
            if (e.NewBounds.Size != e.OldBounds.Size) {
                AutoScrollMinSize = e.NewBounds.Size;
            }

            if (e.IsMove) {
                AutoScrollPosition = new Point(
                    -AutoScrollPosition.X - e.LocationDelta.Width,
                    -AutoScrollPosition.Y - e.LocationDelta.Height
                );
            }
        }

        /// <summary>
        /// 登録されたfigureのBounds変更時にRootFigureのBoundsを調整する．
        /// </summary>
        private void HandleRegisteredFigureBoundsChanged(object sender, BoundsChangedEventArgs e) {
            if (_enableAutoAdjustRootFigureSize && !_DirtManager.IsInDirtying) {
                AdjustRootFigureBounds();
            }
        }

        /// <summary>
        /// スクロール直前に_rootViewのサイズが足りなければ拡張する．
        /// </summary>
        private void HandleAutoScrollerAutoScrolling(object sender, AutoScrollEventArgs e) {
            if (CanvasBackgroundImage != null) {
                Invalidate();
            }
            switch (e.ScrollDirection) {
                case ScrollDirection.Up: {
                    //if (RootFigureScope.Top - e.ScrollInterval < _rootFigure.Top) {
                    //    _rootFigure.Location = _rootFigure.Location - new Size(0, e.ScrollInterval);
                    //    _rootFigure.Height += e.ScrollInterval;
                    //}
                    break;
                }
                case ScrollDirection.Down: {
                    /// (_rootFigureのTop～表示領域の下の端に対応する点の幅) + スクロールインターバル
                    /// が_rootFigureの高さを超えてしまったら
                    if (Viewport.Bottom - _rootFigure.Top + e.ScrollInterval >
                        _rootFigure.Height
                    ) {
                        _rootFigure.Height += e.ScrollInterval;
                    }
                    break;
                }
                case ScrollDirection.Left: {
                    //if (RootFigureScope.Left - e.ScrollInterval < _rootFigure.Left) {
                    //    _rootFigure.Location = _rootFigure.Location - new Size(e.ScrollInterval, 0);
                    //    _rootFigure.Width += e.ScrollInterval;
                    //}
                    break;
                }
                case ScrollDirection.Right: {
                    if (Viewport.Right - _rootFigure.Left + e.ScrollInterval >
                        _rootFigure.Width
                    ) {
                        _rootFigure.Width += e.ScrollInterval;
                    }
                    break;
                }
            }
        }
    }
}
