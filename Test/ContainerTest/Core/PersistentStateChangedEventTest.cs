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
    public class PersistentStateChangedEventTest {
        private IEntityContainer _container1;
        private IEntityContainer _container2;

        private List<PersistentStateChangedEventArgs> _notifiedEvents;

        [TestInitialize]
        public void Setup() {
            _container1 = new EntityContainer(new XmlFileEntityStore(@"d:\tmp\_store"));
            _container2 = new EntityContainer(new XmlFileEntityStore(@"d:\tmp\_store"));
            _notifiedEvents = new List<PersistentStateChangedEventArgs>();
        }

        [TestMethod]
        public void TestCommit() {
            Entity ent = _container1.Create<Entity>();
            _container1.AsEntity(ent).PersistentStateChanged += HandlePersistentStateChanged;

            ent.Value = 1;
            Assert.AreEqual(0, _notifiedEvents.Count);

            _container1.Commit();
            Assert.AreEqual(1, _notifiedEvents.Count);
            Assert.AreEqual(PersistentState.Latest, _notifiedEvents[0].State);

            //ent.Value = 1;
            //Assert.AreEqual(1, _notifiedEvents.Count);

            ent.Value = 3;
            Assert.AreEqual(2, _notifiedEvents.Count);
            Assert.AreEqual(PersistentState.Updated, _notifiedEvents[1].State);

            _container1.Remove(ent);
            Assert.AreEqual(3, _notifiedEvents.Count);
            Assert.AreEqual(PersistentState.Removed, _notifiedEvents[2].State);

            _container1.Commit();
            Assert.AreEqual(4, _notifiedEvents.Count);
            Assert.AreEqual(PersistentState.Discarded, _notifiedEvents[3].State);


        }

        [TestMethod]
        public void TestRollback() {
            Entity ent = _container1.Create<Entity>();
            _container1.AsEntity(ent).PersistentStateChanged += HandlePersistentStateChanged;

            ent.Value = 1;
            Assert.AreEqual(0, _notifiedEvents.Count);

            _container1.Commit();
            Assert.AreEqual(1, _notifiedEvents.Count);
            Assert.AreEqual(PersistentState.Latest, _notifiedEvents[0].State);

            ent.Value = 3;
            Assert.AreEqual(2, _notifiedEvents.Count);
            Assert.AreEqual(PersistentState.Updated, _notifiedEvents[1].State);

            _container1.Rollback();
            Assert.AreEqual(3, _notifiedEvents.Count);
            Assert.AreEqual(PersistentState.Hollow, _notifiedEvents[2].State);

            Assert.AreEqual(3, ent.Value);
            Assert.AreEqual(4, _notifiedEvents.Count);
            Assert.AreEqual(PersistentState.Latest, _notifiedEvents[3].State);

            _container1.Remove(ent);
            Assert.AreEqual(5, _notifiedEvents.Count);
            Assert.AreEqual(PersistentState.Removed, _notifiedEvents[4].State);
        }

        protected void HandlePersistentStateChanged(object sender, PersistentStateChangedEventArgs e) {
            _notifiedEvents.Add(e);
        }

        [Entity]
        public class Entity {
            private int _value;
            public virtual int Value {
                get { return _value; }
                set { _value = value; }
            }
        }
    }
}
