/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mkamo.Common.Externalize {
    [TestClass]
    public class ExternalizeTest {
        [TestMethod]
        public void TestExternalize() {
            var persister = new Externalizer();
            var foo = new Foo();
            var bar = new Bar();

            foo.Value = 5;
            bar.Value = 10;
            foo.Bar = bar;
            bar.Foo = foo;

            var memento = persister.Save(foo);
            var ofoo = persister.Load(memento) as Foo;

            Assert.AreNotEqual(foo, ofoo);
            Assert.AreNotEqual(bar, ofoo.Bar);
            Assert.AreEqual(5, ofoo.Value);
            Assert.AreEqual(10, ofoo.Bar.Value);
        }

        [TestMethod]
        public void TestExternalizeFilter() {
            var persister = new Externalizer();
            var foo = new Foo();
            var bar = new Bar();

            foo.Value = 5;
            bar.Value = 10;
            foo.Bar = bar;
            bar.Foo = foo;

            var memento = persister.Save(foo, (key, ex) => key != "Bar"); 
            var ofoo = persister.Load(memento) as Foo;

            Assert.AreNotEqual(foo, ofoo);
            Assert.AreEqual(null, ofoo.Bar);
            Assert.AreEqual(5, ofoo.Value);
        }

    
        public class Foo: IExternalizable {
            private int _value;
            private Bar _bar;
    
            public int Value {
                get { return _value; }
                set { _value = value; }
            }
            public Bar Bar {
                get { return _bar; }
                set { _bar = value; }
            }
    
            public virtual void WriteExternal(IMemento memento, ExternalizeContext context) {
                memento.WriteInt("Value", _value);
                memento.WriteExternalizable("Bar", Bar);
            }
    
            public virtual void ReadExternal(IMemento memento, ExternalizeContext context) {
                _value = memento.ReadInt("Value");
                _bar = memento.ReadExternalizable("Bar") as Bar;
            }
    
        }
    
        public class Bar: IExternalizable {
            private int _value;
            private Foo _foo;
    
            public int Value {
                get { return _value; }
                set { _value = value; }
            }
            public Foo Foo {
                get { return _foo; }
                set { _foo = value; }
            }
    
            public virtual void WriteExternal(IMemento memento, ExternalizeContext context) {
                memento.WriteInt("Value", _value);
                memento.WriteExternalizable("Foo", _foo);
            }
    
            public virtual void ReadExternal(IMemento memento, ExternalizeContext context) {
                _value = memento.ReadInt("Value");
                _foo = memento.ReadExternalizable("Foo") as Foo;
            }
        }
    }

}
