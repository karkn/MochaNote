/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mkamo.Common.Collection;
using Mkamo.Common.Core;

namespace Mkamo.Common.Collection {
    [TestClass]
    public class ListUtilTest {
        [TestMethod]
        public void TestSubtract() {
            object obj1 = new object();
            object obj2 = new object();
            object obj3 = new object();
            object obj4 = new object();
            object obj5 = new object();

            IList<object> list1 = new List<object>();
            list1.Add(obj1);
            list1.Add(obj2);
            list1.Add(obj3);
            list1.Add(obj4);

            IList<object> list2 = new List<object>();
            list2.Add(obj2);
            list2.Add(obj3);
            list2.Add(obj5);

            ICollection<object> list3 = ICollectionUtil.Subtract(list1, list2);
            Assert.IsTrue(list3.Contains(obj1));
            Assert.IsFalse(list3.Contains(obj2));
            Assert.IsFalse(list3.Contains(obj3));
            Assert.IsTrue(list3.Contains(obj4));
            Assert.IsFalse(list3.Contains(obj5));

            ICollection<object> list4 = ICollectionUtil.Subtract(list2, list1);
            Assert.IsFalse(list4.Contains(obj1));
            Assert.IsFalse(list4.Contains(obj2));
            Assert.IsFalse(list4.Contains(obj3));
            Assert.IsFalse(list4.Contains(obj4));
            Assert.IsTrue(list4.Contains(obj5));
        }
    }
}
