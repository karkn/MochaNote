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
using Control = System.Windows.Forms.Control;

namespace Mkamo.Common.Forms.Utils {

    public static class ControlPaintUtil {
        // ========================================
        // method
        // ========================================
        /// <summary>
        /// controlの周りに枠を描く．
        /// </summary>
        public static void DrawBorder(Graphics g, Control control, Color borderColor, Color backColor, int padding) {
            var r = control.Bounds;
            r.Inflate(padding, padding);
            g.FillRectangle(new SolidBrush(backColor), r);
            ControlPaint.DrawBorder(g, r, borderColor, ButtonBorderStyle.Solid);
        }

        /// <summary>
        /// rの周りに枠を描く．
        /// </summary>
        public static void DrawBorder(Graphics g, Rectangle r, Color borderColor, Color backColor, int padding) {
            r.Inflate(padding, padding);
            g.FillRectangle(new SolidBrush(backColor), r);
            ControlPaint.DrawBorder(g, r, borderColor, ButtonBorderStyle.Solid);
        }
    }
}
