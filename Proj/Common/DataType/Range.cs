/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Mkamo.Common.Diagnostics;

namespace Mkamo.Common.DataType {
    /// <summary>
    /// 範囲を表す．Immutable．
    /// </summary>
    [Serializable]
    public struct Range: IEquatable<Range>, IEnumerable<int> {
        // ========================================
        // static field
        // ========================================
        public static readonly Range Empty = new Range(int.MinValue, 0);


        // ========================================
        // static method
        // ========================================
        public static bool operator ==(Range a, Range b) {
            return a.Equals(b);
        }

        public static bool operator !=(Range a, Range b) {
            return !a.Equals(b);
        }

        public static Range FromStartAndEnd(int start, int end) {
            var offset = Math.Min(start, end);
            var len = Math.Abs(end - start) + 1;
            return new Range(offset, len);
        }

        public static Range FromStartAndEnd(int start, int end, bool excludeEnd) {
            return FromStartAndEnd(start, excludeEnd ? end - 1 : end);
        }

        public static Range FromStartAndLength(int start, int length) {
            return new Range(start, length);
        }

        // ========================================
        // field
        // ========================================
        private int _offset;
        private int _length;

        // ========================================
        // constructor
        // ========================================
        /// <summary>
        /// 開始offset，長さlengthのRangeオブジェクトを生成する．
        /// </summary>
        public Range(int offset, int length) {
            Contract.Requires(length > -1);
            _offset = offset;
            _length = length;
        }


        // ========================================
        // property
        // ========================================
        public int Offset {
            get { return _offset; }
        }

        public int Length {
            get { return _length; }
        }

        public int Start {
            get { return _offset; }
        }

        public int End {
            get { return _offset + _length - 1; }
        }

        public bool IsEmpty {
            get { return _length == 0; }
        }

        // ========================================
        // method
        // ========================================
        public override string ToString() {
            return "Range(" + _offset + "," + _length + ")";
        }

        public override bool Equals(object obj) {
            if (obj == null || obj.GetType() != GetType()) {
                return false;
            } else {
                return Equals((Range) obj);
            }
        }

        public override int GetHashCode() {
            return _offset ^ _length;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        public IEnumerator<int> GetEnumerator() {
            for (int i = _offset, end = _offset + _length; i < end; ++i) {
                yield return i;
            }
        }

        public bool Equals(Range other) {
            return (_offset == other._offset) && (_length == other._length);
        }


        /// <summary>
        /// otherと交差する範囲があるかどうかを返す．
        /// </summary>
        public bool Intersects(Range other) {
            if (IsEmpty || other.IsEmpty) {
                return false;
            }
            return
                other.Contains(Start) ||
                other.Contains(End) ||
                Contains(other);
        }

        /// <summary>
        /// このRangeがotherを含むかどうかを返す．
        /// proper == trueの場合は，端点が重ならない場合にのみtrueを返す．
        /// </summary>
        public bool Contains(Range other) {
            if (IsEmpty || other.IsEmpty) {
                return false;
            }
            return Start <= other.Start && End >= other.End;
        }

        /// <summary>
        /// このRangeがindexを含むかどうかを返す．
        /// proper == trueの場合は，indexが端点と重ならない場合にのみtrueを返す．
        /// </summary>
        public bool Contains(int index) {
            if (IsEmpty) {
                return false;
            }
            return index >= Start && index <= End;
        }

        /// <summary>
        /// このRangeとotherのどちらにも含まれている範囲を返す．
        /// </summary>
        public Range Intersection(Range other) {
            if (IsEmpty || other.IsEmpty) {
                return Range.Empty;
            }

            /// no intersection
            if (!Intersects(other)) {
                return Range.Empty;
            }

            /// one contains the other
            if (Contains(other)) {
                return other;
            }
            if (other.Contains(this)) {
                return this;
            }

            /// partial intersection
            if (Start < other.Start) {
                return new Range(other.Start, End - other.Start + 1);
            }
            if (Start > other.Start) {
                return new Range(Start, other.End - Start + 1);
            }

            throw new Exception("maybe program bug");
        }

        /// <summary>
        /// このRangeからotherの範囲を切り取ってできるRangeの配列を返す．
        /// </summary>
        public Range[] Subtract(Range other) {
            if (IsEmpty || other.IsEmpty) {
                return new [] { this };
            }

            if (Intersects(other)) {
                var ret = new List<Range>(2);
                if (Start < other.Start) {
                    ret.Add(new Range(Start, other.Start - Start));
                }
                if (End > other.End) {
                    ret.Add(new Range(other.End + 1, End - other.End));
                }
                return ret.ToArray();
            } else {
                return new [] { this };
            }
        }

        /// <summary>
        /// deltaだけOffsetを移動したRangeを返す．
        /// </summary>
        public Range Shift(int delta) {
            return new Range(Offset + delta, Length);
        }

        /// <summary>
        /// deltaだけLengthを増加したRangeを返す．
        /// </summary>
        public Range Extend(int delta) {
            return new Range(Offset, Length + delta);
        }

        /// <summary>
        /// このRangeのindexから前方部分のRangeを返す．indexも含む．
        /// </summary>
        public Range Former(int index) {
            if (index < _offset) {
                return Range.Empty;
            }
            if (index > End) {
                return this;
            }
            return new Range(_offset, index - _offset + 1);
        }

        /// <summary>
        /// このRangeのindexから後方部分のRangeを返す．indexも含む．
        /// </summary>
        public Range Latter(int index) {
            if (index < _offset) {
                return this;
            }
            if (index > End) {
                return Range.Empty;
            }
            return new Range(index, End - index + 1);
        }


        /// <summary>
        /// insertedの範囲分挿入してRangeを返す．
        /// </summary>
        public Range Insert(Range inserted) {
            if (inserted.IsEmpty) {
                return this;
            }

            if (inserted.Offset < Offset) {
                return new Range(Offset + inserted.Length, Length);

            } else if (inserted.Offset < Offset + Length) {
                return new Range(Offset, Length + inserted.Length);

            } else {
                return new Range(Offset, Length);
            }
        }

        /// <summary>
        /// removedの範囲分切り取ったRangeを返す．
        /// </summary>
        public Range Remove(Range removed) {
            if (removed.IsEmpty) {
                return this;
            }

            if (Intersects(removed)) {
                if (removed.Start <= Start) {
                    if (Contains(removed.End)) {
                        return new Range(removed.Offset, End - removed.End);
                    } else {
                        return new Range(removed.Offset, 0);
                    }
                } else {
                    if (Contains(removed.End)) {
                        return new Range(Offset, Length - removed.Length);
                    } else {
                        return new Range(Offset, removed.Start - Start);
                    }
                }

            } else if (Start > removed.End) {
                return new Range(Offset - removed.Length, Length);

            } else {
                /// if (End < removed.Start) のときここにくる
                /// do nothing
                return new Range(Offset, Length);
            }
        }

    }

}
