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
    internal class MergeLineSegmentsCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private LineSegment _target;
        private LineSegment _merged;

        private List<Inline> _mergedInlines;
        private int _mergedFirstInlineIndex;

        // ========================================
        // constructor
        // ========================================
        public MergeLineSegmentsCommand(LineSegment target, LineSegment merged) {
            _target = target;
            _merged = merged;

            _mergedInlines = new List<Inline>();
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
            _mergedFirstInlineIndex = _target._Inlines.Count - 1;

            var root = _target.Root;
            root.MergeLineSegments(_target, _merged);
        }

        public override void Undo() {
            var root = _target.Root;
            root.SplitLineSegment(_target, _mergedFirstInlineIndex, _merged);
        }
    }
}
