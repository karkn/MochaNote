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
using Mkamo.Common.Forms.DetailSettings;
using Mkamo.Common.Command;
using Mkamo.Control.Configurator;
using Mkamo.Editor.Commands;
using Mkamo.Editor.Core;

namespace Mkamo.Editor.Forms {
    public partial class NodeBackgroundDetailPage: BackgroundConfigurator, IDetailSettingsPage {

        // ========================================
        // field
        // ========================================
        private IEnumerable<IEditor> _targets;

        // ========================================
        // constructor
        // ========================================
        public NodeBackgroundDetailPage(IEnumerable<IEditor> targets) {
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
                    ret = new SetNodeBackgroundCommand(editor, Background);
                } else {
                    ret = ret.Chain(new SetNodeBackgroundCommand(editor, Background));
                }
            }
            return ret;
        }
    }
}
