/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Forms.Descriptions;
using System.Drawing;
using Mkamo.Figure.Core;
using Mkamo.Common.Externalize;
using Mkamo.Common.Disposable;
using Mkamo.Common.Forms.Drawing;
using System.Windows.Forms;
using Mkamo.Common.DataType;
using DataType = Mkamo.Common.DataType;
using Mkamo.Figure.Internal.Figures;
using Mkamo.Common.Core;
using Mkamo.Common.Diagnostics;
using System.Drawing.Drawing2D;

namespace Mkamo.Figure.Figures {
    using StyledText = Mkamo.StyledText.Core.StyledText;

    public abstract partial class AbstractNode: AbstractConnectable, INode {
        // ========================================
        // static field
        // ========================================
        protected static readonly string FontResourceKey = "AbstractNode.Font";
        protected static readonly string BrushResourceKey = "AbstractNode.Brush";
        protected static readonly string PenResourceKey = "AbstractNode.Pen";
        protected static readonly string SelectionBrushResourceKey = "AbstractNode.SelectionBrush";
        protected static readonly string SelectionPenResourceKey = "AbstractNode.SelectionPen";

        public static bool PreventPaintingLineBreakAndBlockBreak = false;

        // ========================================
        // field
        // ========================================
        private FontDescription _font;
        private Color _fontColor;
        private Color _foreground;
        private IBrushDescription _background;
        private bool _isForegroundEnabled;
        private bool _isBackgroundEnabled;
        private int _borderWidth;
        private DashStyle _borderDashStyle;
        private Size _minSize;
        private Size _maxSize;
        private AutoSizeKinds _autoSize;

        private Insets _padding;
        private INodeOuterFrame _outerFrame; /// lazy load

        // --- plain text ---
        private string _text;
        private Size _textSizeCache;
        private DataType::HorizontalAlignment _textHorizontalAlignment;
        private VerticalAlignment _textVerticalAlignment;

        // --- styled text ---
        private StyledText _styledText;
        private Size _styledTextSizeCache;
        private Color _selectionBorderColor;
        private IBrushDescription _selectionBrush;
        private Range _selection;
        private BlockRenderer _blockRenderer;
        private FontCache _fontCache;
        private SizeCache _sizeCache;
        private BoundsCache _boundsCache;
        private VisualLineCache _visualLineCache;
        private int _updateStyledTextDepth;

        private bool _showLineBreak = false; /// transient
        private bool _showBlockBreak = false; /// transient

        // ========================================
        // constructor
        // ========================================
        public AbstractNode(): base() {
            _font = SystemFontDescriptions.DefaultFont;
            _fontColor = FigureConsts.WindowTextColor;
            _foreground = FigureConsts.WindowTextColor;
            _background = FigureConsts.WindowBrush;
            _isForegroundEnabled = true;
            _isBackgroundEnabled = true;
            _borderWidth = 1;
            _borderDashStyle = DashStyle.Solid;
            _minSize = new Size(4, 4);
            _maxSize = new Size(int.MaxValue, int.MaxValue);
            _autoSize = AutoSizeKinds.None;

            _padding = new Insets(2);

            _text = string.Empty;
            _textSizeCache = Size.Empty;
            _textHorizontalAlignment = DataType::HorizontalAlignment.Left;
            _textVerticalAlignment = VerticalAlignment.Top;

            _styledText = null;
            _selectionBorderColor = FigureConsts.HighlightColor;
            _selectionBrush = FigureConsts.HighlightBrush;
            _fontCache = new FontCache();
            _updateStyledTextDepth = 0;

            _ResourceCache.RegisterResourceCreator(
                FontResourceKey,
                () => _font.CreateFont(),
                ResourceDisposingPolicy.Explicit
            );
            _ResourceCache.RegisterResourceCreator(
                PenResourceKey,
                () => {
                    var ret = new Pen(_foreground, _borderWidth);
                    ret.DashStyle = _borderDashStyle;
                    return ret;
                },
                ResourceDisposingPolicy.Immediate
            );
            _ResourceCache.RegisterResourceCreator(
                BrushResourceKey,
                () => _background == null? null: _background.CreateBrush(Bounds),
                ResourceDisposingPolicy.Immediate
            );
            _ResourceCache.RegisterResourceCreator(
                SelectionPenResourceKey,
                () => new Pen(_selectionBorderColor),
                ResourceDisposingPolicy.Immediate
            );
            _ResourceCache.RegisterResourceCreator(
                SelectionBrushResourceKey,
                () => _selectionBrush.CreateBrush(Bounds),
                ResourceDisposingPolicy.Immediate
            );
        }

