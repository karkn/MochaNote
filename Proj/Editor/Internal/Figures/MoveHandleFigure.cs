/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Figures;
using System.Drawing;
using System.Reflection;
using System.Drawing.Drawing2D;

namespace Mkamo.Editor.Internal.Figures {
    [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = false)]
    internal class MoveHandleFigure: SimpleRect {
        protected override void PaintSelf(System.Drawing.Graphics g) {
            using (_ResourceCache.UseResource())
            using (var path = new GraphicsPath()) {
                var bounds = Bounds;

                var roundSize = 2;

                path.AddLine(
                    new Point(bounds.Left + roundSize, bounds.Top),
                    new Point(bounds.Right - roundSize - 1, bounds.Top)
                );
                path.AddArc(
                    new Rectangle(
                        bounds.Right - roundSize * 2 - 1,
                        bounds.Top,
                        roundSize * 2,
                        roundSize * 2
                    ),
                    270,
                    90
                );
                path.AddLine(
                    new Point(bounds.Right - 1, bounds.Top + roundSize),
                    new Point(bounds.Right - 1, bounds.Bottom - 1)
                );
                path.AddLine(
                    new Point(bounds.Right - 1, bounds.Bottom - 1),
                    new Point(bounds.Left, bounds.Bottom - 1)
                );
                path.AddLine(
                    new Point(bounds.Left, bounds.Bottom - 1),
                    new Point(bounds.Left, bounds.Top + roundSize)
                );
                path.AddArc(
                    new Rectangle(bounds.Left, bounds.Top, roundSize * 2, roundSize * 2),
                    180,
                    90
                );

                
                if (IsBackgroundEnabled && _BrushResource != null) {
                    //g.FillRectangle(brush, Left, Top, Width - 1, Height - 1);
                    g.FillPath(_BrushResource, path);
                }
                if (IsForegroundEnabled && BorderWidth > 0) {
                    //g.DrawRectangle(_PenResource, Left, Top, Width - 1, Height - 1);
                    g.DrawPath(_PenResource, path);
                    //g.DrawPath(Pens.Red, path);
                }
            }
        }
    }
}
