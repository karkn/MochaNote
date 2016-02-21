/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Memopad.Core;

namespace Mkamo.Memopad.Internal.Core {
    internal class MemoInfoEventArgs: EventArgs {
        // ========================================
        // field
        // ========================================
        private MemoInfo _memoInfo;

        // ========================================
        // constructor
        // ========================================
        public MemoInfoEventArgs(MemoInfo memoInfo) {
            _memoInfo = memoInfo;
        }

        // ========================================
        // property
        // ========================================
        public MemoInfo MemoInfo {
            get { return _memoInfo; }
        }
    }
}
