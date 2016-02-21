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
    internal class SplitLineSegmentCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private LineSegment _target;
        private int _inlineIndex;

        private LineSegment _splitted;

        // ========================================
        // constructor
        // ========================================
        public SplitLineSegmentCommand(LineSegment target, int inlineIndex) {
            _target = target;
            _inlineIndex = inlineIndex;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get {
                return
                    _target != null && _target.Root != null &&
                    _inlineIndex > 0 && _inlineIndex < _target.Length - 1;
            }
        }

        public override bool CanUndo {
            get { return true; }
        }

        public LineSegment Splitted {
            get { return _splitted; }
        }


        // ========================================
        // method
        // ========================================
        public override void Execute() {
            var root = _target.Root;
            _splitted = root.SplitLineSegment(_target, _inlineIndex);
        }

        public override void Undo() {
            var root = _target.Root;
            root.MergeLineSegments(_target, _splitted);
        }
    }
}
