/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Mkamo.Common.Forms.Util {
    public static class ColorUtil {
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
        public static bool IsDark(Color color) {
            var a = color.A;
            var r = color.R;
            var g = color.G;
            var b = color.B;

            if (r < 128 || g < 128 || b < 128) {
                return true;
            }
            return false;
        }
    }
}
