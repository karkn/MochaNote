/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Memopad.Internal.Controls;
using System.Windows.Forms;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Model.Memo;
using Mkamo.Common.Forms.KeyMap;
using System.Reflection;

namespace Mkamo.Memopad.Internal.KeyActions {
    [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = true)]
    internal static class PageContentTitleTextBoxKeyActions {
        // ========================================
        // static field
        // ========================================
        [KeyAction("")]
        public static void FocusEditorCanvas(TextBox titleTextBox) {
            var pageContent = (PageContent) titleTextBox.Parent;
            var canvas = pageContent.EditorCanvas;
            canvas.Select();
        }

        // --- misc ---
        [KeyAction("")]
        public static void CloseTabPage(TextBox titleTextBox) {
            var facade = MemopadApplication.Instance;
            var content =  titleTextBox.Parent as PageContent;
            var canvas = content.EditorCanvas;
            var memo = canvas.EditorContent as Memo;
            var memoInfo = facade.FindMemoInfo(memo);
            if (memoInfo != null) {
                facade.CloseMemo(memoInfo);
            }
        }

        [KeyAction("")]
        public static void CloseAllTabPages(TextBox titleTextBox) {
            var facade = MemopadApplication.Instance;
            facade.CloseAllMemos();
        }


    }
}
