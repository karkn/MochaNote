/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Handles;
using Mkamo.Figure.Core;
using Mkamo.Figure.Figures;
using System.Drawing;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Figure.Layouts;

namespace Mkamo.Editor.Handles {
    public class SelectionIndicatingHandle: AbstractAuxiliaryHandle {
        // ========================================
        // field
        // ========================================
        private IFigure _figure;

        // ========================================
        // constructor
        // ========================================
        public SelectionIndicatingHandle() {
            _figure = new MirageLayer();
            _figure.Layout = new StackLayout();

            var fig = new SimpleRect() {
                BorderWidth = 1,
                Foreground = FigureConsts.HighlightColor,
                Background = FigureConsts.HighlightBrush,
            };

            _figure.Children.Add(fig);
        }

        // ========================================
        // property
        // ========================================
        public override IFigure Figure {
            get { return _figure; }
        }

        // ========================================
        // method
        // ========================================

        public override void Relocate(IFigure hostFigure) {
            //_figure.Bounds = hostFigure.Bounds;
            var bounds = hostFigure.Bounds;
            bounds.Inflate(1, 1);
            _figure.Bounds = bounds;
        }
    }
}
