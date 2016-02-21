/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Mkamo.Common.Text.Internal {
    internal class TextSelection: ITextSelection {
        // ========================================
        // field
        // ========================================
        private TextBufferReferer _owner;
        private int _offset;
        private int _length;

        // ========================================
        // constructor
        // ========================================
        public TextSelection(TextBufferReferer owner) {
            _owner = owner;
            _offset = 0;
            _length = 0;
        }

        // ========================================
        // event
        // ========================================
        public event EventHandler<EventArgs> SelectionChanged;

        // ========================================
        // property
        // ========================================
        public bool IsEmpty {
            get { return _length == 0; }
        }

        public int Offset {
            get { return _offset; }
            set {
                _offset = value;
                FireSelectionChanged();
            }
        }

        public int Length {
            get { return _length; }
            set {
                _length = value;
                FireSelectionChanged();
            }
        }

        public string Text {
            get { return _owner.Buffer.Text.Substring(Offset, Length); }
        }

        // ========================================
        // method
        // ========================================
        public void Clear() {
            SetSelection(0, 0);
        }

        public void SetSelection(int offset, int length) {
            _offset = offset;
            _length = length;
            FireSelectionChanged();
        }


        // --- protected internal ---
        protected internal void FireSelectionChanged() {
            EventHandler<EventArgs> tmp = SelectionChanged;
            if (tmp != null) {
                tmp(this, EventArgs.Empty);
            }
        }
    }
}
