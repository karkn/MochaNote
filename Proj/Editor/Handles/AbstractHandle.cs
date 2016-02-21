/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Diagnostics;
using Mkamo.Editor.Core;
using System.Windows.Forms;
using Mkamo.Figure.Core;
using Mkamo.Common.Forms.MouseOperatable;

namespace Mkamo.Editor.Handles {
    public abstract class AbstractHandle: IHandle {
        // ========================================
        // field
        // ========================================
        private IEditor _host;
        private Cursor _cursor;

        // ========================================
        // constructor
        // ========================================
        protected AbstractHandle() {
            _cursor = null;
        }

        // ========================================
        // event
        // ========================================
        public event EventHandler<EventArgs> Installed;
        public event EventHandler<EventArgs> Uninstalling;

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


        // ========================================
        // property
        // ========================================
        public IEditor Host {
            get { return _host; }
        }

        public virtual Cursor Cursor {
            get { return _cursor; }
            set { _cursor = value; }
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected abstract IFigure _Figure { get; }

        // ========================================
        // method
        // ========================================
        public virtual void Install(IEditor host) {
            Contract.Requires(host != null);

            _host = host;

            if (_Figure != null) {
                _Figure.CursorProvider = GetMouseCursor;

                _Figure.MouseClick += HandleFigureMouseClick;
                _Figure.MouseDoubleClick += HandleFigureMouseDoubleClick;
                _Figure.MouseTripleClick += HandleFigureMouseTripleClick;

                _Figure.MouseDown += HandleFigureMouseDown;
                _Figure.MouseUp += HandleFigureMouseUp;
                _Figure.MouseMove += HandleFigureMouseMove;

                _Figure.MouseEnter += HandleFigureMouseEnter;
                _Figure.MouseLeave += HandleFigureMouseLeave;
                _Figure.MouseHover += HandleFigureMouseHover;

                _Figure.DragStart += HandleFigureDragStart;
                _Figure.DragMove += HandleFigureDragMove;
                _Figure.DragFinish += HandleFigureDragFinish;
                _Figure.DragCancel += HandleFigureDragCancel;
            }

            OnInstalled();
        }

        public virtual void Uninstall(IEditor host) {
            Contract.Requires(host != null);

            OnUninstalling();

            if (_Figure != null) {
                _Figure.MouseClick -= HandleFigureMouseClick;
                _Figure.MouseDoubleClick -= HandleFigureMouseDoubleClick;
                _Figure.MouseTripleClick -= HandleFigureMouseTripleClick;

                _Figure.MouseDown -= HandleFigureMouseDown;
                _Figure.MouseUp -= HandleFigureMouseUp;
                _Figure.MouseMove -= HandleFigureMouseMove;

                _Figure.MouseEnter -= HandleFigureMouseEnter;
                _Figure.MouseLeave -= HandleFigureMouseLeave;
                _Figure.MouseHover -= HandleFigureMouseHover;

                _Figure.DragStart -= HandleFigureDragStart;
                _Figure.DragMove -= HandleFigureDragMove;
                _Figure.DragFinish -= HandleFigureDragFinish;
                _Figure.DragCancel -= HandleFigureDragCancel;

                _Figure.CursorProvider = null;
            }

            _host = null;
        }


        // ------------------------------
        // protected
        // ------------------------------
        protected virtual Cursor GetMouseCursor(MouseEventArgs e) {
            return _cursor;
        }

        // === event ==========
        protected virtual void OnInstalled() {
            var handler = Installed;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnUninstalling() {
            var handler = Uninstalling;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

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

    }
}
