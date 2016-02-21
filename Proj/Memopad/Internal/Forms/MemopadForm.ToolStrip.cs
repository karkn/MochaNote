/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Common.Forms.Descriptions;
using System.Drawing;
using Mkamo.Common.DataType;
using Mkamo.Editor.Focuses;
using System.Windows.Forms;
using Mkamo.Figure.Core;
using Mkamo.Model.Memo;
using Mkamo.Editor.Requests;
using ComponentFactory.Krypton.Toolkit;
using Mkamo.Common.Forms.ScreenCapture;
using System.Threading;
using Mkamo.Common.Collection;
using Mkamo.Memopad.Properties;
using Mkamo.Memopad.Internal.Helpers;
using Mkamo.Model.Core;
using Mkamo.Memopad.Internal.Utils;
using Mkamo.Memopad.Core;
using Mkamo.Editor.Commands;
using Mkamo.StyledText.Core;
using Mkamo.Common.Forms.Drawing;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Common.Command;
using Mkamo.Editor.Tools;
using System.IO;

namespace Mkamo.Memopad.Internal.Forms {
    using VerticalAlignment = Mkamo.Common.DataType.VerticalAlignment;
    using HorizontalAlignment = Mkamo.Common.DataType.HorizontalAlignment;
    using StyledText = Mkamo.StyledText.Core.StyledText;
    using Mkamo.Memopad.Internal.Controls;
    using Mkamo.Common.Core;

    partial class MemopadForm {
        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        // --- memo ---
        private void _createMemoToolStripSplitButton_ButtonClick(object sender, EventArgs e) {
            _app.ShowMainForm();
            _app.ActivateMainForm();
            _app.CreateMemo();
        }

        private void _createMemoToolStripMenuItem_Click(object sender, EventArgs e) {
            _app.ShowMainForm();
            _app.ActivateMainForm();
            _app.CreateMemo();
        }

        private void _createFusenMemoToolStripMenuItem_Click(object sender, EventArgs e) {
            _app.CreateMemo(true);
        }

        private void _createMemoFromClipboardToolStripMenuItem_Click(object sender, EventArgs e) {
            _app.ShowMainForm();
            _app.ActivateMainForm();
            _app.CreateMemo();

            if (_currentEditorCanvas != null) {
                MemoEditorHelper.Paste(_currentEditorCanvas.RootEditor.Children.First(), false);
            }
        }

        private void _showAllFusenToolStripButton_Click(object sender, EventArgs e) {
            _app.ShowFusenForms(false);
        }

        private void _showAsFusenToolStripButton_Click(object sender, EventArgs e) {
            if (_currentEditorCanvas == null) {
                return;
            }

            var pageContent = (PageContent) _tabControl.SelectedTab.Tag;
            var memoInfo = pageContent.MemoInfo;
            _app.CloseMemo(memoInfo);
            _app.LoadMemoAsFusen(memoInfo, false);
        }

        private void _activeFolderToolStripDropDownButton_DropDownOpening(object sender, EventArgs e) {
            var items = _activeFolderToolStripDropDownButton.DropDown.Items;
            items.Clear();

            var item = new ToolStripMenuItem("アクティブクリアファイルなし");
            if (_app.ActiveFolder == null) {
                item.Checked = true;
            }
            item.Click += HandleDeselectActiveFolderToolStripMenuItemClick;
            items.Add(item);

            items.Add("-");

            var folders = _app.Workspace.Folders.ToArray();
            var comparer = new DelegatingComparer<MemoFolder>(
                (x, y) =>  Comparer<string>.Default.Compare(x.FullName, y.FullName)
            );
            Array.Sort(folders, comparer);

            var activeFolder = _app.ActiveFolder;
            foreach (var folder in folders) {
                var icon = folder == activeFolder? Resources.clear_folder_horizontal_open: Resources.clear_folder_horizontal;
                item = new ToolStripMenuItem(folder.FullName, icon);
                item.ImageTransparentColor = Color.Magenta;
                item.Tag = folder;
                if (_app.ActiveFolder == folder) {
                    item.Checked = true;
                }
                item.Click += HandleSelectActiveFolderToolStripMenuItemClick;
                items.Add(item);
            }
        }

        private void _spaceRightToolStripMenuItem_Click(object sender, EventArgs e) {
            var size = _EditorCanvas.AutoScrollMinSize;
            _EditorCanvas.ReservedMinSize = new Size(size.Width + 400, size.Height);
        }

        private void _spaceDownToolStripMenuItem_Click(object sender, EventArgs e) {
            var size = _EditorCanvas.AutoScrollMinSize;
            _EditorCanvas.ReservedMinSize = new Size(size.Width, size.Height + 400);
        }

        private void HandleDeselectActiveFolderToolStripMenuItemClick(object sender, EventArgs e) {
            _app.ActiveFolder = null;
        }

        private void HandleSelectActiveFolderToolStripMenuItemClick(object sender, EventArgs e) {
            var item = sender as ToolStripMenuItem;
            if (item == null) {
                return;
            }
            var folder = item.Tag as MemoFolder;
            if (folder == null) {
                return;
            }
            _app.ActiveFolder = folder;
        }

        protected void HandleSelectSpeciaListToolStripDropDownButtonDropDownOpening(object sender, EventArgs e) {
            if (_EditorCanvas == null) {
                return;
            }

            var kind = _CurrentSpecialListKind;
            _checkBoxListToolStripMenuItem.Checked = kind == ListKind.CheckBox;
            _triStateCheckBoxListToolStripMenuItem.Checked = kind == ListKind.TriStateCheckBox;
            _starListToolStripMenuItem.Checked = kind == ListKind.Star;
            _leftArrowListToolStripMenuItem.Checked = kind == ListKind.LeftArrow;
            _rightArrowListToolStripMenuItem.Checked = kind == ListKind.RightArrow;
        }


