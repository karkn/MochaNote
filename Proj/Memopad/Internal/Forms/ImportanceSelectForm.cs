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
using ComponentFactory.Krypton.Toolkit;
using Mkamo.Model.Memo;
using Mkamo.Memopad.Properties;

namespace Mkamo.Memopad.Internal.Forms {
    internal partial class ImportanceSelectForm: Form {
        // ========================================
        // field
        // ========================================
        private MemopadApplication _app;

        // ========================================
        // constructor
        // ========================================
        public ImportanceSelectForm() {
            InitializeComponent();

            _app = MemopadApplication.Instance;

            var theme = _app.Theme;
            Font = theme.CaptionFont;
            _importanceCheckedListBox.StateCommon.Border.Color1 = SystemColors.ControlDark;

            InitCheckListBox();
        }

        private void InitCheckListBox() {
            {
                var item = new KryptonListItem();
                item.ShortText = "重要度高";
                item.Tag = MemoImportanceKind.High;
                item.Image = Resources.sticky_note_important;
                _importanceCheckedListBox.Items.Add(item);
            }

            {
                var item = new KryptonListItem();
                item.ShortText = "重要度普通";
                item.Tag = MemoImportanceKind.Normal;
                item.Image = Resources.sticky_note;
                _importanceCheckedListBox.Items.Add(item);
            }

            {
                var item = new KryptonListItem();
                item.ShortText = "重要度低";
                item.Tag = MemoImportanceKind.Low;
                item.Image = Resources.sticky_note_unimportant;
                _importanceCheckedListBox.Items.Add(item);
            }
        }

        // ========================================
        // event
        // ========================================
        public event EventHandler OkButtonClicked;
        public event EventHandler CancelButtonClicked;

        // ========================================
        // property
        // ========================================
        public IEnumerable<MemoImportanceKind> CheckedImportanceKinds {
            get {
                var ret = new List<MemoImportanceKind>();

                foreach (KryptonListItem item in _importanceCheckedListBox.CheckedItems) {
                    var imp = (MemoImportanceKind) item.Tag;
                    ret.Add(imp);
                }

                return ret;
            }
            set {
                if (value == null) {
                    return;
                }

                _importanceCheckedListBox.SetItemChecked(0, value.Contains(MemoImportanceKind.High));
                _importanceCheckedListBox.SetItemChecked(1, value.Contains(MemoImportanceKind.Normal));
                _importanceCheckedListBox.SetItemChecked(2, value.Contains(MemoImportanceKind.Low));
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
