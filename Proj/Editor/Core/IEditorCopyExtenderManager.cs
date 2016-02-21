/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Editor.Core {
    public interface IEditorCopyExtenderManager {
        IEnumerable<EditorCopyExtender> Extenders { get; }
        void RegisterExtension(EditorCopyExtender extender);
    }
}
