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
using Mkamo.Control.ToolStrip;
using ComponentFactory.Krypton.Toolkit;
using Mkamo.Common.DataType;
using Mkamo.Editor.Focuses;
using Mkamo.Memopad.Internal.Helpers;
using Mkamo.Memopad.Core;
using System.Windows.Forms;
using Mkamo.Editor.Requests;
using Mkamo.Model.Memo;
using Mkamo.Model.Core;
using Mkamo.Editor.Tools;
using System.IO;
using Mkamo.Memopad.Properties;
using System.Threading;
using Mkamo.Common.Forms.ScreenCapture;
using Mkamo.Editor.Commands;
using Mkamo.Common.Command;
using Mkamo.Memopad.Internal.Utils;
using Mkamo.Common.Collection;
using Mkamo.StyledText.Core;
using Mkamo.Common.Core;

namespace Mkamo.Memopad.Internal.Forms {
    partial class MemopadFormBase {
        // --- memo ---
        protected void HandleImportantToolStripButtonClick(object sender, EventArgs e) {
            if (_EditorCanvas == null) {
                return;
            }

            var memo = _EditorCanvas.EditorContent as Memo;
            if (memo.Importance == MemoImportanceKind.High) {
                memo.Importance = MemoImportanceKind.Normal;
            } else {
                memo.Importance = MemoImportanceKind.High;
            }
            UpdateToolStrip();
            if (_facade.IsMainFormLoaded) {
                var info = _facade.FindMemoInfo(memo);
                var main = _facade.MainForm;
                if (main._MemoListView.MemoListBox.SortsImportanceOrder) {
                    var listBox = main._MemoListView.MemoListBox;
                    if (listBox.Items.Contains(info)) {
                        listBox.Sorted = false;
                        listBox.Sorted = true;
                    }
                }
                main._MemoListView.MemoListBox.InvalidateList(new[] { info });
            }
        }

        protected void HandleUnimportantToolStripButtonClick(object sender, EventArgs e) {
            if (_EditorCanvas == null) {
                return;
            }

            var memo = _EditorCanvas.EditorContent as Memo;
            if (memo.Importance == MemoImportanceKind.Low) {
                memo.Importance = MemoImportanceKind.Normal;
            } else {
                memo.Importance = MemoImportanceKind.Low;
            }
            UpdateToolStrip();
            if (_facade.IsMainFormLoaded) {
                var info = _facade.FindMemoInfo(memo);
                var main = _facade.MainForm;
                if (main._MemoListView.MemoListBox.SortsImportanceOrder) {
                    var listBox = main._MemoListView.MemoListBox;
                    if (listBox.Items.Contains(info)) {
                        listBox.Sorted = false;
                        listBox.Sorted = true;
                    }
                }
                main._MemoListView.MemoListBox.InvalidateList(new[] { info });
            }
        }

        // --- edit ---
        protected void HandleSearchInMemoToolStripButtonClick(object sender, EventArgs e) {
            FocusEditorCanvas();
            _UILogic.DoSearchInMemo(_EditorCanvas, _PageContent);
        }

        protected void HandleCutToolStripButtonClick(object sender, EventArgs e) {
            FocusEditorCanvas();
            _UILogic.DoCut(_EditorCanvas);
        }

        protected void HandleCopyToolStripButtonClick(object sender, EventArgs e) {
            FocusEditorCanvas();
            _UILogic.DoCopy(_EditorCanvas);
        }

        protected void HandlePasteToolStripButtonClick(object sender, EventArgs e) {
            FocusEditorCanvas();
            _UILogic.DoPaste(_EditorCanvas);
        }

        protected void HandleUndoToolStripButtonClick(object sender, EventArgs e) {
            FocusEditorCanvas();
            _UILogic.DoUndo(_EditorCanvas);
        }

        protected void HandleRedoToolStripButtonClick(object sender, EventArgs e) {
            FocusEditorCanvas();
            _UILogic.DoRedo(_EditorCanvas);
        }

        // --- paragraph ---
        protected void HandleParagraphKindToolStripComboBoxSelectedIndexChanged(object sender, EventArgs e) {
            if (_EditorCanvas == null) {
                return;
            }

            /// コードによるコンボボックスの値の変更によって
            /// このハンドラが呼ばれた場合は何もしない
            if (_SuppressParagraphKindChangeEventHandler) {
                return;
            }

            var paraKind = GetParagraphKindFromString(_ParagraphKindToolStripComboBox.Text);
            SetFocusParagraphKind(paraKind);

            FocusEditorCanvas();
        }


