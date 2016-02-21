/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mkamo.Container.Internal.Core;
using Mkamo.Container.Core;
using System.Drawing;

namespace Mkamo.Container.Core {
    [TestClass]
    public class PersistTypeConverterTest {
        IEntityContainer _container1;
        IEntityContainer _container2;

        [TestInitialize]
        public void Setup() {
            _container1 = new EntityContainer(new XmlFileEntityStore(@"d:\tmp\_store"));
            _container2 = new EntityContainer(new XmlFileEntityStore(@"d:\tmp\_store"));
        }

        [TestMethod]
        public void TestEntityHasColor() {
            EntityHasColor e = _container1.Create<EntityHasColor>();
            string id = _container1.GetId(e);
            e.Color = Color.Blue;
            _container1.Commit();

            e = _container2.Find<EntityHasColor>(id);
            Assert.IsTrue(e.Color == Color.Blue);
        }

        [Entity]
        public class EntityHasColor {
            private Color _color;
    
            [Persist]
            public virtual Color Color {
                get { return _color; }
                set { _color = value; }
            }
        }

    }
}
