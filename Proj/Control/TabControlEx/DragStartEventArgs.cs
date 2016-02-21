/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Mkamo.Control.TabControlEx {
    public class DragStartEventArgs: EventArgs {
        // ========================================
        // field
        // ========================================
        private IEnumerable<object> _dragDataObjects;
        private DragDropEffects _allowedEffect;

        // ========================================
        // constructor
        // ========================================
        public DragStartEventArgs() {
        }

        // ========================================
        // property
        // ========================================
        public IEnumerable<object> DragDataObjects {
            get { return _dragDataObjects; }
            set { _dragDataObjects = value; }
        }

        public DragDropEffects AllowedEffect {
            get { return _allowedEffect; }
            set { _allowedEffect = value; }
        }

        // ========================================
        // method
        // ========================================

    }
}
