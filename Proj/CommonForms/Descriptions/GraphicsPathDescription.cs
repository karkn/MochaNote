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
using Mkamo.Common.Forms.Drawing;

namespace Mkamo.Common.Forms.Descriptions {
    [Serializable]
    public class GraphicsPathDescription: ICloneable {
        // ========================================
        // field
        // ========================================
        private List<IPathElement> _pathElements;
        private FillMode _fillMode;

        // ========================================
        // constructor
        // ========================================
        public GraphicsPathDescription() {
            _pathElements = new List<IPathElement>();
            _fillMode = FillMode.Alternate;
        }

        // ========================================
        // property
        // ========================================
        public IList<IPathElement> PathElements {
            get { return _pathElements; }
        }

        public FillMode FillMode {
            get { return _fillMode; }
            set { _fillMode = value; }
        }

        // ========================================
        // method
        // ========================================
        public GraphicsPath CreateGraphicsPath() {
            return CreateGraphicsPath(true);
        }

        public GraphicsPath CreateGraphicsPath(bool close) {
            var path = new GraphicsPath(_fillMode);
            path.StartFigure();

            IPathElement elem = null;
            for (int i = 0; i < _pathElements.Count; ++i) {
                elem = _pathElements[i];
                switch (elem.Kind) {
                    case PathElementKind.Arc: {
                        var arc = elem as ArcPathElement;
                        path.AddArc(arc.Rect, arc.StartAngle, arc.SweepAngle);
                        break;
                    }
                    case PathElementKind.Bezier: {
                        var bezier = elem as BezierPathElement;
                        path.AddBezier(bezier.Point1, bezier.Point2, bezier.Point3, bezier.Point4);
                        break;
                    }
                    case PathElementKind.Curve: {
                        var curve = elem as CurvePathElement;
                        if (curve.Tension.HasValue) {
                            path.AddCurve(curve.Points, curve.Tension.Value);
                        } else {
                            path.AddCurve(curve.Points);
                        }
                        break;
                    }
                    case PathElementKind.Ellipse: {
                        var ellipse = elem as EllipsePathElement;
                        path.AddEllipse(ellipse.Rect);
                        break;
                    }
                    case PathElementKind.Line: {
                        var line = elem as LinePathElement;
                        path.AddLine(line.Point1, line.Point2);
                        break;
                    }
                    case PathElementKind.Pie: {
                        var pie = elem as PiePathElement;
                        path.AddPie(pie.Rect, pie.StartAngle, pie.SweepAngle);
                        break;
                    }
                    case PathElementKind.Polygon: {
                        var polygon = elem as PolygonPathElement;
                        path.AddPolygon(polygon.Points);
                        break;
                    }
                    case PathElementKind.Rectangle: {
                        var rectangle = elem as RectanglePathElement;
                        path.AddRectangle(rectangle.Rect);
                        break;
                    }
                    default: {
                        throw new InvalidOperationException("Invalid kind of path element");
                    }
                }
            }

            if (close) {
                path.CloseAllFigures();
            }

            return path;
        }

        public void SetAllElementsBounds(Rectangle bounds) {
            foreach (var elem in _pathElements) {
                elem.Bounds = bounds;
            }
        }

        public void Translate(Size offset) {
            foreach (var elem in _pathElements) {
                elem.Translate(offset);
            }
        }

        public void Scale(SizeF scale) {
            foreach (var elem in _pathElements) {
                elem.Scale(scale);
            }
        }


        public void AddArc(Rectangle rect, float startAngle, float sweepAngle) {
            var elem = new ArcPathElement {
                Rect = rect,
                StartAngle = startAngle,
                SweepAngle = sweepAngle
            };
            _pathElements.Add(elem);
        }

        public void AddBezier(Point pt1, Point pt2, Point pt3, Point pt4) {
            var elem = new BezierPathElement {
                Point1 = pt1,
                Point2 = pt2,
                Point3 = pt3,
                Point4 = pt4,
            };
            _pathElements.Add(elem);
        }

        public void AddCurve(Point[] points) {
            var elem = new CurvePathElement {
                Points = points
            };
            _pathElements.Add(elem);
        }

        public void AddCurve(Point[] points, float tension) {
            var elem = new CurvePathElement {
                Points = points,
                Tension = tension,
            };
            _pathElements.Add(elem);
        }

