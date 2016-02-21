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
    internal class MergeInlineCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private Inline _target;
        private Inline _merged;

        private int _index;

        // ========================================
        // constructor
        // ========================================
        public MergeInlineCommand(Inline target, Inline merged) {
            _target = target;
            _merged = merged;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null && _target.Root != null && _merged != null; }
        }

        public override bool CanUndo {
            get { return true; }
        }

        public Inline Splitted {
            get { return _merged; }
        }

        // ========================================
        // method
        // ========================================
        public override void Execute() {
            _index = _target.Length;

            var root = _target.Root;
            root.MergeInlines(_target, _merged);
        }

        public override void Undo() {
            var root = _target.Root;
            root.SplitInline(_target, _index, _merged);
        }
    }
}
