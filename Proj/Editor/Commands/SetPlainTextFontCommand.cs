/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Command;
using Mkamo.Editor.Core;
using Mkamo.Figure.Core;
using Mkamo.Common.Forms.Descriptions;

namespace Mkamo.Editor.Commands {
    public class SetPlainTextFontCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private IEditor _target;
        private FontDescription _newFont;

        private FontDescription _oldFont;

        // ========================================
        // constructor
        // ========================================
        public SetPlainTextFontCommand(IEditor target, FontDescription newFont) {
            _target = target;
            _newFont = newFont;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null && _target.Figure is INode; }
        }

        public override bool CanUndo {
            get { return true; }
        }

        // ========================================
        // method
        // ========================================
        public override void Execute() {
            var node = _target.Figure as INode;
            _oldFont = node.Font;
            node.Font = _newFont;
        }

        public override void Undo() {
            var node = _target.Figure as INode;
            node.Font = _oldFont;
        }
    }
}
