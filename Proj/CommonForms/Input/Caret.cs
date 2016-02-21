/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Mkamo.Common.Win32.User32;

namespace Mkamo.Common.Forms.Input {
    public class Caret: IDisposable {
        // ========================================
        // field
        // ========================================
        Control _control;
        Size _size;
        Point _position;
        bool _isVisible;

        bool _hasControlFocus;
        bool _isCaretCreated;
        bool _isCaretShown;

        Func<Point, Point> _positionTranslator;

        // ========================================
        // constructor
        // ========================================
        public Caret(Control control, Func<Point, Point> positionTransformer):
            this(control, Point.Empty, new Size(1, (int) (control.Font.GetHeight() + 0.5)), positionTransformer) {

        }

        public Caret(Control control, Point position, Size size, Func<Point, Point> positionTranslator) {
            _control = control;
            _position = position;
            _size = size;
            _positionTranslator = positionTranslator;

            _isVisible = true;

            _hasControlFocus = _control.Focused;

            _isCaretShown = false;
            _isCaretCreated = false;

            _control.GotFocus += HandleControlGotFocus;
            _control.LostFocus += HandleControlLostFocus;
        }

        // ========================================
        // destructor
        // ========================================
        // === IDisposable ==========
        ~Caret() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                _control.GotFocus -= HandleControlGotFocus;
                _control.LostFocus -= HandleControlLostFocus;
            }
            DestroyCaret();
        }


        // ========================================
        // event
        // ========================================
        public event EventHandler CaretMoved;
        public event EventHandler VisibleChanged;

        // ========================================
        // property
        // ========================================
        public Rectangle Bounds {
            get { return new Rectangle(Position, Size); }
        }
        
        public virtual Size Size {
            get { return _size; }
            set {
                if (_size == value) {
                    return;
                }
                _size = value;
                UpdateCaretSize();
            }
        }

        public virtual int Width {
            get { return Size.Width; }
            set { Size = new Size(value, _size.Height); }
        }

        public virtual int Height {
            get { return Size.Height; }
            set { Size = new Size(_size.Width, value); }
        }

        public virtual Point Position {
            get { return _position; }
            set {
                if (value == _position) {
                    return;
                }
                _position = value;
                UpdateCaretPosition();
                OnCaretMoved();
            }
        }

        public virtual int Left {
            get { return Position.X; }
            set { Position = new Point(value, Position.Y); }
        }

        public virtual int Top {
            get { return Position.Y; }
            set { Position = new Point(Position.X, value); }
        }

        public bool IsVisible {
            get { return _isVisible; }
            set {
                if (_isVisible == value) {
                    return;
                }

                _isVisible = value;
                UpdateCaretShown();
                OnVisibleChanged();
            }
        }

        public bool IsShown {
            get { return _isCaretShown; }
        }

        // ========================================
        // method
        // ========================================
        // === Caret ==========
        public void Show() {
            IsVisible = true;
        }

        public void Hide() {
            IsVisible = false;
        }


        // ------------------------------
        // protected
        // ------------------------------
        protected virtual void OnCaretMoved() {
            var handler = CaretMoved;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnVisibleChanged() {
            var handler = VisibleChanged;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        // ------------------------------
        // private
        // ------------------------------
        private void CreateCaret() {
            User32PI.CreateCaret(_control.Handle, IntPtr.Zero, _size.Width, _size.Height);
            _isCaretCreated = true;
        }

        private void DestroyCaret() {
            User32PI.DestroyCaret();
            _isCaretCreated = false;
        }

        private void ShowCaret() {
            User32PI.ShowCaret(_control.Handle);
            _isCaretShown = true;
        }

        private void HideCaret() {
            User32PI.HideCaret(_control.Handle);
            _isCaretShown = false;
        }


        private void UpdateCaretPosition() {
            if (_isCaretCreated) {
                var pos = _positionTranslator == null? _position: _positionTranslator(_position);
                User32PI.SetCaretPos(pos.X, pos.Y);
                //User32PI.SetCaretPos(_position.X, _position.Y);
            }
        }

        private void UpdateCaretSize() {
            if (_isCaretCreated) {
                User32PI.DestroyCaret();
                User32PI.CreateCaret(_control.Handle, IntPtr.Zero, _size.Width, _size.Height);
                if (_isCaretShown) {
                    User32PI.ShowCaret(_control.Handle);
                }
            }
        }

        private void UpdateCaretShown() {
            if (_isVisible && _hasControlFocus) {
                if (!_isCaretShown) {
                    if (!_isCaretCreated) {
                        CreateCaret();
                        UpdateCaretPosition();
                    }
                    ShowCaret();
                }
            } else {
                if (_isCaretShown) {
                    HideCaret();
                    if (_isCaretCreated) {
                        DestroyCaret();
                    }
                }
            }
        }

        private void HandleControlGotFocus(object sender, EventArgs e) {
            _hasControlFocus = true;
            UpdateCaretShown();
        }

        private void HandleControlLostFocus(object sender, EventArgs e) {
            _hasControlFocus = false;
            UpdateCaretShown();
        }
    }
}
