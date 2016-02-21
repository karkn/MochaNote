/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Model.Uml {
    public interface UmlPackagableElement {
        // ========================================
        // property
        // ========================================
        UmlPackage OwingPackage { get; set; }

        // ========================================
        // method
        // ========================================
    }
}
