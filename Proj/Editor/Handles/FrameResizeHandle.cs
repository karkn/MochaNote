/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Mkamo.Common.DataType;
using Mkamo.Common.Forms.Descriptions;
using System.Windows.Forms;
using Mkamo.Figure.Core;
using Mkamo.Editor.Internal.Figures;
using Mkamo.Editor.Handles;

namespace Mkamo.Editor.Handles {
    public class FrameResizeHandle: AbstractResizeHandle {
        // ========================================
        // field
        // ========================================
        private FrameResizeFigure _figure;
        private int _frameWidth;
        private Size _cornerSize;
        private Directions _direction;

        // ========================================
        // constructor
        // ========================================
        public FrameResizeHandle(int hitMargin, int frameWidth, Size cornerSize, Color color) {
            _frameWidth = frameWidth;
            _cornerSize = cornerSize;

            _figure = new FrameResizeFigure(hitMargin, cornerSize);
            _figure.BorderWidth = frameWidth;
            _figure.Foreground = color;
            _figure.IsBackgroundEnabled = false;

            Cursor = Cursors.Cross;
        }

        // ========================================
        // property
        // ========================================
        public override IFigure Figure {
            get { return _figure; }
        }

        public override Directions Direction {
            get { return _direction; }
        }

        // ========================================
        // method
        // ========================================
        public override void Relocate(IFigure hostFigure) {
            var rect = Host.IsFocused? Host.Focus.Figure.Bounds: hostFigure.Bounds;
            _figure.Bounds = new Rectangle(
                rect.Left,
                //rect.Top - _frameWidth - _cornerSize.Height,
                rect.Top,
                rect.Width,
                //rect.Height + _frameWidth + _cornerSize.Height
                rect.Height
            );
        }

        protected override Cursor GetMouseCursor(MouseEventArgs e) {
            if (_figure.ContainsPoint(e.Location)) {
                var direction = _figure.GetDirection(e.Location);
                switch (direction) {
                    case Directions.Left: {
                        return Cursors.SizeWE;
                    }
                    case Directions.Right: {
                        return Cursors.SizeWE;
                    }
                    case Directions.Up: {
                        return Cursors.SizeNS;
                    }
                    case Directions.Down: {
                        return Cursors.SizeNS;
                    }
                    case Directions.UpLeft: {
                        return Cursors.SizeNWSE;
                    }
                    case Directions.UpRight: {
                        return Cursors.SizeNESW;
                    }
                    case Directions.DownLeft: {
                        return Cursors.SizeNESW;
                    }
                    case Directions.DownRight: {
                        return Cursors.SizeNWSE;
                    }
                }
            }

            return null;
        }

        protected override void OnFigureDragStart(MouseEventArgs e) {
            _direction = _figure.GetDirection(e.Location);
            base.OnFigureDragStart(e);
        }
    }
}
