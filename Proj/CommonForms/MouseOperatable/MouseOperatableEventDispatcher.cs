/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Mkamo.Common.Collection;
using Mkamo.Common.Forms.MouseOperatable.Internal;
using Mkamo.Common.Core;
using Mkamo.Common.Forms.Mouse;

namespace Mkamo.Common.Forms.MouseOperatable {
    /// <summary>
    /// DragされたときはClick，DoubleClickイベントをディスパッチしない．
    /// </summary>
    public class MouseOperatableEventDispatcher: IDisposable {
        // ----------------------------------------
        // property
        // ----------------------------------------
        private Func<Point, IMouseOperatable> _targetFinder;
        private Control _control;
        private IDragSource _dragOutDragSource;

        private DragSourceEventProcessor _dragSourceEventProcessor;
        private MouseOperatableDnDProcessor _dndEventProcessor;

        /// dnd process state
        private IMouseOperatable _target;
        private IMouseOperatable _lastTarget;
        private Point _lastMouseMovePosition;
        private MouseEventArgs _lastMouseMoveEventArgs;
        //private long _lastCanceledSessionId;

        /// dragtarget process state
        private IMouseOperatable _lastDragOverTarget;
        private bool _dragOverIsStarted;

        private bool _dragOutToBeCanceledOnQueryContinueDrag;
        private bool _dragOutCanceledOnQueryContinueDrag;

        private Timer _hoverOnTimer;
        private Size _hoverSize = SystemInformation.MouseHoverSize;
        private IMouseOperatable _lastHovered;
       
        // ----------------------------------------
        // constructor
        // ----------------------------------------
        public MouseOperatableEventDispatcher(Func<Point, IMouseOperatable> targetFinder, Control control) {
            _targetFinder = targetFinder;
            _control = control;

            _dragSourceEventProcessor = new DragSourceEventProcessor();
            _dndEventProcessor = new MouseOperatableDnDProcessor();

            _target = null;
            _lastTarget = null;

            _lastDragOverTarget = null;
            _dragOverIsStarted = false;

            _dragOutToBeCanceledOnQueryContinueDrag = false;
            _dragOutCanceledOnQueryContinueDrag = false;

            _hoverOnTimer = new Timer();
            _hoverOnTimer.Interval = SystemInformation.MouseHoverTime;
            _hoverOnTimer.Tick += (se, ev) => {
                _lastHovered = _hoverOnTimer.Tag as IMouseOperatable;
                var hoverEventArgs = new MouseHoverEventArgs(_lastMouseMoveEventArgs);
                if (_lastHovered != null) {
                    _lastHovered.HandleMouseHover(hoverEventArgs);
                }
                if (hoverEventArgs.ResetHover) {
                    _lastHovered = null;
                }
                _hoverOnTimer.Stop();
            };
        }

        // ========================================
        // destructor
        // ========================================
        public void Dispose() {
            if (_hoverOnTimer != null) {
                _hoverOnTimer.Stop();
                _hoverOnTimer.Dispose();
            }
            GC.SuppressFinalize(this);
        }


        // ========================================
        // property
        // ========================================
        public IMouseOperatable Target {
            get { return _target; }
        }

        public bool TargetFoundOnMouseDown {
            get { return _target != null; }
        }

        public bool TargetIsInDragging {
            get { return _dndEventProcessor.IsStarted; }
        }

        public bool DragOverIsStarted {
            get { return _dragOverIsStarted; }
        }

        public IDragSource DragOutDragSource {
            get { return _dragOutDragSource; }
            set { _dragOutDragSource = value; }
        }

        // ----------------------------------------
        // method
        // ----------------------------------------
        public bool HandleMouseMove(MouseEventArgs e) {
            var target = _targetFinder(e.Location);
            var ret = false;
            if (target != null) {
                if (target != _lastTarget) {
                    /// targetが変わった場合
                    if (_lastTarget != null) {
                        _lastTarget.HandleMouseLeave();
                    }
                    target.HandleMouseEnter();
                    _lastTarget = target;

                    _lastHovered = null;
                    _hoverOnTimer.Tag = target;
                    _hoverOnTimer.Stop();
                    _hoverOnTimer.Start();
                } else {
                    if (target != _lastHovered) {
                        _hoverOnTimer.Tag = target;
                        if (!InMouseHoverSize(e.Location, _lastMouseMovePosition)) {
                            _hoverOnTimer.Stop();
                        }
                        _hoverOnTimer.Start();
                    }
                }
                target.HandleMouseMove(e);
                _control.Cursor = target.GetMouseCursor(e)?? Cursors.Default;
                ret = true;
            } else {
                if (_lastTarget != null) {
                    /// targetがなくなった場合
                    _lastTarget.HandleMouseLeave();
                    _control.Cursor = Cursors.Default;
                    _lastTarget = null;
                }
                _lastHovered = null;
                _hoverOnTimer.Stop();
                ret = false;
            }
            _lastMouseMovePosition = e.Location;
            _lastMouseMoveEventArgs = e;
            return ret;
        }

