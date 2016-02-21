/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Mkamo.Editor.Core {
    public interface ITool {
        void Installed(EditorCanvas host);
        void Uninstalled(EditorCanvas host);

        // --- mouse ---
        bool HandleMouseDown(MouseEventArgs e);
        bool HandleMouseMove(MouseEventArgs e);
        bool HandleMouseUp(MouseEventArgs e);

        bool HandleDragStart(MouseEventArgs e);
        bool HandleDragMove(MouseEventArgs e);
        bool HandleDragFinish(MouseEventArgs e);
        bool HandleDragCancel(EventArgs e);

        bool HandleMouseClick(MouseEventArgs e);
        bool HandleMouseDoubleClick(MouseEventArgs e);
        bool HandleMouseEnter(EventArgs e);
        bool HandleMouseLeave(EventArgs e);
        bool HandleMouseHover(EventArgs e);

        // --- key ---
        bool HandleKeyDown(KeyEventArgs e);
        bool HandleKeyUp(KeyEventArgs e);
        bool HandleKeyPress(KeyPressEventArgs e);
        bool HandlePreviewKeyDown(PreviewKeyDownEventArgs e);
    }
}
