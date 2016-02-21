/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Mkamo.Common.Forms.Drawing;
using Mkamo.Common.Forms.Descriptions;

namespace Mkamo.Figure.Core {
    public interface IEdgeDecoration {
        // ========================================
        // event
        // ========================================
        event EventHandler<EventArgs> TargetChanged;
        event EventHandler<EventArgs> ForegroundChanged;
        event EventHandler<EventArgs> BackgroundChanged;
        event EventHandler<EventArgs> LineWidthChanged;

        // ========================================
        // property
        // ========================================
        Line Target { get; set; }
        Color Foreground { get; set; }
        Color Background { get; set; }
        int LineWidth { get; set; }

        // ========================================
        // method
        // ========================================
        void Paint(Graphics g);
        void Activate();
        void Deactivate();
    }

    [Serializable]
    public enum EdgeDecorationKind {
        None,
        Arrow,
        Triangle,
        Circle,
        Diamond,
    }
}
