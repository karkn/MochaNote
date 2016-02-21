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
    using FooProxy = ExternalizableProxy<ExternalizeProxyTest.Foo>;
    using BarProxy = ExternalizableProxy<ExternalizeProxyTest.Bar>;

    [TestClass]
    public class ExternalizeProxyTest {
        [TestMethod]
        public void TestExternalize() {

            var stream = new MemoryStream();
            {
                var foo = new Foo();
                var bar = new Bar();

                foo.Value = 5;
                bar.Value = 10;
                foo.Bar = bar;

                var externalizer = new Externalizer();
                var proxy = new FooProxy(foo, SaveFoo, LoadFoo);
                var mem = externalizer.Save(proxy);

                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, mem);
            }

            stream.Position = 0;
            {
                var formatter = new BinaryFormatter();
                var mem = formatter.Deserialize(stream) as IMemento;

                var externalizer = new Externalizer();
                var proxy = externalizer.Load(mem) as FooProxy;

                var foo = proxy.Real;
                Assert.AreEqual(5, foo.Value);
                Assert.AreEqual(10, foo.Bar.Value);
            }

        }

        public static void SaveFoo(Foo foo, IMemento mem, ExternalizeContext context) {
            mem.WriteInt("Value", foo.Value);
            mem.WriteExternalizable(
                "Bar",
                new BarProxy(foo.Bar, SaveBar, LoadBar)
            );
        }

        public static Foo LoadFoo(IMemento mem, ExternalizeContext context) {
            var ret = new Foo();
            ret.Value = mem.ReadInt("Value");
            var barProxy = mem.ReadExternalizable("Bar") as BarProxy;
            ret.Bar = barProxy.Real;
            return ret;
        }

        public static void SaveBar(Bar bar, IMemento mem, ExternalizeContext context) {
            mem.WriteInt("Value", bar.Value);
        }

        public static Bar LoadBar(IMemento mem, ExternalizeContext context) {
            var ret = new Bar();
            ret.Value = mem.ReadInt("Value");
            return ret;
        }


        public class Foo {
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

        }
    
        public class Bar {
            private int _value;
    
            public int Value {
                get { return _value; }
                set { _value = value; }
            }
        }
    }

}
