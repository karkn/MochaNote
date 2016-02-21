/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mkamo.Common.Core;

namespace Mkamo.Common.Collection {
    /// <summary>
    /// EnumerableExtTest の概要の説明
    /// </summary>
    [TestClass]
    public class EnumerableExtTest {
        public EnumerableExtTest() {
            //
            // TODO: コンストラクタ ロジックをここに追加します
            //
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

        #region 追加のテスト属性
        //
        // テストを作成する際には、次の追加属性を使用できます:
        //
        // クラス内で最初のテストを実行する前に、ClassInitialize を使用してコードを実行してください
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // クラス内のテストをすべて実行したら、ClassCleanup を使用してコードを実行してください
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // 各テストを実行する前に、TestInitialize を使用してコードを実行してください
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // 各テストを実行した後に、TestCleanup を使用してコードを実行してください
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestRange() {
            var list = new List<int>();
            list.Add(0);
            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Add(4);

            var ranged = list.Range(1, 3);
            Assert.AreEqual(3, ranged.Count());
            Assert.AreEqual(1, ranged.ElementAt(0));
            Assert.AreEqual(2, ranged.ElementAt(1));
            Assert.AreEqual(3, ranged.ElementAt(2));

            ranged = list.Range(4, 1);
            Assert.AreEqual(1, ranged.Count());
            Assert.AreEqual(4, ranged.ElementAt(0));
        }
    }
}
