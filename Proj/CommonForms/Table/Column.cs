/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Diagnostics;

namespace Mkamo.Common.Forms.Table {
    [Serializable]
    public class Column<T> {
        // ========================================
        // static field
        // ========================================
        private const int DefaultWidth = 64;

        // ========================================
        // field
        // ========================================
        private TableData<T> _owner;

        private int _columnIndex;

        private int _width;

        // ========================================
        // constructor
        // ========================================
        public Column(TableData<T> owner, int columnIndex) {
            _owner = owner;
            _columnIndex = columnIndex;
            _width = DefaultWidth;
        }

        // ========================================
        // property
        // ========================================
        public virtual IEnumerable<Cell<T>> Cells {
            get {
                Contract.Requires(_columnIndex >= 0 && _columnIndex < _owner.ColumnCount);

                var ret = new List<Cell<T>>();
                foreach (var row in _owner.Rows) {
                    ret.Add(row.Cells.ElementAt(_columnIndex));
                }
                return ret;
            }
        }

        public int ColumnIndex {
            get { return _columnIndex; }
            internal set { _columnIndex = value; }
        }

        public int Width {
            get { return _width; }
            set {
                var newWidth = value > _owner.MinColumnWidth? value: _owner.MinColumnWidth;
                if (newWidth == _width) {
                    return;
                }
                _width = newWidth;
                _owner.OnColumnChanged(this, _columnIndex);
            }
        }

        // ========================================
        // method
        // ========================================

    }
}
