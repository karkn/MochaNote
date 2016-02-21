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
using System.IO;
using Mkamo.Memopad.Internal.Utils;
using Mkamo.Common.Core;

namespace Mkamo.Memopad.Internal.Controls {
    [ToolboxItem(false)]
    internal partial class BackgroundSettingsDetailPage: UserControl, IDetailSettingsPage {
        // ========================================
        // field
        // ========================================
        private ITheme _theme;
        private bool _isModified;
        private MemopadSettings _settings;
        private MemopadWindowSettings _windowSettings;

        // ========================================
        // constructor
        // ========================================
        public BackgroundSettingsDetailPage(MemopadSettings settings, MemopadWindowSettings windowSettings) {
            InitializeComponent();

            _settings = settings;
            _windowSettings = windowSettings;

            _enableBackgroundImageCheckBox.Checked = _windowSettings.MemoEditorBackgroundImageEnabled;
            _imageFileTextBox.Text = _windowSettings.MemoEditorBackgroundImageFilePath;
            _opacityTrackBar.Value = _windowSettings.MemoEditorBackgroundImageOpacityPercent;
            _scaleTrackBar.Value = _windowSettings.MemoEditorBackgroundImageScalePercent;

            _enableBackgroundImageCheckBox.CheckedChanged += HandleEnableBackgroundImageCheckBoxCheckedChanged;
            _imageFileTextBox.TextChanged += HandleValueChanged;
            _opacityTrackBar.ValueChanged += HandleOpacityTrackBarValueChanged;
            _scaleTrackBar.ValueChanged += HandleScaleTrackBarValueChanged;

            _isModified = false;
            UpdateUI();
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
                Font = captionFont;
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
                    _windowSettings.MemoEditorBackgroundImageEnabled = _enableBackgroundImageCheckBox.Checked;
                    _windowSettings.MemoEditorBackgroundImageFilePath = _imageFileTextBox.Text;
                    _windowSettings.MemoEditorBackgroundImageOpacityPercent = _opacityTrackBar.Value;
                    _windowSettings.MemoEditorBackgroundImageScalePercent = _scaleTrackBar.Value;
                }
            );
        }

        private void UpdateUI() {
            var enabled = _enableBackgroundImageCheckBox.Checked;
            _imageFileTextBox.Enabled = enabled;
            _imageFileButton.Enabled = enabled;
            _opacityTrackBar.Enabled = enabled;
            _scaleTrackBar.Enabled = enabled;
            _opacityValueLabel.Text = _opacityTrackBar.Value + "%";
            _scaleValueLabel.Text = _scaleTrackBar.Value + "%";
        }

        private void HandleEnableBackgroundImageCheckBoxCheckedChanged(object sender, EventArgs e) {
            UpdateUI();
            _isModified = true;
        }

        private void HandleOpacityTrackBarValueChanged(object sender, EventArgs e) {
            UpdateUI();
            _isModified = true;
        }

        private void HandleScaleTrackBarValueChanged(object sender, EventArgs e) {
            UpdateUI();
            _isModified = true;
        }

        private void HandleValueChanged(object sender, EventArgs e) {
            _isModified = true;
        }

        private void _imageFileButton_Click(object sender, EventArgs e) {
            var dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.RestoreDirectory = true;
            dialog.ShowHelp = true;
            dialog.Filter = "Image Files(*.bmp;*.jpg;*.gif;*.png)|*.bmp;*.jpg;*.gif;*.png";
            dialog.FileName = _imageFileTextBox.Text;
            if (dialog.ShowDialog(this) == DialogResult.OK) {
                _imageFileTextBox.Text = dialog.FileName;
            }
            dialog.Dispose();
        }
    }
}
