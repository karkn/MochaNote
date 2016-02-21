/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mkamo.Control.Configurator;
using Mkamo.Common.Forms.DetailSettings;
using Mkamo.Editor.Core;
using Mkamo.Common.Command;
using Mkamo.Editor.Commands;

namespace Mkamo.Editor.Forms {
    public partial class NodeBorderDetailPage: BorderConfigurator, IDetailSettingsPage {
        // ========================================
        // field
        // ========================================
        private IEnumerable<IEditor> _targets;

        // ========================================
        // constructor
        // ========================================
        public NodeBorderDetailPage(IEnumerable<IEditor> targets) {
            InitializeComponent();
            _targets = targets;
        }

        // ========================================
        // property
        // ========================================
        public System.Windows.Forms.Control PageControl {
            get { return this; }
        }

        public bool NeedBorder {
            get { return true; }
        }

        // ========================================
        // method
        // ========================================
        public ICommand GetUpdateCommand() {
            var ret = default(ICommand);
            foreach (var editor in _targets) {
                if (ret == null) {
                    ret = new SetNodeBorderCommand(
                        editor,
                        IsBorderEnabled,
                        LineColor,
                        LineWidth,
                        LineDashStyle
                    );
                } else {
                    ret = ret.Chain(
                        new SetNodeBorderCommand(
                            editor,
                            IsBorderEnabled,
                            LineColor,
                            LineWidth,
                            LineDashStyle
                        )
                    );
                }
            }
            return ret;
        }
    }
}
