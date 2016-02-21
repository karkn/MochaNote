/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mkamo.Common.Forms.MouseOperatable;
using Mkamo.Figure.Core;
using System.Drawing;

namespace Mkamo.Figure.Core {
    public class EventDispatcher: IDisposable {
        // ========================================
        // field
        // ========================================
        private Canvas _canvas;
        private MouseOperatableEventDispatcher _mouseDispatcher;

        // ========================================
        // constructor
        // ========================================
        public EventDispatcher(Canvas canvas, Func<Point, IMouseOperatable> finder) {
            _canvas = canvas;
            _mouseDispatcher = MouseOperatableFactory.CreateMouseOperatableEventDispatcher(
                finder, canvas
            );
        }

        public EventDispatcher(Canvas canvas):
            this(
                canvas,
                pt => {
                    if (canvas.FigureContent != null) {
                        return canvas.FigureContent.FindFigure(
                            fig => fig.IsVisible && fig.ContainsPoint(pt),
                            true
                        );
                    }
                    return null;
                }
            )
        {

        }

        // ========================================
        // destructor
        // ========================================
        public void Dispose() {
            if (_mouseDispatcher != null) {
                _mouseDispatcher.Dispose();
            }
            GC.SuppressFinalize(this);
        }


        // ========================================
        // property
        // ========================================
        public IMouseOperatable Target {
            get { return _mouseDispatcher.Target; }
        }

        public bool TargetFoundOnMouseDown {
            get { return _mouseDispatcher.TargetFoundOnMouseDown; }
        }

        public bool DragOverIsStarted {
            get { return _mouseDispatcher.DragOverIsStarted; }
        }

        public IDragSource DragOutDragSource {
            get { return _mouseDispatcher.DragOutDragSource; }
            set { _mouseDispatcher.DragOutDragSource = value; }
        }

        // ========================================
        // method
        // ========================================
        // --- mouse ---
        public virtual void HandleMouseDown(MouseEventArgs e) {
            _mouseDispatcher.HandleMouseDown(e);
        }

        public virtual void HandleMouseMove(MouseEventArgs e) {
            _mouseDispatcher.HandleMouseMove(e);
        }

        public virtual void HandleMouseUp(MouseEventArgs e) {
            _mouseDispatcher.HandleMouseUp(e);
        }

        public virtual void HandleDragStart(MouseEventArgs e) {
            _mouseDispatcher.HandleDragStart(e);
        }
        public virtual void HandleDragMove(MouseEventArgs e) {
            _mouseDispatcher.HandleDragMove(e);
        }
        public virtual void HandleDragFinish(MouseEventArgs e) {
            _mouseDispatcher.HandleDragFinish(e);
        }
        public virtual void HandleDragCancel(EventArgs e) {
            _mouseDispatcher.HandleDragCancel(e);
        }

        public virtual void HandleMouseClick(MouseEventArgs e) {
            _mouseDispatcher.HandleMouseClick(e);
        }

        public virtual void HandleMouseDoubleClick(MouseEventArgs e) {
            _mouseDispatcher.HandleMouseDoubleClick(e);
        }

        public virtual void HandleMouseTripleClick(MouseEventArgs e) {
            _mouseDispatcher.HandleMouseTripleClick(e);
        }

        public virtual void HandleMouseEnter(EventArgs e) {
            _mouseDispatcher.HandleMouseEnter(e);
        }

        public virtual void HandleMouseLeave(EventArgs e) {
            _mouseDispatcher.HandleMouseLeave(e);
        }

        public virtual void HandleMouseHover(EventArgs e) {
            _mouseDispatcher.HandleMouseHover(e);
        }


        // --- dnd ---
        public virtual void HandleDragOver(DragEventArgs e) {
            _mouseDispatcher.HandleDragOver(e);
        }
        
        public virtual void HandleDragDrop(DragEventArgs e) {
            _mouseDispatcher.HandleDragDrop(e);
        }

        public virtual void HandleDragEnter(DragEventArgs e) {
            _mouseDispatcher.HandleDragEnter(e);
        }

        public virtual void HandleDragLeave(EventArgs e) {
            _mouseDispatcher.HandleDragLeave(e);
        }

        public virtual void HandleQueryContinueDrag(QueryContinueDragEventArgs e) {
            _mouseDispatcher.HandleQueryContinueDrag(e);
        }

        public void SetDnDTarget(IMouseOperatable target) {
            _mouseDispatcher.SetDnDTarget(target);
        }

        // --- key ---
        //public virtual void HandleKeyDown(KeyEventArgs e) {
        //    if (_focused != null) {
        //        _focused.HandleKeyDown(e);
        //    }
        //}

        //public virtual void HandleKeyUp(KeyEventArgs e) {
        //    if (_focused != null) {
        //        _focused.HandleKeyUp(e);
        //    }
        //}

        //public virtual void HandleKeyPress(KeyPressEventArgs e) {
        //    if (_focused != null) {
        //        _focused.HandleKeyPress(e);
        //    }
        //}

        //public virtual void HandlePreviewKeyDown(PreviewKeyDownEventArgs e) {
        //    if (_focused != null) {
        //        _focused.HandlePreviewKeyDown(e);
        //    }
        //}

    }
}
