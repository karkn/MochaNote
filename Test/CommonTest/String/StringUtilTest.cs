/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mkamo.Common.Core;

namespace Mkamo.Common.String {
    [TestClass]
    public class StringUtilTest {
        [TestMethod]
        public void TestGetLineIndexOf() {
            string text =
                "foo\n" +
                "bar\r\n" +
                "baz";
            int[] expected = new int[] { 0, 0, 0, 0, 1, 1, 1, 1, 1, 2, 2, 2};
            for (int i = 0; i < text.Length; ++i) {
                Assert.AreEqual(expected[i], StringLineUtil.GetLineIndexOf(text, i));
            }

            // いきなり改行
            text =
                "\r\n" +
                "a";
            Assert.AreEqual(0, StringLineUtil.GetLineIndexOf(text, 0));
            Assert.AreEqual(0, StringLineUtil.GetLineIndexOf(text, 1));
            Assert.AreEqual(1, StringLineUtil.GetLineIndexOf(text, 2));

            // 改行続き
            text =
                "\r\n" +
                "\r\n" +
                "a";
            Assert.AreEqual(0, StringLineUtil.GetLineIndexOf(text, 0));
            Assert.AreEqual(0, StringLineUtil.GetLineIndexOf(text, 1));
            Assert.AreEqual(1, StringLineUtil.GetLineIndexOf(text, 2));
            Assert.AreEqual(1, StringLineUtil.GetLineIndexOf(text, 3));
            Assert.AreEqual(2, StringLineUtil.GetLineIndexOf(text, 4));

            text =
                "foo\r\n" +
                "bar";
            Assert.AreEqual(1, StringLineUtil.GetLineIndexOf(text, 5));
        }

        [TestMethod]
        public void TestGetColumnIndexOf() {
            string text =
                "foo\n" +
                "bar\r\n" +
                "baz";
            int[] expected = new int[] { 0, 1, 2, 3, 0, 1, 2, 3, 4, 0, 1, 2};
            for (int i = 0; i < text.Length; ++i) {
                Assert.AreEqual(expected[i], StringLineUtil.GetColumnIndexOf(text, i));
            }

            // いきなり改行
            text =
                "\r\n" +
                "a";
            Assert.AreEqual(0, StringLineUtil.GetColumnIndexOf(text, 0));
            Assert.AreEqual(1, StringLineUtil.GetColumnIndexOf(text, 1));
            Assert.AreEqual(0, StringLineUtil.GetColumnIndexOf(text, 2));

            // 改行続き
            text =
                "\r\n" +
                "\r\n" +
                "a";
            Assert.AreEqual(0, StringLineUtil.GetColumnIndexOf(text, 0));
            Assert.AreEqual(1, StringLineUtil.GetColumnIndexOf(text, 1));
            Assert.AreEqual(0, StringLineUtil.GetColumnIndexOf(text, 2));
            Assert.AreEqual(1, StringLineUtil.GetColumnIndexOf(text, 3));
            Assert.AreEqual(0, StringLineUtil.GetColumnIndexOf(text, 4));
        }

        [TestMethod]
        public void TestGetLineStartIndexOf() {
            string text =
                "foo\n" +
                "bar\r\n" +
                "baz";
            Assert.AreEqual(0, StringLineUtil.GetLineStartCharacterIndexOf(text, 0));
            Assert.AreEqual(4, StringLineUtil.GetLineStartCharacterIndexOf(text, 1));
            Assert.AreEqual(9, StringLineUtil.GetLineStartCharacterIndexOf(text, 2));

            // いきなり改行
            text =
                "\r\n" +
                "a";
            Assert.AreEqual(0, StringLineUtil.GetLineStartCharacterIndexOf(text, 0));
            Assert.AreEqual(2, StringLineUtil.GetLineStartCharacterIndexOf(text, 1));
            
            // 改行続き
            text =
                "\r\n" +
                "\r\n" +
                "a";
            Assert.AreEqual(0, StringLineUtil.GetLineStartCharacterIndexOf(text, 0));
            Assert.AreEqual(2, StringLineUtil.GetLineStartCharacterIndexOf(text, 1));
            Assert.AreEqual(4, StringLineUtil.GetLineStartCharacterIndexOf(text, 2));
        }

        [TestMethod]
        public void TestGetLineEndIndexOf() {
            string text =
                "foo\n" +
                "bar\r\n" +
                "baz";
            Assert.AreEqual(3, StringLineUtil.GetLineEndCharacterIndexOf(text, 0));
            Assert.AreEqual(8, StringLineUtil.GetLineEndCharacterIndexOf(text, 1));
            Assert.AreEqual(12, StringLineUtil.GetLineEndCharacterIndexOf(text, 2));

            //// いきなり改行
            text =
                "\r\n" +
                "a";
            Assert.AreEqual(1, StringLineUtil.GetLineEndCharacterIndexOf(text, 0));
            Assert.AreEqual(3, StringLineUtil.GetLineEndCharacterIndexOf(text, 1));
            
            //// 改行続き
            text =
                "\r\n" +
                "\r\n" +
                "a";
            Assert.AreEqual(1, StringLineUtil.GetLineEndCharacterIndexOf(text, 0));
            Assert.AreEqual(3, StringLineUtil.GetLineEndCharacterIndexOf(text, 1));
            Assert.AreEqual(5, StringLineUtil.GetLineEndCharacterIndexOf(text, 2));
        }

