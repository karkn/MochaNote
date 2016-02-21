/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Collection;
using System.Drawing;
using Mkamo.Figure.Core;
using Mkamo.Common.Event;
using Mkamo.Common.Externalize;
using Mkamo.Common.Forms.Drawing;
using Mkamo.Common.Disposable;
using System.Drawing.Drawing2D;
using Mkamo.Common.Diagnostics;
using Mkamo.Figure.Utils;
using Mkamo.Common.Win32.Gdi32;
using Mkamo.Common.Core;

namespace Mkamo.Figure.Figures {
    public abstract class AbstractEdge: AbstractConnection, IEdge {
        // ========================================
        // static field
        // ========================================
        protected static readonly string PenResourceKey = "AbstractEdge.Pen";


        // ========================================
        // field
        // ========================================
        private int _hitMargin = 4;
        private Color _lineColor = SystemColors.WindowText;
        private int _lineWidth = 1;
        private DashStyle _lineDashStyle = DashStyle.Solid;
        private ConnectionMethodKind _connectionMethod = ConnectionMethodKind.Intersect;

        private bool _isInEdgePointsMoving = false;

        private IRouter _router;

        private NotifyChangeList<Point> _edgePoints;
        private List<EdgePointRef> _edgePointRefs;

        private EdgeAnchor _sourceAnchor;
        private EdgeAnchor _targetAnchor;

        private object _sourceConnectionOption;
        private object _targetConnectionOption;

        private EdgeBehaviorOptions _behaviorOptions = new EdgeBehaviorOptions(); /// transient

        // ========================================
        // constructor
        // ========================================
        public AbstractEdge(): base() {
            _edgePoints = new NotifyChangeList<Point>(new List<Point>(2));
            _edgePoints.Add(Point.Empty);
            _edgePoints.Add(Point.Empty);

            _edgePoints.EventSender = this;
            _edgePoints.EventPropertyName = IEdgeProperty.EdgePoints;
            _edgePoints.DetailedPropertyChanged += HandleEdgePointsChanged;

            _ResourceCache.RegisterResourceCreator(
                PenResourceKey,
                () => {
                    var ret = new Pen(_lineColor, _lineWidth);
                    ret.DashStyle = _lineDashStyle;
                    return ret;
                },
                ResourceDisposingPolicy.Immediate
            );
        }

        // ========================================
        // event
        // ========================================
        public event EventHandler<DetailedPropertyChangedEventArgs> EdgePointsChanged;

        // ========================================
        // property
        // ========================================
        // === IEdge ===
        public IEnumerable<Point> EdgePoints {
            get { return _edgePoints; }
        }

        public int EdgePointCount {
            get { return _edgePoints.Count; }
        }

        public Point this[int index] {
            get { return _edgePoints[index]; }
            set {
                if (value == _edgePoints[index]) {
                    return;
                }
                _edgePoints[index] = value;
            }
        }

        public virtual IEnumerable<Point> BendPoints {
            get {
                for (int i = 1, len = _edgePoints.Count; i < len - 1; ++i) {
                    yield return _edgePoints[i];
                }
            }
        }

        public virtual IEnumerable<EdgePointRef> EdgePointRefs {
            get {
                if (_edgePointRefs == null) {
                    InitEdgePointRefs();
                }
                return _edgePointRefs;
            }
        }

        public virtual Point First {
            get { return this[0]; }
            set { this[0] = value; }
        }

        public virtual Point Last {
            get { return this[EdgePointCount - 1]; }
            set { this[EdgePointCount - 1] = value; }
        }

        public virtual EdgePointRef FirstRef {
            get {
                if (EdgePointCount < 1) {
                    throw new InvalidOperationException("EdgePoints.Count must be more than zero.");
                }
                return EdgePointRefs.First();
            }
        }
        public virtual EdgePointRef LastRef {
            get {
                if (EdgePointCount < 1) {
                    throw new InvalidOperationException("EdgePoints.Count must be more than zero.");
                }
                return EdgePointRefs.Last();
            }
        }


