/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Memopad.Core;
using Mkamo.Memopad.Internal.Core;

namespace Mkamo.Memopad.Internal.Controls {
    internal class AllMemosPresenter: IMemoInfoListProvider {
        // ========================================
        // field
        // ========================================
        private MemopadApplication _facade;

        // ========================================
        // constructor
        // ========================================
        public AllMemosPresenter() {
            _facade = MemopadApplication.Instance;
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public IEnumerable<MemoInfo> MemoInfos {
            get { return _facade.MemoInfos; }
        }
    }
}
