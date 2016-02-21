/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mkamo.Common.Association;

namespace Mkamo.Common.Association {
    [TestClass]
    public class AssociationUtilTest {
        [TestMethod]
        public void TestEnsureAssociation() {
            var foo1 = new Foo();
            var bar1 = new Bar();
            var bar2 = new Bar();

            Assert.AreEqual(null, foo1.Bar);
            Assert.AreEqual(null, bar1.Foo);
            Assert.AreEqual(null, bar2.Foo);

            foo1.Bar = bar1;
            Assert.AreEqual(bar1, foo1.Bar);
            Assert.AreEqual(foo1, bar1.Foo);

            foo1.Bar = bar2;
            Assert.AreEqual(bar2, foo1.Bar);
            Assert.AreEqual(null, bar1.Foo);
            Assert.AreEqual(foo1, bar2.Foo);

            bar2.Foo = null;
            Assert.AreEqual(null, foo1.Bar);
            Assert.AreEqual(null, bar1.Foo);
            Assert.AreEqual(null, bar2.Foo);
        }

        private class Foo {
            private Bar _bar;
            public Bar Bar {
                get { return _bar; }
                set {
                    AssociationUtil.EnsureAssociation(
                        _bar,
                        value,
                        bar => _bar = bar,
                        bar => bar.Foo = this,
                        bar => bar.Foo = null
                    );
                }
            }
        }

        private class Bar {
            private Foo _foo;
            public Foo Foo {
                get { return _foo; }
                set {
                    AssociationUtil.EnsureAssociation(
                        _foo,
                        value,
                        foo => _foo = foo,
                        foo => foo.Bar = this,
                        foo => foo.Bar = null
                    );
                }
            }
        }
    }
}
