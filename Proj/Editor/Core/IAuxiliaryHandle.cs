/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using Mkamo.StyledText.Core;
using System.Windows.Forms;

namespace Mkamo.Editor.Core {
    public interface IAuxiliaryHandle: IHandle {
        // ========================================
        // property
        // ========================================
        IFigure Figure { get; }
        bool HideOnFocus { get; }

        // ========================================
        // method
        // ========================================
        void Relocate(IFigure hostFigure);
    }

}
