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
using System.Diagnostics;

namespace Mkamo.Memopad.Internal.Forms {
    internal partial class EncourageUpgradeLicenseForm: Form {
        public EncourageUpgradeLicenseForm() {
            InitializeComponent();
        }

        public string Message {
            get { return _messageLabel.Text; }
            set { _messageLabel.Text = value; }
        }

        public override Font Font {
            get { return base.Font; }
            set {
                base.Font = value;
                _messageLabel.Font = new Font(Font.FontFamily, Font.SizeInPoints + 1);
            }
        }
        
        private void _premiumLicenseButton_Click(object sender, EventArgs e) {
            Close();
            Process.Start("http://www.confidante.jp/order.html");
        }

        private void _closeButton_Click(object sender, EventArgs e) {
            Close();
        }
    }
}
