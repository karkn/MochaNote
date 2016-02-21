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
using Mkamo.Memopad.Core;
using Mkamo.Common.Forms.Themes;
using Mkamo.Common.Command;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Control.HotKey;
using Mkamo.Memopad.Internal.Forms;
using Mkamo.Memopad.Internal.Utils;

namespace Mkamo.Memopad.Internal.Controls {
    [ToolboxItem(false)]
    internal partial class HotKeySettingsDetailPage: UserControl, IDetailSettingsPage {
        private ITheme _theme;
        private bool _isModified;

        private HotKey _hotKey;
        private MemopadWindowSettings _windowSettings;

        public HotKeySettingsDetailPage(HotKey hotKey, MemopadWindowSettings windowSettings) {
            InitializeComponent();

            _hotKey = hotKey;
            _windowSettings = windowSettings;

            _activateTextBox.Text = _windowSettings.ActivateHotKey;
            _clipMemoTextBox.Text = _windowSettings.ClipMemoHotKey;
            _captureScreenTextBox.Text = _windowSettings.CaptureScreenHotKey;
            _createMemoTextBox.Text = _windowSettings.CreateMemoHotKey;

            _activateTextBox.TextChanged += HandleKeyBindChanged;
            _clipMemoTextBox.TextChanged += HandleKeyBindChanged;
            _captureScreenTextBox.TextChanged += HandleKeyBindChanged;
            _createMemoTextBox.TextChanged += HandleKeyBindChanged;
        }

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

        public ICommand GetUpdateCommand() {
            return new DelegatingCommand(
                () => {
                    _hotKey.RemoveHotKey(_windowSettings.ActivateHotKey);
                    _windowSettings.ActivateHotKey = _activateTextBox.Text;
                    _hotKey.AddHotKey(
                        _windowSettings.ActivateHotKey,
                        HotKeyUtil.ActivateHotKeyPressed
                    );

                    _hotKey.RemoveHotKey(_windowSettings.ClipMemoHotKey);
                    _windowSettings.ClipMemoHotKey = _clipMemoTextBox.Text;
                    _hotKey.AddHotKey(
                        _windowSettings.ClipMemoHotKey,
                        HotKeyUtil.ClipMemoHotKeyPressed
                    );

                    _hotKey.RemoveHotKey(_windowSettings.CaptureScreenHotKey);
                    _windowSettings.CaptureScreenHotKey = _captureScreenTextBox.Text;
                    _hotKey.AddHotKey(
                        _windowSettings.CaptureScreenHotKey,
                        HotKeyUtil.CaptureScreenHotKeyPressed
                    );

                    _hotKey.RemoveHotKey(_windowSettings.CreateMemoHotKey);
                    _windowSettings.CreateMemoHotKey = _createMemoTextBox.Text;
                    _hotKey.AddHotKey(
                        _windowSettings.CreateMemoHotKey,
                        HotKeyUtil.CreateMemoHotKeyPressed
                    );
                }
            );
        }

        private void HandleKeyBindChanged(object sender, EventArgs e) {
            _isModified = true;
        }
    }
}
