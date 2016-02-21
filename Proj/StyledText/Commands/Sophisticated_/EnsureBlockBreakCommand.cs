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
    /// targetのcharIndexの位置でblockが切れるように保証する．
    /// charIndexがもともとblockの切れ目の場合は何もしない．
    /// </summary>
    public class EnsureBlockBreakCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private StyledText _target;
        private int _charIndex;

        private EnsureLineSegmentBreakCommand _ensureLineSegBreakCmd;
        private SplitBlockCommand _splitBlockCmd;

        private bool _isExecuted;

        // ========================================
        // constructor
        // ========================================
        public EnsureBlockBreakCommand(StyledText target, int charIndex) {
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
            _ensureLineSegBreakCmd = null;
            _splitBlockCmd = null;
            _isExecuted = false;

            int charIndexInBlock, blockOffset;
            var block = _target.GetBlockAt(_charIndex, out charIndexInBlock, out blockOffset);

            if (charIndexInBlock != 0) {
                _ensureLineSegBreakCmd = new EnsureLineSegmentBreakCommand(_target, _charIndex);
                _ensureLineSegBreakCmd.Execute();

                var charIndex = _ensureLineSegBreakCmd.IsExecuted? charIndexInBlock + 1: charIndexInBlock;
                int lineIndex, charIndexInLine;
                block.GetLineSegmentAtLocal(charIndex, out lineIndex, out charIndexInLine);

                _splitBlockCmd = new SplitBlockCommand(block, lineIndex);
                _splitBlockCmd.Execute();
                _isExecuted = true;
            }
        }

        public override void Undo() {
            if (_isExecuted) {
                _splitBlockCmd.Undo();
                _ensureLineSegBreakCmd.Undo();
            }
        }
    }
}
