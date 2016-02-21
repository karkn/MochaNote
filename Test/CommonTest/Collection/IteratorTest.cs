/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mkamo.Common.Collection {
    /// <summary>
    /// IteratorTest の概要の説明
    /// </summary>
    [TestClass]
    public class IteratorTest {
        public IteratorTest() {
        }


        [TestMethod]
        public void TestGetEnumerator() {
            var item1 = new Item(1);
            var item2 = new Item(2);
            var item3 = new Item(3);
            var item4 = new Item(4);
            var item5 = new Item(5);

            item1.Items.Add(item2);
            item2.Items.Add(item3);
            item2.Items.Add(item4);
            item1.Items.Add(item5);
            /// 1
            ///   2
            ///     3
            ///     4
            ///   5
            {
                var ite = new Iterator<Item>(item1, item => item.Items);
                var i = 0;
                foreach (var item in ite) {
                    Assert.AreEqual(i + 1, item.Value);
                    ++i;
                }
                Assert.AreEqual(5, i);
            }

            {
                var ite = new Iterator<Item>(item2, item => item.Items);
                var i = 0;
                foreach (var item in ite) {
                    Assert.AreEqual(i + 2, item.Value);
                    ++i;
                }
                Assert.AreEqual(3, i);
            }
        }


        private class Item {
            private int _value;
            private List<Item> _items;

            public Item(int value) {
                _value = value;
                _items = new List<Item>();
            }

            public List<Item> Items {
                get { return _items; }
                set { _items = value; }
            }
            public int Value {
                get { return _value; }
                set { _value = value; }
            }
        }
    }
}
