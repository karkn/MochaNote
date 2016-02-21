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
using Mkamo.Common.Forms.Themes;

namespace Mkamo.Common.Forms.KeyMap {
    public partial class KeyBinderDetailSettingsPage: UserControl, IDetailSettingsPage {
        // ========================================
        // field
        // ========================================
        private bool _isModified = false;

        private ITheme _theme;

        // ========================================
        // constructor
        // ========================================
        public KeyBinderDetailSettingsPage() {
            InitializeComponent();
        }

        // ========================================
        // property
        // ========================================
        public Control PageControl {
            get { return this; }
        }

        public bool NeedBorder {
            get { return true; }
        }

        public bool IsModified {
            get { return _isModified; }
            set { _isModified = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ITheme Theme {
            get { return _theme; }
            set {
                if (value == _theme) {
                    return;
                }
                _theme = value;
                Font = _theme.CaptionFont;
            }
        }
        // ========================================
        // method
        // ========================================
        private void HandleControlValueChanged(object sender, EventArgs e) {
            _isModified = true;
        }


        public Command.ICommand GetUpdateCommand() {
            throw new NotImplementedException();
        }
    }
}