        // --- font ---
        protected void HandleFontNameToolStripComboBoxSelectedIndexChanged(object sender, EventArgs e) {
            if (_EditorCanvas == null) {
                return;
            }

            /// コードによるコンボボックスの値の変更によって
            /// このハンドラが呼ばれた場合は何もしない
            if (_suppressFontChangeEventHandler) {
                return;
            }

            var fontName = _FontNameToolStripComboBox.Text;
            try {
                if (string.IsNullOrEmpty(fontName)) {
                    FocusEditorCanvas();
                    return;
                }

                var family = new FontFamily(fontName);
                if (!family.IsStyleAvailable(FontStyle.Regular)) {
                    FocusEditorCanvas();
                    return;
                }
            } catch (ArgumentException) {
                FocusEditorCanvas();
                return;
            }

            SetFontBase(
                font => font.Name == fontName? font: new FontDescription(font, fontName),
                FontModificationKinds.Name
            );

            FocusEditorCanvas();
        }

        protected void HandleFontSizeToolStripComboBoxSelectedIndexChanged(object sender, EventArgs e) {
            if (_EditorCanvas == null) {
                return;
            }

            /// コードによるコンボボックスの値の変更によって
            /// このハンドラが呼ばれた場合は何もしない
            if (_suppressFontChangeEventHandler) {
                return;
            }

            var fontSizeStr = _FontSizeToolStripComboBox.Text;
            if (string.IsNullOrEmpty(fontSizeStr)) {
                FocusEditorCanvas();
                return;
            }
            var fontSize = int.Parse(fontSizeStr);

            SetFontBase(
                font => font.Size == fontSize? font: new FontDescription(font, fontSize),
                FontModificationKinds.Size
            );

            FocusEditorCanvas();
        }

        protected void HandleFontBoldToolStripButtonClick(object sender, EventArgs e) {
            if (_EditorCanvas == null) {
                return;
            }

            SetFontStyleBase(
                font => font.IsBold ? font.Style & ~FontStyle.Bold : font.Style | FontStyle.Bold
            );

            UpdateToolStrip();
            FocusEditorCanvas();
        }

        protected void HandleFontItalicToolStripButtonClick(object sender, EventArgs e) {
            if (_EditorCanvas == null) {
                return;
            }
            SetFontStyleBase(
                font => font.IsItalic? font.Style & ~FontStyle.Italic: font.Style | FontStyle.Italic
            );
            UpdateToolStrip();
            FocusEditorCanvas();
        }

        protected void HandleFontUnderlineToolStripButtonClick(object sender, EventArgs e) {
            if (_EditorCanvas == null) {
                return;
            }
            SetFontStyleBase(
                font => font.IsUnderline? font.Style & ~FontStyle.Underline: font.Style | FontStyle.Underline
            );
            UpdateToolStrip();
            FocusEditorCanvas();
        }

        protected void HandleFontStrikeoutToolStripButtonClick(object sender, EventArgs e) {
            if (_EditorCanvas == null) {
                return;
            }
            SetFontStyleBase(
                font => font.IsStrikeout? font.Style & ~FontStyle.Strikeout: font.Style | FontStyle.Strikeout
            );
            UpdateToolStrip();
            FocusEditorCanvas();
        }

        // --- font color ---
        protected void HandleTextColorButtonToolStripItemClick(object sender, EventArgs e) {
            var item = (KryptonColorButtonToolStripItem) sender;
            SetTextColorBase(item.KryptonColorButtonControl.SelectedColor);
            FocusEditorCanvas();
        }

        protected void HandleTextColorButtonToolStripItemSelectedColorChanged(object sender, ColorEventArgs e) {
            SetTextColorBase(e.Color);
            FocusEditorCanvas();
        }

        // --- text alignment ---
        protected void HandleLeftHorizontalAlignmentToolStripButtonClick(object sender, EventArgs e) {
            if (_EditorCanvas == null) {
                return;
            }

            SetStyledTextAlignmentBase(
                AlignmentModificationKinds.Horizontal,
                Mkamo.Common.DataType.HorizontalAlignment.Left,
                VerticalAlignment.Top
            );
            FocusEditorCanvas();
        }

        protected void HandleCenterHorizontalAlignmentToolStripButtonClick(object sender, EventArgs e) {
            if (_EditorCanvas == null) {
                return;
            }
            SetStyledTextAlignmentBase(
                AlignmentModificationKinds.Horizontal,
                Mkamo.Common.DataType.HorizontalAlignment.Center,
                VerticalAlignment.Top
            );
            FocusEditorCanvas();
        }

