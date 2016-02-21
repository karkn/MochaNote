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
    internal class SplitInlineCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private Inline _target;
        private int _index;

        private Inline _splitted;

        // ========================================
        // constructor
        // ========================================
        public SplitInlineCommand(Inline target, int index) {
            _target = target;
            _index = index;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get {
                return
                    _target != null && _target.Root != null &&
                    _index > 0 && _index < _target.Length - 1;
            }
        }

        public override bool CanUndo {
            get { return true; }
        }

        public Inline Splitted {
            get { return _splitted; }
        }

        // ========================================
        // method
        // ========================================
        public override void Execute() {
            var root = _target.Root;
            _splitted = root.SplitInline(_target, _index);
        }

        public override void Undo() {
            var root = _target.Root;
            root.MergeInlines(_target, _splitted);
        }
    }
}
