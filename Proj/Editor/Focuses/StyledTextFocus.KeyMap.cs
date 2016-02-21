/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mkamo.Editor.Core;
using Mkamo.Editor.Commands;
using Mkamo.StyledText.Util;
using Mkamo.StyledText.Core;
using System.Drawing;
using Mkamo.Common.Forms.Descriptions;

namespace Mkamo.Editor.Focuses {

    partial class StyledTextFocus {
        // ========================================
        // method
        // ========================================
        // --- keymap ---
        public void ClearKeyMap() {
            KeyMap.Clear();
        }

        public void ResetDefaultKeyMap() {
            ClearKeyMap();

            // --- move ---
            KeyMap.SetAction(Keys.Left, (focus) => MoveBackwardChar());
            KeyMap.SetAction(Keys.Right, (focus) => MoveForwardChar());
            KeyMap.SetAction(Keys.Up, (focus) => MovePreviousLine());
            KeyMap.SetAction(Keys.Down, (focus) => MoveNextLine());

            KeyMap.SetAction(Keys.PageUp, (focus) => MovePreviousPage());
            KeyMap.SetAction(Keys.PageDown, (focus) => MoveNextPage());

            KeyMap.SetAction(Keys.Home, (focus) => MoveBeginningOfLine());
            KeyMap.SetAction(Keys.End, (focus) => MoveEndOfLine());

            KeyMap.SetAction(Keys.Home | Keys.Control, (focus) => MoveBeginningOfText());
            KeyMap.SetAction(Keys.End | Keys.Control, (focus) => MoveEndOfText());

            KeyMap.SetAction(Keys.Left | Keys.Control, (focus) => MovePreviousWord());
            KeyMap.SetAction(Keys.Right | Keys.Control, (focus) => MoveNextWord());

            // --- select ---
            KeyMap.SetAction(Keys.Left | Keys.Shift, (focus) => SelectBackwardChar());
            KeyMap.SetAction(Keys.Right | Keys.Shift, (focus) => SelectForwardChar());
            KeyMap.SetAction(Keys.Up | Keys.Shift, (focus) => SelectPreviousLine());
            KeyMap.SetAction(Keys.Down | Keys.Shift, (focus) => SelectNextLine());

            KeyMap.SetAction(Keys.A| Keys.Control, (focus) => SelectAll());

            // --- enter ---
            KeyMap.SetAction(Keys.Enter, (focus) => InsertBlockBreak());
            KeyMap.SetAction(Keys.Enter | Keys.Shift, (focus) => InsertLineBreak());
            KeyMap.SetAction(Keys.Enter | Keys.Control, (focus) => CommitAndSelect());

            // --- insert ---
            KeyMap.SetAction(Keys.Space | Keys.Control, (focus) => InsertDynamicAbbrev());

            // --- clipboard ---
            KeyMap.SetAction(Keys.V | Keys.Control, (focus) => PasteInlinesOrText(false));
            KeyMap.SetAction(Keys.X | Keys.Control, (focus) => Cut());
            KeyMap.SetAction(Keys.C | Keys.Control, (focus) => Copy());

            // --- undo ---
            KeyMap.SetAction(Keys.Z | Keys.Control, (focus) => Undo());
            KeyMap.SetAction(Keys.Y | Keys.Control, (focus) => Redo());

            // --- remove ---
            KeyMap.SetAction(Keys.Delete, (focus) => RemoveForward());
            KeyMap.SetAction(Keys.Back, (focus) => RemoveBackward());

            // --- list ---
            KeyMap.SetAction(Keys.L | Keys.Control | Keys.Shift, (focus) => ToggleUnorderedList());
            KeyMap.SetAction(Keys.O | Keys.Control | Keys.Shift, (focus) => ToggleOrderedList());
            /// MemopadFormの値を使うのでStyledTextFocusレベルではバインドできない
            ///KeyMap.SetAction(Keys.P | Keys.Control | Keys.Shift, (focus) => ToggleSpecialList());

            KeyMap.SetAction(Keys.Space | Keys.Control | Keys.Shift, (focus) => ChangeToNextListState());

            // --- indent ---
            KeyMap.SetAction(Keys.Tab, (focus) => Indent());
            KeyMap.SetAction(Keys.Tab | Keys.Shift, (focus) => Outdent());

            // --- paragraph ---
            KeyMap.SetAction(Keys.D1 | Keys.Shift | Keys.Alt, (focus) => SetParagraphKind(ParagraphKind.Heading1));
            KeyMap.SetAction(Keys.D2 | Keys.Shift | Keys.Alt, (focus) => SetParagraphKind(ParagraphKind.Heading2));
            KeyMap.SetAction(Keys.D3 | Keys.Shift | Keys.Alt, (focus) => SetParagraphKind(ParagraphKind.Heading3));
            KeyMap.SetAction(Keys.D4 | Keys.Shift | Keys.Alt, (focus) => SetParagraphKind(ParagraphKind.Heading4));
            KeyMap.SetAction(Keys.D5 | Keys.Shift | Keys.Alt, (focus) => SetParagraphKind(ParagraphKind.Heading5));
            KeyMap.SetAction(Keys.D6 | Keys.Shift | Keys.Alt, (focus) => SetParagraphKind(ParagraphKind.Heading6));
            KeyMap.SetAction(Keys.D0 | Keys.Shift | Keys.Alt, (focus) => SetParagraphKind(ParagraphKind.Normal));

            // --- style ---
            KeyMap.SetAction(Keys.B | Keys.Control, (focus) => SetFont((FontDescription font) => FontDescription.GetBoldToggled(font)));
            KeyMap.SetAction(Keys.I | Keys.Control, (focus) => SetFont((FontDescription font) => FontDescription.GetItalicToggled(font)));
            KeyMap.SetAction(Keys.U | Keys.Control, (focus) => SetFont((FontDescription font) => FontDescription.GetUnderlineToggled(font)));
        }

