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

namespace Mkamo.Control.Progress {
    public partial class ProgressDialog: Form {
        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================
        public ProgressDialog() {
            InitializeComponent();

            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            SupportCancel = false;
        }

        // ========================================
        // property
        // ========================================
        public BackgroundWorker BackgroundWorker {
            get { return _backgroundWorker; }
        }

        public bool SupportCancel {
            get { return _backgroundWorker.WorkerSupportsCancellation; }
            set {
                _backgroundWorker.WorkerSupportsCancellation = value;
                _cancelButton.Enabled = value;
            }
        }

        public string Message {
            get { return _messageLabel.Text; }
            set { _messageLabel.Text = value; }
        }

        // ========================================
        // method
        // ========================================
        public void Run(Form owner, object args) {
            _backgroundWorker.RunWorkerAsync(args);
            ShowDialog(owner);
        }

        private void _backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            _progressBar.Value = e.ProgressPercentage;
        }

        private void _cancelButton_Click(object sender, EventArgs e) {
            _backgroundWorker.CancelAsync();
        }
    }
}
