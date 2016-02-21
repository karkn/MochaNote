/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.StyledText.Core {
    /// <summary>
    /// StyledTextRefererのCaretが移動したときのイベント．
    /// </summary>
    [Serializable]
    public class CaretMovedEventArgs: EventArgs {
        private int _oldIndex;
        private int _newIndex;

        public CaretMovedEventArgs(int oldIndex, int newIndex) {
            _oldIndex = oldIndex;
            _newIndex = newIndex;
        }
        public int OldIndex {
            get { return _oldIndex; }
        }
        public int NewIndex {
            get { return _newIndex; }
        }
    }

    /// <summary>
    /// StyledText更新イベント．
    /// </summary>
    [Serializable]
    public class StyledTextModifiedEventArgs: EventArgs {
        private StyledTextModificationKind _kind;
        private int _offset;
        private int _length;

        public StyledTextModifiedEventArgs(
            StyledTextModificationKind kind,
            int offset,
            int length
        ) {
            _kind = kind;
            _offset = offset;
            _length = length;
        }

        public StyledTextModificationKind Kind {
            get { return _kind; }
        }
        public int Offset {
            get { return _offset; }
        }
        public int Length {
            get { return _length; }
        }
    }

    /// <summary>
    /// Flow更新イベント．
    /// 文字列だけでなくFontやColorなどのすべての変更を含む．
    /// </summary>
    [Serializable]
    public class ContentsChangedEventArgs: EventArgs {
        // ========================================
        // field
        // ========================================
        private Flow _changedContent;

        // ========================================
        // constructor
        // ========================================
        public ContentsChangedEventArgs(Flow changedContent): base() {
            _changedContent = changedContent;
        }

        // ========================================
        // property
        // ========================================
        public Flow ChangedContent {
            get { return _changedContent; }
        }

    }
}
