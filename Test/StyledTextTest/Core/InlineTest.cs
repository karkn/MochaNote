/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mkamo.Common.DataType;
using Mkamo.Common.Forms.Descriptions;

namespace Mkamo.StyledText.Core {
    [TestClass]
    public class InlineTest {

        [TestMethod]
        public void TestCopyRange() {
            var run = new Run("0123456789");

            Assert.AreEqual("23456", ((Inline) run.CopyRange(new Range(2, 5))).Text);
            Assert.AreEqual("01234", ((Inline) run.CopyRange(new Range(0, 5))).Text);
            Assert.AreEqual("789", ((Inline) run.CopyRange(new Range(7, 3))).Text);
        }

        [TestMethod]
        public void TestBlockBreakClone() {
            var bb = new BlockBreak();
            bb.Font = new FontDescription("MS Gothic", 12);

            var clone = bb.Clone() as BlockBreak;

            Assert.AreEqual(bb.Font.Name, clone.Font.Name);
        }
    }
}
