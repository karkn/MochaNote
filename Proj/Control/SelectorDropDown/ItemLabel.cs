/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Mkamo.Control.SelectorDropDown {
    internal class ItemLabel: Label {
        private bool _isMouseEnter;

        public ItemLabel() {
        }

        public bool IsMouseEnter {
            get { return _isMouseEnter; }
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            if (_isMouseEnter) {
                var g = e.Graphics;
                using (var brush = new SolidBrush(Color.FromArgb(64, SystemColors.Highlight))) {
                    g.FillRectangle(brush, new Rectangle(0, 0, Width - 1, Height - 1));
                    g.DrawRectangle(Pens.Blue, new Rectangle(0, 0, Width - 1, Height - 1));
                }
            }
        }

        protected override void OnMouseEnter(EventArgs e) {
            _isMouseEnter = true;
            base.OnMouseEnter(e);
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e) {
            _isMouseEnter = false;
            base.OnMouseLeave(e);
            Invalidate();
        }
    }
}
