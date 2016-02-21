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
using Mkamo.Common.Collection;
using Mkamo.StyledText.Core;

namespace Mkamo.StyledText.Commands {
    internal class SplitBlockCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private Block _target;
        private int _lineIndex;

        private Block _splitted;

        // ========================================
        // constructor
        // ========================================
        public SplitBlockCommand(Block target, int lineIndex) {
            _target = target;
            _lineIndex = lineIndex;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get {
                return
                    _target != null && _target.Root != null &&
                    _lineIndex > 0 && _lineIndex < _target.Length - 1;
            }
        }

        public override bool CanUndo {
            get { return true; }
        }

        public Block Splitted {
            get { return _splitted; }
        }

        // ========================================
        // method
        // ========================================
        public override void Execute() {
            var root = _target.Root;
            _splitted = root.SplitBlock(_target, _lineIndex);
        }

        public override void Undo() {
            var root = _target.Root;
            root.MergeBlocks(_target, _splitted);
        }
    }
}
