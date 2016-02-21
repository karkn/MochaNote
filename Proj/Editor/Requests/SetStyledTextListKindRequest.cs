/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.StyledText.Core;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Common.DataType;

namespace Mkamo.Editor.Requests {
    public class SetStyledTextListKindRequest : AbstractRequest {
        // ========================================
        // field
        // ========================================
        public ListKind ListKind;
        public bool On;

        // ========================================
        // constructor
        // ========================================

        // ========================================
        // property
        // ========================================
        public override string Id {
            get { return RequestIds.SetStyledTextListKind; }
        }

        // ========================================
        // method
        // ========================================
    }
}
