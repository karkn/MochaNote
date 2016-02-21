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
using Mkamo.Memopad.Internal.Utils;
using ComponentFactory.Krypton.Toolkit;

namespace Mkamo.Memopad.Internal.Forms {
    internal partial class MarkSelectForm: Form {
        // ========================================
        // field
        // ========================================
        private MemopadApplication _facade;

        // ========================================
        // constructor
        // ========================================
        public MarkSelectForm() {
            InitializeComponent();

            _facade = MemopadApplication.Instance;

            var theme = _facade.Theme;
            Font = theme.CaptionFont;
            _markDefinitionCheckedListBox.StateCommon.Border.Color1 = SystemColors.ControlDark;

            InitMarkDefCheckListBox();
        }

        private void InitMarkDefCheckListBox() {
            var defs = MemoMarkUtil.GetMemoMarkDefinitions();
            foreach (var def in defs) {
                var item = new KryptonListItem();
                item.ShortText = def.Name;
                item.Image = def.Image;
                item.Tag = def;
                _markDefinitionCheckedListBox.Items.Add(item);
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
        public IEnumerable<MemoMarkDefinition> CheckedMarkDefinitions {
            get {
                var ret = new List<MemoMarkDefinition>();

                foreach (KryptonListItem item in _markDefinitionCheckedListBox.CheckedItems) {
                    var mark = item.Tag as MemoMarkDefinition;
                    if (mark != null) {
                        ret.Add(mark);
                    }
                }

                return ret;
            }
            set {
                if (value == null) {
                    return;
                }

                for (int i = 0, len = _markDefinitionCheckedListBox.Items.Count; i < len; ++i) {
                    var item = _markDefinitionCheckedListBox.Items[i] as KryptonListItem;
                    var markDef = item.Tag as MemoMarkDefinition;
                    if (value.Any(v => v.Kind == markDef.Kind)) {
                        _markDefinitionCheckedListBox.SetItemChecked(i, true);
                    }
                }
            }
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