        public void ResetEmacsKeyMap() {
            ClearKeyMap();

            var xprefix = KeyMap.SetPrefix(Keys.X | Keys.Control);
            var qprefix = KeyMap.SetPrefix(Keys.Q | Keys.Control);

            // --- move ---
            KeyMap.SetAction(Keys.Left, (focus) => MoveBackwardChar());
            KeyMap.SetAction(Keys.Right, (focus) => MoveForwardChar());
            KeyMap.SetAction(Keys.Up, (focus) => MovePreviousLine());
            KeyMap.SetAction(Keys.Down, (focus) => MoveNextLine());

            KeyMap.SetAction(Keys.B | Keys.Control, (focus) => MoveBackwardChar());
            KeyMap.SetAction(Keys.F | Keys.Control, (focus) => MoveForwardChar());
            KeyMap.SetAction(Keys.P | Keys.Control, (focus) => MovePreviousLine());
            KeyMap.SetAction(Keys.N | Keys.Control, (focus) => MoveNextLine());

            KeyMap.SetAction(Keys.PageUp, (focus) => MovePreviousPage());
            KeyMap.SetAction(Keys.PageDown, (focus) => MoveNextPage());
            KeyMap.SetAction(Keys.V | Keys.Alt, (focus) => MovePreviousPage());
            KeyMap.SetAction(Keys.V | Keys.Control, (focus) => MoveNextPage());

            KeyMap.SetAction(Keys.A | Keys.Control, (focus) => MoveBeginningOfLine());
            KeyMap.SetAction(Keys.E | Keys.Control, (focus) => MoveEndOfLine());
            KeyMap.SetAction(Keys.Oemcomma | Keys.Alt | Keys.Shift, (focus) => MoveBeginningOfText());
            KeyMap.SetAction(Keys.OemPeriod | Keys.Alt | Keys.Shift, (focus) => MoveEndOfText());

            KeyMap.SetAction(Keys.Left | Keys.Control, (focus) => MovePreviousWord());
            KeyMap.SetAction(Keys.Right | Keys.Control, (focus) => MoveNextWord());
            KeyMap.SetAction(Keys.B | Keys.Alt, (focus) => MovePreviousWord());
            KeyMap.SetAction(Keys.F | Keys.Alt, (focus) => MoveNextWord());

            // --- select ---
            KeyMap.SetAction(Keys.Left | Keys.Shift, (focus) => SelectBackwardChar());
            KeyMap.SetAction(Keys.Right | Keys.Shift, (focus) => SelectForwardChar());
            KeyMap.SetAction(Keys.Up | Keys.Shift, (focus) => SelectPreviousLine());
            KeyMap.SetAction(Keys.Down | Keys.Shift, (focus) => SelectNextLine());

            xprefix.SetAction(Keys.H, (focus) => SelectAll());

            // --- mark ---
            KeyMap.SetAction(Keys.Space | Keys.Control, (focus) => SetMark());
            KeyMap.SetAction(Keys.Oemtilde | Keys.Control, (focus) => SetMark());
            xprefix.SetAction(Keys.Space | Keys.Control, (focus) => PopMark());
            xprefix.SetAction(Keys.X | Keys.Control, (focus) => ExchangeCaretAndMark());

            // --- enter ---
            KeyMap.SetAction(Keys.Enter, (focus) => InsertBlockBreak());
            KeyMap.SetAction(Keys.Enter | Keys.Shift, (focus) => InsertLineBreak());
            KeyMap.SetAction(Keys.Enter | Keys.Control, (focus) => CommitAndSelect());

            KeyMap.SetAction(Keys.M | Keys.Control, (focus) => InsertBlockBreak());
            KeyMap.SetAction(Keys.J | Keys.Control, (focus) => InsertLineBreak());
            KeyMap.SetAction(Keys.O | Keys.Control, (focus) => OpenLineBreak());

            // --- insert ---
            KeyMap.SetAction(Keys.OemQuestion | Keys.Alt, (focus) => InsertDynamicAbbrev());

            // --- clipboard ---
            KeyMap.SetAction(Keys.Y | Keys.Control, (focus) => PasteInlinesOrText(false));
            KeyMap.SetAction(Keys.Y | Keys.Control | Keys.Alt, (focus) => PasteInlinesOrText(true));
            KeyMap.SetAction(Keys.W | Keys.Control, (focus) => CutRegion());
            KeyMap.SetAction(Keys.W | Keys.Alt, (focus) => CopyRegion());
            KeyMap.SetAction(Keys.C | Keys.Control, (focus) => Copy());

            // --- kill word ---
            KeyMap.SetAction(Keys.D | Keys.Alt, (focus) => KillWord());

            // --- kill line ---
            var killLinePrefix = KeyMap.SetPrefix(
                Keys.K | Keys.Control,
                (key, focus) => KillLineFirst(),
                (key, focus) => {
                    if (KeyMap.IsDefined(key)) {
                        var action = KeyMap.GetAction(key);
                        if (action != null) {
                            action(focus);
                        }
                    }
                },
                (key, focus) => key != (Keys.K | Keys.Control),
                false
            );
            killLinePrefix.SetAction(Keys.K | Keys.Control, (focus) => KillLine());

            // --- undo ---
            KeyMap.SetAction(Keys.OemQuestion | Keys.Control, (focus) => Undo());
            KeyMap.SetAction(Keys.OemBackslash | Keys.Control | Keys.Shift, focus => Undo());
            xprefix.SetAction(Keys.U, (focus) => Undo());

            // --- remove ---
            KeyMap.SetAction(Keys.Delete, (focus) => RemoveForward());
            KeyMap.SetAction(Keys.Back, (focus) => RemoveBackward());
            KeyMap.SetAction(Keys.D | Keys.Control, (focus) => RemoveForward());
            KeyMap.SetAction(Keys.H | Keys.Control, (focus) => RemoveBackward());

            // --- list ---
            KeyMap.SetAction(Keys.L | Keys.Control | Keys.Shift, (focus) => ToggleUnorderedList());
            KeyMap.SetAction(Keys.O | Keys.Control | Keys.Shift, (focus) => ToggleOrderedList());
            /// MemopadFormの値を使うのでStyledTextFocusレベルではバインドできない
            ///KeyMap.SetAction(Keys.P | Keys.Control | Keys.Shift, (focus) => ToggleSpecialList());

            KeyMap.SetAction(Keys.Space | Keys.Control | Keys.Shift, (focus) => ChangeToNextListState());

            // --- indent ---
            KeyMap.SetAction(Keys.Tab, (focus) => Indent());
            KeyMap.SetAction(Keys.Tab | Keys.Shift, (focus) => Outdent());
            KeyMap.SetAction(Keys.I | Keys.Control, (focus) => Indent());
            KeyMap.SetAction(Keys.I | Keys.Control | Keys.Shift, (focus) => Outdent());

            // --- paragraph ---
            KeyMap.SetAction(Keys.D1 | Keys.Shift | Keys.Alt, (focus) => SetParagraphKind(ParagraphKind.Heading1));
            KeyMap.SetAction(Keys.D2 | Keys.Shift | Keys.Alt, (focus) => SetParagraphKind(ParagraphKind.Heading2));
            KeyMap.SetAction(Keys.D3 | Keys.Shift | Keys.Alt, (focus) => SetParagraphKind(ParagraphKind.Heading3));
            KeyMap.SetAction(Keys.D4 | Keys.Shift | Keys.Alt, (focus) => SetParagraphKind(ParagraphKind.Heading4));
            KeyMap.SetAction(Keys.D5 | Keys.Shift | Keys.Alt, (focus) => SetParagraphKind(ParagraphKind.Heading5));
            KeyMap.SetAction(Keys.D6 | Keys.Shift | Keys.Alt, (focus) => SetParagraphKind(ParagraphKind.Heading6));
            KeyMap.SetAction(Keys.D0 | Keys.Shift | Keys.Alt, (focus) => SetParagraphKind(ParagraphKind.Normal));

            // --- style ---
            qprefix.SetAction(Keys.B | Keys.Control, (focus) => SetFont((FontDescription font) => FontDescription.GetBoldToggled(font)));
            qprefix.SetAction(Keys.I | Keys.Control, (focus) => SetFont((FontDescription font) => FontDescription.GetItalicToggled(font)));
            qprefix.SetAction(Keys.U | Keys.Control, (focus) => SetFont((FontDescription font) => FontDescription.GetUnderlineToggled(font)));
        }

