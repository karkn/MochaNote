/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Command;
using Mkamo.Model.Memo;
using Mkamo.Memopad.Internal.Core;

namespace Mkamo.Memopad.Internal.Commands {
    internal class InsertTableColumnCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private MemoTable _target;
        private int _columnIndex;

        private List<MemoTableCell> _createdCells;

        // ========================================
        // constructor
        // ========================================
        public InsertTableColumnCommand(MemoTable target, int columnIndex) {
            _target = target;
            _columnIndex = columnIndex;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null; }
        }

        public override bool CanUndo {
            get { return true; }
        }


        // ========================================
        // method
        // ========================================
        public override void Execute() {
            var createdCol = _target.InsertColumn(_columnIndex);

            _createdCells = new List<MemoTableCell>();
            _createdCells.AddRange(createdCol.Cells);
        }

        public override void Undo() {
            _target.RemoveColumnAt(_columnIndex);
        }

        public override void Redo() {
            var container = MemopadApplication.Instance.Container;
            foreach (var cell in _createdCells) {
                container.Persist(cell);
            }
            _target.InsertColumn(_columnIndex, _createdCells);
        }
    }
}
