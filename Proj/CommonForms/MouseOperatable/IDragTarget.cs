/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Mkamo.Common.Forms.MouseOperatable {
    public interface IDragTarget {
        // ========================================
        // event
        // ========================================
        event EventHandler<DragEventArgs> DragEnter;
        event EventHandler<DragEventArgs> DragDrop;
        event EventHandler<DragEventArgs> DragOver;
        event EventHandler<EventArgs> DragLeave;

        // ========================================
        // property
        // ========================================
        //DragDropEffects AllowedOperations { get; }
        //string[] SupportedFormats { get; }
    }
}
