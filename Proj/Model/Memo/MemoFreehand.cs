/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Externalize;
using Mkamo.Model.Core;
using System.Runtime.Serialization;

namespace Mkamo.Model.Memo {
    [Externalizable(Type = typeof(MemoFreehand), FactoryMethodType = typeof(MemoFactory), FactoryMethod = "CreateFreehand")]
    [DataContract, Serializable]
    public class MemoFreehand: MemoContent {
        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================
        protected internal MemoFreehand() {
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================

    }
}
