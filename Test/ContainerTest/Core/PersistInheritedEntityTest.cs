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
    public class PersistInheritedEntityTest {
        IEntityContainer _container1;
        IEntityContainer _container2;

        [TestInitialize]
        public void Setup() {
            _container1 = new EntityContainer(new XmlFileEntityStore(@"d:\tmp\_store"));
            _container2 = new EntityContainer(new XmlFileEntityStore(@"d:\tmp\_store"));
        }

        [TestMethod]
        public void TestEntityInheritedFromEntity() {
            EntityInheritedFromEntity ent = _container1.Create<EntityInheritedFromEntity>();
            string id = _container1.GetId(ent);
            ent.IntValue = 1;
            ent.StringValue = "テスト";

            _container1.Commit();

            ent = _container2.Find<EntityInheritedFromEntity>(id);
            Assert.AreEqual(1, ent.IntValue);
            Assert.AreEqual("テスト", ent.StringValue);
        }

        //[TestMethod]
        //public void TestEntityInheritedFromClass() {
        //    EntityInheritedFromClass ent = _container1.Create<EntityInheritedFromClass>();
        //    string id = _container1.GetId(ent);
        //    ent.IntValue = 1;
        //    ent.StringValue = "テスト";

        //    _container1.Commit();

        //    ent = _container2.Find<EntityInheritedFromClass>(id);
        //    Assert.AreEqual(1, ent.IntValue);
        //    Assert.AreEqual("テスト", ent.StringValue);
        //}


        [Entity]
        public class BaseEntity {
            private int _intValue;

            [Persist]
            public virtual int IntValue {
                get { return _intValue; }
                set { _intValue = value; }
            }
        }

        [Entity]
        public class EntityInheritedFromEntity: BaseEntity {
            private string _stringValue;

            [Persist]
            public virtual string StringValue {
                get { return _stringValue; }
                set { _stringValue = value; }
            }
        }

        public class BaseClass {
            private int _intValue;

            public virtual int IntValue {
                get { return _intValue; }
                set { _intValue = value; }
            }
        }

        [Entity]
        public class EntityInheritedFromClass: BaseClass {
            private string _stringValue;

            [Persist]
            public virtual string StringValue {
                get { return _stringValue; }
                set { _stringValue = value; }
            }
        }

    }
}
