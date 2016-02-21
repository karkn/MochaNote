/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mkamo.Common.Forms.Drawing {
    [TestClass]
    public class PointUtilTest {
        [TestMethod]
        public void TestMiddlePoint() {
            Assert.AreEqual(
                new Point(10, 10),
                PointUtil.MiddlePoint(new Point(0, 0), new Point(20, 20))
            );
            Assert.AreEqual(
                new Point(10, 10),
                PointUtil.MiddlePoint(new Point(20, 20), new Point(0, 0))
            );
            Assert.AreEqual(
                new Point(10, 10),
                PointUtil.MiddlePoint(new Point(20, 0), new Point(0, 20))
            );
            Assert.AreEqual(
                new Point(10, 10),
                PointUtil.MiddlePoint(new Point(0, 20), new Point(20, 0))
            );
        }

        [TestMethod]
        public void TestScaleWithCircumscribeRect() {
            var pts = new Point[] {
                new Point(10, 10),
                new Point(20, 10),
                new Point(20, 20),
                new Point(10, 20),
            };

            PointUtil.ScaleWithCircumscribeRect(pts, new SizeF(0.5f, 2));
            Assert.AreEqual(new Point(10, 10), pts[0]);
            Assert.AreEqual(new Point(15, 10), pts[1]);
            Assert.AreEqual(new Point(15, 30), pts[2]);
            Assert.AreEqual(new Point(10, 30), pts[3]);

            PointUtil.ScaleWithCircumscribeRect(pts, new SizeF(2, 0.5f));
            Assert.AreEqual(new Point(10, 10), pts[0]);
            Assert.AreEqual(new Point(20, 10), pts[1]);
            Assert.AreEqual(new Point(20, 20), pts[2]);
            Assert.AreEqual(new Point(10, 20), pts[3]);
        }
    }
}
