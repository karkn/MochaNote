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

namespace Mkamo.Figure.Figures {
    public class MirageFigureGroup: FigureGroup {
        // ========================================
        // method
        // ========================================
        // === IFigure ==========
        public override bool ContainsPoint(Point pt) {
            return false;
        }

        public override bool IntersectsWith(Rectangle rect) {
            return false;
        }
    }
}
