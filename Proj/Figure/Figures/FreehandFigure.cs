/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Mkamo.Common.Externalize;
using Mkamo.Common.Forms.Drawing;

namespace Mkamo.Figure.Figures {
    public class FreehandFigure: AbstractNode {
        // ========================================
        // field
        // ========================================
        private List<Point> _points;
        private int _hitMargin;

        // ========================================
        // constructor
        // ========================================
        public FreehandFigure() {
            _points = new List<Point>();
            _hitMargin = 4;
        }

        // ========================================
        // property
        // ========================================
        public IEnumerable<Point> Points {
            get { return _points; }
        }

        public virtual int HitMargin {
            get { return _hitMargin; }
            set { _hitMargin = value; }
        }

        // ========================================
        // method
        // ========================================
        public override void WriteExternal(IMemento memento, ExternalizeContext context) {
            base.WriteExternal(memento, context);

            memento.WriteSerializable("Points", _points);
            memento.WriteInt("HitMargin", _hitMargin);
        }

        public override void ReadExternal(IMemento memento, ExternalizeContext context) {
            base.ReadExternal(memento, context);

            _points = memento.ReadSerializable("Points") as List<Point>;
            _hitMargin = memento.ReadInt("HitMargin");
        }

        public void SetPoints(IEnumerable<Point> pts) {
            if (pts == null) {
                return;
            }

            var r = PointUtil.CircumscribeRect(pts);

            _points.Clear();
            foreach (var pt in pts) {
                _points.Add(pt - (Size) r.Location);
            }

            if (_points.Count > 1) {
                Bounds = r;
            } else {
                Bounds = Rectangle.Empty;
            }
            InvalidatePaint();
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected override void PaintSelf(Graphics g) {
            if (_points.Count > 1) {
                using (_ResourceCache.UseResource()) {
                    var pen = _PenResource;
                    pen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                    pen.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                    pen.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;

                    var loc = Location;
                    var pts = new Point[_points.Count];
                    pts[0] = _points[0] + (Size) loc;
                    pts[1] = new Point(
                        (_points[0].X + _points[1].X) / 2 + loc.X,
                        (_points[0].Y + _points[1].Y) / 2 + loc.Y
                    );
                    for (int i = 2, ilen = _points.Count; i < ilen; ++i) {
                        var pt1 = _points[i - 2];
                        var pt2 = _points[i - 1];
                        var pt3 = _points[i];
                        /// 手ぶれ補正 移動平均法
                        var x = (pt1.X + pt2.X + pt3.X) / 3 + loc.X;
                        var y = (pt1.Y + pt2.Y + pt3.Y) / 3 + loc.Y;
                        pts[i] = new Point(x, y);
                    }

                    //var loc = Location;
                    //var pts = new Point[_points.Count];
                    //pts[0] = _points[0] + (Size) loc;
                    //for (int i = 1, ilen = _points.Count; i < ilen; ++i) {
                    //    var pt1 = _points[i - 1];
                    //    var pt2 = _points[i];
                    //    /// 手ぶれ補正 移動平均法
                    //    var x = (pt1.X + pt2.X) / 2 + loc.X;
                    //    var y = (pt1.Y + pt2.Y) / 2 + loc.Y;
                    //    pts[i] = new Point(x, y);
                    //}

                    //g.DrawLines(_PenResource, pts);
                    g.DrawCurve(pen, pts, 0.5f);
                }
            }
        }

        public override bool ContainsPoint(Point pt) {
            if (_points.Count > 1) {
                var translated = pt - (Size) Location;
                for (int i = 0, ilen = _points.Count; i < ilen - 1; ++i) {
                    if (
                        Vector2D.GetDistanceLineSegAndPoint(
                            (Vector2D) _points[i],
                            (Vector2D) _points[i + 1],
                            (Vector2D) translated
                        ) < _hitMargin
                    ) {
                        return true;
                    }
                }
            }

            return false;
        }

        public override bool IntersectsWith(Rectangle rect) {
            if (_points.Count > 1) {
                var translated = new Rectangle(
                    rect.Location - (Size) Location,
                    rect.Size
                );
                for (int i = 0; i < _points.Count - 1; ++i) {
                    if (RectUtil.IntersectsWith(translated, _points[i], _points[i + 1])) {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
