/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Figures;
using Mkamo.Common.Externalize;
using System.Drawing;
using System.Reflection;
using Mkamo.Memopad.Properties;

namespace Mkamo.Memopad.Internal.Figures {
    [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = false)]
    internal class MemoTextWidthHandleFigure: AbstractNode {
        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================
        public MemoTextWidthHandleFigure() {
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        protected override void PaintSelf(Graphics g) {
            var img = Resources.text_width_handle;
            g.DrawImage(img, new Rectangle(Location, img.Size));
            //using (_ResourceCache.UseResource()) {
            //    var brush = _BrushResource;
            //    var pen = _PenResource;

            //    var leftTriPts = new Point[] {
            //        new Point((Left + Right) / 2 - 2, Top),
            //        new Point((Left + Right) / 2 - 2, Bottom - 2),
            //        new Point(Left + 1, (Top + Bottom) / 2 - 1),
            //    };
            //    var rightTriPts = new Point[] {
            //        new Point((Left + Right) / 2 + 1, Top),
            //        new Point((Left + Right) / 2 + 1, Bottom - 2),
            //        new Point(Right - 2, (Top + Bottom) / 2 - 1),
            //    };

            //    if (IsBackgroundEnabled && brush != null) {
            //        g.FillPolygon(brush, leftTriPts);
            //        g.FillPolygon(brush, rightTriPts);
            //    }
            //    if (IsForegroundEnabled && BorderWidth > 0) {
            //        g.DrawPolygon(pen, leftTriPts);
            //        g.DrawPolygon(pen, rightTriPts);
            //    }
            //}
        }

    }
}
