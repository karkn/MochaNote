/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.StyledText;
using System.Drawing;
using Mkamo.Common.Command;
using Mkamo.StyledText.Core;

namespace Mkamo.StyledText.Commands {
    internal class SetColorOfFlowCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private Flow _target;
        private Color _color;

        private Color _oldColor;

        // ========================================
        // constructor
        // ========================================
        public SetColorOfFlowCommand(Flow target, Color color) {
            _target = target;
            _color = color;
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
            _oldColor = _target.Color;
            _target.Color = _color;
        }

        public override void Undo() {
            _target.Color = _oldColor;
        }
    }
}