        public virtual int HitMargin {
            get { return _hitMargin; }
            set { _hitMargin = value; }
        }

        public virtual Color LineColor {
            get { return _lineColor; }
            set {
                if (value == _lineColor) {
                    return;
                }
                _lineColor = value;
                _ResourceCache.DisposeResource(PenResourceKey);
                InvalidatePaint();
            }
        }

        public virtual int LineWidth {
            get { return _lineWidth; }
            set {
                if (value == _lineWidth) {
                    return;
                }
                _lineWidth = Math.Min(value, 8); // 8 == DirtManager.REPAIR_MARGIN * 2
                _ResourceCache.DisposeResource(PenResourceKey);
                InvalidatePaint();
            }
        }

        public virtual DashStyle LineDashStyle {
            get { return _lineDashStyle; }
            set {
                if (value == _lineDashStyle) {
                    return;
                }
                _lineDashStyle = value;
                _ResourceCache.DisposeResource(PenResourceKey);
                InvalidatePaint();
            }
        }

        public IRouter Router {
            get { return _router; }
            set {
                if (value == _router) {
                    return;
                }
                _router = value;
                
                if (_router != null && _edgePoints.Count > 1) {
                    using (DirtManager.BeginDirty()) {
                        _router.Route(this);
                    }
                }
            }
        }

        public ConnectionMethodKind ConnectionMethod {
            get { return _connectionMethod; }
            set {
                if (value == _connectionMethod) {
                    return;
                }
                _connectionMethod = value;
            }
        }

        public object SourceConnectionOption {
            get { return _sourceConnectionOption; }
            set {
                if (value == _sourceConnectionOption) {
                    return ;
                }
                _sourceConnectionOption = value;
            }
        }

        public object TargetConnectionOption {
            get { return _targetConnectionOption; }
            set {
                if (value == _targetConnectionOption) {
                    return ;
                }
                _targetConnectionOption = value;
            }
        }

