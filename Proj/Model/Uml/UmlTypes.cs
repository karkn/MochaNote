/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Model.Uml {
    [Serializable]
    public enum UmlVisibilityKind {
        None = 0,
        Private,
        Protected,
        Public,
        Package,
    }

    [Serializable]
    public enum UmlAggregationKind {
        None,
        Shared,
        Composite,
    }

    [Serializable]
    public enum UmlParameterDirectionKind {
        In,
        Inout,
        Out,
        Return,
    }
}
