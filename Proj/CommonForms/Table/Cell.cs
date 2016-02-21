/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Mkamo.Common.Diagnostics;

namespace Mkamo.Common.Forms.Table {
    [Serializable]
    public class Cell<T> {
        // ========================================
        // field
        // ========================================
        private TableData<T> _owner;

        private T _value;
        private Color _color;
        private int _columnSpan;
        private int _rowSpan;

        // --- merge state ---
        [NonSerialized]
        private bool _isMerged;

        [NonSerialized]
        private Cell<T> _merging;

        //private RuledLine _leftRuledLine;
        //private RuledLine _rightRuledLine;
        //private RuledLine _topRuledLine;
        //private RuledLine _bottomRuledLine;

        // ========================================
        // constructor
        // ========================================
        public Cell(TableData<T> owner) {
            _owner = owner;
            _columnSpan = 1;
            _rowSpan = 1;

            _isMerged = false;
            _merging = null;
        }

        // ========================================
        // property
        // ========================================
        public T Value {
            get { return _value; }
            set {
                _value = value;
                _owner.OnCellChanged(this);
            }
        }

        public Color Color {
            get { return _color; }
            set {
                _color = value;
                _owner.OnCellChanged(this);
            }
        }

        public int ColumnSpan {
            get { return _columnSpan; }
            internal set {
                Contract.Requires(value > 0);
                _columnSpan = value;
                _owner.OnCellChanged(this);
            }
        }

        public int RowSpan {
            get { return _rowSpan; }
            internal set {
                Contract.Requires(value > 0);
                _rowSpan = value;
                _owner.OnCellChanged(this);
            }
        }


        // --- merge state ---
        public bool IsMerging {
            get { return _columnSpan != 1 || _rowSpan != 1; }
        }

        public bool IsMerged {
            get { return _isMerged; }
            internal set { _isMerged = value; }
        }

        public Cell<T> Merging {
            get { return _merging; }
            internal set { _merging = value; }
        }


        //public RuledLine LeftRuledLine {
        //    get { return _leftRuledLine; }
        //    set { _leftRuledLine = value; }
        //}

        //public RuledLine RightRuledLine {
        //    get { return _rightRuledLine; }
        //    set { _rightRuledLine = value; }
        //}

        //public RuledLine TopRuledLine {
        //    get { return _topRuledLine; }
        //    set { _topRuledLine = value; }
        //}

        //public RuledLine BottomRuledLine {
        //    get { return _bottomRuledLine; }
        //    set { _bottomRuledLine = value; }
        //}

        // ========================================
        // method
        // ========================================

    }
}
