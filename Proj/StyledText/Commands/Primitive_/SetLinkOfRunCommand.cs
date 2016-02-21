/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.StyledText;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Common.Command;
using Mkamo.StyledText.Core;

namespace Mkamo.StyledText.Commands {
    internal class SetLinkOfRunCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private Run _target;
        private Link _newLink;

        private Link _oldLink;

        // ========================================
        // constructor
        // ========================================
        public SetLinkOfRunCommand(Run target, Link newLink) {
            _target = target;
            _newLink = newLink;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null; }
        }

        public override bool CanUndo {
            get { return true; }
        }


        // ========================================
        // method
        // ========================================
        public override void Execute() {
            _oldLink = _target.Link;
            _target.Link = _newLink;
        }

        public override void Undo() {
            _target.Link = _oldLink;
        }
    }
}
