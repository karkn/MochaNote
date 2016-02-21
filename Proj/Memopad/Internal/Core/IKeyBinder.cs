/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Forms.Input;
using Mkamo.Common.Forms.KeyMap;

namespace Mkamo.Memopad.Internal.Core {
    internal interface IKeyBinderOld<T> {
        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        void Bind(IKeyMap<T> keyMap);
    }
}
