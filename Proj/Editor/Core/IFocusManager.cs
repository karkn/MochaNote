/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Editor.Core {
    public interface IFocusManager {
        // ========================================
        // property
        // ========================================
        bool IsEditorFocused { get; }
        IEditor FocusedEditor { get; }
        IFocus Focus { get; }

        // ========================================
        // event
        // ========================================
        event EventHandler<FocusChangedEventArgs> FocusChanged;

        // ========================================
        // method
        // ========================================
        void PerformFocus(IEditor focused);
        void ClearFocus();
    }

    // ========================================
    // class
    // ========================================
    public class FocusChangedEventArgs: EventArgs {
        private IEditor _oldFocusedEditor;
        private IEditor _newFocusedEditor;

        public FocusChangedEventArgs(IEditor oldFocusedEditor, IEditor newFocusedEditor) {
            _oldFocusedEditor = oldFocusedEditor;
            _newFocusedEditor = newFocusedEditor;
        }

        public IEditor OldFocusedEditor {
            get { return _oldFocusedEditor; }
        }

        public IEditor NewFocusedEditor {
            get { return _newFocusedEditor; }
        }
    }
}
