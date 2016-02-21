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
    [Externalizable(
        Type = typeof(MemoTableLine),
        FactoryMethodType = typeof(MemoFactory), FactoryMethod = "CreateTableLine"
    )]
    [DataContract, Serializable]
    public class MemoTableLine {
        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================
        protected internal MemoTableLine() {
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================

    }
}
