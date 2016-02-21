/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using Mkamo.Common.Win32.Core;

namespace Mkamo.Common.Win32.Gdi32 {
    public static class Gdi32PI {
        [DllImport("Gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr gdiObj);
        [DllImport("GDI32.dll")]
        public static extern bool DeleteObject(IntPtr gdiObj);

        [DllImport("Gdi32.dll")]
        public static extern int SetTextColor(IntPtr hDC, int crColor);
        [DllImport("Gdi32.dll")]
        public static extern int SetBkColor(IntPtr hDC, int crColor);
        [DllImport("Gdi32.dll")]
        public static extern int SetBkMode(IntPtr hdc, BkMode iBkMode);


        [DllImport("Gdi32.dll")]
        public static extern int SetTextAlign(IntPtr hdc, TextAlignModes fMode);
        [DllImport("Gdi32.dll")]
        public static extern TextAlignModes GetTextAlign(IntPtr hdc);

        [DllImport("Gdi32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern bool ExtTextOutW(
            IntPtr hDC, int x, int y, ExtTextOutFormatOptions wOptions,
            ref RECT lpRect, string lpString, int nCount, int[] lpDX
        );
        [DllImport("Gdi32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern Int32 GetTextExtentExPointW(
            IntPtr hDC, string text, int textLen, int maxWidth, out int fitLength, int[] dx, out SIZE size
        );

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);
        [DllImport("gdi32.dll")]
        public static extern bool DeleteDC(IntPtr hdc);

        /// <summary>
        ///    Performs a bit-block transfer of the color data corresponding to a
        ///    rectangle of pixels from the specified source device context into
        ///    a destination device context.
        /// </summary>
        /// <param name="hdc">Handle to the destination device context.</param>
        /// <param name="nXDest">The leftmost x-coordinate of the destination rectangle (in pixels).</param>
        /// <param name="nYDest">The topmost y-coordinate of the destination rectangle (in pixels).</param>
        /// <param name="nWidth">The width of the source and destination rectangles (in pixels).</param>
        /// <param name="nHeight">The height of the source and the destination rectangles (in pixels).</param>
        /// <param name="hdcSrc">Handle to the source device context.</param>
        /// <param name="nXSrc">The leftmost x-coordinate of the source rectangle (in pixels).</param>
        /// <param name="nYSrc">The topmost y-coordinate of the source rectangle (in pixels).</param>
        /// <param name="dwRop">A raster-operation code.</param>
        /// <returns>
        ///    <c>true</c> if the operation succeeded, <c>false</c> otherwise.
        /// </returns>
        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(
            IntPtr hdc,
            int nXDest,
            int nYDest,
            int nWidth,
            int nHeight,
            IntPtr hdcSrc,
            int nXSrc,
            int nYSrc,
            TernaryRasterOperations dwRop
        );
    }
}
