/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Common.Core {
    public static class MathUtil {
        // ========================================
        // static method
        // ========================================
        public static int RoundDiv(int i, int j) {
            //return (int) Math.Round((double) i / (double) j, MidpointRounding.ToEven);
            return (int) Math.Round((double) i / (double) j, MidpointRounding.AwayFromZero);
        }
    }
}
