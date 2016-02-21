/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Mkamo.Common.Forms.Drawing {
    [Serializable]
    public struct Vector2D {
        // ========================================
        // static field
        // ========================================
        public static readonly Vector2D Empty = new Vector2D();

        public const double Epsilon = 1e-10;

        // ========================================
        // field
        // ========================================
        public double X;
        public double Y;

        // ========================================
        // constructor
        // ========================================
        public Vector2D(double x, double y) {
            X = x;
            Y = y;
        }

        public Vector2D(Point pt) {
            X = pt.X;
            Y = pt.Y;
        }

        public Vector2D(PointF pt) {
            X = pt.X;
            Y = pt.Y;
        }

        // ========================================
        // property
        // ========================================
        public double Abs {
            get { return Math.Sqrt(X * X + Y * Y); }
        }
        public double Length {
            get { return Abs; }
        }

        public Vector2D UnitVector() {
            var len = Length;
            return new Vector2D(X / len, Y / len);
        }

        // ========================================
        // operator
        // ========================================
        public static Vector2D operator +(Vector2D v1, Vector2D v2) {
            return new Vector2D(v1.X + v2.X, v1.Y + v2.Y);
        }
        public static Vector2D operator -(Vector2D v1, Vector2D v2) {
            return new Vector2D(v1.X - v2.X, v1.Y - v2.Y);
        }
        public static Vector2D operator *(Vector2D v, double d) {
            return new Vector2D(v.X * d, v.Y * d);
        }
        public static Vector2D operator /(Vector2D v, double d) {
            return new Vector2D(v.X / d, v.Y / d);
        }
        public static explicit operator Vector2D(Point pt) {
            return new Vector2D(pt);
        }
        public static explicit operator Vector2D(PointF pt) {
            return new Vector2D(pt);
        }
        public static explicit operator Point(Vector2D v) {
            return new Point((int) v.X, (int) v.Y);
        }
        public static explicit operator PointF(Vector2D v) {
            return new PointF((float) v.X, (float) v.Y);
        }

        // ========================================
        // method
        // ========================================
        public double GetDistance(Vector2D other) {
            return Vector2D.GetDistance(this, other);
        }

        public double GetDotProduct(Vector2D other) {
            return Vector2D.GetDotProduct(this, other);
        }

        public double GetCrossProduct(Vector2D other) {
            return Vector2D.GetCrossProduct(this, other);
        }

        // ========================================
        // static method
        // ========================================
        public static bool NearlyEq(double d1, double d2) {
            return Math.Abs(d1 - d2) < Epsilon;
        }

        public static bool NearlyEq(Vector2D v1, Vector2D v2) {
            return NearlyEq(v1.X, v2.X) && NearlyEq(v1.Y, v2.Y);
        }

        /// <summary>
        /// 距離を返す．
        /// </summary>
        public static double GetDistance(Vector2D v1, Vector2D v2) {
            return (v1 - v2).Abs;
        }

        /// <summary>
        /// 内積を返す．
        /// </summary>
        public static double GetDotProduct(Vector2D v1, Vector2D v2) {
            return (v1.X * v2.X + v1.Y * v2.Y);
        }

        /// <summary>
        /// 外積を返す．
        /// </summary>
        public static double GetCrossProduct(Vector2D v1, Vector2D v2) {
            return (v1.X * v2.Y - v1.Y * v2.X);
        }

        /// <summary>
        /// 直交するかどうかを返す．
        /// </summary>
        public static bool IsOrthogonal(
            Vector2D line1Start, Vector2D line1End, Vector2D line2Start, Vector2D line2End
        ) {
            /// 内積が0なら直交
            return NearlyEq(GetDotProduct(line1Start - line1End, line2Start - line2End), 0.0);
        }

        /// <summary>
        /// 平行かどうかを返す．
        /// </summary>
        public static bool IsParallel(
            Vector2D line1Start, Vector2D line1End, Vector2D line2Start, Vector2D line2End
        ) {
            /// 外積が0なら平行
            return NearlyEq(GetCrossProduct(line1Start - line1End, line2Start - line2End), 0.0);
        }

        /// <summary>
        /// pointが直線上にあるかどうかを返す．
        /// </summary>
        public static bool IsPointOnLine(Vector2D linePointA, Vector2D linePointB, Vector2D point) {
            return NearlyEq(GetCrossProduct(linePointB - linePointA, point - linePointA), 0.0);
        }

        /// <summary>
        /// pointが線分上にあるかどうかを返す．
        /// </summary>
        public static bool IsPointOnLineSeg(Vector2D lineStart, Vector2D lineEnd, Vector2D point) {
            return NearlyEq(GetCrossProduct(lineEnd - lineStart, point - lineStart), 0.0) &&
                (GetDotProduct(lineEnd - lineStart, point - lineStart) > -Epsilon) &&
                (GetDotProduct(lineStart - lineEnd, point - lineEnd) > -Epsilon);
        }

        /// <summary>
        /// pointと直線の距離を返す．
        /// </summary>
        public static double GetDistanceLineAndPoint(Vector2D linePointA, Vector2D linePointB, Vector2D point) {
            return
                Math.Abs(GetCrossProduct(linePointB - linePointA, point - linePointA)) /
                (linePointB - linePointA).Abs;
        }

        /// <summary>
        /// pointと線分の距離を返す．
        /// </summary>
        public static double GetDistanceLineSegAndPoint(Vector2D lineStart, Vector2D lineEnd, Vector2D point) {
            if (GetDotProduct(lineEnd - lineStart, point - lineStart) < Epsilon) {
                return (point - lineStart).Abs;
            }
            if (GetDotProduct(lineStart - lineEnd, point - lineEnd) < Epsilon) {
                return (point - lineEnd).Abs;
            }
            return Math.Abs(GetCrossProduct(lineEnd - lineStart, point - lineStart)) / (lineEnd - lineStart).Abs;
        }

        /// <summary>
        /// pointに最も近い線分上の点を返す．
        /// </summary>
        public static Vector2D GetNearestPointOnLineSegFromPoint(Vector2D lineStart, Vector2D lineEnd, Vector2D point) {
            if (GetDotProduct(lineEnd - lineStart, point - lineStart) < Epsilon) {
                return lineStart;
            }
            if (GetDotProduct(lineStart - lineEnd, point - lineEnd) < Epsilon) {
                return lineEnd;
            }
            var ab = lineEnd - lineStart;
            var ap = point - lineStart;
            var k = GetDotProduct(ab, ap) / (ab.Abs * ab.Abs);
            return new Vector2D(k * lineEnd.X + (1 - k) * lineStart.X, k * lineEnd.Y + (1 - k) * lineStart.Y);

        }

        /// <summary>
        /// 線分が交わるかどうかを返す．
        /// </summary>
        public static bool IsIntersectLineSegs(
            Vector2D line1Start, Vector2D line1End, Vector2D line2Start, Vector2D line2End
        ) {
            /// 明らかに交わらない状況を高速に除去
            /// そんなに早くならない?
            //{
            //    var l1 = Math.Min(line1Start.X, line1End.X);
            //    var r1 = Math.Max(line1Start.X, line1End.X);
            //    var t1 = Math.Min(line1Start.Y, line1End.Y);
            //    var b1 = Math.Max(line1Start.Y, line1End.Y);

            //    var l2 = Math.Min(line2Start.X, line2End.X);
            //    var r2 = Math.Max(line2Start.X, line2End.X);
            //    var t2 = Math.Min(line2Start.Y, line2End.Y);
            //    var b2 = Math.Max(line2Start.Y, line2End.Y);

            //    if (l1 > r2) {
            //        return false;
            //    } else if (r1 < l2) {
            //        return false;
            //    } else if (t1 > b2) {
            //        return false;
            //    } else if (b1 < t2) {
            //        return false;
            //    }
            //}

            return 
                (GetCrossProduct(line1End - line1Start, line2Start - line1Start) *
                    GetCrossProduct(line1End - line1Start, line2End - line1Start) < Epsilon) &&
                (GetCrossProduct(line2End - line2Start, line1Start - line2Start) *
                    GetCrossProduct(line2End - line2Start, line1End - line2Start) < Epsilon);
        }


        /// <summary>
        /// 線分の交点を返す．
        /// </summary>
        public static Vector2D GetIntersectionPointOfLineSegs(
            Vector2D line1Start, Vector2D line1End, Vector2D line2Start, Vector2D line2End
        ) {
            var b = line2End - line2Start;
            var d1 = Math.Abs(GetCrossProduct(b, line1Start - line2Start));
            var d2 = Math.Abs(GetCrossProduct(b, line1End - line2Start));
            var t = d1 / (d1 + d2);
            return line1Start + (line1End - line1Start) * t;
        }

        /// <summary>
        /// 直線が交わるかどうかを返す．
        /// </summary>
        public static bool IsIntersectLines(
            Vector2D line1PointA, Vector2D line1PointB, Vector2D line2PointA, Vector2D line2PointB
        ) {
            return !NearlyEq(GetCrossProduct(line1PointA - line1PointB, line2PointA - line2PointB), 0.0);
        }

        /// <summary>
        /// 直線の交点を返す．
        /// </summary>
        public static Vector2D GetIntersectionPointOfLines(
            Vector2D line1PointA, Vector2D line1PointB, Vector2D line2PointA, Vector2D line2PointB
        ) {
            var a = line1PointB - line1PointA; 
            var b = line2PointB - line2PointA;
            return line1PointA + a * GetCrossProduct(b, line2PointA - line1PointA) / GetCrossProduct(b, a);
        }

    }
}
