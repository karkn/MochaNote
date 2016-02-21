/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mkamo.StyledText.Core;

namespace Mkamo.StyledText.Writer {
    [TestClass]
    public class HtmlWriterTest {
        [TestMethod]
        public void TestToHtmlBodyContent1() {
            var flows = new List<Flow>();

            {
                var para = new Paragraph(new Run("aho"));
                para.ListKind = ListKind.Unordered;
                flows.Add(para);
            }

            {
                var para = new Paragraph(new Run("baka"));
                para.ListKind = ListKind.Unordered;
                flows.Add(para);
            }

            {
                var para = new Paragraph(new Run("hoge"));
                para.ListKind = ListKind.Unordered;
                flows.Add(para);
            }

            var writer = new HtmlWriter();

            var html = writer.ToHtmlBodyContent(flows);
            var expected = @"<ul><li>aho</li><li>baka</li><li>hoge</li></ul>";
            Assert.AreEqual(expected, html);
        }

        [TestMethod]
        public void TestToHtmlBodyContent2() {
            var flows = new List<Flow>();

            {
                var para = new Paragraph(new Run("aho"));
                para.ListKind = ListKind.Unordered;
                flows.Add(para);
            }

            {
                var para = new Paragraph(new Run("baka"));
                para.ListKind = ListKind.Unordered;
                para.ListLevel = 1;
                flows.Add(para);
            }

            {
                var para = new Paragraph(new Run("hoge"));
                para.ListKind = ListKind.Unordered;
                flows.Add(para);
            }

            var writer = new HtmlWriter();

            var html = writer.ToHtmlBodyContent(flows);
            var expected = @"<ul><li>aho</li><ul><li>baka</li></ul><li>hoge</li></ul>";
            Assert.AreEqual(expected, html);
        }

    }
}
