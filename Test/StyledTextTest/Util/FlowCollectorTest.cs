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

namespace Mkamo.StyledText.Util {
    ///// <summary>
    ///// InlineCollectorTest の概要の説明
    ///// </summary>
    //[TestClass]
    //public class FlowCollectorTest: StyledTextTestBase {
    //    // ========================================
    //    // field
    //    // ========================================

    //    // ========================================
    //    // constructor
    //    // ========================================
    //    public FlowCollectorTest() {
    //    }


    //    // ========================================
    //    // method
    //    // ========================================
    //    [TestInitialize()]
    //    public override void Init() {
    //        base.Init();
    //    }
        

    //    [TestMethod]
    //    public void TestCollectInlines() {
    //        var col = new FlowCollector(_styledText);

    //        /// 一致
    //        var result = col.Collect(new Range(0, 18)); /// 0-17
    //        Assert.AreEqual(null, result.FirstInline);
    //        Assert.AreEqual(null, result.LastInline);
    //        Assert.AreEqual(8, result.MiddleInlines.Length);

    //        /// 後方一致
    //        result = col.Collect(new Range(1, 17)); /// 1-17
    //        Assert.AreEqual(_foo, result.FirstInline);
    //        Assert.AreEqual(new Range(1, 2), result.FirstInlineRange);
    //        Assert.AreEqual(new Range(1, 2), result.FirstInlineRangeInStyledText);
    //        Assert.AreEqual(null, result.LastInline);
    //        Assert.AreEqual(7, result.MiddleInlines.Length);
        
    //        /// 前方一致
    //        result = col.Collect(new Range(0, 15)); /// 0-14
    //        Assert.AreEqual(null, result.FirstInline);
    //        Assert.AreEqual(_cfo, result.LastInline);
    //        Assert.AreEqual(new Range(0, 1), result.LastInlineRange);
    //        Assert.AreEqual(new Range(14, 1), result.LastInlineRangeInStyledText);
    //        Assert.AreEqual(6, result.MiddleInlines.Length);

    //        /// 内部
    //        result = col.Collect(new Range(1, 15)); /// 1-15
    //        Assert.AreEqual(_foo, result.FirstInline);
    //        Assert.AreEqual(_cfo, result.LastInline);
    //        Assert.AreEqual(new Range(1, 2), result.FirstInlineRange);
    //        Assert.AreEqual(new Range(1, 2), result.FirstInlineRangeInStyledText);
    //        Assert.AreEqual(new Range(0, 2), result.LastInlineRange);
    //        Assert.AreEqual(new Range(14, 2), result.LastInlineRangeInStyledText);
    //        Assert.AreEqual(5, result.MiddleInlines.Length);

    //    }

    //    [TestMethod]
    //    public void TestCollectBlocks() {
    //        var col = new FlowCollector(_styledText);

    //        var result = col.Collect(new Range(0, 18));
    //        Assert.AreEqual(null, result.FirstBlock);
    //        Assert.AreEqual(null, result.LastBlock);
    //        Assert.AreEqual(2, result.MiddleBlocks.Length);

    //        result = col.Collect(new Range(1, 17));
    //        Assert.AreEqual(_para1, result.FirstBlock);
    //        Assert.AreEqual(new Range(1, 9), result.FirstBlockRange);
    //        Assert.AreEqual(new Range(1, 9), result.FirstBlockRangeInStyledText);
    //        Assert.AreEqual(null, result.LastBlock);
    //        Assert.AreEqual(1, result.MiddleBlocks.Length);
        
    //        result = col.Collect(new Range(0, 15));
    //        Assert.AreEqual(null, result.FirstBlock);
    //        Assert.AreEqual(_para2, result.LastBlock);
    //        Assert.AreEqual(new Range(0, 5), result.LastBlockRange);
    //        Assert.AreEqual(new Range(10, 5), result.LastBlockRangeInStyledText);
    //        Assert.AreEqual(1, result.MiddleBlocks.Length);

    //    }

    //    [TestMethod]
    //    public void TestCollectSkip() {
    //        var col = new FlowCollector(_styledText);

    //        var result = col.Collect(new Range(0, 18), false, true);
    //        Assert.AreEqual(null, result.FirstInline);
    //        Assert.AreEqual(null, result.LastInline);
    //        Assert.AreEqual(0, result.MiddleInlines.Length);
    //        Assert.AreEqual(null, result.FirstBlock);
    //        Assert.AreEqual(null, result.LastBlock);
    //        Assert.AreEqual(2, result.MiddleBlocks.Length);

    //        result = col.Collect(new Range(0, 18), true, false);
    //        Assert.AreEqual(null, result.FirstInline);
    //        Assert.AreEqual(null, result.LastInline);
    //        Assert.AreEqual(8, result.MiddleInlines.Length);
    //        Assert.AreEqual(null, result.FirstBlock);
    //        Assert.AreEqual(null, result.LastBlock);
    //        Assert.AreEqual(0, result.MiddleBlocks.Length);

    //    }
    //}
}
