/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.ComponentModel;

namespace Mkamo.Common.Win32.Gdi32 {
    public static class Gdi32Util {
        // to int 0x00bbggrr, where bb, gg, rr is byte
        public static int ToColorRefInt(Color color) {
            return ((color.R | (color.G << 0x08)) | ((int) ((ulong) (color.B << 0x10))));
        }

        public static void CopyGraphics(Graphics tgtGra, Rectangle tgtRect, Graphics srcGra, Point srcLoc) {
            var srcHdc = srcGra.GetHdc();
            var tgtHdc = tgtGra.GetHdc();

            try {
                Gdi32PI.BitBlt(
                    tgtHdc, tgtRect.X, tgtRect.Y, tgtRect.Width, tgtRect.Height, srcHdc, srcLoc.X, srcLoc.Y, TernaryRasterOperations.SRCCOPY
                );

            } finally {
                srcGra.ReleaseHdc(srcHdc);
                tgtGra.ReleaseHdc(tgtHdc);
            }

        }
    }
}
