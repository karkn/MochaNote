/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Figure.Core {
    /// <summary>
    /// Edgeの経路を調整する．
    /// </summary>
    public interface IRouter {
        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        void Route(IEdge edge);
    }
}
