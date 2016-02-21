/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Mkamo.Common.DataType;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mkamo.Common.Core;

namespace Mkamo.Common.Forms.Drawing {
    [TestClass]
    public class RectangleUtilTest {
        [TestMethod]
        public void TestGetInDirectionCode() {
            var rect = new Rectangle(10, 10, 100, 100);

            Assert.IsTrue(
                EnumUtil.HasAllFlags(
                    (int) RectUtil.GetInnerDirection(rect, rect.Location + new Size(50, 10)),
                    (int) (Directions.Up | Directions.HorizontalNeutral)
                )
            );

            Assert.IsTrue(
                EnumUtil.HasAllFlags(
                    (int) RectUtil.GetInnerDirection(rect, rect.Location + new Size(10, 50)),
                    (int) (Directions.Left | Directions.VerticalNeutral)
                )
            );

            Assert.IsTrue(
                EnumUtil.HasAllFlags(
                    (int) RectUtil.GetInnerDirection(rect, rect.Location + new Size(50, 90)),
                    (int) (Directions.Down | Directions.HorizontalNeutral)
                )
            );

            Assert.IsTrue(
                EnumUtil.HasAllFlags(
                    (int) RectUtil.GetInnerDirection(rect, rect.Location + new Size(90, 50)),
                    (int) (Directions.Right | Directions.VerticalNeutral)
                )
            );

            Assert.IsTrue(
                EnumUtil.HasAllFlags(
                    (int) RectUtil.GetInnerDirection(rect, rect.Location),
                    (int) (Directions.Up | Directions.Left)
                )
            );

            Assert.IsTrue(
                EnumUtil.HasAllFlags(
                    (int) RectUtil.GetInnerDirection(rect, new Point(rect.Right - 1, rect.Bottom - 1)),
                    (int) (Directions.Down| Directions.Right)
                )
            );

        }
    }
}