        public void AddEllipse(Rectangle rect) {
            var elem = new EllipsePathElement {
                Rect = rect,
            };
            _pathElements.Add(elem);
        }

        public void AddLine(Point pt1, Point pt2) {
            var elem = new LinePathElement {
                Point1 = pt1,
                Point2 = pt2,
            };
            _pathElements.Add(elem);
        }

        public void AddPie(Rectangle rect, float startAngle, float sweepAngle) {
            var elem = new PiePathElement {
                Rect = rect,
                StartAngle = startAngle,
                SweepAngle = sweepAngle
            };
            _pathElements.Add(elem);
        }

        public void AddPolygon(Point[] points) {
            var elem = new PolygonPathElement {
                Points = points,
            };
            _pathElements.Add(elem);
        }

        public void AddRectangle(Rectangle rect) {
            var elem = new RectanglePathElement {
                Rect = rect,
            };
            _pathElements.Add(elem);
        }

        public object Clone() {
            var ret = new GraphicsPathDescription();
            ret._fillMode = _fillMode;
            ret._pathElements  = new List<IPathElement>(_pathElements.Count);
            for (int i = 0, len = _pathElements.Count; i < len; ++i) {
                ret._pathElements.Add(_pathElements[i].Clone() as IPathElement);
            }
            return ret;
        }
    }

    [Serializable]
    public enum PathElementKind {
        Arc,
        Bezier,
        Curve,
        Ellipse,
        Line,
        Pie,
        Polygon,
        Rectangle,
    }

    public interface IPathElement: ICloneable {
        // ========================================
        // property
        // ========================================
        PathElementKind Kind { get; }
        Rectangle Bounds { get; set; }

        // ========================================
        // method
        // ========================================
        void Translate(Size offset);
        void Scale(SizeF scale);
    }

    [Serializable]
    public abstract class AbstractPathElement: IPathElement {
        public abstract PathElementKind Kind { get; }
        public abstract Rectangle Bounds { get; set; }

        public abstract void Translate(Size offset);
        public abstract void Scale(SizeF scale);

        public virtual object Clone() {
            return MemberwiseClone();
        }
    }

    [Serializable]
    public class ArcPathElement: AbstractPathElement {
        public Rectangle Rect;
        public float StartAngle;
        public float SweepAngle;

        public override PathElementKind Kind {
            get { return PathElementKind.Arc; }
        }

        public override Rectangle Bounds {
            get { return Rect; }
            set { Rect = value; }
        }

        public override void Translate(Size offset) {
            Rect.Location += offset;
        }

        public override void Scale(SizeF scale) {
            Rect.Size = Size.Round(new SizeF(Rect.Width * scale.Width, Rect.Height * scale.Height));
        }
    }

    [Serializable]
    public class BezierPathElement: AbstractPathElement {
	    public Point Point1;
	    public Point Point2;
	    public Point Point3;
	    public Point Point4;

        public override PathElementKind Kind {
            get { return PathElementKind.Bezier; }
        }

        public override Rectangle Bounds {
            get { return PointUtil.CircumscribeRect(new Point[] { Point1, Point2, Point3, Point4, }); }
            set {
                Rectangle bounds = Bounds;
                Scale(
                    new SizeF(
                        (float) value.Width / (float) bounds.Width,
                        (float) value.Height / (float) bounds.Height
                    )
                );
            }
        }

        public override void Translate(Size offset) {
            Point1 += offset;
            Point2 += offset;
            Point3 += offset;
            Point4 += offset;
        }

        public override void Scale(SizeF scale) {
            var pts = new Point[] { Point1, Point2, Point3, Point4, };
            PointUtil.ScaleWithCircumscribeRect(pts, scale);
            Point1 = pts[0];
            Point2 = pts[1];
            Point3 = pts[2];
            Point4 = pts[3];
        }
    }


    [Serializable]
    public class CurvePathElement: AbstractPathElement {
        public Point[] Points;
        public float? Tension;

        public override PathElementKind Kind {
            get { return PathElementKind.Curve; }
        }

