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
using Mkamo.Common.Forms.Themes;

namespace Mkamo.Common.Forms.DetailSettings {
    public interface IDetailSettingsPage: IDisposable {
        // ========================================
        // property
        // ========================================
        Control PageControl { get; }
        bool NeedBorder { get; }
        ITheme Theme { get; set; }

        bool IsModified { get; }

        // ========================================
        // method
        // ========================================
        ICommand GetUpdateCommand();
    }
}
