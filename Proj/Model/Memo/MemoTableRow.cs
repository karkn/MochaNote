/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Externalize;
using Mkamo.Model.Core;
using System.Collections.ObjectModel;
using Mkamo.Container.Core;
using Mkamo.Common.Diagnostics;
using System.Runtime.Serialization;

namespace Mkamo.Model.Memo {
    [Externalizable(
        Type = typeof(MemoTableRow),
        FactoryMethodType = typeof(MemoFactory), FactoryMethod = "CreateTableRow"
    )]
    [DataContract, Serializable]
    public class MemoTableRow: MemoElement {
        // ========================================
        // field
        // ========================================
        [DataMember(Name = "Cells")]
        private Collection<MemoTableCell> _cells;

        // ========================================
        // constructor
        // ========================================
        protected internal MemoTableRow() {
            _cells = new Collection<MemoTableCell>();
        }

        // ========================================
        // property
        // ========================================
        [Persist(Add = "AddCellInternal", Cascade = true), External(Add = "AddCellInternal")]
        public virtual IEnumerable<MemoTableCell> Cells {
            get { return _cells; }
        }


        // ========================================
        // method
        // ========================================
        public virtual void AddCellInternal(MemoTableCell cell) {
            Contract.Requires(cell != null);

            _cells.Add(cell);
        }

        [Dirty]
        public virtual MemoTableCell AddCell() {
            var ret = MemoFactory.CreateTableCell();
            _cells.Add(ret);
            return ret;
        }

        [Dirty]
        public virtual MemoTableCell InsertCell(int index) {
            Contract.Requires(index >= 0 && index <= _cells.Count);

            var ret = MemoFactory.CreateTableCell();
            InsertCell(index, ret);
            return ret;
        }

        [Dirty]
        public virtual void InsertCell(int index, MemoTableCell cell) {
            Contract.Requires(index >= 0 && index <= _cells.Count);

            _cells.Insert(index, cell);
        }

        [Dirty]
        public virtual void RemoveCell(MemoTableCell cell) {
            Contract.Requires(cell != null);

            _cells.Remove(cell);
        }

        [Dirty]
        public virtual void RemoveCellAt(int index) {
            Contract.Requires(index >= 0 && index < _cells.Count);

            _cells.RemoveAt(index);
        }
    }
}
