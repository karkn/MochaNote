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
using System.Drawing;

namespace Mkamo.Editor.Requests {
    public class SetStyledTextColorRequest: AbstractRequest {
        // ========================================
        // field
        // ========================================
        public Color Color;

        // ========================================
        // constructor
        // ========================================
        public SetStyledTextColorRequest(Color color) {
            Color = color;
        }

        // ========================================
        // property
        // ========================================
        public override string Id {
            get { return RequestIds.SetStyledTextColor; }
        }

        // ========================================
        // method
        // ========================================
    }
}
