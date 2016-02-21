/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Model.Memo;
using Mkamo.Common.Forms.KeyMap;
using System.Reflection;

namespace Mkamo.Memopad.Internal.KeyActions {
    [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = true)]
    internal static class NoSelectionKeyActions {
        [KeyAction("")]
        public static void Undo(EditorCanvas canvas) {
            canvas.CommandExecutor.Undo();
        }

        [KeyAction("")]
        public static void Redo(EditorCanvas canvas) {
            canvas.CommandExecutor.Redo();
        }

        [KeyAction("")]
        public static void SelectAllChildren(EditorCanvas canvas) {
            foreach (var child in canvas.RootEditor.Content.Children) {
                child.RequestSelect(SelectKind.True, false);
            }
        }

        // --- misc ---
        [KeyAction("")]
        public static void CloseTabPage(EditorCanvas canvas) {
            var facade = MemopadApplication.Instance;
            var memo = canvas.EditorContent as Memo;
            var memoInfo = facade.FindMemoInfo(memo);
            if (memoInfo != null) {
                facade.CloseMemo(memoInfo);
            }
        }

        [KeyAction("")]
        public static void CloseAllTabPages(EditorCanvas canvas) {
            var facade = MemopadApplication.Instance;
            facade.CloseAllMemos();
        }


        [KeyAction("")]
        public static void ScrollUp(EditorCanvas canvas) {
            canvas.ScrollUp();
        }

        [KeyAction("")]
        public static void ScrollDown(EditorCanvas canvas) {
            canvas.ScrollDown();
        }
    }
}
