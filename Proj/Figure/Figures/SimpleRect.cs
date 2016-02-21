/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using System.Drawing;
using Mkamo.Common.Externalize;

namespace Mkamo.Figure.Figures {
    public class SimpleRect: AbstractNode {
        // ========================================
        // method
        // ========================================

        // ------------------------------
        // protected
        // ------------------------------
        // === AbstractFigure ==========
        protected override void PaintSelf(Graphics g) {
            using (_ResourceCache.UseResource()) {
                var brush = _BrushResource;
                if (IsBackgroundEnabled && brush != null) {
                    g.FillRectangle(brush, Left, Top, Width - 1, Height - 1);
                }
                if (IsForegroundEnabled && BorderWidth > 0) {
                    g.DrawRectangle(_PenResource, Left, Top, Width - 1, Height - 1);
                }
                PaintText(g);
                PaintSelection(g);
                PaintStyledText(g);
            }
        }

    }
}
