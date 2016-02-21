/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Memopad.Internal.Core;
using System.Windows.Forms;
using Mkamo.Editor.Core;
using Mkamo.Memopad.Internal.Controls;
using Mkamo.Common.Forms.KeyMap;

namespace Mkamo.Memopad.Internal.KeySchemes {
    internal class DefaultKeyScheme: AbstractKeyScheme {
        // ========================================
        // constructor
        // ========================================
        public DefaultKeyScheme() {
        }

        // ========================================
        // method
        // ========================================
        protected override void ResetMemoContentFocusKeyBind(KeyBinder<IFocus> binder) {
            // --- move ---
            binder.SetBind(Keys.None, Keys.Left, "MoveBackwardChar");
            binder.SetBind(Keys.None, Keys.Right, "MoveForwardChar");
            binder.SetBind(Keys.None, Keys.Up, "MovePreviousLine");
            binder.SetBind(Keys.None, Keys.Down, "MoveNextLine");

            binder.SetBind(Keys.None, Keys.PageUp, "MovePreviousPage");
            binder.SetBind(Keys.None, Keys.PageDown, "MoveNextPage");

            binder.SetBind(Keys.None, Keys.Home, "MoveBeginningOfLine");
            binder.SetBind(Keys.None, Keys.End, "MoveEndOfLine");

            binder.SetBind(Keys.None, Keys.Home | Keys.Control, "MoveBeginningOfText");
            binder.SetBind(Keys.None, Keys.End | Keys.Control, "MoveEndOfText");

            binder.SetBind(Keys.None, Keys.Left | Keys.Control, "MovePreviousWord");
            binder.SetBind(Keys.None, Keys.Right | Keys.Control, "MoveNextWord");

            // --- select ---
            binder.SetBind(Keys.None, Keys.Left | Keys.Shift, "SelectBackwardChar");
            binder.SetBind(Keys.None, Keys.Right | Keys.Shift, "SelectForwardChar");
            binder.SetBind(Keys.None, Keys.Up | Keys.Shift, "SelectPreviousLine");
            binder.SetBind(Keys.None, Keys.Down | Keys.Shift, "SelectNextLine");

            binder.SetBind(Keys.None, Keys.A | Keys.Control, "SelectAll");

            // --- enter ---
            binder.SetBind(Keys.None, Keys.Enter, "InsertBlockBreak");
            binder.SetBind(Keys.None, Keys.Enter | Keys.Shift, "InsertLineBreak");
            binder.SetBind(Keys.None, Keys.Enter | Keys.Control, "CommitAndSelect");

            // --- insert ---
            binder.SetBind(Keys.None, Keys.Space | Keys.Control, "InsertDynamicAbbrev");

            // --- clipboard ---
            binder.SetBind(Keys.None, Keys.V | Keys.Control, "PasteInlinesOrText");
            binder.SetBind(Keys.None, Keys.X | Keys.Control, "Cut");
            binder.SetBind(Keys.None, Keys.C | Keys.Control, "Copy");

            // --- undo ---
            binder.SetBind(Keys.None, Keys.Z | Keys.Control, "Undo");
            binder.SetBind(Keys.None, Keys.Y | Keys.Control, "Redo");

            // --- remove ---
            binder.SetBind(Keys.None, Keys.Delete, "RemoveForward");
            binder.SetBind(Keys.None, Keys.Back, "RemoveBackward");

            // --- list ---
            binder.SetBind(Keys.None, Keys.L | Keys.Control | Keys.Shift, "ToggleUnorderedList");
            binder.SetBind(Keys.None, Keys.O | Keys.Control | Keys.Shift, "ToggleOrderedList");
            binder.SetBind(Keys.None, Keys.Space | Keys.Control | Keys.Shift, "ChangeToNextListState");

            // --- indent ---
            binder.SetBind(Keys.None, Keys.Tab, "Indent");
            binder.SetBind(Keys.None, Keys.Tab | Keys.Shift, "Outdent");

            // --- paragraph ---
            binder.SetBind(Keys.None, Keys.D1 | Keys.Shift | Keys.Alt, "SetParagraphKindToHeading1");
            binder.SetBind(Keys.None, Keys.D2 | Keys.Shift | Keys.Alt, "SetParagraphKindToHeading2");
            binder.SetBind(Keys.None, Keys.D3 | Keys.Shift | Keys.Alt, "SetParagraphKindToHeading3");
            binder.SetBind(Keys.None, Keys.D4 | Keys.Shift | Keys.Alt, "SetParagraphKindToHeading4");
            binder.SetBind(Keys.None, Keys.D5 | Keys.Shift | Keys.Alt, "SetParagraphKindToHeading5");
            binder.SetBind(Keys.None, Keys.D6 | Keys.Shift | Keys.Alt, "SetParagraphKindToHeading6");
            binder.SetBind(Keys.None, Keys.D0 | Keys.Shift | Keys.Alt, "SetParagraphKindToNormal");

            // --- style ---
            binder.SetBind(Keys.None, Keys.B | Keys.Control, "ToggleFontBold");
            binder.SetBind(Keys.None, Keys.I | Keys.Control, "ToggleFontItalic");
            binder.SetBind(Keys.None, Keys.U | Keys.Control, "ToggleFontUnderline");

            // --- move out ---
            binder.SetBind(Keys.None, Keys.Left | Keys.Control | Keys.Alt, "MoveOutLeft");
            binder.SetBind(Keys.None, Keys.Right | Keys.Control | Keys.Alt, "MoveOutRight");
            binder.SetBind(Keys.None, Keys.Up | Keys.Control | Keys.Alt, "MoveOutUp");
            binder.SetBind(Keys.None, Keys.Down | Keys.Control | Keys.Alt, "MoveOutDown");

            // --- comment ---
            /// MemoText以外は無視される
            binder.SetBind(Keys.None, Keys.Oemtilde | Keys.Control | Keys.Shift, "AddComment");

            // --- tab ---
            binder.SetBind(Keys.None, Keys.W | Keys.Control, "CloseTabPage");

            // --- special list ---
            /// MemopadFormの値を使うのでStyledTextFocusレベルではバインドできない
            binder.SetBind(Keys.None, Keys.P | Keys.Control | Keys.Shift, "ToggleSpecialList");
        }

