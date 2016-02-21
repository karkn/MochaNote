/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.DataType;
using Mkamo.Editor.Requests;
using System.Drawing;
using System.Windows.Forms;

namespace Mkamo.Editor.Handles {
    public abstract class AbstractResizeHandle: AbstractAuxiliaryHandle {
        // ========================================
        // field
        // ========================================
        private ChangeBoundsRequest _request;
        private Point _startPoint;

        // ========================================
        // constructor
        // ========================================
        public AbstractResizeHandle() {
            _request = new ChangeBoundsRequest();
        }

        // ========================================
        // property
        // ========================================
        public abstract Directions Direction { get; }

        // ========================================
        // method
        // ========================================
        // ------------------------------
        // protected
        // ------------------------------
        protected override void OnFigureDragStart(MouseEventArgs e) {
            base.OnFigureDragStart(e);

            _startPoint = e.Location;
            _request.ResizeDirection = Direction;
            _request.SizeDelta = Size.Empty;
            _request.MovingEditors = new [] { Host };
            Host.ShowFeedback(_request);
        }

        protected override void OnFigureDragMove(MouseEventArgs e) {
            base.OnFigureDragMove(e);

            var delta = (Size) e.Location - (Size) _startPoint;
            _request.ResizeDirection = Direction;
            _request.SizeDelta = delta;
            Host.ShowFeedback(_request);
        }

        protected override void OnFigureDragFinish(MouseEventArgs e) {
            base.OnFigureDragFinish(e);

            var delta = (Size) e.Location - (Size) _startPoint;
            _request.ResizeDirection = Direction;
            _request.SizeDelta = delta;
            Host.HideFeedback(_request);
            Host.PerformRequest(_request);
        }

        protected override void OnFigureDragCancel() {
            base.OnFigureDragCancel();

            Host.HideFeedback(_request);
        }

    }
}
