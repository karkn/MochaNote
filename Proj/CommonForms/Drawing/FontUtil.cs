/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Mkamo.Common.Forms.Drawing {
    public static class FontUtil {
        public static bool IsMeiryo(Font font) {
            return
                string.Equals(font.Name, "Meiryo", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(font.Name, "メイリオ", StringComparison.OrdinalIgnoreCase);
        }
    }
}
