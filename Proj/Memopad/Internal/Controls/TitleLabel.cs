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
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace Mkamo.Memopad.Internal.Controls {
    [ToolboxItem(false)]
    internal class TitleLabel: Label {
        public TitleLabel() {
            ForeColor = Color.Navy;
            BackColor = Color.FromArgb(215, 215, 235);
            AutoSize = false;
            TextAlign = ContentAlignment.MiddleLeft;
        }

        protected override void OnPaintBackground(PaintEventArgs pevent) {
            //base.OnPaintBackground(pevent);
            var g = pevent.Graphics;
            var r = new Rectangle(Left, Top, Width - 1, Height - 1);
            using (var brush = new SolidBrush(Color.FromArgb(220, 230, 240))) {
                g.FillRectangle(brush, r);
            }
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            //var g = e.Graphics;
            //using (var pen = new Pen(Color.FromArgb(160, 170, 200), 1)) {
            //    g.DrawLine(pen, new Point(0, Bottom - 1), new Point(Width - 2, Bottom - 1));
            //}

            //g.DrawLine(SystemPens.ControlDark, new Point(0, 0), new Point(Width - 2, 0));
            //g.DrawLine(SystemPens.ControlDark, new Point(0, 0), new Point(0, Bottom - 1));
            //g.DrawLine(SystemPens.ControlDark, new Point(Width - 1, 0), new Point(Width - 1, Bottom - 1));
        }
    }

}