        protected override void ResetMemoContentSingleLineFocusKeyBind(KeyBinder<IFocus> binder) {
            // --- move ---
            binder.SetBind(Keys.None, Keys.Left, "MoveBackwardChar");
            binder.SetBind(Keys.None, Keys.Right, "MoveForwardChar");

            binder.SetBind(Keys.None, Keys.Home, "MoveBeginningOfLine");
            binder.SetBind(Keys.None, Keys.End, "MoveEndOfLine");

            // --- select ---
            binder.SetBind(Keys.None, Keys.Left | Keys.Shift, "SelectBackwardChar");
            binder.SetBind(Keys.None, Keys.Right | Keys.Shift, "SelectForwardChar");

            // --- enter ---
            binder.SetBind(Keys.None, Keys.Enter, "CommitAndSelect");
            binder.SetBind(Keys.None, Keys.Enter | Keys.Shift, "CommitAndSelect");
            binder.SetBind(Keys.None, Keys.Enter | Keys.Control, "CommitAndSelect");

            // --- clipboard ---
            binder.SetBind(Keys.None, Keys.V | Keys.Control, "PasteLastLine");
            binder.SetBind(Keys.None, Keys.X | Keys.Control, "Cut");
            binder.SetBind(Keys.None, Keys.C | Keys.Control, "Copy");

            // --- undo ---
            binder.SetBind(Keys.None, Keys.Z | Keys.Control, "Undo");
            binder.SetBind(Keys.None, Keys.Y | Keys.Control, "Redo");

            // --- remove ---
            binder.SetBind(Keys.None, Keys.Delete, "RemoveForward");
            binder.SetBind(Keys.None, Keys.Back, "RemoveBackward");

            // --- tab ---
            binder.SetBind(Keys.None, Keys.W | Keys.Control, "CloseTabPage");
        }

        protected override void ResetTextBoxKeyBind(KeyBinder<TextBox> binder) {
            binder.SetBind(Keys.None, Keys.U | Keys.Control, "ClearText");
        }

        protected override void ResetTreeViewKeyBind(KeyBinder<TreeView> binder) {
            binder.SetBind(Keys.None, Keys.F2, "BeginEdit");
            binder.SetBind(Keys.None, Keys.Space, "ToggleChecked");
        }

        protected override void ResetComboBoxKeyBind(KeyBinder<ComboBox> binder) {
            
        }

        protected override void ResetMemoListViewKeyBind(KeyBinder<MemoListView> binder) {
            binder.SetBind(Keys.None, Keys.A | Keys.Control, "SelectAll");
            binder.SetBind(Keys.None, Keys.Enter, "LoadSelectedMemos");
            binder.SetBind(Keys.None, Keys.Delete, "RemoveSelectedMemos");
        }

