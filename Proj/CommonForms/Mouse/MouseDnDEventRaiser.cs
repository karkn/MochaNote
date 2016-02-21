/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Mkamo.Common.Forms.Drawing;

namespace Mkamo.Common.Forms.Mouse {
    /// 各種Handelメソッドを呼ぶと適宜各種イベントを発火する
    ///   - DragMove中はMouseMoveは発火しない．
    ///   - Dragが開始されたらMouseUpは発火しない．
    public class MouseDnDEventRaiser {
        // ========================================
        // field
        // ========================================
        private object _eventSender;

        private Size _dragSize;
        private bool _isStarted;
        private Point _dragStartPoint;
        private Rectangle _dragStartRect;
        private CancelMouseEventArgs _dragStartMouseEventArgs;

        // --- multi click ---
        private int _clickCount = 1;
        private DateTime _prevMouseDownTime;
        private Point _prevMouseDownLocation;
        private Size _doubleClickSize = SystemInformation.DoubleClickSize;
        private int _doubleClickTime = SystemInformation.DoubleClickTime;

        // ========================================
        // constructor
        // ========================================
        public MouseDnDEventRaiser(object eventSender) {
            _dragSize = SystemInformation.DragSize;
            _eventSender = eventSender;
        }

        // ========================================
        // property
        // ========================================
        public bool IsPrepared {
            get { return !_dragStartRect.IsEmpty && _dragStartMouseEventArgs != null; }
        }

        public bool IsStarted {
            get { return _isStarted; }
        }

        // ========================================
        // event
        // ========================================
        public event EventHandler<MouseEventArgs> MouseDown;
        public event EventHandler<MouseEventArgs> MouseMove;
        public event EventHandler<MouseEventArgs> MouseUp;
        public event EventHandler<MouseEventArgs> MouseClick;
        public event EventHandler<MouseEventArgs> MouseDoubleClick;
        public event EventHandler<MouseEventArgs> MouseTripleClick;

        public event EventHandler<MouseEventArgs> DragStart;
        public event EventHandler<MouseEventArgs> DragMove;
        public event EventHandler<MouseEventArgs> DragFinish;
        public event EventHandler<EventArgs> DragCancel;

        public event EventHandler<KeyEventArgs> KeyDown;

        // ========================================
        // method
        // ========================================
        public void HandleMouseDown(MouseEventArgs e) {
            OnMouseDown(e);
        }

        public void HandleMouseMove(MouseEventArgs e) {
            OnMouseMove(e);
        }

        public void HandleMouseUp(MouseEventArgs e) {
            OnMouseUp(e);
        }

        public void HandleKeyDown(KeyEventArgs e) {
            OnKeyDown(e);
        }

        public void HandleMouseClick(MouseEventArgs e) {
            OnMouseClick(e);
        }

        public void HandleMouseDoubleClick(MouseEventArgs e) {
            OnMouseDoubleClick(e);
        }

