/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Common.Win32.Imm32 {
    public enum Imm32WindowMessage {
        WM_IME_STARTCOMPOSITION = 0x010D,
        WM_IME_ENDCOMPOSITION = 0x010E,
        WM_IME_COMPOSITION = 0x010F,
        WM_IME_SETCONTEXT = 0x0281,
    }

    public enum ImmAssociateContextExFlag: int {
        IACE_CHILDREN = 0x0001,
        IACE_DEFAULT = 0x0010,
        IACE_IGNORENOCONTEXT = 0x0020,
    }

    public enum ImmGetCompositionStringInfoType: int {
        GCS_COMPSTR = 0x0008,
        GCS_CURSORPOS = 0x0080,
        GCS_RESULTSTR = 0x0800,
    }
}
