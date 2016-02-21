/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Linq;
using Mkamo.Common.DataType;
using Mkamo.Common.Diagnostics;
using Mkamo.Common.Core;

namespace Mkamo.Common.Forms.Drawing {

    public static class RectUtil {
        public static Rectangle GetArroundingRectangle(Point center, Size arroundingSize) {
            return new Rectangle(
                center.X - arroundingSize.Width,
                center.Y - arroundingSize.Height,
                arroundingSize.Width * 2 + 1,
                arroundingSize.Height * 2 + 1
            );
        }


        public static Point GetPreferredLocation(Rectangle newRect, IEnumerable<Rectangle> exists, int rightMax, int leftMargin, int topMargin) {
            var intersects = false;
            var intersected = Rectangle.Empty;
            foreach (var exist in exists) {
                if (exist.IntersectsWith(newRect)) {
                    intersects = true;
                    intersected = exist;
                    break;
                }
            }
            if (intersects) {
                /// 交差していた場合，
                /// 既存の矩形をすべて含んだ矩形(以下，union)の一番下のできるだけ左に配置しようとする
                /// それが無理であればunionの下の一番左に配置する
                var union = exists.Aggregate((r1, r2) => Rectangle.Union(r1, r2));
                var top = union.Bottom - newRect.Height;
                var left = leftMargin;
                var cantArrangeRight = false;
                foreach (var exist in exists) {
                    if (exist.Bottom >= top) {
                        if (exist.Right + leftMargin >= left) {
                            left = exist.Right + leftMargin;
                            if (left + newRect.Width > rightMax) {
                                cantArrangeRight = true;
                                break;
                            }
                        }
                    }
                }
                if (cantArrangeRight) {
                    return new Point(leftMargin, union.Bottom + topMargin);
                } else {
                    return new Point(left, top);
                }
            } else {
                return newRect.Location;
            }
        }

        /// <summary>
        /// rectから0より小さい領域を切り取ったRectangleを返す．
        /// </summary>
        public static Rectangle EnsureNoNegative(Rectangle rect) {
            var size = new Size(
                rect.Left < 0? rect.Width + rect.Left: rect.Width,
                rect.Top < 0? rect.Height + rect.Top: rect.Height
            );

            return new Rectangle(
                PointUtil.EnsureNotNegative(rect.Location),
                size
            );
        }

        /// <summary>
        /// rectをdelta平行移動したRectangleを返す．
        /// </summary>
        public static Rectangle Translate(Rectangle rect, Size delta) {
            return new Rectangle(rect.Location + delta, rect.Size);
        }

        /// <summary>
        /// rects内のRectangleをすべてdelta平行移動した配列を返す．
        /// </summary>
        public static Rectangle[] Translate(Rectangle[] rects, Size delta) {
            Contract.Requires(rects != null);
            var ret = new Rectangle[rects.Length];
            for (int i = 0, len = rects.Length; i < len; ++i) {
                ret[i] = new Rectangle(rects[i].Location + delta, rects[i].Size);
            }
            return ret;
        }

        /// <summary>
        /// rectの中心点を返す．
        /// </summary>
        public static Point GetCenter(Rectangle rect) {
            return new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
        }

        /// <summary>
        /// rectの中心点を返す．
        /// </summary>
        public static PointF GetCenterF(Rectangle rect) {
            return new PointF(rect.Left + (float) rect.Width / 2, rect.Top + (float) rect.Height / 2);
        }

        /// <summary>
        /// rectの中心点に対してptがどの方向にあるかを返す．
        /// 答えは Left | Up のように重なりがある。
        /// </summary>
        public static Directions GetDirectionFromCenter(Rectangle rect, Point pt) {
            return PointUtil.GetDirection(GetCenter(rect), pt);
        }

        /// <summary>
        /// rectの中心点に対してptがどの方向にあるかを返す．
        /// 答えは Left や Up や None のように重なりがない。
        /// </summary>
        public static Directions GetDirectionStraightlyFromCenter(Rectangle rect, Point pt) {
            return PointUtil.GetDirectionStraightly(GetCenter(rect), pt);
        }