        protected void HandleRightHorizontalAlignmentToolStripButtonClick(object sender, EventArgs e) {
            if (_EditorCanvas == null) {
                return;
            }
            SetStyledTextAlignmentBase(
                AlignmentModificationKinds.Horizontal,
                Mkamo.Common.DataType.HorizontalAlignment.Right,
                VerticalAlignment.Top
            );
            FocusEditorCanvas();
        }

        protected void HandleTopVAlignToolStripMenuItemClick(object sender, EventArgs e) {
            if (_EditorCanvas == null) {
                return;
            }
            SetStyledTextAlignmentBase(
                AlignmentModificationKinds.Vertical,
                Mkamo.Common.DataType.HorizontalAlignment.Left,
                VerticalAlignment.Top
            );
            FocusEditorCanvas();
        }

        protected void HandleCenterVAlignToolStripMenuItemClick(object sender, EventArgs e) {
            if (_EditorCanvas == null) {
                return;
            }
            SetStyledTextAlignmentBase(
                AlignmentModificationKinds.Vertical,
                Mkamo.Common.DataType.HorizontalAlignment.Left,
                VerticalAlignment.Center
            );
            FocusEditorCanvas();
        }

        protected void HandleBottomVAlignToolStripMenuItemClick(object sender, EventArgs e) {
            if (_EditorCanvas == null) {
                return;
            }
            SetStyledTextAlignmentBase(
                AlignmentModificationKinds.Vertical,
                Mkamo.Common.DataType.HorizontalAlignment.Left,
                VerticalAlignment.Bottom
            );
            FocusEditorCanvas();
        }


        // --- list ---
        protected void HandleUnorderedListToolStripButtonClick(object sender, EventArgs e) {
            SetParagraphListKind(ListKind.Unordered);
        }

        protected void HandleOrderedListToolStripButtonClick(object sender, EventArgs e) {
            SetParagraphListKind(ListKind.Ordered);
        }

        protected void HandleSpecialListToolStripButtonClick(object sender, EventArgs e) {
            SetParagraphListKind(_currentSpecialListKind);
        }

        protected void HandleCheckBoxListToolStripMenuItemClick(object sender, EventArgs e) {
            _currentSpecialListKind = ListKind.CheckBox;
        }

        protected void HandleTriStateCheckBoxListToolStripMenuItemClick(object sender, EventArgs e) {
            _currentSpecialListKind = ListKind.TriStateCheckBox;
        }

        protected void HandleStarListToolStripMenuItemClick(object sender, EventArgs e) {
            _currentSpecialListKind = ListKind.Star;
        }

        protected void HandleLeftArrowListToolStripMenuItemClick(object sender, EventArgs e) {
            _currentSpecialListKind = ListKind.LeftArrow;
        }

        protected void HandleRightArrowListToolStripMenuItemClick(object sender, EventArgs e) {
            _currentSpecialListKind = ListKind.RightArrow;
        }

        // --- indent ---
        protected void HandleIndentToolStripButtonClick(object sender, EventArgs e) {
            if (_EditorCanvas == null) {
                return;
            }

            if (_EditorCanvas.FocusManager.IsEditorFocused) {
                var focus = _EditorCanvas.FocusManager.Focus as StyledTextFocus;
                focus.Indent();
                UpdateToolStrip();
            }

        }

        protected void HandleOutdentToolStripButtonClick(object sender, EventArgs e) {
            if (_EditorCanvas == null) {
                return;
            }

            if (_EditorCanvas.FocusManager.IsEditorFocused) {
                var focus = _EditorCanvas.FocusManager.Focus as StyledTextFocus;
                focus.Outdent();
                UpdateToolStrip();
            }
        }


        // --- add ---
        protected void HandleSelectToolToolStripButtonClick(object sender, EventArgs e) {
            if (_EditorCanvas == null) {
                return;
            }

            _EditorCanvas.Tool = new SelectTool();
        }

