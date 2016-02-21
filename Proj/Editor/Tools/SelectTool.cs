/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using System.Windows.Forms;

namespace Mkamo.Editor.Tools {
    /// <summary>
    /// すべての処理をHandleやFocusに任せる．
    /// </summary>
    public class SelectTool: AbstractTool {
        // ========================================
        // method
        // ========================================
        public override bool HandleMouseDown(MouseEventArgs e) {
            return false;
        }

        public override bool HandleMouseMove(MouseEventArgs e) {
            return false;
        }

        public override bool HandleMouseUp(MouseEventArgs e) {
            return false;
        }

        public override bool HandleDragStart(MouseEventArgs e) {
            return false;
        }

        public override bool HandleDragMove(MouseEventArgs e) {
            return false;
        }

        public override bool HandleDragFinish(MouseEventArgs e) {
            return false;
        }

        public override bool HandleDragCancel(EventArgs e) {
            return false;
        }

        public override bool HandleMouseClick(MouseEventArgs e) {
            return false;
        }

        public override bool HandleMouseDoubleClick(MouseEventArgs e) {
            return false;
        }

        public override bool HandleMouseEnter(EventArgs e) {
            return false;
        }

        public override bool HandleMouseLeave(EventArgs e) {
            return false;
        }

        public override bool HandleMouseHover(EventArgs e) {
            return false;
        }

        public override bool HandleKeyDown(KeyEventArgs e) {
            return false;
        }

        public override bool HandleKeyUp(KeyEventArgs e) {
            return false;
        }

        public override bool HandleKeyPress(KeyPressEventArgs e) {
            return false;
        }

        public override bool HandlePreviewKeyDown(PreviewKeyDownEventArgs e) {
            return false;
        }

    }
}
