/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Mkamo.Common.Win32.Core {
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT {
        public static RECT FromRectangle(Rectangle rect) {
            return new RECT() {
                left = rect.Left,
                top = rect.Top,
                right = rect.Right,
                bottom = rect.Bottom,
            };
        }

        public int left;
        public int top;
        public int right;
        public int bottom;

        public Rectangle ToRectangle() {
            return new Rectangle(left, top, right - left, bottom - top);
        }
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct SIZE {
        public int cx;
        public int cy;
        public SIZE(int cx, int cy) {
            this.cx = cx;
            this.cy = cy;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct COMPOSITIONFORM {
        public CompositionFormStyle dwStyle;
        public POINT ptCurrentPos;
        public RECT rcArea;
    }

    [Flags]
    public enum CompositionFormStyle: int {
        CFS_DEFAULT = 0x0000,
        CFS_RECT = 0x0001,
        CFS_POINT = 0x0002,
        CFS_FORCE_POSITION = 0x0020,
        CFS_CANDIDATEPOS = 0x0040,
        CFS_EXCLUDE = 0x0080,
    }


    [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
    public struct LOGFONT {
        public int lfHeight;
        public int lfWidth;
        public int lfEscapement;
        public int lfOrientation;
        public int lfWeight;
        public byte lfItalic;
        public byte lfUnderline;
        public byte lfStrikeOut;
        public byte lfCharSet;
        public byte lfOutPrecision;
        public byte lfClipPrecision;
        public byte lfQuality;
        public byte lfPitchAndFamily;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string lfFaceName;
    }

    public struct MOUSEINPUT {
        public int dx;
        public int dy;
        public uint mouseData;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    public struct KEYBDINPUT {
        public ushort wVk;
        public ushort wScan;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    public struct HARDWAREINPUT {
        public uint uMsg;
        public ushort wParamL;
        public ushort wParamH;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct INPUT {
        [FieldOffset(0)]
        public uint type;
        [FieldOffset(4)]
        public MOUSEINPUT mi;
        [FieldOffset(4)]
        public KEYBDINPUT ki;
        [FieldOffset(4)]
        public HARDWAREINPUT hi;
    }

    public struct TRACKMOUSEEVENT {
        private const int TME_HOVER = 0x1;
        public int cbSize;
        public int dwFlags;
        public IntPtr hwndTrack;
        public int dwHoverTime;
        public TRACKMOUSEEVENT(IntPtr hWnd, int hoverTime) {
            this.cbSize = Marshal.SizeOf(typeof(TRACKMOUSEEVENT));
            this.hwndTrack = hWnd;
            this.dwHoverTime = hoverTime;
            this.dwFlags = TME_HOVER;
        }
    }

}
