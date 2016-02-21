/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Command;
using Mkamo.StyledText.Core;

namespace Mkamo.StyledText.Commands {
    internal class AppendStringToInlineCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private Inline _target;
        private string _text;

        private int _oldLength;

        // ========================================
        // constructor
        // ========================================
        public AppendStringToInlineCommand(Inline target, string text) {
            _target = target;
            _text = text;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null && _text != null; }
        }

        public override bool CanUndo {
            get { return true; }
        }

        // ========================================
        // method
        // ========================================
        public override void Execute() {
            _oldLength = _target.Length;
            _target.Append(_text);
        }

        public override void Undo() {
            _target.Remove(_oldLength);
        }
    }
}
