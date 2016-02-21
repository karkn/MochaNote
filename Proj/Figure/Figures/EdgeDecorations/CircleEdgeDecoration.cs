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
    public class CircleEdgeDecoration: AbstractPathEdgeDecoration {
        // ========================================
        // constructor
        // ========================================
        public CircleEdgeDecoration() {
        }

        // ========================================
        // method
        // ========================================
        protected override bool _IsPathClosed {
            get { return true; }
        }

        protected override GraphicsPathDescription CreatePath() {
            var ret = new GraphicsPathDescription();
            ret.AddEllipse(new Rectangle(new Point(0, -2), new Size(4, 4)));
            return ret;
        }
    }
}
