/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Editor.Commands;

namespace Mkamo.Editor.Requests {
    public class ReorderRequest: AbstractRequest {
        // ========================================
        // field
        // ========================================
        public ReorderKind Kind;

        // ========================================
        // property
        // ========================================
        public override string Id {
            get { return RequestIds.Reorder; }
        }
    }

}
