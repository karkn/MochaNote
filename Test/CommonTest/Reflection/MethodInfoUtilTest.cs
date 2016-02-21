/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mkamo.Common.Reflection {
    [TestClass]
    public class MethodInfoUtilTest {
        [TestMethod, ExpectedException(typeof(InvalidOperationException))]
        public void TestToStaticAction() {
            var foo = GetType().GetMethod("Foo");
            var act = MethodInfoUtil.ToStaticAction<int>(foo);
            act(10);
        }


        public static void Foo(int i) {
            throw new InvalidOperationException(i.ToString());
        }
    }
}
