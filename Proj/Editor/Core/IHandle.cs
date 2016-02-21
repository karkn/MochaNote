/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mkamo.Common.Forms.MouseOperatable;

namespace Mkamo.Editor.Core {
    public interface IHandle {
        // ========================================
        // event
        // ========================================
        event EventHandler<EventArgs> Installed;
        event EventHandler<EventArgs> Uninstalling;

        event EventHandler<MouseEventArgs> MouseClick;
        event EventHandler<MouseEventArgs> MouseDoubleClick;
        event EventHandler<MouseEventArgs> MouseTripleClick;
        event EventHandler<MouseEventArgs> MouseDown;
        event EventHandler<MouseEventArgs> MouseUp;
        event EventHandler<MouseEventArgs> MouseMove;
        event EventHandler<EventArgs> MouseEnter;
        event EventHandler<EventArgs> MouseLeave;
        event EventHandler<MouseHoverEventArgs> MouseHover;
        event EventHandler<MouseEventArgs> DragStart;
        event EventHandler<MouseEventArgs> DragMove;
        event EventHandler<MouseEventArgs> DragFinish;
        event EventHandler<EventArgs> DragCancel;
        
        // ========================================
        // property
        // ========================================
        IEditor Host { get; }
        Cursor Cursor { get; set; }

        // ========================================
        // method
        // ========================================
        void Install(IEditor host);
        void Uninstall(IEditor host);
    }
}
