/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Mkamo.Common.Win32.Kernel32 {
    public static class Kernel32PI {
        [DllImport("kernel32")]
        public static extern int GetCurrentThreadId();

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateFile(
            string lpFileName, // ファイル名
            DesiredAccess dwDesiredAccess, // アクセスモード
            ShareMode dwShareMode, // 共有モード
            int lpSecurityAttributes, // セキュリティ記述子
            CreationDisposition dwCreationDisposition, // 作成方法
            FlagsAndAttributes dwFlagsAndAttributes, // ファイル属性
            IntPtr hTemplateFile // テンプレートファイルのハンドル
            );

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteFile(string name);

        [DllImport("kernel32", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr hObject);


    }
}
