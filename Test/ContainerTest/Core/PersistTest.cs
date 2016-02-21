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

namespace Mkamo.Container.Core {
    [TestClass]
    public class PersistTest {
        IEntityContainer _container1;
        IEntityContainer _container2;

        [TestInitialize]
        public void Setup() {
            _container1 = new EntityContainer(new XmlFileEntityStore(@"d:\tmp\_store"));
            _container2 = new EntityContainer(new XmlFileEntityStore(@"d:\tmp\_store"));
        }

        [TestMethod]
        public void TestEntitySave() {
            Foo foo = _container1.Create<Foo>();
            Bar bar = _container1.Create<Bar>();
            string fooId = _container1.GetId(foo);
            string barId = _container1.GetId(bar);

            bar.Value = 3;
            foo.BarValue1 = bar;
            //foo.BarValue1 = null;
            foo.BarValue2 = new Bar() { Value = 5 };

            foo.BoolValue = true;
            foo.IntValue = 5000;
            foo.FloatValue = 1.25f;
            foo.ByteValue = 100;
            foo.StringValue = "ほげほげ";
            //foo.StringValue = "";
            foo.IntArrayValue = new int[] { 1, 3, 5, };

            foo.LongColValue.Add(5);
            foo.LongColValue.Add(10);
            foo.LongColValue.Add(15);
            foo.LongColValue.Add(20);

            _container1.Commit();

            foo = _container2.Find<Foo>(fooId);
            Assert.AreEqual(3, foo.BarValue1.Value);
            //Assert.AreEqual(null, foo.BarValue1);
            Assert.AreEqual(5, foo.BarValue2.Value);
            Assert.AreEqual(true, foo.BoolValue);
            Assert.AreEqual(5000, foo.IntValue);
            Assert.AreEqual(1.25f, foo.FloatValue);
            Assert.AreEqual(100, foo.ByteValue);
            Assert.AreEqual("ほげほげ", foo.StringValue);
            //Assert.AreEqual("", foo.StringValue);
            CollectionAssert.AreEqual(new int[] { 1, 3, 5, }, foo.IntArrayValue);
            Assert.AreEqual(4, foo.LongColValue.Count);

            _container2.Rollback();

            foo = _container2.Find<Foo>(fooId);
            bar = _container2.Find<Bar>(barId);
            _container2.Remove(foo);
            _container2.Remove(bar);
            _container2.Commit();
        }

        [TestMethod]
        public void TestEntityHasNormalClass() {
            EntityHasNormalClass e = _container1.Create<EntityHasNormalClass>();
            string id = _container1.GetId(e);
            e.BarValue = new Bar() { Value = 5 };
            _container1.Commit();

            e = _container2.Find<EntityHasNormalClass>(id);
            Assert.AreEqual(PersistentState.Hollow, _container1.AsEntity(e).State);
            Assert.AreEqual(5, e.BarValue.Value);
            Assert.AreEqual(PersistentState.Latest, _container1.AsEntity(e).State);
       }

        [TestMethod]
        public void TestEntityHasCascadeClass() {
            var e = _container1.Create<EntityHasCascade>();
            var id = _container1.GetId(e);
            var bar = _container1.Create<Bar>();
            var barId = _container1.GetId(bar);

            bar.Value = 5;
            e.BarValue = bar;

            Assert.AreEqual(true, _container1.IsExist<EntityHasCascade>(id));
            Assert.AreEqual(true, _container1.IsExist<Bar>(barId));
            Assert.AreEqual(e, _container1.Find<EntityHasCascade>(id));
            Assert.AreEqual(bar, _container1.Find<Bar>(barId));

            _container1.Commit();

            Assert.AreEqual(true, _container1.IsExist<EntityHasCascade>(id));
            Assert.AreEqual(true, _container1.IsExist<Bar>(barId));
            Assert.AreEqual(e, _container1.Find<EntityHasCascade>(id));
            Assert.AreEqual(bar, _container1.Find<Bar>(barId));
            
            _container1.Remove(e);

            Assert.AreEqual(false, _container1.IsExist<EntityHasCascade>(id));
            Assert.AreEqual(false, _container1.IsExist<Bar>(barId));
            Assert.AreEqual(null, _container1.Find<EntityHasCascade>(id));
            Assert.AreEqual(null, _container1.Find<Bar>(barId));

            _container1.Commit();

            Assert.AreEqual(false, _container1.IsExist<EntityHasCascade>(id));
            Assert.AreEqual(false, _container1.IsExist<Bar>(barId));
            Assert.AreEqual(null, _container1.Find<EntityHasCascade>(id));
            Assert.AreEqual(null, _container1.Find<Bar>(barId));

       }

