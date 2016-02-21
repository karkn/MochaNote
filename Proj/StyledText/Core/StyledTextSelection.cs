/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.DataType;

namespace Mkamo.StyledText.Core {
    public class StyledTextSelection {
        // ========================================
        // field
        // ========================================
        private StyledTextReferer _owner;
        private Range _range = Range.Empty;

        // ========================================
        // constructor
        // ========================================
        public StyledTextSelection(StyledTextReferer owner) {
            _owner = owner;
        }

        // ========================================
        // event
        // ========================================
        public event EventHandler<EventArgs> SelectionChanged;

        // ========================================
        // property
        // ========================================
        public bool IsEmpty {
            get { return Length == 0; }
        }

        public int Offset {
            get { return _range.Offset; }
            set { Range = new Range(value, Length); }
        }

        public int Length {
            get { return _range.Length; }
            set { Range = new Range(Offset, value); }
        }

        public Range Range {
            get { return _range; }
            set {
                if (value == _range) {
                    return;
                }
                if (value.IsEmpty) {
                    _range = Range.Empty;
                } else {
                    if (value.End == _owner.Target.Length - 1) {
                        _range = new Range(value.Offset, value.Length - 1);
                    } else {
                        _range = value;
                    }
                }
                OnSelectionChanged();
            }
        }

        // ========================================
        // method
        // ========================================
        public void Clear() {
            Range = Range.Empty;
        }

        // --- protected ---
        protected void OnSelectionChanged() {
            var handler = SelectionChanged;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