        protected void HandleHandToolToolStripButtonClick(object sender, EventArgs e) {
            if (_EditorCanvas == null) {
                return;
            }

            var tool = new HandTool(_EditorCanvas);
            _EditorCanvas.Tool = tool;
            tool.MouseDown += (s, ev) => {
                using (var stream = new MemoryStream(Resources.cursor_hand_grab)) {
                    _EditorCanvas.Cursor = new Cursor(stream);
                }
            };
            tool.MouseUp += (s, ev) => {
                using (var stream = new MemoryStream(Resources.cursor_hand)) {
                    _EditorCanvas.Cursor = new Cursor(stream);
                }
            };
            tool.DragFinish += (s, ev) => {
                using (var stream = new MemoryStream(Resources.cursor_hand)) {
                    _EditorCanvas.Cursor = new Cursor(stream);
                }
            };
            tool.DragCancel += (s, ev) => {
                using (var stream = new MemoryStream(Resources.cursor_hand)) {
                    _EditorCanvas.Cursor = new Cursor(stream);
                }
            };
        }

        protected void HandleAdjustSpaceToolToolStripButtonClick(object sender, EventArgs e) {
            if (_EditorCanvas == null) {
                return;
            }

            _EditorCanvas.Tool = new AdjustSpaceTool(_EditorCanvas, new SelectTool());
        }

        protected void HandleAddImageFromFileToolStripMenuItemClick(object sender, EventArgs e) {
            if (_EditorCanvas == null) {
                return;
            }

            var dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.RestoreDirectory = true;
            dialog.ShowHelp = true;
            dialog.Filter = "Image Files(*.bmp;*.jpg;*.gif;*.png;*.emf)|*.bmp;*.jpg;*.gif;*.png;*.emf";
            if (dialog.ShowDialog(this) == DialogResult.OK) {
                AddImage(
                    _EditorCanvas.RootEditor.Content,
                    dialog.FileName
                );
            }
            dialog.Dispose();
        }

        protected void HandleAddImageFromScreenToolStripMenuItemClick(object sender, EventArgs e) {
            Hide();

            try {
                Thread.Sleep(500);
                using (var form = new ScreenCaptureForm()) {
                    form.Font = _facade.Theme.CaptionFont;
                    form.Setup();
                    form.ShowDialog(this);
                
                    if (form.IsCaptured) {
                        using (var img = form.CreateCaptured()) {
                            MemoEditorHelper.AddImage(_EditorCanvas.RootEditor.Content, new Point(8, 8), img, true, true);
                        }
    
                    }
                    form.Close();
                }
                Thread.Sleep(200);

            } finally {
                Show();
                Invalidate();
            }
        }

        protected void HandleAddEmbededFileToolStripMenuItemClick(object sender, EventArgs e) {
            if (_EditorCanvas == null) {
                return;
            }

            var dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.RestoreDirectory = true;
            dialog.ShowHelp = true;
            dialog.Filter = "Files(*.*)|*.*";
            if (dialog.ShowDialog(this) == DialogResult.OK) {
                MemoEditorHelper.AddFile(
                    _EditorCanvas.RootEditor.Content,
                    MemopadConsts.DefaultCaretPosition,
                    dialog.FileName,
                    true,
                    true,
                    true
                );
            }
            dialog.Dispose();
        }

        protected void HandleAddShortcutFileToolStripMenuItemClick(object sender, EventArgs e) {
            if (_EditorCanvas == null) {
                return;
            }

            using (var dialog = new OpenFileDialog()) {
                dialog.Multiselect = false;
                dialog.RestoreDirectory = true;
                dialog.ShowHelp = true;
                dialog.Filter = "Files(*.*)|*.*";
                if (dialog.ShowDialog(this) == DialogResult.OK) {
                    MemoEditorHelper.AddFile(
                        _EditorCanvas.RootEditor.Content,
                        MemopadConsts.DefaultCaretPosition,
                        dialog.FileName,
                        false,
                        true,
                        true
                    );
                }
            }
        }

        protected void HandleAddFolderShortcutFileToolStripMenuItemClick(object sender, EventArgs e) {
            if (_EditorCanvas == null) {
                return;
            }

            using (var dialog = new FolderBrowserDialog()) {
                dialog.Description = "フォルダの選択";
                if (dialog.ShowDialog(this) == DialogResult.OK) {
                    MemoEditorHelper.AddFile(
                        _EditorCanvas.RootEditor.Content,
                        MemopadConsts.DefaultCaretPosition,
                        dialog.SelectedPath,
                        false,
                        true,
                        true
                    );
                }
            }
        }


