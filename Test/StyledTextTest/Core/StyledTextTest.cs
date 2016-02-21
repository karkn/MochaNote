/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mkamo.Common.DataType;
using Mkamo.StyledText.Writer;
using System.IO;

namespace Mkamo.StyledText.Core {

    [TestClass]
    public class StyledTextTest {

        [TestMethod]
        public void TestXmlSerialization() {
            var stext = new StyledText(CreateParagraph());
            stext.InsertAfter(CreateParagraph());
            stext.InsertAfter(CreateParagraph());

            var tmpFilename = Path.GetTempFileName();
            XmlUtil.Save(tmpFilename, stext);

            var deserialized = XmlUtil.Load(tmpFilename);
            Assert.AreEqual(3, deserialized.BlocksCount);

            var block = deserialized.Blocks.First();
            Assert.AreEqual(3, block.LineCount);

            var line = block.LineSegments.First();
            Assert.AreEqual(2, line.Inlines.Count());

            var run = line.Inlines.First();
            Assert.AreEqual("0123456789", run.Text);

            var lbreak = line.Inlines.Last();
            Assert.IsTrue(lbreak is LineBreak);

            File.Delete(tmpFilename);
        }

        [TestMethod]
        public void TestCopyRange() {
            var stext = new StyledText(CreateParagraph());
            stext.InsertAfter(CreateParagraph());
            stext.InsertAfter(CreateParagraph());

            var copy = "";

            /// 最初
            copy = ((StyledText) stext.CopyRange(new Range(0, 3))).Text;
            Assert.AreEqual("012\n", copy);

            /// rangeが一つのLineSegment内
            copy = ((StyledText) stext.CopyRange(new Range(2, 3))).Text;
            Assert.AreEqual("234\n", copy);

            /// 行またぐ
            copy = ((StyledText) stext.CopyRange(new Range(7, 6))).Text;
            Assert.AreEqual("789\r01\n", copy);

            /// 段落をまたぐ
            copy = ((StyledText) stext.CopyRange(new Range(30, 5))).Text;
            Assert.AreEqual("89\n01\n", copy);

            /// 段落を含む
            copy = ((StyledText) stext.CopyRange(new Range(30, 40))).Text;
            Assert.AreEqual("89\n0123456789\r0123456789\r0123456789\n0123\n", copy);

            /// 最後 (改段落は入っていてもいなくても同じ結果)
            copy = ((StyledText) stext.CopyRange(new Range(94, 4))).Text;
            Assert.AreEqual("6789\n", copy);
            copy = ((StyledText) stext.CopyRange(new Range(94, 5))).Text;
            Assert.AreEqual("6789\n", copy);
        }

        [TestMethod]
        public void TestCopyInlines() {
            var stext = new StyledText(CreateParagraph());
            stext.InsertAfter(CreateParagraph());
            stext.InsertAfter(CreateParagraph());

            var copy = default(IEnumerable<Inline>);

            /// 最初
            copy = stext.CopyInlines(new Range(0, 3));
            Assert.AreEqual(1, copy.Count());
            Assert.AreEqual("012", copy.ElementAt(0).Text);

            /// rangeが一つのLineSegment内
            copy = stext.CopyInlines(new Range(2, 3));
            Assert.AreEqual(1, copy.Count());
            Assert.AreEqual("234", copy.ElementAt(0).Text);

            /// 行またぐ
            copy = stext.CopyInlines(new Range(7, 6));
            Assert.AreEqual(3, copy.Count());
            Assert.AreEqual("789", copy.ElementAt(0).Text);
            Assert.AreEqual("\r", copy.ElementAt(1).Text);
            Assert.AreEqual("01", copy.ElementAt(2).Text);

            /// 段落をまたぐ
            copy = stext.CopyInlines(new Range(30, 5));
            Assert.AreEqual(3, copy.Count());
            Assert.AreEqual("89", copy.ElementAt(0).Text);
            Assert.AreEqual("\n", copy.ElementAt(1).Text);
            Assert.AreEqual("01", copy.ElementAt(2).Text);

            /// 段落を含む
            copy = stext.CopyInlines(new Range(30, 40));
            Assert.AreEqual(9, copy.Count());
            Assert.AreEqual("89", copy.ElementAt(0).Text);
            Assert.AreEqual("\n", copy.ElementAt(1).Text);
            Assert.AreEqual("0123456789", copy.ElementAt(2).Text);
            Assert.AreEqual("\r", copy.ElementAt(3).Text);
            Assert.AreEqual("0123456789", copy.ElementAt(4).Text);
            Assert.AreEqual("\r", copy.ElementAt(5).Text);
            Assert.AreEqual("0123456789", copy.ElementAt(6).Text);
            Assert.AreEqual("\n", copy.ElementAt(7).Text);
            Assert.AreEqual("0123", copy.ElementAt(8).Text);

            /// 最後
            copy = stext.CopyInlines(new Range(94, 4));
            Assert.AreEqual(1, copy.Count());
            Assert.AreEqual("6789", copy.ElementAt(0).Text);

            copy = stext.CopyInlines(new Range(94, 5));
            Assert.AreEqual(2, copy.Count());
            Assert.AreEqual("6789", copy.ElementAt(0).Text);
            Assert.AreEqual("\n", copy.ElementAt(1).Text);
        }

