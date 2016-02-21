/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Common.Event {
    [Serializable]
    public enum PropertyChangeKind {
        Set,
        Unset,

        /// list based property changes
        Add,
        Remove,
        Clear,
        // Replace,
        // Move,

        /// 上記で表せないすごい変更
        //Reset,
    }
}
