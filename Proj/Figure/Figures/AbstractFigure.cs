/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using System.Drawing;
using Mkamo.Common.Visitor;
using Mkamo.Common.Structure;
using Mkamo.Common.Util;
using Mkamo.Common.Event;
using System.Drawing.Drawing2D;
using Mkamo.Common.Collection;
using Mkamo.Common.Forms.MouseOperatable;
using System.Windows.Forms;
using Mkamo.Figure.Internal.Core;
using System.Runtime.Serialization;
using Mkamo.Common.Externalize;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Mkamo.Common.Disposable;
using System.Diagnostics;
using log4net;
using System.Reflection;
using System.Collections.ObjectModel;
using Mkamo.Common.Diagnostics;
using Mkamo.Common.Win32.Gdi32;
using Mkamo.Common.Core;

namespace Mkamo.Figure.Figures {
    public abstract partial class AbstractFigure: IFigure {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected static readonly Size PaintMarginDefault = new Size(4, 4);


        // ========================================
        // field
        // ========================================
        private StructuredSupport<IFigure> _structure;

        private IDragSource _dragSource;
        private IDragTarget _dragTarget;

        private ILayout _layout;
        private Dictionary<IFigure, object> _layoutConstraints; /// lazy

        private Size _preferredSize;

        private bool _isVisible;

        private Func<MouseEventArgs, Cursor> _cursorProvider;

        private List<IMouseOperatable> _mouseEventsToForwards; /// lazy

        private IDictionary<string, object> _transiendData; /// lazy
        private IDictionary<string, object> _persistentData; /// lazy

        private ResourceCache _resourceCache; /// lazy

        private RootFigure _rootCache = null;

        // ========================================
        // constructor
        // ========================================
        public AbstractFigure() {
            _structure = new StructuredSupport<IFigure>(this);
            _structure.DetailedPropertyChanged += (sender, e) => {
                if (e.PropertyName == ICompositeProperty.Parent) {
                    OnParentChanged(e);
                } else if (e.PropertyName == ICompositeProperty.Children) {
                    OnChildrenChanged(e);
                }
            };

            _preferredSize = Size.Empty;

            _isVisible = true;

            _cursorProvider = null;
        }

        // ========================================
        // event
        // ========================================
        public event EventHandler<DetailedPropertyChangedEventArgs> ParentChanged;
        public event EventHandler<DetailedPropertyChangedEventArgs> ChildrenChanged;
        public event EventHandler<DetailedPropertyChangedEventArgs> DescendantChanged;

        public event EventHandler<BoundsChangedEventArgs> BoundsChanged;
        public event EventHandler<EventArgs> LayoutDone;
        public event EventHandler<EventArgs> VisibleChanged;

        public event EventHandler<MouseEventArgs> MouseClick;
        public event EventHandler<MouseEventArgs> MouseDoubleClick;
        public event EventHandler<MouseEventArgs> MouseTripleClick;
        public event EventHandler<MouseEventArgs> MouseDown;
        public event EventHandler<MouseEventArgs> MouseUp;
        public event EventHandler<MouseEventArgs> MouseMove;
        public event EventHandler<EventArgs> MouseEnter;
        public event EventHandler<EventArgs> MouseLeave;
        public event EventHandler<MouseHoverEventArgs> MouseHover;
        public event EventHandler<MouseEventArgs> DragStart;
        public event EventHandler<MouseEventArgs> DragMove;
        public event EventHandler<MouseEventArgs> DragFinish;
        public event EventHandler<EventArgs> DragCancel;

        public event EventHandler<ShortcutKeyProcessEventArgs> ShortcutKeyProcess;
        public event EventHandler<KeyEventArgs> KeyDown;
        public event EventHandler<KeyEventArgs> KeyUp;
        public event EventHandler<KeyPressEventArgs> KeyPress;
        public event EventHandler<PreviewKeyDownEventArgs> PreviewKeyDown;


        // ========================================
        // property
        // ========================================
        // === IMouseOperatable ==========
        public IDragSource DragSource {
            get { return _dragSource; }
            set { _dragSource = value; }
        }

        public IDragTarget DragTarget {
            get { return _dragTarget; }
            set { _dragTarget = value; }
        }

        // === IFigure ==========
        public virtual IFigure Parent {
            get { return _structure.Parent; }
            set { _structure.Parent = value; }
        }

