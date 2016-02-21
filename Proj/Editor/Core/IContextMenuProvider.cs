/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Mkamo.Editor.Core {
    public interface IContextMenuProvider: IDisposable {
        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        /// <summary>
        /// 右クリックしたときに表示するコンテキストメニューの項目群を返す．
        /// </summary>
        ContextMenuStrip GetContextMenu(MouseEventArgs e);

    }
}
