/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Mkamo.Common.Text.Internal {
    /// <summary>
    /// 内部表現では改行はすべて'EOL == \n'で扱う．
    /// 外部から見るとcharIndex == Text.LengthにEOT文字があるものとしてインデックスを扱う．
    /// 内部的にはEOTは保持されていない．
    /// </summary>
    [Serializable]
    internal class TextBuffer: ITextBuffer {
        // ========================================
        // static field
        // ========================================
        private static readonly char EOL = '\n';
        private static readonly char EOT = '\0';

        // ========================================
        // field
        // ========================================
        StringBuilder _buffer;
        bool _isBufferDirty;

        [NonSerialized]
        string[] _linesCache;
        [NonSerialized]
        bool _isLinesCacheDirty;

        // ========================================
        // constructor
        // ========================================
        public TextBuffer(string text) {
            _buffer = (text == null)? new StringBuilder(): new StringBuilder(SanitizeLineEnd(text));
            _isBufferDirty = false;
            _linesCache = null;
            _isLinesCacheDirty = true;
        }

        [OnDeserialized]
        protected void OnDeserialized(StreamingContext context) {
            _linesCache = null;
            _isLinesCacheDirty = true;
        }

        // ========================================
        // event
        // ========================================
        [field:NonSerialized] 
        public event EventHandler<TextModifiedEventArgs> TextModified;
        [field:NonSerialized] 
        public event EventHandler<EventArgs> BufferDirtyChanged;
        [field:NonSerialized] 
        public event EventHandler<LoadRequestedEventArgs> LoadRequested;
        [field:NonSerialized] 
        public event EventHandler<SaveRequestedEventArgs> SaveRequested;

        // ========================================
        // property
        // ========================================
        public char this[int index] {
            get { return _buffer[index]; }
        }

        public string Text{
            get { return _buffer.ToString(); }
            set {
                _buffer = new StringBuilder(SanitizeLineEnd(value));
                IsBufferDirty = true;
                _isLinesCacheDirty = true;
                FireTextModified(new TextModifiedEventArgs());
            }
        }

        public int Length {
            get { return _buffer.Length; }
        }

        /// <summary>
        /// EOTを付加するので_bufferが空でもLength==1の配列を返す．
        /// </summary>
        public string[] Lines {
            get {
                if (_isLinesCacheDirty) {
                    _linesCache = null;
                    List<string> lines = new List<string>();

                    string eotTerminatedText = Text + EOT;
                    int lineIndex = 0;
                    int columnIndex = 0;
                    int lineStartIndex = 0;
                    for (int i = 0; i < eotTerminatedText.Length; ++i) {
                        if (columnIndex == 0) {
                            lineStartIndex = i;
                        }

                        if (eotTerminatedText[i] == EOL || eotTerminatedText[i] == EOT) {
                            lines.Add(eotTerminatedText.Substring(lineStartIndex, i - lineStartIndex));
                            ++lineIndex;
                            columnIndex = 0;
                        } else {
                            ++columnIndex;
                        }
                    }

                    _linesCache = lines.ToArray();
                    _isLinesCacheDirty = false;
                }

                return _linesCache;
            }

            set {
                StringBuilder buf = new StringBuilder();
                for (int i = 0; i < value.Length - 1; ++i) {
                    buf.Append(value[i]);
                    buf.Append(EOL);
                }
                if (value.Length > 0) {
                    buf.Append(value[value.Length - 1]);
                }
                _buffer = buf;
                IsBufferDirty = true;
                _linesCache = value;
                _isLinesCacheDirty = false;
                FireTextModified(new TextModifiedEventArgs());
            }
        }

        public int LineCount {
            get { return Lines.Length; }
        }

        public bool IsBufferDirty {
            get { return _isBufferDirty; }
            set {
                if (_isBufferDirty != value) {
                    _isBufferDirty = value;
                    FireBufferDirtyChanged();
                }
            }
        }

        // ========================================
        // method
        // ========================================
        // === object ==========
        public override string ToString() {
            return Text;
        }

        // --- load, save ---
        public bool Load() {
            LoadRequestedEventArgs e = new LoadRequestedEventArgs();
            FireLoadRequested(e);
            if (!e.Canceled) {
                IsBufferDirty = false;
            }
            return !e.Canceled;
        }

        public bool Save() {
            SaveRequestedEventArgs e = new SaveRequestedEventArgs();
            FireSaveRequested(e);
            if (!e.Canceled) {
                IsBufferDirty = false;
            }
            return !e.Canceled;
        }

        // --- read ---
        public bool IsEOL(int index) {
            if (index < 0 || index > _buffer.Length) {
                throw new ArgumentOutOfRangeException("charIndex");
            }
            if (index == _buffer.Length) {
                return false;
            }
            return _buffer[index] == EOL;
        }

        public bool IsEOT(int index) {
            if (index < 0 || index > _buffer.Length) {
                throw new ArgumentOutOfRangeException("charIndex");
            }
            return index == _buffer.Length;
        }

        public int GetLineIndexOf(int index) {
            if (index < 0) {
                throw new ArgumentOutOfRangeException("charIndex");
            }

            int lineStartIndex = 0;
            int lineIndex;
            for (lineIndex = 0; lineIndex < Lines.Length; ++lineIndex) {
                string line = Lines[lineIndex];
                if (index < lineStartIndex + line.Length + 1) {
                    return lineIndex;
                }
                lineStartIndex += line.Length + 1;
            }

            throw new ArgumentOutOfRangeException("charIndex");
        }

        public int GetColumnIndexOf(int index) {
            if (index < 0) {
                throw new ArgumentOutOfRangeException("charIndex");
            }

            int lineStartIndex = 0;
            int lineIndex;
            for (lineIndex = 0; lineIndex < Lines.Length; ++lineIndex) {
                string line = Lines[lineIndex];
                if (index < lineStartIndex + line.Length + 1) {
                    return index - lineStartIndex;
                }
                lineStartIndex += line.Length + 1;
            }

            throw new ArgumentOutOfRangeException("charIndex");
        }

        public int GetLineStartCharIndexOf(int lineIndex) {
            if (lineIndex < 0 || lineIndex > Lines.Length - 1) {
                throw new ArgumentOutOfRangeException("lineIndex");
            }

            int charIndex = 0;
            for (int i = 0; i < lineIndex; ++i) {
                charIndex += Lines[i].Length + 1;
            }
            return charIndex;
        }

        // lineIndexで指定された行の最後の改行文字のインデックスを返す．
        public int GetLineEndCharIndexOf(int lineIndex) {
            if (lineIndex < 0 || lineIndex > Lines.Length - 1) {
                throw new ArgumentOutOfRangeException("lineIndex");
            }

            int charIndex = 0;
            for (int i = 0; i < lineIndex + 1; ++i) {
                charIndex += Lines[i].Length + 1;
            }
            return charIndex - 1;
        }

        public int GetCharIndexOf(int lineIndex, int columnIndex) {
            if (lineIndex < 0 || lineIndex > Lines.Length - 1) {
                throw new ArgumentOutOfRangeException("lineIndex");
            }

            string line = Lines[lineIndex];
            if (columnIndex < 0 || columnIndex > line.Length) {
                throw new ArgumentOutOfRangeException("columnIndex");
            }

            return GetLineStartCharIndexOf(lineIndex) + columnIndex;
        }

        public int GetColumnCount(int lineIndex) {
            if (lineIndex < 0 || lineIndex > Lines.Length) {
                throw new ArgumentOutOfRangeException("lineIndex");
            }
            return Lines[lineIndex].Length;
        }


        // --- write ---
        public void Append(string s) {
            int offset = _buffer.Length - 1;
            _buffer.Append(SanitizeLineEnd(s));
            IsBufferDirty = true;
            _isLinesCacheDirty = true;
            FireTextModified(new TextModifiedEventArgs(ModificationKind.Insert, s, offset, s.Length));
        }

        public void AppendFormat(string format, params Object[] args) {
            string appended = string.Format(format, args);
            appended = SanitizeLineEnd(appended);
            Append(appended);
        }
        
        public void AppendLine(string s) {
            Append(SanitizeLineEnd(s) + EOL);
        }

        public void Insert(int charIndex, string s) {
            _buffer.Insert(charIndex, SanitizeLineEnd(s));
            IsBufferDirty = true;
            _isLinesCacheDirty = true;
            FireTextModified(new TextModifiedEventArgs(ModificationKind.Insert, s, charIndex, s.Length));
        }

        public void Remove(int startIndex, int length) {
            string modification = _buffer.ToString().Substring(startIndex, length);
            _buffer.Remove(startIndex, length);
            IsBufferDirty = true;
            _isLinesCacheDirty = true;
            FireTextModified(new TextModifiedEventArgs(ModificationKind.Remove, modification, startIndex, length));
        }

        public bool Move(int sourceStartIndex, int sourceLength, int targetIndex) {
            if (sourceStartIndex <= targetIndex && targetIndex < sourceStartIndex + sourceLength) {
                return false;
            }

            string s = _buffer.ToString().Substring(sourceStartIndex, sourceLength);
            if (sourceStartIndex < targetIndex) {
                _buffer.Insert(targetIndex, s);
                _buffer.Remove(sourceStartIndex, sourceLength);
            } else {
                _buffer.Remove(sourceStartIndex, sourceLength);
                _buffer.Insert(targetIndex, s);
            }

            IsBufferDirty = true;
            _isLinesCacheDirty = true;
            FireTextModified(new TextModifiedEventArgs());
            return true;
        }

        public void Replace(char oldChar, char newChar) {
            _buffer.Replace(oldChar, newChar);
            IsBufferDirty = true;
            _isLinesCacheDirty = true;
            FireTextModified(new TextModifiedEventArgs());
        }

        public void Replace(string oldString, string newString) {
            _buffer.Replace(oldString, SanitizeLineEnd(newString));
            IsBufferDirty = true;
            _isLinesCacheDirty = true;
            FireTextModified(new TextModifiedEventArgs());
        }

        public void Replace(char oldChar, char newChar, int startIndex, int count) {
            _buffer.Replace(oldChar, newChar, startIndex, count);
            IsBufferDirty = true;
            _isLinesCacheDirty = true;
            FireTextModified(new TextModifiedEventArgs());
        }

        public void Replace(string oldString, string newString, int startIndex, int count) {
            _buffer.Replace(oldString, SanitizeLineEnd(newString), startIndex, count);
            IsBufferDirty = true;
            _isLinesCacheDirty = true;
            FireTextModified(new TextModifiedEventArgs());
        }


        // --- protected internal ---
        protected internal void FireTextModified(TextModifiedEventArgs e) {
            EventHandler<TextModifiedEventArgs> tmp = TextModified;
            if (tmp != null) {
                tmp(this, e);
            }
        }

        protected internal void FireBufferDirtyChanged() {
            EventHandler<EventArgs> tmp = BufferDirtyChanged;
            if (tmp != null) {
                tmp(this, EventArgs.Empty);
            }
        }

        protected internal void FireLoadRequested(LoadRequestedEventArgs e) {
            EventHandler<LoadRequestedEventArgs> tmp = LoadRequested;
            if (tmp != null) {
                tmp(this, e);
            }
        }

        protected internal void FireSaveRequested(SaveRequestedEventArgs e) {
            EventHandler<SaveRequestedEventArgs> tmp = SaveRequested;
            if (tmp != null) {
                tmp(this, e);
            }
        }

        // --- internal ---
        internal string SanitizeLineEnd(string text) {
            StringBuilder ret = new StringBuilder();
            for (int i = 0; i < text.Length; ++i) {
                LineEndKinds kind = GetLineEndKind(text, i);
                /// crlf: crを無視，cr: lfをappend，lf: そのままappend，その他: そのままappend
                if ((kind & LineEndKinds.FirstOfDoubleByte) == LineEndKinds.FirstOfDoubleByte) {
                    /// do nothing
                } else if (((kind & LineEndKinds.CarrigeReturnOfSingleByte) == LineEndKinds.CarrigeReturnOfSingleByte)) {
                    ret.Append('\n');
                } else {
                    ret.Append(text[i]);
                }
            }
            return ret.ToString();
        }

    
        [Flags, Serializable]
        public enum LineEndKinds {
            NotLineEnd = 0x0000,

            CarrigeReturn = 0x0001,
            LineFeed = 0x0002,

            SingleByte = 0x0010,
            DoubleByte = 0x0020,

            CarrigeReturnOfSingleByte = CarrigeReturn | SingleByte,
            LineFeedOfSingleByte = LineFeed | SingleByte,
            FirstOfDoubleByte = CarrigeReturn | DoubleByte,
            SecondOfDoubleByte = LineFeed | DoubleByte,
        }

        internal LineEndKinds GetLineEndKind(string text, int index) {
            if (index < 0 | index > text.Length - 1) {
                throw new ArgumentOutOfRangeException("charIndex");
            }

            switch (text[index]) {
                case '\r': {
                    if (index == text.Length - 1) {
                        return LineEndKinds.CarrigeReturn | LineEndKinds.SingleByte;
                    } else if (text[index + 1] == '\n') {
                        return LineEndKinds.CarrigeReturn | LineEndKinds.DoubleByte;
                    } else {
                        return LineEndKinds.CarrigeReturn | LineEndKinds.SingleByte;
                    }
                }
                case '\n': {
                    if (index == 0) {
                        return LineEndKinds.LineFeed | LineEndKinds.SingleByte;
                    } else if (text[index - 1] == '\r') {
                        return LineEndKinds.LineFeed | LineEndKinds.DoubleByte;
                    } else {
                        return LineEndKinds.LineFeed | LineEndKinds.SingleByte;
                    }
                }
                default: {
                    return LineEndKinds.NotLineEnd;
                }
            }
        }

    }
}
