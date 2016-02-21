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
    public static class Contract {
        [Conditional("DEBUG")]
        public static void Requires(bool condition) {
            if (!condition) {
                throw new Exception("Requires failed");
                //Debug.Assert(false, "Requires failed");
            }
        }

        [Conditional("DEBUG")]
        public static void Requires(bool condition, string description) {
            if (!condition) {
                throw new Exception("Requires failed by " + description);
                //Debug.Assert(false, "Requires failed by " + description);
            }
        }

        [Conditional("DEBUG")]
        public static void Ensures(bool condition) {
            if (!condition) {
                throw new Exception("Ensures failed");
                //Debug.Assert(false, "Ensures failed");
            }
        }

        [Conditional("DEBUG")]
        public static void Ensures(bool condition, string description) {
            if (!condition) {
                throw new Exception("Ensures failed by " + description);
                //Debug.Assert(false, "Ensures failed by " + description);
            }
        }
    }
}
