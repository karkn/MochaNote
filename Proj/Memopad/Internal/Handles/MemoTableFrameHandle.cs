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
using Mkamo.Memopad.Internal.Figures;
using Mkamo.Common.Core;
using System.Windows.Forms;
using Mkamo.Editor.Handles.Scenarios;
using Mkamo.Common.DataType;

namespace Mkamo.Memopad.Internal.Handles {
    internal class MemoTableFrameHandle: AbstractAuxiliaryHandle {
        // ========================================
        // field
        // ========================================
        private Lazy<MemoTableFrameHandleFigure> _figure;

        private MoveScenario _moveScenario;
        private ResizeScenario _resizeScenario;

        private bool _isInMoving;
        private bool _isInResizing;

        // ========================================
        // constructor
        // ========================================
        public MemoTableFrameHandle() {
            _figure = new Lazy<MemoTableFrameHandleFigure>(
                () => new MemoTableFrameHandleFigure()
            );

            _moveScenario = new MoveScenario(this);
            _resizeScenario = new ResizeScenario(this);

            _isInMoving = false;
            _isInResizing = false;
        }

        // ========================================
        // property
        // ========================================
        public override IFigure Figure {
            get { return _figure.Value; }
        }


        // ========================================
        // method
        // ========================================
        public override void Relocate(Mkamo.Figure.Core.IFigure hostFigure) {
            _figure.Value.InnerBounds = hostFigure.Bounds;
        }

        protected override Cursor GetMouseCursor(MouseEventArgs e) {
            if (_figure.Value.IsOnLeftTopCorner(e.Location)) {
                return Cursors.SizeNWSE;
            } else if (_figure.Value.IsOnRightTopCorner(e.Location)) {
                return Cursors.SizeNESW;
            } else if (_figure.Value.IsOnLeftBottomCorner(e.Location)) {
                return Cursors.SizeNESW;
            } else if (_figure.Value.IsOnRightBottomCorner(e.Location)) {
                return Cursors.SizeNWSE;

            } else if (_figure.Value.IsOnMoveBar(e.Location)) {
                return Cursors.SizeAll;

            } else if (_figure.Value.IsOnLeftBorder(e.Location)) {
                return Cursors.SizeWE;
            } else if (_figure.Value.IsOnRightBorder(e.Location)) {
                return Cursors.SizeWE;
            } else if (_figure.Value.IsOnTopBorder(e.Location)) {
                return Cursors.SizeNS;
            } else if (_figure.Value.IsOnBottomBorder(e.Location)) {
                return Cursors.SizeNS;
            }

            return base.GetMouseCursor(e);
        }

        protected override void OnFigureMouseDown(MouseEventArgs e) {
            _moveScenario.HandleMouseDown(this, e);
            base.OnFigureMouseDown(e);
        }

        protected override void OnFigureMouseUp(MouseEventArgs e) {
            _moveScenario.HandleMouseUp(this, e);
            base.OnFigureMouseUp(e);
        }

        protected override void OnFigureDragStart(MouseEventArgs e) {
            _isInMoving = false;
            _isInResizing = false;

            if (_figure.Value.IsOnLeftTopCorner(e.Location)) {
                _resizeScenario.Direction = Directions.UpLeft;
                _isInResizing = true;
            } else if (_figure.Value.IsOnRightTopCorner(e.Location)) {
                _resizeScenario.Direction = Directions.UpRight;
                _isInResizing = true;
            } else if (_figure.Value.IsOnLeftBottomCorner(e.Location)) {
                _resizeScenario.Direction = Directions.DownLeft;
                _isInResizing = true;
            } else if (_figure.Value.IsOnRightBottomCorner(e.Location)) {
                _resizeScenario.Direction = Directions.DownRight;
                _isInResizing = true;

            } else if (_figure.Value.IsOnMoveBar(e.Location)) {
                _isInMoving = true;

            } else if (_figure.Value.IsOnLeftBorder(e.Location)) {
                _resizeScenario.Direction = Directions.Left;
                _isInResizing = true;
            } else if (_figure.Value.IsOnRightBorder(e.Location)) {
                _resizeScenario.Direction = Directions.Right;
                _isInResizing = true;
            } else if (_figure.Value.IsOnTopBorder(e.Location)) {
                _resizeScenario.Direction = Directions.Up;
                _isInResizing = true;
            } else if (_figure.Value.IsOnBottomBorder(e.Location)) {
                _resizeScenario.Direction = Directions.Down;
                _isInResizing = true;
            }

            if (_isInMoving) {
                _moveScenario.HandleDragStart(this, e);
            } else if (_isInResizing) {
                _resizeScenario.HandleDragStart(this, e);
            }

            base.OnFigureDragStart(e);
        }

        protected override void OnFigureDragMove(MouseEventArgs e) {
            if (_isInMoving) {
                _moveScenario.HandleDragMove(this, e);
            } else if (_isInResizing) {
                _resizeScenario.HandleDragMove(this, e);
            }

            base.OnFigureDragMove(e);
        }

        protected override void OnFigureDragFinish(MouseEventArgs e) {
            if (_isInMoving) {
                _moveScenario.HandleDragFinish(this, e);
            } else if (_isInResizing) {
                _resizeScenario.HandleDragFinish(this, e);
            }

            _isInMoving = false;
            _isInResizing = false;

            base.OnFigureDragFinish(e);
        }

        protected override void OnFigureDragCancel() {
            if (_isInMoving) {
                _moveScenario.HandleDragCancel(this, EventArgs.Empty);
            } else if (_isInResizing) {
                _resizeScenario.HandleDragCancel(this, EventArgs.Empty);
            }

            _isInMoving = false;
            _isInResizing = false;

            base.OnFigureDragCancel();
        }
    }
}
