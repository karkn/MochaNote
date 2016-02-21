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
using Mkamo.StyledText.Core;

namespace Mkamo.StyledText.Commands {
    using StyledText = Mkamo.StyledText.Core.StyledText;

    public class InsertLineBreakCommand: AbstractCommand, IRangeCommand {
        // ========================================
        // field
        // ========================================
        private StyledText _target;
        private int _index;

        private ICommand _command;
        private bool _isOpen;

        // ========================================
        // constructor
        // ========================================
        public InsertLineBreakCommand(StyledText target, int index, bool isOpen) {
            _target = target;
            _index = index;
            _isOpen = isOpen;

            _command = null;
        }

        public InsertLineBreakCommand(StyledText target, int index): this(target, index, false) {
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

        public Range ExecutedRange {
            get { return _isOpen? new Range(_index, 0): new Range(_index + 1, 0); }
        }

        public Range UndoneRange {
            get { return new Range(_index, 0); }
        }


        // ========================================
        // method
        // ========================================
        public override void Execute() {
            _command = null;
 
            int lineSegOffset, charIndexInLine;
            var lineSeg = _target.GetLineSegmentAt(_index, out charIndexInLine, out lineSegOffset);
            var block = lineSeg.Parent as Block;

            if (charIndexInLine == 0) {
                /// lineSegの最初の文字を指している場合，lineSegの直前に空のLineを作成して挿入する
                var lineIndex = block._Lines.IndexOf(lineSeg);
                var newLine = new LineSegment();
                block.Transfer(newLine);
                _command = new InsertLineSegmentToBlockCommand(block, newLine, lineIndex);

            } else {
                /// lineSegの途中の文字を指している場合，lineSegをそこで分割する
                _command = new EnsureLineSegmentBreakCommand(_target, _index);
            }

            _command.Execute();
        }

        public override void Undo() {
            if (_command != null) {
                _command.Undo();
            }
        }
    }
}
