/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using Mkamo.Editor.Internal.Core;
using Mkamo.Common.Command;
using Mkamo.Common.Forms.Input;

namespace Mkamo.Editor.Core {
    public interface IEditorSite {
        // ========================================
        // property
        // ========================================
        bool SuppressUpdateHandleLayer { get; set; }

        EditorCanvas EditorCanvas { get; }

        IControllerFactory ControllerFactory { get; }

        ICommandExecutor CommandExecutor { get; }

        ISelectionManager SelectionManager { get; }
        IFocusManager FocusManager { get; }

        //IEditorDataAggregatorManager EditorDataAggregatorManager { get; }
        IEditorCopyExtenderManager EditorCopyExtenderManager { get; }

        IGridService GridService { get; }

        IFigure PrimaryLayer { get; }
        IFigure HandleLayer { get; }
        IFigure ShowOnPointHandleLayer { get; }
        IFigure FeedbackLayer { get; }
        IFigure FocusLayer { get; }

        IDirtManager DirtManager { get; }
        Caret Caret { get; }

        // ========================================
        // method
        // ========================================
        void UpdateHandleLayer();
        void UpdateFocusLayer();
    }
}