        public virtual Collection<IFigure> Children {
            get { return _structure.Children; }
        }

        public abstract Rectangle Bounds { get; set; }

        public virtual Point Location {
            get { return Bounds.Location; }
            set { Bounds = new Rectangle(value, Bounds.Size); }
        }

        public virtual Size Size {
            get { return Bounds.Size; }
            set { Bounds = new Rectangle(Bounds.Location, value); }
        }

        public virtual int Left {
            get { return Bounds.Left; }
            set { Bounds = new Rectangle(value, Top, Width, Height); }
        }
        public virtual int Top {
            get { return Bounds.Top; }
            set { Bounds = new Rectangle(Left, value, Width, Height); }
        }
        public virtual int Width {
            get { return Bounds.Width; }
            set { Bounds = new Rectangle(Left, Top, value, Height); }
        }
        public virtual int Height {
            get { return Bounds.Height; }
            set { Bounds = new Rectangle(Left, Top, Width, value); }
        }
        public virtual int Right {
            get { return Bounds.Right; }
        }
        public virtual int Bottom {
            get { return Bounds.Bottom; }
        }
        public virtual Point Center {
            get {
                var bounds = Bounds;
                return new Point(
                    bounds.Left + bounds.Width / 2,
                    bounds.Top + bounds.Height / 2
                );
            }
        }

        public virtual Rectangle PaintBounds {
            get { return Bounds; }
        }

        public virtual Size PreferredSize {
            get { return _preferredSize; }
        }

        public virtual RootFigure Root {
            get {
                if (_rootCache == null) {
                    _rootCache = Parent == null ? null : Parent.Root;
                }
                return _rootCache;
            }
        }


        public virtual bool IsVisible {
            get { return _isVisible; }
            set {
                if (value == _isVisible) {
                    return;
                }
                _isVisible = value;
                OnVisibleChanged();
            }
        }

        public virtual ILayout Layout {
            get { return _layout; }
            set {
                if (value == _layout) {
                    return;
                }

                if (_layout != null) {
                    _layout.Owner = null;
                }
                _layout = value;
                if (_layout != null) {
                    _layout.Owner = this;
                }
                InvalidateLayout();
            }
        }

        /// <summary>
        /// SaveTo()/LoadFrom()でlayoutのもつIFigure->objectの情報をうまく保存するために使う．
        /// </summary>
        public IDictionary<IFigure, object> LayoutConstraints {
            get {
                if (_layoutConstraints == null) {
                    _layoutConstraints = new Dictionary<IFigure, object>();
                }
                return _layoutConstraints;
            }
        }

        public virtual IDirtManager DirtManager {
            get {
                if (Parent == null || Parent.DirtManager == null) {
                    return FigureConsts.NullDirtManager;
                }
                return Parent.DirtManager;
            }
        }

        public virtual IDictionary<string, object> TransientData {
            get {
                if (_transiendData == null) {
                    _transiendData = new Dictionary<string, object>();
                }
                return _transiendData;
            }
        }

        public virtual IDictionary<string, object> PersistentData {
            get {
                if (_persistentData == null) {
                    _persistentData = new Dictionary<string, object>();
                }
                return _persistentData;
            }
        }

        public virtual Func<MouseEventArgs, Cursor> CursorProvider {
            get { return _cursorProvider; }
            set { _cursorProvider = value; }
        }

        // ------------------------------
        // protected
        // ------------------------------
        /// <summary>
        /// Locationが変わったときに子Figureが追随して移動するべきかどうか
        /// </summary>
        protected virtual bool _ChildrenFollowOnBoundsChanged {
            get { return true; }
        }

        protected List<IMouseOperatable> _MouseEventsToForwards {
            get {
                return _mouseEventsToForwards?? (_mouseEventsToForwards = new List<IMouseOperatable>());
            }
        }

        // ------------------------------
        // protected internal
        // ------------------------------
        protected internal virtual EventDispatcher _EventDispatcher {
            get {
                RootFigure root = Root;
                return root == null? null: root._EventDispatcher;
            }
            set { }
        }

        protected internal ResourceCache _ResourceCache {
            get {
                if (_resourceCache == null) {
                    _resourceCache = new ResourceCache();

                    /// 親がnullになったときにのみcacheを解放するので
                    /// 親がnullのときはcacheさせない
                    _resourceCache.Enabled = Parent != null;
                }
                return _resourceCache;
            }
        }



