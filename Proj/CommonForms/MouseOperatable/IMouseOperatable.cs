/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Mkamo.Common.Forms.MouseOperatable {
    public interface IMouseOperatable {
        // ========================================
        // property
        // ========================================
        IDragSource DragSource { get; set; }
        IDragTarget DragTarget { get; set; }

        // ========================================
        // method
        // ========================================
        void HandleMouseClick(MouseEventArgs e);
        void HandleMouseDoubleClick(MouseEventArgs e);
        void HandleMouseTripleClick(MouseEventArgs e);

        void HandleMouseDown(MouseEventArgs e);
        void HandleMouseUp(MouseEventArgs e);
        void HandleMouseMove(MouseEventArgs e);

        void HandleMouseEnter();
        void HandleMouseLeave();
        void HandleMouseHover(MouseHoverEventArgs e);

        void HandleDragStart(MouseEventArgs e);
        void HandleDragMove(MouseEventArgs e);
        void HandleDragFinish(MouseEventArgs e);
        void HandleDragCancel();

        Cursor GetMouseCursor(MouseEventArgs e);
    }
}
