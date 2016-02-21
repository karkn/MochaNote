/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using Mkamo.Editor.Core;
using Mkamo.Editor.Handles.Scenarios;

namespace Mkamo.Editor.Handles {
    public class ContainerEditorHandle: DefaultEditorHandle, IEditorHandle {
        private ContainerScenario _scenario;

        public ContainerEditorHandle() {
            _scenario = new ContainerScenario(this);
            _scenario.Apply();
        }
    }
}
