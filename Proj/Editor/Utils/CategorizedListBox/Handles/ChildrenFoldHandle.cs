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
using Mkamo.Editor.Utils.CategorizedListBox.Figures;
using System.Drawing;
using Mkamo.Figure.Figures;
using System.Windows.Forms;
using Mkamo.Editor.Requests;
using Mkamo.Editor.Core;

namespace Mkamo.Editor.Utils.CategorizedListBox.Handles {
    public class ChildrenFoldHandle: AbstractAuxiliaryHandle {
        // ========================================
        // field
        // ========================================
        private bool _folded;

        private ChildrenFoldHandleFigure _figure;
        private Size _figureSize;
        private int _leftMargin = 4;

        // ========================================
        // constructor
        // ========================================
        public ChildrenFoldHandle() {
            _figure = new ChildrenFoldHandleFigure();
            _figureSize = new Size(9, 9);
            _folded = false;
        }

        // ========================================
        // property
        // ========================================
        public Size FigureSize {
            get { return _figureSize; }
            set { _figureSize = value; }
        }

        public int LeftMargin {
            get { return _leftMargin; }
            set { _leftMargin = value; }
        }

        // ========================================
        // method
        // ========================================
        public override IFigure Figure {
            get { return _figure; }
        }

        public override void Relocate(IFigure hostFigure) {
            var cateLabel = hostFigure.Children[0];
            var cateLabelSize = cateLabel.PreferredSize;
            _figure.Location = new Point(
                hostFigure.Left + _leftMargin,
                hostFigure.Top + (cateLabelSize.Height - _figure.Height) / 2
            );
            _figure.Size = _figureSize;
        }

        protected override void OnFigureMouseDown(MouseEventArgs e) {
            using (Figure.DirtManager.BeginDirty()) {
                _folded = !_folded;
                foreach (var child in Host.Children) {
                    child.IsEnabled = !_folded;
                }
                _figure.Folded = _folded;
            }
        }
    }
}
