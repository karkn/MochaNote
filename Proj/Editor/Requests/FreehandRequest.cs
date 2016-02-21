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
    public class FreehandRequest: AbstractRequest {
        // ========================================
        // field
        // ========================================
        public IModelFactory ModelFactory;
        public List<Point> Points = new List<Point>();
        public int Width;
        public Color Color;

        // ========================================
        // constructor
        // ========================================
        public FreehandRequest() {
        }

        // ========================================
        // property
        // ========================================
        public override string Id {
            get { return RequestIds.Freehand; }
        }

        // ========================================
        // method
        // ========================================

    }
}