        /// <summary>
        /// ptにもっとも近いrectの外枠上の点と，その点がrectの上下左右のどの辺にあるかを返す．
        /// 同じ距離の場合は，左・上・右・下の優先順位で結果を返す．
        /// </summary>
        public static Tuple<Point, Directions> GetNearestPointAndLineDirection(Rectangle rect, Point pt) {
            if (rect.Contains(pt)) {
                return GetNearestPointForInnerPoint(rect, pt);
            } else {
                return GetNearestPointForOuterPoint(rect, pt);
            }
        }

        /// <summary>
        /// ptにもっとも近いrectの外枠上の点を返す．
        /// 同じ距離の場合は，左・上・右・下の優先順位で結果を返す．
        /// </summary>
        public static Point GetNearestPoint(Rectangle rect, Point pt) {
            return GetNearestPointAndLineDirection(rect, pt).Item1;
        }

        /// <summary>
        /// rectの上下左右の辺のうちptに一番近いものを返す．
        /// 同じ距離の場合は，左・上・右・下の優先順位で結果を返す．
        /// </summary>
        public static Directions GetNearestLineDirection(Rectangle rect, Point pt) {
            return GetNearestPointAndLineDirection(rect, pt).Item2;
        }


        /// <summary>
        /// rect外の点ptがrectのどの方向にあるかを返す．
        /// rect内にptが含まれている場合はNoneを返す．
        /// </summary>
        public static Directions GetOuterDirection(Rectangle rect, Point pt) {
            if (rect.Contains(pt)) {
                return Directions.None;
            }

            var ret = Directions.None;
            if (rect.Width <= 0) {
                ret |= Directions.Left | Directions.Right;
            } else if (pt.X < rect.Left) {
                ret |= Directions.Left;
            } else if (pt.X >= rect.Right) {
                ret |= Directions.Right;
            }

            if (rect.Height <= 0) {
                ret |= Directions.Up | Directions.Down;
            } else if (pt.Y < rect.Top) {
                ret |= Directions.Up;
            } else if (pt.Y >= rect.Bottom) {
                ret |= Directions.Down;
            }

            return ret;
        }

        /// rectの枠線上の点ptがrectのどの方向の枠線上にあるかを返す．
        public static Directions GetOnDirectionCode(Rectangle rect, Point pt) {
            return GetOnDirectionCode(rect, pt.X, pt.Y);
        }

        public static Directions GetOnDirectionCode(Rectangle rect, int x, int y) {
            Directions ret = Directions.None;

            Vector2D point = new Vector2D(x, y);
            Vector2D leftTop = new Vector2D(rect.Left, rect.Top);
            Vector2D leftBottom = new Vector2D(rect.Left, rect.Bottom);
            Vector2D rightTop = new Vector2D(rect.Right, rect.Top);
            Vector2D rightBottom = new Vector2D(rect.Right, rect.Bottom);

            if (Vector2D.IsPointOnLineSeg(leftTop, leftBottom, point)) {
                ret |= Directions.Left;
            }
            if (Vector2D.IsPointOnLineSeg(rightTop, rightBottom, point)) {
                ret |= Directions.Right;
            }
            if (Vector2D.IsPointOnLineSeg(leftTop, rightTop, point)) {
                ret |= Directions.Up;
            }
            if (Vector2D.IsPointOnLineSeg(leftBottom, rightBottom, point)) {
                ret |= Directions.Down;
            }

            return ret;
        }

        /// <summary>
        /// rect内の点ptがrect内(枠線含む)のどの方向の領域にあるかを返す．
        /// </summary>
        public static Directions GetInnerDirection(Rectangle rect, Point pt) {
            Contract.Requires(rect.Contains(pt));

            var ret = Directions.None;
            var middle = new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);

            if (pt.X >= rect.Left && pt.X <= middle.X) {
                ret |= Directions.Left;
            }
            if (pt.X >= middle.X && pt.X <= rect.Right) {
                ret |= Directions.Right;
            }

            if (pt.Y >= rect.Top && pt.Y <= middle.Y) {
                ret |= Directions.Up;
            }
            if (pt.Y >= middle.Y && pt.Y <= rect.Bottom) {
                ret |= Directions.Down;
            }

            return ret;
        }

        /// <summary>
        /// rectとlineが交差するかどうかを返す．
        /// </summary>
        public static bool IntersectsWith(Rectangle rect, Line line) {
            return IntersectsWith(rect, line.Start, line.End);
        }

