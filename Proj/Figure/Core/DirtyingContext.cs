/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Figure.Core {
    public class DirtyingContext: IDisposable {
        // ========================================
        // field
        // ========================================
        private IDirtManager _owner;

        // ========================================
        // constructor
        // ========================================
        internal DirtyingContext(IDirtManager owner) {
            _owner = owner;
        }

        // ========================================
        // method
        // ========================================
        public void Dispose() {
            _owner.EndDirty();
            GC.SuppressFinalize(this);
        }
    }
}
