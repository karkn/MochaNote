/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Editor.Core {
    public class SelectionChangedEventArgs: EventArgs {
        // ========================================
        // field
        // ========================================
        private bool _selected;

        // ========================================
        // constructor
        // ========================================
        public SelectionChangedEventArgs(bool selected) {
            _selected = selected;
        }

        // ========================================
        // property
        // ========================================
        public bool Selected {
            get { return _selected; }
        }

    }
}
