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

namespace Mkamo.Figure.Figures {
    public class Ellipse: AbstractPathBoundingNode {
        // ========================================
        // constructor
        // ========================================
        public Ellipse() {
            
        }

        // ========================================
        // method
        // ========================================
        // === AbstractPathBoundingFigure ==========
        protected override GraphicsPathDescription CreatePath(Rectangle bounds) {
            var ret = new GraphicsPathDescription();
            ret.AddEllipse(bounds);
            return ret;
        }
    }
}
