/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.StyledText.Core;

namespace Mkamo.StyledText.Writer {
    public class PlainTextWriter {
        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public string ToPlainText(IEnumerable<Flow> blocksAndInlines, PlainTextWriterSettings settings) {
            var buf = new StringBuilder();
            foreach (var flow in blocksAndInlines) {
                buf.Append(flow.ToPlainText(settings));
            }
            return buf.ToString();
        }

        public string ToPlainText(IEnumerable<Flow> blocksAndInlines) {
            var settings = new PlainTextWriterSettings();
            return ToPlainText(blocksAndInlines, settings);
        }
    }

    public class PlainTextWriterSettings {
        public PlainTextWriterSettings() {
            UnorderedListBullet = "  - ";
            StarListBullet = "  * ";
            LeftArrowListBullet = "  <-";
            RightArrowListBullet = "  ->";
            CheckBoxCheckedListBullet = "  * ";
            CheckBoxUncheckedListBullet = "  - ";
            CheckBoxIndeterminateListBullet = "  + ";
        }

        public string UnorderedListBullet { get; set; }
        public string StarListBullet { get; set; }
        public string LeftArrowListBullet { get; set; }
        public string RightArrowListBullet { get; set; }
        public string CheckBoxCheckedListBullet { get; set; }
        public string CheckBoxUncheckedListBullet { get; set; }
        public string CheckBoxIndeterminateListBullet { get; set; }
    }
}
