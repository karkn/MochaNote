/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.DataType;

namespace Mkamo.StyledText.Commands {
    using StyledText = Mkamo.StyledText.Core.StyledText;
    using Mkamo.Common.Command;

    /// <summary>
    /// targetのcharIndexの位置でinlineが切れるように保証する．
    /// charIndexがもともとinlineの切れ目の場合は何もしない．
    /// </summary>
    public class EnsureInlineBreakCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private StyledText _target;
        private int _charIndex;

        private SplitInlineCommand _command;
        private bool _isExecuted;

        // ========================================
        // constructor
        // ========================================
        public EnsureInlineBreakCommand(StyledText target, int charIndex) {
            _target = target;
            _charIndex = charIndex;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null && _charIndex > 0 && _charIndex < _target.Length + 1; }
        }

        public override bool CanUndo {
            get { return true; }
        }

        //public override Range UndoneRange {
        //    get { return new Range(_charIndex, 0); }
        //}

        // === EnsureBreakCommand ==========
        public StyledText Target {
            get { return _target; }
        }

        public int CharIndex {
            get { return _charIndex; }
        }

        public bool IsExecuted {
            get { return _isExecuted; }
        }

        // ========================================
        // method
        // ========================================
        public override void Execute() {
            _command = null;
            _isExecuted = false;

            int charIndexInInline, inlineOffset;
            var inline = _target.GetInlineAt(_charIndex, out charIndexInInline, out inlineOffset);

            if (charIndexInInline != 0) {
                _command = new SplitInlineCommand(inline, charIndexInInline);
                _command.Execute();
                _isExecuted = true;
            }
        }

        public override void Undo() {
            if (_isExecuted) {
                _command.Undo();
            }
        }
    }
}