        // ========================================
        // method
        // ========================================
        // === IStructured ==========
        public void Accept(IVisitor<IFigure> visitor) {
            _structure.Accept(visitor);
        }
        public void Accept(IVisitor<IFigure> visitor, NextVisitOrder order) {
            _structure.Accept(visitor, order);
        }

        public void Accept(Predicate<IFigure> visitPred) {
            _structure.Accept(visitPred);
        }

        public void Accept(Predicate<IFigure> visitPred, Action<IFigure> endVisitAction, NextVisitOrder order) {
            _structure.Accept(visitPred, endVisitAction, order);
        }

        // === IKeyOperatable ==========
        public void HandleShortcutKeyProcess(ShortcutKeyProcessEventArgs e) {
            OnShortcutKeyProcess(e);
        }

        public void HandleKeyDown(KeyEventArgs e) {
            OnKeyDown(e);
        }

        public void HandleKeyUp(KeyEventArgs e) {
            OnKeyUp(e);
        }

        public void HandleKeyPress(KeyPressEventArgs e) {
            OnKeyPress(e);
        }

        public void HandlePreviewKeyDown(PreviewKeyDownEventArgs e) {
            OnPreviewKeyDown(e);
        }


        // === IFigure ==========
        // --- bounds ---
        public abstract bool ContainsPoint(Point pt);
        public abstract bool IntersectsWith(Rectangle rect);
        public abstract void Move(Size delta, IEnumerable<IFigure> movingFigures);

        public void Move(Size delta) {
            Move(delta, null);
        }

        // --- paint ---
        public void Paint(Graphics g) {
            PaintSelf(g);
            PaintChildren(g);
        }

        public Rectangle GetVisibleBounds() {
            if (Root != null) {
                var viewport = Root.Viewport;
                return Rectangle.Union(Bounds, viewport);
            } else {
                return Rectangle.Empty;
            }
        }

        // --- layout ---
        public void Measure(SizeConstraint constraint) {
            var selfSize = MeasureSelf(constraint);
            var childrenSize = MeasureChildren(constraint);
            _preferredSize = new Size(
                Math.Max(selfSize.Width, childrenSize.Width),
                Math.Max(selfSize.Height, childrenSize.Height)
            );
        }

        public void Arrange() {
            if (_layout != null) {
                _layout.Arrange(this);
                OnLayoutDone();
            }
        }

        public void SetLayoutConstraint(IFigure child, object constraint) {
            Contract.Requires(child != null && child.Parent == this);

            LayoutConstraints[child] = constraint;
            InvalidateLayout();
        }

        public object GetLayoutConstraint(IFigure child) {
            Contract.Requires(child != null && child.Parent == this);

            if (LayoutConstraints.ContainsKey(child)) {
                return LayoutConstraints[child];
            }
            return null;
        }

        // --- cloning ---
        public IFigure CloneFigure() {
            var keys = GetNonCompositeExternalizableKeys();
            return CloneFigure(
                (key, ext) => {
                    return keys == null || !keys.Contains(key);
                }
            );
        }

        public IFigure CloneFigureOnly() {
            var keys = GetNonCompositeExternalizableKeys();
            return CloneFigure(
                (key, ext) => {
                    if (key == "Children") {
                        return false;
                    }
                    return keys == null || !keys.Contains(key);
                }
            );
        }

        public IFigure CloneFigure(ExternalizableFilter filter) {
            var persister = new Externalizer();
            var mem = persister.Save(this, filter);
            return persister.Load(mem, null) as IFigure;
        }

        protected virtual IEnumerable<string> GetNonCompositeExternalizableKeys() {
            return null;
        }

        // --- invalidation ---
        public virtual void InvalidatePaint() {
            InvalidatePaint(PaintBounds);
        }

        public virtual void InvalidatePaint(Rectangle rect) {
            DirtManager.DirtyPaint(rect);
        }

        public void InvalidateLayout() {
            DirtManager.DirtyLayout(this);
        }


        // --- find ---
        public virtual IFigure FindFigure(Predicate<IFigure> finder, bool stopOnThisNotFinderTarget) {
            var isTargetThis = finder(this);

            if (stopOnThisNotFinderTarget) {
                if (!isTargetThis) {
                    return null;
                }
            }

            var found = default(IFigure);
            if (_structure.HasChildren) {
                for (int i = Children.Count - 1; i >= 0; --i) {
                    found = Children[i].FindFigure(finder, stopOnThisNotFinderTarget);
                    if (found != null) {
                        return found;
                    }
                }
            }

            if (isTargetThis) {
                return this;
            }
            return null;
        }

