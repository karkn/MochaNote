/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using Mkamo.Common.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Mkamo.Common
{
    
    
    /// <summary>
    ///ArrayOrderedComparerTest のテスト クラスです。すべての
    ///ArrayOrderedComparerTest 単体テストをここに含めます
    ///</summary>
    [TestClass()]
    public class ArrayOrderedComparerTest {


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
        //テストを作成するときに、次の追加属性を使用することができます:
        //
        //クラスの最初のテストを実行する前にコードを実行するには、ClassInitialize を使用
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //クラスのすべてのテストを実行した後にコードを実行するには、ClassCleanup を使用
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //各テストを実行する前にコードを実行するには、TestInitialize を使用
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //各テストを実行した後にコードを実行するには、TestCleanup を使用
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        [TestMethod()]
        public void TestCompare() {
            var obj1 = "1";
            var obj2 = "2";
            var obj3 = "3";
            var obj4 = "4";
            var obj5 = "5";
            var array = new[] { obj2, obj4, obj3, obj5, obj1 };
            var target = new ArrayOrderingComparer<string>(array);

            Assert.IsTrue(target.Compare(obj2, obj4) < 0);
            Assert.IsTrue(target.Compare(obj4, obj5) < 0);
            Assert.IsTrue(target.Compare(obj5, obj1) < 0);

            Assert.IsTrue(target.Compare(obj2, obj2) == 0);
            Assert.IsTrue(target.Compare(obj1, obj1) == 0);
            Assert.IsTrue(target.Compare(obj3, obj3) == 0);

            Assert.IsTrue(target.Compare(obj4, obj2) > 0);
            Assert.IsTrue(target.Compare(obj5, obj4) > 0);
            Assert.IsTrue(target.Compare(obj1, obj5) > 0);

            Assert.IsTrue(target.Compare(obj1, null) < 0);
            Assert.IsTrue(target.Compare(null, obj1) > 0);
            Assert.IsTrue(target.Compare(null, null) == 0);

        }

    }
}
