/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Mkamo.Common.Win32.Psapi {
    public static class PsapiPI {
        [DllImport("psapi.dll")]
        public extern static bool EmptyWorkingSet(IntPtr handle);

    }
}
