/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Mkamo.Memopad.Internal.Utils;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Editor.Requests;
using Mkamo.Editor.Core;
using Mkamo.Editor.Focuses;
using Mkamo.Model.Memo;
using Mkamo.Common.Command;
using Mkamo.Memopad.Internal.Helpers;
using Mkamo.Common.Forms.KeyMap;
using System.Reflection;

namespace Mkamo.Memopad.Internal.KeyActions {
    [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = true)]
    internal static class MemoContentFocusKeyActions {
        // ========================================
        // static method
        // ========================================
        [KeyAction("キャレットを文頭に移動")]
        public static void MoveBeginningOfText(IFocus focus) {
            var stFocus = (StyledTextFocus) focus;
            stFocus.MoveBeginningOfText();
            focus.Host.Site.EditorCanvas.ScrollRecenter();
        }

        [KeyAction("キャレットを文末に移動")]
        public static void MoveEndOfText(IFocus focus) {
            var stFocus = (StyledTextFocus) focus;
            stFocus.MoveEndOfText();
            focus.Host.Site.EditorCanvas.ScrollRecenter();
        }

        //public static void MoveDown(IFocus focus) {
        //    var stFocus = (StyledTextFocus) focus;
        //    if (focus.Figure.IsLastVisualLine(stFocus.Referer.CaretIndex)) {
        //        MoveOutDown(focus);
        //        var caret = focus.Host.Site.EditorCanvas.Caret;
        //        FocusMemoText(focus.Host.Parent, caret.Position);
        //    } else {
        //        stFocus.MoveNextLine();
        //    }
        //}

        //public static void MoveUp(IFocus focus) {
        //    var stFocus = (StyledTextFocus) focus;
        //    if (focus.Figure.IsFirstVisualLine(stFocus.Referer.CaretIndex)) {
        //        MoveOutUp(focus);
        //        var caret = focus.Host.Site.EditorCanvas.Caret;
        //        FocusMemoText(focus.Host.Parent, caret.Position);
        //    } else {
        //        stFocus.MovePreviousLine();
        //    }
        //}

        [KeyAction("確定してキャレットを下に移動")]
        public static void MoveOutDown(IFocus focus) {
            var host = focus.Host;
            var pos = host.Site.Caret.Position;

            host.RequestFocusCommit(true);
            host.Parent.RequestSelect(SelectKind.True, true);
            host.Site.Caret.Position = host.Site.GridService.GetAdjustedPoint(new Point(pos.X, host.Figure.Bottom - 1 + 8));
        }

        [KeyAction("確定してキャレットを上に移動")]
        public static void MoveOutUp(IFocus focus) {
            var host = focus.Host;
            var pos = host.Site.Caret.Position;

            host.RequestFocusCommit(true);
            host.Parent.RequestSelect(SelectKind.True, true);
            host.Site.Caret.Position = host.Site.GridService.GetAdjustedPoint(new Point(pos.X, host.Figure.Top - 16));
        }

        [KeyAction("確定してキャレットを左に移動")]
        public static void MoveOutLeft(IFocus focus) {
            var host = focus.Host;
            var pos = host.Site.Caret.Position;

            host.RequestFocusCommit(true);
            host.Parent.RequestSelect(SelectKind.True, true);
            host.Site.Caret.Position = host.Site.GridService.GetAdjustedPoint(new Point(host.Figure.Left - 8, pos.Y));
        }

        [KeyAction("確定してキャレットを右に移動")]
        public static void MoveOutRight(IFocus focus) {
            var host = focus.Host;
            var pos = host.Site.Caret.Position;

            host.RequestFocusCommit(true);
            host.Parent.RequestSelect(SelectKind.True, true);
            host.Site.Caret.Position = host.Site.GridService.GetAdjustedPoint(new Point(host.Figure.Right - 1 + 8, pos.Y));
        }


        [KeyAction("タブを閉じる")]
        public static void CloseTabPage(IFocus focus) {
            var editor = focus.Host;
            var facade = MemopadApplication.Instance;
            var memo = editor.Site.EditorCanvas.EditorContent as Memo;
            var memoInfo = facade.FindMemoInfo(memo);
            if (memoInfo != null) {
                facade.CloseMemo(memoInfo);
            }
        }

        [KeyAction("すべてのタブを閉じる")]
        public static void CloseAllTabPages(IFocus focus) {
            var facade = MemopadApplication.Instance;
            facade.CloseAllMemos();
        }

        [KeyAction("コメントを追加")]
        public static void AddComment(IFocus focus) {
            var stfocus = focus as StyledTextFocus;
            if (stfocus != null) {
                MemoEditorHelper.AddCommentForMemoText(stfocus);
            }
        }

        [KeyAction("特殊箇条書きトグル")]
        public static void ToggleSpecialList(IFocus focus) {
            var app = MemopadApplication.Instance;
            app.MainForm.ToggleSpecialList();
        }

        [KeyAction("キャレット位置をセンター")]
        public static void ScrollRecenter(IFocus focus) {
            focus.Host.Site.EditorCanvas.ScrollRecenter();
        }

        // ------------------------------
        // private
        // ------------------------------
        private static void FocusMemoText(IEditor editor, Point pt) {
            foreach (var child in editor.Children) {
                if (child.Model is MemoText && child.Figure.ContainsPoint(pt)) {
                    child.RequestFocus(FocusKind.Begin, pt);
                }
            }
        }


    }
}
