/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Figure.Core;
using System.Windows.Forms;
using Mkamo.Common.Diagnostics;
using Mkamo.Editor.Internal.Core;
using Mkamo.Common.Forms.KeyMap;

namespace Mkamo.Editor.Handles {
    public class DefaultEditorHandle: AbstractHandle, IEditorHandle {
        // ========================================
        // field
        // ========================================
        private IKeyMap<IEditor> _keyMap;

        // ========================================
        // constructor
        // ========================================
        public DefaultEditorHandle() {
        }

        // ========================================
        // event
        // ========================================
        public event EventHandler<ShortcutKeyProcessEventArgs> ShortcutKeyProcess;
        public event EventHandler<KeyEventArgs> KeyDown;
        public event EventHandler<KeyEventArgs> KeyUp;
        public event EventHandler<KeyPressEventArgs> KeyPress;
        public event EventHandler<PreviewKeyDownEventArgs> PreviewKeyDown;


        // ========================================
        // property
        // ========================================
        public IKeyMap<IEditor> KeyMap {
            get { return _keyMap; }
            set { _keyMap = value; }
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected override IFigure _Figure {
            get { return Host.Figure; }
        }

        // ========================================
        // method
        // ========================================
        public override void Install(IEditor host) {
            base.Install(host);

            if (_Figure != null) {
                _Figure.ShortcutKeyProcess += HandleFigureShortcutKeyProcess;
                _Figure.KeyDown += HandleFigureKeyDown;
                _Figure.KeyUp += HandleFigureKeyUp;
                _Figure.KeyPress += HandleFigureKeyPress;
                _Figure.PreviewKeyDown += HandleFigurePreviewKeyDown;
            }
        }

        public override void Uninstall(IEditor host) {
            if (_Figure != null) {
                _Figure.ShortcutKeyProcess -= HandleFigureShortcutKeyProcess;
                _Figure.KeyDown -= HandleFigureKeyDown;
                _Figure.KeyUp -= HandleFigureKeyUp;
                _Figure.KeyPress -= HandleFigureKeyPress;
                _Figure.PreviewKeyDown -= HandleFigurePreviewKeyDown;
            }

            base.Uninstall(host);
        }


        // ------------------------------
        // protected
        // ------------------------------
        protected virtual void OnFigureShortcutKeyProcess(ShortcutKeyProcessEventArgs e) {
            var keyData = e.KeyData;
            if (keyData == Keys.ProcessKey && !Host.Site.EditorCanvas.IsInImeComposition) {
                keyData = Host.Site.EditorCanvas._ImmVirtualKey;
            }

            if (_keyMap != null && _keyMap.IsDefined(keyData)) {
                var action = _keyMap.GetAction(keyData);
                if (action != null) {
                    if (action(Host)) {
                        e.Handled = true;
                    }
                }
            }

            var handler = ShortcutKeyProcess;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void OnFigureKeyDown(KeyEventArgs e) {
            var handler = KeyDown;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void OnFigureKeyUp(KeyEventArgs e) {
            var handler = KeyUp;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void OnFigureKeyPress(KeyPressEventArgs e) {
            var handler = KeyPress;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void OnFigurePreviewKeyDown(PreviewKeyDownEventArgs e) {
            var handler = PreviewKeyDown;
            if (handler != null) {
                handler(this, e);
            }
        }

        // ------------------------------
        // private
        // ------------------------------
        private void HandleFigureShortcutKeyProcess(object sender, ShortcutKeyProcessEventArgs e) {
            OnFigureShortcutKeyProcess(e);
        }

        private void HandleFigureKeyDown(object sender, KeyEventArgs e) {
            OnFigureKeyDown(e);
        }

        private void HandleFigureKeyUp(object sender, KeyEventArgs e) {
            OnFigureKeyUp(e);
        }

        private void HandleFigureKeyPress(object sender, KeyPressEventArgs e) {
            OnFigureKeyPress(e);
        }

        private void HandleFigurePreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
            OnFigurePreviewKeyDown(e);
        }

    }
}
