/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mkamo.Figure.Core;
using Mkamo.Common.Forms.KeyMap;

namespace Mkamo.Editor.Core {
    /// <summary>
    /// Editor#Figureに関連付けられたHandle．
    /// </summary>
    public interface IEditorHandle: IHandle {
        // ========================================
        // event
        // ========================================
        event EventHandler<ShortcutKeyProcessEventArgs> ShortcutKeyProcess;
        event EventHandler<KeyEventArgs> KeyDown;
        event EventHandler<KeyEventArgs> KeyUp;
        event EventHandler<KeyPressEventArgs> KeyPress;
        event EventHandler<PreviewKeyDownEventArgs> PreviewKeyDown;

        // ========================================
        // property
        // ========================================
        IKeyMap<IEditor> KeyMap { get; set; }
    }
}
