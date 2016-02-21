/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Common.Forms.Table {
    public class RowChangedEventArgs<T>: EventArgs {
        // ========================================
        // field
        // ========================================
        private Row<T> _row;
        private int _index;

        // ========================================
        // constructor
        // ========================================
        public RowChangedEventArgs(Row<T> row, int index) {
            _row = row;
            _index = index;
        }

        // ========================================
        // property
        // ========================================
        public Row<T> Row {
            get { return _row; }
        }

        public int Index {
            get { return _index; }
        }
    }

    public class ColumnChangedEventArgs<T>: EventArgs {
        // ========================================
        // field
        // ========================================
        private Column<T> _column;
        private int _index;

        // ========================================
        // constructor
        // ========================================
        public ColumnChangedEventArgs(Column<T> column, int index) {
            _column = column;
            _index = index;
        }

        // ========================================
        // property
        // ========================================
        public Column<T> Column {
            get { return _column; }
        }

        public int Index {
            get { return _index; }
        }
    }


    public class CellChangedEventArgs<T>: EventArgs {
        // ========================================
        // field
        // ========================================
        private Cell<T> _cell;

        // ========================================
        // constructor
        // ========================================
        public CellChangedEventArgs(Cell<T> cell) {
            _cell = cell;
        }

        // ========================================
        // property
        // ========================================
        public Cell<T> Cell {
            get { return _cell; }
        }
    }
}
