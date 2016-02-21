/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Diagnostics;
using System.Drawing;
using Mkamo.Common.Event;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Mkamo.Common.Core;

namespace Mkamo.Common.Forms.Table {
    /// <summary>
    /// Tableの表示に必要な情報を扱う。
    /// </summary>
    [Serializable]
    public class TableData<T> {
        // ========================================
        // static field
        // ========================================
        private const int DefaultMinColumnWidth = 16;
        private const int DefaultMinRowHeight = 16;

        // ========================================
        // field
        // ========================================
        private Collection<Row<T>> _rows;
        private Collection<Column<T>> _columns;

        private int _rowCount;
        private int _columnCount;

        private int _minColumnWidth;
        private int _minRowHeight;

        [NonSerialized]
        private Lazy<HashSet<Cell<T>>> _mergingCells;

        // ========================================
        // constructor
        // ========================================
        public TableData() {
            _rows = new Collection<Row<T>>();
            _columns = new Collection<Column<T>>();
            _rowCount = 0;
            _columnCount = 0;

            _minColumnWidth = DefaultMinColumnWidth;
            _minRowHeight = DefaultMinRowHeight;

            _mergingCells = new Lazy<HashSet<Cell<T>>>(() => new HashSet<Cell<T>>());
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) {
            _mergingCells = new Lazy<HashSet<Cell<T>>>(() => new HashSet<Cell<T>>());

            foreach (var row in _rows) {
                foreach (var cell in row.Cells) {
                    if (cell.ColumnSpan != 1 || cell.RowSpan != 1) {
                        SetMergeState(cell, cell.ColumnSpan, cell.RowSpan, true);
                    }
                }
            }
        }

        // ========================================
        // event
        // ========================================
        [field:NonSerialized]
        public event EventHandler TableChanged;

        [field:NonSerialized]
        public event EventHandler<RowChangedEventArgs<T>> RowInserted;
        [field:NonSerialized]
        public event EventHandler<RowChangedEventArgs<T>> RowRemoved;
        [field:NonSerialized]
        public event EventHandler<RowChangedEventArgs<T>> RowChanged;

        [field:NonSerialized]
        public event EventHandler<ColumnChangedEventArgs<T>> ColumnInserted;
        [field:NonSerialized]
        public event EventHandler<ColumnChangedEventArgs<T>> ColumnRemoved;
        [field:NonSerialized]
        public event EventHandler<ColumnChangedEventArgs<T>> ColumnChanged;

        [field:NonSerialized]
        public event EventHandler<CellChangedEventArgs<T>> CellChanged;

        // ========================================
        // property
        // ========================================
        public IEnumerable<Row<T>> Rows {
            get { return _rows; }
        }

        public IEnumerable<Column<T>> Columns {
            get { return _columns; }
        }

        public IEnumerable<Cell<T>> Cells {
            get {
                foreach (var row in _rows) {
                    foreach (var cell in row.Cells) {
                        yield return cell;
                    }
                }
            }
        }

        public int RowCount {
            get { return _rowCount; }
        }

        public int ColumnCount {
            get { return _columnCount; }
        }

        public int CellCount {
            get { return _rowCount * _columnCount; }
        }

        public int Width {
            get { return _columns.Sum(col => col.Width); }
            set {
                if (!_columns.Any()) {
                    return;
                }

                SetWidth(value);
            }
        }

        public int Height {
            get { return _rows.Sum(row => row.Height); }
            set {
                if (!_rows.Any()) {
                    return;
                }

                SetHeight(value);
            }
        }

        public Size Size {
            get { return new Size(Width, Height); }
            set {
                if (value != Size) {
                    SetWidth(value.Width);
                    SetHeight(value.Height);
                }
            }
        }

        public int MinColumnWidth {
            get { return _minColumnWidth; }
            set {
                _minColumnWidth = value;
                OnTableChanged();
            }
        }

        public int MinRowHeight {
            get { return _minRowHeight; }
            set {
                _minRowHeight = value;
                OnTableChanged();
            }
        }

        public int MinWidth {
            get { return _columnCount * _minColumnWidth; }
        }

