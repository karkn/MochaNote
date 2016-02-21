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
using Mkamo.Common.String;
using Mkamo.Common.Core;

namespace Mkamo.Memopad.Internal.Forms {
    internal partial class RegisterLicenseForm: Form {
        public RegisterLicenseForm() {
            InitializeComponent();

            _licenseFileTextBox.TextChanged += (se, ev) => UpdateUI();
            UpdateUI();
        }

        public string LicenseFilePath {
            get { return _licenseFileTextBox.Text; }
        }

        private void UpdateUI() {
            _registerButton.Enabled = !StringUtil.IsNullOrWhitespace(_licenseFileTextBox.Text);
        }

        private void _licenseFileButton_Click(object sender, EventArgs e) {
            using (var dialog = new OpenFileDialog()) {
                dialog.ShowHelp = true;
                dialog.Filter = "Confidante2 License File(*.cnflic2)|*.cnflic2";
                if (dialog.ShowDialog(this) == DialogResult.OK) {
                    _licenseFileTextBox.Text = dialog.FileName;
                }
            }
        }
    }
}
