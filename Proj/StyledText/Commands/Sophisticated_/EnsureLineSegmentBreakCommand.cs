/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.DataType;
using Mkamo.Common.Diagnostics;

namespace Mkamo.StyledText.Commands {
    using StyledText = Mkamo.StyledText.Core.StyledText;
    using Mkamo.Common.Command;

    /// <summary>
    /// targetのcharIndexの位置でline segmentが切れるように保証する．
    /// charIndexがもともとline segmentの切れ目の場合は何もしない．
    /// </summary>
    public class EnsureLineSegmentBreakCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private StyledText _target;
        private int _charIndex;

        private EnsureInlineBreakCommand _ensureInlineBreakCmd;
        private SplitLineSegmentCommand _splitLineSegCmd;
        private bool _isExecuted;

        // ========================================
        // constructor
        // ========================================
        public EnsureLineSegmentBreakCommand(StyledText target, int charIndex) {
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
            _ensureInlineBreakCmd = null;
            _splitLineSegCmd = null;
            _isExecuted = false;

            int charIndexInLine, lineSegOffset;
            var line = _target.GetLineSegmentAt(_charIndex, out charIndexInLine, out lineSegOffset);

            if (charIndexInLine != 0) {
                _ensureInlineBreakCmd = new EnsureInlineBreakCommand(_target, _charIndex);
                _ensureInlineBreakCmd.Execute();

                int inlineIndex, charIndexInInline;
                line.GetInlineAtLocal(charIndexInLine, out inlineIndex, out charIndexInInline);

                _splitLineSegCmd = new SplitLineSegmentCommand(line, inlineIndex);
                _splitLineSegCmd.Execute();
                _isExecuted = true;
            }
        }

        public override void Undo() {
            if (_isExecuted) {
                _splitLineSegCmd.Undo();
                _ensureInlineBreakCmd.Undo();
            }
        }
    }
}