        [TestMethod]
        public void TestEntityHasSerializableClass() {
            var e = _container1.Create<EntityHasSerializableClass>();
            var id = _container1.GetId(e);
            e.Ser = new SerializableClass() { Value = 9 };
            _container1.Commit();

            e = _container2.Find<EntityHasSerializableClass>(id);
            Assert.AreEqual(PersistentState.Hollow, _container1.AsEntity(e).State);
            Assert.AreEqual(9, e.Ser.Value);
            Assert.AreEqual(PersistentState.Latest, _container1.AsEntity(e).State);
       }

       // [TestMethod]
       // public void TestLoadIllegalXMLOfEntityHasNormalClass() {
       //     string id = "eb0151ea-2dac-4372-853c-df800122f797";
       //     EntityContainer container = new EntityContainer(new XmlEntityStore(@"d:\tmp\_store"));
       //     EntityHasNormalClass e = container.Find(typeof(EntityHasNormalClass), id) as EntityHasNormalClass;
       //     Assert.AreEqual(PersistentState.Hollow, (e as IEntityProxy)._IEntityProxy_State);
       //     Assert.AreEqual(null, e.BarValue);
       //     Assert.AreEqual(PersistentState.Latest, (e as IEntityProxy)._IEntityProxy_State);
       //}

        [TestMethod]
        public void TestEntityHasNormalClassNull() {
            EntityHasNormalClass e = _container1.Create<EntityHasNormalClass>();
            string id = _container1.GetId(e);
            _container1.Commit();

            e = _container2.Find<EntityHasNormalClass>(id);
            Assert.IsNull(e.BarValue);
        }

        [TestMethod]
        public void TestEntityHasSaveMethod() {
            EntityHasSaveMethod e = _container1.Create<EntityHasSaveMethod>();
            string id = _container1.GetId(e);
            e.IntValue = 3;
            e.BarValue = new Bar() { Value = 5 };
            _container1.Commit();

            e = _container2.Find<EntityHasSaveMethod>(id);
            Assert.AreEqual(5, e.BarValue.Value);
            Assert.AreEqual(3, e.IntValue);
       }

        [TestMethod]
        public void TestEntityHasIListOfNormalClass() {
            EntityHasIListOfNormalClass e = _container1.Create<EntityHasIListOfNormalClass>();
            string id = _container1.GetId(e);
            for (int i = 0; i < 10; ++i) {
                e.Bars.Add(new Bar() { Value = i });
            }
            _container1.Commit();

            e = _container2.Find<EntityHasIListOfNormalClass>(id);
            Assert.AreEqual(PersistentState.Hollow, _container1.AsEntity(e).State);
            Assert.AreEqual(10, e.Bars.Count);
            Assert.AreEqual(PersistentState.Latest, _container1.AsEntity(e).State);
            e.Bars.Add(new Bar());
            Assert.AreEqual(PersistentState.Updated, _container1.AsEntity(e).State);
        }

        [TestMethod]
        public void TestEntityHasIDictionaryOfNormalClass() {
            EntityHasIDictionaryOfNormalClass e = _container1.Create<EntityHasIDictionaryOfNormalClass>();
            string id = _container1.GetId(e);
            for (int i = 0; i < 10; ++i) {
                e.IntToBar[i] = new Bar() { Value = i * 10 };
            }
            _container1.Commit();

            e = _container2.Find<EntityHasIDictionaryOfNormalClass>(id);
            Assert.AreEqual(10, e.IntToBar.Count);
            Assert.AreEqual(0, e.IntToBar[0].Value);
            Assert.AreEqual(10, e.IntToBar[1].Value);
            Assert.AreEqual(20, e.IntToBar[2].Value);
            Assert.AreEqual(90, e.IntToBar[9].Value);
        }

