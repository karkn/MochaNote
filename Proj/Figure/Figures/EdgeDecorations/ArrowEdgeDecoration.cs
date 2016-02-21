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
    public class ArrowEdgeDecoration: AbstractPathEdgeDecoration {
        // ========================================
        // constructor
        // ========================================
        public ArrowEdgeDecoration() {
        }

        // ========================================
        // method
        // ========================================
        protected override bool _IsPathClosed {
            get { return false; }
        }

        protected override Mkamo.Common.Forms.Descriptions.GraphicsPathDescription CreatePath() {
            var ret = new GraphicsPathDescription();
            ret.AddLine(Point.Empty, new Point(8, 4));
            ret.AddLine(Point.Empty, new Point(8, -4));
            return ret;
        }
    }
}
