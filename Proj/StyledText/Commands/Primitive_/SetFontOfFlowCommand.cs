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
    internal class SetFontOfFlowCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private Flow _target;
        private FontDescription _font;

        private FontDescription _oldFont;

        // ========================================
        // constructor
        // ========================================
        public SetFontOfFlowCommand(Flow target, FontDescription font) {
            _target = target;
            _font = font;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null && _font != null; }
        }

        public override bool CanUndo {
            get { return true; }
        }


        // ========================================
        // method
        // ========================================
        public override void Execute() {
            _oldFont = _target._OwnFont;
            _target.Font = _font;
        }

        public override void Undo() {
            _target.Font = _oldFont;
        }
    }
}
