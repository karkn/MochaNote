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
using System.Windows.Forms;
using Mkamo.Figure.Figures;
using System.Drawing;
using Mkamo.Editor.Requests;
using Mkamo.Common.Forms.Input;
using Mkamo.Editor.Handles.Scenarios;

namespace Mkamo.Editor.Handles {
    public abstract class AbstractSelectHandle: AbstractHandle {
        // ========================================
        // constructor
        // ========================================
        protected AbstractSelectHandle() {
            new SelectScenario(this).Apply();
        }
    }
}
