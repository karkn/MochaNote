/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Requests;
using Mkamo.Editor.Core;
using System.Drawing;
using Mkamo.Common.Forms.Drawing;

namespace Mkamo.Editor.Tools {
    public class EraserTool: AbstractTool {
        // ========================================
        // field
        // ========================================
        private RemoveRequest _request;
        private Func<IEditor, bool> _removeJudge;

        private Point _prevPoint;

        // ========================================
        // constructor
        // ========================================
        public EraserTool(Func<IEditor, bool> removeJudge) {
            _removeJudge = removeJudge;
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public override bool HandleMouseDown(System.Windows.Forms.MouseEventArgs e) {
            _request = new RemoveRequest();
            _prevPoint = e.Location;
            return true;
        }

        public override bool HandleMouseMove(System.Windows.Forms.MouseEventArgs e) {
            return true;
        }

        public override bool HandleMouseUp(System.Windows.Forms.MouseEventArgs e) {
            return true;
        }

        public override bool HandleDragStart(System.Windows.Forms.MouseEventArgs e) {
            return true;
        }

        public override bool HandleDragMove(System.Windows.Forms.MouseEventArgs e) {
            if (e.Location == _prevPoint) {
                return true;
            }

            var r = RectUtil.GetRectangleFromDiagonalPoints(e.Location, _prevPoint);

            var target = _Host.RootEditor.FindEditor(
                editor => _removeJudge(editor) && editor.Figure.IntersectsWith(r) && editor.CanUnderstand(_request)
            );
            if (target != null) {
                target.RequestRemove();
            }

            _prevPoint = e.Location;

            return true;
        }

        public override bool HandleDragFinish(System.Windows.Forms.MouseEventArgs e) {
            return true;
        }

        public override bool HandleDragCancel(EventArgs e) {
            return true;
        }

        public override bool HandleMouseClick(System.Windows.Forms.MouseEventArgs e) {
            return true;
        }

        public override bool HandleMouseDoubleClick(System.Windows.Forms.MouseEventArgs e) {
            return true;
        }

        public override bool HandleMouseEnter(EventArgs e) {
            return true;
        }

        public override bool HandleMouseLeave(EventArgs e) {
            return true;
        }

        public override bool HandleMouseHover(EventArgs e) {
            return true;
        }

        public override bool HandleKeyDown(System.Windows.Forms.KeyEventArgs e) {
            return true;
        }

        public override bool HandleKeyUp(System.Windows.Forms.KeyEventArgs e) {
            return true;
        }

        public override bool HandleKeyPress(System.Windows.Forms.KeyPressEventArgs e) {
            return true;
        }

        public override bool HandlePreviewKeyDown(System.Windows.Forms.PreviewKeyDownEventArgs e) {
            return true;
        }
    }
}
