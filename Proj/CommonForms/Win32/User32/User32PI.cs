/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Mkamo.Common.Win32.Core;

namespace Mkamo.Common.Win32.User32 {
    public static class User32PI {
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);
        
        // --- device context ---
        [DllImport("User32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("User32.dll")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hdc);

        // --- caret ---
        [DllImport("user32.dll")]
        public static extern int CreateCaret(
            IntPtr hwnd, IntPtr hbm, int cx, int cy
        );

        [DllImport("user32.dll")]
        public static extern int DestroyCaret();

        [DllImport("user32.dll")]
        public static extern int SetCaretPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern int ShowCaret(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern int HideCaret(IntPtr hwnd);

        // --- window ---
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, WindowShowStyle nCmdShow);
        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, WindowShowStyle nCmdShow);


        [DllImport("user32.dll")]
        public static extern bool IsIconic(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(
            IntPtr hWnd,
            SetWindowPosHWnd hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPosFlags uFlags
        );

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.Dll")]
        public static extern int EnumWindows(EnumWindowsProc proc, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, GetWindowCmd uCmd);

        [DllImport("user32", EntryPoint = "GetWindow")]
        public static extern IntPtr GetNextWindow(IntPtr hWnd, GetNextWindowCommand uCmd);

        [DllImport("user32.dll")]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool LockWindowUpdate(long hWnd);


        // --- clipboard ---
        [DllImport("user32.dll")]
        public static extern IntPtr GetOpenClipboardWindow();

        [DllImport("user32.dll")]
        public static extern bool OpenClipboard(IntPtr hWndNewOwner);
        [DllImport("user32.dll")]
        public static extern bool EmptyClipboard();
        [DllImport("user32.dll")]
        public static extern IntPtr SetClipboardData(ClipboardFormat uFormat, IntPtr hMem);
        [DllImport("user32.dll")]
        public static extern bool CloseClipboard();
        [DllImport("gdi32.dll")]
        public static extern IntPtr CopyEnhMetaFile(IntPtr hemfSrc, IntPtr hNULL);
        [DllImport("gdi32.dll")]
        public static extern bool DeleteEnhMetaFile(IntPtr hemf);
        [DllImport("user32.dll")]
        public static extern int IsClipboardFormatAvailable(ClipboardFormat uFormat);
        [DllImport("user32.dll")]
        public static extern IntPtr GetClipboardData(ClipboardFormat uFormat);

        // --- process ---
        [DllImport("user32.dll")]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        // --- icon ---
        [DllImport("user32.dll")]
    	public extern static bool DestroyIcon(IntPtr handle);

        // --- text ---
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int DrawTextExW(
            IntPtr hdc,
            //StringBuilder lpchText,
            string lpchText,
            int cchText,
            ref RECT lprc,
            DRAWTEXTFORMATS dwDTFormat,
            ref DRAWTEXTPARAMS lpDTParams
        );


        // --- mouse ---
        [DllImport("user32")]
        public static extern bool TrackMouseEvent(ref TRACKMOUSEEVENT lpEventTrack);

        // --- focus ---
        [DllImport("user32")]
        public static extern IntPtr GetFocus();


        // --- misc ---
        [DllImport("user32.dll")]
        public extern static bool AttachThreadInput(int idAttach, int idAttachTo, bool fAttach);

        // --- keyboard ---
        [DllImport("user32.dll")]
        public static extern bool GetKeyboardState(byte[] lpKeyState);
        [DllImport("user32.dll")]
        public static extern bool SetKeyboardState(byte[] lpKeyState);


        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern int SendInput(int nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        public static extern IntPtr GetMessageExtraInfo();

    
    }
}
