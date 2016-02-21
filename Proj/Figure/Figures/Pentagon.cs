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
    public class Pentagon: AbstractPathBoundingNode {
        // ========================================
        // constructor
        // ========================================
        public Pentagon() {
            
        }

        // ========================================
        // method
        // ========================================
        // === AbstractPathBoundingFigure ==========
        protected override GraphicsPathDescription CreatePath(Rectangle bounds) {
            var ret = new GraphicsPathDescription();

            var triangleLeft = Math.Max(bounds.Right - bounds.Height / 2, bounds.Left + bounds.Width / 2);

            var pt1 = bounds.Location;
            var pt2 = new Point(triangleLeft, bounds.Top);
            var pt3 = new Point(bounds.Right, bounds.Top + bounds.Height / 2);
            var pt4 = new Point(triangleLeft, bounds.Bottom);
            var pt5 = new Point(bounds.Left, bounds.Bottom);

            ret.AddLine(pt1, pt2);
            ret.AddLine(pt2, pt3);
            ret.AddLine(pt3, pt4);
            ret.AddLine(pt4, pt5);
            ret.AddLine(pt5, pt1);

            return ret;
        }
    }
}
