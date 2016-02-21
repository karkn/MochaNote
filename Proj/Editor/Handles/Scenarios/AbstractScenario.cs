/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;

namespace Mkamo.Editor.Handles.Scenarios {
    public abstract class AbstractScenario {
        // ========================================
        // field
        // ========================================
        private IHandle _handle;

        // ========================================
        // constructor
        // ========================================
        protected AbstractScenario(IHandle handle) {
            _handle = handle;
        }

        // ========================================
        // property
        // ========================================
        public IHandle Handle {
            get { return _handle; }
        }

        public IEditor Host {
            get { return _handle.Host; }
        }

        // ========================================
        // method
        // ========================================
        public abstract void Apply();
    }
}
