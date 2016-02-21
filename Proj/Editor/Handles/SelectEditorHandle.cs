/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Figure.Core;
using System.Windows.Forms;
using Mkamo.Editor.Requests;
using Mkamo.Editor.Handles.Scenarios;

namespace Mkamo.Editor.Handles {
    public class SelectEditorHandle: DefaultEditorHandle, IEditorHandle {
        private SelectScenario _scenario;

        public SelectEditorHandle() {
            _scenario = new SelectScenario(this);
            _scenario.Apply();
        }
    }
}
