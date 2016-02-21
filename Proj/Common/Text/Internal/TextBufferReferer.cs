/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Mkamo.Common.Text.Internal {
    internal class TextBufferReferer: ITextBufferReferer {
        // ========================================
        // field
        // ========================================
        private TextBuffer _target;

        private int _caretPosition;
        private int? _mark;
        private TextSelection _textSelection;

        // ========================================
        // constructor
        // ========================================
        public TextBufferReferer(TextBuffer target) {
            _caretPosition = 0;
            _mark = null;
            _textSelection = new TextSelection(this);

            if (target == null) {
                _target = TextUtil.CreateTextBuffer() as TextBuffer;
            } else {
                _target = target;
            }
        }

        // ========================================
        // event
        // ========================================
        public event EventHandler<CaretMovedEventArgs> CaretMoved;
        public event EventHandler<EventArgs> MarkSet;

        // ========================================
        // property
        // ========================================
        public ITextBuffer Buffer {
            get { return _target; }
            set { _target = value as TextBuffer; }
        }

        public int CaretPosition {
            get { return _caretPosition; }
            set {
                int old = _caretPosition;
                _caretPosition = value;
                FireCaretMoved(new CaretMovedEventArgs(old, value));
            }
        }

        public int? Mark {
            get { return _mark; }
            set {
                _mark = value;
                FireMarkSet();
            }
        }

        public bool IsMarkSet {
            get { return _mark != null; }
        }

        public ITextSelection TextSelection {
            get { return _textSelection; }
        }

        // ========================================
        // method
        // ========================================
        public void MoveForwardChar() {
            if (CaretPosition < Buffer.Text.Length) {
                ClearSelection();
                ++CaretPosition;
            }
        }

        public void MoveBackwardChar() {
            if (CaretPosition > 0) {
                ClearSelection();
                --CaretPosition;
            }
        }

        public void MovePreviousLine() {
            int curLineIndex = Buffer.GetLineIndexOf(CaretPosition);
            if (curLineIndex > 0) {
                ClearSelection();
                int columnIndex = Buffer.GetColumnIndexOf(CaretPosition);
                int prevLineColumnCount = Buffer.GetColumnCount(curLineIndex - 1);
                columnIndex = columnIndex < prevLineColumnCount? columnIndex: prevLineColumnCount;
                CaretPosition = Buffer.GetCharIndexOf(curLineIndex - 1, columnIndex);
            }
        }

        public void MoveNextLine() {
            int curLineIndex = Buffer.GetLineIndexOf(CaretPosition);
            int lineCount = Buffer.Lines.Length;
            if (curLineIndex < lineCount - 1) {
                ClearSelection();
                int columnIndex = Buffer.GetColumnIndexOf(CaretPosition);
                int nextLineColumnCount = Buffer.GetColumnCount(curLineIndex + 1);
                columnIndex = columnIndex < nextLineColumnCount? columnIndex: nextLineColumnCount;
                CaretPosition = Buffer.GetCharIndexOf(curLineIndex + 1, columnIndex);
            }
        }

        public void MoveBeginningOfText() {
            ClearSelection();
            CaretPosition = 0;
        }

        public void MoveEndOfText() {
            ClearSelection();
            CaretPosition = Buffer.Text.Length;
        }


        public void Insert(string s) {
            if (TextSelection.IsEmpty) {
                Buffer.Insert(CaretPosition, s);
                CaretPosition += s.Length;
            } else {
                Buffer.Remove(TextSelection.Offset, TextSelection.Length);
                CaretPosition = TextSelection.Offset;
                ClearSelection();
                Buffer.Insert(CaretPosition, s);
                CaretPosition += s.Length;
            }
        }

        public void InsertAndSelect(string s) {
            ClearSelection();
            Buffer.Insert(CaretPosition, s);
            TextSelection.Offset = CaretPosition;
            TextSelection.Length = s.Length;
        }

        public void RemoveForward() {
            if (TextSelection.IsEmpty) {
                if (CaretPosition < Buffer.Text.Length) {
                    Buffer.Remove(CaretPosition, 1);
                }
            } else {
                Buffer.Remove(TextSelection.Offset, TextSelection.Length);
                CaretPosition = TextSelection.Offset;
                ClearSelection();
            }
        }

        public void RemoveBackward() {
            if (TextSelection.IsEmpty) {
                if (CaretPosition > 0) {
                    Buffer.Remove(CaretPosition - 1, 1);
                    --CaretPosition;
                }
            } else {
                Buffer.Remove(TextSelection.Offset, TextSelection.Length);
                CaretPosition = TextSelection.Offset;
                ClearSelection();
            }
        }

        public void YankSelectionContent(int charIndex) {
            if (!TextSelection.IsEmpty) {
                if (Buffer.Move(TextSelection.Offset, TextSelection.Length, charIndex)) {
                    if (charIndex < TextSelection.Offset) {
                        TextSelection.Offset = charIndex;
                        CaretPosition = TextSelection.Offset;
                    } else {
                        TextSelection.Offset = charIndex - TextSelection.Length;
                        CaretPosition = TextSelection.Offset;
                    }
                }
            }
        }

        public void SetMark() {
            Mark = CaretPosition;
        }

        public void SelectRegion() {
            if (IsMarkSet) {
                TextSelection.SetSelection(Math.Min(Mark.Value, CaretPosition), Math.Abs(Mark.Value - CaretPosition));
            }
        }

        public void ClearSelection() {
            TextSelection.Clear();
        }


        // --- protected internal ---
        protected internal void FireCaretMoved(CaretMovedEventArgs e) {
            EventHandler<CaretMovedEventArgs> tmp = CaretMoved;
            if (tmp != null) {
                CaretMoved(this, e);
            }
        }

        protected internal void FireMarkSet() {
            EventHandler<EventArgs> tmp = MarkSet;
            if (tmp != null) {
                MarkSet(this, EventArgs.Empty);
            }
        }
    }

}