        /// <summary>
        /// rectと線分pt1-pt2が交差するかどうかを返す．
        /// </summary>
        public static bool IntersectsWith(Rectangle rect, Point pt1, Point pt2) {
            Directions out1, out2;
            /// pt2がrectに含まれている
            if ((out2 = GetOuterDirection(rect, pt2)) == Directions.None) {
                return true;
            }
            while ((out1 = GetOuterDirection(rect, pt1)) != Directions.None) {
                /// pt1とpt2がrectから見て上下左右の同じ方向にある
                if ((out1 & out2) != Directions.None) {
                    return false;
                }
                /// pt1がrectの横方向にある
                if ((out1 & (Directions.Left | Directions.Right)) != Directions.None) {
                    var x = rect.Left;
                    if ((out1 & Directions.Right) != Directions.None) {
                        x += rect.Width - 1;
                    }
                    pt1.Y = pt1.Y + (x - pt1.X) * (pt2.Y - pt1.Y) / (pt2.X - pt1.X);
                    pt1.X = x;
                } else {
                    var y = rect.Top;
                    if ((out1 & Directions.Down) != Directions.None) {
                        y += rect.Height - 1;
                    }
                    pt1.X = pt1.X + (y - pt1.Y) * (pt2.X - pt1.X) / (pt2.Y - pt1.Y);
                    pt1.Y = y;
                }
            }
            return true;
        }

        /// <summary>
        /// 線分pt1-pt2を対角線とするRectangleを返す
        /// </summary>
        public static Rectangle GetRectangleFromDiagonalPoints(Point pt1, Point pt2) {
            return new Rectangle(
                new Point(Math.Min(pt1.X, pt2.X), Math.Min(pt1.Y, pt2.Y)),
                new Size(Math.Abs(pt1.X - pt2.X) + 1, Math.Abs(pt1.Y - pt2.Y) + 1)
            );
        }

        /// <summary>
        /// 線分(lineSegStart，lineSegEnd)と矩形rectが交差する点を返す．
        /// そのような点が存在しない場合はArgumentExceptionをthrowする．
        /// 複数交点がある場合は，左，右，上，下の優先順で結果を返す．
        /// </summary>
        public static Point GetIntersectionPointOfRectAndLineSeg(Rectangle rect, Point lineSegStart, Point lineSegEnd) {
            var lineStartVec = (Vector2D) lineSegStart;
            var lineEndVec = (Vector2D) lineSegEnd;

            // 左
            {
                Vector2D rectLineStartVec = new Vector2D(rect.Left, rect.Top);
                Vector2D rectLineEndVec = new Vector2D(rect.Left, rect.Bottom);
                if (Vector2D.IsIntersectLineSegs(lineStartVec, lineEndVec, rectLineStartVec, rectLineEndVec)) {
                    return (Point) Vector2D.GetIntersectionPointOfLineSegs(
                        lineStartVec, lineEndVec, rectLineStartVec, rectLineEndVec
                    );
                }
            }

            // 右
            {
                Vector2D rectLineStartVec = new Vector2D(rect.Right, rect.Top);
                Vector2D rectLineEndVec = new Vector2D(rect.Right, rect.Bottom);
                if (Vector2D.IsIntersectLineSegs(lineStartVec, lineEndVec, rectLineStartVec, rectLineEndVec)) {
                    return (Point) Vector2D.GetIntersectionPointOfLineSegs(
                        lineStartVec, lineEndVec, rectLineStartVec, rectLineEndVec
                    );
                }
            }

            // 上
            {
                Vector2D rectLineStartVec = new Vector2D(rect.Left, rect.Top);
                Vector2D rectLineEndVec = new Vector2D(rect.Right, rect.Top);
                if (Vector2D.IsIntersectLineSegs(lineStartVec, lineEndVec, rectLineStartVec, rectLineEndVec)) {
                    return (Point) Vector2D.GetIntersectionPointOfLineSegs(
                        lineStartVec, lineEndVec, rectLineStartVec, rectLineEndVec
                    );
                }
            }

            // 下
            {
                Vector2D rectLineStartVec = new Vector2D(rect.Left, rect.Bottom);
                Vector2D rectLineEndVec = new Vector2D(rect.Right, rect.Bottom);
                if (Vector2D.IsIntersectLineSegs(lineStartVec, lineEndVec, rectLineStartVec, rectLineEndVec)) {
                    return (Point) Vector2D.GetIntersectionPointOfLineSegs(
                        lineStartVec, lineEndVec, rectLineStartVec, rectLineEndVec
                    );
                }
            }

            throw new ArgumentException("rect and line don't intersect");
        }

