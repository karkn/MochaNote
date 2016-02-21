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
    internal partial class MiscSettingsDetailPage: UserControl, IDetailSettingsPage {
        // ========================================
        // field
        // ========================================
        private ITheme _theme;
        private bool _isModified;
        private MemopadSettings _settings;
        private MemopadWindowSettings _windowSettings;

        private bool _isWinXP;

        // ========================================
        // constructor
        // ========================================
        public MiscSettingsDetailPage(MemopadSettings settings, MemopadWindowSettings windowSettings) {
            InitializeComponent();

            _isWinXP = EnvironmentUtil.IsWinXP();

            _settings = settings;
            _windowSettings = windowSettings;

            _emptyTrasBoxCheckBox.Checked = _settings.EmptyTrashBoxOnExit;
            _showStartPageOnStartCheckBox.Checked = _windowSettings.ShowStartPageOnStart;
            _checkLatestOnStartCheckBox.Checked = _settings.CheckLatestOnStart;
            _minimizeToTaskTrayCheckBox.Checked = _windowSettings.MinimizeToTaskTray;
            _minimizeOnStartUpCheckBox.Checked = _windowSettings.MinimizeOnStartUp;
            if (_isWinXP) {
                _startOnWindowsStartUpCheckBox.Checked = File.Exists(MemopadConsts.StartUpShortcutFilePath);
            } else {
                _startOnWindowsStartUpCheckBox.Checked = false;
                _startOnWindowsStartUpCheckBox.Visible = false;
            }
            _replaceMeiryoCheckBox.Checked = _windowSettings.ReplaceMeiryoWithMeiryoUI;

            _emptyTrasBoxCheckBox.CheckedChanged += HandleCheckBoxCheckedChanged;
            _showStartPageOnStartCheckBox.CheckedChanged += HandleCheckBoxCheckedChanged;
            _checkLatestOnStartCheckBox.CheckedChanged += HandleCheckBoxCheckedChanged;
            _minimizeToTaskTrayCheckBox.CheckedChanged += HandleCheckBoxCheckedChanged;
            _minimizeOnStartUpCheckBox.CheckedChanged += HandleCheckBoxCheckedChanged;
            if (_isWinXP) {
                _startOnWindowsStartUpCheckBox.CheckedChanged += HandleCheckBoxCheckedChanged;
            }
            _replaceMeiryoCheckBox.CheckedChanged += HandleCheckBoxCheckedChanged;

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
                Font = value.CaptionFont;
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
                    _settings.EmptyTrashBoxOnExit = _emptyTrasBoxCheckBox.Checked;
                    _windowSettings.ShowStartPageOnStart = _showStartPageOnStartCheckBox.Checked;
                    _settings.CheckLatestOnStart = _checkLatestOnStartCheckBox.Checked;
                    _windowSettings.MinimizeToTaskTray = _minimizeToTaskTrayCheckBox.Checked;
                    _windowSettings.MinimizeOnStartUp = _minimizeOnStartUpCheckBox.Checked;
                    _windowSettings.ReplaceMeiryoWithMeiryoUI = _replaceMeiryoCheckBox.Checked;

                    if (_isWinXP) {
                        var shortcutPath = MemopadConsts.StartUpShortcutFilePath;
                        if (_startOnWindowsStartUpCheckBox.Checked) {
                            if (!File.Exists(shortcutPath) && !Directory.Exists(shortcutPath)) {
                                ShortcutUtil.CreateShortcut(shortcutPath, Application.ExecutablePath);
                            }
                        } else {
                            if (File.Exists(shortcutPath)) {
                                File.Delete(shortcutPath);
                            }
                        }
                    }
                }
            );
        }

        private void HandleCheckBoxCheckedChanged(object sender, EventArgs e) {
            _isModified = true;
            UpdateUI();
        }

        private void UpdateUI() {
            _minimizeOnStartUpCheckBox.Enabled = _minimizeToTaskTrayCheckBox.Checked;
        }
    }
}
