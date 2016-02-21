/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mkamo.Common.Reflection {
    [TestClass]
    public class TypeUtilTest {
        [TestMethod]
        public void TestArrayIsICollection() {
            int[] i = new int[10];
            Assert.IsTrue(GenericTypeUtil.IsGenericICollection(i.GetType()));
        }

        [TestMethod]
        public void TestIList() {
            //Type foo = typeof(ICollection<int>).GetGenericTypeDefinition();
            //Assert.IsTrue(typeof(IList<>) == typeof(IList<>));
            Assert.IsTrue(GenericTypeUtil.IsGenericIList(typeof(IList<>)));
            //Type[] foo = typeof(ICollection<int>).GetInterfaces();
             //IList<int> list = new List<int>();
            //Assert.IsTrue(TypeUtil.GenericICollectionIsAssignableFrom(typeof(IList<int>)));
            //Type foo = typeof(IList<int>).GetInterface(typeof(IList<>).FullName);
            //Type[] foo = typeof(ICollection<int>).GetInterfaces();
            //Console.WriteLine(foo);
        }
    }
}
