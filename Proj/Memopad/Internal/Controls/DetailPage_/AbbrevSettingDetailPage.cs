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
using Mkamo.Memopad.Internal.Core;
using Mkamo.Common.Command;

namespace Mkamo.Memopad.Internal.Controls {
    internal partial class AbbrevSettingDetailPage: UserControl, IDetailSettingsPage {
        private ITheme _theme;
        private bool _isModified;

        private AbbrevWordPersister _abbrevWordPersister;

        public AbbrevSettingDetailPage(AbbrevWordPersister abbrevWordPersister) {
            InitializeComponent();

            _messageLabel.Text = @"
一行に一単語を入力してください。
空行や長さが1文字以下の行は
単語として登録されません。";

            _abbrevWordPersister = abbrevWordPersister;
            _wordsTextBox.Text = _abbrevWordPersister.ToText();
            _wordsTextBox.TextChanged += HandleControlValueChanged;
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
        public Common.Command.ICommand GetUpdateCommand() {
            return new DelegatingCommand(
                () => {
                    _abbrevWordPersister.FromText(_wordsTextBox.Text);
                }
            );
        }

        private void HandleControlValueChanged(object sender, EventArgs e) {
            _isModified = true;
        }
    }
}
