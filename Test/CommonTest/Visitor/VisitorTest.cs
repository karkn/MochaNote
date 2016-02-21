/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mkamo.Common.Visitor {
    [TestClass]
    public class VisitorTest {
        [TestMethod]
        public void TestVisitor() {
            Foo root = new Foo(0);
            Foo foo1 = new Foo(1);
            Foo foo2 = new Foo(2);
            Foo foo3 = new Foo(3);
            Foo foo4 = new Foo(4);
            Foo foo5 = new Foo(5);

            // root
            //   foo1
            //     foo3
            //     foo4
            //       foo5
            //   foo2
            root.Children.Add(foo1);
            root.Children.Add(foo2);
            foo1.Children.Add(foo3);
            foo1.Children.Add(foo4);
            foo4.Children.Add(foo5);

            ResultCollectingVisitor visitor = new ResultCollectingVisitor();
            root.Accept(visitor, NextVisitOrder.PositiveOrder);

            Result[] results = new Result[] {
                new Result() { Foo = root, IsEnd = false},
                new Result() { Foo = foo1, IsEnd = false},
                new Result() { Foo = foo3, IsEnd = false},
                new Result() { Foo = foo3, IsEnd = true},
                new Result() { Foo = foo4, IsEnd = false},
                new Result() { Foo = foo5, IsEnd = false},
                new Result() { Foo = foo5, IsEnd = true},
                new Result() { Foo = foo4, IsEnd = true},
                new Result() { Foo = foo1, IsEnd = true},
                new Result() { Foo = foo2, IsEnd = false},
                new Result() { Foo = foo2, IsEnd = true},
                new Result() { Foo = root, IsEnd = true},
            };

            for (int i = 0; i < visitor.Result.Count; ++i) {
                Assert.AreEqual(results[i], visitor.Result[i]);
            }
        }

        public struct Result {
            public Foo Foo;
            public bool IsEnd;
        }

        public class ResultCollectingVisitor: IVisitor<Foo> {
            public List<Result> Result = new List<Result>();

            public bool Visit(Foo elem) {
                Result.Add(new Result() { Foo = elem, IsEnd = false});
                return false;
            }
            public void EndVisit(Foo elem) {
                Result.Add(new Result() { Foo = elem, IsEnd = true});
            }
        }

        public class Foo: IVisitable<Foo> {
            private int _value;
            private List<Foo> _children;
            private VisitableSupport<Foo> _fooVisitable;

            public Foo(int value) {
                _value = value;
                _children = new List<Foo>();
                _fooVisitable = new VisitableSupport<Foo>(this, _children);
            }

            public IList<Foo> Children {
                get { return _children; }
            }

            public int Value {
                get { return _value; }
            }

            public void Accept(IVisitor<Foo> visitor, NextVisitOrder order) {
                _fooVisitable.Accept(visitor, order);
            }

            public void Accept(IVisitor<Foo> visitor) {
                _fooVisitable.Accept(visitor);
            }

            public void Accept(Predicate<Foo> visitPred) {
                _fooVisitable.Accept(visitPred);
            }

            public void Accept(Predicate<Foo> visitPred, Action<Foo> endVisitAction, NextVisitOrder order) {
                _fooVisitable.Accept(visitPred, endVisitAction, order);
            }
        }
    }
}
