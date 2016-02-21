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
    public class SetNodeBackgroundCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private IEditor _target;
        private IBrushDescription _newBackground;

        private bool _oldIsBackgroundEnabled;
        private IBrushDescription _oldBackground;

        // ========================================
        // constructor
        // ========================================
        public SetNodeBackgroundCommand(IEditor target, IBrushDescription newBackground) {
            _target = target;
            _newBackground = newBackground;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null && (_target.Figure as INode) != null; }
        }

        public override bool CanUndo {
            get { return true; }
        }


        // ========================================
        // method
        // ========================================
        public override void Execute() {
            var node = _target.Figure as INode;
            if (node != null) {
                _oldBackground = node.Background;
                _oldIsBackgroundEnabled = node.IsBackgroundEnabled;

                node.Background = _newBackground;
                node.IsBackgroundEnabled = _newBackground != null;
            }
        }

        public override void Undo() {
            var node = _target.Figure as INode;
            if (node != null) {
                node.Background = _oldBackground;
                node.IsBackgroundEnabled = _oldIsBackgroundEnabled;
            }
        }
    }
}
