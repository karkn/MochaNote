/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using Mkamo.Figure.Figures;

namespace Mkamo.Figure.Core {
    [TestClass]
    public class FigureTest {
        [TestMethod]
        public void TestFindFigure() {
            var content = new Layer();
            var fig1 = new SimpleRect();

            content.Bounds = new Rectangle(0, 0, 50, 50);
            fig1.Bounds = new Rectangle(0, 0, 100, 100);
            content.Children.Add(fig1);

            Assert.AreEqual(fig1, content.FindFigure(fig => fig.ContainsPoint(new Point(10, 10)), true));
            Assert.AreEqual(null, content.FindFigure(fig => fig.ContainsPoint(new Point(60, 60)), true));
        }

        [TestMethod]
        public void TestNodeToEdgeRelation() {
            var node = new SimpleRect();
            var edge = new LineEdge();

            Assert.AreEqual(0, node.Outgoings.Count);
            Assert.AreEqual(0, node.Incomings.Count);
            Assert.AreEqual(null, edge.Source);
            Assert.AreEqual(null, edge.Target);

            edge.Source = node;
            Assert.AreEqual(1, node.Outgoings.Count);
            Assert.AreEqual(node, edge.Source);

            node.Outgoings.Remove(edge);
            Assert.AreEqual(0, node.Outgoings.Count);
            Assert.AreEqual(null, edge.Source);

            node.Incomings.Add(edge);
            Assert.AreEqual(1, node.Incomings.Count);
            Assert.AreEqual(node, edge.Target);

            edge.Target = null;
            Assert.AreEqual(0, node.Outgoings.Count);
            Assert.AreEqual(null, edge.Source);
        }


        [TestMethod]
        public void TestClone() {
            var n1 = new SimpleRect();
            var n2 = new SimpleRect();
            var n3 = new SimpleRect();
            var n4 = new SimpleRect();
            var n5 = new SimpleRect();

            // n1
            //   n2
            //     n4
            //     n5
            //   n3
            n2.Parent = n1;
            n3.Parent = n1;
            n4.Parent = n2;
            n5.Parent = n2;

            n1.Location = new Point(1, 1);
            n2.Location = new Point(2, 2);
            n3.Location = new Point(3, 3);
            n4.Location = new Point(4, 4);
            n5.Location = new Point(5, 5);

            Assert.AreEqual(2, n2.Children.Count);
            
            var n2clone = n2.CloneFigure();
            Assert.AreNotEqual(n2, n2clone);
            Assert.AreEqual(new Point(2, 2), n2clone.Location);
            Assert.AreEqual(null, n2clone.Parent);
            Assert.AreEqual(2, n2clone.Children.Count);

            Assert.AreEqual(new Point(4, 4), n2clone.Children[0].Location);
            Assert.AreEqual(new Point(5, 5), n2clone.Children[1].Location);
            Assert.AreNotEqual(n4, n2clone.Children[0]);
            Assert.AreNotEqual(n5, n2clone.Children[1]);

            n2clone.Children[0].Location = new Point(40, 40);
            Assert.AreEqual(new Point(40, 40), n2clone.Children[0].Location);
            Assert.AreEqual(new Point(4, 4), n4.Location);
        }

    
        [TestMethod]
        public void TestBringTo() {
            var p = new SimpleRect();
            var c0 = new SimpleRect();
            var c1 = new SimpleRect();
            var c2 = new SimpleRect();

            c0.Parent = p;
            c1.Parent = p;
            c2.Parent = p;

            Assert.AreEqual(3, p.Children.Count);
            Assert.AreEqual(c0, p.Children[0]);
            Assert.AreEqual(c1, p.Children[1]);
            Assert.AreEqual(c2, p.Children[2]);


            c0.BringToFront(1);
            Assert.AreEqual(3, p.Children.Count);
            Assert.AreEqual(c1, p.Children[0]);
            Assert.AreEqual(c0, p.Children[1]);
            Assert.AreEqual(c2, p.Children[2]);

            c1.BringToFront(2);
            Assert.AreEqual(3, p.Children.Count);
            Assert.AreEqual(c0, p.Children[0]);
            Assert.AreEqual(c2, p.Children[1]);
            Assert.AreEqual(c1, p.Children[2]);

            c0.BringToFront(3);
            Assert.AreEqual(3, p.Children.Count);
            Assert.AreEqual(c2, p.Children[0]);
            Assert.AreEqual(c1, p.Children[1]);
            Assert.AreEqual(c0, p.Children[2]);

            c0.BringToBack(1);
            Assert.AreEqual(3, p.Children.Count);
            Assert.AreEqual(c2, p.Children[0]);
            Assert.AreEqual(c0, p.Children[1]);
            Assert.AreEqual(c1, p.Children[2]);

            c1.BringToBack(2);
            Assert.AreEqual(3, p.Children.Count);
            Assert.AreEqual(c1, p.Children[0]);
            Assert.AreEqual(c2, p.Children[1]);
            Assert.AreEqual(c0, p.Children[2]);

            c0.BringToBack(3);
            Assert.AreEqual(3, p.Children.Count);
            Assert.AreEqual(c0, p.Children[0]);
            Assert.AreEqual(c1, p.Children[1]);
            Assert.AreEqual(c2, p.Children[2]);

            c0.BringToFrontMost();
            Assert.AreEqual(3, p.Children.Count);
            Assert.AreEqual(c1, p.Children[0]);
            Assert.AreEqual(c2, p.Children[1]);
            Assert.AreEqual(c0, p.Children[2]);

            c0.BringToBackMost();
            Assert.AreEqual(3, p.Children.Count);
            Assert.AreEqual(c0, p.Children[0]);
            Assert.AreEqual(c1, p.Children[1]);
            Assert.AreEqual(c2, p.Children[2]);

        }

    }
}
