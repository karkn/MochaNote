/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Mkamo.Common.Structure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mkamo.Common.Visitor;
using System.Collections.ObjectModel;

namespace Mkamo.Common.Structure {
    [TestClass]
    public class CompositeTest {
        private Tree _tree;

        [TestInitialize]
        public void SetUp() {
            // 1+-2+-4+-6
            //  |  +-5
            //  +-3+-7
            //     +-8+-9

            Tree t1 = new Tree(1);
            Tree t2 = new Tree(2);
            Tree t3 = new Tree(3);
            Tree t4 = new Tree(4);
            Tree t5 = new Tree(5);
            Tree t6 = new Tree(6);
            Tree t7 = new Tree(7);
            Tree t8 = new Tree(8);
            Tree t9 = new Tree(9);

            t1.Children.Add(t2);
            t1.Children.Add(t3);

            t2.Children.Add(t4);
            t2.Children.Add(t5);

            t4.Children.Add(t6);

            t3.Children.Add(t7);
            t3.Children.Add(t8);
            t8.Children.Add(t9);

            _tree = t1;
        }

        [TestMethod]
        public void TestParentAndChildren() {
            var t1 = new Tree(1);
            var t2 = new Tree(2);

            Assert.AreEqual(null, t1.Parent);
            Assert.AreEqual(null, t2.Parent);
            Assert.AreEqual(0, t1.Children.Count);
            Assert.AreEqual(0, t2.Children.Count);

            t1.Children.Add(t2);
            Assert.AreEqual(null, t1.Parent);
            Assert.AreEqual(t1, t2.Parent);
            Assert.AreEqual(1, t1.Children.Count);
            Assert.AreEqual(0, t2.Children.Count);

            t2.Parent = null;
            Assert.AreEqual(null, t1.Parent);
            Assert.AreEqual(null, t2.Parent);
            Assert.AreEqual(0, t1.Children.Count);
            Assert.AreEqual(0, t2.Children.Count);
        }

        [TestMethod]
        public void TestAcceptBeforePositive() {
            List<int> ret = new List<int>();
            _tree.Accept(
                elem => { ret.Add(elem.Value); return false; },
                null,
                NextVisitOrder.PositiveOrder
            );
            CollectionAssert.AreEqual(
                new int[] { 1, 2, 4, 6, 5, 3, 7, 8, 9 },
                ret.ToArray()
            );
        }

        [TestMethod]
        public void TestAcceptBeforeNegative() {
            List<int> ret = new List<int>();
            _tree.Accept(
                elem => { ret.Add(elem.Value); return false; },
                null,
                NextVisitOrder.NegativeOrder
            );
            CollectionAssert.AreEqual(
                new int[] { 1, 3, 8, 9, 7, 2, 5, 4, 6 },
                ret.ToArray()
            );
        }

        [TestMethod]
        public void TestAcceptAfterPositive() {
            List<int> ret = new List<int>();
            _tree.Accept(
                null,
                elem => ret.Add(elem.Value),
                NextVisitOrder.PositiveOrder
            );
            CollectionAssert.AreEqual(
                new int[] { 6, 4, 5, 2, 7, 9, 8, 3, 1 },
                ret.ToArray()
            );
        }

        [TestMethod]
        public void TestAcceptAfterNegative() {
            List<int> ret = new List<int>();
            _tree.Accept(
                null,
                elem => ret.Add(elem.Value),
                NextVisitOrder.NegativeOrder
            );
            CollectionAssert.AreEqual(
                new int[] { 9, 8, 7, 3, 5, 6, 4, 2, 1 },
                ret.ToArray()
            );
        }


        [TestMethod]
        public void TestAcceptBeforePositiveStop() {
            List<int> ret = new List<int>();
            _tree.Accept(
                elem => { ret.Add(elem.Value); return elem.Value == 3; },
                null,
                NextVisitOrder.PositiveOrder
            );
            CollectionAssert.AreNotEqual(
                new int[] { 1, 2, 4, 6, 5, 3, 7, 8, 9 },
                ret.ToArray()
            );
            CollectionAssert.AreEqual(
                new int[] { 1, 2, 4, 6, 5, 3 },
                ret.ToArray()
            );
        }

        [TestMethod]
        public void TestAcceptBeforeNegativeStop() {
            List<int> ret = new List<int>();
            _tree.Accept(
                elem => { ret.Add(elem.Value); return elem.Value == 2; },
                null,
                NextVisitOrder.NegativeOrder
            );
            CollectionAssert.AreNotEqual(
                new int[] { 1, 3, 8, 9, 7, 2, 5, 4, 6 },
                ret.ToArray()
            );
            CollectionAssert.AreEqual(
                new int[] { 1, 3, 8, 9, 7, 2, },
                ret.ToArray()
            );
        }
        private class Tree: IComposite<Tree, Tree>, IVisitable<Tree> {
            private CompositeSupport<Tree, Tree> _str;
            private int _value;

            public Tree(int value) {
                _value = value;
                _str = new CompositeSupport<Tree, Tree>(this);
            }

            public int Value {
                get { return _value; }
            }

            public Tree Parent {
                get { return _str.Parent; }
                set { _str.Parent = value; }
            }

            public Collection<Tree> Children {
                get { return _str.Children; }
            }

            public void Accept(IVisitor<Tree> visitor) {
                _str.Accept(visitor);
            }

            void IVisitable<Tree>.Accept(IVisitor<Tree> visitor, NextVisitOrder order) {
                _str.Accept(visitor, order);
            }

            void IVisitable<Tree>.Accept(Predicate<Tree> visitPred) {
                _str.Accept(visitPred);
            }

            public void Accept(Predicate<Tree> visitPred, Action<Tree> endVisitAction, NextVisitOrder order) {
                _str.Accept(visitPred, endVisitAction, order);
            }
        }
    }

}
