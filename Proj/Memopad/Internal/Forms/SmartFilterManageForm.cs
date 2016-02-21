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
using Mkamo.Common.Forms.Utils;
using Mkamo.Model.Core;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Model.Memo;
using Mkamo.Common.Core;
using ComponentFactory.Krypton.Toolkit;

namespace Mkamo.Memopad.Internal.Forms {
    internal partial class SmartFilterManageForm: Form {
        
        // ========================================
        // constructor
        // ========================================
        public SmartFilterManageForm() {
            InitializeComponent();

        }

        // ========================================
        // property
        // ========================================
        private MemoSmartFilter SelectedSmartFilter {
            get {
                var item = _smartFilterListBox.SelectedItem as KryptonListItem;
                if (item == null) {
                    return null;
                }

                return item.Tag as MemoSmartFilter;
            }
        }

        // ========================================
        // method
        // ========================================
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            _smartFilterListBox.ListBox.MouseDoubleClick += HandleSmartFilterListBoxMouseDoubleClick;
            _smartFilterListBox.StateCommon.Border.Color1 = SystemColors.ControlDark;
            UpdateSmartFilterListBox();
            UpdateToolStrip();
        }


        private void UpdateSmartFilterListBox() {
            _smartFilterListBox.BeginUpdate();
            _smartFilterListBox.Sorted = false;
            _smartFilterListBox.SelectedIndices.Clear();
            _smartFilterListBox.Items.Clear();

            var app = MemopadApplication.Instance;
            var filters = app.Container.FindAll<MemoSmartFilter>();
            foreach (var filter in filters) {
                var item = new KryptonListItem(filter.Name);
                item.Tag = filter;
                _smartFilterListBox.Items.Add(item);
            }

            _smartFilterListBox.Sorted = true;
            _smartFilterListBox.EndUpdate();
        }

        private void UpdateToolStrip() {
            _removeSmartFilterToolStripButton.Enabled = SelectedSmartFilter != null;
            _editSmartFilterToolStripButton.Enabled = SelectedSmartFilter != null;
        }

        private void EditSelectedSmartFilter() {
            var filter = SelectedSmartFilter;
            if (filter == null) {
                return;
            }

            using (var dialog = new QueryHolderEditForm()) {
                var app = MemopadApplication.Instance;
                dialog.QueryHolder = filter;
                if (dialog.ShowDialog(app.MainForm) == DialogResult.OK) {
                    UpdateSmartFilterListBox();
                    UpdateToolStrip();
                }
            }
        }

        // --- event handler ---
        private void HandleSmartFilterListBoxMouseDoubleClick(object sender, MouseEventArgs e) {
            EditSelectedSmartFilter();
        }

        // --- generated event handler ---
        private void _smartFilterListBox_SelectedIndexChanged(object sender, EventArgs e) {
            UpdateToolStrip();
        }

        private void _createSmartFilterToolStripButton_Click(object sender, EventArgs e) {
            using (var dialog = new QueryHolderEditForm()) {
                var app = MemopadApplication.Instance;
                var filter = MemoFactory.CreateTransientSmartFilter();
                filter.Query = MemoFactory.CreateTransientQuery();
                filter.Name = "新しいスマートフィルタ";
                dialog.QueryHolder = filter;
                if (dialog.ShowDialog(app.MainForm) == DialogResult.OK) {
                    app.Container.Persist(dialog.QueryHolder.Query);
                    app.Container.Persist(dialog.QueryHolder);

                    UpdateSmartFilterListBox();
                    UpdateToolStrip();
                }
            }
        }

        private void _removeSmartFilterToolStripButton_Click(object sender, EventArgs e) {
            var filter = SelectedSmartFilter;
            if (filter == null) {
                return;
            }

            var app = MemopadApplication.Instance;
            app.Container.Remove(filter);
            UpdateSmartFilterListBox();
            UpdateToolStrip();
        }

        private void _editToolStripButton_Click(object sender, EventArgs e) {
            EditSelectedSmartFilter();
        }

    }
}
