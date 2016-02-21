/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Mkamo.Common.Forms.MouseOperatable {
    public interface IDragSource {
        // ========================================
        // event
        // ========================================
        event EventHandler<DragSourceEventArgs> JudgeDragStart;
        event EventHandler<DragSourceEventArgs> DragSetData;
        event EventHandler<DragSourceEventArgs> DragStart;
        event EventHandler<DragSourceEventArgs> DragFinish;
        event GiveFeedbackEventHandler GiveFeedback;
        event QueryContinueDragEventHandler QueryContinueDrag;

        // ========================================
        // property
        // ========================================
        DragDropEffects AllowedEffects { get; }
        //string[] SupportedFormats { get; }
    }
}
