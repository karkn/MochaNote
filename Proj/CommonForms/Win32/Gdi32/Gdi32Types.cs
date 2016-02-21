/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Common.Win32.Gdi32 {
    [Flags]
    public enum ExtTextOutFormatOptions: int {
        ETO_NONE = 0x0000,
        ETO_OPAQUE                   = 0x0002,
        ETO_CLIPPED                  = 0x0004,
        ETO_GLYPH_INDEX              = 0x0010,
        ETO_RTLREADING               = 0x0080,
        ETO_NUMERICSLOCAL            = 0x0400,
        ETO_NUMERICSLATIN            = 0x0800,
        ETO_IGNORELANGUAGE           = 0x1000,
        ETO_PDY                      = 0x2000,
    }

    public enum BkMode: int {
        TRANSPARENT = 1,
        OPAQUE = 2,
    }


    [Flags]
    public enum TextAlignModes: int {
        TA_NOUPDATECP = 0,
        TA_UPDATECP   = 1,

        TA_LEFT       = 0,
        TA_RIGHT      = 2,
        TA_CENTER     = 6,

        TA_TOP        = 0,
        TA_BOTTOM     = 8,
        TA_BASELINE   = 24,
    }


    /// <summary>
    ///     Specifies a raster-operation code. These codes define how the color data for the
    ///     source rectangle is to be combined with the color data for the destination
    ///     rectangle to achieve the final color.
    /// </summary>
    public  enum TernaryRasterOperations : int {
        /// <summary>dest = source</summary>
        SRCCOPY = 0x00CC0020,
        /// <summary>dest = source OR dest</summary>
        SRCPAINT = 0x00EE0086,
        /// <summary>dest = source AND dest</summary>
        SRCAND = 0x008800C6,
        /// <summary>dest = source XOR dest</summary>
        SRCINVERT = 0x00660046,
        /// <summary>dest = source AND (NOT dest)</summary>
        SRCERASE = 0x00440328,
        /// <summary>dest = (NOT source)</summary>
        NOTSRCCOPY = 0x00330008,
        /// <summary>dest = (NOT src) AND (NOT dest)</summary>
        NOTSRCERASE = 0x001100A6,
        /// <summary>dest = (source AND pattern)</summary>
        MERGECOPY = 0x00C000CA,
        /// <summary>dest = (NOT source) OR dest</summary>
        MERGEPAINT = 0x00BB0226,
        /// <summary>dest = pattern</summary>
        PATCOPY    = 0x00F00021,
        /// <summary>dest = DPSnoo</summary>
        PATPAINT = 0x00FB0A09,
        /// <summary>dest = pattern XOR dest</summary>
        PATINVERT = 0x005A0049,
        /// <summary>dest = (NOT dest)</summary>
        DSTINVERT = 0x00550009,
        /// <summary>dest = BLACK</summary>
        BLACKNESS = 0x00000042,
        /// <summary>dest = WHITE</summary>
        WHITENESS = 0x00FF0062
    }
}
