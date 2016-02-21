/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Mkamo.StyledText.Core {
    // ========================================
    // type
    // ========================================
    /// <summary>
    /// ParagraphのListの種類．
    /// </summary>
    [Serializable]
    public enum ListKind {
        None,
        Unordered,
        Ordered,
        Star,
        LeftArrow,
        RightArrow,
        CheckBox,
        TriStateCheckBox,
    }

    [Serializable]
    public enum ListStateKind {
        Unchecked = 0,
        Checked,
        Indeterminate,
    }

    [Serializable]
    public enum ParagraphKind {
        Normal = 0,
        Heading1,
        Heading2,
        Heading3,
        Heading4,
        Heading5,
        Heading6,
    }

    [Serializable]
    public enum StyledTextModificationKind {
        Insert,
        Remove,
        Replace,
    }

}
