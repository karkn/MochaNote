/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Figure.Core;
using System.Drawing;
using Mkamo.Common.Forms.Drawing;
using Mkamo.Common.Command;

namespace Mkamo.Editor.Commands {
    public class MoveEdgePointCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private IEdge _target;
        private EdgePointRef _edgePointRef;
        private Point _location;

        private Point _oldLocation;

        // ========================================
        // constructor
        // ========================================
        public MoveEdgePointCommand(IEdge target, EdgePointRef edgePointRef, Point location) {
            _target = target;
            _edgePointRef = edgePointRef;
            _location = location;
        }

        // ========================================
        // property
        // ========================================
        // === ICommand ==========
        public override bool CanExecute {
            get { return _target != null && _edgePointRef != null; }
        }

        public override bool CanUndo {
            get { return true; }
        }

        // === MoveEdgePointCommand ==========
        public IEdge Target {
            get { return _target; }
        }

        public EdgePointRef EdgePointRef {
            get { return _edgePointRef; }
        }

        public Point Location {
            get { return _location; }
        }

        // ========================================
        // method
        // ========================================
        public override void Execute() {
            _oldLocation = _edgePointRef.EdgePoint;
            _edgePointRef.EdgePoint = _location;
        }

        public override void Undo() {
            _edgePointRef.EdgePoint = _oldLocation;
        }

    }
}
