/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mkamo.StyledText.Core;

namespace Mkamo.StyledText.Util {
    public static class ClipboardUtil {
        // ========================================
        // method
        // ========================================
        public static bool ContainsBlocksAndInlines() {
            return Clipboard.GetDataObject().GetDataPresent(StyledTextConsts.BlocksAndInlinesFormat.Name);
        }

        public static bool ContainsText() {
            return Clipboard.ContainsText(TextDataFormat.UnicodeText);
        }
    }
}
