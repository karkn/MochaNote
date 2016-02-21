/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Drawing;
using Mkamo.Common.Diagnostics;
using Mkamo.Figure.Core;
using Mkamo.Common.Forms.Table;
using Mkamo.Figure.Layouts;
using Mkamo.Common.Externalize;

namespace Mkamo.Figure.Figures {
    using StyledText = StyledText.Core.StyledText;

    [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = false)]
    public class TableCellFigure: AbstractNode {

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
        protected override void PaintSelf(Graphics g) {
            using (_ResourceCache.UseResource()) {
                var brush = _BrushResource;
                var pen = _PenResource;
                if (IsBackgroundEnabled && brush != null) {
                    /// 背景
                    g.FillRectangle(brush, Left, Top, Width - 1, Height - 1);
                }
                if (IsForegroundEnabled && BorderWidth > 0) {
                    /// 枠
                    g.DrawRectangle(pen, Left, Top, Width - 1, Height - 1);
                }

                PaintText(g);
                PaintSelection(g);
                PaintStyledText(g);
            }
        }
    }
}
