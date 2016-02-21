/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mkamo.Common.Association;
using System.Collections.ObjectModel;

namespace Mkamo.Common.Association {
    [TestClass]
    public class AssociationCollectionTest {
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
        }

        private class FooGroup {
            private AssociationCollection<Foo> _foos;
            public FooGroup() {
                _foos = new AssociationCollection<Foo>(
                    foo => foo.Group = this,
                    foo => foo.Group = null
                );
            }
            public Collection<Foo> Foos {
                get { return _foos; }
            }
        }
    }
}