        public IFigure FindFigure(Predicate<IFigure> finder) {
            return FindFigure(finder, false);
        }

        public virtual IList<IFigure> FindFigures(Predicate<IFigure> finder, bool stopOnThisNotFinderTarget) {
            List<IFigure> ret = new List<IFigure>();

            /// 子IFigureの走査をやめるかを決める処理
            Predicate<IFigure> stopDecider = fig => !finder(fig);

            Accept(
                stopOnThisNotFinderTarget? stopDecider: null,
                fig => {
                    if (finder(fig)) {
                        ret.Add(fig);
                    }
                },
                NextVisitOrder.NegativeOrder
            );

            return ret;
        }

        public IList<IFigure> FindFigures(Predicate<IFigure> finder) {
            return FindFigures(finder, false);
        }


        // --- z-order ---
        public void BringToBefore(IFigure before) {
            if (before == null || before == this) {
                return;
            }

            if (Parent == null || !Parent.Children.Contains(before)) {
                return;
            }

            Parent.Children.Remove(this);
            int beforeIndex = Parent.Children.IndexOf(before);
            Parent.Children.Insert(beforeIndex, this);
        }

        public void BringToAfter(IFigure after) {
            if (after == null || after == this) {
                return;
            }

            if (Parent == null || !Parent.Children.Contains(after)) {
                return;
            }

            Parent.Children.Remove(this);
            int afterIndex = Parent.Children.IndexOf(after);
            Parent.Children.Insert(afterIndex + 1, this);
        }

        public void BringToFront(int step) {
            if (step < 0) {
                throw new ArgumentOutOfRangeException("Step must be at least 0");
            }

            if (Parent != null) {
                var parent = Parent;
                int index = parent.Children.IndexOf(this);
                if (index + step < Parent.Children.Count) {
                    parent.Children.RemoveAt(index);
                    parent.Children.Insert(index + step, this);
                } else {
                    parent.Children.RemoveAt(index);
                    parent.Children.Add(this);
                }
            }
        }

        public void BringToBack(int step) {
            if (step < 0) {
                throw new ArgumentOutOfRangeException("Step must be at least 0");
            }

            if (Parent != null) {
                var parent = Parent;
                int index = parent.Children.IndexOf(this);
                if (index - step > 0) {
                    parent.Children.RemoveAt(index);
                    parent.Children.Insert(index - step, this);
                } else {
                    parent.Children.RemoveAt(index);
                    parent.Children.Insert(0, this);
                }
            }
        }

        public void BringToFrontMost() {
            if (Parent != null) {
                var parent = Parent;
                int index = parent.Children.IndexOf(this);
                if (index < Parent.Children.Count - 1) {
                    parent.Children.RemoveAt(index);
                    parent.Children.Add(this);
                }
            }
        }

        public void BringToBackMost() {
            if (Parent != null) {
                var parent = Parent;
                int index = parent.Children.IndexOf(this);
                if (index > 0) {
                    parent.Children.RemoveAt(index);
                    parent.Children.Insert(0, this);
                }
            }
        }

        public abstract void MakeTransparent(float ratio);

        public void ForwardMouseEvents(IMouseOperatable mouseOperatable) {
            if (mouseOperatable == null || _MouseEventsToForwards.Contains(mouseOperatable)) {
                return;
            }
            _MouseEventsToForwards.Add(mouseOperatable);
        }

        public void StopForwardMouseEvents(IMouseOperatable mouseOperatable) {
            if (_mouseEventsToForwards == null) {
                return;
            }
            if (_MouseEventsToForwards.Contains(mouseOperatable)) {
                _MouseEventsToForwards.Remove(mouseOperatable);
            }
        }


        // --- text ---
        public virtual Size MeasureText(Graphics g, string text, Font font, int proposedWidth) {
            if (Root != null && Root.Canvas.UseGdiPlus) {
                /// Feedbackの表示でRoot == nullのことがある

                var fmt = new StringFormat(StringFormat.GenericTypographic);
                fmt.FormatFlags =
                    StringFormatFlags.NoWrap |
                    StringFormatFlags.NoClip;
                return Size.Ceiling(g.MeasureString(text, font, proposedWidth, fmt));

            } else {
                using (var f = (Font) font.Clone())
                using (var renderer = new Gdi32TextRenderer(g, f)) {
                    //renderer.TextAlignMode = TextAlignModes.TA_LEFT | TextAlignModes.TA_TOP;
                    return renderer.MeasureText(text);
                }
            }
        }

