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
    internal class RemoveInlineFromLineSegmentCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private LineSegment _target;
        private Inline _removedInline;

        private int _removedIndex;

        // ========================================
        // constructor
        // ========================================
        public RemoveInlineFromLineSegmentCommand(LineSegment target, Inline removedInline) {
            _target = target;
            _removedInline = removedInline;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null && _removedInline != null; }
        }

        public override bool CanUndo {
            get { return true; }
        }


        // ========================================
        // method
        // ========================================
        public override void Execute() {
            _removedIndex = _target._Inlines.IndexOf(_removedInline);
            _target.Remove(_removedInline);
        }

        public override void Undo() {
            _target.Insert(_removedIndex, _removedInline);
        }
    }
}
