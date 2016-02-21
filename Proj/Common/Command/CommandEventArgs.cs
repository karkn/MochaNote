/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Common.Command {
    public class CommandEventArgs: EventArgs {
        // ========================================
        // field
        // ========================================
        private ICommand _command;

        // ========================================
        // constructor
        // ========================================
        public CommandEventArgs(ICommand command) {
            _command = command;
        }

        // ========================================
        // property
        // ========================================
        public ICommand Command {
            get { return _command; }
        }

        // ========================================
        // method
        // ========================================

    }
}
