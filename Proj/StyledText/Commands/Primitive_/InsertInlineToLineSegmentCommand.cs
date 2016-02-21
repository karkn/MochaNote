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
    internal class InsertInlineToLineSegmentCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private LineSegment _target;
        private Inline _insertedInline;
        private int _index;

        // ========================================
        // constructor
        // ========================================
        public InsertInlineToLineSegmentCommand(LineSegment target, Inline insertedInline, int index) {
            _target = target;
            _insertedInline = insertedInline;
            _index = index;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null && _insertedInline != null; }
        }

        public override bool CanUndo {
            get { return true; }
        }


        // ========================================
        // method
        // ========================================
        public override void Execute() {
            _target.Insert(_index, _insertedInline);
        }

        public override void Undo() {
            _target.Remove(_insertedInline);
        }
    }
}
