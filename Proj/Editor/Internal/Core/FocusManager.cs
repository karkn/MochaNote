/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Editor.Internal.Editors;

namespace Mkamo.Editor.Internal.Core {
    internal class FocusManager: IFocusManager {
        // ========================================
        // field
        // ========================================
        private readonly RootEditor _root;

        private IEditor _focusedEditor;

        // ========================================
        // constructor
        // ========================================
        public FocusManager(RootEditor root) {
            _root = root;
        }

        // ========================================
        // property
        // ========================================
        public bool IsEditorFocused {
            get { return _focusedEditor != null; }
        }

        public IEditor FocusedEditor {
            get { return _focusedEditor; }
        }

        public IFocus Focus {
            get { return _focusedEditor == null? null: _focusedEditor.Focus; }
        }

        // ========================================
        // event
        // ========================================
        public event EventHandler<FocusChangedEventArgs> FocusChanged;

        // ========================================
        // method
        // ========================================
        /// <summary>
        /// 古いFocusを解除してから新しいFocusを設定してLayerをアップデートする．
        /// </summary>
        public void PerformFocus(IEditor focused) {
            if (focused == null || focused == _focusedEditor) {
                return;
            }

            var oldFocusedEditor = _focusedEditor;

            if (_focusedEditor != null) {
                _focusedEditor.IsFocused = false;
            }

            _focusedEditor = (focused.Focus == null || focused.Focus.Figure == null)? null: focused;
            _root.Site.UpdateFocusLayer();
            _root.Site.UpdateHandleLayer();

            OnFocusChanged(oldFocusedEditor, _focusedEditor);
        }

        public void ClearFocus() {
            var oldFocusedEditor = _focusedEditor;

            if (_focusedEditor != null) {
                _focusedEditor.IsFocused = false;
            }

            _focusedEditor = null;
            _root.Site.UpdateFocusLayer();
            _root.Site.UpdateHandleLayer();

            OnFocusChanged(oldFocusedEditor, null);
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected void OnFocusChanged(IEditor oldFocusedEditor, IEditor newFocusedEditor) {
            var tmp = FocusChanged;
            if (tmp != null) {
                tmp(this, new FocusChangedEventArgs(oldFocusedEditor, newFocusedEditor));
            }
        }
    }

}
