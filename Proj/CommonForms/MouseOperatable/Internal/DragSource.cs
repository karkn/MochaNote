/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Mkamo.Common.Forms.MouseOperatable.Internal {
    internal class DragSource: IDragSource {
        // ========================================
        // field
        // ========================================
        private DragDropEffects _allowedOperations;

        // ========================================
        // constructor
        // ========================================
        public DragSource(DragDropEffects allowedOperations) {
            _allowedOperations = allowedOperations;
        }

        // ========================================
        // event
        // ========================================
        public event EventHandler<DragSourceEventArgs> JudgeDragStart;
        public event EventHandler<DragSourceEventArgs> DragSetData;
        public event EventHandler<DragSourceEventArgs> DragStart;
        public event EventHandler<DragSourceEventArgs> DragFinish;
        public event GiveFeedbackEventHandler GiveFeedback;
        public event QueryContinueDragEventHandler QueryContinueDrag;

        // ========================================
        // property
        // ========================================
        public DragDropEffects AllowedEffects {
            get { return _allowedOperations; }
        }

        // ========================================
        // method
        // ========================================
        public void HandleJudgeDragStart(object sender, DragSourceEventArgs e) {
            var handler = JudgeDragStart;
            if (handler != null) {
                handler(sender, e);
            }
        }

        public void HandleDragSetData(object sender, DragSourceEventArgs e) {
            var handler = DragSetData;
            if (handler != null) {
                handler(sender, e);
            }
        }

        public void HandleDragStart(object sender, DragSourceEventArgs e) {
            var handler = DragStart;
            if (handler != null) {
                handler(sender, e);
            }
        }

        public void HandleDragFinish(object sender, DragSourceEventArgs e) {
            var handler = DragFinish;
            if (handler != null) {
                handler(sender, e);
            }
        }

        public void HandleGiveFeedback(object sender, GiveFeedbackEventArgs e) {
            var handler = GiveFeedback;
            if (handler != null) {
                handler(sender, e);
            }
        }

        public void HandleQueryContinueDrag(object sender, QueryContinueDragEventArgs e) {
            var handler = QueryContinueDrag;
            if (handler != null) {
                handler(sender, e);
            }
        }


    }
}