        public override Rectangle Bounds {
            get { return PointUtil.CircumscribeRect(Points); }
            set {
                Rectangle bounds = Bounds;
                Scale(
                    new SizeF(
                        (float) value.Width / (float) bounds.Width,
                        (float) value.Height / (float) bounds.Height
                    )
                );
            }
        }

        public override void Translate(Size offset) {
            for (int i = 0, len = Points.Length; i < len; ++i) {
                Points[i] += offset;
            }
        }

        public override void Scale(SizeF scale) {
            PointUtil.ScaleWithCircumscribeRect(Points, scale);
        }

        public override object Clone() {
            var ret = new CurvePathElement();
            ret.Points = new Point[Points.Length];
            Array.Copy(Points, ret.Points, Points.Length);
            ret.Tension = Tension;
            return ret;
        }
    }

    [Serializable]
    public class EllipsePathElement: AbstractPathElement {
        public Rectangle Rect;

        public override PathElementKind Kind {
            get { return PathElementKind.Ellipse; }
        }

        public override Rectangle Bounds {
            get { return Rect; }
            set { Rect = value; }
        }

        public override void Translate(Size offset) {
            Rect.Location += offset;
        }

        public override void Scale(SizeF scale) {
            Rect.Size = Size.Round(new SizeF(Rect.Width * scale.Width, Rect.Height * scale.Height));
        }
    }

    [Serializable]
    public class LinePathElement: AbstractPathElement {
        public Point Point1;
        public Point Point2;

        public override Rectangle Bounds {
            get { return PointUtil.CircumscribeRect(new Point[] { Point1, Point2, }); }
            set {
                Rectangle bounds = Bounds;
                Scale(
                    new SizeF(
                        (float) value.Width / (float) bounds.Width,
                        (float) value.Height / (float) bounds.Height
                    )
                );
            }
        }

        public override PathElementKind Kind {
            get { return PathElementKind.Line; }
        }

        public override void Translate(Size offset) {
            Point1 += offset;
            Point2 += offset;
        }

        public override void Scale(SizeF scale) {
            var pts = new Point[] { Point1, Point2, };
            PointUtil.ScaleWithCircumscribeRect(pts, scale);
            Point1 = pts[0];
            Point2 = pts[1];
        }
    }

    [Serializable]
    public class PiePathElement: AbstractPathElement {
        public Rectangle Rect;
        public float StartAngle;
        public float SweepAngle;

        public override PathElementKind Kind {
            get { return PathElementKind.Pie; }
        }

        public override Rectangle Bounds {
            get { return Rect; }
            set { Rect = value; }
        }

        public override void Translate(Size offset) {
            Rect.Location += offset;
        }

        public override void Scale(SizeF scale) {
            Rect.Size = Size.Round(new SizeF(Rect.Width * scale.Width, Rect.Height * scale.Height));
        }
    }

    [Serializable]
    public class PolygonPathElement: AbstractPathElement {
        public Point[] Points;

        public override PathElementKind Kind {
            get { return PathElementKind.Polygon; }
        }

        public override Rectangle Bounds {
            get { return PointUtil.CircumscribeRect(Points); }
            set {
                Rectangle bounds = Bounds;
                Scale(
                    new SizeF(
                        (float) value.Width / (float) bounds.Width,
                        (float) value.Height / (float) bounds.Height
                    )
                );
            }
        }

        public override void Translate(Size offset) {
            for (int i = 0, len = Points.Length; i < len; ++i) {
                Points[i] += offset;
            }
        }

        public override void Scale(SizeF scale) {
            PointUtil.ScaleWithCircumscribeRect(Points, scale);
        }

        public override object Clone() {
            var ret = new PolygonPathElement();
            ret.Points = new Point[Points.Length];
            Array.Copy(Points, ret.Points, Points.Length);
            return ret;
        }
    }

    [Serializable]
    public class RectanglePathElement: AbstractPathElement {
        public Rectangle Rect;

        public override PathElementKind Kind {
            get { return PathElementKind.Rectangle; }
        }

        public override Rectangle Bounds {
            get { return Rect; }
            set { Rect = value; }
        }

        public override void Translate(Size offset) {
            Rect.Location += offset;
        }

        public override void Scale(SizeF scale) {
            Rect.Size = Size.Round(new SizeF(Rect.Width * scale.Width, Rect.Height * scale.Height));
        }
    }
}
