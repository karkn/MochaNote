/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Mkamo.Memopad.Internal.Forms {
    internal partial class SmtpDetailSettingsForm: Form {
        // ========================================
        // constructor
        // ========================================
        public SmtpDetailSettingsForm() {
            InitializeComponent();
            UpdateAuthUI();
        }

        // ========================================
        // property
        // ========================================
        public string UserName {
            get { return _userNameTextBox.Text; }
            set { _userNameTextBox.Text = value; }
        }

        public string Password {
            get { return _passwordTextBox.Text; }
            set { _passwordTextBox.Text = value; }
        }

        public bool EnableAuth {
            get { return _enableAuthCheckBox.Checked; }
            set { _enableAuthCheckBox.Checked = value; }
        }

        public bool EnableSsl {
            get { return _enableSslCheckBox.Checked; }
            set { _enableSslCheckBox.Checked = value; }
        }

        public int Port {
            get {
                var ret = 0;
                return int.TryParse(_portTextBox.Text, out ret) ? ret : 25;
            }
            set { _portTextBox.Text = value.ToString(); }
        }


        // ========================================
        // method
        // ========================================
        private void UpdateAuthUI() {
            _userNameTextBox.Enabled = _enableAuthCheckBox.Checked;
            _userNameLabel.Enabled = _enableAuthCheckBox.Checked;
            _passwordTextBox.Enabled = _enableAuthCheckBox.Checked;
            _passwordLabel.Enabled = _enableAuthCheckBox.Checked;
        }

        private void _enableAuthCheckBox_CheckedChanged(object sender, EventArgs e) {
            UpdateAuthUI();
        }

        private void _portTextBox_Validating(object sender, CancelEventArgs e) {
            var i = 0;
            e.Cancel = !int.TryParse(_portTextBox.Text, out i);
            if (e.Cancel) {
                MessageBox.Show(this, "数値を入力してください。", "ポート入力値エラー");
            }
        }

    }
}