        // ========================================
        // event
        // ========================================
        public event EventHandler MaxSizeChanged;

        // ========================================
        // property
        // ========================================
        // === IFigure ==========
        public override Rectangle Bounds {
            get { return base.Bounds; }
            set {
                /// 図形の移動ならAutoSizeは求めない
                if (Size == value.Size) {
                    base.Bounds = value;
                } else {
                    var newSize = MeasureAutoSize(value.Size);
                    base.Bounds = new Rectangle(value.Location, newSize);
                }
            }
        }

        public override Rectangle PaintBounds {
            get {
                var ret = GetStyledTextBoundsFor(Bounds);
                if (ret.IsEmpty) {
                    ret = Bounds;
                } else {
                    ret = Rectangle.Union(Bounds, ret);
                }
                ret.Inflate(Padding.Width + _borderWidth, Padding.Height + _borderWidth);
                return ret;
            }
        }

        // === INode ==========
        public virtual FontDescription Font {
            get { return _font; }
            set {
                if (value == _font) {
                    return;
                }
                _font = value;
                _ResourceCache.DisposeResource(FontResourceKey);
                InvalidatePaint();
            }
        }

        public virtual Color FontColor {
            get { return _fontColor; }
            set {
                if (value == _fontColor) {
                    return;
                }
                _fontColor = value;
                _ResourceCache.DisposeResource(FontResourceKey);
                InvalidatePaint();
            }
        }

        public virtual Color Foreground {
            get { return _foreground; }
            set {
                if (value == _foreground) {
                    return;
                }
                _foreground = value;
                _ResourceCache.DisposeResource(PenResourceKey);
                InvalidatePaint();
            }
        }

        public virtual IBrushDescription Background {
            get { return _background; }
            set {
                if (value == _background) {
                    return;
                }
                _background = value;
                _ResourceCache.DisposeResource(BrushResourceKey);
                InvalidatePaint();
            }
        }

        public bool IsForegroundEnabled {
            get { return _isForegroundEnabled; }
            set {
                if (value == _isForegroundEnabled) {
                    return;
                }
                _isForegroundEnabled = value;
                _ResourceCache.DisposeResource(PenResourceKey);
                InvalidatePaint();
            }
        }

        public bool IsBackgroundEnabled {
            get { return _isBackgroundEnabled; }
            set {
                if (value == _isBackgroundEnabled) {
                    return;
                }
                _isBackgroundEnabled= value;
                _ResourceCache.DisposeResource(BrushResourceKey);
                InvalidatePaint();
            }
        }

        public virtual int BorderWidth {
            get { return _borderWidth; }
            set {
                if (value == _borderWidth || value < 1) {
                    return;
                }
                _borderWidth = Math.Min(value, 8); /// 8 == DirtManager.REPAIR_MARGIN * 2
                _ResourceCache.DisposeResource(PenResourceKey);
                InvalidatePaint();
            }
        }

        public virtual DashStyle BorderDashStyle {
            get { return _borderDashStyle; }
            set {
                if (value == _borderDashStyle) {
                    return;
                }
                _borderDashStyle = value;
                _ResourceCache.DisposeResource(PenResourceKey);
                InvalidatePaint();
            }
        }

        public virtual Size MinSize {
            get { return _minSize; }
            set {
                if (value == _minSize) {
                    return;
                }
                if (value.Width > _maxSize.Width || value.Height > _maxSize.Height) {
                    return;
                }

                _minSize = value;
                if (Bounds.Width < _minSize.Width || Bounds.Height < _minSize.Height) {
                    Bounds = new Rectangle(
                        Bounds.Left,
                        Bounds.Top,
                        Math.Max(Bounds.Width, _minSize.Width),
                        Math.Max(Bounds.Height, _minSize.Height)
                    );
                }
            }
        }

        public virtual AutoSizeKinds AutoSizeKinds {
            get { return _autoSize; }
            set {
                if (_autoSize == value) {
                    return;
                }
                _autoSize = value;
                AdjustSize();
            }
        }

        public virtual Insets Padding {
            get { return _padding; }
            set { _padding = value; }
        }

        public virtual Rectangle ClientArea {
            get { return _padding.GetClientArea(Bounds); }
        }


        public Line LeftLine {
            get {
                var bounds = Bounds;
                return new Line(
                    new Point(bounds.Left, bounds.Top),
                    new Point(bounds.Left, bounds.Bottom - 1)
                );
            }
        } 