        public virtual Size MeasureText(Graphics g, string text, Font font, int clipWidth, out int drawableLen) {
            //Logger.Debug("MeasureText clipped " + text);
            // todo: BlockRendererのMeasureVisualLines()でContract.Ensure()を満たさなくなる
            //if (Root.Canvas.UseGdiPlus) {
            //    var fmt = new StringFormat(StringFormat.GenericTypographic);
            //    var dummy = -1;
            //    var size = new SizeF(clipWidth, 20);
            //    return Size.Ceiling(g.MeasureString(text, font, size, fmt, out drawableLen, out dummy));

            //} else {
                using (var f = (Font) font.Clone())
                using (var renderer = new Gdi32TextRenderer(g, f)) {
                    //renderer.TextAlignMode = TextAlignModes.TA_LEFT | TextAlignModes.TA_TOP;
                    return renderer.MeasureText(text, clipWidth, out drawableLen);
                }
            //}
        }


        // ------------------------------
        // protected internal
        // ------------------------------
        protected internal GraphicsUsingContext GetGraphicsUsingContext() {
            return (Root == null || Root.Canvas == null || Root.Canvas.IsDisposed) ? null : Root.Canvas.GetGraphicsUsingContext();
        }

        // ------------------------------
        // protected
        // ------------------------------
        // --- text ---
        protected virtual void DrawText(
            Graphics g, string text, Font font, Rectangle rect, Color color
        ) {
            if (Root.Canvas.UseGdiPlus) {
                using (var brush = new SolidBrush(color)) {
                    var fmt = new StringFormat(StringFormat.GenericTypographic);
                    fmt.FormatFlags =
                        StringFormatFlags.NoWrap |
                        StringFormatFlags.NoClip;
                    g.DrawString(text, font, brush, rect.Location, fmt);
                }
            } else {
                using (var f = (Font) font.Clone())
                using (var renderer = new Gdi32TextRenderer(g, f)) {
                    renderer.BkMode = BkMode.TRANSPARENT;
                    //renderer.TextAlignMode = TextAlignModes.TA_LEFT | TextAlignModes.TA_TOP;
                    renderer.TextColor = color;
                    var r = new Rectangle(
                        rect.Left + Root.Canvas.AutoScrollPosition.X - Root.Left,
                        rect.Top + Root.Canvas.AutoScrollPosition.Y - Root.Top,
                        rect.Width,
                        rect.Height
                    );
                    renderer.DrawText(text, r);
                }
            }

            // for debug
            //Console.WriteLine("DrawText: " + text);
            //var trace = new System.Diagnostics.StackTrace();
            //for (int StackLoop = 0; StackLoop < trace.FrameCount; ++StackLoop) {
            //    var frame = trace.GetFrame(StackLoop);
            //    var method = frame.GetMethod();
            //    Console.WriteLine(method.ToString());
            //}
            //Console.WriteLine("---");
        }

        protected virtual void DrawTextCenter(
            Graphics g, string text, Font font, Rectangle rect, Color color
        ) {
            if (Root.Canvas.UseGdiPlus) {
                using (var brush = new SolidBrush(color)) {
                    var fmt = (StringFormat) StringFormat.GenericTypographic.Clone();
                    fmt.Alignment = StringAlignment.Center;
                    fmt.LineAlignment = StringAlignment.Center;
                    fmt.FormatFlags =
                        StringFormatFlags.NoWrap |
                        StringFormatFlags.NoClip;
                    g.DrawString(text, font, brush, rect, fmt);
                }
            } else {
                //TextRenderer.DrawText(g, text, font, rect, color, GetBulletFormatFlags());

                using (var f = (Font) font.Clone())
                using (var renderer = new Gdi32TextRenderer(g, f)) {
                    renderer.BkMode = BkMode.TRANSPARENT;
                    //renderer.TextAlignMode = TextAlignModes.TA_CENTER | TextAlignModes.TA_TOP;
                    renderer.TextColor = color;

                    var size = renderer.MeasureText(text);
                    var left = rect.Left + (rect.Width - size.Width) / 2 + 1;
                    var top = rect.Top + (rect.Height - size.Height) / 2 + 1;
                    var width = size.Width;
                    var height = size.Height;

                    var r = new Rectangle(
                        left + Root.Canvas.AutoScrollPosition.X - Root.Left,
                        top + Root.Canvas.AutoScrollPosition.Y - Root.Top,
                        width,
                        height
                    );

                    renderer.DrawText(text, r);
                }
            }
        }

