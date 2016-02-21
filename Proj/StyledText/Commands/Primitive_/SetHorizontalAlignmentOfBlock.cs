/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.DataType;
using Mkamo.Common.Command;
using Mkamo.StyledText.Core;

namespace Mkamo.StyledText.Commands {
    public class SetHorizontalAlignmentOfBlock: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private Block _target;
        private HorizontalAlignment _horizontalAlignment;

        private HorizontalAlignment _oldHorizontalAlignment;

        // ========================================
        // constructor
        // ========================================
        public SetHorizontalAlignmentOfBlock(Block target, HorizontalAlignment horizontalAlignment) {
            _target = target;
            _horizontalAlignment = horizontalAlignment;
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
            _oldHorizontalAlignment = _target.HorizontalAlignment;
            _target.HorizontalAlignment = _horizontalAlignment;
        }

        public override void Undo() {
            _target.HorizontalAlignment = _oldHorizontalAlignment;
        }

    }
}
