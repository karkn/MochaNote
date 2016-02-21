/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using System.Drawing;
using Mkamo.Common.Externalize;

namespace Mkamo.Figure.Figures {
    /// <summary>
    /// 配下のFigureがすべての検索対象にならない，
    /// 表示だけのためだけのFigureを含むLayer
    /// </summary>
    public class MirageLayer: Layer {
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
