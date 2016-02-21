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
using Mkamo.Editor.Core;
using Mkamo.Editor.Requests;
using System.Drawing;
using Mkamo.Common.Forms.Drawing;
using System.Windows.Forms;
using Mkamo.Common.Forms.Input;

namespace Mkamo.Editor.Handles {
    /// <summary>
    /// 子Editorを持つEditorのHandle．
    /// 子Editorをを範囲選択する．
    /// </summary>
    public abstract class AbstractContainerHandle: AbstractAuxiliaryHandle {
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
        public AbstractContainerHandle() {
            _selectRequest = new SelectRequest();
            _rubberbandRequest = new RubberbandRequest();
        }

        // ========================================
        // method
        // ========================================
        // ------------------------------
        // protected
        // ------------------------------
        protected override void OnFigureMouseDown(MouseEventArgs e) {
            base.OnFigureMouseDoubleClick(e);

            /// 他のEditorがFocusされていたらCommitしておく
            var prevFocused = Host.Site.FocusManager.FocusedEditor;
            if (prevFocused != null && prevFocused != Host) {
                prevFocused.RequestFocusCommit(true);
            }

            _toggle = KeyUtil.IsControlPressed();
            if (!_toggle) {
                Host.RequestSelect(SelectKind.True, true);
            }
        }

        protected override void OnFigureMouseDoubleClick(MouseEventArgs e) {
            base.OnFigureMouseDoubleClick(e);

            /// 他のEditorがFocusされていたらCommitしておく
            var prevFocused = Host.Site.FocusManager.FocusedEditor;
            if (prevFocused != null && prevFocused != Host) {
                prevFocused.RequestFocusCommit(true);
            }

            Host.RequestFocus(FocusKind.Begin, e.Location);
        }

        protected override void OnFigureDragStart(MouseEventArgs e) {
            base.OnFigureDragStart(e);

            _startPoint = e.Location;
            _rubberbandRequest.Toggle = _toggle;
            _rubberbandRequest.Bounds = new Rectangle(_startPoint, Size.Empty);
            Host.ShowFeedback(_rubberbandRequest);

            _oldCursor = Host.Site.EditorCanvas.Cursor;
            Host.Site.EditorCanvas.Cursor = Cursors.Default;
        }

        protected override void OnFigureDragMove(MouseEventArgs e) {
            base.OnFigureDragMove(e);

            _rubberbandRequest.Toggle = _toggle;
            _rubberbandRequest.Bounds = RectUtil.GetRectangleFromDiagonalPoints(_startPoint, e.Location);
            Host.ShowFeedback(_rubberbandRequest);
        }

        protected override void OnFigureDragFinish(MouseEventArgs e) {
            base.OnFigureDragFinish(e);

            _rubberbandRequest.Toggle = _toggle;
            _rubberbandRequest.Bounds = RectUtil.GetRectangleFromDiagonalPoints(_startPoint, e.Location);
            Host.HideFeedback(_rubberbandRequest);
            Host.PerformRequest(_rubberbandRequest);
            Host.Site.EditorCanvas.Cursor = _oldCursor;
        }

        protected override void OnFigureDragCancel() {
            base.OnFigureDragCancel();

            Host.HideFeedback(_rubberbandRequest);
            Host.Site.EditorCanvas.Cursor = _oldCursor;
        }

    }
}
