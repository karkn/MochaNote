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
    public class NewEdgePointHandle: AbstractAuxiliaryHandle {
        // ========================================
        // field
        // ========================================
        private IFigure _figure;
        private Size _figureSize;

        private NewEdgePointRequest _request;

        // ========================================
        // constructor
        // ========================================
        public NewEdgePointHandle(EdgePointRef prevEdgePointRef) {
            var fig = new SimpleRect();
            fig.Foreground = Color.Gray;
            _figure = fig;
            _figureSize = new Size(8, 8);

            _request = new NewEdgePointRequest();
            _request.PrevEdgePointRef = prevEdgePointRef;
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

        // ========================================
        // method
        // ========================================
        public override void Relocate(IFigure hostFigure) {
            _figure.Size = _figureSize;
            var middle = PointUtil.MiddlePoint(
                _request.PrevEdgePointRef.EdgePoint,
                _request.PrevEdgePointRef.Next.EdgePoint
            );
            _figure.Location = new Point(
                middle.X - _figureSize.Width / 2,
                middle.Y - _figureSize.Height / 2
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
