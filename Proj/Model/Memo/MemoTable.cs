/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Container.Core;
using Mkamo.Common.Externalize;
using Mkamo.Model.Core;
using System.Collections.ObjectModel;
using Mkamo.Common.Diagnostics;
using System.Runtime.Serialization;

namespace Mkamo.Model.Memo {
    [Entity]
    [Externalizable(
        Type = typeof(MemoTable),
        FactoryMethodType = typeof(MemoFactory), FactoryMethod = "CreateTable"
    )]
    [DataContract, Serializable]
    public class MemoTable: MemoContent {
        // ========================================
        // field
        // ========================================
        [DataMember(Name = "Rows")]
        private Collection<MemoTableRow> _rows;
        
        [NonSerialized]
        private Collection<MemoTableColumn> _columns;

        [DataMember(Name = "RowCount")]
        private int _rowCount;
        [DataMember(Name = "ColumnCount")]
        private int _columnCount;

        // ========================================
        // constructor
        // ========================================
        protected internal MemoTable() {
            _rows = new Collection<MemoTableRow>();
            _columns = new Collection<MemoTableColumn>();
        }

        // ========================================
        // property
        // ========================================
        [Persist, External]
        public virtual int RowCount {
            get { return _rowCount; }
            set {
                /// load時にのみ呼ばれる
                _rowCount = value;
            }
        }

        [Persist, External]
        public virtual int ColumnCount {
            get { return _columnCount; }
            set {
                /// load時にのみ呼ばれる
                for (int i = 0; i < value; ++i) {
                    _columns.Add(new MemoTableColumn(this, i));
                }
                _columnCount = value;
            }
        }

        [Persist(Add = "AddRowInternal", Cascade = true), External(Add = "AddRowInternal")]
        public virtual IEnumerable<MemoTableRow> Rows {
            get { return _rows; }
        }

        public virtual IEnumerable<MemoTableColumn> Columns {
            get { return _columns; }
        }

        public virtual IEnumerable<MemoTableCell> Cells {
            get {
                foreach (var row in _rows) {
                    foreach (var cell in row.Cells) {
                        yield return cell;
                    }
                }
            }
        }

        // ========================================
        // method
        // ========================================
        // --- row ---
        public virtual void AddRowInternal(MemoTableRow row) {
            Contract.Requires(row != null);
            _rows.Add(row);
        }

        [Dirty]
        public virtual MemoTableRow AddRow() {
            var ret = MemoFactory.CreateTableRow();
            for (int i = 0; i < _columnCount; ++i) {
                ret.AddCell();
            }
            _rows.Add(ret);
            ++_rowCount;
            OnPropertyAdded(this, "Rows", ret, _rowCount - 1);
            return ret;
        }

        [Dirty]
        public virtual MemoTableRow InsertRow(int index) {
            var ret = MemoFactory.CreateTableRow();
            for (int i = 0; i < _columnCount; ++i) {
                ret.AddCell();
            }
            InsertRow(index, ret);
            return ret;
        }

        [Dirty]
        public virtual void InsertRow(int index, MemoTableRow row) {
            _rows.Insert(index, row);
            ++_rowCount;
            OnPropertyAdded(this, "Rows", row, index);
        }

        [Dirty]
        public virtual void RemoveRow(MemoTableRow row) {
            Contract.Requires(row != null);
            var index = _rows.IndexOf(row);
            if (index > -1) {
                RemoveRowAt(index);
            }
        }

        [Dirty]
        public virtual void RemoveRowAt(int index) {
            Contract.Requires(index >= 0 && index < _rows.Count);
            var row = _rows[index];
            _rows.RemoveAt(index);
            --_rowCount;
            OnPropertyRemoved(this, "Rows", row, index);
            MemoFactory.Remove(row);
        }

        // --- column ---
        [Dirty]
        public virtual MemoTableColumn AddColumn() {
            foreach (var row in _rows) {
                row.AddCell();
            }
            var ret = new MemoTableColumn(this, _columnCount);
            _columns.Add(ret);
            ++_columnCount;
            OnPropertyAdded(this, "Columns", ret, _columnCount - 1);
            return ret;
        }

        [Dirty]
        public virtual MemoTableColumn InsertColumn(int index) {
            foreach (var row in _rows) {
                row.InsertCell(index);
            }

            for (int i = index, len = _columns.Count; i < len; ++i) {
                var col = _columns[i];
                ++col.ColumnIndex;
            }

            var ret = new MemoTableColumn(this, index);
            _columns.Insert(index, ret);
            ++_columnCount;
            OnPropertyAdded(this, "Columns", ret, index);
            return ret;
        }

        [Dirty]
        public virtual MemoTableColumn InsertColumn(int index, IEnumerable<MemoTableCell> cells) {
            Contract.Requires(cells.Count() == _rowCount);

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

            var ret = new MemoTableColumn(this, index);
            _columns.Insert(index, ret);
            ++_columnCount;
            OnPropertyAdded(this, "Columns", ret, index);
            return ret;
        }

        [Dirty]
        public virtual void RemoveColumn(MemoTableColumn column) {
            Contract.Requires(column != null);
            
            var index = _columns.IndexOf(column);
            if (index > -1) {
                RemoveColumnAt(index);
            }
        }

        [Dirty]
        public virtual void RemoveColumnAt(int index) {
            Contract.Requires(index >= 0 && index < _columns.Count);

            var removedCells = new List<MemoTableCell>();

            var column = _columns[index];
            _columns.RemoveAt(index);
            foreach (var row in _rows) {
                removedCells.Add(row.Cells.ElementAt(index));
                row.RemoveCellAt(index);
            }

            for (int i = index, len = _columns.Count; i < len; ++i) {
                var col = _columns[i];
                --col.ColumnIndex;
            }
            --_columnCount;
            OnPropertyRemoved(this, "Columns", column, index);

            foreach (var cell in removedCells) {
                MemoFactory.Remove(cell);
            }
        }

    }
}