        public bool HandleMouseDown(MouseEventArgs e) {
            if (_dndEventProcessor.IsStarted) {
                return false;
            }
            _target = _targetFinder(e.Location);
            if (_target != null) {
                _dragSourceEventProcessor.PrepareDnDOnMouseDown(_target, e);
                _dndEventProcessor.PrepareDnD(_target, e);
                _target.HandleMouseDown(e);
                return true;
            }
            return false;
        }

        public bool HandleMouseUp(MouseEventArgs e) {
            if (_dndEventProcessor.IsPrepared) {
                _dndEventProcessor.ClearDnDState();
            }
            if (_dragSourceEventProcessor.IsPrepared) {
                _dragSourceEventProcessor.ClearDnDState();
            }
            if (_target != null) {
                _target.HandleMouseUp(e);
                _target = null;
                return true;
            }

            return false;
        }
        public bool HandleDragStart(MouseEventArgs e) {
            if (_dragSourceEventProcessor.ShouldProcessDnDOnMouseDown(e.Location)) {
                /// DragSourceへのイベントディスパッチ
                _dndEventProcessor.ClearDnDState();
                _dragSourceEventProcessor.ProcessDnD(_control, _target);

                /// MouseDnDEventProducerのDragStartをキャンセルしたことを設定
                ((CancelMouseEventArgs) e).IsCanceled = true;

                return true;

            } else if (_dndEventProcessor.IsPrepared && _dndEventProcessor.ShouldStartDnD(e.Location)) {
                /// MouseOperatableへのイベントディスパッチ
                _dragSourceEventProcessor.ClearDnDState();
                _dndEventProcessor.StartDnD(e);
                return true;
            }
            return false;
        }

        public bool HandleDragMove(MouseEventArgs e) {
            if (_dndEventProcessor.IsStarted) {
                if (_dndEventProcessor.IsPaused) {
                    _dndEventProcessor.RestartDnD();
                    _control.Capture = true;
                } else {
                    var dragOutDragSource = _dragOutDragSource as DragSource;
                    if (dragOutDragSource != null &&
                        _dragSourceEventProcessor.ShouldDnDOnDragOut(dragOutDragSource, _target, e)
                    ) {
                        _dndEventProcessor.PauseDnD();
                        if (
                            _dragSourceEventProcessor.ProcessDnD(_control, _target) ||
                            !_dragOutCanceledOnQueryContinueDrag
                        ) {
                            _dndEventProcessor.CancelDnD(e);
                        }
                    } else {
                        _dndEventProcessor.ProcessDnD(e);
                    }
                }
                return true;
            }
            return false;
        }

        public bool HandleDragFinish(MouseEventArgs e) {
            if (_dndEventProcessor.IsStarted) {
                _dndEventProcessor.FinishDnD(e);
                return true;
            }

            return false;
        }

        public bool HandleDragCancel(EventArgs e) {
            if (_dndEventProcessor.IsStarted) {
                _dndEventProcessor.CancelDnD(EventArgs.Empty);
                //_lastCanceledSessionId = _dndEventProcessor.SessionId;
                return true;
            }
            return false;
        }

        public bool HandleMouseClick(MouseEventArgs e) {
            if (_target != null) {
                //if (
                //    !_dndEventProcessor.IsStarted &&
                //    !(
                //        _dndEventProcessor.IsLastDndCanceled &&
                //        _dndEventProcessor.SessionId == _lastCanceledSessionId
                //    )
                //) {
                    _target.HandleMouseClick(e);
                    return true;
                //}
            }
            return false;
        }

        public bool HandleMouseDoubleClick(MouseEventArgs e) {
            if (_target != null) {
                //if (
                //    !_dndEventProcessor.IsStarted &&
                //    !(
                //        _dndEventProcessor.IsLastDndCanceled &&
                //        _dndEventProcessor.SessionId == _lastCanceledSessionId
                //    )
                //) {
                    _target.HandleMouseDoubleClick(e);
                    return true;
                //}
            }
            return false;
        }

