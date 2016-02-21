/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Mkamo.Common.Diagnostics {
    public static class StackTraceUtil {
        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public static string GetStackTraceString() {
            var ret = new StringBuilder();
            var trace = new StackTrace();
            for (int StackLoop = 0; StackLoop < trace.FrameCount; ++StackLoop) {
                var frame = trace.GetFrame(StackLoop);
                var method = frame.GetMethod();
                ret.AppendLine(method.ToString());
            }
            return ret.ToString();
        }
    }
}
