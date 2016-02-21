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
using System.Drawing;

namespace Mkamo.Figure.Core {
    [TestClass]
    public class EdgeTest {
        [TestMethod]
        public void TestMoveEdge() {
            var edge = new LineEdge();

            edge.First = new Point(100, 100);
            edge.Last = new Point(200, 200);
            Assert.AreEqual(new Rectangle(new Point(100, 100), new Size(100, 100)), edge.Bounds);

            edge.Location = new Point(300, 300);
            Assert.AreEqual(new Rectangle(new Point(300, 300), new Size(100, 100)), edge.Bounds);
        }

    }
}