        public bool HandleMouseTripleClick(MouseEventArgs e) {
            if (_target != null) {
                _target.HandleMouseTripleClick(e);
                return true;
            }
            return false;
        }

        public bool HandleMouseEnter(EventArgs e) {
            return false;
        }

        public bool HandleMouseLeave(EventArgs e) {
            if (_lastTarget != null) {
                _lastTarget.HandleMouseLeave();
                _control.Cursor = Cursors.Default;
                _lastTarget = null;
                return true;
            }
            return false;
        }

        public bool HandleMouseHover(EventArgs e) {
            //var target = _targetFinder(_lastMouseMovePosition);
            //if (target != null) {
            //    target.HandleMouseHover();
            //    return true;
            //}
            return false;
        }


        public bool HandleDragOver(DragEventArgs e) {
            if (_dndEventProcessor.IsPaused) {
                _dragOutToBeCanceledOnQueryContinueDrag = true;
                return true;
            }

            var pt = new Point(e.X, e.Y);
            var target = _targetFinder(pt);

            if (target != null) {
                var dragTarget = target.DragTarget as DragTarget;

                /// drag overのターゲットが変わったとき
                if (target != _lastDragOverTarget) {
                    if (_lastDragOverTarget != null) {
                        var lastDragTarget = _lastDragOverTarget.DragTarget as DragTarget;
                        lastDragTarget.HandleDragLeave(lastDragTarget, e);
                    }

                    //if (dragTarget != null && dragTarget.SupportedFormats.ContainsAny(e.Data.GetFormats())) {
                    if (dragTarget != null) {
                        dragTarget.HandleDragEnter(dragTarget, e);
                        _lastDragOverTarget = target;
                        _dragOverIsStarted = true;
                    } else {
                        _lastDragOverTarget = null;
                        _dragOverIsStarted = false;
                        e.Effect = DragDropEffects.None;
                    }
                }

                if (dragTarget != null) {
                    dragTarget.HandleDragOver(dragTarget, e);
                }
                return true;
            } else {
                /// drag overのターゲットがなくなったとき
                if (_lastDragOverTarget != null) {
                    var lastDragTarget = _lastDragOverTarget.DragTarget as DragTarget;
                    lastDragTarget.HandleDragLeave(lastDragTarget, e);
                    _lastDragOverTarget = null;
                    _dragOverIsStarted = false;
                    e.Effect = DragDropEffects.None;
                }
                return false;
            }
        }

        public bool HandleDragDrop(DragEventArgs e) {
            var pt = new Point(e.X, e.Y);
            var target = _targetFinder(pt);

            if (target != null) {
                var dragTarget = target.DragTarget as DragTarget;
                if (dragTarget != null) {
                    dragTarget.HandleDragDrop(dragTarget, e);
                }
                _lastDragOverTarget = null;
                _dragOverIsStarted = false;
                return true;
            }
            return false;
        }

        public bool HandleDragEnter(DragEventArgs e) {
            return false;
        }

        public bool HandleDragLeave(EventArgs e) {
            if (_lastDragOverTarget != null) {
                var dragTarget = _lastDragOverTarget.DragTarget as DragTarget;
                dragTarget.HandleDragLeave(dragTarget, EventArgs.Empty);
                _lastDragOverTarget = null;
                _dragOverIsStarted = false;

                /// DragをキャンセルしてDragLeaveが呼ばれた後にMouseClickが発生するのを防ぐ
                _target = null;
                return true;
            }
            return false;
        }

        public bool HandleQueryContinueDrag(QueryContinueDragEventArgs e) {
            if (_dragOutToBeCanceledOnQueryContinueDrag) {
                e.Action = DragAction.Cancel;
                _dragOutToBeCanceledOnQueryContinueDrag = false;
                _dragOutCanceledOnQueryContinueDrag = true;
                return true;
            } else {
                _dragOutCanceledOnQueryContinueDrag = false;
                return false;
            }
        }

        private bool InMouseHoverSize(Point pt, Point last) {
            return
                (pt.X >= last.X - _hoverSize.Width) &&
                (pt.X <= last.X + _hoverSize.Width) &&
                (pt.Y >= last.Y - _hoverSize.Height) &&
                (pt.Y <= last.Y + _hoverSize.Height);
        }


        public void SetDnDTarget(IMouseOperatable target) {
            _dndEventProcessor._Target = target;
        }

    }
}
