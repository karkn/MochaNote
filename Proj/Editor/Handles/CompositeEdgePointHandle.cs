/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using Mkamo.Editor.Core;
using Mkamo.Editor.Internal.Core;

namespace Mkamo.Editor.Handles {
    public class CompositeEdgePointHandle: CompositeHandle {
        // ========================================
        // constructor
        // ========================================
        public CompositeEdgePointHandle() {
        }

        // ========================================
        // method
        // ========================================
        public override void Relocate(IFigure hostFigure) {
            UpdateHandles(hostFigure);
            base.Install(Host);
            base.Relocate(hostFigure);
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected virtual void UpdateHandles(IFigure hostFigure) {
            var edge = hostFigure as IEdge;
            if (edge == null) {
                return;
            }

            Children.Clear();
            foreach (var pointRef in edge.EdgePointRefs) {
                Children.Add(new MoveEdgePointHandle(pointRef) { Cursor = Cursor });
            }
        }
    }
}
