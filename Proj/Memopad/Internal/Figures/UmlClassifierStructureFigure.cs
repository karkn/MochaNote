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
using Mkamo.Figure.Core;
using System.Reflection;

namespace Mkamo.Memopad.Internal.Figures {
    [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = false)]
    internal class UmlClassifierStructureFigure: AbstractNode {
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
                    g.DrawLine(_PenResource, new Point(Left, Top), new Point(Right - 1, Top));
                }
                PaintText(g);
                PaintSelection(g);
                PaintStyledText(g);
            }
        }

        public override bool ContainsPoint(Point pt) {
            /// 子供だけにあたり判定，自分は透過
            if (!base.ContainsPoint(pt)) {
                return false;
            }
            foreach (var child in Children) {
                if (child.ContainsPoint(pt)) {
                    return true;
                }
            }
            return false;
        }

        public override bool IntersectsWith(Rectangle rect) {
            /// 子供だけにあたり判定，自分は透過
            if (!base.IntersectsWith(rect)) {
                return false;
            }
            foreach (var child in Children) {
                if (child.IntersectsWith(rect)) {
                    return true;
                }
            }
            return false;
        }

        //protected override Size MeasureSelf(SizeConstraint constraint) {
        //    return Size.Empty;
        //}

    }
}
