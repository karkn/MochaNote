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
using Mkamo.Container.Core;

namespace Mkamo.Model.Memo {
    [Externalizable(Type = typeof(MemoTagCollection),
        FactoryMethodType = typeof(MemoFactory),
        FactoryMethod = "CreateMemoTagCollection")]
    public class MemoTagCollection: EntityCollection<MemoTag> {
        // ========================================
        // constructor
        // ========================================
        protected internal MemoTagCollection() {
        }
    }
}
