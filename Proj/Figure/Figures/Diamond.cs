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
    public class Diamond: AbstractPathBoundingNode {
        // ========================================
        // constructor
        // ========================================
        public Diamond() {
            
        }

        // ========================================
        // method
        // ========================================
        // === AbstractPathBoundingFigure ==========
        protected override GraphicsPathDescription CreatePath(Rectangle bounds) {
            var ret = new GraphicsPathDescription();

            var center = RectUtil.GetCenter(bounds);

            var top = new Point(center.X, bounds.Top);
            var left = new Point(bounds.Left, center.Y);
            var bottom = new Point(center.X, bounds.Bottom);
            var right = new Point(bounds.Right, center.Y);

            var pts = new[] {
                top,
                right,
                bottom,
                left,
                top,
            };
            ret.AddPolygon(pts);

            return ret;
        }
    }
}
