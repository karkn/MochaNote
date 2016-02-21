/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Mkamo.Common.Forms.Descriptions {
    [Serializable]
    public enum BrushKind {
        Solid,
        Gradient,
    }

    public interface IBrushDescription: ICloneable {
        // ========================================
        // property
        // ========================================
        BrushKind Kind { get; }
        bool IsDark { get; }

        // ========================================
        // method
        // ========================================
        Brush CreateBrush(Rectangle bounds);
    }

}
