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
using System.Diagnostics;

namespace Mkamo.Figure.Layouts {
    [Externalizable]
    public class StackLayout: AbstractLayout {
        // ========================================
        // method
        // ========================================
        public override void Arrange(IFigure parent) {
            foreach (var child in parent.Children) {
                child.Bounds = parent.Bounds;
            }
        }

    }
}
