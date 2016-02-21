/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;

namespace Mkamo.StyledText.Core {
    //[TestClass]
    //public class StyledTextRangeTest: StyledTextTestBase {
    //    // ========================================
    //    // field
    //    // ========================================

    //    // ========================================
    //    // constructor
    //    // ========================================

    //    // ========================================
    //    // property
    //    // ========================================

    //    // ========================================
    //    // method
    //    // ========================================
    //    [TestInitialize]
    //    public override void Init() {
    //        base.Init();
    //    }

    //    [TestMethod]
    //    public void TestText() {
    //        var s = "foobarbaz\n";
    //        s += "afo\r";
    //        s += "cfo\n";
    //        Assert.AreEqual(s, _styledText.Text);
    //    }

    //    /// <summary>
    //    /// InsertBlockBreakのテスト．
    //    /// </summary>
    //    //[TestMethod]
    //    //public void TestInsertBlockBreak1() {
    //    //    var range = _styledText.CreateRange(3, 1);

    //    //    range.InsertBlockBreakBefore();
    //    //    Assert.AreEqual(2, _styledText.Blocks[0].Inlines.Count());
    //    //    Assert.AreEqual("foo\n", _styledText.Blocks[0].Text);
    //    //    Assert.AreEqual(3, _styledText.Blocks[1].Inlines.Count());
    //    //    Assert.AreEqual("barbaz\n", _styledText.Blocks[1].Text);
    //    //}

    //    ///// <summary>
    //    ///// InsertBlockBreakのテスト．
    //    ///// </summary>
    //    //[TestMethod]
    //    //public void TestInsertBlockBreak2() {
    //    //    var range = _styledText.CreateRange(1, 1);

    //    //    range.InsertBlockBreakBefore();
    //    //    Assert.AreEqual(2, _styledText.Blocks[0].Inlines.Count());
    //    //    Assert.AreEqual("f\n", _styledText.Blocks[0].Text);
    //    //    Assert.AreEqual(4, _styledText.Blocks[1].Inlines.Count());
    //    //    Assert.AreEqual("oobarbaz\n", _styledText.Blocks[1].Text);
    //    //}

    //    ///// <summary>
    //    ///// InsertBlockBreakのテスト．
    //    ///// </summary>
    //    //[TestMethod]
    //    //public void TestInsertBlockBreak3() {
    //    //    var range = _styledText.CreateRange(11, 1);

    //    //    range.InsertBlockBreakBefore();
    //    //    Assert.AreEqual(2, _styledText.Blocks[1].Inlines.Count());
    //    //    Assert.AreEqual("a\n", _styledText.Blocks[1].Text);
    //    //    Assert.AreEqual(4, _styledText.Blocks[2].Inlines.Count());
    //    //    Assert.AreEqual("fo\rcfo\n", _styledText.Blocks[2].Text);
    //    //}

    //    /// <summary>
    //    /// SetColor後のInlineの統合チェック
    //    /// </summary>
    //    //[TestMethod]
    //    //public void TestMergeInlinesAfterSetColor1() {
    //    //    Assert.AreEqual(4, _para1.Inlines.Count());

    //    //    var range = _styledText.CreateRange(0, 6);
    //    //    range.SetColor(Color.White);
    //    //    Assert.AreEqual("foobar", _styledText.Blocks[0].Inlines.ElementAt(0).Text);
    //    //    Assert.AreEqual(3, _para1.Inlines.Count());
    //    //}

    //    //[TestMethod]
    //    //public void TestMergeInlinesAfterSetColor2() {
    //    //    Assert.AreEqual(4, _para1.Inlines.Count());
    //    //    Assert.AreEqual(4, _para2.Inlines.Count());

    //    //    var range = _styledText.CreateRange(0, 11);
    //    //    range.SetColor(Color.White);

    //    //    Assert.AreEqual("foobarbaz", _para1.Inlines.ElementAt(0).Text);
    //    //    Assert.AreEqual(2, _para1.Inlines.Count());

    //    //    Assert.AreEqual("a", _para2.Inlines.ElementAt(0).Text);
    //    //    Assert.AreEqual(5, _para2.Inlines.Count());
    //    //}

    //    //[TestMethod]
    //    //public void TestMergeInlinesAfterSetColor3() {
    //    //    Assert.AreEqual(4, _para1.Inlines.Count());
    //    //    Assert.AreEqual(4, _para2.Inlines.Count());

    //    //    var range = _styledText.CreateRange(11, 4);
    //    //    range.SetColor(Color.White);

    //    //    Assert.AreEqual("a", _para2.Inlines.ElementAt(0).Text);
    //    //    Assert.AreEqual("fo", _para2.Inlines.ElementAt(1).Text);
    //    //    Assert.AreEqual("\r", _para2.Inlines.ElementAt(2).Text);
    //    //    Assert.AreEqual("c", _para2.Inlines.ElementAt(3).Text);
    //    //    Assert.AreEqual("fo", _para2.Inlines.ElementAt(4).Text);
    //    //    Assert.AreEqual(6, _para2.Inlines.Count());
    //    //}

    //    //[TestMethod]
    //    //public void TestMergeInlinesAfterSetColor4() {
    //    //    _foo.Color = Color.White;
    //    //    _bar.Color = Color.Black;
    //    //    _baz.Color = Color.White;

    //    //    Assert.AreEqual(4, _para1.Inlines.Count());
    //    //    Assert.AreEqual(4, _para2.Inlines.Count());

    //    //    var range = _styledText.CreateRange(3, 3);
    //    //    range.SetColor(Color.White);

    //    //    Assert.AreEqual("foobarbaz", _para1.Inlines.ElementAt(0).Text);
    //    //    Assert.AreEqual(2, _para1.Inlines.Count());
    //    //}
    //}
}