        /// <summary>
        /// 線分(lineSegStart，lineSegEnd)のlineSegEnd側を無限に延長した直線と矩形rectの交点を返す．
        /// lineSegStartとlineSegEndが同じ点の場合，
        /// rectへの垂線が存在する場合はその垂線とrectの左側または右側の交点を返す．
        /// 交点が存在しない場合はArgumentExceptionを返す．
        /// lineSegStartはrectに含まれる点でなければならない．
        /// </summary>
        public static Point GetIntersectionPointOfRectAndExtendedLineSeg(
            Rectangle rect, Point lineSegStart, Point lineSegEnd
        ) {
            if (!rect.Contains(lineSegStart)) {
                throw new ArgumentException("lineSegStart not contained by rect");
            }

            Vector2D lineStartVec = (Vector2D) lineSegStart;
            Vector2D lineEndVec = (Vector2D) lineSegEnd;

            // 2点が同じ点なら
            if (lineSegStart == lineSegEnd) {
                if (lineSegStart.Y >= rect.Top && lineSegStart.Y < rect.Bottom) {
                    return new Point(rect.Left, lineSegStart.Y);
                }
                if (lineSegStart.X >= rect.Left && lineSegStart.X < rect.Right) {
                    return new Point(lineSegStart.X, rect.Top);
                }
                throw new ArgumentException("rect and line don't intersect");
            }

            // startが左にある場合
            if (lineSegStart.X < lineSegEnd.X) {
                Vector2D rectLineStartVec = new Vector2D(rect.Right, rect.Top);
                Vector2D rectLineEndVec = new Vector2D(rect.Right, rect.Bottom);
                if (Vector2D.IsIntersectLines(lineStartVec, lineEndVec, rectLineStartVec, rectLineEndVec)) {
                    Vector2D ret = Vector2D.GetIntersectionPointOfLines(
                        lineStartVec, lineEndVec, rectLineStartVec, rectLineEndVec
                    );
                    if (ret.Y >= rect.Top && ret.Y < rect.Bottom) {
                        return (Point) ret;
                    }
                }
            } else if (lineSegStart.X > lineSegEnd.X) {
                // startが右にある場合
                Vector2D rectLineStartVec = new Vector2D(rect.Left, rect.Top);
                Vector2D rectLineEndVec = new Vector2D(rect.Left, rect.Bottom);
                if (Vector2D.IsIntersectLines(lineStartVec, lineEndVec, rectLineStartVec, rectLineEndVec)) {
                    Vector2D ret = Vector2D.GetIntersectionPointOfLines(
                        lineStartVec, lineEndVec, rectLineStartVec, rectLineEndVec
                    );
                    if (ret.Y >= rect.Top && ret.Y < rect.Bottom) {
                        return (Point) ret;
                    }
                }
            }


            // startが上にある場合
            if (lineSegStart.Y < lineSegEnd.Y) {
                Vector2D rectLineStartVec = new Vector2D(rect.Left, rect.Bottom);
                Vector2D rectLineEndVec = new Vector2D(rect.Right, rect.Bottom);
                if (Vector2D.IsIntersectLines(lineStartVec, lineEndVec, rectLineStartVec, rectLineEndVec)) {
                    Vector2D ret = Vector2D.GetIntersectionPointOfLines(
                        lineStartVec, lineEndVec, rectLineStartVec, rectLineEndVec
                    );
                    if (ret.X >= rect.Left && ret.X < rect.Right) {
                        return (Point) ret;
                    }
                }
            } else if (lineSegStart.Y > lineSegEnd.Y) {
                // startが下にある場合
                Vector2D rectLineStartVec = new Vector2D(rect.Left, rect.Top);
                Vector2D rectLineEndVec = new Vector2D(rect.Right, rect.Top);
                if (Vector2D.IsIntersectLines(lineStartVec, lineEndVec, rectLineStartVec, rectLineEndVec)) {
                    Vector2D ret = Vector2D.GetIntersectionPointOfLines(
                        lineStartVec, lineEndVec, rectLineStartVec, rectLineEndVec
                    );
                    if (ret.X >= rect.Left && ret.X < rect.Right) {
                        return (Point) ret;
                    }
                }
            }

            throw new ArgumentException("rect and line don't intersect");
        }


