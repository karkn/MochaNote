/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mkamo.Common.Text {
    [TestClass]
    public class TextBufferTest {
        private static ITextBuffer _textBuffer1;
        private static ITextBuffer _textBuffer2;
        private static ITextBuffer _textBuffer3;

        [TestInitialize]
        public void SetUp() {
            _textBuffer1 = TextUtil.CreateTextBuffer();
            _textBuffer1.Text =
                "foo" + "\n" +  // 0 1 2 3
                "bar" + "\n" +  // 4 5 6 7
                "";             // 8*
            _textBuffer2 = TextUtil.CreateTextBuffer();
            _textBuffer2.Text =
                "foo" + "\n" +  // 0 1 2 3 
                "bar";          // 4 5 6 7*
            _textBuffer3 = TextUtil.CreateTextBuffer();
            _textBuffer3.Text =
                "foo" + "\n" +  // 0 1 2 3
                "bar" + "\n" +  // 4 5 6 7
                "\n" +          // 8
                "";             // 9*
        }

        [TestMethod]
        public void TestLineCount() {
            Assert.AreEqual(3, _textBuffer1.LineCount);
            Assert.AreEqual(2, _textBuffer2.LineCount);
            Assert.AreEqual(4, _textBuffer3.LineCount);
        }

        [TestMethod]
        public void TestLines() {
            Assert.AreEqual(3, _textBuffer1.Lines.Length);
            Assert.AreEqual("foo", _textBuffer1.Lines[0]);
            Assert.AreEqual("bar", _textBuffer1.Lines[1]);
            Assert.AreEqual("", _textBuffer1.Lines[2]);

            Assert.AreEqual(2, _textBuffer2.Lines.Length);
            Assert.AreEqual("bar", _textBuffer2.Lines[1]);

            Assert.AreEqual(4, _textBuffer3.Lines.Length);
            Assert.AreEqual("", _textBuffer3.Lines[2]);
            Assert.AreEqual("", _textBuffer3.Lines[3]);
        }

        [TestMethod]
        public void TestIsNewLine() {
            Assert.AreEqual(false, _textBuffer1.IsEOL(0));
            Assert.AreEqual(true, _textBuffer1.IsEOL(3));
            Assert.AreEqual(false, _textBuffer1.IsEOL(4));
            Assert.AreEqual(false, _textBuffer1.IsEOL(6));
            Assert.AreEqual(true, _textBuffer1.IsEOL(7));
            Assert.AreEqual(false, _textBuffer1.IsEOL(8));
            try {
                _textBuffer1.IsEOL(9);
                Assert.Fail();
            } catch (ArgumentOutOfRangeException) {
            }


            Assert.AreEqual(false, _textBuffer2.IsEOL(6));
            Assert.AreEqual(false, _textBuffer2.IsEOL(7));
            try {
                _textBuffer2.IsEOL(8);
                Assert.Fail();
            } catch (ArgumentOutOfRangeException) {
            }

            Assert.AreEqual(false, _textBuffer3.IsEOL(6));
            Assert.AreEqual(true, _textBuffer3.IsEOL(7));
            Assert.AreEqual(true, _textBuffer3.IsEOL(8));
            Assert.AreEqual(false, _textBuffer3.IsEOL(9));
            try {
                _textBuffer3.IsEOL(10);
                Assert.Fail();
            } catch (ArgumentOutOfRangeException) {
            }
        }

        [TestMethod]
        public void TestGetLineIndexOf() {
            Assert.AreEqual(0, _textBuffer1.GetLineIndexOf(0));
            Assert.AreEqual(0, _textBuffer1.GetLineIndexOf(3));
            Assert.AreEqual(1, _textBuffer1.GetLineIndexOf(4));
            Assert.AreEqual(1, _textBuffer1.GetLineIndexOf(7));
            Assert.AreEqual(2, _textBuffer1.GetLineIndexOf(8));
            try {
                _textBuffer1.GetLineIndexOf(9);
                Assert.Fail();
            } catch (ArgumentOutOfRangeException) {
            }

            Assert.AreEqual(1, _textBuffer2.GetLineIndexOf(6));
            Assert.AreEqual(1, _textBuffer2.GetLineIndexOf(7));
            try {
                _textBuffer2.GetLineIndexOf(8);
                Assert.Fail();
            } catch (ArgumentOutOfRangeException) {
            }

            Assert.AreEqual(2, _textBuffer3.GetLineIndexOf(8));
            Assert.AreEqual(3, _textBuffer3.GetLineIndexOf(9));
            try {
                _textBuffer3.GetLineIndexOf(10);
                Assert.Fail();
            } catch (ArgumentOutOfRangeException) {
            }
        }

        [TestMethod]
        public void TestGetColumnIndexOf() {
            Assert.AreEqual(0, _textBuffer1.GetColumnIndexOf(0));
            Assert.AreEqual(1, _textBuffer1.GetColumnIndexOf(1));
            Assert.AreEqual(2, _textBuffer1.GetColumnIndexOf(2));
            Assert.AreEqual(3, _textBuffer1.GetColumnIndexOf(3));
            Assert.AreEqual(0, _textBuffer1.GetColumnIndexOf(4));
            Assert.AreEqual(1, _textBuffer1.GetColumnIndexOf(5));
            Assert.AreEqual(2, _textBuffer1.GetColumnIndexOf(6));
            Assert.AreEqual(3, _textBuffer1.GetColumnIndexOf(7));
            Assert.AreEqual(0, _textBuffer1.GetColumnIndexOf(8));
            try {
                _textBuffer1.GetColumnIndexOf(9);
                Assert.Fail();
            } catch (ArgumentOutOfRangeException) {
            }

            Assert.AreEqual(2, _textBuffer2.GetColumnIndexOf(6));
            Assert.AreEqual(3, _textBuffer2.GetColumnIndexOf(7));
            try {
                _textBuffer2.GetColumnIndexOf(8);
                Assert.Fail();
            } catch (ArgumentOutOfRangeException) {
            }

            Assert.AreEqual(2, _textBuffer3.GetColumnIndexOf(6));
            Assert.AreEqual(3, _textBuffer3.GetColumnIndexOf(7));
            Assert.AreEqual(0, _textBuffer3.GetColumnIndexOf(8));
            Assert.AreEqual(0, _textBuffer3.GetColumnIndexOf(9));
            try {
                _textBuffer3.GetColumnIndexOf(10);
                Assert.Fail();
            } catch (ArgumentOutOfRangeException) {
            }
        }

        [TestMethod]
        public void TestGetLineStartCharIndexOf() {
            Assert.AreEqual(0, _textBuffer1.GetLineStartCharIndexOf(0));
            Assert.AreEqual(4, _textBuffer1.GetLineStartCharIndexOf(1));
            Assert.AreEqual(8, _textBuffer1.GetLineStartCharIndexOf(2));

            Assert.AreEqual(0, _textBuffer2.GetLineStartCharIndexOf(0));
            Assert.AreEqual(4, _textBuffer2.GetLineStartCharIndexOf(1));

            Assert.AreEqual(0, _textBuffer3.GetLineStartCharIndexOf(0));
            Assert.AreEqual(4, _textBuffer3.GetLineStartCharIndexOf(1));
            Assert.AreEqual(8, _textBuffer3.GetLineStartCharIndexOf(2));
            Assert.AreEqual(9, _textBuffer3.GetLineStartCharIndexOf(3));
        }

        [TestMethod]
        public void TestGetLineEndCharIndexOf() {
            Assert.AreEqual(3, _textBuffer1.GetLineEndCharIndexOf(0));
            Assert.AreEqual(7, _textBuffer1.GetLineEndCharIndexOf(1));
            Assert.AreEqual(8, _textBuffer1.GetLineEndCharIndexOf(2));

            Assert.AreEqual(3, _textBuffer2.GetLineEndCharIndexOf(0));
            Assert.AreEqual(7, _textBuffer2.GetLineEndCharIndexOf(1));

            Assert.AreEqual(3, _textBuffer3.GetLineEndCharIndexOf(0));
            Assert.AreEqual(7, _textBuffer3.GetLineEndCharIndexOf(1));
            Assert.AreEqual(8, _textBuffer3.GetLineEndCharIndexOf(2));
            Assert.AreEqual(9, _textBuffer3.GetLineEndCharIndexOf(3));
        }

        [TestMethod]
        public void TestGetCharIndexOf() {
            Assert.AreEqual(0, _textBuffer1.GetCharIndexOf(0, 0));
            Assert.AreEqual(1, _textBuffer1.GetCharIndexOf(0, 1));
            Assert.AreEqual(2, _textBuffer1.GetCharIndexOf(0, 2));
            Assert.AreEqual(3, _textBuffer1.GetCharIndexOf(0, 3));
            Assert.AreEqual(4, _textBuffer1.GetCharIndexOf(1, 0));
            Assert.AreEqual(7, _textBuffer1.GetCharIndexOf(1, 3));
            Assert.AreEqual(8, _textBuffer1.GetCharIndexOf(2, 0));

            Assert.AreEqual(6, _textBuffer2.GetCharIndexOf(1, 2));
            Assert.AreEqual(7, _textBuffer2.GetCharIndexOf(1, 3));

            Assert.AreEqual(7, _textBuffer3.GetCharIndexOf(1, 3));
            Assert.AreEqual(8, _textBuffer3.GetCharIndexOf(2, 0));
            Assert.AreEqual(9, _textBuffer3.GetCharIndexOf(3, 0));
        }

        [TestMethod]
        public void TestGetColumnCount() {
            Assert.AreEqual(3, _textBuffer1.GetColumnCount(0));
            Assert.AreEqual(3, _textBuffer1.GetColumnCount(1));
            Assert.AreEqual(0, _textBuffer1.GetColumnCount(2));

            Assert.AreEqual(3, _textBuffer2.GetColumnCount(0));
            Assert.AreEqual(3, _textBuffer2.GetColumnCount(1));

            Assert.AreEqual(3, _textBuffer3.GetColumnCount(0));
            Assert.AreEqual(3, _textBuffer3.GetColumnCount(1));
            Assert.AreEqual(0, _textBuffer3.GetColumnCount(2));
            Assert.AreEqual(0, _textBuffer3.GetColumnCount(3));
        }

        //[TestMethod]
        //public void TestSwap() {
        //    _textBuffer1.Swap(0, 2, 4, 0);
        //    Assert.AreEqual("o\nfobar\n", _textBuffer1.Text);

        //    SetUp();
        //    _textBuffer1.Swap(0, 4, 4, 0);
        //    Assert.AreEqual("foo\nbar\n", _textBuffer1.Text);

        //    SetUp();
        //    _textBuffer1.Swap(0, 4, 2, 0);
        //    Assert.AreEqual("foo\nbar\n", _textBuffer1.Text);

        //}
    }
}
