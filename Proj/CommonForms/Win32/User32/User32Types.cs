/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Mkamo.Common.Win32.User32 {
    // ========================================
    // type
    // ========================================
    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    public enum WindowShowStyle: int {
        Hide = 0,
        ShowNormal = 1,
        ShowMinimized = 2,
        ShowMaximized = 3,
        Maximize = 3,
        ShowNormalNoActivate = 4,
        Show = 5,
        Minimize = 6,
        ShowMinNoActivate = 7,
        ShowNoActivate = 8,
        Restore = 9,
        ShowDefault = 10,
        ForceMinimized = 11
    }

    public enum SetWindowPosHWnd: int {
        TopMost = -1,
        NoTopMost = -2,
        Top = 0,
        Bottom = 1,
    }

    [Flags]
    public enum SetWindowPosFlags: int {
        NoSize = 0x0001,
        NoMove = 0x0002,
        NoZOrder = 0x0004,
        NoRedraw = 0x0008,
        NoActivate = 0x0010,
        FrameChanged = 0x0020,  /* The frame changed: send WM_NCCALCSIZE */
        ShowWindow = 0x0040,
        HideWindow = 0x0080,
        NoCopyBits = 0x0100,
        NoOwnerZOrder = 0x0200,  /* Don't do owner Z ordering */
        NoSendChanging = 0x0400,  /* Don't send WM_WINDOWPOSCHANGING */
    }

    public enum ClipboardFormat: int {
        CF_TEXT = 1,
        CF_BITMAP = 2,
        CF_METAFILEPICT = 3,
        CF_SYLK = 4,
        CF_DIF = 5,
        CF_TIFF = 6,
        CF_OEMTEXT = 7,
        CF_DIB = 8,
        CF_PALETTE = 9,
        CF_PENDATA = 10,
        CF_RIFF = 11,
        CF_WAVE = 12,
        CF_UNICODETEXT = 13,
        CF_ENHMETAFILE = 14,
        CF_HDROP = 15,
        CF_LOCALE = 16,
        CF_DIBV5 = 17,
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct DRAWTEXTPARAMS {
        public uint cbSize;
        public int iTabLength;
        public int iLeftMargin;
        public int iRightMargin;
        public uint uiLengthDrawn;

        public void CalcCbSize() {
            cbSize = (uint) Marshal.SizeOf(typeof(DRAWTEXTPARAMS));
        }
    }

    [Flags]
    public enum DRAWTEXTFORMATS: int {
        DT_TOP = 0x00000000,
        DT_LEFT = 0x00000000,
        DT_CENTER = 0x00000001,
        DT_RIGHT = 0x00000002,
        DT_VCENTER = 0x00000004,
        DT_BOTTOM = 0x00000008,
        DT_WORDBREAK = 0x00000010,
        DT_SINGLELINE = 0x00000020,
        DT_EXPANDTABS = 0x00000040,
        DT_TABSTOP = 0x00000080,
        DT_NOCLIP = 0x00000100,
        DT_EXTERNALLEADING = 0x00000200,
        DT_CALCRECT = 0x00000400,
        DT_NOPREFIX = 0x00000800,
        DT_INTERNAL = 0x00001000,
        DT_EDITCONTROL = 0x00002000,
        DT_PATH_ELLIPSIS = 0x00004000,
        DT_END_ELLIPSIS = 0x00008000,
        DT_MODIFYSTRING = 0x00010000,
        DT_RTLREADING = 0x00020000,
        DT_WORD_ELLIPSIS = 0x00040000,
        DT_NOFULLWIDTHCHARBREAK = 0x00080000,
        DT_HIDEPREFIX = 0x00100000,
        DT_PREFIXONLY = 0x00200000,
    }

    public enum GetWindowCmd: uint {
        GW_HWNDFIRST = 0,
        GW_HWNDLAST = 1,
        GW_HWNDNEXT = 2,
        GW_HWNDPREV = 3,
        GW_OWNER = 4,
        GW_CHILD = 5,
        GW_ENABLEDPOPUP = 6
    }

    public enum GetNextWindowCommand: uint {
        Next = 2,
        Prev = 3,
    }
}
