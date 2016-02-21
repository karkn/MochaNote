/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Mkamo.Common.Text {
    public interface ITextBuffer {
        // ========================================
        // event
        // ========================================
        event EventHandler<TextModifiedEventArgs> TextModified;
        event EventHandler<EventArgs> BufferDirtyChanged;
        event EventHandler<LoadRequestedEventArgs> LoadRequested;
        event EventHandler<SaveRequestedEventArgs> SaveRequested;

        // ========================================
        // property
        // ========================================
        char this[int index] { get; }
        string Text{ get; set; }
        int Length { get; }
        // 各Lineは改行文字を含まない
        string[] Lines { get; set; }
        int LineCount { get; }
        bool IsBufferDirty { get; }

        // ========================================
        // method
        // ========================================
        // --- load, save ---
        bool Load();
        bool Save();

        // --- read ---
        /// <summary>
        /// index番目の文字が改行文字かどうかを返す．
        /// </summary>
        bool IsEOL(int index);

        /// <summary>
        /// index番目の文字が文書終端文字かどうかを返す．
        /// </summary>
        bool IsEOT(int index);

        /// <summary>
        /// index番目の文字の行インデックスを返す．
        /// </summary>
        int GetLineIndexOf(int index);

        /// <summary>
        /// index番目の文字の列インデックスを返す．
        /// </summary>
        int GetColumnIndexOf(int index);

        /// <summary>
        /// lineIndexで指定された行の最初の文字のインデックスを返す．
        /// </summary>
        int GetLineStartCharIndexOf(int lineIndex);

        /// <summary>
        /// lineIndexで指定された行の最後の改行文字のインデックスを返す．
        /// 最後の行のindexが指定されたときはEOTのインデックス(= Text.Length)を返す
        /// </summary>
        int GetLineEndCharIndexOf(int lineIndex);

        /// <summary>
        /// lineIndex行，columnIndex列の文字のインデックスを返す．
        /// </summary>
        int GetCharIndexOf(int lineIndex, int columnIndex);

        /// <summary>
        /// lineIndex行に含まれる文字数を返す．
        /// 文字数にはEOLやEOTはカウントされない．
        /// </summary>
        int GetColumnCount(int lineIndex);

        // --- write ---
        void Append(string s);
        void AppendFormat(string format, params Object[] args);
        void AppendLine(string s);
        void Insert(int charIndex, string s);
        void Remove(int startIndex, int length);
        bool Move(int sourceStartIndex, int sourceLength, int targetIndex);
        void Replace(char oldChar, char newChar);
        void Replace(string oldString, string newString);
        void Replace(char oldChar, char newChar, int startIndex, int count);
        void Replace(string oldString, string newString, int startIndex, int count);
    }

    public class LoadRequestedEventArgs: EventArgs {
        private bool _canceled;
        public LoadRequestedEventArgs() {
            _canceled = false;
        }
        public bool Canceled {
            get { return _canceled; }
            set { _canceled = value; }
        }
    }

    public class SaveRequestedEventArgs: EventArgs {
        private bool _canceled;
        public SaveRequestedEventArgs() {
            _canceled = false;
        }
        public bool Canceled {
            get { return _canceled; }
            set { _canceled = value; }
        }
    }

    [Serializable]
    public enum ModificationKind {
        Insert,
        Remove,
        Significant,
    }

    public class TextModifiedEventArgs: EventArgs {
        private ModificationKind _kind;
        private string _modification;
        private int _offset;
        private int _length;

        public TextModifiedEventArgs() {
            _kind = ModificationKind.Significant;
        }

        public TextModifiedEventArgs(
            ModificationKind kind,
            string modification,
            int offset,
            int length
        ) {
            _kind = kind;
            _modification = modification;
            _offset = offset;
            _length = length;
        }

        public ModificationKind Kind {
            get { return _kind; }
        }
        public string Modification {
            get { return _modification; }
        }
        public int Offset {
            get { return _offset; }
        }
        public int Length {
            get { return _length; }
        }
    }

}