        private void InitToolStripHandlers() {
            // --- memo ---
            _importantToolStripButton.Click += HandleImportantToolStripButtonClick;
            _unimportantToolStripButton.Click += HandleUnimportantToolStripButtonClick;

            // --- edit ---
            _searchInMemoToolStripButton.Click += HandleSearchInMemoToolStripButtonClick;
            _cutToolStripButton.Click += HandleCutToolStripButtonClick;
            _copyToolStripButton.Click += HandleCopyToolStripButtonClick;
            _pasteToolStripButton.Click += HandlePasteToolStripButtonClick;
            _undoToolStripButton.Click += HandleUndoToolStripButtonClick;
            _redoToolStripButton.Click += HandleRedoToolStripButtonClick;

            // --- paragraph ---
            _paragraphKindToolStripComboBox.SelectedIndexChanged += HandleParagraphKindToolStripComboBoxSelectedIndexChanged;

            // --- font ---
            _fontNameToolStripComboBox.SelectedIndexChanged += HandleFontNameToolStripComboBoxSelectedIndexChanged;
            _fontSizeToolStripComboBox.SelectedIndexChanged += HandleFontSizeToolStripComboBoxSelectedIndexChanged;
            _fontBoldToolStripButton.Click += HandleFontBoldToolStripButtonClick;
            _fontItalicToolStripButton.Click += HandleFontItalicToolStripButtonClick;
            _fontUnderlineToolStripButton.Click += HandleFontUnderlineToolStripButtonClick;
            _fontStrikeoutToolStripButton.Click += HandleFontStrikeoutToolStripButtonClick;

            // --- font color ---
            _textColorButtonToolStripItem.Click += HandleTextColorButtonToolStripItemClick;
            _textColorButtonToolStripItem.KryptonColorButtonControl.SelectedColorChanged +=
                HandleTextColorButtonToolStripItemSelectedColorChanged;

            // --- text alignment ---
            _leftHorizontalAlignmentToolStripButton.Click += HandleLeftHorizontalAlignmentToolStripButtonClick;
            _centerHorizontalAlignmentToolStripButton.Click += HandleCenterHorizontalAlignmentToolStripButtonClick;
            _rightHorizontalAlignmentToolStripButton.Click += HandleRightHorizontalAlignmentToolStripButtonClick;
            _topVAlignToolStripMenuItem.Click += HandleTopVAlignToolStripMenuItemClick;
            _centerVAlignToolStripMenuItem.Click += HandleCenterVAlignToolStripMenuItemClick;
            _bottomVAlignToolStripMenuItem.Click += HandleBottomVAlignToolStripMenuItemClick;

            // --- list ---
            _unorderedListToolStripButton.Click += HandleUnorderedListToolStripButtonClick;
            _orderedListToolStripButton.Click += HandleOrderedListToolStripButtonClick;
            _specialListToolStripButton.Click += HandleSpecialListToolStripButtonClick;
            _selectSpecialListToolStripDropDownButton.DropDownOpening += HandleSelectSpeciaListToolStripDropDownButtonDropDownOpening;
            _checkBoxListToolStripMenuItem.Click += HandleCheckBoxListToolStripMenuItemClick;
            _triStateCheckBoxListToolStripMenuItem.Click += HandleTriStateCheckBoxListToolStripMenuItemClick;
            _starListToolStripMenuItem.Click += HandleStarListToolStripMenuItemClick;
            _leftArrowListToolStripMenuItem.Click += HandleLeftArrowListToolStripMenuItemClick;
            _rightArrowListToolStripMenuItem.Click += HandleRightArrowListToolStripMenuItemClick;

            // --- indent ---
            _indentToolStripButton.Click += HandleIndentToolStripButtonClick;
            _outdentToolStripButton.Click += HandleOutdentToolStripButtonClick;

            // --- main ---
            // --- add ---
            _selectToolToolStripButton.Click += HandleSelectToolToolStripButtonClick;
            _handToolToolStripButton.Click += HandleHandToolToolStripButtonClick;
            _adjustSpaceToolToolStripButton.Click += HandleAdjustSpaceToolToolStripButtonClick;
            _addImageFromFileToolStripMenuItem.Click += HandleAddImageFromFileToolStripMenuItemClick;
            _addImageFromScreenToolStripMenuItem.Click += HandleAddImageFromScreenToolStripMenuItemClick;
            _addEmbededFileToolStripMenuItem.Click += HandleAddEmbededFileToolStripMenuItemClick;
            _addShortcutFileToolStripMenuItem.Click += HandleAddShortcutFileToolStripMenuItemClick;
            _addFolderShortcutFileToolStripMenuItem.Click += HandleAddFolderShortcutFileToolStripMenuItemClick;
            _addTableToolStripButton.Click += HandleAddTableToolStripButtonClick;

            // --- color ---
            _shapeColorButtonToolStripItem.Click += HandleShapeColorButtonToolStripItemClick;
            _shapeColorButtonToolStripItem.KryptonColorButtonControl.SelectedColorChanged +=
                HandleShapeColorButtonToolStripItemSelectedColorChanged;

            // --- mark ---
            _memoMarkToolStripSplitButton.DropDownOpening += HandleMemoMarkToolStripSplitButtonDropDownOpening;
            _memoMarkToolStripSplitButton.ButtonClick += HandleMemoMarkToolStripSplitButtonButtonClick;

            // --- comment ---
            _addCommentToolStripButton.Click += HandleAddCommentToolStripButtonClick;
        }

    }
}
