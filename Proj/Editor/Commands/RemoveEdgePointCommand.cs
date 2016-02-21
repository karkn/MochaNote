/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using System.Drawing;
using Mkamo.Common.Command;

namespace Mkamo.Editor.Commands {
    public class RemoveEdgePointCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private IEdge _target;
        private int _index;

        private Point _oldLocation;

        // ========================================
        // constructor
        // ========================================
        public RemoveEdgePointCommand(IEdge target, int index) {
            _target = target;
            _index = index;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null && _index > -1 && _index < _target.EdgePointCount; }
        }

        public override bool CanUndo {
            get { return true; }
        }

        public IEdge Target {
            get { return _target; }
        }

        public int Index {
            get { return _index; }
        }

        // ========================================
        // method
        // ========================================
        public override void Execute() {
            _oldLocation = _target[_index];
            _target.RemoveBendPoint(_index);
        }

        public override void Undo() {
            _target.InsertBendPoint(_index, _oldLocation);
        }
    }
}
