/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;

namespace Mkamo.Editor.Internal.Core {
    internal class NullFocusManager: IFocusManager {
        // ========================================
        // property
        // ========================================
        public bool IsEditorFocused {
            get { return false; }
        }

        public IEditor FocusedEditor {
            get { return null; }
        }

        public IFocus Focus {
            get { return null; }
        }

        // ========================================
        // event
        // ========================================
        public event EventHandler<FocusChangedEventArgs> FocusChanged {
            add { }
            remove { }
        }


        // ========================================
        // method
        // ========================================
        public void PerformFocus(IEditor focused) {

        }

        public void StopFocus(IEditor focused) {

        }

        public void ClearFocus() {

        }


    }
}
