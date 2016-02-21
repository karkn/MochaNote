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
using Mkamo.Model.Memo;
using Mkamo.Memopad.Internal.Utils;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Common.String;
using Mkamo.Model.Core;
using Mkamo.Common.Forms.TreeView;
using Mkamo.Container.Core;
using Mkamo.Common.Forms.Utils;
using Mkamo.Memopad.Internal.Helpers;
using Mkamo.Common.Forms.Input;
using Mkamo.Common.Forms.Themes;
using Mkamo.Common.Forms.KeyMap;

namespace Mkamo.Memopad.Internal.Forms {
    internal partial class TagManageForm: Form {
        // ========================================
        // field
        // ========================================
        private MemopadApplication _app;

        private KeyMap<TreeView> _tagTreeViewKeyMap;

        // ========================================
        // constructor
        // ========================================
        public TagManageForm() {
            InitializeComponent();

            _app = MemopadApplication.Instance;

            _tagTreeViewKeyMap = new KeyMap<TreeView>();
            _app.KeySchema.TreeViewKeyBinder.Bind(_tagTreeViewKeyMap);

            _tagTreeView.Updated += (sender, e) => _tagTreeView.ExpandAll();
            _tagTreeView.UpdateTags();
        }

        // ========================================
        // method
        // ========================================
        //protected override void OnPaint(PaintEventArgs e) {
        //    base.OnPaint(e);
        //    ControlPaintUtil.DrawBorder(e.Graphics, _tagTreeView, SystemColors.ControlDark, _tagTreeView.BackColor, 3);
        //}
        
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (_tagTreeView.Focused) {
                if (_tagTreeViewKeyMap.IsDefined(keyData)) {
                    var action = _tagTreeViewKeyMap.GetAction(keyData);
                    if (action != null) {
                        if (action(_tagTreeView)) {
                            return true;
                        }
                    }
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        // ------------------------------
        // private
        // ------------------------------
        private void TagEditForm_Load(object sender, EventArgs e) {
            _newTagToolStripButton.Click += HandleNewTagToolStripButton;
            _removeTagToolStripButton.Click += HandleRemoveTagToolStripButton;
            _renameTagToolStripButton.Click += HandleRenameTagToolStripButton;

            _tagTreeView.AfterSelect += HandleTagTreeViewAfterSelect;

            _mainToolStripContainer.ContentPanel.Paint += (ps, pe) => {
                ControlPaintUtil.DrawBorder(
                    pe.Graphics, _tagTreeView, SystemColors.ControlDark, _tagTreeView.BackColor, 3
                );
            };
        }

        private void HandleTagTreeViewAfterSelect(object sender, EventArgs e) {
            _newTagToolStripButton.Enabled = false;
            _removeTagToolStripButton.Enabled = false;
            _renameTagToolStripButton.Enabled = false;

            if (_tagTreeView.IsTagRootSelected) {
                _newTagToolStripButton.Enabled = true;
                _removeTagToolStripButton.Enabled = false;
                _renameTagToolStripButton.Enabled = false;
            } else if (_tagTreeView.IsTagSelected) {
                _newTagToolStripButton.Enabled = true;
                _removeTagToolStripButton.Enabled = true;
                _renameTagToolStripButton.Enabled = true;
            }
        }

        private void HandleNewTagToolStripButton(object sender, EventArgs e) {
            _tagTreeView.CreateTag();
        }

        private void HandleRemoveTagToolStripButton(object sender, EventArgs e) {
            _tagTreeView.RemoveTag();
        }

        private void HandleRenameTagToolStripButton(object sender, EventArgs e) {
            _tagTreeView.EditTag();
        }
    }
}
