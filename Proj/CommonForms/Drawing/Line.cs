/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Mkamo.Common.Diagnostics;

namespace Mkamo.Common.Forms.Drawing {
    [Serializable]
    public struct Line: IEquatable<Line> {
        // ========================================
        // static field
        // ========================================
        public static readonly Line Empty = new Line();

        // ========================================
        // static field
        // ========================================
        public static bool operator ==(Line a, Line b) {
            return a.Equals(b);
        }

        public static bool operator !=(Line a, Line b) {
            return !a.Equals(b);
        }

        // ========================================
        // field
        // ========================================
        private Point _start;
        private Point _end;

        // ========================================
        // constructor
        // ========================================
        public Line(Point start, Point end) {
            //Require.Argument(start != end, "start, end");
            _start = start;
            _end = end;
        }

        // ========================================
        // property
        // ========================================
        public Point Start {
            get { return _start; }
            set {
                Contract.Requires(value != _end);
                _start = value;
            }
        }

        public Point End {
            get { return _end; }
            set {
                Contract.Requires(value != _start);
                _end = value;
            }
        }

        public Vector2D StartVector {
            get { return (Vector2D) _start; }
        }

        public Vector2D EndVector {
            get { return (Vector2D) _end; }
        }

        // ========================================
        // method
        // ========================================
        // === object ==========
        public override int GetHashCode() {
            return _start.GetHashCode() ^ _end.GetHashCode();
        }

        public override bool Equals(object obj) {
            if (obj is Line) {
                return Equals((Line) obj);
            } else {
                return false;
            }
        }

        // === IEquitable ==========
        public bool Equals(Line other) {
            return _start == other._start && _end == other._end;
        }

        // === Line ==========
        public bool IntersectsWith(Line other) {
            return IntersectsWith(other, false);
        }

        /// <summary>
        /// lengthenがtrueのときは直線同士が交わるかどうか，
        /// falseのときは線分同士が交わるかどうかを返す．
        /// </summary>
        public bool IntersectsWith(Line other, bool lengthen) {
            if (lengthen) {
                return Vector2D.IsIntersectLines(
                    StartVector, EndVector, other.StartVector, other.EndVector
                );
            } else {
                return Vector2D.IsIntersectLineSegs(
                    StartVector, EndVector, other.StartVector, other.EndVector
                );
            }
        }

        public bool Contains(Point pt) {
            return Contains(pt, false);
        }

        public bool Contains(Point pt, bool lengthen) {
            if (lengthen) {
                return Vector2D.IsPointOnLine(StartVector, EndVector, (Vector2D) pt);
            } else {
                return Vector2D.IsPointOnLineSeg(StartVector, EndVector, (Vector2D) pt);
            }
        }

        public Point GetIntersectionPoint(Line other) {
            return GetIntersectionPoint(other, false);
        }

        /// <summary>
        /// lengthenがtrueのときは直線同士の交点，
        /// falseのときは線分同士の交点を求める
        /// </summary>
        public Point GetIntersectionPoint(Line other, bool lengthen) {
            if (lengthen) {
                return (Point) Vector2D.GetIntersectionPointOfLines(
                    StartVector, EndVector, other.StartVector, other.EndVector
                );
            } else {
                return (Point) Vector2D.GetIntersectionPointOfLineSegs(
                    StartVector, EndVector, other.StartVector, other.EndVector
                );
            }
        }

        public int GetDistance(Point pt) {
            return (int) Vector2D.GetDistanceLineSegAndPoint(
                StartVector, EndVector, (Vector2D) pt
            );
        }

        public Point GetNearestPointFrom(Point pt) {
            return (Point) Vector2D.GetNearestPointOnLineSegFromPoint(
                StartVector, EndVector, (Vector2D) pt
            );
        }

    }
}
