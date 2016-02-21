/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using Mkamo.Common.Crypto;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography;
namespace Mkamo.Common.Crypto
{
    
    
    /// <summary>
    ///AesUtilTest のテスト クラスです。すべての
    ///AesUtilTest 単体テストをここに含めます
    ///</summary>
    [TestClass()]
    public class AesUtilTest {


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


        /// <summary>
        ///EncryptString のテスト
        ///</summary>
        [TestMethod()]
        public void EncryptStringTest() {
            string str = "ほげ";
            string password = "pass";
            string expected = "4eP/nJepziYcWAJVLI+JLQ==";
            string actual;
            actual = AesUtil.EncryptString(str, password);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///DecryptString のテスト
        ///</summary>
        [TestMethod()]
        public void DecryptStringTest() {
            string str = "4eP/nJepziYcWAJVLI+JLQ==";
            string password = "pass";
            string expected = "ほげ";
            string actual;
            actual = AesUtil.DecryptString(str, password);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod(), ExpectedException(typeof(CryptographicException))]
        public void DecryptStringExceptionTest() {
            string str = "4eP/nJepziYcWAJVLI+JLQ==";
            string password = "pass_false"; /// 間違ったパスワード
            string expected = "ほげ";
            string actual;
            actual = AesUtil.DecryptString(str, password);
            Assert.AreEqual(expected, actual);
        }

    }
}