        protected void HandleAddTableToolStripButtonClick(object sender, EventArgs e) {
            if (_EditorCanvas == null) {
                return;
            }

            var dialog = new CreateTableForm();
            if (dialog.ShowDialog(this) != DialogResult.OK) {
                return;
            }

            var colCount = dialog.ColumnCount;
            var rowCount = dialog.RowCount;

            var target = _EditorCanvas.RootEditor.Content;
            var bounds = new Rectangle(32, 32, 100, 100);
            var modelFactory = new DelegatingModelFactory<MemoTable>(
                () => {
                    var ret = MemoFactory.CreateTable();
                    return ret;
                }
            );

            var req = new CreateNodeRequest();
            req.ModelFactory = modelFactory;
            req.Bounds = bounds;

            var createdBounds = Rectangle.Empty;
            var cmd1 = target.GetCommand(req) as CreateNodeCommand;
            var cmd2 = new DelegatingCommand(
                () => {
                    var created = cmd1.CreatedEditor;
                    if (created != null) {
                        var table = created.Model as MemoTable;
                        for (int i = 0; i < colCount; ++i) {
                            table.AddColumn();
                        }
                        for (int i = 0; i < rowCount; ++i) {
                            table.AddRow();
                        }
                    }
                },
                () => {},
                () => {
                    // todo: これでもundo，redoで行，列の幅が復元できない
                    // Boundsは復元される
                    var created = cmd1.CreatedEditor;
                    created.Figure.Bounds = createdBounds;
                }
            );

            var cmd = cmd1.Chain(cmd2);
            _EditorCanvas.CommandExecutor.Execute(cmd);
            createdBounds = cmd1.CreatedEditor.Figure.Bounds;

            dialog.Dispose();

            FocusEditorCanvas();

            //var cmd = target.RequestCreateNode(modelFactory, bounds) as CreateNodeCommand;
            //var created = cmd.CreatedEditor;
            //if (created != null) {
            //    var colCount = 3;
            //    var rowCount = 3;
            //    var table = created.Model as MemoTable;
            //    for (int i = 0; i < colCount; ++i) {
            //        table.AddColumn();
            //    }
            //    for (int i = 0; i < rowCount; ++i) {
            //        foreach (var cell in table.AddRow().Cells) {
            //            var run = new Run("テスト" + i);
            //            var para = new Paragraph(run);
            //            para.Padding = new Insets();
            //            para.HorizontalAlignment = HorizontalAlignment.Left;
            //            var stext = new StyledText(para);
            //            stext.VerticalAlignment = VerticalAlignment.Center;
            //            stext.Font = _facade.Settings.GetDefaultMemoContentFont();
            //            cell.StyledText = stext;
            //        }
            //    }
            //}
        }

        // --- color ---
        protected void HandleShapeColorButtonToolStripItemClick(object sender, EventArgs e) {
            if (_EditorCanvas == null) {
                return;
            }

            var item = (KryptonColorButtonToolStripItem) sender;
            SetShapeColorBase(item.KryptonColorButtonControl.SelectedColor);
            FocusEditorCanvas();
        }

        protected void HandleShapeColorButtonToolStripItemSelectedColorChanged(object sender, ColorEventArgs e) {
            if (_EditorCanvas == null) {
                return;
            }

            SetShapeColorBase(e.Color);
            FocusEditorCanvas();
        }

        // --- mark ---
        protected void HandleMemoMarkToolStripSplitButtonDropDownOpening(object sender, EventArgs e) {
            var button = (ToolStripSplitButton) sender;
            var items = button.DropDownItems;
            items.Clear();

            if (_EditorCanvas == null) {
                return;
            }

            var memo = _EditorCanvas.EditorContent as Memo;
            var marks = memo.Marks;

            var item = default(ToolStripMenuItem);
            var defs = MemoMarkUtil.GetMemoMarkDefinitions();

            item = new ToolStripMenuItem("すべてクリア");
            item.Click += (s, ev) => MemoMarkDropDownClearMenuItemClick();
            items.Add(item);
            items.Add(new ToolStripSeparator());

            foreach (var def in defs) {
                var kind = def.Kind;
                var image = def.Image;
                item = new ToolStripMenuItem(def.Name, image);
                item.Click += (s, ev) => MemoMarkDropDownMenuItemClick(button, kind, image);
                item.Checked = marks.Any(mark => mark.Kind == kind);
                items.Add(item);
            }

            /// 選択中のノートのマーク表示
            //var targets = _EditorCanvas.SelectionManager.SelectedEditors;
            //if (!targets.Any(
            //    editor => {
            //        var content = editor.Model as MemoContent;
            //        return content != null && content.IsMarkable;
            //    }
            //)) {
            //    return;
            //}

            //var first = targets.First(
            //    editor => {
            //        var content = editor.Model as MemoContent;
            //        return content != null && content.IsMarkable;
            //    }
            //);
            //var firstContent = first.Model as MemoContent;
            //if (firstContent == null) {
            //    return;
            //}
            //var firstMarks = firstContent.Marks;

            //var item = default(ToolStripMenuItem);
            //var defs = MemoMarkUtil.GetMemoMarkDefinitions();

            //foreach (var def in defs) {
            //    var kind = def.Kind;
            //    var image = def.Image;
            //    item = new ToolStripMenuItem(def.Name, image);
            //    item.Click += (s, ev) => MemoMarkDropDownMenuItemClick(button, kind, image);
            //    item.Checked = firstMarks.Any(mark => mark.Kind == kind);
            //    items.Add(item);
            //}
        }

