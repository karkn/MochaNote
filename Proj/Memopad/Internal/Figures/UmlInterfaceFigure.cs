/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using Mkamo.Figure.Figures;
using System.Drawing;
using Mkamo.Common.Externalize;
using Mkamo.Common.Forms.Drawing;
using Mkamo.Figure.Layouts;
using Mkamo.Common.DataType;
using System.Reflection;

namespace Mkamo.Memopad.Internal.Figures {
    [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = false)]
    internal class UmlInterfaceFigure: AbstractNode {
        // ========================================
        // static field
        // ========================================
        private const int DefaultHeight = 20;

        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================
        public UmlInterfaceFigure() {
            Padding = Padding.GetTopChanged(4);
            TextHorizontalAlignment = HorizontalAlignment.Center;
            TextVerticalAlignment = VerticalAlignment.Top;

            Layout = new ListLayout() {
                Padding = new Insets(
                    0,
                    DefaultHeight,
                    0,
                    0
                ),
                ItemSpace = -1,
                ExtendLast = true,
            };
        }

        // ========================================
        // method
        // ========================================
        protected override void PaintSelf(Graphics g) {
            using (_ResourceCache.UseResource()) {
                var brush = _BrushResource;
                var pen = _PenResource;

                if (IsBackgroundEnabled && brush != null) {
                    g.FillRectangle(brush, Left, Top, Width - 1, Height - 1);
                }
                if (IsForegroundEnabled && BorderWidth > 0) {
                    pen.Width = BorderWidth;
                    g.DrawRectangle(pen, Left, Top, Width - 1, Height - 1);
                }

                PaintStyledText(g);
            }
        }


        public override void WriteExternal(IMemento memento, ExternalizeContext context) {
            base.WriteExternal(memento, context);
        }

        public override void ReadExternal(IMemento memento, ExternalizeContext context) {
            base.ReadExternal(memento, context);
        }

    }
}