        protected virtual TextFormatFlags GetFormatFlags() {
            var ret =
                TextFormatFlags.PreserveGraphicsClipping |
                TextFormatFlags.PreserveGraphicsTranslateTransform |
                TextFormatFlags.NoClipping |
                TextFormatFlags.NoPrefix |
                TextFormatFlags.NoPadding;

            return ret;
        }

        protected virtual TextFormatFlags GetBulletFormatFlags() {
            var ret =
                TextFormatFlags.PreserveGraphicsClipping |
                TextFormatFlags.PreserveGraphicsTranslateTransform |
                TextFormatFlags.NoClipping |
                TextFormatFlags.NoPrefix |
                TextFormatFlags.NoPadding |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.HorizontalCenter;

            return ret;
        }

        // --- paint ---
        protected abstract void PaintSelf(Graphics g);

        protected virtual void PaintChildren(Graphics g) {
            if (_structure.HasChildren) {
                for (int i = 0, len = Children.Count; i < len; ++i) {
                    var child = Children[i];
                    var clipBounds = g.ClipBounds;
                    var r = child.PaintBounds;
                    if (child.IsVisible && clipBounds.IntersectsWith(r)) {
                        try {
                            if (!Root.Canvas.UseGdiPlus) {
                                /// DrawText()の幅がかなり変わるので
                                /// SetClip()すると右端が切れてしまう
                                g.SetClip(r, CombineMode.Intersect);
                            }
                            child.Paint(g);
                        } catch (Exception e) {
                            Logger.Warn("Paint Child failed.", e);
                            var childBounds = child.Bounds;
                            g.DrawRectangle(Pens.Red, childBounds);
                            g.DrawLine(Pens.Red, childBounds.Location, new Point(childBounds.Right - 1, childBounds.Bottom - 1));
                            g.DrawLine(Pens.Red, new Point(childBounds.Right - 1, childBounds.Top), new Point(childBounds.Left, childBounds.Bottom - 1));
                                
                        } finally {
                            g.SetClip(clipBounds);
                        }
                    }
                }
            }
        }

        // --- layout ---
        protected virtual Size MeasureSelf(SizeConstraint constraint) {
            /// デフォルトでは今のサイズをconstraintに収めたサイズ
            return constraint.MeasureConstrainedSize(Bounds.Size);
        }

        protected Size MeasureChildren(SizeConstraint constraint) {
            if (_layout == null) {
                // todo: _layoutが設定されていなければ子の現在位置から，すべての子を囲む最小の矩形のsizeを返すようにする
                //       FreeLayoutクラスを作ってそれのMeasurePreferredSize()を呼ぶようにする
                return Size.Empty;
            } else {
                return _layout.Measure(this, constraint);
            }
        }

        // --- event handler ---
        protected virtual void HandleChildrenDescendantChanged(object sender, DetailedPropertyChangedEventArgs e) {
            OnDescendantChanged(e);
        }

