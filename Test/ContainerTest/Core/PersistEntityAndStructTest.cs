/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mkamo.Container.Core;
using Mkamo.Container.Internal.Core;

namespace Mkamo.Container.Core {
    [TestClass]
    public class PersistEntityAndStructTest {
        IEntityContainer _container1;
        IEntityContainer _container2;

        [TestInitialize]
        public void Setup() {
            _container1 = new EntityContainer(new XmlFileEntityStore(@"d:\tmp\_store"));
            _container2 = new EntityContainer(new XmlFileEntityStore(@"d:\tmp\_store"));
        }

        [TestMethod]
        public void Test() {
            Entity ent = _container1.Create<Entity>();
            string id = _container1.GetId(ent);
            ent.IntValue = 1;
            ent.FooValue = new Foo() { IntValue = 1 };
            
            _container1.Commit();

            ent = _container2.Find<Entity>(id);
            Assert.AreEqual(0, ent.IntValue); // 初期値
            Assert.AreEqual(1, ent.FooValue.IntValue);
        }

        [Entity]
        public class Entity {
            private int _intValue = 0;
            private Foo _fooValue;

            public virtual int IntValue {
                get { return _intValue; }
                set { _intValue = value; }
            }

            [Persist]
            public virtual Foo FooValue {
                get { return _fooValue; }
                set { _fooValue = value; }
            }
        }

        public struct Foo {
            private int _intValue;
            public int IntValue {
                get { return _intValue; }
                set { _intValue = value; }
            }
        }
    }
}
