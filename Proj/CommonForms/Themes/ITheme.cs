/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Mkamo.Common.Forms.Themes {
    public interface ITheme {
        // ========================================
        // property
        // ========================================
        // --- font ---
        Font CaptionFont { get; set; }
        Font DescriptionFont { get; set; }
        Font InputFont { get; set; }
        Font MenuFont { get; set; }
        Font StatusFont { get; set; }

        // --- color ---
        Color DarkBackColor { get; set; }

        // ========================================
        // method
        // ========================================
        void ReplaceFont(string oldFontName, string newFontName);
    }
}