        public EdgeBehaviorOptions EdgeBehaviorOptions {
            get { return _behaviorOptions; }
            set { _behaviorOptions = value; }
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected NotifyChangeList<Point> _EdgePoints {
            get { return _edgePoints; }
        }

        protected bool IsValidEdge {
            get { return _edgePoints != null && _edgePoints.Count > 1; }
        }

        protected Pen _PenResource {
            get { return (Pen) _ResourceCache.GetResource(PenResourceKey); }
        }

        protected override Point _SourcePoint {
            get { return this[0]; }
            set { this[0] = value; }
        }

        protected override Point _TargetPoint {
            get { return this[_EdgePoints.Count - 1]; }
            set { this[_EdgePoints.Count - 1] = value; }
        }

        protected override AbstractConnection.ConnectionAnchor _SourceAnchor {
            get {
                if (_sourceAnchor == null) {
                    _sourceAnchor = new EdgeAnchor(this, ConnectionAnchorKind.Source);
                }
                return _sourceAnchor;
            }
        }

        protected override AbstractConnection.ConnectionAnchor _TargetAnchor {
            get {
                if (_targetAnchor == null) {
                    _targetAnchor = new EdgeAnchor(this, ConnectionAnchorKind.Target);
                }
                return _targetAnchor;
            }
        }

        // ========================================
        // method
        // ========================================
        // === IPersistable ==========
        public override void WriteExternal(IMemento memento, ExternalizeContext context) {
            base.WriteExternal(memento, context);

            memento.WriteExternalizable("Router", _router);

            memento.WriteSerializable("EdgePoints", _edgePoints.ToArray());
            memento.WriteInt("HitMargin", _hitMargin);
            memento.WriteSerializable("LineColor", _lineColor);
            memento.WriteInt("LineWidth", _lineWidth);
            memento.WriteSerializable("DashStyle", _lineDashStyle);
            memento.WriteSerializable("ConnectionMethod", _connectionMethod);

            memento.WriteSerializable("SourceConnectionOption", _sourceConnectionOption);
            memento.WriteSerializable("TargetConnectionOption", _targetConnectionOption);

            memento.WriteSerializable("EdgeBehaviorOptions", _behaviorOptions);
        }

        public override void ReadExternal(IMemento memento, ExternalizeContext context) {
            base.ReadExternal(memento, context);

            _router = (IRouter) memento.ReadExternalizable("Router");

            var edgePoints = (Point[]) memento.ReadSerializable("EdgePoints");
            _edgePoints.Clear();
            for (int i = 0, len = edgePoints.Length; i < len; ++i) {
                _edgePoints.Add(edgePoints[i]);
            }
            _hitMargin = memento.ReadInt("HitMargin");
            _lineColor = (Color) memento.ReadSerializable("LineColor");
            _lineWidth = memento.ReadInt("LineWidth");
            _lineDashStyle = (DashStyle) memento.ReadSerializable("DashStyle");
            _connectionMethod = (ConnectionMethodKind) memento.ReadSerializable("ConnectionMethod");

            _sourceConnectionOption = memento.ReadSerializable("SourceConnectionOption");
            _targetConnectionOption = memento.ReadSerializable("TargetConnectionOption");

            {
                var opt = memento.ReadSerializable("EdgeBehaviorOptions") as EdgeBehaviorOptions;
                if (opt == null) {
                    _behaviorOptions = new EdgeBehaviorOptions();
                } else {
                    _behaviorOptions = opt;
                }
            }
        }

        // === IFigure ===
        public override bool ContainsPoint(Point pt) {
            if (!IsValidEdge) {
                return false;
            }

            for (int i = 0; i < _edgePoints.Count - 1; ++i) {
                if (
                    Vector2D.GetDistanceLineSegAndPoint(
                        (Vector2D) _edgePoints[i], (Vector2D) _edgePoints[i + 1], (Vector2D) pt
                    ) < _hitMargin
                ) {
                    return true;
                }
            }
            return false;
        }


        public override bool IntersectsWith(Rectangle rect) {
            if (!IsValidEdge) {
                return false;
            }

            for (int i = 0; i < _edgePoints.Count - 1; ++i) {
                if (RectUtil.IntersectsWith(rect, _edgePoints[i], _edgePoints[i + 1])) {
                    return true;
                }
            }
            return false;
        }

        public override void Move(Size delta, IEnumerable<IFigure> movingFigures) {
            if (delta == Size.Empty) {
                return;
            }

            _isInEdgePointsMoving = true;
            try {
                MoveEdgePoints(delta);
            } finally {
                _isInEdgePointsMoving = false;
                InvalidateLayout();
                InvalidatePaint();
                base.Move(delta, movingFigures);
            }
        }

        public override void MakeTransparent(float ratio) {
            LineColor = Color.FromArgb((int) (LineColor.A * ratio), LineColor);
        }


        // === AbstractWrappingFigure ==========
        protected override Rectangle? CalcSelfBounds() {
            return PointUtil.CircumscribeRect(_edgePoints);
        }

        // === AbstractConnection ==========
        protected override void OnConnectableChanged(ConnectableChangedEventArgs e) {
            base.OnConnectableChanged(e);

            if (e.OldValue != null) {
                e.OldValue.BoundsChanged -= HandleConnectableBoundsChanged;

                var oldNode = e.OldValue as INode;
                if (oldNode != null) {
                    oldNode.MaxSizeChanged -= HandleNodeMaxSizeChanged;
                }
            }
            if (e.NewValue != null) {
                e.NewValue.BoundsChanged += HandleConnectableBoundsChanged;

                var newNode = e.NewValue as INode;
                if (newNode != null) {
                    newNode.MaxSizeChanged += HandleNodeMaxSizeChanged;
                }
            }

            if (
                _router == null &&
                e.NewValue != null &&
                (
                    (e.Kind == ConnectionAnchorKind.Source && e.NewValue == Target) ||
                    (e.Kind == ConnectionAnchorKind.Target && e.NewValue == Source)
                )
            ) {
                SetEdgePoints(EdgeUtil.GetLoopPoints(Source.Bounds));
            }

            if (_router != null && IsValidEdge) {
                using (DirtManager.BeginDirty()) {
                    _router.Route(this);
                }
            }
        }
        
        // === AbstractEdge ==========
        public virtual void AddBendPoint(Point pt) {
            _edgePoints.Insert(_edgePoints.Count - 1, pt);
        }

        public virtual void InsertBendPoint(int index, Point pt) {
            Contract.Requires(index > 0 && index < EdgePointCount);
            _edgePoints.Insert(index, pt);
        }

        public virtual void RemoveBendPoint(int index) {
            Contract.Requires(index > 0 && index < EdgePointCount);
            _edgePoints.RemoveAt(index);
        }

        public virtual void ClearBendPoints() {
            for (int i = 1, len = _edgePoints.Count; i < len - 1; ++i) {
                _edgePoints.RemoveAt(1);
            }
        }

        public virtual void SetEdgePoints(IEnumerable<Point> edgePoints) {
            Contract.Requires(edgePoints != null);

            ClearBendPoints();
            var i = 0;
            var len = edgePoints.Count();
            Contract.Requires(len > 1);
            foreach (var pt in edgePoints) {
                if (i == 0) {
                    First = pt;
                } else if (i == len - 1) {
                    Last = pt;
                } else {
                    AddBendPoint(pt);
                }
                ++i;
            }
        }

        
        public virtual void Route() {
            if (_router != null && IsValidEdge) {
                using (DirtManager.BeginDirty()) {
                    _router.Route(this);
                }
            }
        }

        public Point GetConnectionPoint(IAnchor anchor, INode node, Point location) {
            var outerFrame = node.OuterFrame;

            switch (_connectionMethod) {
                case ConnectionMethodKind.Intersect: {
                    var nextLoc = anchor.Kind == ConnectionAnchorKind.Source?
                        FirstRef.Next.EdgePoint:
                        LastRef.Prev.EdgePoint;
                    var line = new Line(location, nextLoc);
                    if (outerFrame.IntersectsWith(line)) {
                        return outerFrame.GetIntersectionPoint(line);
                    } else {
                        /// 交わってなければ仕方がないのでNearestを返しておく
                        return outerFrame.GetNearestPoint(line.Start);
                    }
                }
                case ConnectionMethodKind.UpperSideMidpoint: {
                    var bounds = node.Bounds;
                    var center = RectUtil.GetCenter(bounds);
                    return anchor.Kind == ConnectionAnchorKind.Source ?
                        outerFrame.GetNearestPoint(new Point(center.X, bounds.Top)):
                        outerFrame.GetNearestPoint(new Point(center.X, bounds.Bottom));
                }
                case ConnectionMethodKind.LowerSideMidpoint: {
                    var bounds = node.Bounds;
                    var center = RectUtil.GetCenter(bounds);
                    return anchor.Kind == ConnectionAnchorKind.Source ?
                        outerFrame.GetNearestPoint(new Point(center.X, bounds.Bottom)):
                        outerFrame.GetNearestPoint(new Point(center.X, bounds.Top));
                }
                case ConnectionMethodKind.LeftSideMidpoint: {
                    var bounds = node.Bounds;
                    var center = RectUtil.GetCenter(bounds);
                    return anchor.Kind == ConnectionAnchorKind.Source ?
                        outerFrame.GetNearestPoint(new Point(bounds.Left, center.Y)):
                        outerFrame.GetNearestPoint(new Point(bounds.Right, center.Y));                    
                }
                case ConnectionMethodKind.RightSideMidpoint: {
                    var bounds = node.Bounds;
                    var center = RectUtil.GetCenter(bounds);
                    return anchor.Kind == ConnectionAnchorKind.Source ?
                        outerFrame.GetNearestPoint(new Point(bounds.Right, center.Y)):
                        outerFrame.GetNearestPoint(new Point(bounds.Left, center.Y));
                }
                case ConnectionMethodKind.SideMidpointOfOpposite: {
                    var oppositeLoc = anchor.Kind == ConnectionAnchorKind.Source?
                        LastRef.EdgePoint:
                        FirstRef.EdgePoint;
                    var bounds = node.Bounds;
                    var dir = RectUtil.GetDirectionStraightlyFromCenter(bounds, oppositeLoc);
                    var center = RectUtil.GetCenter(bounds);
                    if (EnumUtil.HasAllFlags((int) dir, (int) Mkamo.Common.DataType.Directions.Left)) {
                        return outerFrame.GetNearestPoint(new Point(bounds.Left, center.Y));
                    } else if (EnumUtil.HasAllFlags((int) dir, (int) Mkamo.Common.DataType.Directions.Right)) {
                        return outerFrame.GetNearestPoint(new Point(bounds.Right, center.Y));
                    } else if (EnumUtil.HasAllFlags((int) dir, (int) Mkamo.Common.DataType.Directions.Up)) {
                        return outerFrame.GetNearestPoint(new Point(center.X, bounds.Top));
                    } else {
                        /// } else if (EnumUtil.HasFlag((int) dir, (int) Mkamo.Common.DataType.Directions.Left)) {
                        return outerFrame.GetNearestPoint(new Point(center.X, bounds.Bottom));
                    } 
                }
                case ConnectionMethodKind.SideMidpointOfNearest: {
                    //var nearestLoc = outerFrame.GetNearestPoint(location);
                    var bounds = node.Bounds;
                    //var dir = RectUtil.GetDirectionStraightlyFromCenter(bounds, nearestLoc);
                    var dir = RectUtil.GetNearestLineDirection(bounds, location);
                    var center = RectUtil.GetCenter(bounds);
                    if (EnumUtil.HasAllFlags((int) dir, (int) Mkamo.Common.DataType.Directions.Left)) {
                        return outerFrame.GetNearestPoint(new Point(bounds.Left, center.Y));
                    } else if (EnumUtil.HasAllFlags((int) dir, (int) Mkamo.Common.DataType.Directions.Right)) {
                        return outerFrame.GetNearestPoint(new Point(bounds.Right, center.Y));
                    } else if (EnumUtil.HasAllFlags((int) dir, (int) Mkamo.Common.DataType.Directions.Up)) {
                        return outerFrame.GetNearestPoint(new Point(center.X, bounds.Top));
                    } else {
                        /// } else if (EnumUtil.HasFlag((int) dir, (int) Mkamo.Common.DataType.Directions.Left)) {
                        return outerFrame.GetNearestPoint(new Point(center.X, bounds.Bottom));
                    } 
                    
                }
                case ConnectionMethodKind.Nearest:
                    return outerFrame.GetNearestPoint(location);

                case ConnectionMethodKind.Center:
                    // todo: ちゃんと実装
                    // とりあえずこれでもCentralRouterが設定されていればすぐルーティングされるので問題なく動く
                    return outerFrame.GetNearestPoint(location);

                case ConnectionMethodKind.Comment:
                    if (anchor.Kind == ConnectionAnchorKind.Source) {
                        var leftMiddle = new Point(node.Left, node.Top + node.Height / 2);
                        return outerFrame.GetNearestPoint(leftMiddle); 
                        //return outerFrame.GetNearestPoint(location); 
                    } else {
                        var pt = node.GetConnectionPoint(_targetConnectionOption);
                        if (pt.HasValue) {
                            return pt.Value;
                        } else {
                            return outerFrame.GetNearestPoint(location); 
                        }
                    }

                default:
                    return outerFrame.GetNearestPoint(location);
            }
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected virtual void MoveEdgePoints(Size delta) {
            for (int i = 0; i < _EdgePoints.Count; ++i) {
                if (i == 0) {
                    if (!IsSourceConnected) {
                        First += delta;
                    }
                } else if (i == _EdgePoints.Count - 1) {
                    if (!IsTargetConnected) {
                        Last += delta;
                    }
                } else {
                    _EdgePoints[i] += delta;
                }
            }
        }


        // --- edge point refs ---
        protected virtual void InitEdgePointRefs() {
            _edgePointRefs = new List<EdgePointRef>();
            for (int i = 0; i < EdgePointCount; ++i) {
                _edgePointRefs.Add(new EdgePointRef(this, i));
            }
        }

        protected virtual void UpdateEdgePointRefs(DetailedPropertyChangedEventArgs e) {
            if (_edgePointRefs != null && e != null && e.IsIndexed) {
                int changedIndex = e.Index;
                switch (e.Kind) {
                    case PropertyChangeKind.Add: {
                        _edgePointRefs.Insert(changedIndex, new EdgePointRef(this, changedIndex));
                        for (int i = changedIndex + 1; i < _edgePointRefs.Count; ++i) {
                            ++_edgePointRefs[i].Index;
                        }
                        break;
                    }
                    case PropertyChangeKind.Remove: {
                        _edgePointRefs.RemoveAt(changedIndex);
                        for (int i = changedIndex; i < _edgePointRefs.Count; ++i) {
                            --_edgePointRefs[i].Index;
                        }
                        break;
                    }
                    case PropertyChangeKind.Clear: {
                        _edgePointRefs = null;
                        break;
                    }
                    case PropertyChangeKind.Set: {
                        /// do nothing
                        break;
                    }
                    default: {
                        throw new ArgumentException();
                    }
                }
            }
        }

        // --- event ---
        protected virtual void OnConnectableBoundsChanged(BoundsChangedEventArgs e) {
            if (_router != null && IsValidEdge) {
                if (e.MovingFigures == null || !e.MovingFigures.Contains(this)) {
                    using (DirtManager.BeginDirty()) {
                        _router.Route(this);
                    }
                }
            }
        }

        protected virtual void OnNodeMaxSizeChanged() {
            if (_router != null && IsValidEdge) {
                if (_behaviorOptions.RouteOnNodeMaxSizeChanged) {
                    using (DirtManager.BeginDirty()) {
                        _router.Route(this);
                    }
                }
            }
        }

        protected virtual void OnEdgePointsChanged(DetailedPropertyChangedEventArgs e) {
            if (e.Kind == PropertyChangeKind.Set) {
                /// EdgePointsがAddされたりRemoveされたときにはfireしない
                if (e.Index == 0) {
                    _SourceAnchor.OnAnchorMoved((Point) e.OldValue, (Point) e.NewValue);
                } else if (e.Index == EdgePointCount - 1) {
                    _TargetAnchor.OnAnchorMoved((Point) e.OldValue, (Point) e.NewValue);
                }
            }

            if (!_isInEdgePointsMoving) {
                UpdateEdgePointRefs(e);
                DirtyBoundsCache(null);
                InvalidateLayout();
                InvalidatePaint();
            }

            var handler = EdgePointsChanged;
            if (handler != null) {
                handler(this, e);
            }
        }

        // ------------------------------
        // private
        // ------------------------------
        private void HandleEdgePointsChanged(object sender, DetailedPropertyChangedEventArgs e) {
            OnEdgePointsChanged(e);
        }

        private void HandleConnectableBoundsChanged(object sender, BoundsChangedEventArgs e) {
            OnConnectableBoundsChanged(e);
        }

        private void HandleNodeMaxSizeChanged(object sender, EventArgs e) {
            OnNodeMaxSizeChanged();
        }


        // ========================================
        // class
        // ========================================
        [Serializable]
        protected class EdgeAnchor: ConnectionAnchor {
            [NonSerialized]
            private AbstractEdge _owner;

            public EdgeAnchor(AbstractEdge owner, ConnectionAnchorKind kind): base(owner, kind) {
                _owner = owner;
            }

            public override Point Location {
                get { return base.Location; }
                set {
                    if (value == Location) {
                        return;
                    }
                    base.Location = value;
                    _owner.Route();
                }
            }
        }
    }
}
