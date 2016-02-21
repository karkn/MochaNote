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
using Mkamo.Common.Command;
using Mkamo.Common.Forms.Drawing;

namespace Mkamo.Editor.Commands {
    public class NewEdgePointCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private IEdge _target;
        private EdgePointRef _prevEdgePointRef;
        private Point _location;

        private EdgePointRef _createdEdgePointRef;

        // ========================================
        // constructor
        // ========================================
        public NewEdgePointCommand(IEdge target, EdgePointRef prevEdgePointRef, Point location) {
            _target = target;
            _prevEdgePointRef = prevEdgePointRef;
            _location = location;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null && _prevEdgePointRef != null; }
        }

        public override bool CanUndo {
            get { return true; }
        }

        // === NewEdgePointCommand ==========
        public IEdge Target {
            get { return _target; }
        }

        public EdgePointRef PrevEdgePointRef {
            get { return _prevEdgePointRef; }
        }

        public Point Location {
            get { return _location; }
        }

        // ========================================
        // method
        // ========================================
        public override void Execute() {
            var newIndex = _prevEdgePointRef.Index + 1;
            _target.InsertBendPoint(newIndex, _location);
            _createdEdgePointRef = _target.EdgePointRefs.ElementAt(newIndex);
        }

        public override void Undo() {
            _target.RemoveBendPoint(_createdEdgePointRef.Index);
        }

    }
}
