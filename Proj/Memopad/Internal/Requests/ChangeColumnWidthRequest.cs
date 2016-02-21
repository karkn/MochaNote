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
    internal class ChangeColumnWidthRequest: AbstractRequest {
        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================
        public ChangeColumnWidthRequest(int colIndex, int newWidth) {
            ColumnIndex = colIndex;
            NewWidth = newWidth;
        }

        public ChangeColumnWidthRequest(): this(0, 0) {
        }

        // ========================================
        // property
        // ========================================
        public int ColumnIndex { get; set; }
        public int NewWidth { get; set; }

        // ========================================
        // method
        // ========================================
        public override string Id {
            get { return MemopadRequestIds.ChangeColumnWidth; }
        }

    }
}
