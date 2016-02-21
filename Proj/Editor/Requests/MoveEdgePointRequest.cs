/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Figure.Core;
using System.Drawing;

namespace Mkamo.Editor.Requests {
    public class MoveEdgePointRequest: AbstractRequest {
        // ========================================
        // field
        // ========================================
        public EdgePointRef EdgePointRef;
        public Point Location;
        public bool AdjustGrid = true;
        public bool DontFlatten = false;

        // ========================================
        // property
        // ========================================
        public override string Id {
            get { return RequestIds.MoveEdgePoint; }
        }

    }
}
