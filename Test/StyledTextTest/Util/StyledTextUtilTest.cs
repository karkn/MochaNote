/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mkamo.StyledText.Util {
    /// <summary>
    /// StyledTextUtilTest の概要の説明
    /// </summary>
    [TestClass]
    public class StyledTextUtilTest {
        public StyledTextUtilTest() {
        }



        [TestMethod]
        public void TestSplitWithLineBreak() {
            CollectionAssert.AreEqual(
                new [] { "foo", "bar", "baz" },
                StyledTextUtil.SplitWithBlockBreak("foo\r\nbar\r\nbaz")
            );

            CollectionAssert.AreEqual(
                new [] { "foo", "bar", "baz" },
                StyledTextUtil.SplitWithBlockBreak("foo\r\nbar\r\nbaz\r\n")
            );

            CollectionAssert.AreEqual(
                new [] { "foo" },
                StyledTextUtil.SplitWithBlockBreak("foo")
            );

            /// \r改行
            CollectionAssert.AreEqual(
                new [] { "foo", "bar", "baz" },
                StyledTextUtil.SplitWithBlockBreak("foo\rbar\rbaz")
            );

            /// \n改行
            CollectionAssert.AreEqual(
                new [] { "foo", "bar", "baz" },
                StyledTextUtil.SplitWithBlockBreak("foo\nbar\nbaz")
            );

            /// 空
            CollectionAssert.AreEqual(
                new string[0],
                StyledTextUtil.SplitWithBlockBreak("")
            );

            CollectionAssert.AreEqual(
                new string[0],
                StyledTextUtil.SplitWithBlockBreak(null)
            );
        }


        [TestMethod]
        public void TestNormalizeLineBreak() {
            Assert.AreEqual("foo\rbar", StyledTextUtil.NormalizeLineBreak("foo\r\nbar", "\r"));
            Assert.AreEqual("foo\nbar", StyledTextUtil.NormalizeLineBreak("foo\rbar", "\n"));
            Assert.AreEqual("foo\rbar", StyledTextUtil.NormalizeLineBreak("foo\nbar", "\r"));
        }

    }
}
