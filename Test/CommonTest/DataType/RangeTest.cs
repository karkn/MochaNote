/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mkamo.Common.DataType {
    /// <summary>
    /// RangeTest の概要の説明
    /// </summary>
    [TestClass]
    public class RangeTest {
        public RangeTest() {
        }

        [TestMethod]
        public void TestGetEnumerator() {
            var range = new Range(10, 10); // 10-19

            var i = 10;
            foreach (var elem in range) {
                Assert.AreEqual(i, elem);
                ++i;
            }
            Assert.AreEqual(20, i);
        }

        [TestMethod]
        public void TestIntersects() {
            var range = new Range(10, 10); // 10-19

            Assert.IsTrue(range.Intersects(new Range(10, 10))); // 10-19

            Assert.IsTrue(range.Intersects(new Range(5, 6))); // 5-10
            Assert.IsTrue(range.Intersects(new Range(19, 6))); // 19-24

            Assert.IsTrue(range.Intersects(new Range(5, 10))); // 5-14
            Assert.IsTrue(range.Intersects(new Range(15, 10))); // 15-24

            Assert.IsFalse(range.Intersects(new Range(5, 5))); // 5-9
            Assert.IsFalse(range.Intersects(new Range(20, 5))); // 20-24
        }

        [TestMethod]
        public void TestContainsRange() {
            var range = new Range(10, 10); // 10-19

            Assert.IsTrue(range.Contains(new Range(10, 10))); // 10-19

            Assert.IsTrue(range.Contains(new Range(11, 8))); // 11-18

            Assert.IsFalse(range.Contains(new Range(5, 10))); // 5-14
            Assert.IsFalse(range.Contains(new Range(15, 10))); // 15-24

            Assert.IsFalse(range.Contains(new Range(5, 5))); // 5-9
            Assert.IsFalse(range.Contains(new Range(20, 5))); // 20-24

            Assert.IsFalse(range.Contains(new Range(11,0)));
        }

        [TestMethod]
        public void TestContainsInt() {
            var range = new Range(10, 10); // 10-19

            Assert.IsTrue(range.Contains(10));
            Assert.IsTrue(range.Contains(19));

            Assert.IsTrue(range.Contains(11));
            Assert.IsTrue(range.Contains(18));

            Assert.IsFalse(range.Contains(5));
            Assert.IsFalse(range.Contains(25));

            Assert.IsFalse(range.Contains(9));
            Assert.IsFalse(range.Contains(20));
        }

        [TestMethod]
        public void TestIntersection() {
            var range = new Range(10, 10); // 10-19

            Assert.AreEqual(new Range(10, 10), range.Intersection(new Range(10, 10))); // 10-19

            Assert.AreEqual(new Range(10, 1), range.Intersection(new Range(5, 6))); // 5-10
            Assert.AreEqual(new Range(19, 1), range.Intersection(new Range(19, 6))); // 19-24

            Assert.AreEqual(new Range(10, 5), range.Intersection(new Range(5, 10))); // 5-14
            Assert.AreEqual(new Range(15, 5), range.Intersection(new Range(15, 10))); // 15-24

            Assert.AreEqual(Range.Empty, range.Intersection(new Range(5, 5))); // 5-9
            Assert.AreEqual(Range.Empty, range.Intersection(new Range(20, 5))); // 20-24
        }

        [TestMethod]
        public void TestSubtract() {
            var range = new Range(10, 10); // 10-19

            /// 前方交差
            CollectionAssert.AreEqual(new[] { new Range(13, 7) }, range.Subtract(new Range(8, 5))); // 8-12

            /// 後方交差
            CollectionAssert.AreEqual(new[] { new Range(10, 8) }, range.Subtract(new Range(18, 5))); // 18-22

            /// 前方一致
            CollectionAssert.AreEqual(new[] { new Range(15, 5) }, range.Subtract(new Range(10, 5))); // 10-14

            /// 後方一致
            CollectionAssert.AreEqual(new[] { new Range(10, 5) }, range.Subtract(new Range(15, 5))); // 15-19

            /// すべて含む
            CollectionAssert.AreEqual(new[] { new Range(10, 2), new Range(18, 2) }, range.Subtract(new Range(12, 6))); // 12-17

            /// すべて含まれる
            CollectionAssert.AreEqual(new Range[0], range.Subtract(new Range(9, 12))); // 9-20

            /// 一致
            CollectionAssert.AreEqual(new Range[0], range.Subtract(new Range(10, 10))); // 10-19

            /// 重ならない前方
            CollectionAssert.AreEqual(new[] { new Range(10, 10) }, range.Subtract(new Range(0, 10))); // 0-9

            /// 重ならない後方
            CollectionAssert.AreEqual(new[] { new Range(10, 10) }, range.Subtract(new Range(20, 10))); // 20-29

            /// 空
            CollectionAssert.AreEqual(new[] { range }, range.Subtract(Range.Empty)); // Empty
            CollectionAssert.AreEqual(new[] { Range.Empty }, Range.Empty.Subtract(Range.Empty)); // Empty
        }

        [TestMethod]
        public void TestShift() {
            var range = new Range(10, 10); // 10-19

            Assert.AreEqual(new Range(15, 10), range.Shift(5));
            Assert.AreEqual(new Range(5, 10), range.Shift(-5));
            Assert.AreEqual(range, range.Shift(0));
        }
        
        [TestMethod]
        public void TestExtend() {
            var range = new Range(10, 10); // 10-19

            Assert.AreEqual(new Range(10, 15), range.Extend(5));
            Assert.AreEqual(new Range(10, 5), range.Extend(-5));
            Assert.AreEqual(range, range.Extend(0));
        }
        
        [TestMethod]
        public void TestLatter() {
            var range = new Range(10, 10); // 10-19

            Assert.AreEqual(new Range(10, 10), range.Latter(10));
            Assert.AreEqual(new Range(15, 5), range.Latter(15));
            Assert.AreEqual(new Range(19, 1), range.Latter(19));

            Assert.AreEqual(range, range.Latter(9));
            Assert.AreEqual(Range.Empty, range.Latter(20));
        }

        [TestMethod]
        public void TestFormer() {
            var range = new Range(10, 10); // 10-19

            Assert.AreEqual(new Range(10, 1), range.Former(10));
            Assert.AreEqual(new Range(10, 6), range.Former(15));
            Assert.AreEqual(new Range(10, 10), range.Former(19));

            Assert.AreEqual(Range.Empty, range.Former(9));
            Assert.AreEqual(range, range.Former(20));
        }

    
        [TestMethod]
        public void TestInsert() {
            var range = new Range(10, 10); // 10-19

            /// 前方交差
            Assert.AreEqual(new Range(15, 10), range.Insert(new Range(8, 5))); // 8-12

            /// 後方交差
            Assert.AreEqual(new Range(10, 15), range.Insert(new Range(18, 5))); // 18-22

            /// 前方一致
            Assert.AreEqual(new Range(10, 15), range.Insert(new Range(10, 5))); // 10-14

            /// 後方一致
            Assert.AreEqual(new Range(10, 15), range.Insert(new Range(15, 5))); // 15-19

            /// すべて含む
            Assert.AreEqual(new Range(10, 16), range.Insert(new Range(12, 6))); // 12-17

            /// すべて含まれる
            Assert.AreEqual(new Range(22, 10), range.Insert(new Range(9, 12))); // 9-20

            /// 一致
            Assert.AreEqual(new Range(10, 20), range.Insert(new Range(10, 10))); // 10-19

            /// 重ならない前方
            Assert.AreEqual(new Range(20, 10), range.Insert(new Range(0, 10))); // 0-9

            /// 重ならない後方
            Assert.AreEqual(range, range.Insert(new Range(20, 10))); // 20-29

            /// 空
            Assert.AreEqual(range, range.Insert(Range.Empty)); // Empty
            Assert.AreEqual(Range.Empty, Range.Empty.Insert(Range.Empty)); // Empty
        }

        [TestMethod]
        public void TestRemove() {
            var range = new Range(10, 10); // 10-19

            /// 前方交差
            Assert.AreEqual(new Range(8, 7), range.Remove(new Range(8, 5))); // 8-12

            /// 後方交差
            Assert.AreEqual(new Range(10, 8), range.Remove(new Range(18, 5))); // 18-22

            /// 前方一致
            Assert.AreEqual(new Range(10, 5), range.Remove(new Range(10, 5))); // 10-14

            /// 後方一致
            Assert.AreEqual(new Range(10, 5), range.Remove(new Range(15, 5))); // 15-19

            /// 一致
            Assert.AreEqual(new Range(10, 0), range.Remove(new Range(10, 10))); // 10-19

            /// すべて含む
            Assert.AreEqual(new Range(10, 4), range.Remove(new Range(12, 6))); // 12-17

            /// すべて含まれる
            Assert.AreEqual(new Range(9, 0), range.Remove(new Range(9, 12))); // 9-20

            /// 重ならない前方
            Assert.AreEqual(new Range(0, 10), range.Remove(new Range(0, 10))); // 0-9

            /// 重ならない後方
            Assert.AreEqual(range, range.Remove(new Range(20, 10))); // 20-29

            /// 空
            Assert.AreEqual(range, range.Remove(Range.Empty)); // Empty
            Assert.AreEqual(Range.Empty, Range.Empty.Remove(Range.Empty)); // Empty


        }

    }
}