        public void ResetDefaultSingleLineKeyMap() {
            ClearKeyMap();

            // --- move ---
            KeyMap.SetAction(Keys.Left, (focus) => MoveBackwardChar());
            KeyMap.SetAction(Keys.Right, (focus) => MoveForwardChar());

            // --- select ---
            KeyMap.SetAction(Keys.Left | Keys.Shift, (focus) => SelectBackwardChar());
            KeyMap.SetAction(Keys.Right | Keys.Shift, (focus) => SelectForwardChar());

            // --- enter ---
            KeyMap.SetAction(Keys.Enter, (focus) => CommitAndSelect());
            KeyMap.SetAction(Keys.Enter | Keys.Shift, (focus) => CommitAndSelect());
            KeyMap.SetAction(Keys.Enter | Keys.Control, (focus) => CommitAndSelect());

            // --- clipboard ---
            KeyMap.SetAction(Keys.V | Keys.Control, (focus) => PasteLastLine());
            KeyMap.SetAction(Keys.X | Keys.Control, (focus) => Cut());
            KeyMap.SetAction(Keys.C | Keys.Control, (focus) => Copy());

            // --- undo ---
            KeyMap.SetAction(Keys.Z | Keys.Control, (focus) => Undo());
            KeyMap.SetAction(Keys.Y | Keys.Control, (focus) => Redo());

            // --- remove ---
            KeyMap.SetAction(Keys.Delete, (focus) => RemoveForward());
            KeyMap.SetAction(Keys.Back, (focus) => RemoveBackward());
        }


