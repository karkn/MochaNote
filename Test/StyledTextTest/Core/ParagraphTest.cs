/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mkamo.Common.DataType;

namespace Mkamo.StyledText.Core {
    [TestClass]
    public class ParagraphTest {

        [TestMethod]
        public void TestCopyRange() {
            var para = new Paragraph(new Run("0123456789"));
            para.InsertAfter(CreateLineSegment());
            para.InsertAfter(CreateLineSegment());
            para.InsertAfter(CreateLineSegment());

            var copy = default(Paragraph);

            /// 最初
            copy = (Paragraph) para.CopyRange(new Range(0, 3));
            Assert.AreEqual("012\n", copy.Text);

            /// rangeが一つのLineSegment内
            copy = (Paragraph) para.CopyRange(new Range(2, 3));
            Assert.AreEqual("234\n", copy.Text);

            /// 1～2行目またぐ
            copy = (Paragraph) para.CopyRange(new Range(7, 8));
            Assert.AreEqual("789\r0123\n", copy.Text);

            /// 1～3行目またぐ
            copy = (Paragraph) para.CopyRange(new Range(7, 18));
            Assert.AreEqual("789\r0123456789\r012\n", copy.Text);

            /// 行末ちょうどでも最後は改段落
            copy = (Paragraph) para.CopyRange(new Range(28, 5));
            Assert.AreEqual("6789\n", copy.Text);

            /// 最後 (改段落は入っていてもいなくても同じ結果)
            copy = (Paragraph) para.CopyRange(new Range(38, 5));
            Assert.AreEqual("56789\n", copy.Text);
            copy = (Paragraph) para.CopyRange(new Range(38, 6));
            Assert.AreEqual("56789\n", copy.Text);
        }

        [TestMethod]
        public void TestCopyInlines() {
            var para = new Paragraph(new Run("0123456789"));
            para.InsertAfter(CreateLineSegment());
            para.InsertAfter(CreateLineSegment());
            para.InsertAfter(CreateLineSegment());


            var copy = default(IEnumerable<Inline>);

            /// 最初
            copy = para.CopyInlines(new Range(0, 5));
            Assert.AreEqual(1, copy.Count());
            Assert.AreEqual("01234", copy.ElementAt(0).Text);

            /// rangeが一つのInline内
            copy = para.CopyInlines(new Range(2, 3));
            Assert.AreEqual(1, copy.Count());
            Assert.AreEqual("234", copy.ElementAt(0).Text);

            /// 1～2行目またぐ
            copy = para.CopyInlines(new Range(7, 8));
            Assert.AreEqual(3, copy.Count());
            Assert.AreEqual("789", copy.ElementAt(0).Text);
            Assert.AreEqual("\r", copy.ElementAt(1).Text);
            Assert.AreEqual("0123", copy.ElementAt(2).Text);

            /// 1～3行目またぐ
            copy = para.CopyInlines(new Range(7, 18));
            Assert.AreEqual(5, copy.Count());
            Assert.AreEqual("789", copy.ElementAt(0).Text);
            Assert.AreEqual("\r", copy.ElementAt(1).Text);
            Assert.AreEqual("0123456789", copy.ElementAt(2).Text);
            Assert.AreEqual("\r", copy.ElementAt(3).Text);
            Assert.AreEqual("012", copy.ElementAt(4).Text);

            /// 行末ちょうど
            copy = para.CopyInlines(new Range(28, 5));
            Assert.AreEqual(2, copy.Count());
            Assert.AreEqual("6789", copy.ElementAt(0).Text);
            Assert.AreEqual("\r", copy.ElementAt(1).Text);

            /// 最後
            copy = para.CopyInlines(new Range(38, 5));
            Assert.AreEqual(1, copy.Count());
            Assert.AreEqual("56789", copy.ElementAt(0).Text);

            copy = para.CopyInlines(new Range(38, 6));
            Assert.AreEqual(2, copy.Count());
            Assert.AreEqual("56789", copy.ElementAt(0).Text);
            Assert.AreEqual("\n", copy.ElementAt(1).Text);
        }

        // ------------------------------
        // private
        // ------------------------------
        private LineSegment CreateLineSegment() {
            return new LineSegment(new Run("0123456789"));
        }
    }


    ///// <summary>
    ///// ParagraphTest の概要の説明
    ///// </summary>
    //[TestClass]
    //public class ParagraphTest {
    //    public ParagraphTest() {
    //        //
    //        // TODO: コンストラクタ ロジックをここに追加します
    //        //
    //    }

    //    private TestContext testContextInstance;

    //    /// <summary>
    //    ///現在のテストの実行についての情報および機能を
    //    ///提供するテスト コンテキストを取得または設定します。
    //    ///</summary>
    //    public TestContext TestContext {
    //        get {
    //            return testContextInstance;
    //        }
    //        set {
    //            testContextInstance = value;
    //        }
    //    }

