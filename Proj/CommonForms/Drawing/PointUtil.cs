/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Linq;
using Mkamo.Common.Collection;
using Mkamo.Common.DataType;
using Mkamo.Common.Core;

namespace Mkamo.Common.Forms.Drawing {
    public static class PointUtil {

        /// <summary>
        /// ptのXまたはYが0より小さければ0にしたPointを返す．
        /// </summary>
        public static Point EnsureNotNegative(Point pt) {
            return new Point(Math.Max(pt.X, 0), Math.Max(pt.Y, 0));
        }

        /// <summary>
        /// ptのXまたはYを0より小さくしないdeltaを返す．
        /// </summary>
        public static Size EnsureNonNegativeDelta(Point pt, Size delta) {
            var x = pt.X + delta.Width;
            var y = pt.Y + delta.Height;
            return new Size(
                x < 0? delta.Width - x: delta.Width,
                y < 0? delta.Height - y: delta.Height
            );
        }

        /// <summary>
        /// ptを原点を中心にangle度回転した点を返す．
        /// </summary>
        public static Point RotateAtOrigin(Point pt, Single angle) {
            var d = angle * Math.PI / 180;
            return new Point(
                (int) Math.Round((pt.X * Math.Cos(d) - pt.Y * Math.Sin(d)), MidpointRounding.AwayFromZero),
                (int) Math.Round((pt.X * Math.Sin(d) + pt.Y * Math.Cos(d)), MidpointRounding.AwayFromZero)
            );
        }

        /// <summary>
        /// ptをdelta平行移動した点を返す．
        /// </summary>
        public static Point Translate(Point pt, Size delta) {
            return pt + delta;
        }


        /// <summary>
        /// pt1とpt2の距離を返す．
        /// </summary>
        public static double GetDistance(Point pt1, Point pt2) {
            return Vector2D.GetDistance((Vector2D) pt1, (Vector2D) pt2);
        }

        /// <summary>
        /// pt1に対してpt2がどの方向にあるかを返す．
        /// X軸が同じ場合はLeftかつRight，Y軸が同じ場合はUpかつDownであるものとする．
        /// 戻り値は例えばLeft|Upといった重なった答え。
        /// </summary>
        public static Directions GetDirection(Point pt1, Point pt2) {
            var ret = Directions.None;

            if (pt2.X <= pt1.X) {
                ret |= Directions.Left;
            }
            if (pt2.X >= pt1.X) {
                ret |= Directions.Right;
            }
            if (pt2.Y <= pt1.Y) {
                ret |= Directions.Up;
            }
            if (pt2.Y >= pt1.Y) {
                ret |= Directions.Down;
            }

            return ret;
        }

        /// <summary>
        /// pt1に対してpt2がどの方向にあるかを返す．
        /// 同じ点の場合はNoneを返す。
        /// 戻り値は例えばLeftやUpといった重なりのない答え。
        /// </summary>
        public static Directions GetDirectionStraightly(Point pt1, Point pt2) {
            var dx = Math.Abs(pt2.X - pt1.X);
            var dy = Math.Abs(pt2.Y - pt1.Y);

            if (dx >= dy) {
                if (pt2.X < pt1.X) {
                    return Directions.Left;
                } else if (pt2.X > pt1.X) {
                    return Directions.Right;
                } else {
                    return Directions.None;
                }
            } else {
                /// dx < dy
                if (pt2.Y < pt1.Y) {
                    return Directions.Up;
                } else if (pt2.Y > pt1.Y) {
                    return Directions.Down;
                } else {
                    return Directions.None;
                }
            }
        }

        /// <summary>
        /// pt1とpt2の中点を返す．
        /// </summary>
        public static Point MiddlePoint(Point pt1, Point pt2) {
            Point dist = pt1 - (Size) pt2;
            return new Point(
                pt1.X - MathUtil.RoundDiv(dist.X, 2),
                pt1.Y - MathUtil.RoundDiv(dist.Y, 2)
            );
        }

        /// <summary>
        /// すべてのPointのX座標の値のうち最小のものを返す．
        /// </summary>
        public static int Left(IEnumerable<Point> points) {
            int? left = null;
            foreach (var point in points) {
                left = left.HasValue? Math.Min(left.Value, point.X): point.X;
            }
            return left.HasValue? left.Value: 0;
        }

        /// <summary>
        /// すべてのPointのY座標の値のうち最小のものを返す．
        /// </summary>
        public static int Top(IEnumerable<Point> points) {
            int? top = null;
            foreach (var point in points) {
                top = top.HasValue? Math.Min(top.Value, point.Y): point.Y;
            }
            return top.HasValue? top.Value: 0;
        }

        /// <summary>
        /// すべてのPointのX座標の値のうち最大のものを返す．
        /// </summary>
        public static int Right(IEnumerable<Point> points) {
            int? right = null;
            foreach (var point in points) {
                right = right.HasValue? Math.Max(right.Value, point.X): point.X;
            }
            return right.HasValue? right.Value: 0;
        }

        /// <summary>
        /// すべてのPointのY座標の値のうち最大のものを返す．
        /// </summary>
        public static int Bottom(IEnumerable<Point> points) {
            int? bottom = null;
            foreach (var point in points) {
                bottom = bottom.HasValue? Math.Max(bottom.Value, point.Y): point.Y;
            }
            return bottom.HasValue? bottom.Value: 0;
        }

        /// <summary>
        /// すべてのPointを含む矩形を返す．
        /// </summary>
        public static Rectangle CircumscribeRect(IEnumerable<Point> points) {
            if (!points.Any()) {
                return Rectangle.Empty;
            }

            var first = points.First();
            var left = first.X;
            var top = first.Y;
            var right = first.X;
            var bottom = first.Y;

            foreach (var pt in points) {
                left = Math.Min(left, pt.X);
                top = Math.Min(top, pt.Y);
                right = Math.Max(right, pt.X);
                bottom = Math.Max(bottom, pt.Y);
            }

            return new Rectangle(left, top, right - left, bottom - top);
            //return new Rectangle(left, top, right - left + 1, bottom - top + 1);
        }

        /// <summary>
        /// すべてのpointsを含む矩形をscale倍したpointsを返す．
        /// </summary>
        public static void ScaleWithCircumscribeRect(Point[] points, SizeF scale) {
            if (points.Length == 0) {
                return;
            }

            Rectangle rect = CircumscribeRect(points);
            for (int i = 0, len = points.Length; i < len; ++i) {
                points[i].X = (int) Math.Round(rect.X + (points[i].X - rect.X) * scale.Width);
                points[i].Y = (int) Math.Round(rect.Y + (points[i].Y - rect.Y) * scale.Height);
            }
        }
    }
}
