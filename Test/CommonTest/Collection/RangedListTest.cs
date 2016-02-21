/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mkamo.Common.Collection {
    [TestClass]
    public class RangedListTest {
        [TestMethod]
        public void TestRangedListRead() {
            List<int> list = new List<int> {
                1, 2, 3, 4, 5, 6,
            };
            RangedList<int> ranged = new RangedList<int>(list);

            Assert.AreEqual(list.Count, ranged.Count);

            ranged.SetRange(2, 4);
            Assert.AreEqual(4, ranged.Count);
            Assert.AreEqual(3, ranged[0]);
            Assert.AreEqual(4, ranged[1]);
            Assert.AreEqual(5, ranged[2]);
            Assert.AreEqual(6, ranged[3]);

            List<int> ret = new List<int>();
            foreach (int i in ranged) {
                ret.Add(i);
            }
            Assert.AreEqual(3, ret[0]);
            Assert.AreEqual(4, ret[1]);
            Assert.AreEqual(5, ret[2]);
            Assert.AreEqual(6, ret[3]);

            try {
                ranged.SetRange(2, 5);
                Assert.Fail();
            } catch (ArgumentOutOfRangeException) {
                Assert.IsTrue(true);
            }
        }

    
        [TestMethod]
        public void TestRangedListAdd() {
            List<int> list = new List<int> {
                1, 2, 3, 4, 5, 6,
            };
            RangedList<int> ranged = new RangedList<int>(list, 2, 3);

            ranged.Add(100);
            Assert.AreEqual(7, list.Count);
            Assert.AreEqual(4, ranged.Count);
            Assert.AreEqual(3, ranged[0]);
            Assert.AreEqual(4, ranged[1]);
            Assert.AreEqual(5, ranged[2]);
            Assert.AreEqual(100, ranged[3]);
        }

        [TestMethod]
        public void TestRangedListInsert() {
            List<int> list = new List<int> {
                1, 2, 3, 4, 5, 6,
            };
            RangedList<int> ranged = new RangedList<int>(list, 2, 3);

            ranged.Insert(1, 100);
            Assert.AreEqual(7, list.Count);
            Assert.AreEqual(4, ranged.Count);
            Assert.AreEqual(3, ranged[0]);
            Assert.AreEqual(100, ranged[1]);
            Assert.AreEqual(4, ranged[2]);
            Assert.AreEqual(5, ranged[3]);
        }

        [TestMethod]
        public void TestRangedListRemove() {
            List<int> list = new List<int> {
                1, 2, 3, 4, 5, 6,
            };
            RangedList<int> ranged = new RangedList<int>(list, 2, 3);

            Assert.AreEqual(false, ranged.Remove(1));

            ranged.Remove(4);
            Assert.AreEqual(5, list.Count);
            Assert.AreEqual(2, ranged.Count);
            Assert.AreEqual(3, ranged[0]);
            Assert.AreEqual(5, ranged[1]);
        }

    
        [TestMethod]
        public void TestRangedListClear() {
            List<int> list = new List<int> {
                1, 2, 3, 4, 5, 6,
            };
            RangedList<int> ranged = new RangedList<int>(list, 2, 3);

            ranged.Clear();
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(0, ranged.Count);

            ranged.Add(100);
            Assert.AreEqual(4, list.Count);
            Assert.AreEqual(1, ranged.Count);
            Assert.AreEqual(100, list[2]);
            Assert.AreEqual(100, ranged[0]);
        }
    }
}
