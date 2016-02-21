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
    public class Triangle: AbstractPathBoundingNode {
        // ========================================
        // constructor
        // ========================================
        public Triangle() {
            
        }

        // ========================================
        // method
        // ========================================
        // === AbstractPathBoundingFigure ==========
        protected override GraphicsPathDescription CreatePath(Rectangle bounds) {
            var ret = new GraphicsPathDescription();

            var center = RectUtil.GetCenter(bounds);

            var top = new Point(center.X, bounds.Top);
            var left = new Point(bounds.Left, bounds.Bottom);
            var right = new Point(bounds.Right, bounds.Bottom);

            var pts = new[] {
                top,
                right,
                left,
                top,
            };
            ret.AddPolygon(pts);

            return ret;
        }
    }
}
