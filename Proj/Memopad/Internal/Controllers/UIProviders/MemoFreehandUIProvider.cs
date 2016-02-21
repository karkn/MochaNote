/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Mkamo.Memopad.Internal.Controllers.UIProviders {
    internal class MemoFreehandUIProvider: AbstractMemoContentUIProvider {

        // ========================================
        // constructor
        // ========================================
        public MemoFreehandUIProvider(MemoFreehandController owner): base(owner, false) {
        }

        // ========================================
        // method
        // ========================================
        public override ContextMenuStrip GetContextMenu(MouseEventArgs e) {
            _ContextMenu.Items.Clear();

            _ContextMenu.Items.Add(_CutInNewMemo);
            return _ContextMenu;
        }
    }
}
