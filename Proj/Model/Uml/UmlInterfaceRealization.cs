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

namespace Mkamo.Model.Uml {
    [Externalizable(Type = typeof(UmlInterfaceRealization), FactoryMethodType = typeof(UmlFactory), FactoryMethod = "CreateInterfaceRealization")]
    public class UmlInterfaceRealization: UmlRealization {
        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================
        protected internal UmlInterfaceRealization() {
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================

    }
}
