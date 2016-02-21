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
    internal partial class CreateMemoForm: Form {
        // ========================================
        // type
        // ========================================
        internal enum OriginalModificationKind {
            None,
            Remove,
            ReplaceWithLink,
        }

        // ========================================
        // constructor
        // ========================================
        public CreateMemoForm() {
            InitializeComponent();
            UpdateUI();
        }

        // ========================================
        // property
        // ========================================
        public OriginalModificationKind OriginalModification {
            get {
                if (_doNothingRadioButton.Checked) {
                    return OriginalModificationKind.None;
                } else if (_removeSelectionRadioButton.Checked) {
                    return OriginalModificationKind.Remove;
                } else if (_replaceWithLinkRadioButton.Checked) {
                    return OriginalModificationKind.ReplaceWithLink;
                }
                return OriginalModificationKind.None;
            }
        }

        public string MemoTitle {
            get { return _titleTextBox.Text; }
            set { _titleTextBox.Text = value; }
        }

        public RadioButton ReplaceWithLinkRadioButton {
            get { return _replaceWithLinkRadioButton; }
        }

        // ========================================
        // method
        // ========================================
        private void UpdateUI() {
            _createButton.Enabled = !StringUtil.IsNullOrWhitespace(_titleTextBox.Text);
        }

        private void _titleTextBox_TextChanged(object sender, EventArgs e) {
            UpdateUI();
        }

        //private void _createButton_Click(object sender, EventArgs e) {
        //    Close();
        //}

        //private void _cancelButton_Click(object sender, EventArgs e) {
        //    Close();
        //}


    }
}
