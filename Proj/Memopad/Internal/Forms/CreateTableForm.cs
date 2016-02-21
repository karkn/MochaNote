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
using Mkamo.Memopad.Internal.Core;

namespace Mkamo.Memopad.Internal.Forms {
    internal partial class CreateTableForm: Form {
        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================
        public CreateTableForm() {
            InitializeComponent();

            var facade = MemopadApplication.Instance;
            var theme = facade.Theme;
            Font = theme.CaptionFont;
            _okButton.Font = theme.CaptionFont;
            _cancelButton.Font = theme.CaptionFont;
            _rowCountLabel.Font = theme.CaptionFont;
            _columnCountLabel.Font = theme.CaptionFont;

            _rowCountComboBox.Font = theme.InputFont;
            _columnCountComboBox.Font = theme.InputFont;

            _columnCountComboBox.SelectedIndex = 4;
            _rowCountComboBox.SelectedIndex = 3;
        }

        // ========================================
        // property
        // ========================================
        public int RowCount {
            get { return int.Parse(_rowCountComboBox.Text); }
        }

        public int ColumnCount {
            get { return int.Parse(_columnCountComboBox.Text); }
        }

        // ========================================
        // method
        // ========================================
        private void _okButton_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void _cancelButton_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }


    }
}