        protected void HandleMemoMarkToolStripSplitButtonButtonClick(object sender, EventArgs e) {
            var button = (ToolStripSplitButton) sender;
            var kind = (MemoMarkKind) button.Tag;
            MemoMarkDropDownMenuItemClick(button, kind, button.Image);
        }

        protected void MemoMarkDropDownClearMenuItemClick() {
            if (_EditorCanvas == null) {
                return;
            }

            var memo = _EditorCanvas.EditorContent as Memo;
            var marks = memo.Marks;
            if (marks.Any()) {
                marks.Clear();
                
                if (_facade.IsMainFormLoaded) {
                    var info = _PageContent.MemoInfo;
                    _facade.MainForm.InvalidateMemoListBox(new[] { info });
                }

                _PageContent.IsModified = true;
            }
        }

        protected void MemoMarkDropDownMenuItemClick(ToolStripSplitButton button, MemoMarkKind kind, Image image) {
            if (_EditorCanvas == null) {
                return;
            }

            var memo = _EditorCanvas.EditorContent as Memo;
            var marks = memo.Marks;

            var isAdding = !marks.Any(mark => mark.Kind == kind);

            if (memo.Marks.Any(mark => mark.Kind == kind)) {
                ICollectionUtil.Remove(memo.Marks, mark => mark.Kind == kind);
            } else {
                var newMark = MemoFactory.CreateMark();
                newMark.Kind = kind;
                memo.Marks.Add(newMark);
            }

            /// ToolStripSplitButtonを選択されたkindに変更
            button.Tag = kind;
            button.Image = image;

            if (_facade.IsMainFormLoaded) {
                var info = _PageContent.MemoInfo;
                _facade.MainForm.InvalidateMemoListBox(new [] { info });
            }

            _PageContent.IsModified = true;

            
            /// 選択中のノートのマーク設定
            //var targets = _EditorCanvas.SelectionManager.SelectedEditors;
            //if (!targets.Any(
            //    editor => {
            //        var content = editor.Model as MemoContent;
            //        return content != null && content.IsMarkable;
            //    }
            //)) {
            //    return;
            //}

            ///// markの追加か削除かの判定，最初のIsMarkableなeditorで判断する
            //var first = targets.First(
            //    editor => {
            //        var content = editor.Model as MemoContent;
            //        return content != null && content.IsMarkable;
            //    }
            //);
            //var firstContent = first.Model as MemoContent;
            //if (firstContent == null) {
            //    return;
            //}
            //var firstMarks = firstContent.Marks;
            //var isAdding = !firstMarks.Any(mark => mark.Kind == kind);

            ///// markの変更
            //foreach (var target in targets) {
            //    var content = target.Model as MemoContent;
            //    if (content == null || !content.IsMarkable) {
            //        continue;
            //    }
            //    if (isAdding && !content.Marks.Any(mark => mark.Kind == kind)) {
            //        var newMark = MemoFactory.CreateMark();
            //        newMark.Kind = kind;
            //        content.Marks.Add(newMark);
            //    } else if (!isAdding && content.Marks.Any(mark => mark.Kind == kind)) {
            //        ICollectionUtil.Remove(content.Marks, mark => mark.Kind == kind);
            //    }
            //}

            ///// ToolStripSplitButtonを選択されたkindに変更
            //button.Tag = kind;
            //button.Image = image;
        }

        protected void HandleAddCommentToolStripButtonClick(object sender, EventArgs e) {
            if (_EditorCanvas == null) {
                return;
            }

            var focus = _EditorCanvas.FocusManager.Focus as StyledTextFocus;
            if (focus != null) {
                MemoEditorHelper.AddCommentForMemoText(focus);
            }

        }

    }
}
