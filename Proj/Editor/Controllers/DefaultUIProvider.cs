/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Forms.DetailSettings;
using System.Windows.Forms;
using Core = Mkamo.StyledText.Core;

namespace Mkamo.Editor.Controllers {
    public class DefaultUIProvider: AbstractUIProvider {
        // ========================================
        // field
        // ========================================
        private Action<DetailSettingsForm> _detailFormConfigurator;

        // ========================================
        // constructor
        // ========================================
        public DefaultUIProvider(): base() {
        }

        public DefaultUIProvider(
            Action<DetailSettingsForm> detailFormConfigurator
        ): base(
            detailFormConfigurator != null
        ) {
            _detailFormConfigurator = detailFormConfigurator;
        }

        // ========================================
        // property
        // ========================================
        
        // ========================================
        // method
        // ========================================
        public override void ConfigureDetailForm(DetailSettingsForm detailForm) {
            if (_detailFormConfigurator != null) {
                _detailFormConfigurator(detailForm);
            }
        }

        public override ContextMenuStrip GetContextMenu(MouseEventArgs e) {
            return null;
        }
    }
}