        protected override void ResetPageContentTitleTextBoxKeyBind(KeyBinder<TextBox> binder) {
            ResetTextBoxKeyBind(binder);

            binder.SetBind(Keys.None, Keys.Enter, "FocusEditorCanvas");
            binder.SetBind(Keys.None, Keys.W | Keys.Control, "CloseTabPage");
        }

        protected override void ResetNoSelectionKeyBind(KeyBinder<EditorCanvas> binder) {
            // --- select ---
            binder.SetBind(Keys.None, Keys.A | Keys.Control, "SelectAllChildren");

            // --- scroll ---
            binder.SetBind(Keys.None, Keys.PageUp, "ScrollUp");
            binder.SetBind(Keys.None, Keys.PageDown, "ScrollDown");

            // --- undo ---
            binder.SetBind(Keys.None, Keys.Z | Keys.Control, "Undo");
            binder.SetBind(Keys.None, Keys.Y | Keys.Control, "Redo");

            // --- misc ---
            binder.SetBind(Keys.None, Keys.W | Keys.Control, "CloseTabPage");
        }

        protected override void ResetMemoEditorKeyBind(KeyBinder<IEditor> binder) {
            // --- noop ---
            binder.SetBind(Keys.None, Keys.Tab, "DoNothing");
            binder.SetBind(Keys.None, Keys.Tab | Keys.Shift, "DoNothing");

            // --- move ---
            binder.SetBind(Keys.None, Keys.Left, "MoveCaretLeft");
            binder.SetBind(Keys.None, Keys.Right, "MoveCaretRight");
            binder.SetBind(Keys.None, Keys.Up, "MoveCaretUp");
            binder.SetBind(Keys.None, Keys.Down, "MoveCaretDown");

            binder.SetBind(Keys.None, Keys.Home, "MoveCaretLeftMost");
            binder.SetBind(Keys.None, Keys.End, "MoveCaretRightMost");

            binder.SetBind(Keys.None, Keys.PageUp, "ScrollUp");
            binder.SetBind(Keys.None, Keys.PageDown, "ScrollDown");

            // --- select ---
            binder.SetBind(Keys.None, Keys.A | Keys.Control, "SelectAllChildren");

            // --- clipboard ---
            binder.SetBind(Keys.None, Keys.V | Keys.Control, "Paste");

            // --- undo ---
            binder.SetBind(Keys.None, Keys.Z | Keys.Control, "Undo");
            binder.SetBind(Keys.None, Keys.Y | Keys.Control, "Redo");

            // --- misc ---
            binder.SetBind(Keys.None, Keys.W | Keys.Control, "CloseTabPage");
        }

        protected override void ResetMemoContentEditorKeyBind(KeyBinder<IEditor> binder) {
            // --- move ---
            binder.SetBind(Keys.None, Keys.Left, "MoveLeft");
            binder.SetBind(Keys.None, Keys.Right, "MoveRight");
            binder.SetBind(Keys.None, Keys.Up, "MoveUp");
            binder.SetBind(Keys.None, Keys.Down, "MoveDown");

            binder.SetBind(Keys.None, Keys.PageUp, "ScrollUp");
            binder.SetBind(Keys.None, Keys.PageDown, "ScrollDown");

            // --- move out ---
            binder.SetBind(Keys.None, Keys.Left | Keys.Control | Keys.Alt, "MoveOutLeft");
            binder.SetBind(Keys.None, Keys.Right | Keys.Control | Keys.Alt, "MoveOutRight");
            binder.SetBind(Keys.None, Keys.Up | Keys.Control | Keys.Alt, "MoveOutUp");
            binder.SetBind(Keys.None, Keys.Down | Keys.Control | Keys.Alt, "MoveOutDown");

            // --- select ---
            binder.SetBind(Keys.None, Keys.Tab, "SelectNextElement");
            binder.SetBind(Keys.None, Keys.Tab | Keys.Shift, "SelectPreviousElement");

            // --- remove ---
            binder.SetBind(Keys.None, Keys.Delete, "Remove");

            // --- clipboard ---
            binder.SetBind(Keys.None, Keys.X | Keys.Control, "Cut");
            binder.SetBind(Keys.None, Keys.C | Keys.Control, "Copy");

            // --- focus ---
            binder.SetBind(Keys.None, Keys.Enter, "BeginFocus");
            binder.SetBind(Keys.None, Keys.Enter | Keys.Shift, "BeginFocus");
            binder.SetBind(Keys.None, Keys.Enter | Keys.Control, "BeginFocus");

            // --- undo ---
            binder.SetBind(Keys.None, Keys.Z | Keys.Control, "Undo");
            binder.SetBind(Keys.None, Keys.Y | Keys.Control, "Redo");

            // --- misc ---
            binder.SetBind(Keys.None, Keys.W | Keys.Control, "CloseTabPage");
        }

