/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Common.Core;
using Mkamo.Common.Externalize;
using Mkamo.Common.Disposable;
using Mkamo.Common.DataType;
using Mkamo.Figure.Core;
using Mkamo.Common.Forms.Drawing;
using Mkamo.Common.Diagnostics;

namespace Mkamo.Figure.Figures {
    public abstract class AbstractPathBoundingNode: AbstractNode, IRotatable, IFlippable {
        // ========================================
        // static field
        // ========================================
        protected static readonly string PathResourceKey = "AbstractPathBoundingNode.Path";
        protected static readonly string DrawPathResourceKey = "AbstractPathBoundingNode.DrawPath";
        protected static readonly string OutlinePathResourceKey = "AbstractPathBoundingNode.OutlinePath";

        private const int OutlineHitMargin = 6;

        // ========================================
        // field
        // ========================================
        private GraphicsPathDescription _pathDesc;

        private GraphicsPathDescription _drawPathDesc;

        private PathBoundingNodeOuterFrame _outerFrame; /// lazy load

        private float _angle;
        private bool _isFlippedHorizontal;
        private bool _isFlippedVertical;

        // ========================================
        // constructor
        // ========================================
        protected AbstractPathBoundingNode(): base() {
            _pathDesc = null;
            _drawPathDesc = null;
            _angle = 0;
            _isFlippedHorizontal = false;
            _isFlippedVertical = false;

            _ResourceCache.RegisterResourceCreator(
                PathResourceKey,
                () => {
                    if (_pathDesc == null) {
                        _pathDesc = CreatePath(GetBoundsForPathCreation());
                    }
                    var ret = _pathDesc.CreateGraphicsPath();

                    var matrix = CreateMatrix();
                    if (matrix != null) {
                        ret.Transform(matrix);
                        matrix.Dispose();
                    }

                    return ret;
                },
                ResourceDisposingPolicy.Explicit
            );

            _ResourceCache.RegisterResourceCreator(
                DrawPathResourceKey,
                () => {
                    if (_drawPathDesc == null) {
                        _drawPathDesc = CreateDrawPath(GetBoundsForPathCreation());
                    }
                    var ret = _drawPathDesc.CreateGraphicsPath();

                    var matrix = CreateMatrix();
                    if (matrix != null) {
                        ret.Transform(matrix);
                        matrix.Dispose();
                    }

                    return ret;
                },
                ResourceDisposingPolicy.Explicit
            );

            _ResourceCache.RegisterResourceCreator(
                OutlinePathResourceKey,
                () => {
                    if (_pathDesc == null) {
                        _pathDesc = CreatePath(GetBoundsForPathCreation());
                    }
                    var ret = _pathDesc.CreateGraphicsPath();

                    var matrix = CreateMatrix();
                    if (matrix != null) {
                        ret.Transform(matrix);
                        matrix.Dispose();
                    }
                    ret.Flatten(null, 0.50f);
                    return ret;
                },
                ResourceDisposingPolicy.Explicit
            );
            
        }

        // ========================================
        // property
        // ========================================
        public override INodeOuterFrame OuterFrame {
            get {
                if (_outerFrame == null) {
                    _outerFrame = new PathBoundingNodeOuterFrame(this);
                }
                return _outerFrame;
            }
        }

        public float Angle {
            get { return _angle; }
        }

        public bool IsFlippedHorizontal {
            get { return _isFlippedHorizontal; }
        }

