/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Mkamo.Common.Text {
    public interface ITextSelection {
        // ========================================
        // event
        // ========================================
        event EventHandler<EventArgs> SelectionChanged;

        // ========================================
        // property
        // ========================================
        bool IsEmpty { get; }
        int Offset { get; set; }
        int Length { get; set; }
        string Text { get; }

        // ========================================
        // method
        // ========================================
        void Clear();
        void SetSelection(int offset, int length);
    }
}
