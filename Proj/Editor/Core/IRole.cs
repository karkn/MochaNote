/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using Mkamo.Common.Command;

namespace Mkamo.Editor.Core {
    /// <summary>
    /// IEditorに設定可能な役割を表す．
    /// requestに対してcommandやfeedbackを生成する．
    /// </summary>
    public interface IRole {
        void Installed(IEditor host);
        void Uninstalled(IEditor host);

        bool CanUnderstand(IRequest request);
        ICommand CreateCommand(IRequest request);

        IFigure CreateFeedback(IRequest request);
        void UpdateFeedback(IRequest request, IFigure feedback);
        void DisposeFeedback(IRequest request, IFigure feedback);
    }
}
