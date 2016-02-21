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

namespace Mkamo.Editor.Internal.Figures {
    [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = false)]
    internal class AdjustSpaceFeedbackFigure: AbstractNode {
        private bool _horizontal;

        public bool Horizontal {
            get { return _horizontal; }
            set { _horizontal = value; }
        }

        protected override void PaintSelf(System.Drawing.Graphics g) {
            using (_ResourceCache.UseResource())
            using (var pen = new Pen(_PenResource.Color)) {
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                pen.StartCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                pen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;

                if (_horizontal) {
                    g.DrawLine(pen, new Point(Left, Top + 8), new Point(Right - 1, Top + 8));
                    g.DrawLine(_PenResource, new Point(Left, Top), new Point(Left, Bottom - 1));
                    g.DrawLine(_PenResource, new Point(Right - 1, Top), new Point(Right - 1, Bottom - 1));
                } else {
                    g.DrawLine(pen, new Point(Left + 8, Top), new Point(Left + 8, Bottom - 1));
                    g.DrawLine(_PenResource, new Point(Left, Top), new Point(Right - 1, Top));
                    g.DrawLine(_PenResource, new Point(Left, Bottom - 1), new Point(Right - 1, Bottom - 1));
                }
            }
        }
    }
}
