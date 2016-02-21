/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Mkamo.Common.Text {
    // Move: カーソル移動
    // Insert: 文字列挿入
    // Remove: 文字列削除
    // Yank: 文字列移動
    // Paste: copy bufから貼り付け
    // Cut: copy bufに切り取り
    // Copy: copy bufにコピー
    // 
    // Region: MarkとCaretの間
    // Selection: 選択範囲
    public interface ITextBufferReferer {
        // ========================================
        // event
        // ========================================
        event EventHandler<CaretMovedEventArgs> CaretMoved;
        event EventHandler<EventArgs> MarkSet;

        // ========================================
        // property
        // ========================================
        ITextBuffer Buffer { get; set; }
        int CaretPosition { get; set; }
        int? Mark { get; set; }
        bool IsMarkSet { get; }
        ITextSelection TextSelection { get; }

        // ========================================
        // method
        // ========================================
        void MoveForwardChar();
        void MoveBackwardChar();
        void MovePreviousLine();
        void MoveNextLine();
        void MoveBeginningOfText();
        void MoveEndOfText();

        void Insert(string s);
        void InsertAndSelect(string s);

        void RemoveForward();
        void RemoveBackward();

        void YankSelectionContent(int charIndex);

        void SetMark();
        void SelectRegion();
        void ClearSelection();

    }


    public class CaretMovedEventArgs: EventArgs {
        private int _oldPosition;
        private int _newPosition;
        public CaretMovedEventArgs(int oldPosition, int newPosition) {
            _oldPosition = oldPosition;
            _newPosition = newPosition;
        }
        public int OldPosition {
            get { return _oldPosition; }
        }
        public int NewPosition {
            get { return _newPosition; }
        }
    }
}
