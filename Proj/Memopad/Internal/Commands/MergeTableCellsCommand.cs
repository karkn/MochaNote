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
using Mkamo.Model.Memo;

namespace Mkamo.Memopad.Internal.Commands {
    internal class MergeTableCellsCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private IEditor _target;
        private int _columnIndex;
        private int _rowIndex;
        private int _columnSpan;
        private int _rowSpan;

        private MemoTableCell _cell;
        private int _oldColumnSpan;
        private int _oldRowSpan;

        // ========================================
        // constructor
        // ========================================
        public MergeTableCellsCommand(
            IEditor target,
            int columnIndex,
            int rowIndex,
            int columnSpan,
            int rowSpan
        ) {
            _target = target;
            _columnIndex = columnIndex;
            _rowIndex = rowIndex;
            _columnSpan = columnSpan;
            _rowSpan = rowSpan;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null && _target.Model is MemoTable; }
        }

        public override bool CanUndo {
            get { return true; }
        }


        // ========================================
        // method
        // ========================================
        public override void Execute() {
            var model = _target.Model as MemoTable;

            var row = model.Rows.ElementAt(_rowIndex);
            _cell = row.Cells.ElementAt(_columnIndex);

            _oldColumnSpan = _cell.ColumnSpan;
            _oldRowSpan = _cell.RowSpan;

            _cell.ColumnSpan = _columnSpan;
            _cell.RowSpan = _rowSpan;
        }

        public override void Undo() {
            _cell.ColumnSpan = _oldColumnSpan;
            _cell.RowSpan = _oldRowSpan;
        }
    }
}