        // ------------------------------
        // private
        // ------------------------------
        private Paragraph CreateParagraph() {
            var para = new Paragraph(new Run("0123456789"));
            para.InsertAfter(new LineSegment(new Run("0123456789")));
            para.InsertAfter(new LineSegment(new Run("0123456789")));
            return para;
        }
    }


    //[TestClass]
    //public class StyledTextTest: StyledTextTestBase {
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
    //    /// Acceptのアクセス順序テスト
    //    /// </summary>
    //    [TestMethod]
    //    public void TestAccept() {
    //        var elems = new Flow[] {
    //            _styledText,

    //            _para1,
    //            _foo,
    //            _bar,
    //            _baz,
    //            _para1BlockBreak,

    //            _para2,
    //            _afo,
    //            _bfo,
    //            _cfo,
    //            _para2BlockBreak
    //        };

    //        var i = 0;
    //        _styledText.Accept(
    //            elem => {
    //                Assert.AreEqual(elems[i], elem);
    //                ++i;
    //                return false;
    //            }
    //        );
    //    }

    //    /// <summary>
    //    /// Has/GetPrevInline()テスト
    //    /// </summary>
    //    [TestMethod]
    //    public void TestGetPrevInline() {
    //        Assert.IsFalse(_styledText.HasPrevInline(_foo));
    //        Assert.IsTrue(_styledText.HasPrevInline(_bar));
    //        Assert.IsTrue(_styledText.HasPrevInline(_baz));

    //        Assert.AreEqual(_foo, _styledText.GetPrevInline(_bar));
    //        Assert.AreEqual(_bar, _styledText.GetPrevInline(_baz));
    //    }

    //    /// <summary>
    //    /// Has/GetNextInline()テスト
    //    /// </summary>
    //    [TestMethod]
    //    public void TestGetNextInline() {
    //        Assert.IsTrue(_styledText.HasNextInline(_foo));
    //        Assert.IsTrue(_styledText.HasNextInline(_bar));
    //        Assert.IsTrue(_styledText.HasNextInline(_baz));

    //        Assert.AreEqual(_bar, _styledText.GetNextInline(_foo));
    //        Assert.AreEqual(_baz, _styledText.GetNextInline(_bar));
    //        Assert.IsTrue(_styledText.GetNextInline(_baz) is BlockBreak);
    //    }


    //    /// <summary>
    //    /// CreateRange()とTextテスト
    //    /// </summary>
    //    //[TestMethod]
    //    //public void TestCreateRangeText() {
    //    //    /// Inlineをちょうど含む
    //    //    Assert.AreEqual("foo", _styledText.CreateRange(0, 3).Text);
    //    //    Assert.AreEqual("foobar", _styledText.CreateRange(0, 6).Text);
    //    //    Assert.AreEqual("foobarbaz\n", _styledText.CreateRange(0, 10).Text);
    //    //    Assert.AreEqual("afo\rcfo", _styledText.CreateRange(10, 7).Text);

    //    //    /// Inlineをまたぐ
    //    //    Assert.AreEqual("ooba", _styledText.CreateRange(1, 4).Text);
    //    //    Assert.AreEqual("oobarba", _styledText.CreateRange(1, 7).Text);
    //    //    Assert.AreEqual("fo\rcf", _styledText.CreateRange(11, 5).Text);

    //    //    /// Blockをまたぐ
    //    //    Assert.AreEqual("baz\nafo", _styledText.CreateRange(6, 7).Text);
    //    //    Assert.AreEqual("az\naf", _styledText.CreateRange(7, 5).Text);

    //    //    /// 全部
    //    //    Assert.AreEqual(_styledText.Text, _styledText.CreateRange(0, _styledText.Length).Text);
    //    //}

    //    [TestMethod]
    //    public void TestCloneRange() {
    //        //var clone = _styledText.Clone(new Range(3, 2));
    //        //Assert.AreEqual(1, clone.Length);
    //        //Assert.AreEqual("ba\n", clone[0].Text);

    //        //clone = _styledText.Clone(new Range(0, 6));
    //        //Assert.AreEqual(1, clone.Length);
    //        //Assert.AreEqual("foobar\n", clone[0].Text);
            
    //        //clone = _styledText.Clone(new Range(0, 18));
    //        //Assert.AreEqual(2, clone.Length);
    //        //Assert.AreEqual("foobarbaz\n", clone[0].Text);
    //        //Assert.AreEqual("afo\rcfo\n", clone[1].Text);
            
    //        //clone = _styledText.Clone(new Range(4, 11));
    //        //Assert.AreEqual(2, clone.Length);
    //        //Assert.AreEqual("arbaz\n", clone[0].Text);
    //        //Assert.AreEqual("afo\rc\n", clone[1].Text);
          
    //    }

    //}
}