        public void CancelDrag() {
            if (IsPrepared || IsStarted) {
                OnDragCancel();
                ClearDnDState();
            }
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected virtual void OnMouseDown(MouseEventArgs e) {
            _dragStartPoint = e.Location;
            _dragStartRect = new Rectangle(
                new Point(e.X - _dragSize.Width / 2, e.Y - _dragSize.Height / 2),
                _dragSize
            );
            _dragStartMouseEventArgs = new CancelMouseEventArgs(e);

            if (_isStarted && e.Button == MouseButtons.Right) {
                CancelDrag();
            }

            ProcessClickCount(e);
            if (e.Clicks != _clickCount) {
                e = new MouseEventArgs(e.Button, _clickCount, e.X, e.Y, e.Delta);
            }

            var handler = MouseDown;
            if (handler != null) {
                handler(_eventSender, e);
            }
        }

        protected virtual void OnMouseMove(MouseEventArgs e) {
            if (_isStarted) {
                OnDragMove(e);
            } else if (IsPrepared && !_dragStartRect.Contains(e.Location)) {
                if (!_isStarted) {
                    var eventArgs = _dragStartMouseEventArgs;
                    OnDragStart(eventArgs);
                    if (eventArgs.IsCanceled) {
                        OnDragCancel();
                        ClearDnDState();
                        return;
                    }
                    _isStarted = true;
                }
                OnDragMove(e);
            } else {
                var handler = MouseMove;
                if (handler != null) {
                    handler(_eventSender, e);
                }
            }
        }


        protected virtual void OnMouseUp(MouseEventArgs e) {
            if (_isStarted) {
                ClearDnDState();
                OnDragFinish(e);
            } else {
                ClearDnDState();
                
                var handler = MouseUp;
                if (handler != null) {
                    handler(_eventSender, e);
                }
            }
        }

        protected virtual void OnMouseClick(MouseEventArgs e) {
            /// ドラッグ時は発火しない
            if (!_isStarted) {

                if (!MultiClick(e)) {
                    var handler = MouseClick;
                    if (handler != null) {
                        handler(_eventSender, e);
                    }
                }
            }
        }

        protected virtual void OnMouseDoubleClick(MouseEventArgs e) {
            /// ドラッグ時は発火しない
            if (!_isStarted) {
                if (!MultiClick(e)) {
                    var handler = MouseDoubleClick;
                    if (handler != null) {
                        handler(_eventSender, e);
                    }
                }
            }
        }

        protected virtual void OnMouseTripleClick(MouseEventArgs e) {
            if (!_isStarted) {
                var handler = MouseTripleClick;
                if (handler != null) {
                    handler(_eventSender, e);
                }
            }
        }
        
        protected virtual void OnKeyDown(KeyEventArgs e) {
            if (_isStarted && (e.KeyData == Keys.Escape)) {
                CancelDrag();
                e.Handled = true;
            } else {
                var handler = KeyDown;
                if (handler != null) {
                    handler(_eventSender, e);
                }
            }
        }

        protected virtual void OnDragStart(MouseEventArgs e) {
            var handler = DragStart;
            if (handler != null) {
                handler(_eventSender, e);
            }
        }

        protected virtual void OnDragMove(MouseEventArgs e) {
            var handler = DragMove;
            if (handler != null) {
                handler(_eventSender, e);
            }
        }

        protected virtual void OnDragFinish(MouseEventArgs e) {
            var handler = DragFinish;
            if (handler != null) {
                handler(_eventSender, e);
            }
        }

        protected virtual void OnDragCancel() {
            var handler = DragCancel;
            if (handler != null) {
                handler(_eventSender, EventArgs.Empty);
            }
        }

        protected virtual void ClearDnDState() {
            _isStarted = false;
            _dragStartPoint = Point.Empty;
            _dragStartRect = Rectangle.Empty;
            _dragStartMouseEventArgs = null;
        }
        
        private void ProcessClickCount(MouseEventArgs e) {
            var rect = RectUtil.GetArroundingRectangle(_prevMouseDownLocation, _doubleClickSize);
            var now = DateTime.Now;

            if (rect.Contains(e.Location)) {
                var span = now - _prevMouseDownTime;
                if (_clickCount < 4) {
                    if (span.TotalMilliseconds <= _doubleClickTime) {
                        ++_clickCount;
                    } else {
                        _clickCount = 1;
                    }
                } else {
                    _clickCount = 1;
                }
            } else {
                _clickCount = 1;
            }

            _prevMouseDownTime = now;
            _prevMouseDownLocation = e.Location;
        }

        private bool MultiClick(MouseEventArgs e) {
            if (_clickCount == 3) {
                OnMouseTripleClick(e);
                return true;
            } else if (_clickCount == 4) {
                // todo: quadruple click
                return true;
            } else {
                return false;
            }
        }

    
    }
}
