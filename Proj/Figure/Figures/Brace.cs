/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Externalize;
using System.Drawing;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Common.Forms.Drawing;

namespace Mkamo.Figure.Figures {
    public class Brace: AbstractPathBoundingNode {
        // ========================================
        // constructor
        // ========================================
        public Brace() {
        }

        // ========================================
        // method
        // ========================================
        // === AbstractPathBoundingFigure ==========
        protected override GraphicsPathDescription CreatePath(Rectangle bounds) {
            // todo: widthが大きくなると図形が崩れる，背景を塗りつぶさない，線だけでマウスクリックを拾う，edgeとの接続方法など未解決

            var ret = new GraphicsPathDescription();

            var triangleLeft = Math.Max(bounds.Right - bounds.Height / 2, bounds.Left + bounds.Width / 2);

            var center = RectUtil.GetCenter(bounds);
            var half = new Size(bounds.Width / 2, bounds.Height / 2);

            var pt1 = new Point(center.X, bounds.Top + half.Width);
            var pt2 = new Point(center.X, center.Y - half.Width);
            var pt3 = new Point(center.X, center.Y + half.Width);
            var pt4 = new Point(center.X, bounds.Bottom - half.Width);

            var r1 = new Rectangle(center.X, bounds.Top, bounds.Width - 1, bounds.Width - 1);
            var r2 = new Rectangle(bounds.Left - half.Width, center.Y - bounds.Width, bounds.Width - 1, bounds.Width - 1);
            var r3 = new Rectangle(bounds.Left - half.Width, center.Y, bounds.Width - 1, bounds.Width - 1);
            var r4 = new Rectangle(center.X, bounds.Bottom - bounds.Width, bounds.Width - 1, bounds.Width - 1);

            ret.AddArc(r1, 270, -90);
            ret.AddLine(pt1, pt2);
            ret.AddArc(r2, 0, 90);
            ret.AddArc(r3, 270, 90);
            ret.AddLine(pt3, pt4);
            ret.AddArc(r4, 180, -90);

            return ret;
        }
    }
}