        [TestMethod]
        public void TestEntityHasArrayOfNormalClass() {
            EntityHasArrayOfNormalClass e = _container1.Create<EntityHasArrayOfNormalClass>();
            string id = _container1.GetId(e);
            Bar[] bars = new Bar[10];
            for (int i = 0; i < 10; ++i) {
                bars[i] = new Bar();
                bars[i].Value = i;
            }
            e.Bars = bars;
            _container1.Commit();

            e = _container2.Find<EntityHasArrayOfNormalClass>(id);
            Assert.AreEqual(10, e.Bars.Length);
        }

        [TestMethod]
        public void TestEntityHasArrayOfNormalClassProxy() {
            EntityHasArrayOfNormalClass e = _container1.Create<EntityHasArrayOfNormalClass>();
            string id = _container1.GetId(e);
            Bar[] bars = new Bar[10];
            for (int i = 0; i < 10; ++i) {
                bars[i] = _container1.Create<Bar>();
                bars[i].Value = i;
            }
            e.Bars = bars;
            _container1.Commit();

            e = _container2.Find<EntityHasArrayOfNormalClass>(id);
            Assert.AreEqual(10, e.Bars.Length);
            for (int i = 0; i < 10; ++i) {
                Assert.AreEqual(i, e.Bars[i].Value);
            }
        }

        [TestMethod]
        public void TestEntityHasEnum() {
            EntityHasEnum e = _container1.Create<EntityHasEnum>();
            string id = _container1.GetId(e);
            e.Number = NumberEnum.Two;
            _container1.Commit();

            e = _container2.Find<EntityHasEnum>(id);
            Assert.AreEqual(NumberEnum.Two, e.Number);
        }
    
        //[TestMethod]
        //public void TestNullReplacementWhenProxyRemoved() {
        //    EntityHasNormalClass ent = _container1.Create<EntityHasNormalClass>();
        //    Bar bar = _container1.Create<Bar>();
        //    string id = _container1.GetId(ent);

        //    ent.BarValue = bar;
        //    _container1.Commit();

        //    ent = _container2.Find<EntityHasNormalClass>(id);
        //    Assert.IsNotNull(ent.BarValue);
        //    _container2.Remove(ent.BarValue);
        //    Assert.IsNull(ent.BarValue);

        //    _container2.Commit();
        //    Assert.IsNull(ent.BarValue);

        //    bar = _container2.Create<Bar>();
        //    ent.BarValue = bar;
        //    Assert.AreEqual(bar, ent.BarValue);
        //    _container2.Remove(bar);
        //    Assert.IsNull(ent.BarValue);

        //    _container2.Commit();
        //    Assert.IsNull(ent.BarValue);
        //}

        [TestMethod]
        public void TestRollback() {
            EntityHasNormalClass ent = _container1.Create<EntityHasNormalClass>();
            Bar bar = _container1.Create<Bar>();
            
            ent.BarValue = bar;
            bar.Value = 1;
            _container1.Commit();

            Assert.AreEqual(bar, ent.BarValue);
            Assert.AreEqual(1, bar.Value);

            ent.BarValue = null;
            bar.Value = 3;

            Assert.AreEqual(null, ent.BarValue);
            Assert.AreEqual(3, bar.Value);

            _container1.Rollback();

            Assert.AreEqual(bar, ent.BarValue);
            Assert.AreEqual(1, bar.Value);
        }

        [Entity]
        public class EntityHasNormalClass {
            private Bar _bar;
    
            [Persist]
            public virtual Bar BarValue {
                get { return _bar; }
                set { _bar = value; }
            }
        }
    
        [Entity]
        public class EntityHasCascade {
            private Bar _bar;
    
            [Persist(Cascade=true)]
            public virtual Bar BarValue {
                get { return _bar; }
                set { _bar = value; }
            }
        }
    
