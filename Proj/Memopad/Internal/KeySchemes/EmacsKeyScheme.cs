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
    internal class EmacsKeyScheme: AbstractKeyScheme {
        // ========================================
        // constructor
        // ========================================
        public EmacsKeyScheme() {
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
            binder.SetBind(Keys.None, Keys.B | Keys.Control, "MoveBackwardChar");
            binder.SetBind(Keys.None, Keys.F | Keys.Control, "MoveForwardChar");
            binder.SetBind(Keys.None, Keys.P | Keys.Control, "MovePreviousLine");
            binder.SetBind(Keys.None, Keys.N | Keys.Control, "MoveNextLine");

            binder.SetBind(Keys.None, Keys.PageUp, "MovePreviousPage");
            binder.SetBind(Keys.None, Keys.PageDown, "MoveNextPage");
            binder.SetBind(Keys.None, Keys.V | Keys.Alt, "MovePreviousPage");
            binder.SetBind(Keys.None, Keys.V | Keys.Control, "MoveNextPage");

            binder.SetBind(Keys.None, Keys.Home, "MoveBeginningOfLine");
            binder.SetBind(Keys.None, Keys.End, "MoveEndOfLine");
            binder.SetBind(Keys.None, Keys.A | Keys.Control, "MoveBeginningOfLine");
            binder.SetBind(Keys.None, Keys.E | Keys.Control, "MoveEndOfLine");

            binder.SetBind(Keys.None, Keys.Home | Keys.Control, "MoveBeginningOfText");
            binder.SetBind(Keys.None, Keys.End | Keys.Control, "MoveEndOfText");
            binder.SetBind(Keys.None, Keys.Oemcomma | Keys.Alt | Keys.Shift, "MoveBeginningOfText");
            binder.SetBind(Keys.None, Keys.OemPeriod | Keys.Alt | Keys.Shift, "MoveEndOfText");

            binder.SetBind(Keys.None, Keys.Left | Keys.Control, "MovePreviousWord");
            binder.SetBind(Keys.None, Keys.Right | Keys.Control, "MoveNextWord");
            binder.SetBind(Keys.None, Keys.B | Keys.Alt, "MovePreviousWord");
            binder.SetBind(Keys.None, Keys.F | Keys.Alt, "MoveNextWord");

            // --- select ---
            binder.SetBind(Keys.None, Keys.Left | Keys.Shift, "SelectBackwardChar");
            binder.SetBind(Keys.None, Keys.Right | Keys.Shift, "SelectForwardChar");
            binder.SetBind(Keys.None, Keys.Up | Keys.Shift, "SelectPreviousLine");
            binder.SetBind(Keys.None, Keys.Down | Keys.Shift, "SelectNextLine");

            binder.SetBind(Keys.X | Keys.Control, Keys.H, "SelectAll");

            // --- mark ---
            binder.SetBind(Keys.None, Keys.Space | Keys.Control, "SetMark");
            binder.SetBind(Keys.None, Keys.Oemtilde | Keys.Control, "SetMark");
            binder.SetBind(Keys.X | Keys.Control, Keys.Space | Keys.Control, "PopMark");
            binder.SetBind(Keys.X | Keys.Control, Keys.X | Keys.Control, "ExchangeCaretAndMark");

            // --- enter ---
            binder.SetBind(Keys.None, Keys.Enter, "InsertBlockBreak");
            binder.SetBind(Keys.None, Keys.Enter | Keys.Shift, "InsertLineBreak");
            binder.SetBind(Keys.None, Keys.Enter | Keys.Control, "CommitAndSelect");
            binder.SetBind(Keys.None, Keys.M | Keys.Control, "InsertBlockBreak");
            binder.SetBind(Keys.None, Keys.J | Keys.Control, "InsertLineBreak");
            binder.SetBind(Keys.None, Keys.O | Keys.Control, "OpenLineBreak");

            // --- insert ---
            binder.SetBind(Keys.None, Keys.OemQuestion | Keys.Alt, "InsertDynamicAbbrev");

            // --- clipboard ---
            binder.SetBind(Keys.None, Keys.Y | Keys.Control, "PasteInlinesOrText");
            binder.SetBind(Keys.None, Keys.Y | Keys.Control | Keys.Alt, "PasteInlinesOrTextInBlock");
            binder.SetBind(Keys.None, Keys.W | Keys.Control, "CutRegion");
            binder.SetBind(Keys.None, Keys.W | Keys.Alt, "CopyRegion");
            binder.SetBind(Keys.None, Keys.C | Keys.Control, "Copy");

            // --- kill word ---
            binder.SetBind(Keys.None, Keys.D | Keys.Alt, "KillWord");

            // --- kill line ---
            binder.SetBind(Keys.None, Keys.K | Keys.Control, "KillLine"); // todo: 

            // --- undo ---
            binder.SetBind(Keys.None, Keys.OemQuestion | Keys.Control, "Undo");
            binder.SetBind(Keys.None, Keys.OemBackslash | Keys.Control | Keys.Shift, "Undo");
            binder.SetBind(Keys.X | Keys.Control, Keys.U, "Undo");

            // --- remove ---
            binder.SetBind(Keys.None, Keys.Delete, "RemoveForward");
            binder.SetBind(Keys.None, Keys.Back, "RemoveBackward");
            binder.SetBind(Keys.None, Keys.D | Keys.Control, "RemoveForward");
            binder.SetBind(Keys.None, Keys.H | Keys.Control, "RemoveBackward");

            // --- list ---
            binder.SetBind(Keys.None, Keys.L | Keys.Control | Keys.Shift, "ToggleUnorderedList");
            binder.SetBind(Keys.None, Keys.O | Keys.Control | Keys.Shift, "ToggleOrderedList");
            binder.SetBind(Keys.None, Keys.Space | Keys.Control | Keys.Shift, "ChangeToNextListState");

            // --- indent ---
            binder.SetBind(Keys.None, Keys.Tab, "Indent");
            binder.SetBind(Keys.None, Keys.Tab | Keys.Shift, "Outdent");
            binder.SetBind(Keys.None, Keys.I | Keys.Control, "Indent");
            binder.SetBind(Keys.None, Keys.I | Keys.Control | Keys.Shift, "Outdent");

            // --- paragraph ---
            binder.SetBind(Keys.None, Keys.D1 | Keys.Shift | Keys.Alt, "SetParagraphKindToHeading1");
            binder.SetBind(Keys.None, Keys.D2 | Keys.Shift | Keys.Alt, "SetParagraphKindToHeading2");
            binder.SetBind(Keys.None, Keys.D3 | Keys.Shift | Keys.Alt, "SetParagraphKindToHeading3");
            binder.SetBind(Keys.None, Keys.D4 | Keys.Shift | Keys.Alt, "SetParagraphKindToHeading4");
            binder.SetBind(Keys.None, Keys.D5 | Keys.Shift | Keys.Alt, "SetParagraphKindToHeading5");
            binder.SetBind(Keys.None, Keys.D6 | Keys.Shift | Keys.Alt, "SetParagraphKindToHeading6");
            binder.SetBind(Keys.None, Keys.D0 | Keys.Shift | Keys.Alt, "SetParagraphKindToNormal");

            // --- style ---
            binder.SetBind(Keys.Q | Keys.Control, Keys.B | Keys.Control, "ToggleFontBold");
            binder.SetBind(Keys.Q | Keys.Control, Keys.I | Keys.Control, "ToggleFontItalic");
            binder.SetBind(Keys.Q | Keys.Control, Keys.U | Keys.Control, "ToggleFontUnderline");

            // --- move out ---
            binder.SetBind(Keys.None, Keys.B | Keys.Control | Keys.Alt, "MoveOutLeft");
            binder.SetBind(Keys.None, Keys.F | Keys.Control | Keys.Alt, "MoveOutRight");
            binder.SetBind(Keys.None, Keys.P | Keys.Control | Keys.Alt, "MoveOutUp");
            binder.SetBind(Keys.None, Keys.N | Keys.Control | Keys.Alt, "MoveOutDown");

            // --- recenter ---
            binder.SetBind(Keys.None, Keys.L | Keys.Control, "ScrollRecenter");

            // --- comment ---
            /// MemoText以外は無視される
            binder.SetBind(Keys.None, Keys.Oemtilde | Keys.Control | Keys.Shift, "AddComment");

            // --- tab ---
            binder.SetBind(Keys.X | Keys.Control, Keys.K, "CloseTabPage");
            binder.SetBind(Keys.X | Keys.Control, Keys.C | Keys.Control, "CloseAllTabPages");

            // --- special list ---
            /// MemopadFormの値を使うのでStyledTextFocusレベルではバインドできない
            binder.SetBind(Keys.None, Keys.P | Keys.Control | Keys.Shift, "ToggleSpecialList");

            // --- misc ---
            binder.SetBind(Keys.X | Keys.Control, Keys.B | Keys.Control, "SelectOpenNotesNodeInWorkspace");
            binder.SetBind(Keys.X | Keys.Control, Keys.D | Keys.Control, "FocusWorkspaceView");
            binder.SetBind(Keys.X | Keys.Control, Keys.F | Keys.Control, "FocusMemoListView");
        }

        protected override void ResetMemoContentSingleLineFocusKeyBind(KeyBinder<IFocus> binder) {
            // --- move ---
            binder.SetBind(Keys.None, Keys.Left, "MoveBackwardChar");
            binder.SetBind(Keys.None, Keys.Right, "MoveForwardChar");
            binder.SetBind(Keys.None, Keys.B | Keys.Control, "MoveBackwardChar");
            binder.SetBind(Keys.None, Keys.F | Keys.Control, "MoveForwardChar");

            binder.SetBind(Keys.None, Keys.Home, "MoveBeginningOfLine");
            binder.SetBind(Keys.None, Keys.End, "MoveEndOfLine");
            binder.SetBind(Keys.None, Keys.A | Keys.Control, "MoveBeginningOfLine");
            binder.SetBind(Keys.None, Keys.E | Keys.Control, "MoveEndOfLine");

            // --- select ---
            binder.SetBind(Keys.None, Keys.Left | Keys.Shift, "SelectBackwardChar");
            binder.SetBind(Keys.None, Keys.Right | Keys.Shift, "SelectForwardChar");

            // --- mark ---
            binder.SetBind(Keys.None, Keys.Space | Keys.Control, "SetMark");
            binder.SetBind(Keys.None, Keys.Oemtilde | Keys.Control, "SetMark");
            binder.SetBind(Keys.X | Keys.Control, Keys.Space | Keys.Control, "PopMark");
            binder.SetBind(Keys.X | Keys.Control, Keys.X | Keys.Control, "ExchangeCaretAndMark");

            // --- enter ---
            binder.SetBind(Keys.None, Keys.Enter, "CommitAndSelect");
            binder.SetBind(Keys.None, Keys.Enter | Keys.Shift, "CommitAndSelect");
            binder.SetBind(Keys.None, Keys.Enter | Keys.Control, "CommitAndSelect");

            // --- clipboard ---
            binder.SetBind(Keys.None, Keys.Y | Keys.Control, "PasteLastLine");
            binder.SetBind(Keys.None, Keys.Y | Keys.Control | Keys.Alt, "PasteLastLine");
            binder.SetBind(Keys.None, Keys.W | Keys.Control, "CutRegion");
            binder.SetBind(Keys.None, Keys.W | Keys.Alt, "CopyRegion");
            binder.SetBind(Keys.None, Keys.C | Keys.Control, "Copy");

            // --- kill word ---
            binder.SetBind(Keys.None, Keys.D | Keys.Alt, "KillWord");

            // --- undo ---
            binder.SetBind(Keys.None, Keys.OemQuestion | Keys.Control, "Undo");
            binder.SetBind(Keys.None, Keys.OemBackslash | Keys.Control | Keys.Shift, "Undo");
            binder.SetBind(Keys.X | Keys.Control, Keys.U, "Undo");

            // --- remove ---
            binder.SetBind(Keys.None, Keys.Delete, "RemoveForward");
            binder.SetBind(Keys.None, Keys.Back, "RemoveBackward");
            binder.SetBind(Keys.None, Keys.D | Keys.Control, "RemoveForward");
            binder.SetBind(Keys.None, Keys.H | Keys.Control, "RemoveBackward");

            // --- recenter ---
            binder.SetBind(Keys.None, Keys.L | Keys.Control, "ScrollRecenter");

            // --- tab ---
            binder.SetBind(Keys.X | Keys.Control, Keys.K, "CloseTabPage");
            binder.SetBind(Keys.X | Keys.Control, Keys.C | Keys.Control, "CloseAllTabPages");

            // --- misc ---
            binder.SetBind(Keys.X | Keys.Control, Keys.B | Keys.Control, "SelectOpenNotesNodeInWorkspace");
            binder.SetBind(Keys.X | Keys.Control, Keys.D | Keys.Control, "FocusWorkspaceView");
            binder.SetBind(Keys.X | Keys.Control, Keys.F | Keys.Control, "FocusMemoListView");
        }

        protected override void ResetTextBoxKeyBind(KeyBinder<TextBox> binder) {
            binder.SetBind(Keys.None, Keys.F | Keys.Control, "MoveForward");
            binder.SetBind(Keys.None, Keys.B | Keys.Control, "MoveBackward");
            binder.SetBind(Keys.None, Keys.A | Keys.Control, "MoveBeginningOfLine");
            binder.SetBind(Keys.None, Keys.E | Keys.Control, "MoveEndOfLine");

            binder.SetBind(Keys.None, Keys.D | Keys.Control, "RemoveForward");

            binder.SetBind(Keys.None, Keys.K | Keys.Control, "KillLine");
            binder.SetBind(Keys.None, Keys.Y | Keys.Control, "Paste");

            binder.SetBind(Keys.None, Keys.U | Keys.Control, "ClearText");

            // --- misc ---
            binder.SetBind(Keys.X | Keys.Control, Keys.B | Keys.Control, "SelectOpenNotesNodeInWorkspace");
            binder.SetBind(Keys.X | Keys.Control, Keys.D | Keys.Control, "FocusWorkspaceView");
            binder.SetBind(Keys.X | Keys.Control, Keys.F | Keys.Control, "FocusMemoListView");
        }

        protected override void ResetTreeViewKeyBind(KeyBinder<TreeView> binder) {
            binder.SetBind(Keys.None, Keys.F2, "BeginEdit");
            binder.SetBind(Keys.None, Keys.Space, "ToggleChecked");

            binder.SetBind(Keys.None, Keys.F | Keys.Control, "MoveForward");
            binder.SetBind(Keys.None, Keys.B | Keys.Control, "MoveBackward");
            binder.SetBind(Keys.None, Keys.N | Keys.Control, "MoveNextLine");
            binder.SetBind(Keys.None, Keys.P | Keys.Control, "MovePreviousLine");

            binder.SetBind(Keys.None, Keys.A | Keys.Control, "MoveBeginningOfLine");
            binder.SetBind(Keys.None, Keys.E | Keys.Control, "MoveEndOfLine");

            // --- misc ---
            binder.SetBind(Keys.X | Keys.Control, Keys.B | Keys.Control, "SelectOpenNotesNodeInWorkspace");
            binder.SetBind(Keys.X | Keys.Control, Keys.D | Keys.Control, "FocusWorkspaceView");
            binder.SetBind(Keys.X | Keys.Control, Keys.F | Keys.Control, "FocusMemoListView");
        }

        protected override void ResetComboBoxKeyBind(KeyBinder<ComboBox> binder) {
            binder.SetBind(Keys.None, Keys.N | Keys.Control, "NextItem");
            binder.SetBind(Keys.None, Keys.P | Keys.Control, "PreviousItem");

            // --- misc ---
            binder.SetBind(Keys.X | Keys.Control, Keys.B | Keys.Control, "SelectOpenNotesNodeInWorkspace");
            binder.SetBind(Keys.X | Keys.Control, Keys.D | Keys.Control, "FocusWorkspaceView");
            binder.SetBind(Keys.X | Keys.Control, Keys.F | Keys.Control, "FocusMemoListView");
        }

        protected override void ResetMemoListViewKeyBind(KeyBinder<MemoListView> binder) {
            binder.SetBind(Keys.None, Keys.A | Keys.Control, "SelectAll");
            binder.SetBind(Keys.None, Keys.Enter, "LoadSelectedMemos");
            binder.SetBind(Keys.None, Keys.Delete, "RemoveSelectedMemos");

            binder.SetBind(Keys.None, Keys.P | Keys.Control, "SelectPreviousItem");
            binder.SetBind(Keys.None, Keys.N | Keys.Control, "SelectNextItem");
            binder.SetBind(Keys.None, Keys.D | Keys.Control, "RemoveSelectedMemos");

            // --- misc ---
            binder.SetBind(Keys.X | Keys.Control, Keys.B | Keys.Control, "SelectOpenNotesNodeInWorkspace");
            binder.SetBind(Keys.X | Keys.Control, Keys.D | Keys.Control, "FocusWorkspaceView");
            binder.SetBind(Keys.X | Keys.Control, Keys.F | Keys.Control, "FocusMemoListView");
        }

        protected override void ResetPageContentTitleTextBoxKeyBind(KeyBinder<TextBox> binder) {
            ResetTextBoxKeyBind(binder);

            binder.SetBind(Keys.None, Keys.Enter, "FocusEditorCanvas");

            binder.SetBind(Keys.X | Keys.Control, Keys.K, "CloseTabPage");
            binder.SetBind(Keys.X | Keys.Control, Keys.C | Keys.Control, "CloseAllTabPages");

            // --- misc ---
            binder.SetBind(Keys.X | Keys.Control, Keys.B | Keys.Control, "SelectOpenNotesNodeInWorkspace");
            binder.SetBind(Keys.X | Keys.Control, Keys.D | Keys.Control, "FocusWorkspaceView");
            binder.SetBind(Keys.X | Keys.Control, Keys.F | Keys.Control, "FocusMemoListView");
        }

        protected override void ResetNoSelectionKeyBind(KeyBinder<EditorCanvas> binder) {
            // --- select ---
            binder.SetBind(Keys.Q | Keys.Control, Keys.A | Keys.Control, "SelectAllChildren");
            binder.SetBind(Keys.X | Keys.Control, Keys.H, "SelectAllChildren");

            // --- scroll ---
            binder.SetBind(Keys.None, Keys.PageUp, "ScrollUp");
            binder.SetBind(Keys.None, Keys.PageDown, "ScrollDown");
            binder.SetBind(Keys.None, Keys.V | Keys.Alt, "ScrollUp");
            binder.SetBind(Keys.None, Keys.V | Keys.Control, "ScrollDown");

            // --- undo ---
            binder.SetBind(Keys.None, Keys.OemQuestion | Keys.Control, "Undo");
            binder.SetBind(Keys.None, Keys.OemBackslash | Keys.Control | Keys.Shift, "Undo");
            binder.SetBind(Keys.X | Keys.Control, Keys.U, "Undo");

            // --- misc ---
            binder.SetBind(Keys.X | Keys.Control, Keys.K, "CloseTabPage");
            binder.SetBind(Keys.X | Keys.Control, Keys.C | Keys.Control, "CloseAllTabPages");

            // --- misc ---
            binder.SetBind(Keys.X | Keys.Control, Keys.B | Keys.Control, "SelectOpenNotesNodeInWorkspace");
            binder.SetBind(Keys.X | Keys.Control, Keys.D | Keys.Control, "FocusWorkspaceView");
            binder.SetBind(Keys.X | Keys.Control, Keys.F | Keys.Control, "FocusMemoListView");
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
            binder.SetBind(Keys.None, Keys.B | Keys.Control, "MoveCaretLeft");
            binder.SetBind(Keys.None, Keys.F | Keys.Control, "MoveCaretRight");
            binder.SetBind(Keys.None, Keys.P | Keys.Control, "MoveCaretUp");
            binder.SetBind(Keys.None, Keys.N | Keys.Control, "MoveCaretDown");

            binder.SetBind(Keys.None, Keys.Home, "MoveCaretLeftMost");
            binder.SetBind(Keys.None, Keys.End, "MoveCaretRightMost");
            binder.SetBind(Keys.None, Keys.A | Keys.Control, "MoveCaretLeftMost");
            binder.SetBind(Keys.None, Keys.E | Keys.Control, "MoveCaretRightMost");

            binder.SetBind(Keys.None, Keys.L | Keys.Control, "ScrollRecenter");

            binder.SetBind(Keys.None, Keys.PageUp, "ScrollUp");
            binder.SetBind(Keys.None, Keys.PageDown, "ScrollDown");
            binder.SetBind(Keys.None, Keys.V | Keys.Alt, "ScrollUp");
            binder.SetBind(Keys.None, Keys.V | Keys.Control, "ScrollDown");

            binder.SetBind(Keys.None, Keys.Oemcomma | Keys.Alt | Keys.Shift, "MoveCaretDefault");

            // --- select ---
            binder.SetBind(Keys.Q | Keys.Control, Keys.A | Keys.Control, "SelectAllChildren");
            binder.SetBind(Keys.X | Keys.Control, Keys.H, "SelectAllChildren");

            // --- clipboard ---
            binder.SetBind(Keys.None, Keys.Y | Keys.Control, "Paste");
            binder.SetBind(Keys.None, Keys.Y | Keys.Control | Keys.Alt, "PasteInBlock");

            // --- undo ---
            binder.SetBind(Keys.None, Keys.OemQuestion | Keys.Control, "Undo");
            binder.SetBind(Keys.None, Keys.OemBackslash | Keys.Control | Keys.Shift, "Undo");
            binder.SetBind(Keys.X | Keys.Control, Keys.U, "Undo");

            // --- misc ---
            binder.SetBind(Keys.X | Keys.Control, Keys.K, "CloseTabPage");
            binder.SetBind(Keys.X | Keys.Control, Keys.C | Keys.Control, "CloseAllTabPages");

            // --- misc ---
            binder.SetBind(Keys.X | Keys.Control, Keys.B | Keys.Control, "SelectOpenNotesNodeInWorkspace");
            binder.SetBind(Keys.X | Keys.Control, Keys.D | Keys.Control, "FocusWorkspaceView");
            binder.SetBind(Keys.X | Keys.Control, Keys.F | Keys.Control, "FocusMemoListView");
        }

        protected override void ResetMemoContentEditorKeyBind(KeyBinder<IEditor> binder) {
            // --- move ---
            binder.SetBind(Keys.None, Keys.Left, "MoveLeft");
            binder.SetBind(Keys.None, Keys.Right, "MoveRight");
            binder.SetBind(Keys.None, Keys.Up, "MoveUp");
            binder.SetBind(Keys.None, Keys.Down, "MoveDown");
            binder.SetBind(Keys.None, Keys.B | Keys.Control, "MoveLeft");
            binder.SetBind(Keys.None, Keys.F | Keys.Control, "MoveRight");
            binder.SetBind(Keys.None, Keys.P | Keys.Control, "MoveUp");
            binder.SetBind(Keys.None, Keys.N | Keys.Control, "MoveDown");

            binder.SetBind(Keys.None, Keys.L | Keys.Control, "ScrollRecenter");

            binder.SetBind(Keys.None, Keys.PageUp, "ScrollUp");
            binder.SetBind(Keys.None, Keys.PageDown, "ScrollDown");
            binder.SetBind(Keys.None, Keys.V | Keys.Alt, "ScrollUp");
            binder.SetBind(Keys.None, Keys.V | Keys.Control, "ScrollDown");

            // --- move out ---
            binder.SetBind(Keys.None, Keys.B | Keys.Control | Keys.Alt, "MoveOutLeft");
            binder.SetBind(Keys.None, Keys.F | Keys.Control | Keys.Alt, "MoveOutRight");
            binder.SetBind(Keys.None, Keys.P | Keys.Control | Keys.Alt, "MoveOutUp");
            binder.SetBind(Keys.None, Keys.N | Keys.Control | Keys.Alt, "MoveOutDown");

            // --- select ---
            binder.SetBind(Keys.None, Keys.Tab, "SelectNextElement");
            binder.SetBind(Keys.None, Keys.Tab | Keys.Shift, "SelectPreviousElement");

            // --- remove ---
            binder.SetBind(Keys.None, Keys.Delete, "Remove");
            binder.SetBind(Keys.None, Keys.D | Keys.Control, "Remove");

            // --- clipboard ---
            binder.SetBind(Keys.None, Keys.W | Keys.Control, "Cut");
            binder.SetBind(Keys.None, Keys.W | Keys.Alt, "Copy");

            // --- focus ---
            binder.SetBind(Keys.None, Keys.Enter, "BeginFocus");
            binder.SetBind(Keys.None, Keys.Enter | Keys.Shift, "BeginFocus");
            binder.SetBind(Keys.None, Keys.Enter | Keys.Control, "BeginFocus");

            // --- undo ---
            binder.SetBind(Keys.None, Keys.OemQuestion | Keys.Control, "Undo");
            binder.SetBind(Keys.None, Keys.OemBackslash | Keys.Control, "Undo");
            binder.SetBind(Keys.X | Keys.Control, Keys.U, "Undo");

            // --- misc ---
            binder.SetBind(Keys.X | Keys.Control, Keys.K, "CloseTabPage");
            binder.SetBind(Keys.X | Keys.Control, Keys.C | Keys.Control, "CloseAllTabPages");

            // --- misc ---
            binder.SetBind(Keys.X | Keys.Control, Keys.B | Keys.Control, "SelectOpenNotesNodeInWorkspace");
            binder.SetBind(Keys.X | Keys.Control, Keys.D | Keys.Control, "FocusWorkspaceView");
            binder.SetBind(Keys.X | Keys.Control, Keys.F | Keys.Control, "FocusMemoListView");
        }

        protected override void ResetMemoTableCellEditorKeyBind(KeyBinder<IEditor> binder) {
            // --- move ---
            binder.SetBind(Keys.None, Keys.Left, "MoveLeftCell");
            binder.SetBind(Keys.None, Keys.Right, "MoveRightCell");
            binder.SetBind(Keys.None, Keys.Up, "MoveUpCell");
            binder.SetBind(Keys.None, Keys.Down, "MoveDownCell");
            binder.SetBind(Keys.None, Keys.B | Keys.Control, "MoveLeftCell");
            binder.SetBind(Keys.None, Keys.F | Keys.Control, "MoveRightCell");
            binder.SetBind(Keys.None, Keys.P | Keys.Control, "MoveUpCell");
            binder.SetBind(Keys.None, Keys.N | Keys.Control, "MoveDownCell");

            binder.SetBind(Keys.None, Keys.Enter, "MoveDownCell");
            binder.SetBind(Keys.None, Keys.Tab, "MoveRightCellOrCreateRow");
            binder.SetBind(Keys.None, Keys.Tab | Keys.Shift, "MoveLeftCell");

            binder.SetBind(Keys.None, Keys.Home, "MoveLeftmostCell");
            binder.SetBind(Keys.None, Keys.End, "MoveRightmostCell");
            binder.SetBind(Keys.None, Keys.A | Keys.Control, "MoveLeftmostCell");
            binder.SetBind(Keys.None, Keys.E | Keys.Control, "MoveRightmostCell");

            binder.SetBind(Keys.None, Keys.L | Keys.Control, "ScrollRecenter");

            binder.SetBind(Keys.None, Keys.PageUp, "ScrollUp");
            binder.SetBind(Keys.None, Keys.PageDown, "ScrollDown");
            binder.SetBind(Keys.None, Keys.V | Keys.Alt, "ScrollUp");
            binder.SetBind(Keys.None, Keys.V | Keys.Control, "ScrollDown");

            // --- empty ---
            binder.SetBind(Keys.None, Keys.Delete, "EmptyCell");
            binder.SetBind(Keys.None, Keys.D | Keys.Control, "EmptyCell");

            // --- focus ---
            binder.SetBind(Keys.None, Keys.Enter | Keys.Shift, "BeginFocus");
            binder.SetBind(Keys.None, Keys.Enter | Keys.Control, "BeginFocus");

            // --- undo ---
            binder.SetBind(Keys.None, Keys.OemQuestion | Keys.Control, "Undo");
            binder.SetBind(Keys.None, Keys.OemBackslash | Keys.Control, "Undo");
            binder.SetBind(Keys.X | Keys.Control, Keys.U, "Undo");

            // --- misc ---
            binder.SetBind(Keys.X | Keys.Control, Keys.K, "CloseTabPage");
            binder.SetBind(Keys.X | Keys.Control, Keys.C | Keys.Control, "CloseAllTabPages");

            // --- misc ---
            binder.SetBind(Keys.X | Keys.Control, Keys.B | Keys.Control, "SelectOpenNotesNodeInWorkspace");
            binder.SetBind(Keys.X | Keys.Control, Keys.D | Keys.Control, "FocusWorkspaceView");
            binder.SetBind(Keys.X | Keys.Control, Keys.F | Keys.Control, "FocusMemoListView");
        }

        protected override void ResetMemoTableCellFocusKeyBind(KeyBinder<IFocus> binder) {
            ResetMemoContentFocusKeyBind(binder);

            binder.SetBind(Keys.None, Keys.Home | Keys.Control, "MoveBeginningOfText");
            binder.SetBind(Keys.None, Keys.End | Keys.Control, "MoveEndOfText");
            binder.SetBind(Keys.None, Keys.Oemcomma | Keys.Alt | Keys.Shift, "MoveBeginningOfText");
            binder.SetBind(Keys.None, Keys.OemPeriod | Keys.Alt | Keys.Shift, "MoveEndOfText");

            // --- move out ---
            binder.SetBind(Keys.None, Keys.B | Keys.Control | Keys.Alt, "MoveOutLeft");
            binder.SetBind(Keys.None, Keys.F | Keys.Control | Keys.Alt, "MoveOutRight");
            binder.SetBind(Keys.None, Keys.P | Keys.Control | Keys.Alt, "MoveOutUp");
            binder.SetBind(Keys.None, Keys.N | Keys.Control | Keys.Alt, "MoveOutDown");

            // --- scroll ---
            binder.SetBind(Keys.None, Keys.L | Keys.Control, "ScrollRecenter");

            // --- tab ---
            binder.SetBind(Keys.X | Keys.Control, Keys.K, "CloseTabPage");
            binder.SetBind(Keys.X | Keys.Control, Keys.C | Keys.Control, "CloseAllTabPages");

            // --- cell ---
            binder.SetBind(Keys.None, Keys.Enter, "CommitAndMoveDownCell");
            binder.SetBind(Keys.None, Keys.Enter | Keys.Alt, "InsertBlockBreak");
            binder.SetBind(Keys.None, Keys.Tab, "CommitAndMoveRightCellOrCreateRow");
            binder.SetBind(Keys.None, Keys.Tab | Keys.Shift, "CommitAndMoveLeftCell");

            // --- misc ---
            binder.SetBind(Keys.X | Keys.Control, Keys.B | Keys.Control, "SelectOpenNotesNodeInWorkspace");
            binder.SetBind(Keys.X | Keys.Control, Keys.D | Keys.Control, "FocusWorkspaceView");
            binder.SetBind(Keys.X | Keys.Control, Keys.F | Keys.Control, "FocusMemoListView");
        }

        protected override void ResetUmlFeatureEditorKeyBind(KeyBinder<IEditor> binder) {
            binder.SetBind(Keys.None, Keys.Up, "SelectPreviousItem");
            binder.SetBind(Keys.None, Keys.Down, "SelectNextItem");
            binder.SetBind(Keys.None, Keys.P | Keys.Control, "SelectPreviousItem");
            binder.SetBind(Keys.None, Keys.N | Keys.Control, "SelectNextItem");

            // --- remove ---
            binder.SetBind(Keys.None, Keys.Delete, "RemoveItem");
            binder.SetBind(Keys.None, Keys.D | Keys.Control, "RemoveItem");

            // --- focus ---
            binder.SetBind(Keys.None, Keys.Enter, "BeginFocus");
            binder.SetBind(Keys.None, Keys.Enter | Keys.Control, "BeginFocus");
            binder.SetBind(Keys.None, Keys.Enter | Keys.Shift, "BeginFocus");

            // --- misc ---
            binder.SetBind(Keys.X | Keys.Control, Keys.B | Keys.Control, "SelectOpenNotesNodeInWorkspace");
            binder.SetBind(Keys.X | Keys.Control, Keys.D | Keys.Control, "FocusWorkspaceView");
            binder.SetBind(Keys.X | Keys.Control, Keys.F | Keys.Control, "FocusMemoListView");
        }
    }
}