        public bool IsFlippedVertical {
            get { return _isFlippedVertical; }
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected virtual bool _UseDrawPath {
            get { return false; }
        }

        protected GraphicsPathDescription _PathDescription {
            get { return _pathDesc; }
            set {
                if (value == _pathDesc) {
                    return;
                }
                _pathDesc = value;
                _ResourceCache.DisposeResource(PathResourceKey);
                _ResourceCache.DisposeResource(OutlinePathResourceKey);
            }
        }

        protected GraphicsPathDescription _DrawPathDescription {
            get { return _drawPathDesc; }
            set {
                if (value == _drawPathDesc) {
                    return;
                }
                _drawPathDesc = value;
                _ResourceCache.DisposeResource(DrawPathResourceKey);
            }
        }

        protected GraphicsPath _PathResouce {
            get { return (GraphicsPath) _ResourceCache.GetResource(PathResourceKey); }
        }

        protected GraphicsPath _DrawPathResouce {
            get { return (GraphicsPath) _ResourceCache.GetResource(DrawPathResourceKey); }
        }

        protected GraphicsPath _OutlinePathResouce {
            get { return (GraphicsPath) _ResourceCache.GetResource(OutlinePathResourceKey); }
        }

        // ========================================
        // method
        // ========================================
        // === IPersistable ==========
        public override void WriteExternal(IMemento memento, ExternalizeContext context) {
            base.WriteExternal(memento, context);
            memento.WriteFloat("Angle", _angle);
            memento.WriteBool("IsFlippedHorizontal", _isFlippedHorizontal);
            memento.WriteBool("IsFlippedVertical", _isFlippedVertical);
        }

        public override void ReadExternal(IMemento memento, ExternalizeContext context) {
            base.ReadExternal(memento, context);
            _angle = memento.ReadFloat("Angle");
            _isFlippedHorizontal = memento.ReadBool("IsFlippedHorizontal");
            _isFlippedVertical = memento.ReadBool("IsFlippedVertical");
        }

        // === IFiguer ==========
        public override bool ContainsPoint(Point pt) {
            if (IsBackgroundEnabled) {
                if (!Bounds.Contains(pt)) {
                    return false;
                }

                using (_ResourceCache.UseResource()) {
                    return _OutlinePathResouce.IsVisible(pt);
                }
            } else {
                using (_ResourceCache.UseResource())
                using (var pen = new Pen(Color.Black, BorderWidth + OutlineHitMargin)) {
                    return _OutlinePathResouce.IsOutlineVisible(pt, pen);
                }
            }
        }

        public override bool IntersectsWith(Rectangle rect) {
            return Bounds.IntersectsWith(rect);
        }

        protected override void PaintSelf(Graphics g) {
            using (_ResourceCache.UseResource()) {
                var brush = _BrushResource;
                if (IsBackgroundEnabled && brush != null) {
                    g.FillPath(brush, _PathResouce);
                }
                if (IsForegroundEnabled && BorderWidth > 0) {
                    if (_UseDrawPath) {
                        g.DrawPath(_PenResource, _DrawPathResouce);
                    } else {
                        g.DrawPath(_PenResource, _PathResouce);
                    }
                }
                PaintText(g);
                PaintSelection(g);
                PaintStyledText(g);
            }
        }

        // === AbstractPathBoundingNode ==========
        public void Rotate(float rot) {
            Contract.Requires(rot == 90 || rot == 180 || rot == 270);

            if (_isFlippedHorizontal ^ _isFlippedVertical) {
                if (rot == 90) {
                    rot = 270;
                } else if (rot == 270) {
                    rot = 90;
                }
            }

            _angle = (_angle + rot) % 360;

            if (rot == 90 || rot == 270) {
                var bounds = _Bounds;
                var center = RectUtil.GetCenterF(bounds);
                _Bounds = new Rectangle(
                    (int) Math.Round(center.X - (float) bounds.Height / 2),
                    (int) Math.Round(center.Y - (float) bounds.Width / 2),
                    bounds.Height,
                    bounds.Width
                );

            } else if (rot == 180) {
                _ResourceCache.DisposeResource(PathResourceKey);
                _ResourceCache.DisposeResource(DrawPathResourceKey);
                _ResourceCache.DisposeResource(OutlinePathResourceKey);
                InvalidatePaint();
            }
        }

        public void FlipHorizontal() {
            _isFlippedHorizontal = !_isFlippedHorizontal;
            _ResourceCache.DisposeResource(PathResourceKey);
            _ResourceCache.DisposeResource(DrawPathResourceKey);
            _ResourceCache.DisposeResource(OutlinePathResourceKey);
            InvalidatePaint();
        }

        public void FlipVertical() {
            _isFlippedVertical = !_isFlippedVertical;
            _ResourceCache.DisposeResource(PathResourceKey);
            _ResourceCache.DisposeResource(DrawPathResourceKey);
            _ResourceCache.DisposeResource(OutlinePathResourceKey);
            InvalidatePaint();
        }
        

        // ------------------------------
        // protected
        // ------------------------------
        protected abstract GraphicsPathDescription CreatePath(Rectangle bounds);
        protected virtual GraphicsPathDescription CreateDrawPath(Rectangle bounds) {
            return CreatePath(bounds);
        }

        protected override void OnBoundsChanged(
            Rectangle oldBounds, Rectangle newBounds, IEnumerable<IFigure> movingFigures
        ) {
            var r = GetBoundsForPathCreation();
            _PathDescription = CreatePath(r);
            if (_UseDrawPath) {
                _DrawPathDescription = CreateDrawPath(r);
            }
            base.OnBoundsChanged(oldBounds, newBounds, movingFigures);
        }

        protected override Point GetExpectedConnectLocationForConnectedAnchor(
            IAnchor anchor, Rectangle oldBounds, Rectangle newBounds
        ) {
            if (oldBounds == newBounds) {
                return anchor.Location;
            }

            var isMoveOnly = oldBounds.Size == newBounds.Size;
            if (isMoveOnly) {
                var locDelta = newBounds.Location - (Size) oldBounds.Location;
                return anchor.Location + (Size) locDelta;
            }

            var leftDelta = newBounds.Left - oldBounds.Left;
            var topDelta = newBounds.Top - oldBounds.Top;
            var rightDelta = newBounds.Right - oldBounds.Right;
            var bottomDelta = newBounds.Bottom - oldBounds.Bottom;

            var dir = RectUtil.GetDirectionFromCenter(oldBounds, anchor.Location);
            //var dir = RectUtil.GetDirectionStraightlyFromCenter(oldBounds, anchor.Location);
            var newCenter = RectUtil.GetCenter(newBounds);
            var newLoc = anchor.Location;
            if (EnumUtil.HasAllFlags((int) dir, (int) Directions.Left)) {
                newLoc.X = Math.Min(newLoc.X + leftDelta, newCenter.X);
            }
            if (EnumUtil.HasAllFlags((int) dir, (int) Directions.Right)) {
                newLoc.X = Math.Max(newLoc.X + rightDelta, newCenter.X);
            }
            if (EnumUtil.HasAllFlags((int) dir, (int) Directions.Up)) {
                newLoc.Y = Math.Min(newLoc.Y + topDelta, newCenter.Y);
            }
            if (EnumUtil.HasAllFlags((int) dir, (int) Directions.Down)) {
                newLoc.Y = Math.Max(newLoc.Y + bottomDelta, newCenter.Y);
            }

            using (_ResourceCache.UseResource()) {
                var path = _OutlinePathResouce;
                var pts = path.PathPoints;
                var ptAndDists = new List<Tuple<Point, int>>();
                for (int i = 0, len = pts.Length; i < len; ++i) {
                    var pt1 = Point.Round(pts[i]);
                    var pt2 = Point.Round(i + 1 < len ? pts[i + 1] : pts[0]);
                    if (pt1 != pt2) {
                        var pathPortion = new Line(pt1, pt2);
                        var npt = pathPortion.GetNearestPointFrom(newLoc);
                        var dist = pathPortion.GetDistance(newLoc);
                        ptAndDists.Add(Tuple.Create(npt, dist));
                    }
                }
                return ptAndDists.Count == 0? Point.Empty: ptAndDists.FindMin(t => t.Item2).Item1;
            }
        }


        // ------------------------------
        // private
        // ------------------------------
        private Matrix CreateMatrix() {
            var bounds = _Bounds;
            var center = RectUtil.GetCenterF(bounds);

            var ret = default(Matrix);
            if (_isFlippedHorizontal && _isFlippedVertical) {
                ret = new Matrix(
                    -1, 0, 0, -1, bounds.Left * 2 + bounds.Width - 1, bounds.Top * 2 + bounds.Height - 1
                );
                if (_angle != 0) {
                    ret.RotateAt(_angle, center);
                }

            } else if (_isFlippedHorizontal) {
                ret = new Matrix(-1, 0, 0, 1, bounds.Left * 2 + bounds.Width - 1, 0);
                if (_angle != 0) {
                    ret.RotateAt(_angle, center);
                }

            } else if (_isFlippedVertical) {
                ret = new Matrix(1, 0, 0, -1, 0, bounds.Top * 2 + bounds.Height - 1);
                if (_angle != 0) {
                    ret.RotateAt(_angle, center);
                }
            } else if (_angle != 0) {
                ret = new Matrix();
                ret.RotateAt(_angle, center);
            }
            return ret;
        }

        private Rectangle GetBoundsForPathCreation() {
            var bounds = _Bounds;

            var ret = Rectangle.Empty;
            if (_angle == 0) {
                ret = new Rectangle(bounds.Left, bounds.Top, bounds.Width - 1, bounds.Height - 1);

            } else if (_angle == 90) {
                var center = RectUtil.GetCenterF(bounds);
                ret = new Rectangle(
                    (int) Math.Round(center.X - (float) bounds.Height / 2),
                    (int) Math.Round(center.Y - (float) bounds.Width / 2) + 1,
                    bounds.Height - 1,
                    bounds.Width - 1
                );

            } else if (_angle == 180) {
                ret = new Rectangle(bounds.Left + 1, bounds.Top + 1, bounds.Width - 1, bounds.Height - 1);

            } else if (_angle == 270) {
                var center = RectUtil.GetCenterF(bounds);
                ret = new Rectangle(
                    (int) Math.Round(center.X - (float) bounds.Height / 2) + 1,
                    (int) Math.Round(center.Y - (float) bounds.Width / 2),
                    bounds.Height - 1,
                    bounds.Width - 1
                );
            }

            return ret;
        }

        // ========================================
        // class
        // ========================================
        private class PathBoundingNodeOuterFrame: INodeOuterFrame {
            // ========================================
            // field
            // ========================================
            private AbstractPathBoundingNode _owner;

            // ========================================
            // constructor
            // ========================================
            public PathBoundingNodeOuterFrame(AbstractPathBoundingNode owner) {
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
                using (_owner._ResourceCache.UseResource()) {
                    var path = _owner._OutlinePathResouce;
                    var pts = path.PathPoints;
                    for (int i = 0, len = pts.Length; i < len; ++i) {
                        var pt1 = Point.Round(pts[i]);
                        var pt2 = Point.Round(i + 1 < len ? pts[i + 1] : pts[0]);
                        if (pt1 != pt2) {
                            var l = new Line(pt1, pt2);
                            if (line.IntersectsWith(l)) {
                                return true;
                            }
                        }
                    }
                    return false;
                }
            }

            /// <summary>
            /// 外枠との交点を求める．
            /// 複数の交点がある場合，line.Startに最も近い点を返す．
            /// </summary>
            public virtual Point GetIntersectionPoint(Line line) {
                using (_owner._ResourceCache.UseResource()) {
                    var path = _owner._OutlinePathResouce;
                    var ipts = new List<Point>();
                    var pts = path.PathPoints;
                    for (int i = 0, len = pts.Length; i < len; ++i) {
                        var pt1 = Point.Round(pts[i]);
                        var pt2 = Point.Round(i + 1 < len? pts[i + 1]: pts[0]);
                        if (pt1 != pt2) {
                            var l = new Line(pt1, pt2);
                            if (line.IntersectsWith(l)) {
                                ipts.Add(line.GetIntersectionPoint(l));
                            }
                        }
                    }
                    return ipts.Count == 0? Point.Empty: ipts.FindMin(pt => (int) PointUtil.GetDistance(line.Start, pt));
                }
            }
    
            /// <summary>
            /// ptに最も近い外枠上点を返す．
            /// </summary>
            public virtual Point GetNearestPoint(Point pt) {
                using (_owner._ResourceCache.UseResource()) {
                    var path = _owner._OutlinePathResouce;
                    var lineAndDists = new List<Tuple<Line, int>>();
                    var pts = path.PathPoints;
                    for (int i = 0, len = pts.Length; i < len; ++i) {
                        var pt1 = Point.Round(pts[i]);
                        var pt2 = Point.Round(i + 1 < len? pts[i + 1]: pts[0]);
                       if (pt1 != pt2) {
                            var l = new Line(pt1, pt2);
                            lineAndDists.Add(Tuple.Create(l, l.GetDistance(pt)));
                        }
                    }
                    if (lineAndDists.Count == 0) {
                        return Point.Empty;
                    } else {
                        var line = lineAndDists.FindMin(t => t.Item2).Item1;
                        return line.GetNearestPointFrom(pt);
                    }
                }
            }

        }
    }
}
