/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Mkamo.Common.Diagnostics;

namespace Mkamo.Model.Memo {
    /// <summary>
    /// MemoTableで必要時にnewされるがpersistはされない。
    /// </summary>
    public class MemoTableColumn {
        // ========================================
        // field
        // ========================================
        private MemoTable _table;
        private int _columnIndex;

        // ========================================
        // constructor
        // ========================================
        internal MemoTableColumn(MemoTable table, int columnIndex) {
            Contract.Requires(table != null);

            _table = table;
            _columnIndex = columnIndex;
        }

        // ========================================
        // property
        // ========================================
        public virtual IEnumerable<MemoTableCell> Cells {
            get {
                Contract.Requires(_columnIndex >= 0 && _columnIndex < _table.ColumnCount);

                var ret = new List<MemoTableCell>();
                foreach (var row in _table.Rows) {
                    ret.Add(row.Cells.ElementAt(_columnIndex));
                }
                return ret;
            }
        }

        internal virtual int ColumnIndex {
            get { return _columnIndex; }
            set { _columnIndex = value; }
        }

        // ========================================
        // method
        // ========================================

    }
}
