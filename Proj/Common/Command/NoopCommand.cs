/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Common.Command {
    public class NoopCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================
        public NoopCommand() {
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return true; }
        }

        public override bool CanUndo {
            get { return false; }
        }


        // ========================================
        // method
        // ========================================
        public override void Execute() {
        }

        public override void Undo() {
        }
    }
}
