/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Mkamo.Common.Win32.Core;

namespace Mkamo.Common.Win32.Imm32 {

    public static class Imm32PI {

        [DllImport("Imm32.dll")]
        public static extern bool ImmAssociateContextEx(IntPtr hWnd, IntPtr himc, ImmAssociateContextExFlag dwFlags);

        [DllImport("Imm32.dll")]
        public static extern bool ImmSetOpenStatus(IntPtr himc, bool flag);

        [DllImport("Imm32.dll")]
        public static extern bool ImmGetOpenStatus(IntPtr himc);

        [DllImport("Imm32.dll")]
        public static extern IntPtr ImmCreateContext();

        [DllImport("Imm32.dll")]
        public static extern IntPtr ImmGetContext(IntPtr hwnd);

        [DllImport("Imm32.dll")]
        public static extern int ImmReleaseContext(IntPtr hwnd, IntPtr himc);

        [DllImport("Imm32.dll")]
        public static extern int ImmSetCompositionWindow(IntPtr himc, ref COMPOSITIONFORM lpCompositionForm);

        [DllImport("Imm32.dll")]
        public static extern int ImmGetCompositionWindow(IntPtr himc, ref COMPOSITIONFORM lpCompositionForm);

        [DllImport("Imm32.dll", CharSet = CharSet.Auto)]
        public static extern int ImmSetCompositionFont(IntPtr himc, ref LOGFONT lpLogFont);

        [DllImport("Imm32.dll")]
        public static extern int ImmGetCompositionString(IntPtr himc, ImmGetCompositionStringInfoType infoType, byte[] str, int len);

        [DllImport("imm32.dll")]
        public static extern int ImmGetVirtualKey(IntPtr hwnd);
    }
}
