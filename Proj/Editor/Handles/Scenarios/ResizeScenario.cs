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
using Mkamo.Editor.Core;
using System.Windows.Forms;

namespace Mkamo.Editor.Handles.Scenarios {
    public class ResizeScenario: AbstractScenario {
        // ========================================
        // field
        // ========================================
        private ChangeBoundsRequest _request;
        private Point _startPoint;
        private Directions _direction;

        // ========================================
        // constructor
        // ========================================
        public ResizeScenario(IHandle handle): base(handle) {
            _request = new ChangeBoundsRequest();
        }

        // ========================================
        // property
        // ========================================
        public Directions Direction {
            get { return _direction; }
            set { _direction = value; }
        }

        // ========================================
        // method
        // ========================================
        public override void Apply() {
            Handle.DragStart += HandleDragStart;
            Handle.DragMove += HandleDragMove;
            Handle.DragFinish += HandleDragFinish;
            Handle.DragCancel += HandleDragCancel;
        }

        public virtual void HandleDragStart(object sender, MouseEventArgs e) {
            _startPoint = e.Location;
            _request.ResizeDirection = Direction;
            _request.SizeDelta = Size.Empty;
            _request.MovingEditors = new [] { Host };
            Host.ShowFeedback(_request);
        }

        public virtual void HandleDragMove(object sender, MouseEventArgs e) {
            var delta = (Size) e.Location - (Size) _startPoint;
            _request.ResizeDirection = Direction;
            _request.SizeDelta = delta;
            Host.ShowFeedback(_request);
        }

        public virtual void HandleDragFinish(object sender, MouseEventArgs e) {
            var delta = (Size) e.Location - (Size) _startPoint;
            _request.ResizeDirection = Direction;
            _request.SizeDelta = delta;
            Host.HideFeedback(_request);
            Host.PerformRequest(_request);
        }

        public virtual void HandleDragCancel(object sender, EventArgs e) {
            Host.HideFeedback(_request);
        }
    }
}