        public Line TopLine {
            get {
                var bounds = Bounds;
                return new Line(
                    new Point(bounds.Left, bounds.Top),
                    new Point(bounds.Right - 1, bounds.Top)
                );
            }
        } 

        public Line RightLine {
            get {
                var bounds = Bounds;
                return new Line(
                    new Point(bounds.Right - 1, bounds.Top),
                    new Point(bounds.Right - 1, bounds.Bottom - 1)
                );
            }
        } 

        public Line BottomLine {
            get {
                var bounds = Bounds;
                return new Line(
                    new Point(bounds.Left, bounds.Bottom - 1),
                    new Point(bounds.Right - 1, bounds.Bottom - 1)
                );
            }
        } 

        public virtual INodeOuterFrame OuterFrame {
            get {
                if (_outerFrame == null) {
                    _outerFrame = new NodeOuterFrame(this);
                }
                return _outerFrame;
            }
        }

        
        public virtual string Text {
            get { return _text; }
            set {
                _text = value?? string.Empty;
                _textSizeCache = Size.Empty;
            }
        }

        public virtual Size TextSize {
            get {
                if (string.IsNullOrEmpty(_text) || Root == null || Root.Canvas == null || Root.Canvas.IsDisposed) {
                    return Size.Empty;
                }
                if (_textSizeCache.IsEmpty) {
                    using (_ResourceCache.UseResource())
                    using (var context = GetGraphicsUsingContext()) {
                        if (context == null) {
                            return Size.Empty;
                        }

                        var g = context.Graphics;
                        var cliRect = Padding.GetClientArea(Bounds);
                        _textSizeCache = MeasureText(g, Text, _FontResource, cliRect.Width);
                    }
                }
                return _textSizeCache;
            }
        }

        //public virtual Rectangle TextBounds {
        //    get {
        //        var size = TextSize;
        //        if (size.IsEmpty) {
        //            return Rectangle.Empty;
        //        } else {
        //            var left = 0;
        //            switch (_textHorizontalAlignment) {
        //                case Mkamo.Common.DataType.HorizontalAlignment.Left: {
        //                    left = Padding.Left;
        //                    break;
        //                }
        //                case Mkamo.Common.DataType.HorizontalAlignment.Center: {

        //                    break;
        //                }
        //                case Mkamo.Common.DataType.HorizontalAlignment.Right: {
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //}

        public virtual DataType::HorizontalAlignment TextHorizontalAlignment {
            get { return _textHorizontalAlignment; }
            set { _textHorizontalAlignment = value; }
        }

        public virtual VerticalAlignment TextVerticalAlignment {
            get { return _textVerticalAlignment; }
            set { _textVerticalAlignment = value; }
        }

        public virtual bool ShowLineBreak {
            get { return _showLineBreak; }
            set {
                if (value == _showLineBreak) {
                    return;
                }
                _showLineBreak = value;
                InvalidatePaint();
            }
        }