        // --- event ---
        protected virtual void OnParentChanged(DetailedPropertyChangedEventArgs e) {
            _rootCache = null;

            if (_resourceCache != null) {
                /// 親がnullすなわちCanvasから切り離されたら
                if (Parent == null && this != Root) {
                    /// ResourceCacheを解放する
                    DisposeResourceCacheAll();
                }
                var enabled = Parent != null;
                Accept(
                    fig => {
                        var af = fig as AbstractFigure;
                        if (af != null) {
                            af._ResourceCache.Enabled = enabled;
                        }
                        return false;
                    }
                );
            }

            var handler = ParentChanged;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void OnChildrenChanged(DetailedPropertyChangedEventArgs e) {
            using (DirtManager.BeginDirty()) {
                switch (e.Kind) {
                    case PropertyChangeKind.Add: {
                        var child = e.NewValue as IFigure;
                        child.DescendantChanged += HandleChildrenDescendantChanged;
                        child.InvalidatePaint();
                        child.InvalidateLayout();
                        break;
                    }
                    case PropertyChangeKind.Remove: {
                        var child = e.OldValue as IFigure;
                        if (_layoutConstraints != null && _layoutConstraints.ContainsKey(child)) {
                            _layoutConstraints.Remove(child);
                        }
                        child.DescendantChanged -= HandleChildrenDescendantChanged;
                        InvalidatePaint();
                        break;
                    }
                    case PropertyChangeKind.Clear: {
                        var children = e.OldValue as IFigure[];
                        if (_layoutConstraints != null) {
                            _layoutConstraints.Clear();
                        }
                        foreach (var child in children) {
                            child.DescendantChanged -= HandleChildrenDescendantChanged;
                        }
                        InvalidatePaint();
                        break;
                    }
                    case PropertyChangeKind.Set: {
                        var oldChild = e.OldValue as IFigure;
                        var newChild = e.NewValue as IFigure;
                        if (_layoutConstraints != null && _layoutConstraints.ContainsKey(oldChild)) {
                            _layoutConstraints.Remove(oldChild);
                        }
                        oldChild.DescendantChanged -= HandleChildrenDescendantChanged;
                        newChild.DescendantChanged += HandleChildrenDescendantChanged;
                        oldChild.InvalidatePaint();
                        newChild.InvalidatePaint();
                        newChild.InvalidateLayout();
                        break;
                    }
                    default: {
                        throw new ArgumentException("kind");
                    }
                }

                InvalidateLayout();
    
                var handler = ChildrenChanged;
                if (handler != null) {
                    handler(this, e);
                }
    
                OnDescendantChanged(e);
            }
        }

        protected virtual void OnDescendantChanged(DetailedPropertyChangedEventArgs e) {
            var handler = DescendantChanged;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void OnBoundsChanged(
            Rectangle oldBounds, Rectangle newBounds, IEnumerable<IFigure> movingFigures
        ) {
            /// EndDirty()時のAdjustRootFigureBounds()で無限ループになるのでBeginDirty()しない
            ///using (DirtManager.BeginDirty()) {
            var e = new BoundsChangedEventArgs(oldBounds, newBounds, movingFigures);

            if (_ChildrenFollowOnBoundsChanged && e.IsMove) {
                if (_structure.HasChildren) {
                    foreach (var child in Children) {
                        child.Move(e.LocationDelta, movingFigures);
                    }
                }
            }

            InvalidatePaint(oldBounds);
            //InvalidatePaint(newBounds);
            InvalidatePaint(PaintBounds);

            if (
                Layout != null &&
                (e.IsResize || (e.IsMove && !_ChildrenFollowOnBoundsChanged))
            ) {
                InvalidateLayout();
            }

            OnBoundsChangedAfter(oldBounds, newBounds, movingFigures);

            var handler = BoundsChanged;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void OnBoundsChangedAfter(
            Rectangle oldBounds, Rectangle newBounds, IEnumerable<IFigure> movingFigures
        ) {
            /// do nothing
        }

        protected virtual void OnVisibleChanged() {
            var handler = VisibleChanged;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
            //InvalidateLayout();
            InvalidatePaint();
        }

        protected virtual void OnLayoutDone() {
            var handler = LayoutDone;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }
    
        protected virtual void OnShortcutKeyProcess(ShortcutKeyProcessEventArgs e) {
            var handler = ShortcutKeyProcess;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void OnKeyDown(KeyEventArgs e) {
            var handler = KeyDown;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void OnKeyUp(KeyEventArgs e) {
            var handler = KeyUp;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void OnKeyPress(KeyPressEventArgs e) {
            var handler = KeyPress;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void OnPreviewKeyDown(PreviewKeyDownEventArgs e) {
            var handler = PreviewKeyDown;
            if (handler != null) {
                handler(this, e);
            }
        }

        // ------------------------------
        // internal
        // ------------------------------
        /// <summary>
        /// 自分のResourceCacheを解放する．
        /// </summary>
        protected internal virtual void DisposeResourceCache() {
            if (_resourceCache != null) {
                _ResourceCache.DisposeResources();
            }
        }

        /// <summary>
        /// 配下のすべてのfigureのResourceCacheを開放する．
        /// </summary>
        internal void DisposeResourceCacheAll() {
            Accept(
                (fig) => {
                    var abstfig = fig as AbstractFigure;
                    if (abstfig != null) {
                        abstfig.DisposeResourceCache();
                    }
                    return false;
                }
            );
        }

    }
}
