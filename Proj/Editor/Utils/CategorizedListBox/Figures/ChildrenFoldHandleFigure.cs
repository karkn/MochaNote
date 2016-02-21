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

namespace Mkamo.Editor.Utils.CategorizedListBox.Figures {
    public class ChildrenFoldHandleFigure: AbstractNode {
        // ========================================
        // field
        // ========================================
        private bool _folded;

        // ========================================
        // constructor
        // ========================================
        public ChildrenFoldHandleFigure() {
            _folded = false;
        }

        // ========================================
        // property
        // ========================================
        public bool Folded {
            get { return _folded; }
            set {
                _folded = value;
                InvalidatePaint();
            }
        }

        // ========================================
        // method
        // ========================================
        protected override void PaintSelf(Graphics g) {
            using (_ResourceCache.UseResource()) {
                var brush = _BrushResource;
                var pen = _PenResource;

                if (brush != null) {
                    g.FillRectangle(brush, Left, Top, Width - 1, Height - 1);
                }
                g.DrawRectangle(pen, Left, Top, Width - 1, Height - 1);
                if (_folded) {
                    // プラスアイコン描画
                    g.DrawLine(pen, Left + 2, Top + Height / 2, Right - 3, Top + Height / 2);
                    g.DrawLine(pen, Left + Width / 2, Top + 2, Left + Width / 2, Bottom - 3);
                } else {
                    // マイナスアイコン描画
                    g.DrawLine(pen, Left + 2, Top + Height / 2, Right - 3, Top + Height / 2);
                }
                
            }
        }

    }
}
