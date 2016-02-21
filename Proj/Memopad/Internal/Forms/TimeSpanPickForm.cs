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
using Mkamo.Memopad.Internal.Controls;
using Mkamo.Memopad.Internal.Core;

namespace Mkamo.Memopad.Internal.Forms {
    internal partial class TimeSpanPickForm: Form {
        // ========================================
        // field
        // ========================================
        private MemopadApplication _facade;

        // ========================================
        // constructor
        // ========================================
        public TimeSpanPickForm() {
            _facade = MemopadApplication.Instance;

            InitializeComponent();

            var theme = _facade.Theme;
            Font = theme.CaptionFont;
            _okButton.Font = theme.CaptionFont;
            _cancelButton.Font = theme.CaptionFont;
            _timeSpanPicker.CaptionFont = theme.CaptionFont;
            _timeSpanPicker.InputFont = theme.InputFont;
        }

        // ========================================
        // event
        // ========================================
        public event EventHandler OkButtonClicked;
        public event EventHandler CancelButtonClicked;

        // ========================================
        // property
        // ========================================
        public TimeSpanPicker TimeSpanPicker {
            get { return _timeSpanPicker; }
        }

        // ========================================
        // method
        // ========================================
        protected virtual void OnOkButtonClicked() {
            var handler = OkButtonClicked;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnCancelButtonClicked() {
            var handler = CancelButtonClicked;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        private void _okButton_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.OK;
            OnOkButtonClicked();
            Close();
        }

        private void _cancelButton_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            OnCancelButtonClicked();
            Close();
        }
    }
}
