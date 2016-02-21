/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Mkamo.Common.Externalize {
    [TestClass]
    public class PropertyExternalizableSupportTest {
        [TestMethod]
        public void TestExternalize() {

            var stream = new MemoryStream();
            {
                var foo = new Foo();
                var bar = new Bar();

                foo.Value = 5;

                bar.Value = 10;
                foo.Bar = bar;

                foo.Bars.Add(new Bar() { Value = 9, });
                foo.Bars.Add(new Bar() { Value = 99, });
                foo.Bars.Add(new Bar() { Value = 999, });

                var externalizer = new Externalizer();
                var mem = externalizer.Save(foo);

                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, mem);
            }

            stream.Position = 0;
            {
                var formatter = new BinaryFormatter();
                var mem = formatter.Deserialize(stream) as IMemento;

                var externalizer = new Externalizer();
                var foo = externalizer.Load(mem) as Foo;
                Assert.AreEqual(5, foo.Value);
                Assert.AreEqual(10, foo.Bar.Value);
                Assert.AreEqual(3, foo.Bars.Count);
                Assert.AreEqual(9, foo.Bars[0].Value);
                Assert.AreEqual(99, foo.Bars[1].Value);
                Assert.AreEqual(999, foo.Bars[2].Value);
            }

        }

        [Externalizable]
        public class Foo {
            private int _value;
            private Bar _bar;
            private List<Bar> _bars = new List<Bar>();
    
            [External]
            public int Value {
                get { return _value; }
                set { _value = value; }
            }
            [External]
            public Bar Bar {
                get { return _bar; }
                set { _bar = value; }
            }
            [External]
            public List<Bar> Bars {
                get { return _bars; }
            }

        }
    
        [Externalizable]
        public class Bar {
            private int _value;
    
            [External]
            public int Value {
                get { return _value; }
                set { _value = value; }
            }
        }
    }

}
