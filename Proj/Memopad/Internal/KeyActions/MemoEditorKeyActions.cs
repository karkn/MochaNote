/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Memopad.Internal.Utils;
using System.Drawing;
using Mkamo.Editor.Core;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Model.Memo;
using Mkamo.Memopad.Internal.Helpers;
using Mkamo.Memopad.Core;
using Mkamo.Common.Forms.KeyMap;
using System.Reflection;

namespace Mkamo.Memopad.Internal.KeyActions {
    [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = true)]
    internal static class MemoEditorKeyActions {
        // ========================================
        // static field
        // ========================================

        // ========================================
        // method
        // ========================================
        [KeyAction("")]
        public static void DoNothing(IEditor editor) {
        }        

        [KeyAction("貼り付け(改行文字を改段落)")]
        public static void Paste(IEditor editor) {
            MemoEditorHelper.Paste(editor, false);
        }

        [KeyAction("貼り付け(改行文字を改行)")]
        public static void PasteInBlock(IEditor editor) {
            MemoEditorHelper.Paste(editor, true);
        }

        // --- undo ---
        [KeyAction("")]
        public static void Undo(IEditor editor) {
            var canvas = editor.Site.EditorCanvas;
            canvas.CommandExecutor.Undo();
        }

        [KeyAction("")]
        public static void Redo(IEditor editor) {
            var canvas = editor.Site.EditorCanvas;
            canvas.CommandExecutor.Redo();
        }

        // --- move ---
        [KeyAction("")]
        public static void MoveCaretUp(IEditor editor) {
            var caret = editor.Site.EditorCanvas.Caret;
            var expectedMemoPos = CaretUtil.GetExpectedMemoTextPosition(caret.Position);
            var newExpectedMemoPos = new Point(expectedMemoPos.X, expectedMemoPos.Y - MemopadConsts.CaretMoveDelta);
            caret.Position = CaretUtil.GetExpectedCaretPosition(
                newExpectedMemoPos, editor.Site.GridService
            );
            FocusMemoText(editor, caret.Position);
        }

        [KeyAction("")]
        public static void MoveCaretDown(IEditor editor) {
            var caret = editor.Site.EditorCanvas.Caret;
            var expectedMemoPos = CaretUtil.GetExpectedMemoTextPosition(caret.Position);
            var newExpectedMemoPos = new Point(expectedMemoPos.X, expectedMemoPos.Y + MemopadConsts.CaretMoveDelta);
            caret.Position = CaretUtil.GetExpectedCaretPosition(
                newExpectedMemoPos, editor.Site.GridService
            );
            FocusMemoText(editor, caret.Position);
        }

        [KeyAction("")]
        public static void MoveCaretLeft(IEditor editor) {
            var caret = editor.Site.EditorCanvas.Caret;
            var expectedMemoPos = CaretUtil.GetExpectedMemoTextPosition(caret.Position);
            var newExpectedMemoPos = new Point(expectedMemoPos.X - MemopadConsts.CaretMoveDelta, expectedMemoPos.Y);
            caret.Position = CaretUtil.GetExpectedCaretPosition(
                newExpectedMemoPos, editor.Site.GridService
            );
            FocusMemoText(editor, caret.Position);
        }

        [KeyAction("")]
        public static void MoveCaretRight(IEditor editor) {
            var caret = editor.Site.EditorCanvas.Caret;
            var expectedMemoPos = CaretUtil.GetExpectedMemoTextPosition(caret.Position);
            var newExpectedMemoPos = new Point(expectedMemoPos.X + MemopadConsts.CaretMoveDelta, expectedMemoPos.Y);
            caret.Position = CaretUtil.GetExpectedCaretPosition(
                newExpectedMemoPos, editor.Site.GridService
            );
            FocusMemoText(editor, caret.Position);
        }

        [KeyAction("")]
        public static void MoveCaretLeftMost(IEditor editor) {
            var caret = editor.Site.EditorCanvas.Caret;
            caret.Position = new Point(MemopadConsts.DefaultCaretPosition.X, caret.Top);
            FocusMemoText(editor, caret.Position);
        }

        [KeyAction("")]
        public static void MoveCaretRightMost(IEditor editor) {
            var caret = editor.Site.EditorCanvas.Caret;
            caret.Position = new Point(editor.Site.EditorCanvas.ClientSize.Width - 36, caret.Top);
            FocusMemoText(editor, caret.Position);
        }

        [KeyAction("")]
        public static void MoveCaretDefault(IEditor editor) {
            var caret = editor.Site.EditorCanvas.Caret;
            caret.Position = MemopadConsts.DefaultCaretPosition;
            FocusMemoText(editor, caret.Position);
        }

        // --- select ---
        [KeyAction("")]
        public static void SelectAllChildren(IEditor editor) {
            foreach (var child in editor.Children) {
                child.RequestSelect(SelectKind.True, false);
            }
        }

        // --- misc ---
        [KeyAction("")]
        public static void CloseTabPage(IEditor editor) {
            var facade = MemopadApplication.Instance;
            var memo = editor.Site.EditorCanvas.EditorContent as Memo;
            var memoInfo = facade.FindMemoInfo(memo);
            if (memoInfo != null) {
                facade.CloseMemo(memoInfo);
            }
        }

        [KeyAction("")]
        public static void CloseAllTabPages(IEditor editor) {
            var facade = MemopadApplication.Instance;
            facade.CloseAllMemos();
        }


        [KeyAction("")]
        public static void ScrollUp(IEditor editor) {
            var canvas = editor.Site.EditorCanvas;
            canvas.ScrollUp();
        }

        [KeyAction("")]
        public static void ScrollDown(IEditor editor) {
            var canvas = editor.Site.EditorCanvas;
            canvas.ScrollDown();
        }

        [KeyAction("キャレット位置をセンター")]
        public static void ScrollRecenter(IEditor editor) {
            editor.Site.EditorCanvas.ScrollRecenter();
        }

        // ------------------------------
        // private
        // ------------------------------
        private static void FocusMemoText(IEditor editor, Point pt) {
            foreach (var child in editor.Children) {
                var loc = pt + new Size(1, 1);
                if (child.Model is MemoText && child.Figure.ContainsPoint(loc)) {
                    child.RequestSelect(SelectKind.True, true);
                    child.RequestFocus(FocusKind.Begin, loc);
                }
            }
        }


    }
}
