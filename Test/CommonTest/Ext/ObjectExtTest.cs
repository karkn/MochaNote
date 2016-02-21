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
    public class ObjectExtTest {
        // ========================================
        // method
        // ========================================
        [TestMethod]
        public void TestSymbol() {
            var foo = new Foo();
            Assert.AreEqual("Hoge", foo.Symbol(f => f.Hoge));
            Assert.AreEqual("Geho", foo.Symbol(f => f.Geho));
        }


        private class Foo {
            public int Hoge {
                get { return 1; }
            }

            public string Geho {
                get { return ""; }
            }

            public void Aho() {
            }
        }
    }
}
