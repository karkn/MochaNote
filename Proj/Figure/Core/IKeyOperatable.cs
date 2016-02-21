/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Mkamo.Figure.Core {
    public interface IKeyOperatable {
        void HandleShortcutKeyProcess(ShortcutKeyProcessEventArgs e);

        void HandleKeyDown(KeyEventArgs e);
        void HandleKeyUp(KeyEventArgs e);
        void HandleKeyPress(KeyPressEventArgs e);
        void HandlePreviewKeyDown(PreviewKeyDownEventArgs e);
    }
}
