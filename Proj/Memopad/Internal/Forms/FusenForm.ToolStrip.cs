/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Focuses;
using Mkamo.StyledText.Core;

namespace Mkamo.Memopad.Internal.Forms {
    partial class FusenForm {
        protected void HandleSelectSpeciaListToolStripDropDownButtonDropDownOpening(object sender, EventArgs e) {
            if (_EditorCanvas == null) {
                return;
            }

            var kind = _CurrentSpecialListKind;
            _toolStripForm._checkBoxListToolStripMenuItem.Checked = kind == ListKind.CheckBox;
            _toolStripForm._triStateCheckBoxListToolStripMenuItem.Checked = kind == ListKind.TriStateCheckBox;
            _toolStripForm._starListToolStripMenuItem.Checked = kind == ListKind.Star;
            _toolStripForm._leftArrowListToolStripMenuItem.Checked = kind == ListKind.LeftArrow;
            _toolStripForm._rightArrowListToolStripMenuItem.Checked = kind == ListKind.RightArrow;
        }

        private void InitToolStripHandlers() {
            // --- memo ---
            _toolStripForm._importantToolStripButton.Click += HandleImportantToolStripButtonClick;
            _toolStripForm._unimportantToolStripButton.Click += HandleUnimportantToolStripButtonClick;

            // --- edit ---
            _toolStripForm._searchInMemoToolStripButton.Click += HandleSearchInMemoToolStripButtonClick;
            _toolStripForm._cutToolStripButton.Click += HandleCutToolStripButtonClick;
            _toolStripForm._copyToolStripButton.Click += HandleCopyToolStripButtonClick;
            _toolStripForm._pasteToolStripButton.Click += HandlePasteToolStripButtonClick;
            _toolStripForm._undoToolStripButton.Click += HandleUndoToolStripButtonClick;
            _toolStripForm._redoToolStripButton.Click += HandleRedoToolStripButtonClick;

            // --- paragraph ---
            _toolStripForm._paragraphKindToolStripComboBox.SelectedIndexChanged += HandleParagraphKindToolStripComboBoxSelectedIndexChanged;

            // --- font ---
            _toolStripForm._fontNameToolStripComboBox.SelectedIndexChanged += HandleFontNameToolStripComboBoxSelectedIndexChanged;
            _toolStripForm._fontSizeToolStripComboBox.SelectedIndexChanged += HandleFontSizeToolStripComboBoxSelectedIndexChanged;
            _toolStripForm._fontBoldToolStripButton.Click += HandleFontBoldToolStripButtonClick;
            _toolStripForm._fontItalicToolStripButton.Click += HandleFontItalicToolStripButtonClick;
            _toolStripForm._fontUnderlineToolStripButton.Click += HandleFontUnderlineToolStripButtonClick;
            _toolStripForm._fontStrikeoutToolStripButton.Click += HandleFontStrikeoutToolStripButtonClick;

            // --- font color ---
            _toolStripForm._textColorButtonToolStripItem.Click += HandleTextColorButtonToolStripItemClick;
            _toolStripForm._textColorButtonToolStripItem.KryptonColorButtonControl.SelectedColorChanged +=
                HandleTextColorButtonToolStripItemSelectedColorChanged;

            // --- text alignment ---
            _toolStripForm._leftHorizontalAlignmentToolStripButton.Click += HandleLeftHorizontalAlignmentToolStripButtonClick;
            _toolStripForm._centerHorizontalAlignmentToolStripButton.Click += HandleCenterHorizontalAlignmentToolStripButtonClick;
            _toolStripForm._rightHorizontalAlignmentToolStripButton.Click += HandleRightHorizontalAlignmentToolStripButtonClick;
            _toolStripForm._topVAlignToolStripMenuItem.Click += HandleTopVAlignToolStripMenuItemClick;
            _toolStripForm._centerVAlignToolStripMenuItem.Click += HandleCenterVAlignToolStripMenuItemClick;
            _toolStripForm._bottomVAlignToolStripMenuItem.Click += HandleBottomVAlignToolStripMenuItemClick;

            // --- list ---
            _toolStripForm._unorderedListToolStripButton.Click += HandleUnorderedListToolStripButtonClick;
            _toolStripForm._orderedListToolStripButton.Click += HandleOrderedListToolStripButtonClick;
            _toolStripForm._specialListToolStripButton.Click += HandleSpecialListToolStripButtonClick;
            _toolStripForm._selectSpecialListToolStripDropDownButton.DropDownOpening += HandleSelectSpeciaListToolStripDropDownButtonDropDownOpening;
            _toolStripForm._checkBoxListToolStripMenuItem.Click += HandleCheckBoxListToolStripMenuItemClick;
            _toolStripForm._triStateCheckBoxListToolStripMenuItem.Click += HandleTriStateCheckBoxListToolStripMenuItemClick;
            _toolStripForm._starListToolStripMenuItem.Click += HandleStarListToolStripMenuItemClick;
            _toolStripForm._leftArrowListToolStripMenuItem.Click += HandleLeftArrowListToolStripMenuItemClick;
            _toolStripForm._rightArrowListToolStripMenuItem.Click += HandleRightArrowListToolStripMenuItemClick;

            // --- indent ---
            _toolStripForm._indentToolStripButton.Click += HandleIndentToolStripButtonClick;
            _toolStripForm._outdentToolStripButton.Click += HandleOutdentToolStripButtonClick;

            // --- main ---
            // --- add ---
            _toolStripForm._selectToolToolStripButton.Click += HandleSelectToolToolStripButtonClick;
            _toolStripForm._handToolToolStripButton.Click += HandleHandToolToolStripButtonClick;
            _toolStripForm._adjustSpaceToolToolStripButton.Click += HandleAdjustSpaceToolToolStripButtonClick;
            _toolStripForm._addImageFromFileToolStripMenuItem.Click += HandleAddImageFromFileToolStripMenuItemClick;
            _toolStripForm._addImageFromScreenToolStripMenuItem.Click += HandleAddImageFromScreenToolStripMenuItemClick;
            _toolStripForm._addEmbededFileToolStripMenuItem.Click += HandleAddEmbededFileToolStripMenuItemClick;
            _toolStripForm._addShortcutFileToolStripMenuItem.Click += HandleAddShortcutFileToolStripMenuItemClick;
            _toolStripForm._addFolderShortcutFileToolStripMenuItem.Click += HandleAddFolderShortcutFileToolStripMenuItemClick;
            _toolStripForm._addTableToolStripButton.Click += HandleAddTableToolStripButtonClick;

            // --- color ---
            _toolStripForm._shapeColorButtonToolStripItem.Click += HandleShapeColorButtonToolStripItemClick;
            _toolStripForm._shapeColorButtonToolStripItem.KryptonColorButtonControl.SelectedColorChanged +=
                HandleShapeColorButtonToolStripItemSelectedColorChanged;

            // --- mark ---
            _toolStripForm._memoMarkToolStripSplitButton.DropDownOpening += HandleMemoMarkToolStripSplitButtonDropDownOpening;
            _toolStripForm._memoMarkToolStripSplitButton.ButtonClick += HandleMemoMarkToolStripSplitButtonButtonClick;

            // --- comment ---
            _toolStripForm._addCommentToolStripButton.Click += HandleAddCommentToolStripButtonClick;
        }
    }
}
