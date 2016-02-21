/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;
using Mkamo.Common.Win32.Core;

namespace Mkamo.Common.Win32.Util {
    public class ExceptionUtil {
        public static void ThrowOnWin32Error(string message) {
            var lastWin32Error = Marshal.GetLastWin32Error();
            
            if (lastWin32Error != Win32Consts.ERROR_SUCCESS) {
                throw new Win32Exception(lastWin32Error, message + " (" + lastWin32Error.ToString() + ")");
            }
        }
    }
}
