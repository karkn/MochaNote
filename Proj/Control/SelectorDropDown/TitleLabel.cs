/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Mkamo.Control.SelectorDropDown {
    internal class TitleLabel: Label {
        public TitleLabel() {
            BackColor = Color.FromArgb(215, 215, 215);
            AutoSize = false;
            TextAlign = ContentAlignment.MiddleLeft;
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            e.Graphics.DrawLine(new Pen(Color.Gray, 2), new Point(0, Bottom), new Point(Width, Bottom));
        }
    }
}
