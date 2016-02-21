/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using Mkamo.Figure.Figures;
using System.Drawing;
using System.Windows.Forms;
using Mkamo.Editor.Core;
using Mkamo.Editor.Requests;
using Mkamo.Common.DataType;

namespace Mkamo.Editor.Handles {
    public class ResizeHandle: AbstractResizeHandle {
        // ========================================
        // static field
        // ========================================
        public static readonly Size FigureSizeDefault = new Size(8, 8);

        // ========================================
        // field
        // ========================================
        private IFigure _figure;
        private Size _figureSize;
        private Directions _direction;

        // ========================================
        // constructor
        // ========================================
        public ResizeHandle(Directions direction, Size figureSize) {
            _figure = new SimpleRect();
            _figureSize = FigureSizeDefault;
            _direction = direction;
        }

        public ResizeHandle(Directions direction): this(direction, FigureSizeDefault) {
        }

        // ========================================
        // property
        // ========================================
        public override IFigure Figure {
            get { return _figure; }
        }

        // === ResizeHandle ==========
        public Size FigureSize {
            get { return _figureSize; }
            set { _figureSize = value; }
        }

        public override Directions Direction {
            get { return _direction; }
        }

        // ========================================
        // method
        // ========================================
        public override void Relocate(IFigure hostFigure) {
            _figure.Size = _figureSize;

            switch (Direction) {
                case Directions.Left: {
                    _figure.Location = new Point(
                        hostFigure.Left - _figure.Width / 2,
                        hostFigure.Top + (hostFigure.Height - _figure.Height) / 2
                    );
                    break;
                }
                case Directions.Up: {
                    _figure.Location = new Point(
                        hostFigure.Left + (hostFigure.Width - _figure.Width) / 2,
                        hostFigure.Top - _figure.Height / 2
                    );
                    break;
                }
                case Directions.Right: {
                    _figure.Location = new Point(
                        hostFigure.Left + hostFigure.Width - _figure.Width / 2,
                        hostFigure.Top + (hostFigure.Height - _figure.Height) / 2
                    );
                    break;
                }
                case Directions.Down: {
                    _figure.Location = new Point(
                        hostFigure.Left + (hostFigure.Width - _figure.Width) / 2,
                        hostFigure.Top + hostFigure.Height - _figure.Height / 2
                    );
                    break;
                }
                case Directions.UpLeft: {
                    _figure.Location = new Point(
                        hostFigure.Left - _figure.Width / 2,
                        hostFigure.Top - _figure.Height / 2
                    );
                    break;
                }
                case Directions.UpRight: {
                    _figure.Location = new Point(
                        hostFigure.Left + hostFigure.Width - _figure.Width / 2,
                        hostFigure.Top - _figure.Height / 2
                    );
                    break;
                }
                case Directions.DownLeft: {
                    _figure.Location = new Point(
                        hostFigure.Left - _figure.Width / 2,
                        hostFigure.Top + hostFigure.Height - _figure.Height / 2
                    );
                    break;
                }
                case Directions.DownRight: {
                    _figure.Location = new Point(
                        hostFigure.Left + hostFigure.Width - _figure.Width / 2,
                        hostFigure.Top + hostFigure.Height - _figure.Height / 2
                    );
                    break;
                }
            }
        }

    }
}