        // ------------------------------
        // private
        // ------------------------------
        private static Tuple<Point, Directions> GetNearestPointForInnerPoint(Rectangle rect, Point innerPt) {
            Contract.Requires(rect.Contains(innerPt));

            var dir = RectUtil.GetDirectionFromCenter(rect, innerPt);

            if (EnumUtil.HasAllFlags((int) dir, (int) Directions.UpLeft)) {
                return (innerPt.X - rect.Left <= innerPt.Y - rect.Top) ?
                    Tuple.Create(new Point(rect.Left, innerPt.Y), Directions.Left):
                    Tuple.Create(new Point(innerPt.X, rect.Top), Directions.Up);
            }
            if (EnumUtil.HasAllFlags((int) dir, (int) Directions.UpRight)) {
                return (rect.Right - innerPt.X <= innerPt.Y - rect.Top)?
                    Tuple.Create(new Point(rect.Right - 1, innerPt.Y), Directions.Right):
                    Tuple.Create(new Point(innerPt.X, rect.Top), Directions.Up);
            }
            if (EnumUtil.HasAllFlags((int) dir, (int) Directions.DownLeft)) {
                return (innerPt.X - rect.Left <= rect.Bottom - innerPt.Y)?
                    Tuple.Create(new Point(rect.Left, innerPt.Y), Directions.Left):
                    Tuple.Create(new Point(innerPt.X, rect.Bottom - 1), Directions.Down);
            }
            if (EnumUtil.HasAllFlags((int) dir, (int) Directions.DownRight)) {
                return (rect.Right - innerPt.X <= rect.Bottom - innerPt.Y)?
                    Tuple.Create(new Point(rect.Right - 1, innerPt.Y), Directions.Right):
                    Tuple.Create(new Point(innerPt.X, rect.Bottom - 1), Directions.Down);
            }

            throw new InvalidProgramException();
        }

        private static Tuple<Point, Directions> GetNearestPointForOuterPoint(Rectangle rect, Point outerPt) {
            Contract.Requires(!rect.Contains(outerPt));

            var dir = RectUtil.GetOuterDirection(rect, outerPt);
            Contract.Requires(dir != Directions.None);

            if (EnumUtil.HasAllFlags((int) dir, (int) Directions.UpLeft)) {
                return Tuple.Create(rect.Location, Directions.Left);
            }
            if (EnumUtil.HasAllFlags((int) dir, (int) Directions.UpRight)) {
                return Tuple.Create(new Point(rect.Right - 1, rect.Top), Directions.Right);
            }
            if (EnumUtil.HasAllFlags((int) dir, (int) Directions.DownLeft)) {
                return Tuple.Create(new Point(rect.Left, rect.Bottom - 1), Directions.Left);
            }
            if (EnumUtil.HasAllFlags((int) dir, (int) Directions.DownRight)) {
                return Tuple.Create(new Point(rect.Right - 1, rect.Bottom - 1), Directions.Right);
            }

            if (EnumUtil.HasAllFlags((int) dir, (int) Directions.Left)) {
                return Tuple.Create(new Point(rect.Left, outerPt.Y), Directions.Left);
            }
            if (EnumUtil.HasAllFlags((int) dir, (int) Directions.Right)) {
                return Tuple.Create(new Point(rect.Right - 1, outerPt.Y), Directions.Right);
            }
            if (EnumUtil.HasAllFlags((int) dir, (int) Directions.Up)) {
                return Tuple.Create(new Point(outerPt.X, rect.Top), Directions.Up);
            }
            if (EnumUtil.HasAllFlags((int) dir, (int) Directions.Down)) {
                return Tuple.Create(new Point(outerPt.X, rect.Bottom - 1), Directions.Down);
            }

            throw new InvalidProgramException();
        }

    }
}
