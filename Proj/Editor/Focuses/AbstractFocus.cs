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
using Mkamo.Editor.Internal.Core;
using System.Drawing;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Common.Core;
using Mkamo.Common.Win32.Imm32;
using Mkamo.Common.Forms.MouseOperatable;
using Mkamo.Common.Forms.KeyMap;

namespace Mkamo.Editor.Focuses {
    public abstract class AbstractFocus: IFocus {
        // ========================================
        // field
        // ========================================
        private IEditor _host;
        private IKeyMap<IFocus> _keyMap;

        private Action<IFocus, INode, IFigure> _relocator;

        // ========================================
        // constructor
        // ========================================
        public AbstractFocus() {
        }

        // ========================================
        // event
        // ========================================
        public event EventHandler<MouseEventArgs> MouseClick;
        public event EventHandler<MouseEventArgs> MouseDoubleClick;
        public event EventHandler<MouseEventArgs> MouseTripleClick;
        public event EventHandler<MouseEventArgs> MouseDown;
        public event EventHandler<MouseEventArgs> MouseUp;
        public event EventHandler<MouseEventArgs> MouseMove;
        public event EventHandler<EventArgs> MouseEnter;
        public event EventHandler<EventArgs> MouseLeave;
        public event EventHandler<MouseHoverEventArgs> MouseHover;
        public event EventHandler<MouseEventArgs> DragStart;
        public event EventHandler<MouseEventArgs> DragMove;
        public event EventHandler<MouseEventArgs> DragFinish;
        public event EventHandler<EventArgs> DragCancel;

        public event EventHandler<ShortcutKeyProcessEventArgs> ShortcutKeyProcess;
        public event EventHandler<KeyEventArgs> KeyDown;
        public event EventHandler<KeyEventArgs> KeyUp;
        public event EventHandler<KeyPressEventArgs> KeyPress;
        public event EventHandler<PreviewKeyDownEventArgs> PreviewKeyDown;

        // ========================================
        // property
        // ========================================
        public abstract object Value { get; set; }

        /// <summary>
        /// Installed()が呼ばれたときにgetできる必要がある．
        /// </summary>
        public abstract INode Figure { get; }

        public abstract bool IsModified { get; }

        public Action<IFocus, INode, IFigure> Relocator {
            get { return _relocator; }
            set { _relocator = value; }
        }

        public IKeyMap<IFocus> KeyMap {
            get { return _keyMap; }
            set { _keyMap = value; }
        }

        public IEditor Host {
            get { return _host; }
        }


        // ========================================
        // method
        // ========================================
        public virtual void Install(IEditor host) {
            _host = host;

            Figure.MouseClick += HandleFigureMouseClick;
            Figure.MouseDoubleClick += HandleFigureMouseDoubleClick;
            Figure.MouseTripleClick += HandleFigureMouseTripleClick;

            Figure.MouseDown += HandleFigureMouseDown;
            Figure.MouseUp += HandleFigureMouseUp;
            Figure.MouseMove += HandleFigureMouseMove;

            Figure.MouseEnter += HandleFigureMouseEnter;
            Figure.MouseLeave += HandleFigureMouseLeave;
            Figure.MouseHover += HandleFigureMouseHover;

            Figure.DragStart += HandleFigureDragStart;
            Figure.DragMove += HandleFigureDragMove;
            Figure.DragFinish += HandleFigureDragFinish;
            Figure.DragCancel += HandleFigureDragCancel;

            Figure.ShortcutKeyProcess += HandleFigureShortcutKeyProcess;
            Figure.KeyDown += HandleFigureKeyDown;
            Figure.KeyUp += HandleFigureKeyUp;
            Figure.KeyPress += HandleFigureKeyPress;
            Figure.PreviewKeyDown += HandleFigurePreviewKeyDown;

            Figure.SetRole(EditorConsts.FocusFigureFigureRole);
            Figure.SetFocus(this);
        }

