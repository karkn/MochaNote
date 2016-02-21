/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using Mkamo.Common.Externalize;
using System.Windows.Forms;

namespace Mkamo.Editor.Core {
    public interface IController {
        // ========================================
        // property
        // ========================================
        IModelDescriptor ModelDescriptor { get; }
        IUIProvider UIProvider { get; }
        bool NeedAdjustSizeOnPrint { get; }

        // ========================================
        // method
        // ========================================
        // --- install ---
        /// <summary>
        /// インストール時の処理を行う
        /// </summary>
        void Installed(IEditor host);

        /// <summary>
        /// アンインストール時の処理を行う
        /// </summary>
        void Uninstalled(IEditor host);

        // --- activate ---
        void Activate();
        void Deactivate();

        // --- editor ---
        void ConfigureEditor(IEditor editor);

        // --- refresh ---
        void RefreshEditor(RefreshContext context, IFigure figure, object model);

        // --- figure ---
        IFigure CreateFigure(object model);

        // --- model lifecycle ---
        void DisposeModel(object model);
        void RestoreModel(object model);

        // --- externalize ---
        IMemento GetModelMemento();
        TransferInitializer GetTransferInitializer();
        object GetTranserInitArgs();

        // --- text ---
        string GetText();
    }
}
