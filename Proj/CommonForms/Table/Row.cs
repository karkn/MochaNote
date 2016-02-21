/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Diagnostics;
using System.Collections.ObjectModel;

namespace Mkamo.Common.Forms.Table {
    [Serializable]
    public class Row<T> {
        // ========================================
        // static field
        // ========================================
        private const int DefaultHeight = 24;

        // ========================================
        // field
        // ========================================
        private TableData<T> _owner;

        internal  Collection<Cell<T>> _cells;
        private int _height;

        // ========================================
        // constructor
        // ========================================
        public Row(TableData<T> owner) {
            _owner = owner;
            _cells = new Collection<Cell<T>>();
            _height = DefaultHeight;
        }

        // ========================================
        // property
        // ========================================
        public IEnumerable<Cell<T>> Cells {
             get { return _cells; }
        }

        public int Height {
            get { return _height; }
            set {
                var newHeight = value > _owner.MinRowHeight? value: _owner.MinRowHeight;
                if (newHeight == _height) {
                    return;
                }
                _height = newHeight;
                _owner.OnRowChanged(this, _owner.GetRowIndex(this));
            }
        }

        // ========================================
        // method
        // ========================================
        internal Cell<T> AddCell() {
            var ret = new Cell<T>(_owner);
            _cells.Add(ret);
            return ret;
        }

        internal Cell<T> InsertCell(int index) {
            Contract.Requires(index >= 0 && index <= _cells.Count);

            var ret = new Cell<T>(_owner);
            _cells.Insert(index, ret);
            return ret;
        }

        internal void InsertCell(int index, Cell<T> cell) {
            Contract.Requires(index >= 0 && index <= _cells.Count);

            _cells.Insert(index, cell);
        }

        internal void RemoveCell(Cell<T> cell) {
            Contract.Requires(cell != null);

            _cells.Remove(cell);
        }

        internal void RemoveCellAt(int index) {
            Contract.Requires(index >= 0 && index < _cells.Count);

            _cells.RemoveAt(index);
        }

    }
}
