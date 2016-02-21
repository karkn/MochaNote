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
    internal class EditorCopyExtenderManager: IEditorCopyExtenderManager {
        // ========================================
        // field
        // ========================================
        private List<EditorCopyExtender> _extenders = new List<EditorCopyExtender>();

        // ========================================
        // constructor
        // ========================================

        // ========================================
        // property
        // ========================================
        public IEnumerable<EditorCopyExtender> Extenders {
            get { return _extenders; }
        }

        public void RegisterExtension(EditorCopyExtender extender) {
            if (!_extenders.Contains(extender)) {
                _extenders.Add(extender);
            }
        }
    }
}
