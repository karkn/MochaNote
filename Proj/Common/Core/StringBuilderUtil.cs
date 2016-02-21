/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Common.Core {
    public static class StringBuilderUtil {
        // ========================================
        // static method
        // ========================================
        public static void Clear(this StringBuilder sb) {
            sb.Length = 0;
        }
    }
}