        public void ResetEmacsSingleLineKeyMap() {
            ClearKeyMap();

            // --- move ---
            KeyMap.SetAction(Keys.Left, (focus) => MoveBackwardChar());
            KeyMap.SetAction(Keys.Right, (focus) => MoveForwardChar());
            KeyMap.SetAction(Keys.B | Keys.Control, (focus) => MoveBackwardChar());
            KeyMap.SetAction(Keys.F | Keys.Control, (focus) => MoveForwardChar());

            KeyMap.SetAction(Keys.A | Keys.Control, (focus) => MoveBeginningOfLine());
            KeyMap.SetAction(Keys.E | Keys.Control, (focus) => MoveEndOfLine());

            // --- select ---
            KeyMap.SetAction(Keys.Left | Keys.Shift, (focus) => SelectBackwardChar());
            KeyMap.SetAction(Keys.Right | Keys.Shift, (focus) => SelectForwardChar());

            KeyMap.SetAction(Keys.Space | Keys.Control, (focus) => SetMark());

            // --- enter ---
            KeyMap.SetAction(Keys.Enter, (focus) => CommitAndSelect());
            KeyMap.SetAction(Keys.Enter | Keys.Shift, (focus) => CommitAndSelect());
            KeyMap.SetAction(Keys.Enter | Keys.Control, (focus) => CommitAndSelect());

            // --- clipboard ---
            KeyMap.SetAction(Keys.Y | Keys.Control, (focus) => PasteLastLine());
            KeyMap.SetAction(Keys.W | Keys.Control, (focus) => CutRegion());
            KeyMap.SetAction(Keys.W | Keys.Alt, (focus) => CopyRegion());

            // --- undo ---
            KeyMap.SetAction(Keys.Z | Keys.Control, (focus) => Undo());

            // --- remove ---
            KeyMap.SetAction(Keys.Delete, (focus) => RemoveForward());
            KeyMap.SetAction(Keys.Back, (focus) => RemoveBackward());
            KeyMap.SetAction(Keys.D | Keys.Control, (focus) => RemoveForward());
            KeyMap.SetAction(Keys.H | Keys.Control, (focus) => RemoveBackward());
        }

        public void CommitAndSelect() {
            var cmd = Host.RequestFocus(FocusKind.Commit, null) as FocusCommand;
            if (cmd != null && cmd.ResultKind != FocusCommitResultKind.Canceled) {
                Host.RequestSelect(SelectKind.True, true);
            }
        }
    }
}
