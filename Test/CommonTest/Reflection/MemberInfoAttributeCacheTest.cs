/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace Mkamo.Common.Reflection {
    [TestClass]
    public class MemberInfoAttributeCacheTest {
        [TestMethod]
        public void Test() {
            var cache = new AttributeCache();
            var hogeMethod = typeof(Foo).GetMethod("Hoge");
            Assert.IsTrue(cache.IsDefined(hogeMethod, typeof(AfoAttribute)));
            Assert.IsFalse(cache.IsDefined(hogeMethod, typeof(BfoAttribute)));
        }

        public class AfoAttribute: Attribute {}
        public class BfoAttribute: Attribute {}

        public class Foo {
            [Afo]
            public void Hoge() {
            }
        }
    }
}
