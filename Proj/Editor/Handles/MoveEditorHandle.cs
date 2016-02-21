/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Requests;
using System.Drawing;
using System.Windows.Forms;
using Mkamo.Editor.Core;
using Mkamo.Figure.Core;
using Mkamo.Editor.Handles.Scenarios;

namespace Mkamo.Editor.Handles {
    public class MoveEditorHandle: DefaultEditorHandle, IEditorHandle {
        private MoveScenario _scenario;

        public MoveEditorHandle() {
            _scenario = new MoveScenario(this);
            _scenario.Apply();
        }

    }
}
