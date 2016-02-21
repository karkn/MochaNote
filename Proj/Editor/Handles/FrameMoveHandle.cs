/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Figures;
using System.Windows.Forms;
using System.Drawing;
using Mkamo.Editor.Core;
using Mkamo.Figure.Core;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Editor.Handles;
using Mkamo.Editor.Internal.Figures;

namespace Mkamo.Editor.Handles {
    public class FrameMoveHandle: AbstractMoveHandle, IAuxiliaryHandle {
        // ========================================
        // field
        // ========================================
        private INode _figure;
        int _height;
        int _frameWidth;

        // ========================================
        // constructor
        // ========================================
        public FrameMoveHandle(int height, int frameWidth, Color foreground, IBrushDescription background) {
            //_figure = new SimpleRect();
            _figure = new MoveHandleFigure();
            _figure.Foreground = foreground;
            _figure.Background = background;

            _height = height;
            _frameWidth = frameWidth;

            Cursor = Cursors.SizeAll;
        }

        // ========================================
        // property
        // ========================================
        public IFigure Figure {
            get { return _Figure; }
        }

        public virtual bool HideOnFocus {
            get { return false; }
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected override IFigure _Figure {
            get { return _figure; }
        }

        // ========================================
        // method
        // ========================================
        public void Relocate(IFigure hostFigure) {
            var fig = Host.IsFocused? Host.Focus.Figure: hostFigure;
            _figure.Bounds = new Rectangle(
                fig.Left,
                fig.Top - _height - 1,
                fig.Width,
                _height + _frameWidth * 2
            );
        }
    }

}
