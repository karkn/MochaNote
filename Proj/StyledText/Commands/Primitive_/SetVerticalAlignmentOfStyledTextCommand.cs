/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Command;
using Mkamo.Common.DataType;

namespace Mkamo.StyledText.Commands {
    using StyledText = Mkamo.StyledText.Core.StyledText;

    public class SetVerticalAlignmentOfStyledTextCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private StyledText _target;
        private VerticalAlignment _verticalAlignment;

        private VerticalAlignment _oldVerticalAlignment;

        // ========================================
        // constructor
        // ========================================
        public SetVerticalAlignmentOfStyledTextCommand(StyledText target, VerticalAlignment vAlign) {
            _target = target;
            _verticalAlignment = vAlign;
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
            _oldVerticalAlignment = _target.VerticalAlignment;
            _target.VerticalAlignment = _verticalAlignment;
        }

        public override void Undo() {
            _target.VerticalAlignment = _oldVerticalAlignment;
        }
    }
}
