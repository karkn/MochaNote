/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mkamo.Common.Command;
using Mkamo.Common.Forms.DetailSettings;
using STCore = Mkamo.StyledText.Core;

namespace Mkamo.Editor.Core {
    public interface IUIProvider: IContextMenuProvider {
        // ========================================
        // property
        // ========================================
        bool SupportDetailForm { get; }

        //bool CanModifyHorizontalAlignment { get; }
        //bool CanModifyVerticalAlignment{ get; }

        // ========================================
        // method
        // ========================================
        void ConfigureDetailForm(DetailSettingsForm detailForm);

        /// <summary>
        /// 右クリックしたときに表示するミニツールバーを返す。
        /// </summary>
        ToolStripDropDown GetMiniToolBar(MouseEventArgs e);
    }
}
