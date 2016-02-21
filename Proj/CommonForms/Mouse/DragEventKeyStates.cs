/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Mkamo.Common.Forms.Mouse {
    [Flags, Serializable]
    public enum DragEventKeyStates: int {
        LeftButton = 0x01,
        RightButton = 0x02,
        Shift = 0x04,
        Ctrl = 0x08,
        MiddleButton = 0x10,
        Alt = 0x20,
    }
}
