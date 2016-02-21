/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using System.Drawing;

namespace Mkamo.Editor.Requests {
    public class DropTextRequest: AbstractRequest {
        // ========================================
        // field
        // ========================================
        public Rectangle Bounds;

        public DropTextRequest(Rectangle bounds) {
            Bounds = bounds;
        }

        // ========================================
        // property
        // ========================================
        public override string Id {
            get { return RequestIds.DropText; }
        }
    }
}