        public virtual bool ShowBlockBreak {
            get { return _showBlockBreak; }
            set {
                if (value == _showBlockBreak) {
                    return;
                }
                _showBlockBreak = value;
                InvalidatePaint();
            }
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected Font _FontResource {
            get { return (Font) _ResourceCache.GetResource(FontResourceKey); }
        }

        protected Pen _PenResource {
            get { return (Pen) _ResourceCache.GetResource(PenResourceKey); }
        }

        protected Brush _BrushResource {
            get { return (Brush) _ResourceCache.GetResource(BrushResourceKey); }
        }

        protected Pen _SelectionPenResource {
            get { return (Pen) _ResourceCache.GetResource(SelectionPenResourceKey); }
        }

        protected Brush _SelectionBrushResource {
            get { return (Brush) _ResourceCache.GetResource(SelectionBrushResourceKey); }
        }


        //protected FontCache _FontCache {
        //    get { return _fontCache; }
        //}

        //protected SizeCache _SizeCache {
        //    get { return _sizeCache; }
        //}

        //protected BoundsCache _BoundsCache {
        //    get { return _boundsCache; }
        //}

        //protected VisualLineCache _VisualLineCache {
        //    get { return _visualLineCache; }
        //}

        // ========================================
        // method
        // ========================================
        // === IPersistable ==========
        public override void WriteExternal(IMemento memento, ExternalizeContext context) {
            base.WriteExternal(memento, context);

            memento.WriteSerializable("Font", _font == null? null: _font.Clone());
            memento.WriteSerializable("FontColor", _fontColor);
            memento.WriteSerializable("Foreground", _foreground);
            memento.WriteSerializable("Background", _background == null? null: _background.Clone());
            memento.WriteBool("IsForegroundEnabled", _isForegroundEnabled);
            memento.WriteBool("IsBackgroundEnabled", _isBackgroundEnabled);
            memento.WriteInt("BorderWidth", _borderWidth);
            memento.WriteSerializable("BorderDashStyle", _borderDashStyle);
            memento.WriteSerializable("MinSize", _minSize);
            memento.WriteSerializable("MaxSize", _maxSize);
            memento.WriteSerializable("AutoSize", _autoSize);

            memento.WriteSerializable("Padding", _padding);

            memento.WriteSerializable("StyledText", _styledText == null? null: _styledText.CloneDeeply());
            memento.WriteSerializable("SelectionBorderColor", _selectionBorderColor);
            memento.WriteSerializable("SelectionBrush", _selectionBrush);

            memento.WriteString("Text", _text);
            memento.WriteSerializable("TextHorizontalAlignment", _textHorizontalAlignment);
            memento.WriteSerializable("TextVerticalAlignment", _textVerticalAlignment);
        }

        public override void ReadExternal(IMemento memento, ExternalizeContext context) {
            base.ReadExternal(memento, context);

            _font = (FontDescription) memento.ReadSerializable("Font");
            _fontColor = (Color) memento.ReadSerializable("FontColor");
            _foreground = (Color) memento.ReadSerializable("Foreground");
            _background = (IBrushDescription) memento.ReadSerializable("Background");
            _isForegroundEnabled = memento.ReadBool("IsForegroundEnabled");
            _isBackgroundEnabled = memento.ReadBool("IsBackgroundEnabled");
            _borderWidth = memento.ReadInt("BorderWidth");
            _borderDashStyle = (DashStyle) memento.ReadSerializable("BorderDashStyle");
            _minSize = (Size) memento.ReadSerializable("MinSize");
            _maxSize = (Size) memento.ReadSerializable("MaxSize");
            _autoSize = (AutoSizeKinds) memento.ReadSerializable("AutoSize");

            _padding = (Insets) memento.ReadSerializable("Padding");

            _selectionBorderColor = (Color) memento.ReadSerializable("SelectionBorderColor");
            _selectionBrush = (IBrushDescription) memento.ReadSerializable("SelectionBrush");
            StyledText = (StyledText) memento.ReadSerializable("StyledText");
            
            _text = memento.ReadString("Text");
            _textHorizontalAlignment =
                (DataType::HorizontalAlignment) memento.ReadSerializable("TextHorizontalAlignment");
            _textVerticalAlignment =
                (VerticalAlignment) memento.ReadSerializable("TextVerticalAlignment");
        }

        public override void MakeTransparent(float ratio) {
            FontColor = Color.FromArgb(
                (int) (255 - (255 - FontColor.R) * ratio),
                (int) (255 - (255 - FontColor.G) * ratio),
                (int) (255 - (255 - FontColor.B) * ratio)
            );

            /// 枠を重ねると色が濃くなってしまうので透過させない
            /// Foreground = Color.FromArgb((int) (Foreground.A * ratio), Foreground);
            Foreground = Color.FromArgb(
                (int) (255 - (255 - Foreground.R) * ratio),
                (int) (255 - (255 - Foreground.G) * ratio),
                (int) (255 - (255 - Foreground.B) * ratio)
            );

            Background = BrushDescriptionUtil.CreateFrom(Background, ratio);

            if (_styledText != null) {
                _styledText.Accept(
                    flow => {
                        flow.Color = Color.FromArgb(
                            (int) (255 - (255 - flow.Color.R) * ratio),
                            (int) (255 - (255 - flow.Color.G) * ratio),
                            (int) (255 - (255 - flow.Color.B) * ratio)
                        );
                        return false;
                    }
                );
            }
        }

        // === INode ==========
        /// <summary>
        /// AutoSizeの設定にしたがって，PreferredSizeとMinSize/MaxSizeを使って
        /// expectedを元に自動で適切なサイズを計測する
        /// </summary>
        public virtual Size MeasureAutoSize(Size expected) {
            if (AutoSizeKinds == AutoSizeKinds.None) {
                return new Size(
                    Math.Min(
                        Math.Max(expected.Width, MinSize.Width),
                        MaxSize.Width
                    ),
                    Math.Min(
                        Math.Max(expected.Height, MinSize.Height),
                        MaxSize.Height
                    )
                );
            }

            Size ret = expected;

            int? maxWidth =
                (EnumUtil.HasAllFlags((int) AutoSizeKinds, (int) AutoSizeKinds.GrowWidth)?
                _maxSize.Width: ret.Width);
            int? maxHeight =
                (EnumUtil.HasAllFlags((int) AutoSizeKinds, (int) AutoSizeKinds.GrowHeight)?
                _maxSize.Height: ret.Height);
            var constraint = new SizeConstraint(maxWidth, maxHeight);

            Measure(constraint);
            var prefSize = PreferredSize;

            if (
                EnumUtil.HasAllFlags((int) AutoSizeKinds, (int) AutoSizeKinds.GrowWidth) &&
                ret.Width < prefSize.Width
            ) {
                ret.Width = prefSize.Width;
            }
            if (
                EnumUtil.HasAllFlags((int) AutoSizeKinds, (int) AutoSizeKinds.ReduceWidth) &&
                ret.Width > prefSize.Width
            ) {
                ret.Width = prefSize.Width;
            }

            if (
                EnumUtil.HasAllFlags((int) AutoSizeKinds, (int) AutoSizeKinds.GrowHeight) &&
                ret.Height < prefSize.Height
            ) {
                ret.Height = prefSize.Height;
            }
            if (
                EnumUtil.HasAllFlags((int) AutoSizeKinds, (int) AutoSizeKinds.ReduceHeight) &&
                ret.Height > prefSize.Height
            ) {
                ret.Height = prefSize.Height;
            }

            return new Size(
                Math.Min(
                    Math.Max(ret.Width, MinSize.Width),
                    MaxSize.Width
                ),
                Math.Min(
                    Math.Max(ret.Height, MinSize.Height),
                    MaxSize.Height
                )
            );
        }

        /// <summary>
        /// AutoSizeKindsの設定に従ってSizeを調整する．
        /// </summary>
        public void AdjustSize() {
            Size = MeasureAutoSize(Size);
        }


        // ------------------------------
        // protected
        // ------------------------------
        // === AbstractNode ==========
        protected virtual void PaintText(Graphics g) {
            PaintText(g, ClientArea);
        }

        protected virtual void PaintText(Graphics g, Rectangle rect) {
            if (string.IsNullOrEmpty(_text)) {
                return;
            }

            using (_ResourceCache.UseResource()) {
                DrawText(g, Text, _FontResource, rect, FontColor);
            }
        }

        // ========================================
        // class
        // ========================================
        private class NodeOuterFrame: INodeOuterFrame {
            // ========================================
            // field
            // ========================================
            private AbstractNode _owner;

            // ========================================
            // constructor
            // ========================================
            public NodeOuterFrame(AbstractNode owner) {
                _owner = owner;
            }

            // ========================================
            // property
            // ========================================
            protected AbstractNode _Owner {
                get { return _owner; }
            }

            // ========================================
            // method
            // ========================================
            public virtual bool IntersectsWith(Line line) {
                if (_owner.LeftLine.IntersectsWith(line)) {
                    return true;
                }
                if (_owner.RightLine.IntersectsWith(line)) {
                    return true;
                }
                if (_owner.TopLine.IntersectsWith(line)) {
                    return true;
                }
                if (_owner.BottomLine.IntersectsWith(line)) {
                    return true;
                }
                return false;
            }

            /// <summary>
            /// 外枠との交点を求める．
            /// 複数の交点がある場合，line.Startに最も近い点を返す．
            /// </summary>
            public virtual Point GetIntersectionPoint(Line line) {
                /// 単純にBoundsの矩形との交点を求める

                var lLine = _owner.LeftLine;
                var tLine = _owner.TopLine;
                var rLine = _owner.RightLine;
                var bLine = _owner.BottomLine;
    
                var pts = new List<Point>(4);
                if (line.IntersectsWith(lLine)) {
                    pts.Add(line.GetIntersectionPoint(lLine));
                }
                if (line.IntersectsWith(tLine)) {
                    pts.Add(line.GetIntersectionPoint(tLine));
                }
                if (line.IntersectsWith(rLine)) {
                    pts.Add(line.GetIntersectionPoint(rLine));
                }
                if (line.IntersectsWith(bLine)) {
                    pts.Add(line.GetIntersectionPoint(bLine));
                }
    
                return pts.Count == 0? Point.Empty: pts.FindMin(pt => (int) PointUtil.GetDistance(line.Start, pt));
            }
    
            /// <summary>
            /// ptに最も近い外枠上点を返す．
            /// </summary>
            public virtual Point GetNearestPoint(Point pt) {
                return RectUtil.GetNearestPointAndLineDirection(_owner.Bounds, pt).Item1;
            }

        }

    }
}
