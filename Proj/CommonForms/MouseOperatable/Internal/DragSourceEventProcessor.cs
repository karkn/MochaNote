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

namespace Mkamo.Common.Forms.MouseOperatable.Internal {
    internal class DragSourceEventProcessor {
        // ========================================
        // field
        // ========================================
        private DragSource _currentProceededDragSource;
        private DragSourceEventArgs _lastCreatedDragSourceEventArgs;
        private bool _isPrepared;

        // ========================================
        // constructor
        // ========================================
        public DragSourceEventProcessor() {
            ClearDnDState();
        }

        // ========================================
        // property
        // ========================================
        public bool IsPrepared {
            get { return _isPrepared; }
        }

        // ========================================
        // method
        // ========================================
        public void PrepareDnDOnMouseDown(IMouseOperatable target, MouseEventArgs e) {
            if (target == null) {
                return;
            }
            ClearDnDState();
            var dragSource = target.DragSource as DragSource;
            if (dragSource == null) {
                return;
            }
            var dragSourceEventArgs = new DragSourceEventArgs(e);
            dragSource.HandleJudgeDragStart(target, dragSourceEventArgs);
            if (dragSourceEventArgs.DoIt) {
                _currentProceededDragSource = dragSource;
                _lastCreatedDragSourceEventArgs = dragSourceEventArgs;
                _isPrepared = true;
            }
        }

        public bool ShouldDnDOnDragOut(IDragSource dragSource, object eventSender, MouseEventArgs e) {
            ClearDnDState();
            var dragSrc = dragSource as DragSource;
            if (dragSource == null) {
                return false;
            }
            var dragSourceEventArgs = new DragSourceEventArgs(e);
            dragSrc.HandleJudgeDragStart(eventSender, dragSourceEventArgs);
            if (dragSourceEventArgs.DoIt) {
                _currentProceededDragSource = dragSrc;
                _lastCreatedDragSourceEventArgs = dragSourceEventArgs;
                _isPrepared = true;
                return true;
            } else {
                return false;
            }
        }

        public bool ShouldProcessDnDOnMouseDown(Point pt) {
            return _isPrepared && _currentProceededDragSource != null;
        }

        public bool ProcessDnD(Control control, object eventSender) {
            bool ret = false;

            _currentProceededDragSource.HandleDragStart(eventSender, _lastCreatedDragSourceEventArgs);
            _currentProceededDragSource.HandleDragSetData(eventSender, _lastCreatedDragSourceEventArgs);

            control.GiveFeedback += HandleGiveFeedback;
            control.QueryContinueDrag += HandleQueryContinueDrag;
            try {
                DragDropEffects effect = control.DoDragDrop(
                    _lastCreatedDragSourceEventArgs.DataObject,
                    _currentProceededDragSource.AllowedEffects
                );
                _lastCreatedDragSourceEventArgs.Effects = effect;
                ret = effect != DragDropEffects.None;
            } finally {
                control.GiveFeedback -= HandleGiveFeedback;
                control.QueryContinueDrag -= HandleQueryContinueDrag;
            }
            _currentProceededDragSource.HandleDragFinish(eventSender, _lastCreatedDragSourceEventArgs);

            ClearDnDState();

            return ret;
        }

        public void ClearDnDState() {
            _currentProceededDragSource = null;
            _lastCreatedDragSourceEventArgs = null;
            _isPrepared = false;
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected void HandleGiveFeedback(object sender, GiveFeedbackEventArgs e) {
            if (_currentProceededDragSource != null) {
                _currentProceededDragSource.HandleGiveFeedback(sender, e);
            }
        }

        protected void HandleQueryContinueDrag(object sender, QueryContinueDragEventArgs e) {
            if (_currentProceededDragSource != null) {
                _currentProceededDragSource.HandleQueryContinueDrag(sender, e);
            }
        }
    }
}
