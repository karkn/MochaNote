/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mkamo.StyledText.Core;

namespace Mkamo.StyledText.Core {
    /// <summary>
    /// SpanTest の概要の説明
    /// </summary>
    [TestClass]
    public class SpanTest {
        public SpanTest() {
        }


        [TestMethod]
        public void TestText() {
            var span = new Run();
            span.Text = "foo\rbar\nbaz";
            Assert.AreEqual("foo bar baz", span.Text);
        }

        [TestMethod]
        public void TestRemove() {
            var span = new Run("foobar");

            span.Remove(3, 3);
            Assert.AreEqual("foo", span.Text);

            span = new Run("foobar");
            span.Remove(3);
            Assert.AreEqual("foo", span.Text);

            span = new Run("foobar");
            span.Remove(0, 3);
            Assert.AreEqual("bar", span.Text);

            span = new Run("foobar");
            span.Remove(2, 2);
            Assert.AreEqual("foar", span.Text);
        }
    }
}
