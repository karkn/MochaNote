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
using Mkamo.Editor.Requests;
using Mkamo.Common.Forms.Drawing;
using System.Windows.Forms;
using Mkamo.Figure.Figures;
using Mkamo.Common.Forms.Descriptions;

namespace Mkamo.Editor.Handles {
    public class MoveOrthogonalEdgePointHandle: AbstractAuxiliaryHandle {
        // ========================================
        // field
        // ========================================
        private IFigure _figure;
        private Size _figureSize;

        private MoveEdgePointRequest _prevPointRequest;
        private MoveEdgePointRequest _nextPointRequest;

        // ========================================
        // constructor
        // ========================================
        public MoveOrthogonalEdgePointHandle(EdgePointRef prevEdgePointRef) {
            var fig = new SimpleRect();
            fig.Foreground = Color.Gray;
            fig.Background = new SolidBrushDescription(Color.Yellow);
            _figure = fig;
            _figureSize = new Size(8, 8);

            _prevPointRequest = new MoveEdgePointRequest();
            _prevPointRequest.AdjustGrid = false;
            _prevPointRequest.DontFlatten = true;
            _prevPointRequest.EdgePointRef = prevEdgePointRef;

            _nextPointRequest = new MoveEdgePointRequest();
            _nextPointRequest.AdjustGrid = false;
            _nextPointRequest.DontFlatten = true;
            _nextPointRequest.EdgePointRef = prevEdgePointRef.Next;
        }

        // ========================================
        // property
        // ========================================
        public override IFigure Figure {
            get { return _figure; }
        }

        public Size FigureSize {
            get { return _figureSize; }
            set { _figureSize = value; }
        }

        private Point _PrevPoint {
            get { return _prevPointRequest.EdgePointRef.EdgePoint; }
        }

        private Point _NextPoint {
            get { return _nextPointRequest.EdgePointRef.EdgePoint; }
        }


        private EdgePointRef _PrevPointRef {
            get { return _prevPointRequest.EdgePointRef; }
        }

        private EdgePointRef _NextPointRef {
            get { return _nextPointRequest.EdgePointRef; }
        }


        // ========================================
        // method
        // ========================================
        public override void Relocate(IFigure hostFigure) {
            _figure.Size = _figureSize;
            var middle = PointUtil.MiddlePoint(
                _prevPointRequest.EdgePointRef.EdgePoint,
                _nextPointRequest.EdgePointRef.EdgePoint
            );
            _figure.Location = new Point(
                middle.X - _figureSize.Width / 2,
                middle.Y - _figureSize.Height / 2
            );
        }

        // === AbstractHandle ==========
        protected override void OnFigureDragStart(MouseEventArgs e) {
            base.OnFigureDragStart(e);

            _prevPointRequest.Location = GetPrevMovedLoc(e.Location);
            _nextPointRequest.Location = GetNextMovedLoc(e.Location);

            Host.ShowFeedback(_prevPointRequest);
            Host.ShowFeedback(_nextPointRequest);
        }

        protected override void OnFigureDragMove(MouseEventArgs e) {
            base.OnFigureDragMove(e);

            _prevPointRequest.Location = GetPrevMovedLoc(e.Location);
            _nextPointRequest.Location = GetNextMovedLoc(e.Location);

            Host.ShowFeedback(_prevPointRequest);
            Host.ShowFeedback(_nextPointRequest);
        }

        protected override void OnFigureDragFinish(MouseEventArgs e) {
            base.OnFigureDragFinish(e);

            _prevPointRequest.Location = GetPrevMovedLoc(e.Location);
            _nextPointRequest.Location = GetNextMovedLoc(e.Location);

            Host.HideFeedback(_prevPointRequest);
            Host.HideFeedback(_nextPointRequest);

            using (Host.Site.CommandExecutor.BeginChain()) {
                Host.PerformRequest(_prevPointRequest);
                Host.PerformRequest(_nextPointRequest);
            }
        }

        protected override void OnFigureDragCancel() {
            base.OnFigureDragCancel();

            Host.HideFeedback(_prevPointRequest);
            Host.HideFeedback(_nextPointRequest);
        }

        // ------------------------------
        // private
        // ------------------------------
        private bool IsHorizontalLine() {
            var prev = _PrevPoint;;
            var next = _NextPoint;

            var dx = Math.Abs(prev.X - next.X);
            var dy = Math.Abs(prev.Y - next.Y);

            if (dx == dy) {
                if (_PrevPointRef.HasPrevEdgePointReference) {
                    prev = _PrevPointRef.Prev.EdgePoint;
                }
                if (_NextPointRef.HasNextEdgePointReference) {
                    next = _NextPointRef.Next.EdgePoint;
                }
                dx = Math.Abs(prev.X - next.X);
                dy = Math.Abs(prev.Y - next.Y);

                return dx <= dy;
            } else {
                return dx >= dy;
            }
        }

        private Point GetPrevMovedLoc(Point loc) {
            if (IsHorizontalLine()) {
                return new Point(_PrevPoint.X, loc.Y);
            } else {
                return new Point(loc.X, _PrevPoint.Y);
            }
        }

        private Point GetNextMovedLoc(Point loc) {
            if (IsHorizontalLine()) {
                return new Point(_NextPoint.X, loc.Y);
            } else {
                return new Point(loc.X, _NextPoint.Y);
            }
        }
    }
}
