/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mkamo.Common.Forms.Drawing {
    [TestClass]
    public class Vector2DTest {
        [TestMethod]
        public void TestDistanceLineSegAndPoint() {
            Assert.AreEqual(
                10,
                Vector2D.GetDistanceLineSegAndPoint(
                    new Vector2D(0, 0), new Vector2D(100, 0), new Vector2D(50, 10)
                )
            );
            Assert.AreEqual(
                50,
                Vector2D.GetDistanceLineSegAndPoint(
                    new Vector2D(0, 0), new Vector2D(100, 0), new Vector2D(150, 0)
                )
            );
        }

        [TestMethod]
        public void TestIntersectionPointOfLines() {
            Vector2D v1 = new Vector2D(0, 100);
            Vector2D v2 = new Vector2D(100, 100);
            Vector2D v3 = new Vector2D(50, 50);
            Vector2D v4 = new Vector2D(50, 700);
            Vector2D ip = Vector2D.GetIntersectionPointOfLines(v1, v2, v3, v4);
            Console.WriteLine("{0}, {1}", ip.X, ip.Y);
        }

        [TestMethod]
        public void TestGetNearestPointFrom() {
            var v1 = new Vector2D(0, 0);
            var v2 = new Vector2D(100, 100);
            var v3 = new Vector2D(0, 100);
            var ret = Vector2D.GetNearestPointOnLineSegFromPoint(v1, v2, v3);
            Assert.AreEqual(new Vector2D(50, 50), ret);

            v1 = new Vector2D(0, 0);
            v2 = new Vector2D(100, 50);
            v3 = new Vector2D(0, 50);
            ret = Vector2D.GetNearestPointOnLineSegFromPoint(v1, v2, v3);
            Assert.AreEqual(new Vector2D(20, 10), ret);

        }
    }
}
