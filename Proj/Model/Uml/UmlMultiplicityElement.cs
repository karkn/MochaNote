/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Model.Uml {
    public interface UmlMultiplicityElement {
        // ========================================
        // property
        // ========================================
        bool IsOrdered { get; set; }
        bool IsUnique { get; set; }
        bool IsUpperUnlimited { get; set; }
        int Upper { get; set; }
        int Lower { get; set; }
    }
}
