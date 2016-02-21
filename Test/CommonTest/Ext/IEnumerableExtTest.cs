/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mkamo.Common.Core {
    [TestClass]
    public class IEnumerableExtTest {
        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        [TestMethod]
        public void TestRange() {
            var arr = new int[] { 0, 1, 2, 3, 4, 5, };

            var ranged = arr.Range(0, 6);
            Assert.AreEqual(6, ranged.Count());
            Assert.AreEqual(0, ranged.First());
            Assert.AreEqual(5, ranged.Last());

            ranged = arr.Range(0, 5);
            Assert.AreEqual(5, ranged.Count());
            Assert.AreEqual(0, ranged.First());
            Assert.AreEqual(4, ranged.Last());

            ranged = arr.Range(1, 5);
            Assert.AreEqual(5, ranged.Count());
            Assert.AreEqual(1, ranged.First());
            Assert.AreEqual(5, ranged.Last());
        }

        [TestMethod]
        public void TestEqualsEachInOrder() {
            var enum1 = new[] { 1, 2, 3, 4, };

            Assert.AreEqual(true, IEnumerableUtil.EqualsEachInOrder(enum1, new[] { 1, 2, 3, 4, }));

            Assert.AreEqual(false, IEnumerableUtil.EqualsEachInOrder(enum1, new[] { 1, 3, 2, 4 }));
            Assert.AreEqual(false, IEnumerableUtil.EqualsEachInOrder(enum1, new[] { 1, 2, 3 }));
            Assert.AreEqual(false, IEnumerableUtil.EqualsEachInOrder(enum1, new[] { 1, 2, 3, 4, 5, }));
            Assert.AreEqual(false, IEnumerableUtil.EqualsEachInOrder(enum1, new[] { 1, 3, 2 }));
            Assert.AreEqual(false, IEnumerableUtil.EqualsEachInOrder(enum1, new[] { 1, 3, 2, 4, 5, }));
        }

        [TestMethod]
        public void TestEqualsEachInRandomOrder() {
            var enum1 = new[] { 1, 2, 3, 4, };

            Assert.AreEqual(true, IEnumerableUtil.EqualsEachInRandomOrder(enum1, new[] { 1, 2, 3, 4, }));
            Assert.AreEqual(true, IEnumerableUtil.EqualsEachInRandomOrder(enum1, new[] { 1, 3, 2, 4 }));

            Assert.AreEqual(false, IEnumerableUtil.EqualsEachInRandomOrder(enum1, new[] { 1, 2, 3 }));
            Assert.AreEqual(false, IEnumerableUtil.EqualsEachInRandomOrder(enum1, new[] { 1, 2, 3, 4, 5, }));
            Assert.AreEqual(false, IEnumerableUtil.EqualsEachInRandomOrder(enum1, new[] { 1, 3, 2 }));
            Assert.AreEqual(false, IEnumerableUtil.EqualsEachInRandomOrder(enum1, new[] { 1, 3, 2, 4, 5, }));
        }
    }
}