        protected override void ResetMemoTableCellEditorKeyBind(KeyBinder<IEditor> binder) {
            // --- move ---
            binder.SetBind(Keys.None, Keys.Left, "MoveLeftCell");
            binder.SetBind(Keys.None, Keys.Right, "MoveRightCell");
            binder.SetBind(Keys.None, Keys.Up, "MoveUpCell");
            binder.SetBind(Keys.None, Keys.Down, "MoveDownCell");

            binder.SetBind(Keys.None, Keys.Enter, "MoveDownCell");
            binder.SetBind(Keys.None, Keys.Tab, "MoveRightCellOrCreateRow");
            binder.SetBind(Keys.None, Keys.Tab | Keys.Shift, "MoveLeftCell");

            binder.SetBind(Keys.None, Keys.Home, "MoveLeftmostCell");
            binder.SetBind(Keys.None, Keys.End, "MoveRightmostCell");

            binder.SetBind(Keys.None, Keys.PageUp, "ScrollUp");
            binder.SetBind(Keys.None, Keys.PageDown, "ScrollDown");

            // --- empty ---
            binder.SetBind(Keys.None, Keys.Delete, "EmptyCell");

            // --- focus ---
            binder.SetBind(Keys.None, Keys.Enter | Keys.Shift, "BeginFocus");
            binder.SetBind(Keys.None, Keys.Enter | Keys.Control, "BeginFocus");

            // --- undo ---
            binder.SetBind(Keys.None, Keys.Z | Keys.Control, "Undo");
            binder.SetBind(Keys.None, Keys.Y | Keys.Control, "Redo");

            // --- misc ---
            binder.SetBind(Keys.None, Keys.W | Keys.Control, "CloseTabPage");
        }

        protected override void ResetMemoTableCellFocusKeyBind(KeyBinder<IFocus> binder) {
            ResetMemoContentFocusKeyBind(binder);

            binder.SetBind(Keys.None, Keys.Home | Keys.Control, "MoveBeginningOfText");
            binder.SetBind(Keys.None, Keys.End | Keys.Control, "MoveEndOfText");

            // --- move out ---
            binder.SetBind(Keys.None, Keys.Left | Keys.Control | Keys.Alt, "MoveOutLeft");
            binder.SetBind(Keys.None, Keys.Right | Keys.Control | Keys.Alt, "MoveOutRight");
            binder.SetBind(Keys.None, Keys.Up | Keys.Control | Keys.Alt, "MoveOutUp");
            binder.SetBind(Keys.None, Keys.Down | Keys.Control | Keys.Alt, "MoveOutDown");

            // --- tab ---
            binder.SetBind(Keys.None, Keys.W | Keys.Control, "CloseTabPage");

            // --- cell ---
            binder.SetBind(Keys.None, Keys.Enter, "CommitAndMoveDownCell");
            binder.SetBind(Keys.None, Keys.Enter | Keys.Alt, "InsertBlockBreak");
            binder.SetBind(Keys.None, Keys.Tab, "CommitAndMoveRightCellOrCreateRow");
            binder.SetBind(Keys.None, Keys.Tab | Keys.Shift, "CommitAndMoveLeftCell");
        }

        protected override void ResetUmlFeatureEditorKeyBind(KeyBinder<IEditor> binder) {
            binder.SetBind(Keys.None, Keys.Up, "SelectPreviousItem");
            binder.SetBind(Keys.None, Keys.Down, "SelectNextItem");

            // --- remove ---
            binder.SetBind(Keys.None, Keys.Delete, "RemoveItem");

            // --- focus ---
            binder.SetBind(Keys.None, Keys.Enter, "BeginFocus");
            binder.SetBind(Keys.None, Keys.Enter | Keys.Control, "BeginFocus");
            binder.SetBind(Keys.None, Keys.Enter | Keys.Shift, "BeginFocus");
        }

    }
}
