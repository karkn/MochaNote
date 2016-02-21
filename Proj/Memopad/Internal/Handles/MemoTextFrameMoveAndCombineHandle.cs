/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Handles;
using System.Drawing;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Model.Memo;
using Mkamo.Editor.Requests;
using Mkamo.Editor.Core;
using Mkamo.Editor.Focuses;

namespace Mkamo.Memopad.Internal.Handles {
    internal class MemoTextFrameMoveAndCombineHandle: FrameMoveHandle {
        // ========================================
        // field
        // ========================================
        CombineRequest _request;
        IEditor _target;

        // ========================================
        // constructor
        // ========================================
        public MemoTextFrameMoveAndCombineHandle(int height, int frameWidth, Color foreground, IBrushDescription background)
            : base(height, frameWidth, foreground, background)
        {

        }

        // ========================================
        // method
        // ========================================
        protected override void OnFigureMouseDoubleClick(System.Windows.Forms.MouseEventArgs e) {
            base.OnFigureMouseDoubleClick(e);

            if (Host.Site.FocusManager.IsEditorFocused) {
                var stfocus = Host.Site.FocusManager.Focus as StyledTextFocus;
                if (stfocus != null) {
                    stfocus.SelectAll();
                }
            }
        }

        protected override void OnFigureDragStart(System.Windows.Forms.MouseEventArgs e) {
            base.OnFigureDragStart(e);

            _request = new CombineRequest();
            _request.Combineds = Host.Site.SelectionManager.SelectedEditors.Where(editor => editor.Model is MemoText).ToArray();
        }
        
        protected override void OnFigureDragMove(System.Windows.Forms.MouseEventArgs e) {
            base.OnFigureDragMove(e);

            var prev = _target;
            _target = Host.Root.FindEditor(
                e.Location,
                editor => !_request.Combineds.Contains(editor) && editor.Model is MemoText
            );
            if (_target != prev && prev != null) {
                prev.HideFeedback(_request);
            }
            if (_target != null) {
                if (_target.CanUnderstand(_request)) {
                    _target.ShowFeedback(_request);
                } else {
                    _target = null;
                }
            }
        }

        protected override void OnFigureDragFinish(System.Windows.Forms.MouseEventArgs e) {
            var prev = _target;
            _target = Host.Root.FindEditor(
                e.Location,
                editor => !_request.Combineds.Contains(editor) && editor.Model is MemoText
            );
            if (_target != prev && prev != null) {
                prev.HideFeedback(_request);
            }
            if (_target != null) {
                if (_target.CanUnderstand(_request)) {

                    /// Moveのフィードバック消去・キャンセル
                    base.OnFigureDragCancel();

                    _target.HideFeedback(_request);
                    _target.PerformRequest(_request);
                    _target = null;
                    return;
                }
                _target.HideFeedback(_request);
                _target = null;
            }

            base.OnFigureDragFinish(e);
        }

        protected override void OnFigureDragCancel() {
            if (_target != null) {
                _target.HideFeedback(_request);
                _target = null;
            }
            base.OnFigureDragCancel();
        }
    }
}
