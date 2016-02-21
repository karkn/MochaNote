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
using Mkamo.Model.Memo;
using Mkamo.Common.Forms.Utils;

namespace Mkamo.Memopad.Internal.Forms {
    internal partial class TagSelectForm: Form {
        // ========================================
        // field
        // ========================================
        private MemopadApplication _facade;

        // ========================================
        // constructor
        // ========================================
        public TagSelectForm() {
            _facade = MemopadApplication.Instance;

            InitializeComponent();

            var theme = _facade.Theme;
            Font = theme.CaptionFont;

            _anyRadioButton.Checked = true;
            _allRadioButton.Checked = false;

            _tagTree.IsUntaggedEnabled = true;
            _tagTree.IsAllTagsEnabled = false;
            _tagTree.UpdateTags();
            _tagTree.TagRootNode.ExpandAll();

            _tagTreePanel.Paint += (ps, pe) => {
                ControlPaintUtil.DrawBorder(
                    pe.Graphics, _tagTree, SystemColors.ControlDark, _tagTree.BackColor, 1
                );
            };
        }


        // ========================================
        // event
        // ========================================
        public event EventHandler OkButtonClicked;
        public event EventHandler CancelButtonClicked;


        // ========================================
        // property
        // ========================================
        public IEnumerable<MemoTag> CheckedTags {
            get { return _tagTree.CheckedTags; }
            set { _tagTree.CheckedTags = value; }
        }

        public bool IsUntaggedChecked {
            get { return _tagTree.IsUntaggedChecked; }
            set { _tagTree.IsUntaggedChecked = value; }
        }

        public bool IsAnyChecked {
            get { return _anyRadioButton.Checked; }
            set {
                _anyRadioButton.Checked = value;
                _allRadioButton.Checked = !value;
            }
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
