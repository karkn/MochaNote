/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using System.Drawing;
using System.Windows.Forms;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Common.Forms.MouseOperatable;
using Mkamo.Common.Forms.KeyMap;

namespace Mkamo.Editor.Core {
    public interface IFocus {
        // ========================================
        // event
        // ========================================
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

        event EventHandler<ShortcutKeyProcessEventArgs> ShortcutKeyProcess;
        event EventHandler<KeyEventArgs> KeyDown;
        event EventHandler<KeyEventArgs> KeyUp;
        event EventHandler<KeyPressEventArgs> KeyPress;
        event EventHandler<PreviewKeyDownEventArgs> PreviewKeyDown;

        // ========================================
        // property
        // ========================================
        object Value { get; set; }
        bool IsModified { get; }

        IEditor Host { get; }
        INode Figure { get; }
        IKeyMap<IFocus> KeyMap { get; set; }

        // ========================================
        // method
        // ========================================
        void Install(IEditor host);
        void Uninstall(IEditor host);
        void Relocate(IFigure hostFigure);

        void Begin(Point? location);
        bool Commit();
        void Rollback();

        IContextMenuProvider GetContextMenuProvider();

        FontDescription GetNextInputFont();
    }
}
