/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mkamo.Common.Forms.Descriptions {
    /// <summary>
    /// FontDescriptionTest の概要の説明
    /// </summary>
    [TestClass]
    public class FontDescriptionTest {
        public FontDescriptionTest() {
        }

        private TestContext testContextInstance;

        /// <summary>
        ///現在のテストの実行についての情報および機能を
        ///提供するテスト コンテキストを取得または設定します。
        ///</summary>
        public TestContext TestContext {
            get {
                return testContextInstance;
            }
            set {
                testContextInstance = value;
            }
        }

        [TestMethod]
        public void TestEquals() {
            var font1 = new FontDescription(SystemFontDescriptions.DefaultFont, 8);
            var font2 = new FontDescription(SystemFontDescriptions.DefaultFont, 8);

            Assert.AreEqual(font1, font2);
            Assert.IsTrue(font1 != font2);
        }

        [TestMethod]
        public void TestGetHashCode() {
            var font1 = new FontDescription(SystemFontDescriptions.DefaultFont, 8);
            var font2 = new FontDescription(SystemFontDescriptions.DefaultFont, 8);

            Assert.AreEqual(font1.GetHashCode(), font2.GetHashCode());

            var dict = new Dictionary<FontDescription, string>();
            dict[font1] = "hoge";
            Assert.IsTrue(dict.ContainsKey(font1));
            Assert.IsTrue(dict.ContainsKey(font2));
            Assert.AreEqual("hoge", dict[font1]);
            Assert.AreEqual("hoge", dict[font2]);
        }
    }
}
