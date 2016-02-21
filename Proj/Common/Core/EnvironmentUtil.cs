/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Common.Core {
    public static class EnvironmentUtil {
        public static bool IsWinXP() {
            var osver = Environment.OSVersion;
            return
                osver.Platform == PlatformID.Win32NT &&
                osver.Version.Major == 5 &&
                osver.Version.Minor == 1;
        }

        public static bool IsWinVista() {
            var osver = Environment.OSVersion;
            return
                osver.Platform == PlatformID.Win32NT &&
                osver.Version.Major == 6 &&
                osver.Version.Minor == 0;
        }

        public static bool IsWin7() {
            var osver = Environment.OSVersion;
            return
                osver.Platform == PlatformID.Win32NT &&
                osver.Version.Major == 6 &&
                osver.Version.Minor == 1;
        }
    }
}
