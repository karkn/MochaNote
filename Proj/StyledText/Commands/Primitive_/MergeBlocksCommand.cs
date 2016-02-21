/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Command;
using Mkamo.Common.Core;
using Mkamo.StyledText.Core;

namespace Mkamo.StyledText.Commands {
    internal class MergeBlocksCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private Block _target;
        private Block _merged;

        private int _mergedFirstLineIndex;

        // ========================================
        // constructor
        // ========================================
        public MergeBlocksCommand(Block target, Block merged) {
            _target = target;
            _merged = merged;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get {
                return
                    _target != null && _target.Parent != null &&
                    _merged != null && _target.Parent == _merged.Parent;
            }
        }

        public override bool CanUndo {
            get { return true; }
        }


        // ========================================
        // method
        // ========================================
        public override void Execute() {
            _mergedFirstLineIndex = _target._Lines.Count;

            var root = _target.Root;
            root.MergeBlocks(_target, _merged);
        }

        public override void Undo() {
            var root = _target.Root;
            root.SplitBlock(_target, _mergedFirstLineIndex, _merged);
        }
    }
}
