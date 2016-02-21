/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Figure.Core;
using System.Drawing;
using Mkamo.Common.DataType;
using Mkamo.Common.Command;
using Mkamo.Common.Forms.Drawing;
using Mkamo.Common.Core;

namespace Mkamo.Editor.Commands {
    public class ChangeBoundsCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private IEditor _target;
        private Size _moveDelta;
        private Size _sizeDelta;
        private Directions _resizeDirection;
        private IEnumerable<IEditor> _movingEditors;

        private IEnumerable<IFigure> _movingFigures;
        private Rectangle _oldBounds;
        private IEnumerable<Point> _oldEdgePoints;

        // ========================================
        // constructor
        // ========================================
        public ChangeBoundsCommand(
            IEditor target,
            Size moveDelta,
            Size sizeDelta,
            Directions resizeDirection,
            IEnumerable<IEditor> movingEditors
        ) {
            _target = target;
            _moveDelta = moveDelta;
            _sizeDelta = sizeDelta;
            _resizeDirection = resizeDirection;
            _movingEditors = movingEditors;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null; }
        }

        public override bool CanUndo {
            get { return true; }
        }

        // === ChangeBoundsCommand ==========
        public IEditor Target {
            get { return _target; }
        }

        public Size MoveDelta {
            get { return _moveDelta; }
        }

        public Size SizeDelta {
            get { return _sizeDelta; }
        }

        public Directions ResizeDirection {
            get { return _resizeDirection; }
        }

        public Rectangle OldBounds {
            get { return _oldBounds; }
        }
        
        // ========================================
        // method
        // ========================================
        public override void Execute() {
            _movingFigures = _movingEditors == null? null: _movingEditors.Select(editor => editor.Figure);

            if (_moveDelta != Size.Empty) {
                _target.Figure.Move(_moveDelta, _movingFigures);

                var edge = _target.Figure as IEdge;
                if (edge != null && edge.EdgeBehaviorOptions.RouteOnMoved) {
                    _oldEdgePoints = edge.EdgePoints.ToArray();
                    edge.Route();
                }
            }

            if (_sizeDelta != Size.Empty) {
                _oldBounds = _target.Figure.Bounds;

                var left = _oldBounds.Left;
                var top = _oldBounds.Top;
                var width = _oldBounds.Width;
                var height = _oldBounds.Height;

                if (EnumUtil.HasAllFlags((int) _resizeDirection, (int) Directions.Left)) {
                    var node = _target.Figure as INode;
                    var expectedSize = node.MeasureAutoSize(
                        new Size(width - _sizeDelta.Width, height - _sizeDelta.Height)
                    );
                    width = expectedSize.Width;
                    left -= width - _oldBounds.Width;
                }
                if (EnumUtil.HasAllFlags((int) _resizeDirection, (int) Directions.Up)) {
                    var node = _target.Figure as INode;
                    var expectedSize = node.MeasureAutoSize(
                        new Size(width - _sizeDelta.Width, height - _sizeDelta.Height)
                    );
                    height = expectedSize.Height;
                    top -= height - _oldBounds.Height;
                }
                if (EnumUtil.HasAllFlags((int) _resizeDirection, (int) Directions.Right)) {
                    width += _sizeDelta.Width;
                }
                if (EnumUtil.HasAllFlags((int) _resizeDirection, (int) Directions.Down)) {
                    height += _sizeDelta.Height;
                }
    
                if (left != _oldBounds.Left || top != _oldBounds.Top) {
                    _target.Figure.Move(new Size(left - _oldBounds.Left, top - _oldBounds.Top), _movingFigures);
                }
                _target.Figure.Size = new Size(width, height);
            }
        }

        public override void Undo() {
            if (_sizeDelta != Size.Empty) {
                var fig = _target.Figure;
                if (_oldBounds.Location != fig.Location) {
                    fig.Move((Size) _oldBounds.Location - (Size) fig.Location, _movingFigures);
                }
                fig.Size = _oldBounds.Size;
            }

            if (_moveDelta != Size.Empty) {
                _target.Figure.Move(new Size(-_moveDelta.Width, -_moveDelta.Height), _movingFigures);

                var edge = _target.Figure as IEdge;
                if (edge != null && edge.EdgeBehaviorOptions.RouteOnMoved && _oldEdgePoints != null) {
                    edge.SetEdgePoints(_oldEdgePoints);
                }
            }
        }
    }
}
