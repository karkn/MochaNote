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
    public class EmptyListTest {
        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestSet() {
            var list = new EmptyList<object>();
            list[0] = new object();
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestGet() {
            var list = new EmptyList<object>();
            object ret = list[0];
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void TestAdd() {
            IList<object> list = new EmptyList<object>();
            list.Add(new object());
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void TestInsert() {
            IList<object> list = new EmptyList<object>();
            list.Insert(0, new object());
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void TestRemove() {
            IList<object> list = new EmptyList<object>();
            list.Remove(new object());
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void TestRemoveAt() {
            IList<object> list = new EmptyList<object>();
            list.RemoveAt(0);
        }

        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void TestClear() {
            IList<object> list = new EmptyList<object>();
            list.Clear();
        }

        [TestMethod]
        public void TestCount() {
            var list = new EmptyList<object>();
            Assert.AreEqual(0, list.Count);
        }

        [TestMethod]
        public void TestIsReadOnly() {
            var list = new EmptyList<object>();
            Assert.AreEqual(true, list.IsReadOnly);
        }

        [TestMethod]
        public void TestContains() {
            var list = new EmptyList<object>();
            Assert.AreEqual(false, list.Contains(new object()));
        }

        [TestMethod]
        public void TestIndexOf() {
            var list = new EmptyList<object>();
            Assert.AreEqual(-1, list.IndexOf(new object()));
        }

        [TestMethod]
        public void TestCopyTo() {
            var list = new EmptyList<object>();
            var arr = new object[1];
            list.CopyTo(arr, 0);
            Assert.AreEqual(null, arr[0]);
        }
    
        [TestMethod]
        public void TestGetEnumerator() {
            var list = new EmptyList<object>();
            int i = 0;
            foreach (object o in list) {
                ++i;
            }
            Assert.AreEqual(0, i);
        }
        
    }
}
