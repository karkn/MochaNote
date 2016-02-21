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
    internal class InsertTableRowCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private MemoTable _target;
        private int _rowIndex;

        private MemoTableRow _createdRow;

        // ========================================
        // constructor
        // ========================================
        public InsertTableRowCommand(MemoTable target, int rowIndex) {
            _target = target;
            _rowIndex = rowIndex;
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
            _createdRow = _target.InsertRow(_rowIndex);
        }

        public override void Undo() {
            _target.RemoveRowAt(_rowIndex);
        }

        public override void Redo() {
            var container = MemopadApplication.Instance.Container;
            container.Persist(_createdRow);
            _target.InsertRow(_rowIndex, _createdRow);
        }
    }
}
