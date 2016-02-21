/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mkamo.Figure.Figures;
using Mkamo.Common.Externalize;
using System.Drawing;

namespace Mkamo.Figure.Core {
    [TestClass]
    public class FigurePersistTest {
        [TestMethod]
        public void TestNodeFigurePersist() {
            var persister = new Externalizer();
            var node = new SimpleRect();

            node.Bounds = new Rectangle(0, 0, 100, 100);
            IMemento mem = persister.Save(node);

            var onode = persister.Load(mem) as IConnectable;
            Assert.AreNotEqual(node, onode);
            Assert.AreEqual(new Rectangle(0, 0, 100, 100), onode.Bounds);
        }

        //[TestMethod]
        //public void TestEdgeFigurePersist() {
        //    var persister = new Externalizer();
        //    var edge = new LineEdge();

        //    edge.EdgePoints.Add(new Point(10, 10));
        //    edge.EdgePoints.Add(new Point(20, 20));
        //    edge.EdgePoints.Add(new Point(30, 30));
        //    IMemento mem = persister.Save(edge);

        //    var oedge = persister.Load(mem) as IEdge;
        //    Assert.AreNotEqual(edge, oedge);
        //    Assert.AreEqual(3, oedge.EdgePoints.Count);
        //    Assert.AreEqual(new Point(10, 10), edge.EdgePoints[0]);
        //    Assert.AreEqual(new Point(20, 20), edge.EdgePoints[1]);
        //    Assert.AreEqual(new Point(30, 30), edge.EdgePoints[2]);
        //}

        //[TestMethod]
        //public void TestNodeAndEdgePersist() {
        //    var persister = new Externalizer();
        //    var node1 = new Rect();
        //    var node2 = new Rect();
        //    var edge = new LineEdge();

        //    node1.Bounds = new Rectangle(10, 10, 10, 10);
        //    node2.Bounds = new Rectangle(20, 20, 20, 20);
        //    edge.EdgePoints.Add(new Point(10, 10));
        //    edge.EdgePoints.Add(new Point(20, 20));
        //    edge.Source = node1;
        //    edge.Target = node2;

        //    var mem = persister.Save(node1);

        //    var onode1 = persister.Load(mem) as IConnectable;
        //    Assert.AreNotEqual(node1, onode1);
        //    Assert.AreEqual(new Rectangle(10, 10, 10, 10), onode1.Bounds);
        //    Assert.AreEqual(1, onode1.Outgoings.Count);

        //    var oedge = onode1.Outgoings[0];
        //    Assert.AreNotEqual(edge, oedge);
        //    Assert.AreEqual(new Point(10, 10), (oedge as IEdge).EdgePoints[0]);

        //    var onode2 = oedge.Target;
        //    Assert.AreNotEqual(node2, onode2);
        //    Assert.AreEqual(new Rectangle(20, 20, 20, 20), onode2.Bounds);
        //}
    }
}
