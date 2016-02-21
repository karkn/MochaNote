/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Common.Forms.Win32.Util {
    public static class WindowMessageUtil {
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
        public static int GetHiWord(int num) {
            return ((num >> 16) & 0xffff);
        }

        public static int GetLoWord(int num) {
            return (num & 0xffff);
        }

        public static int MakeInt(int lo, int hi) {
            return (hi << 16) | (lo & 0xffff);
        } 

    }
}
