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
using Mkamo.Editor.Requests;

namespace Mkamo.Editor.Handles {
    public class MoveEdgePointHandle: AbstractAuxiliaryHandle {
        // ========================================
        // field
        // ========================================
        private IFigure _figure;
        private Size _figureSize;

        private MoveEdgePointRequest _request;

        // ========================================
        // constructor
        // ========================================
        public MoveEdgePointHandle(EdgePointRef edgePointRef) {
            var fig = new SimpleRect();
            _figure = fig;
            _figureSize = new Size(8, 8);

            _request = new MoveEdgePointRequest();
            _request.EdgePointRef = edgePointRef;
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

        // ------------------------------
        // protected
        // ------------------------------
        protected MoveEdgePointRequest _MoveEdgePointRequest {
            get { return _request; }
        }

        // ========================================
        // method
        // ========================================
        public override void Relocate(IFigure hostFigure) {
            _figure.Size = _figureSize;
            _figure.Location = new Point(
                _request.EdgePointRef.EdgePoint.X - _figureSize.Width / 2,
                _request.EdgePointRef.EdgePoint.Y - _figureSize.Height / 2
            );
        }

        // === AbstractHandle ==========
        protected override void OnFigureDragStart(MouseEventArgs e) {
            base.OnFigureDragStart(e);
            _request.Location = e.Location;
            Host.ShowFeedback(_request);
        }

        protected override void OnFigureDragMove(MouseEventArgs e) {
            base.OnFigureDragMove(e);
            _request.Location = e.Location;
            Host.ShowFeedback(_request);
        }

        protected override void OnFigureDragFinish(MouseEventArgs e) {
            base.OnFigureDragFinish(e);
            _request.Location = e.Location;
            Host.HideFeedback(_request);
            Host.PerformRequest(_request);
        }

        protected override void OnFigureDragCancel() {
            base.OnFigureDragCancel();
            Host.HideFeedback(_request);
        }
    }
}