    //    #region 追加のテスト属性
    //    //
    //    // テストを作成する際には、次の追加属性を使用できます:
    //    //
    //    // クラス内で最初のテストを実行する前に、ClassInitialize を使用してコードを実行してください
    //    // [ClassInitialize()]
    //    // public static void MyClassInitialize(TestContext testContext) { }
    //    //
    //    // クラス内のテストをすべて実行したら、ClassCleanup を使用してコードを実行してください
    //    // [ClassCleanup()]
    //    // public static void MyClassCleanup() { }
    //    //
    //    // 各テストを実行する前に、TestInitialize を使用してコードを実行してください
    //    // [TestInitialize()]
    //    // public void MyTestInitialize() { }
    //    //
    //    // 各テストを実行した後に、TestCleanup を使用してコードを実行してください
    //    // [TestCleanup()]
    //    // public void MyTestCleanup() { }
    //    //
    //    #endregion

    //    [TestMethod]
    //    public void TestLines() {
    //        var para = new Paragraph();

    //        Assert.AreEqual(1, para.Lines.Count());

    //        var line = para.Lines.ElementAt(0);
    //        Assert.AreEqual(1, line.Inlines.Count());
    //        Assert.IsTrue(line.Inlines.ElementAt(0) is BlockBreak);

    //    }

    //    [TestMethod]
    //    public void TestLines2() {
    //        var para = new Paragraph();
    //        para.InsertAfter(new Run("hoge"));

    //        Assert.AreEqual(1, para.Lines.Count());

    //        var line = para.Lines.ElementAt(0);
    //        Assert.AreEqual(2, line.Inlines.Count());

    //        Assert.IsTrue(line.Inlines.ElementAt(0).Text == "hoge");
    //        Assert.IsTrue(line.Inlines.ElementAt(1) is BlockBreak);
    //    }

    //    [TestMethod]
    //    public void TestLines3() {
    //        var para = new Paragraph();
    //        para.InsertAfter(new Run("hoge"));
    //        para.InsertAfter(new LineBreak());

    //        Assert.AreEqual(2, para.Lines.Count());

    //        var line = para.Lines.ElementAt(0);
    //        Assert.AreEqual(2, line.Inlines.Count());

    //        Assert.IsTrue(line.Inlines.ElementAt(0).Text == "hoge");
    //        Assert.IsTrue(line.Inlines.ElementAt(1) is LineBreak);

    //        line = para.Lines.ElementAt(1);
    //        Assert.AreEqual(1, line.Inlines.Count());
    //        Assert.IsTrue(line.Inlines.ElementAt(0) is BlockBreak);
    //    }

    //    [TestMethod]
    //    public void TestLines4() {
    //        var para = new Paragraph();
    //        para.InsertAfter(new Run("hoge"));
    //        para.InsertAfter(new LineBreak());
    //        para.InsertAfter(new Run("geho"));

    //        Assert.AreEqual(2, para.Lines.Count());

    //        var line = para.Lines.ElementAt(0);
    //        Assert.AreEqual(2, line.Inlines.Count());

    //        Assert.IsTrue(line.Inlines.ElementAt(0).Text == "hoge");
    //        Assert.IsTrue(line.Inlines.ElementAt(1) is LineBreak);

    //        line = para.Lines.ElementAt(1);
    //        Assert.AreEqual(2, line.Inlines.Count());
    //        Assert.IsTrue(line.Inlines.ElementAt(0).Text == "geho");
    //        Assert.IsTrue(line.Inlines.ElementAt(1) is BlockBreak);
    //    }


        
    //    [TestMethod]
    //    public void TestLineStrings() {
    //        var para = new Paragraph();
    //        CollectionAssert.AreEqual(new string[] {""} , para.LineStrings);
    //    }

    //    [TestMethod]
    //    public void TestLineStrings2() {
    //        var para = new Paragraph();
    //        para.InsertAfter(new Run("hoge"));
    //        CollectionAssert.AreEqual(new string[] {"hoge"}, para.LineStrings);
    //    }

    //    [TestMethod]
    //    public void TestLineStrings3() {
    //        var para = new Paragraph();
    //        para.InsertAfter(new Run("hoge"));
    //        para.InsertAfter(new LineBreak());
    //        CollectionAssert.AreEqual(new string[] {"hoge", ""}, para.LineStrings);
    //    }

    //    [TestMethod]
    //    public void TestLineStrings4() {
    //        var para = new Paragraph();
    //        para.InsertAfter(new Run("hoge"));
    //        para.InsertAfter(new LineBreak());
    //        para.InsertAfter(new Run("geho"));
    //        CollectionAssert.AreEqual(new string[] {"hoge", "geho"}, para.LineStrings);
    //    }



    //    [TestMethod]
    //    public void TestLineCount() {
    //        var para = new Paragraph();
    //        var lines = para.LineStrings;
    //        Assert.AreEqual(1, para.LineCount);
    //    }

    //    [TestMethod]
    //    public void TestLineCount2() {
    //        var para = new Paragraph();
    //        para.InsertAfter(new Run("hoge"));
    //        Assert.AreEqual(1, para.LineCount);
    //    }

    //    [TestMethod]
    //    public void TestLineCount3() {
    //        var para = new Paragraph();
    //        para.InsertAfter(new Run("hoge"));
    //        para.InsertAfter(new LineBreak());
    //        Assert.AreEqual(2, para.LineCount);
    //    }

    //    [TestMethod]
    //    public void TestLineCount4() {
    //        var para = new Paragraph();
    //        para.InsertAfter(new Run("hoge"));
    //        para.InsertAfter(new LineBreak());
    //        para.InsertAfter(new Run("geho"));
    //        Assert.AreEqual(2, para.LineCount);
    //    }



    //}
}
