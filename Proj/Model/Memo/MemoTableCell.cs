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
using Mkamo.Container.Core;
using System.Runtime.Serialization;

namespace Mkamo.Model.Memo {
    [Externalizable(
        Type = typeof(MemoTableCell),
        FactoryMethodType = typeof(MemoFactory), FactoryMethod = "CreateTableCell"
    )]
    [DataContract, Serializable]
    public class MemoTableCell: MemoStyledText {
        // ========================================
        // field
        // ========================================
        [DataMember(Name = "RowSpan")]
        private int _rowSpan;
        [DataMember(Name = "ColumnSpan")]
        private int _columnSpan;

        // ========================================
        // constructor
        // ========================================
        protected internal MemoTableCell() {
            _rowSpan = 1;
            _columnSpan = 1;
        }

        // ========================================
        // property
        // ========================================
        [Persist, External]
        public virtual int RowSpan {
            get { return _rowSpan; }
            set {
                if (_rowSpan == value) {
                    return;
                }
                var old = _rowSpan;
                _rowSpan = value;
                OnPropertySet(this, "RowSpan", old, value);
            }
        }

        [Persist, External]
        public virtual int ColumnSpan {
            get { return _columnSpan; }
            set {
                if (_columnSpan == value) {
                    return;
                }
                var old = _columnSpan;
                _columnSpan = value;
                OnPropertySet(this, "ColumnSpan", old, value);
            }
        }


        // ========================================
        // method
        // ========================================

    }
}
