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
    internal partial class ProgressForm: Form {
        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================
        public ProgressForm() {
            InitializeComponent();
        }

        // ========================================
        // property
        // ========================================
        public int Progress {
            get { return _recoverProgressBar.Value; }
            set { _recoverProgressBar.Value = value; }
        }

        public int ProgressMax {
            get { return _recoverProgressBar.Maximum; }
            set { _recoverProgressBar.Maximum = value; }
        }

        public string Message {
            get { return _messageLabel.Text; }
            set { _messageLabel.Text = value; }
        }

        // ========================================
        // method
        // ========================================

    }
}
