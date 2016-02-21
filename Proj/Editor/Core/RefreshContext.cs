/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Mkamo.Editor.Core {
    public class RefreshContext {
        // ========================================
        // field
        // ========================================
        private EditorRefreshKind _kind;
        private PropertyChangedEventArgs _cause;

        // ========================================
        // constructor
        // ========================================
        public RefreshContext(EditorRefreshKind kind) {
            _kind = kind;
        }

        public RefreshContext(EditorRefreshKind kind, PropertyChangedEventArgs cause): this(kind) {
            _cause = cause;
        }

        // ========================================
        // property
        // ========================================
        public EditorRefreshKind Kind {
            get { return _kind; }
        }

        public PropertyChangedEventArgs Cause {
            get { return _cause; }
        }

        // ========================================
        // method
        // ========================================

    }
}
