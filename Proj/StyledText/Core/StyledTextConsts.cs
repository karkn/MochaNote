/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Mkamo.StyledText.Core {
    public static class StyledTextConsts {
        // ========================================
        // static field
        // ========================================
        /// <summary>
        /// StyledTextをクリップボードに格納するときのフォーマット．
        /// </summary>
        public static readonly DataFormats.Format BlocksAndInlinesFormat = DataFormats.GetFormat("Mkamo.StyledText.BlocksAndInlinesFormat");

        internal const int IndentPaddingWidth = 20;
    }
}
