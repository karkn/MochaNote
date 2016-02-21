/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Memopad.Core;
using Mkamo.Editor.Requests;

namespace Mkamo.Memopad.Internal.Requests {
    internal class ChangeRowHeightRequest: AbstractRequest {
        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================
        public ChangeRowHeightRequest(int rowIndex, int newHeight) {
            RowIndex = rowIndex;
            NewHeight = newHeight;
        }

        public ChangeRowHeightRequest(): this(0, 0) {
        }

        // ========================================
        // property
        // ========================================
        public int RowIndex { get; set; }
        public int NewHeight { get; set; }

        // ========================================
        // method
        // ========================================
        public override string Id {
            get { return MemopadRequestIds.ChangeRowHeight; }
        }

    }
}
