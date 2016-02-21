/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mkamo.Figure.Core;
using Mkamo.Figure.Figures;

namespace Mkamo.Figure.Core {
    [TestClass]
    public class FigureStructureTest {
        [TestMethod]
        public void TestNode() {
            var fig1 = new SimpleRect();
            var fig2 = new SimpleRect();
            var fig3 = new SimpleRect();
            TestStructure(fig1, fig2, fig3);
        }

        [TestMethod]
        public void TestEdge() {
            var fig1 = new LineEdge();
            var fig2 = new LineEdge();
            var fig3 = new LineEdge();
            TestStructure(fig1, fig2, fig3);
        }

        [TestMethod]
        public void TestNodeAndEdgeAndGroup() {
            TestStructure(new SimpleRect(), new LineEdge(), new FigureGroup());
            TestStructure(new SimpleRect(), new FigureGroup(), new LineEdge());
            TestStructure(new LineEdge(), new SimpleRect(), new FigureGroup());
            TestStructure(new LineEdge(), new FigureGroup(), new SimpleRect());
            TestStructure(new FigureGroup(), new SimpleRect(), new LineEdge());
            TestStructure(new FigureGroup(), new LineEdge(), new SimpleRect());
        }


        protected void TestStructure(IFigure fig1, IFigure fig2, IFigure fig3) {
            Assert.AreEqual(null, fig1.Parent);
            Assert.AreEqual(null, fig2.Parent);
            Assert.AreEqual(null, fig3.Parent);
            Assert.AreEqual(0, fig1.Children.Count);
            Assert.AreEqual(0, fig2.Children.Count);
            Assert.AreEqual(0, fig3.Children.Count);

            fig1.Children.Add(fig2);
            Assert.AreEqual(null, fig1.Parent);
            Assert.AreEqual(fig1, fig2.Parent);
            Assert.AreEqual(null, fig3.Parent);
            Assert.AreEqual(1, fig1.Children.Count);
            Assert.AreEqual(0, fig2.Children.Count);
            Assert.AreEqual(0, fig3.Children.Count);

            fig2.Parent = fig3;
            Assert.AreEqual(null, fig1.Parent);
            Assert.AreEqual(fig3, fig2.Parent);
            Assert.AreEqual(null, fig3.Parent);
            Assert.AreEqual(0, fig1.Children.Count);
            Assert.AreEqual(0, fig2.Children.Count);
            Assert.AreEqual(1, fig3.Children.Count);

            fig2.Parent = fig1;
            fig1.Children.Add(fig3);
            Assert.AreEqual(null, fig1.Parent);
            Assert.AreEqual(fig1, fig2.Parent);
            Assert.AreEqual(fig1, fig3.Parent);
            Assert.AreEqual(2, fig1.Children.Count);
            Assert.AreEqual(0, fig2.Children.Count);
            Assert.AreEqual(0, fig3.Children.Count);

            fig2.Parent = null;
            fig1.Children.Remove(fig3);
            Assert.AreEqual(null, fig1.Parent);
            Assert.AreEqual(null, fig2.Parent);
            Assert.AreEqual(null, fig3.Parent);
            Assert.AreEqual(0, fig1.Children.Count);
            Assert.AreEqual(0, fig2.Children.Count);
            Assert.AreEqual(0, fig3.Children.Count);
        }
    }
}
