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
using Mkamo.Common.DataType;
using Mkamo.StyledText.Core;

namespace Mkamo.StyledText.Commands {
    internal class RemoveStringFromInlineCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private Inline _target;
        private int _index;
        private int _length;

        private string _removedString;

        // ========================================
        // constructor
        // ========================================
        public RemoveStringFromInlineCommand(Inline target, int index, int length) {
            _target = target;
            _index = index;
            _length = length;
        }

        public RemoveStringFromInlineCommand(Inline target, int index): this(target, index, 0) {
            var range = new Range(0, _target.Length);
            var removingRange = range.Latter(index);
            _length = removingRange.Length;
        }

        public RemoveStringFromInlineCommand(Inline target, Range range): this(target, range.Offset, range.Length) {

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

        public Inline Target {
            get { return _target; }
        }

        public int Index {
            get { return _index; }
        }

        public int Length {
            get { return _length; }
        }

        // ========================================
        // method
        // ========================================
        public override void Execute() {
            _removedString = _target.Text.Substring(_index, _length);
            _target.Remove(_index, _length);
        }

        public override void Undo() {
            _target.Insert(_index, _removedString);
        }
    }
}
