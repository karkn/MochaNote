/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Mkamo.Figure.Core {
    public interface ILayout: ICloneable {
        // ========================================
        // property
        // ========================================
        IFigure Owner { get; set; }

        // ========================================
        // method
        // ========================================
        void Arrange(IFigure parent);
        Size Measure(IFigure parent, SizeConstraint constraint);
    }
}