        [TestMethod]
        public void TestGetIndexOf() {
            string text =
                "foo\n" +
                "bar\r\n" +
                "baz";
            Assert.AreEqual(0, StringLineUtil.GetCharacterIndexOf(text, 0, 0));
            Assert.AreEqual(3, StringLineUtil.GetCharacterIndexOf(text, 0, 3));
            Assert.AreEqual(4, StringLineUtil.GetCharacterIndexOf(text, 1, 0));
            Assert.AreEqual(8, StringLineUtil.GetCharacterIndexOf(text, 1, 4));
            Assert.AreEqual(9, StringLineUtil.GetCharacterIndexOf(text, 2, 0));
        }

        [TestMethod]
        public void TestGetLineCount() {
            string text =
                "foo\n" +
                "bar\r\n" +
                "baz";
            Assert.AreEqual(3, StringLineUtil.GetLineCount(text));

            text =
                "foo\n" +
                "bar\r\n" +
                "baz\n";
            Assert.AreEqual(4, StringLineUtil.GetLineCount(text));

            //// いきなり改行
            text =
                "\r\n" +
                "a";
            Assert.AreEqual(2, StringLineUtil.GetLineCount(text));

            //// 改行続き
            text =
                "\r\n" +
                "\r\n" +
                "a";
            Assert.AreEqual(3, StringLineUtil.GetLineCount(text));
        }

        [TestMethod]
        public void TestGetColumnCount() {
            string text =
                "foo\n" +
                "bar\r\n" +
                "baz";
            Assert.AreEqual(4, StringLineUtil.GetColumnCount(text, 0));
            Assert.AreEqual(5, StringLineUtil.GetColumnCount(text, 1));
            Assert.AreEqual(4, StringLineUtil.GetColumnCount(text, 2));
        }

        [TestMethod]
        public void TestSplitLines() {
            string text =
                "foo\n" +
                "bar\r\n" +
                "baz";
            string[] ret = StringLineUtil.SplitLines(text);
            Assert.AreEqual(3, ret.Length);
            Assert.AreEqual("foo\n", ret[0]);
            Assert.AreEqual("bar\r\n", ret[1]);
            Assert.AreEqual("baz\0", ret[2]);
        }

        [TestMethod]
        public void TestSplitLines_int_int() {
            string text =
                "foo\n" +
                "bar\r\n" +
                "baz";
            string[] ret = StringLineUtil.SplitLines(text, 1, 2);
            Assert.AreEqual(2, ret.Length);
            Assert.AreEqual("bar\r\n", ret[0]);
            Assert.AreEqual("baz\0", ret[1]);

            ret = StringLineUtil.SplitLines(text, 0, 3);
            Assert.AreEqual(3, ret.Length);
            Assert.AreEqual("foo\n", ret[0]);
            Assert.AreEqual("bar\r\n", ret[1]);
            Assert.AreEqual("baz\0", ret[2]);
        }

        [TestMethod]
        public void TestGetUrlRange() {
            var s = "ほげhhttp://www.confidante.jp あほ";
            var range = StringUtil.GetUrlRange(s);
            Assert.AreEqual("http://www.confidante.jp", s.Substring(range.Offset, range.Length));

            s = "ほげ http://www.confidante.jp あほ";
            range = StringUtil.GetUrlRange(s);
            Assert.AreEqual("http://www.confidante.jp", s.Substring(range.Offset, range.Length));

            s = "ほげ http://www.confidante.jp/ あほ";
            range = StringUtil.GetUrlRange(s);
            Assert.AreEqual("http://www.confidante.jp/", s.Substring(range.Offset, range.Length));

            s = "ほげ http://www.confidante.jp/index.html あほ";
            range = StringUtil.GetUrlRange(s);
            Assert.AreEqual("http://www.confidante.jp/index.html", s.Substring(range.Offset, range.Length));

            s = "ほげ htp://www.confidante.jp/index.html あほ";
            range = StringUtil.GetUrlRange(s);
            Assert.IsTrue(range.IsEmpty);
        }

        [TestMethod]
        public void TestGetUrlRanges() {
            var s = "ほげhhttp://www.confidante.jp あほhttp://www.futamisoft.com/ ばか";
            var ranges = StringUtil.GetUrlRanges(s);

            Assert.AreEqual(2, ranges.Count());
            Assert.AreEqual("http://www.confidante.jp", s.Substring(ranges.ElementAt(0).Offset, ranges.ElementAt(0).Length));
            Assert.AreEqual("http://www.futamisoft.com/", s.Substring(ranges.ElementAt(1).Offset, ranges.ElementAt(1).Length));

        }
    }
}
