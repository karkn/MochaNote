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

namespace Mkamo.StyledText.Core {
    [TestClass]
    public class LineSegmentTest {

        [TestMethod]
        public void TestCopyRange() {
            var line = new LineSegment();

            line.InsertAfter(new Run("0123456789"));
            line.InsertAfter(new Run("0123456789"));
            line.InsertAfter(new Run("0123456789"));

            var copy = default(LineSegment);

            /// 最初
            copy = ((LineSegment) line.CopyRange(new Range(0, 5)));
            Assert.AreEqual("01234\r", copy.Text);

            /// rangeが一つのInline内
            copy = ((LineSegment) line.CopyRange(new Range(2, 3)));
            Assert.AreEqual("234\r", copy.Text);

            /// ひとつまたぐ
            copy = ((LineSegment) line.CopyRange(new Range(7, 5)));
            Assert.AreEqual("78901\r", copy.Text);

            /// 一つcontains
            copy = ((LineSegment) line.CopyRange(new Range(7, 15)));
            Assert.AreEqual("789012345678901\r", copy.Text);

            /// 最後のLineBreakは範囲に入っていてもいなくても結果が同じになる
            copy = ((LineSegment) line.CopyRange(new Range(25, 5)));
            Assert.AreEqual("56789\r", copy.Text);
            copy = ((LineSegment) line.CopyRange(new Range(25, 6)));
            Assert.AreEqual("56789\r", copy.Text);
        }

        [TestMethod]
        public void TestCopyInlines() {
            var line = new LineSegment();

            line.InsertAfter(new Run("0123456789"));
            line.InsertAfter(new Run("0123456789"));
            line.InsertAfter(new Run("0123456789"));

            var copy = default(IEnumerable<Inline>);

            /// 最初
            copy = line.CopyInlines(new Range(0, 5));
            Assert.AreEqual(1, copy.Count());
            Assert.AreEqual("01234", copy.ElementAt(0).Text);

            /// rangeが一つのInline内
            copy = line.CopyInlines(new Range(2, 3));
            Assert.AreEqual(1, copy.Count());
            Assert.AreEqual("234", copy.ElementAt(0).Text);

            /// ひとつまたぐ
            copy = line.CopyInlines(new Range(7, 5));
            Assert.AreEqual(2, copy.Count());
            Assert.AreEqual("789", copy.ElementAt(0).Text);
            Assert.AreEqual("01", copy.ElementAt(1).Text);

            /// 一つ含んでまたぐ
            copy = line.CopyInlines(new Range(7, 15));
            Assert.AreEqual(3, copy.Count());
            Assert.AreEqual("789", copy.ElementAt(0).Text);
            Assert.AreEqual("0123456789", copy.ElementAt(1).Text);
            Assert.AreEqual("01", copy.ElementAt(2).Text);

            /// 最後
            copy = line.CopyInlines(new Range(25, 5));
            Assert.AreEqual(1, copy.Count());
            Assert.AreEqual("56789", copy.ElementAt(0).Text);

            copy = line.CopyInlines(new Range(25, 6));
            Assert.AreEqual(2, copy.Count());
            Assert.AreEqual("56789", copy.ElementAt(0).Text);
            Assert.AreEqual("\r", copy.ElementAt(1).Text);

        }
    }
}
