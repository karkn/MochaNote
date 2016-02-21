/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.StyledText;
using Mkamo.Common.Command;
using Mkamo.StyledText.Core;

namespace Mkamo.StyledText.Commands {
    internal class InsertLineSegmentToBlockCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private Block _target;
        private LineSegment _insertedLine;
        private int _index;

        // ========================================
        // constructor
        // ========================================
        public InsertLineSegmentToBlockCommand(Block target, LineSegment insertedLine, int index) {
            _target = target;
            _insertedLine = insertedLine;
            _index = index;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null && _insertedLine != null; }
        }

        public override bool CanUndo {
            get { return true; }
        }


        // ========================================
        // method
        // ========================================
        public override void Execute() {
            _target.Insert(_index, _insertedLine);
        }

        public override void Undo() {
            _target.Remove(_insertedLine);
        }
    }
}
