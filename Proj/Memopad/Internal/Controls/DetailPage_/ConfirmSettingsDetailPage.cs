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
using Mkamo.Memopad.Core;
using Mkamo.Common.Command;

namespace Mkamo.Memopad.Internal.Controls {
    [ToolboxItem(false)]
    internal partial class ConfirmSettingsDetailPage: UserControl, IDetailSettingsPage {
        // ========================================
        // field
        // ========================================
        private ITheme _theme;
        private bool _isModified;
        private MemopadSettings _settings;

        // ========================================
        // constructor
        // ========================================
        public ConfirmSettingsDetailPage(MemopadSettings settings) {
            InitializeComponent();

            _settings = settings;

            _confirmMemoRemoveCheckBox.Checked = _settings.ConfirmMemoRemoval;
            _confirmTagRemoveCheckBox.Checked = _settings.ConfirmTagRemoval;
            _confirmSmartFolderRemoveCheckBox.Checked = _settings.ConfirmSmartFolderRemoval;
            _confirmFolderRemoveCheckBox.Checked = _settings.ConfirmFolderRemoval;

            _confirmMemoRemoveCheckBox.CheckedChanged += HandleCheckBoxCheckedChanged;
            _confirmTagRemoveCheckBox.CheckedChanged += HandleCheckBoxCheckedChanged;
            _confirmSmartFolderRemoveCheckBox.CheckedChanged += HandleCheckBoxCheckedChanged;
            _confirmFolderRemoveCheckBox.CheckedChanged += HandleCheckBoxCheckedChanged;

            _isModified = false;
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

        public ITheme Theme {
            get { return _theme; }
            set {
                if (value == _theme) {
                    return;
                }
                _theme = value;

                var captionFont = value.CaptionFont;
                _confirmMemoRemoveCheckBox.Font = captionFont;
                _confirmTagRemoveCheckBox.Font = captionFont;
            }
        }

        public bool IsModified {
            get { return _isModified; }
        }

        // ========================================
        // method
        // ========================================
        public Mkamo.Common.Command.ICommand GetUpdateCommand() {
            return new DelegatingCommand(
                () => {
                    _settings.ConfirmMemoRemoval = _confirmMemoRemoveCheckBox.Checked;
                    _settings.ConfirmTagRemoval = _confirmTagRemoveCheckBox.Checked;
                    _settings.ConfirmSmartFolderRemoval = _confirmSmartFolderRemoveCheckBox.Checked;
                    _settings.ConfirmFolderRemoval = _confirmFolderRemoveCheckBox.Checked;
                }
            );
        }

        private void HandleCheckBoxCheckedChanged(object sender, EventArgs e) {
            _isModified = true;
        }
    }
}
