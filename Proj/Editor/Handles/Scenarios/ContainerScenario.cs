/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Editor.Requests;
using System.Drawing;
using Mkamo.Common.Forms.Drawing;
using Mkamo.Common.Forms.Input;
using System.Windows.Forms;

namespace Mkamo.Editor.Handles.Scenarios {
    public class ContainerScenario: AbstractScenario {
        // ========================================
        // field
        // ========================================
        private SelectRequest _selectRequest;
        private RubberbandRequest _rubberbandRequest;

        private Point _startPoint;
        private bool _toggle;

        private Cursor _oldCursor;

        // ========================================
        // constructor
        // ========================================
        public ContainerScenario(IHandle handle): base(handle) {
            _selectRequest = new SelectRequest();
            _rubberbandRequest = new RubberbandRequest();
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public override void Apply() {
            Handle.MouseDown += HandleMouseDown;
            Handle.MouseDoubleClick += HandleMouseDoubleClick;
            Handle.DragStart += HandleDragStart;
            Handle.DragMove += HandleDragMove;
            Handle.DragFinish += HandleDragFinish;
            Handle.DragCancel += HandleDragCancel;
        }

        protected virtual void HandleMouseDown(object sender, MouseEventArgs e) {
            /// 他のEditorがFocusされていたらCommitしておく
            var prevFocused = Handle.Host.Site.FocusManager.FocusedEditor;
            if (prevFocused != null && prevFocused != Handle.Host) {
                prevFocused.RequestFocusCommit(true);
            }

            _toggle = KeyUtil.IsControlPressed();
            if (!_toggle) {
                Handle.Host.RequestSelect(SelectKind.True, true);
            }
        }

        protected virtual void HandleMouseDoubleClick(object sender, MouseEventArgs e) {
            /// 他のEditorがFocusされていたらCommitしておく
            var prevFocused = Handle.Host.Site.FocusManager.FocusedEditor;
            if (prevFocused != null && prevFocused != Handle.Host) {
                prevFocused.RequestFocusCommit(true);
            }

            Handle.Host.RequestFocus(FocusKind.Begin, e.Location);
        }

        protected virtual void HandleDragStart(object sender, MouseEventArgs e) {
            _startPoint = e.Location;
            _rubberbandRequest.Toggle = _toggle;
            _rubberbandRequest.Bounds = new Rectangle(_startPoint, Size.Empty);
            Handle.Host.ShowFeedback(_rubberbandRequest);

            _oldCursor = Host.Site.EditorCanvas.Cursor;
            Host.Site.EditorCanvas.Cursor = Cursors.Default;
        }

        protected virtual void HandleDragMove(object sender, MouseEventArgs e) {
            _rubberbandRequest.Toggle = _toggle;
            _rubberbandRequest.Bounds = RectUtil.GetRectangleFromDiagonalPoints(_startPoint, e.Location);
            Handle.Host.ShowFeedback(_rubberbandRequest);
        }

        protected virtual void HandleDragFinish(object sender, MouseEventArgs e) {
            _rubberbandRequest.Toggle = _toggle;
            _rubberbandRequest.Bounds = RectUtil.GetRectangleFromDiagonalPoints(_startPoint, e.Location);
            Handle.Host.HideFeedback(_rubberbandRequest);
            Handle.Host.PerformRequest(_rubberbandRequest);
            Host.Site.EditorCanvas.Cursor = _oldCursor;
        }

        protected virtual void HandleDragCancel(object sender, EventArgs e) {
            Handle.Host.HideFeedback(_rubberbandRequest);
            Host.Site.EditorCanvas.Cursor = _oldCursor;
        }



    }
}
