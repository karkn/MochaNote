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
    internal class NullSelectionManager: ISelectionManager {
        // ========================================
        // property
        // ========================================
        public IEnumerable<IEditor> SelectedEditors {
            get { return EditorConsts.EmptyEditorList; }
        }

        public bool MultiSelect {
            get { return true; }
            set { }
        }

        
        // ========================================
        // event
        // ========================================
        public event EventHandler<EventArgs> SelectionChanged {
            add { }
            remove { }
        }
        

        // ========================================
        // method
        // ========================================
        public void Select(IEditor selected) {
            
        }

        public void SelectMulti(IEnumerable<IEditor> selecteds, bool toggle) {

        }

        public void Deselect(IEditor selected) {
            
        }

        public void DeselectAll() {
        }

    }
}
