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
using Mkamo.Common.Forms.KeyMap;
using System.Reflection;

namespace Mkamo.Memopad.Internal.KeyActions {
    [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = true)]
    internal static class MemoTableCellFocusKeyActions {
        // ========================================
        // static method
        // ========================================
        [KeyAction("")]
        public static void CommitAndMoveDownCell(IFocus focus) {
            var editor = focus.Host;
            editor.RequestFocusCommit(true);
            MemoTableCellEditorKeyActions.MoveDownCell(editor);
        }

        [KeyAction("")]
        public static void CommitAndMoveRightCellOrCreateRow(IFocus focus) {
            var editor = focus.Host;
            editor.RequestFocusCommit(true);
            MemoTableCellEditorKeyActions.MoveRightCellOrCreateRow(editor);
        }

        [KeyAction("")]
        public static void CommitAndMoveLeftCell(IFocus focus) {
            var editor = focus.Host;
            editor.RequestFocusCommit(true);
            MemoTableCellEditorKeyActions.MoveLeftCell(editor);
        }

        [KeyAction("")]
        public static void InsertBlockBreak(IFocus focus) {
            var stfocus = (StyledTextFocus) focus;
            stfocus.InsertBlockBreak();
        }
    }
}