        [Entity]
        public class EntityHasSerializableClass {
            private SerializableClass _ser;
    
            [Persist]
            public virtual SerializableClass Ser {
                get { return _ser; }
                set { _ser = value; }
            }
        }
    
        [Entity(Save="Save", Load="Load")]
        public class EntityHasSaveMethod {
            private int _intValue;
            private Bar _bar;

            [Persist]
            public virtual int IntValue {
                get { return _intValue; }
                set { _intValue = value; }
            }
            public virtual Bar BarValue {
                get { return _bar; }
                set { _bar = value; }
            }

            public virtual void Save(IDictionary<string, string> values) {
                values["BarValue"] = _bar.Value.ToString();
            }
            public virtual void Load(IDictionary<string, string> values) {
                _bar = new Bar() { Value = int.Parse(values["BarValue"]) };
            }
        }
    
    
        [Entity]
        public class EntityHasArrayOfNormalClass {
            private Bar[] _bars;
    
            [Persist]
            public virtual Bar[] Bars {
                get { return _bars; }
                set { _bars = value; }
            }
        }
    
        [Entity]
        public class EntityHasIListOfNormalClass {
            private IList<Bar> _bars;
    
            public EntityHasIListOfNormalClass() {
                _bars = new List<Bar>();
            }
    
            [Persist]
            public virtual IList<Bar> Bars {
                get { return _bars; }
            }
        }
    
        [Entity]
        public class EntityHasIDictionaryOfNormalClass {
            private IDictionary<int, Bar> _bars;
    
            public EntityHasIDictionaryOfNormalClass() {
                _bars = new Dictionary<int, Bar>();
            }
    
            [Persist]
            public virtual IDictionary<int, Bar> IntToBar {
                get { return _bars; }
            }
        }
    
        public enum NumberEnum {
            One,
            Two,
            Three
        }
    
        [Entity]
        public class EntityHasEnum {
            private NumberEnum _number;
            [Persist]
            public virtual NumberEnum Number {
                get { return _number; }
                set { _number = value; }
            }
        }
    
        [Entity]
        public class Foo {
            private bool _boolValue;
            private int _intValue;
            private float _floatValue;
            private byte _byteValue;
            private string _stringValue;
    
            private int[] _intArrayValue;
            private List<long> _longColValue;
    
            private Bar _barValue1;
            private Bar _barValue2;
    
            private NumberEnum _enumValue;
 
            public Foo() {
                _longColValue = new List<long>();
            }
    
            [Persist]
            public virtual Bar BarValue1 {
                get { return _barValue1; }
                set { _barValue1 = value; }
            }

            [Persist]
            public virtual Bar BarValue2 {
                get { return _barValue2; }
                set { _barValue2 = value; }
            }

            [Persist]
            public virtual NumberEnum EnumValue {
                get { return _enumValue; }
                set { _enumValue = value; }
            }
 
            [Persist]
            public virtual bool BoolValue {
                get { return _boolValue; }
                set { _boolValue = value; }
            }
            [Persist]
            public virtual int IntValue {
                get { return _intValue; }
                set { _intValue = value; }
            }
            [Persist]
            public virtual float FloatValue {
                get { return _floatValue; }
                set { _floatValue = value; }
            }
            [Persist]
            public virtual byte ByteValue {
                get { return _byteValue; }
                set { _byteValue = value; }
            }
            [Persist]
            public virtual string StringValue {
                get { return _stringValue; }
                set { _stringValue = value; }
            }
    
            [Persist]
            public virtual int[] IntArrayValue {
                get { return _intArrayValue; }
                set { _intArrayValue = value; }
            }
    
            [Persist]
            public virtual IList<long> LongColValue {
                get { return _longColValue; }
            }
    
    
        }
    
        [Entity]
        public class Bar {
            private int _value;
            [Persist]
            public virtual int Value {
                get { return _value; }
                set { _value = value; }
            }
        }

        [Serializable]
        public class SerializableClass {
            private int _value;
            public virtual int Value {
                get { return _value; }
                set { _value = value; }
            }
        }
    }
}