        public virtual void Uninstall(IEditor host) {
            Figure.UnsetRole();
            Figure.UnsetFocus();

            Figure.MouseClick -= HandleFigureMouseClick;
            Figure.MouseDoubleClick -= HandleFigureMouseDoubleClick;
            Figure.MouseTripleClick -= HandleFigureMouseTripleClick;

            Figure.MouseDown -= HandleFigureMouseDown;
            Figure.MouseUp -= HandleFigureMouseUp;
            Figure.MouseMove -= HandleFigureMouseMove;

            Figure.MouseEnter -= HandleFigureMouseEnter;
            Figure.MouseLeave -= HandleFigureMouseLeave;
            Figure.MouseHover -= HandleFigureMouseHover;

            Figure.DragStart -= HandleFigureDragStart;
            Figure.DragMove -= HandleFigureDragMove;
            Figure.DragFinish -= HandleFigureDragFinish;
            Figure.DragCancel -= HandleFigureDragCancel;

            Figure.ShortcutKeyProcess -= HandleFigureShortcutKeyProcess;
            Figure.KeyDown -= HandleFigureKeyDown;
            Figure.KeyUp -= HandleFigureKeyUp;
            Figure.KeyPress -= HandleFigureKeyPress;
            Figure.PreviewKeyDown -= HandleFigurePreviewKeyDown;

            _host = null;
        }

        public virtual void Relocate(IFigure hostFigure) {
            if (_relocator != null) {
                _relocator(this, Figure, hostFigure);
            } else {
                Figure.Bounds = hostFigure.Bounds;
            }
        }

        public abstract void Begin(Point? location);
        public abstract bool Commit();
        public abstract void Rollback();

        public abstract IContextMenuProvider GetContextMenuProvider();
        public abstract FontDescription GetNextInputFont();

        // ------------------------------
        // protected
        // ------------------------------
        // === event ==========
        protected virtual void OnFigureMouseClick(MouseEventArgs e) {
            var handler = MouseClick;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void OnFigureMouseDoubleClick(MouseEventArgs e) {
            var handler = MouseDoubleClick;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void OnFigureMouseTripleClick(MouseEventArgs e) {
            var handler = MouseTripleClick;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void OnFigureMouseDown(MouseEventArgs e) {
            var handler = MouseDown;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void OnFigureMouseUp(MouseEventArgs e) {
            var handler = MouseUp;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void OnFigureMouseMove(MouseEventArgs e) {
            var handler = MouseMove;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void OnFigureMouseEnter() {
            var handler = MouseEnter;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnFigureMouseLeave() {
            var handler = MouseLeave;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnFigureMouseHover(MouseHoverEventArgs e) {
            var handler = MouseHover;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void OnFigureDragStart(MouseEventArgs e) {
            var handler = DragStart;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void OnFigureDragMove(MouseEventArgs e) {
            var handler = DragMove;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void OnFigureDragFinish(MouseEventArgs e) {
            var handler = DragFinish;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void OnFigureDragCancel() {
            var handler = DragCancel;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnFigureShortcutKeyProcess(ShortcutKeyProcessEventArgs e) {
            var keyData = e.KeyData;
            if (keyData == Keys.ProcessKey && !Host.Site.EditorCanvas.IsInImeComposition) {
                keyData = Host.Site.EditorCanvas._ImmVirtualKey;
            }

            if (_keyMap != null && _keyMap.IsDefined(keyData)) {
                var action = KeyMap.GetAction(keyData);
                if (action != null) {
                    if (action(this)) {
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

        // === event handler ==========
        private void HandleFigureMouseClick(object sender, MouseEventArgs e) {
            OnFigureMouseClick(e);
        }

        private void HandleFigureMouseDoubleClick(object sender, MouseEventArgs e) {
            OnFigureMouseDoubleClick(e);
        }

        private void HandleFigureMouseTripleClick(object sender, MouseEventArgs e) {
            OnFigureMouseTripleClick(e);
        }

        private void HandleFigureMouseDown(object sender, MouseEventArgs e) {
            OnFigureMouseDown(e);
        }

        private void HandleFigureMouseUp(object sender, MouseEventArgs e) {
            OnFigureMouseUp(e);
        }

        private void HandleFigureMouseMove(object sender, MouseEventArgs e) {
            OnFigureMouseMove(e);
        }

        private void HandleFigureMouseEnter(object sender, EventArgs e) {
            OnFigureMouseEnter();
        }

        private void HandleFigureMouseLeave(object sender, EventArgs e) {
            OnFigureMouseLeave();
        }

        private void HandleFigureMouseHover(object sender, MouseHoverEventArgs e) {
            OnFigureMouseHover(e);
        }

        private void HandleFigureDragStart(object sender, MouseEventArgs e) {
            OnFigureDragStart(e);
        }

        private void HandleFigureDragMove(object sender, MouseEventArgs e) {
            OnFigureDragMove(e);
        }

        private void HandleFigureDragFinish(object sender, MouseEventArgs e) {
            OnFigureDragFinish(e);
        }

        private void HandleFigureDragCancel(object sender, EventArgs e) {
            OnFigureDragCancel();
        }


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
