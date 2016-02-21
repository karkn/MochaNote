/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace Mkamo.Common.Forms.Drawing {
    [Serializable]
    public enum GraphicQuality {
        MaxQuality,
        HighQuality,
        LowQuality,
        DefaultQuality,
    }

    public static class GraphicsUtil {
        // ========================================
        // method
        // ========================================
        public static void SetupGraphics(Graphics g, GraphicQuality quality) {
            switch (quality) {
                case GraphicQuality.MaxQuality: {
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.CompositingQuality = CompositingQuality.HighQuality;
					g.PixelOffsetMode = PixelOffsetMode.Default;
                    break;
                }
                case GraphicQuality.HighQuality: {
					g.SmoothingMode = SmoothingMode.HighQuality;
                    g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
					g.InterpolationMode = InterpolationMode.High;
                    g.CompositingQuality = CompositingQuality.Default;
					g.PixelOffsetMode = PixelOffsetMode.Default;
                    break;
                }
                case GraphicQuality.LowQuality: {
					g.SmoothingMode = SmoothingMode.HighSpeed;
					g.TextRenderingHint = TextRenderingHint.SystemDefault;
					g.InterpolationMode = InterpolationMode.Low;
					g.CompositingQuality = CompositingQuality.HighSpeed;
					g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
                    break;
                }
                case GraphicQuality.DefaultQuality: {
                    g.SmoothingMode = SmoothingMode.Default;
                    g.TextRenderingHint = TextRenderingHint.SystemDefault;
					g.InterpolationMode = InterpolationMode.Default;
					g.CompositingQuality = CompositingQuality.Default;
					g.PixelOffsetMode = PixelOffsetMode.Default;
                    break;
                }
            }
        }
    }

}
