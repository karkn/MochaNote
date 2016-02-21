/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mkamo.Common.Forms.MouseOperatable.Internal;
using System.Drawing;

namespace Mkamo.Common.Forms.MouseOperatable {
    public static class MouseOperatableFactory {
        public static IDragSource CreateDragSource(DragDropEffects allowedOperations) {
            return new DragSource(allowedOperations);
        }

        //public static IDragTarget CreateDragTarget(string[] supportedFormats) {
        //    return new DragTarget(supportedFormats);
        //}

        public static IDragTarget CreateDragTarget() {
            return new DragTarget();
        }

        public static MouseOperatableEventDispatcher CreateMouseOperatableEventDispatcher(
            Func<Point, IMouseOperatable> finder, Control control
        ) {
            return new MouseOperatableEventDispatcher(finder, control);
        }
    }

}
