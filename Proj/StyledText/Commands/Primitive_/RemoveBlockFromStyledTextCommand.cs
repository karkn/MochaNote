/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Command;
using Mkamo.Common.Core;
using Mkamo.StyledText.Core;

namespace Mkamo.StyledText.Commands {
    using StyledText = Mkamo.StyledText.Core.StyledText;

    internal class RemoveBlockFromStyledTextCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private StyledText _target;
        private Block _removed;

        private int _removedIndex;
        

        // ========================================
        // constructor
        // ========================================
        public RemoveBlockFromStyledTextCommand(StyledText target, Block removed) {
            _target = target;
            _removed = removed;
        }


        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null && _removed != null; }
        }

        public override bool CanUndo {
            get { return true; }
        }


        // ========================================
        // method
        // ========================================
        public override void Execute() {
            _removedIndex = _target._Blocks.IndexOf(_removed);
            _target.Remove(_removed);
        }

        public override void Undo() {
            _target.Insert(_removedIndex, _removed);
        }
    }
}
