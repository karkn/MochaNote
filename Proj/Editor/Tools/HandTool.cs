/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace Mkamo.Editor.Tools {
    public class HandTool: AbstractTool {
        // ========================================
        // field
        // ========================================
        private EditorCanvas _canvas;
        private Point _prevLocation;

        // ========================================
        // constructor
        // ========================================
        public HandTool(EditorCanvas canvas) {
            _canvas = canvas;
        }

        // ========================================
        // event
        // ========================================
        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MouseUp;
        public event MouseEventHandler DragFinish;
        public event EventHandler DragCancel;

        // ========================================
        // method
        // ========================================
        public override bool HandleMouseDown(MouseEventArgs e) {
            _prevLocation = _canvas.TranslateToControlPoint(e.Location);
            OnMouseDown(e);
            return true;
        }

        public override bool HandleMouseMove(MouseEventArgs e) {
            return true;
        }

        public override bool HandleMouseUp(MouseEventArgs e) {
            OnMouseUp(e);
            return true;
        }

        public override bool HandleDragStart(MouseEventArgs e) {
            return true;
        }

        public override bool HandleDragMove(MouseEventArgs e) {
            if (e.Location == _prevLocation) {
                return true;
            }

            var loc = _canvas.TranslateToControlPoint(e.Location);
            var delta = (Size) loc - (Size) _prevLocation;

            if (_canvas.CanvasBackgroundImage != null) {
                _canvas.Invalidate();
            }
            _canvas.AutoScrollPosition =
                new Point(
                    -_canvas.AutoScrollPosition.X - delta.Width,
                    -_canvas.AutoScrollPosition.Y - delta.Height
                );

            _prevLocation = loc;
            return true;
        }

        public override bool HandleDragFinish(MouseEventArgs e) {
            OnDragFinish(e);
            return true;
        }

        public override bool HandleDragCancel(EventArgs e) {
            OnDragCancel();
            return true;
        }

        public override bool HandleMouseClick(MouseEventArgs e) {
            return true;
        }

        public override bool HandleMouseDoubleClick(MouseEventArgs e) {
            return true;
        }

        public override bool HandleMouseEnter(EventArgs e) {
            return true;
        }

        public override bool HandleMouseLeave(EventArgs e) {
            return true;
        }

        public override bool HandleMouseHover(EventArgs e) {
            return true;
        }

        public override bool HandleKeyDown(KeyEventArgs e) {
            if (e.KeyCode == Keys.Space) {
                if (e.Shift) {
                    _canvas.ScrollUp();
                } else {
                    _canvas.ScrollDown();
                }
            } else if (e.KeyCode == Keys.Back) {
                _canvas.ScrollUp();
            }
            return true;
        }

        public override bool HandleKeyUp(KeyEventArgs e) {
            return true;
        }

        public override bool HandleKeyPress(KeyPressEventArgs e) {
            return true;
        }

        public override bool HandlePreviewKeyDown(PreviewKeyDownEventArgs e) {
            return true;
        }


        // ------------------------------
        // protected
        // ------------------------------
        protected virtual void OnMouseDown(MouseEventArgs e) {
            var handler = MouseDown;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void OnMouseUp(MouseEventArgs e) {
            var handler = MouseUp;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void OnDragFinish(MouseEventArgs e) {
            var handler = DragFinish;
            if (handler != null) {
                handler(this, e);
            }
        }

        protected virtual void OnDragCancel() {
            var handler = DragCancel;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

    }
}
