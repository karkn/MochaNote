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
using Mkamo.Common.Visitor;

namespace Mkamo.Figure.Internal.Core {
    internal class NullDirtManager: IDirtManager {
        // ========================================
        // field
        // ========================================
        private DirtyingContext _dirtyingContext;

        // ========================================
        // constructor
        // ========================================
        public NullDirtManager() {
            _dirtyingContext = new DirtyingContext(this);
        }

        public bool IsEnabled {
            get { return false; }
            set { }
        }

        // ========================================
        // property
        // ========================================
        public bool IsInDirtying {
            get { return false; }
        }

        public DirtyingContext BeginDirty() {
            return _dirtyingContext;
        }

        public void EndDirty() {
        }

        public void DirtyPaint(Rectangle rect) {
        }

        public void DirtyLayout(IFigure figure) {
            //var constraint = new SizeConstraint();
            //figure.Accept(
            //    fig => fig.Layout == null,
            //    fig => {
            //        if (fig.Layout != null) {
            //            foreach (var child in fig.Children) {
            //                child.Measure(constraint);
            //            }
            //            var node = fig as INode;
            //            if (node != null) {
            //                node.AdjustSize();
            //            }
            //            fig.Arrange();
            //        }
            //    },
            //    NextVisitOrder.PositiveOrder
            //);
        }


    }
}
