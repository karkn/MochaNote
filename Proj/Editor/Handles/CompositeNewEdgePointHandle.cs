/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using System.Windows.Forms;
using Mkamo.Editor.Core;

namespace Mkamo.Editor.Handles {
    public class CompositeNewEdgePointHandle: CompositeHandle {
        // ========================================
        // field
        // ========================================
        private Func<bool> _isOrthogonalProvider;

        // ========================================
        // constructor
        // ========================================
        public CompositeNewEdgePointHandle(Func<bool> isOrthogonalProvider) {
            _isOrthogonalProvider = isOrthogonalProvider;
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
        protected void UpdateHandles(IFigure hostFigure) {
            var edge = hostFigure as IEdge;
            if (edge == null) {
                return;
            }

            Children.Clear();
            var lastIndex = edge.EdgePointRefs.Last().Index;
            var i = 0;
            foreach (var pointRef in edge.EdgePointRefs) {
                if (_isOrthogonalProvider != null && _isOrthogonalProvider()) {
                    if (i != 0 && i != lastIndex && i != lastIndex - 1) {
                        var handle = new MoveOrthogonalEdgePointHandle(pointRef);
                        handle.Cursor = Cursor;
                        Children.Add(handle);
                    }
                } else {
                    if (i != lastIndex) {
                        var handle = new NewEdgePointHandle(pointRef);
                        handle.Cursor = Cursor;
                        Children.Add(handle);
                    }
                }
                ++i;
            }
        }
    }
}
