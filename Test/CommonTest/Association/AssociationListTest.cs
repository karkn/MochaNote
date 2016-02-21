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
    public class AssociationListTest {
        [TestMethod]
        public void TestOneToOne() {
            var foo = new Foo();
            var bar1 = new Bar();
            var bar2 = new Bar();

            Assert.AreEqual(null, foo.Bar);
            Assert.AreEqual(null, bar1.Foo);
            Assert.AreEqual(null, bar2.Foo);

            foo.Bar = bar1;
            Assert.AreEqual(bar1, foo.Bar);
            Assert.AreEqual(foo, bar1.Foo);

            foo.Bar = bar2;
            Assert.AreEqual(bar2, foo.Bar);
            Assert.AreEqual(null, bar1.Foo);
            Assert.AreEqual(foo, bar2.Foo);

            bar2.Foo = null;
            Assert.AreEqual(null, foo.Bar);
            Assert.AreEqual(null, bar1.Foo);
            Assert.AreEqual(null, bar2.Foo);
        }

        [TestMethod]
        public void TestOneToMany() {
            var group1 = new FooGroup();
            var group2 = new FooGroup();
            var foo1 = new Foo();
            var foo2 = new Foo();

            Assert.AreEqual(0, group1.Foos.Count);
            Assert.AreEqual(null, foo1.Group);
            Assert.AreEqual(null, foo2.Group);

            foo1.Group = group1;
            Assert.AreEqual(1, group1.Foos.Count);
            Assert.AreEqual(group1, foo1.Group);
            Assert.AreEqual(null, foo2.Group);

            foo1.Group = null;
            Assert.AreEqual(0, group1.Foos.Count);
            Assert.AreEqual(null, foo1.Group);
            Assert.AreEqual(null, foo2.Group);

            group1.Foos.Add(foo2);
            Assert.AreEqual(1, group1.Foos.Count);
            Assert.AreEqual(null, foo1.Group);
            Assert.AreEqual(group1, foo2.Group);

            group1.Foos.Add(foo1);
            Assert.AreEqual(2, group1.Foos.Count);
            Assert.AreEqual(group1, foo1.Group);
            Assert.AreEqual(group1, foo2.Group);

            group2.Foos.Add(foo1);
            Assert.AreEqual(1, group1.Foos.Count);
            Assert.AreEqual(1, group2.Foos.Count);
            Assert.AreEqual(group2, foo1.Group);
            Assert.AreEqual(group1, foo2.Group);
        }

        private class Foo {
            private FooGroup _group;
            private Bar _bar;

            public FooGroup Group {
                get { return _group; }
                set {
                    AssociationUtil.EnsureAssociation(
                        _group,
                        value,
                        group => _group = group,
                        group => group.Foos.Add(this),
                        group => group.Foos.Remove(this)
                    );
                }
            }
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

        private class FooGroup {
            private AssociationList<Foo> _foos;
            public FooGroup() {
                _foos = new AssociationList<Foo>(
                    foo => foo.Group = this,
                    foo => foo.Group = null
                );
            }
            public IList<Foo> Foos {
                get { return _foos; }
            }
        }
    }
}
