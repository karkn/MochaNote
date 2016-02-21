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
using Mkamo.Common.Forms.Themes;
using Mkamo.Memopad.Core;

namespace Mkamo.Memopad.Internal.Controls {
    [ToolboxItem(false)]
    internal partial class FolderSettingsDetailPage: UserControl, IDetailSettingsPage {
        // ========================================
        // field
        // ========================================
        private ITheme _theme;
        private bool _isModified;
        private BootstrapSettings _bootstrapSettings;
        private MemopadWindowSettings _windowSettings;

        // ========================================
        // constructor
        // ========================================
        public FolderSettingsDetailPage(BootstrapSettings bootstrapSettings, MemopadWindowSettings windowSettings) {
            InitializeComponent();

            _bootstrapSettings = bootstrapSettings;
            _windowSettings = windowSettings;

            _memoRootTextBox.Text = _bootstrapSettings.MemoRoot;
            _backupRootTextBox.Text = _windowSettings.BackupRoot;

            _memoRootTextBox.TextChanged += HandleTextBoxTextChanged;
            _backupRootTextBox.TextChanged += HandleTextBoxTextChanged;

            _isModified = false;

            if (MemopadConsts.UseCommandLineMemoRoot) {
                _memoRootLabel.Enabled = false;
                _memoRootTextBox.Enabled = false;
                _memoRootButton.Enabled = false;
            }
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

                Font = value.CaptionFont;
                //var captionFont = value.CaptionFont;
                //_memoRootLabel.Font = captionFont;
                //_memoRootButton.Font = captionFont;

                //var inputFont = value.InputFont;
                //_memoRootTextBox.Font = inputFont;
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
                    _bootstrapSettings.MemoRoot = _memoRootTextBox.Text;
                    _windowSettings.BackupRoot = _backupRootTextBox.Text;
                }
            );
        }

        private void HandleTextBoxTextChanged(object sender, EventArgs e) {
            _isModified = true;
        }

        private void _memoRootButton_Click(object sender, EventArgs e) {
            using (var dialog = new FolderBrowserDialog()) {
                dialog.Description = "ノート格納フォルダの選択";
                dialog.SelectedPath = _memoRootTextBox.Text;
                if (dialog.ShowDialog() == DialogResult.OK) {
                    _memoRootTextBox.Text = dialog.SelectedPath;
                }
            }
        }

        private void _backupRootButton_Click(object sender, EventArgs e) {
            using (var dialog = new FolderBrowserDialog()) {
                dialog.Description = "自動バックアップフォルダの選択";
                dialog.SelectedPath = _backupRootTextBox.Text;
                if (dialog.ShowDialog() == DialogResult.OK) {
                    _backupRootTextBox.Text = dialog.SelectedPath;
                }
            }
        }

    }
}