        public int MinHeight {
            get { return _rowCount * _minRowHeight; }
        }

        public Size MinSize {
            get { return new Size(MinWidth, MinHeight); }
        }

        // ========================================
        // method
        // ========================================
        public bool Contains(T value) {
            if (_rows.Any(row => row.Cells.Any(cell => EqualityComparer<T>.Default.Equals(cell.Value, value)))) {
                return true;
            }
            return false;
        }

        public Cell<T> GetCell(T value) {
            foreach (var row in _rows) {
                foreach (var cell in row.Cells) {
                    if (EqualityComparer<T>.Default.Equals(cell.Value, value)) {
                        return cell;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 見つからなければ-1を返す。
        /// </summary>
        public int GetRowIndex(T value) {
            for (int i = 0, len = _rows.Count; i < len; ++i) {
                var row = _rows[i];
                if (row.Cells.Any(cell => EqualityComparer<T>.Default.Equals(cell.Value, value))) {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 見つからなければ-1を返す。
        /// </summary>
        public int GetColumnIndex(T value) {
            for (int i = 0, len = _rows.Count; i < len; ++i) {
                var row = _rows[i];
                for (int j = 0, jlen = row.Cells.Count(); j < jlen; ++j) {
                    var cell = row.Cells.ElementAt(j);
                    if (EqualityComparer<T>.Default.Equals(cell.Value, value)) {
                        return j;
                    }
                }
            }
            return -1;
        }

        public int GetRowIndex(Row<T> row) {
            return _rows.IndexOf(row);
        }

        public int GetColumnIndex(Column<T> col) {
            return col.ColumnIndex;
        }

        public int GetRowIndex(Cell<T> cell) {
            for (int i = 0, len = _rows.Count; i < len; ++i) {
                var row = _rows[i];
                if (row.Cells.Any(c => c == cell)) {
                    return i;
                }
            }
            return -1;
        }

        public int GetColumnIndex(Cell<T> cell) {
            foreach (var row in _rows) {
                var i = 0;
                foreach (var c in row.Cells) {
                    if (c == cell) {
                        return i;
                    }
                    ++i;
                }
            }
            return -1;
        }

        public Row<T> GetRow(Cell<T> cell) {
            var row = GetRowIndex(cell);
            return row > -1? _rows[row]: null;
        }

        public Row<T> GetRow(T value) {
            var row = GetRowIndex(value);
            return row > -1? _rows[row]: null;
        }

        public Column<T> GetColumn(Cell<T> cell) {
            var col = GetColumnIndex(cell);
            return col > -1? _columns[col]: null;
        }

        public Column<T> GetColumn(T value) {
            var col = GetColumnIndex(value);
            return col > -1? _columns[col]: null;
        }

        public Cell<T> GetCell(int rowIndex, int colIndex) {
            var row = _rows[rowIndex];
            return row.Cells.ElementAt(colIndex);
        }

        public bool HasNextCell(Cell<T> cell) {
            var rowIndex = GetRowIndex(cell);
            Contract.Requires(rowIndex > -1);
            if (rowIndex < _rowCount - 1) {
                return true;
            }

            var colIndex = GetColumnIndex(cell);
            Contract.Requires(colIndex > -1);
            if (colIndex < _columnCount - 1) {
                return true;
            }

            return false;
        }

        public Cell<T> GetNextCell(Cell<T> cell) {
            var colIndex = GetColumnIndex(cell);
            Contract.Requires(colIndex > -1);
            if (colIndex < _columnCount - 1) {
                var row = GetRow(cell);
                return row.Cells.ElementAt(colIndex + 1);
            }
            
            var rowIndex = GetRowIndex(cell);
            Contract.Requires(rowIndex > -1);
            if (rowIndex < _rowCount - 1) {
                var row = _rows[rowIndex + 1];
                return row.Cells.ElementAt(0);
            }

            return null;
        }

        public bool HasPreviousCell(Cell<T> cell) {
            var rowIndex = GetRowIndex(cell);
            Contract.Requires(rowIndex > -1);
            if (rowIndex > 0) {
                return true;
            }

            var colIndex = GetColumnIndex(cell);
            Contract.Requires(colIndex > -1);
            if (colIndex > 0) {
                return true;
            }

            return false;
        }

        public Cell<T> GetPreviousCell(Cell<T> cell) {
            var colIndex = GetColumnIndex(cell);
            Contract.Requires(colIndex > -1);
            if (colIndex > 0) {
                var row = GetRow(cell);
                return row.Cells.ElementAt(colIndex - 1);
            }
            
            var rowIndex = GetRowIndex(cell);
            Contract.Requires(rowIndex > -1);
            if (rowIndex > 0) {
                var row = _rows[rowIndex - 1];
                return row.Cells.Last();
            }

            return null;
        }

        public bool HasNextRowCell(Cell<T> cell) {
            var rowIndex = GetRowIndex(cell);
            Contract.Requires(rowIndex > -1);
            if (rowIndex < _rowCount - 1) {
                return true;
            }

            return false;
        }

        public Cell<T> GetNextRowCell(Cell<T> cell) {
            var colIndex = GetColumnIndex(cell);
            var rowIndex = GetRowIndex(cell);
            Contract.Requires(colIndex > -1);
            Contract.Requires(rowIndex > -1);

            if (rowIndex < _rowCount - 1) {
                var row = _rows[rowIndex + 1];
                return row.Cells.ElementAt(colIndex);
            }

            return null;
        }

        public bool HasPreviousRowCell(Cell<T> cell) {
            var rowIndex = GetRowIndex(cell);
            Contract.Requires(rowIndex > -1);
            if (rowIndex > 0) {
                return true;
            }

            return false;
        }

        public Cell<T> GetPreviousRowCell(Cell<T> cell) {
            var colIndex = GetColumnIndex(cell);
            var rowIndex = GetRowIndex(cell);
            Contract.Requires(colIndex > -1);
            Contract.Requires(rowIndex > -1);

            if (rowIndex > 0) {
                var row = _rows[rowIndex - 1];
                return row.Cells.ElementAt(colIndex);
            }

            return null;
        }

        public bool HasNextColumn(Column<T> col) {
            var i = _columns.IndexOf(col);
            return i > -1 && i < _columns.Count - 1;
        }

        public bool HasPreviousColumn(Column<T> col) {
            var i = _columns.IndexOf(col);
            return i > 0;
        }

        public Column<T> GetNextColumn(Column<T> col) {
            var i = _columns.IndexOf(col);
            if (i > -1 && i < _columns.Count - 1) {
                return _columns[i + 1];
            }
            return null;
        }

        public Column<T> GetPreviousColumn(Column<T> col) {
            var i = _columns.IndexOf(col);
            if (i > 0) {
                return _columns[i - 1];
            }
            return null;
        }

        public bool HasNextRow(Row<T> row) {
            var i = _rows.IndexOf(row);
            return i > -1 && i < _rows.Count - 1;
        }

        public bool HasPreviousRow(Row<T> row) {
            var i = _rows.IndexOf(row);
            return i > 0;
        }

        public Row<T> GetNextRow(Row<T> row) {
            var i = _rows.IndexOf(row);
            if (i > -1 && i < _rows.Count - 1) {
                return _rows[i + 1];
            }
            return null;
        }

        public Row<T> GetPreviousRow(Row<T> row) {
            var i = _rows.IndexOf(row);
            if (i > 0) {
                return _rows[i - 1];
            }
            return null;
        }

        public int GetLeft(int colIndex) {
            var ret = 0;
            for (int i = 0, ilen = _columns.Count; i < ilen; ++i) {
                if (i == colIndex) {
                    return ret;
                }
                var col = _columns[i];
                ret += col.Width;
            }
            throw new ArgumentException("colIndex");
        }

        public int GetTop(int rowIndex) {
            var ret = 0;
            for (int i = 0, ilen = _rows.Count; i < ilen; ++i) {
                if (i == rowIndex) {
                    return ret;
                }
                var row = _rows[i];
                ret += row.Height;
            }
            throw new ArgumentException("rowIndex");
        }

        // --- redim ---
        public void Redim(int rowCount, int colCount) {
            _rows.Clear();
            _columns.Clear();
            _rowCount = 0;
            _columnCount = 0;

            for (int i = 0; i < colCount; ++i) {
                AddColumn();
            }

            for (int i = 0; i < rowCount; ++i) {
                AddRow();
            }

            OnTableChanged();
        }


        // --- row ---
        public Row<T> AddRow() {
            return InsertRow(_rowCount);
        }

        public Row<T> InsertRow(int index) {
            var ret = new Row<T>(this);
            for (int i = 0; i < _columnCount; ++i) {
                ret.AddCell();
            }
            _rows.Insert(index, ret);
            ++_rowCount;
            OnRowInserted(ret, index);
            return ret;
        }

        public void InsertRow(int index, Row<T> row) {
            _rows.Insert(index, row);
            ++_rowCount;
            OnRowInserted(row, index);
        }

        public void RemoveRow(Row<T> row) {
            Contract.Requires(row != null);
            var index = _rows.IndexOf(row);
            if (index > -1) {
                _rows.Remove(row);
                --_rowCount;
                OnRowRemoved(row, index);
            }
        }

        public void RemoveRowAt(int index) {
            Contract.Requires(index >= 0 && index < _rows.Count);
            var row = _rows[index];
            _rows.RemoveAt(index);
            --_rowCount;
            OnRowRemoved(row, index);
        }

        // --- column ---
        public Column<T> AddColumn() {
            return InsertColumn(_columnCount);
        }

        public Column<T> InsertColumn(int index) {
            foreach (var row in _rows) {
                row.InsertCell(index);
            }

            for (int i = index, len = _columns.Count; i < len; ++i) {
                var col = _columns[i];
                ++col.ColumnIndex;
            }

            var ret = new Column<T>(this, index);
            _columns.Insert(index, ret);
            ++_columnCount;
            OnColumnInserted(ret, index);
            return ret;
        }

        public void InsertColumn(int index, IEnumerable<Cell<T>> cells) {
            var cellsEnum = cells.GetEnumerator();
            foreach (var row in _rows) {
                cellsEnum.MoveNext();
                row.InsertCell(index, cellsEnum.Current);
            }
            cellsEnum.Dispose();

            for (int i = index, len = _columns.Count; i < len; ++i) {
                var col = _columns[i];
                ++col.ColumnIndex;
            }

            var ret = new Column<T>(this, index);
            _columns.Insert(index, ret);
            ++_columnCount;
            OnColumnInserted(ret, index);
        }

        public void RemoveColumn(Column<T> column) {
            Contract.Requires(column != null);
            
            var index = _columns.IndexOf(column);
            if (index > -1) {
                _columns.Remove(column);
                foreach (var row in _rows) {
                    row.RemoveCellAt(index);
                }
                for (int i = index, len = _columns.Count; i < len; ++i) {
                    var col = _columns[i];
                    --col.ColumnIndex;
                }
                --_columnCount;
                OnColumnRemoved(column, index);
            }
        }

        public void RemoveColumnAt(int index) {
            Contract.Requires(index >= 0 && index < _columns.Count);
            var column = _columns[index];
            _columns.RemoveAt(index);
            foreach (var row in _rows) {
                row.RemoveCellAt(index);
            }
            for (int i = index, len = _columns.Count; i < len; ++i) {
                var col = _columns[i];
                --col.ColumnIndex;
            }
            --_columnCount;
            OnColumnRemoved(column, index);
        }


        // --- merge ---
        public void SetSpan(Cell<T> cell, int colSpan, int rowSpan) {
            SetMergeState(cell, colSpan, rowSpan, false);
            cell.ColumnSpan = colSpan;
            cell.RowSpan = rowSpan;
        }


        // ------------------------------
        // protected
        // ------------------------------
        protected internal virtual void OnTableChanged() {
            var handler = TableChanged;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnRowInserted(Row<T> row, int index) {
            var handler = RowInserted;
            if (handler != null) {
                handler(this, new RowChangedEventArgs<T>(row, index));
            }
        }

        protected virtual void OnRowRemoved(Row<T> row, int index) {
            var handler = RowRemoved;
            if (handler != null) {
                handler(this, new RowChangedEventArgs<T>(row, index));
            }
        }

        protected internal virtual void OnRowChanged(Row<T> row, int index) {
            var handler = RowChanged;
            if (handler != null) {
                handler(this, new RowChangedEventArgs<T>(row, index));
            }
        }

        protected virtual void OnColumnInserted(Column<T> col, int index) {
            var handler = ColumnInserted;
            if (handler != null) {
                handler(this, new ColumnChangedEventArgs<T>(col, index));
            }
        }

        protected virtual void OnColumnRemoved(Column<T> col, int index) {
            var handler = ColumnRemoved;
            if (handler != null) {
                handler(this, new ColumnChangedEventArgs<T>(col, index));
            }
        }

        protected internal virtual void OnColumnChanged(Column<T> col, int index) {
            var handler = ColumnChanged;
            if (handler != null) {
                handler(this, new ColumnChangedEventArgs<T>(col, index));
            }
        }

        protected internal virtual void OnCellChanged(Cell<T> cell) {
            var handler = CellChanged;
            if (handler != null) {
                handler(this, new CellChangedEventArgs<T>(cell));
            }
        }

        // ------------------------------
        // private
        // ------------------------------
        private void SetHeight(int height) {
            if (!_rows.Any()) {
                return;
            }

            var newHeight = height > MinHeight ? height : MinHeight;
            var diff = newHeight - Height;
            if (diff == 0) {
                return;
            } else if (diff > 0) {
                _rows.Last().Height += diff;
            } else {
                /// diff < 0
                foreach (var row in _rows.Reverse<Row<T>>()) {
                    if (row.Height + diff >= MinRowHeight) {
                        row.Height += diff;
                        break;

                    } else {
                        diff += row.Height - MinRowHeight;
                        row.Height = MinRowHeight;
                    }
                }
            }
        }
        
        private void SetWidth(int width) {
            if (!_columns.Any()) {
                return;
            }

            var newWidth = width > MinWidth ? width : MinWidth;
            var diff = newWidth - Width;
            if (diff == 0) {
                return;
            } else if (diff > 0) {
                _columns.Last().Width += diff;
            } else {
                /// diff < 0
                foreach (var col in _columns.Reverse<Column<T>>()) {
                    if (col.Width + diff >= MinColumnWidth) {
                        col.Width += diff;
                        break;

                    } else {
                        diff += col.Width - MinColumnWidth;
                        col.Width = MinColumnWidth;
                    }
                }
            }
        }

        // --- merge ---
        private void SetMergeState(Cell<T> cell, int colSpan, int rowSpan, bool isInit) {
            var colIndex = GetColumnIndex(cell);
            var rowIndex = GetRowIndex(cell);

            var merging = cell;

            /// unmerge
            if (!isInit) {
                var oldColSpan = cell.ColumnSpan;
                var oldRowSpan = cell.RowSpan;

                if (oldColSpan != 1 || oldRowSpan != 1) {
                    for (int r = rowIndex; r < rowIndex + oldRowSpan; ++r) {
                        for (int c = colIndex; c < colIndex + oldColSpan; ++c) {
                            if (r == rowIndex && c == colIndex) {
                                var oldMerging = _rows[r]._cells[c];
                                _mergingCells.Value.Remove(oldMerging);

                            } else {
                                var oldMerged = _rows[r]._cells[c];
                                oldMerged.IsMerged = false;
                                oldMerged.Merging = null;
                            }
                        }
                    }
                }
            }

            /// merge
            if (colSpan != 1 || rowSpan != 1) {
                for (int r = rowIndex; r < rowIndex + rowSpan; ++r) {
                    for (int c = colIndex; c < colIndex + colSpan; ++c) {
                        if (r == rowIndex && c == colIndex) {
                            _mergingCells.Value.Add(merging);

                        } else {
                            var merged = _rows[r]._cells[c];
                            merged.IsMerged = true;
                            merged.Merging = merging;
                        }
                    }
                }
            }
        }

    }
}
