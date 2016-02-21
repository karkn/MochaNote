/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Mkamo.Common.Forms.KeyMap {
    public static class KeyMapUtil {
        public static bool ProcessKeyMap<T>(IKeyMap<T> keyMap, T target, Keys keyData) where T: System.Windows.Forms.Control {
            if (keyMap.IsDefined(keyData)) {
                var action = keyMap.GetAction(keyData);
                if (action != null) {
                    if (action(target)) {
                        return true;
                    }
                }
            }
            return false;
        }

    }
}
