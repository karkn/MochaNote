/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Externalize;
using System.Drawing;

namespace Mkamo.Figure.Figures {
    public class Label: AbstractNode {
        // ========================================
        // method
        // ========================================
        // ------------------------------
        // protected
        // ------------------------------
        // === AbstractFigure ==========
        protected override void PaintSelf(Graphics g) {
            using (_ResourceCache.UseResource()) {
                //var brush = _BrushResource;
                //var pen = _PenResource;
                //if (IsBackgroundEnabled && brush != null) {
                //    /// 背景
                //    g.FillRectangle(brush, Left, Top, Width - 1, Height - 1);
                //}
                //if (IsForegroundEnabled && BorderWidth > 0) {
                //    /// 枠
                //    g.DrawRectangle(pen, Left, Top, Width - 1, Height - 1);
                //}

                PaintText(g);
                PaintSelection(g);
                PaintStyledText(g);
            }
        }
    }
}
