/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using System.Windows.Forms;
using Mkamo.Common.Forms.MouseOperatable;

namespace Mkamo.Figure.Figures {
    partial class AbstractFigure {
        // ========================================
        // method
        // ========================================
        // --- event handler ---
        public void HandleMouseClick(MouseEventArgs e) {
            OnMouseClick(e);
        }

        public void HandleMouseDoubleClick(MouseEventArgs e) {
            OnMouseDoubleClick(e);
        }

        public void HandleMouseTripleClick(MouseEventArgs e) {
            OnMouseTripleClick(e);
        }

        public void HandleMouseDown(MouseEventArgs e) {
            OnMouseDown(e);
        }

        public void HandleMouseUp(MouseEventArgs e) {
            OnMouseUp(e);
        }

        public void HandleMouseMove(MouseEventArgs e) {
            OnMouseMove(e);
        }

        public void HandleMouseEnter() {
            OnMouseEnter();
        }

        public void HandleMouseLeave() {
            OnMouseLeave();
        }

        public void HandleMouseHover(MouseHoverEventArgs e) {
            OnMouseHover(e);
        }

        public void HandleDragStart(MouseEventArgs e) {
            OnDragStart(e);
        }

        public void HandleDragMove(MouseEventArgs e) {
            OnDragMove(e);
        }

        public void HandleDragFinish(MouseEventArgs e) {
            OnDragFinish(e);
        }

        public void HandleDragCancel() {
            OnDragCancel();
        }

        public virtual Cursor GetMouseCursor(MouseEventArgs e) {
            return _cursorProvider == null? null: _cursorProvider(e);
        }


        // --- event ---
        protected virtual void OnMouseClick(MouseEventArgs e) {
            var handler = MouseClick;
            if (handler != null) {
                handler(this, e);
            }

            if (_mouseEventsToForwards != null) {
                foreach (var forward in _MouseEventsToForwards) {
                    forward.HandleMouseClick(e);
                }
            }
        }

        protected virtual void OnMouseDoubleClick(MouseEventArgs e) {
            var handler = MouseDoubleClick;
            if (handler != null) {
                handler(this, e);
            }

            if (_mouseEventsToForwards != null) {
                foreach (var forward in _MouseEventsToForwards) {
                    forward.HandleMouseDoubleClick(e);
                }
            }
        }

        protected virtual void OnMouseTripleClick(MouseEventArgs e) {
            var handler = MouseTripleClick;
            if (handler != null) {
                handler(this, e);
            }

            if (_mouseEventsToForwards != null) {
                foreach (var forward in _MouseEventsToForwards) {
                    forward.HandleMouseTripleClick(e);
                }
            }
        }

        protected virtual void OnMouseDown(MouseEventArgs e) {
            var handler = MouseDown;
            if (handler != null) {
                handler(this, e);
            }

            if (_mouseEventsToForwards != null) {
                foreach (var forward in _MouseEventsToForwards) {
                    forward.HandleMouseDown(e);
                }
            }
        }

        protected virtual void OnMouseUp(MouseEventArgs e) {
            var handler = MouseUp;
            if (handler != null) {
                handler(this, e);
            }

            if (_mouseEventsToForwards != null) {
                foreach (var forward in _MouseEventsToForwards) {
                    forward.HandleMouseUp(e);
                }
            }
        }

        protected virtual void OnMouseMove(MouseEventArgs e) {
            var handler = MouseMove;
            if (handler != null) {
                handler(this, e);
            }

            if (_mouseEventsToForwards != null) {
                foreach (var forward in _MouseEventsToForwards) {
                    forward.HandleMouseMove(e);
                }
            }
        }

        protected virtual void OnMouseEnter() {
            var handler = MouseEnter;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }

            if (_mouseEventsToForwards != null) {
                foreach (var forward in _MouseEventsToForwards) {
                    forward.HandleMouseEnter();
                }
            }
        }

        protected virtual void OnMouseLeave() {
            var handler = MouseLeave;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }

            if (_mouseEventsToForwards != null) {
                foreach (var forward in _MouseEventsToForwards) {
                    forward.HandleMouseLeave();
                }
            }
        }

        protected virtual void OnMouseHover(MouseHoverEventArgs e) {
            var handler = MouseHover;
            if (handler != null) {
                handler(this, e);
            }

            if (_mouseEventsToForwards != null) {
                foreach (var forward in _MouseEventsToForwards) {
                    forward.HandleMouseHover(e);
                }
            }
        }

        protected virtual void OnDragStart(MouseEventArgs e) {
            var handler = DragStart;
            if (handler != null) {
                handler(this, e);
            }

            if (_mouseEventsToForwards != null) {
                foreach (var forward in _MouseEventsToForwards) {
                    forward.HandleDragStart(e);
                }
            }
        }

        protected virtual void OnDragMove(MouseEventArgs e) {
            var handler = DragMove;
            if (handler != null) {
                handler(this, e);
            }

            if (_mouseEventsToForwards != null) {
                foreach (var forward in _MouseEventsToForwards) {
                    forward.HandleDragMove(e);
                }
            }
        }

        protected virtual void OnDragFinish(MouseEventArgs e) {
            var handler = DragFinish;
            if (handler != null) {
                handler(this, e);
            }

            if (_mouseEventsToForwards != null) {
                foreach (var forward in _MouseEventsToForwards) {
                    forward.HandleDragFinish(e);
                }
            }
        }

        protected virtual void OnDragCancel() {
            var handler = DragCancel;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }

            if (_mouseEventsToForwards != null) {
                foreach (var forward in _MouseEventsToForwards) {
                    forward.HandleDragCancel();
                }
            }
        }

    }
}
