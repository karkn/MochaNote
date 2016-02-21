/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using System.Windows.Forms;
using Mkamo.Common.Command;
using Mkamo.Common.Forms.DetailSettings;

namespace Mkamo.Editor.Controllers {
    using StyledText = Mkamo.StyledText.Core.StyledText;
using Mkamo.Common.Core;

    public abstract class AbstractUIProvider: AbstractContextMenuProvider, IUIProvider {
        // ========================================
        // field
        // ========================================
        private bool _supportDetailForm;
        private Lazy<ToolStripDropDown> _miniToolBar;

        // ========================================
        // constructor
        // ========================================
        protected AbstractUIProvider(): this(false) {
        }

        protected AbstractUIProvider(bool supportDetailForm) {
            _supportDetailForm = supportDetailForm;

            _miniToolBar = new Lazy<ToolStripDropDown>(() => new ToolStripDropDown());
        }

        // ========================================
        // property
        // ========================================
        public bool SupportDetailForm {
            get { return _supportDetailForm; }
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected bool _SupportDetailForm {
            get { return _supportDetailForm; }
            set { _supportDetailForm = value; }
        }

        protected ToolStripDropDown _MiniToolBar {
            get { return _miniToolBar.Value; }
        }

        // ========================================
        // method
        // ========================================
        public virtual void ConfigureDetailForm(DetailSettingsForm detailForm) {
        }

        public virtual StyledText GetStyledText() {
            return null;
        }

        public virtual ToolStripDropDown GetMiniToolBar(MouseEventArgs e) {
            return _miniToolBar.IsValueCreated? _miniToolBar.Value: null;
        }
    }
}
