/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Mkamo.Common.Forms.Input {
    public static class KeyUtil {
        public static bool IsControlPressed() {
            return (Control.ModifierKeys & Keys.Control) != 0;
        }

        public static bool IsShiftPressed() {
            return (Control.ModifierKeys & Keys.Shift) != 0;
        }

        public static bool IsAltPressed() {
            return (Control.ModifierKeys & Keys.Alt) != 0;
        }

        public static bool IsLWinPressed() {
            return (Control.ModifierKeys & Keys.LWin) != 0;
        }

        public static bool IsRWinPressed() {
            return (Control.ModifierKeys & Keys.RWin) != 0;
        }

        public static bool IsWinPressed() {
            return IsLWinPressed() || IsRWinPressed();
        }
    }
}
