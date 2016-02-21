/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.StyledText;
using Mkamo.Common.Collection;
using Mkamo.Common.Command;
using Mkamo.StyledText.Core;

namespace Mkamo.StyledText.Commands {
    internal class RemoveLineSegmentFromBlockCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private Block _target;
        private LineSegment _removedLine;

        private int _removedIndex;

        // ========================================
        // constructor
        // ========================================
        public RemoveLineSegmentFromBlockCommand(Block target, LineSegment removedLine) {
            _target = target;
            _removedLine = removedLine;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null && _removedLine != null; }
        }

        public override bool CanUndo {
            get { return true; }
        }


        // ========================================
        // method
        // ========================================
        public override void Execute() {
            _removedIndex = _target._Lines.IndexOf(_removedLine);
            _target.Remove(_removedLine);
        }

        public override void Undo() {
            _target.Insert(_removedIndex, _removedLine);
        }
    }
}
