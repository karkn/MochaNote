/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mkamo.Common.Forms.KeyMap;
using System.Reflection;

namespace Mkamo.Memopad.Internal.KeyActions {
    [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = true)]
    internal static class ComboBoxKeyActions {
        [KeyAction("")]
        public static void NextItem(ComboBox comboBox) {
            SendKeys.Send("{DOWN}");
        }

        [KeyAction("")]
        public static void PreviousItem(ComboBox comboBox) {
            SendKeys.Send("{UP}");
        }
    }
}
