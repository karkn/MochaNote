/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Editor.Requests;
using Mkamo.Common.Forms.Input;
using System.Drawing;
using System.Windows.Forms;
using Mkamo.Editor.Handles.Scenarios;

namespace Mkamo.Editor.Handles {
    public abstract class AbstractMoveHandle: AbstractHandle {
        // ========================================
        // constructor
        // ========================================
        protected AbstractMoveHandle() {
            new MoveScenario(this).Apply();
        }
    }
}
