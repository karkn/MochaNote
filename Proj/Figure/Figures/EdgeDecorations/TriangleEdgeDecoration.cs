/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Forms.Descriptions;
using System.Drawing;
using Mkamo.Common.Externalize;

namespace Mkamo.Figure.Figures.EdgeDecorations {
    [Externalizable]
    public class TriangleEdgeDecoration: AbstractPathEdgeDecoration {
        // ========================================
        // constructor
        // ========================================
        public TriangleEdgeDecoration() {
        }

        // ========================================
        // method
        // ========================================
        protected override bool _IsPathClosed {
            get { return true; }
        }

        protected override GraphicsPathDescription CreatePath() {
            var ret = new GraphicsPathDescription();
            ret.AddLine(Point.Empty, new Point(8, 4));
            ret.AddLine(new Point(8, 4), new Point(8, -4));
            ret.AddLine(new Point(8, -4), Point.Empty);
            return ret;
        }
    }
}
