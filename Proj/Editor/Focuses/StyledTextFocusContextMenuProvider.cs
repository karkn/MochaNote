/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mkamo.Editor.Controllers;
using Mkamo.StyledText.Core;
using Mkamo.StyledText.Util;

namespace Mkamo.Editor.Focuses {
    public class StyledTextFocusContextMenuProvider: AbstractContextMenuProvider {
        // ========================================
        // field
        // ========================================
        private StyledTextFocus _owner;

        private ToolStripMenuItem _cut;
        private ToolStripMenuItem _copy;
        private ToolStripMenuItem _pasteInlines;
        private ToolStripMenuItem _pasteText;
        private ToolStripMenuItem _pasteTextInBlock;

        private ToolStripMenuItem _delete;

        private ToolStripSeparator _separator1;

        private ToolStripMenuItem _selectParagraph;
        private ToolStripMenuItem _selectAll;

        // ========================================
        // constructor
        // ========================================
        public StyledTextFocusContextMenuProvider(StyledTextFocus owner) {
            _owner = owner;
        }

        // ========================================
        // property
        // ========================================
        public StyledTextFocus Owner {
            get { return _owner; }
        }


        // ========================================
        // method
        // ========================================
        public override ContextMenuStrip GetContextMenu(MouseEventArgs e) {
            if (_cut == null) {
                InitItems();
            }
            _ContextMenu.Items.Clear();

            _ContextMenu.Items.Add(_cut);
            _ContextMenu.Items.Add(_copy);

            /// clipboard
            var containsBlocksAndInlines = ClipboardUtil.ContainsBlocksAndInlines();
            var containsText = ClipboardUtil.ContainsText();

            if (containsBlocksAndInlines) {
                _ContextMenu.Items.Add(_pasteInlines);
            }

            if (!containsBlocksAndInlines && containsText) {
                _ContextMenu.Items.Add(_pasteText);
            }
            if (containsText) {
                _ContextMenu.Items.Add(_pasteTextInBlock);
            }

            _ContextMenu.Items.Add(_delete);

            _ContextMenu.Items.Add(_separator1);

            _ContextMenu.Items.Add(_selectParagraph);
            _ContextMenu.Items.Add(_selectAll);

            _cut.Enabled = !_owner.Selection.IsEmpty;
            _copy.Enabled = !_owner.Selection.IsEmpty;
            _delete.Enabled = !_owner.Selection.IsEmpty;

            return _ContextMenu;
        }

    
        private void InitItems() {
            _cut = new ToolStripMenuItem();
            _cut.Text = "切り取り(&X)";
            _cut.Click += (sender, ev) => {
                _owner.Cut();
            };

            _copy = new ToolStripMenuItem();
            _copy.Text = "コピー(&C)";
            _copy.Click += (sender, ev) => {
                _owner.Copy();
            };

            _pasteInlines = new ToolStripMenuItem();
            _pasteInlines.Text = "貼り付け(&P)";
            _pasteInlines.Click += (sender, ev) => {
                _owner.Paste();
            };

            _pasteText = new ToolStripMenuItem();
            _pasteText.Text = "貼り付け(&P)";
            _pasteText.Click += (sender, ev) => {
                _owner.PasteText(false);
            };

            _pasteTextInBlock = new ToolStripMenuItem();
            _pasteTextInBlock.Text = "段落内に貼り付け(&P)";
            _pasteTextInBlock.Click += (sender, ev) => {
                _owner.PasteText(true);
            };

            _delete = new ToolStripMenuItem("削除(&D)");
            _delete.Click += (sender, ev) => {
                _owner.RemoveForward();
            };

            _separator1 = new ToolStripSeparator();

            _selectParagraph = new ToolStripMenuItem();
            _selectParagraph.Text = "段落を選択(&P)";
            _selectParagraph.Click += (sender, ev) => {
                _owner.SelectParagraph();
            };
            
            _selectAll = new ToolStripMenuItem();
            _selectAll.Text = "すべて選択(&A)";
            _selectAll.Click += (sender, ev) => {
                _owner.SelectAll();
            };
            
        }

    }

}
