/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Command;
using Mkamo.StyledText.Core;

namespace Mkamo.StyledText.Commands {
    using StyledText = Mkamo.StyledText.Core.StyledText;

    internal class InsertBlockToStyledTextCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private StyledText _target;
        private Block _inserted;
        private int _index;

        // ========================================
        // constructor
        // ========================================
        public InsertBlockToStyledTextCommand(StyledText target, Block inserted, int index) {
            _target = target;
            _inserted = inserted;
            _index = index;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null && _inserted != null; }
        }

        public override bool CanUndo {
            get { return true; }
        }


        // ========================================
        // method
        // ========================================
        public override void Execute() {
            _target.Insert(_index, _inserted);
        }

        public override void Undo() {
            _target.Remove(_inserted);
        }
    }
}
