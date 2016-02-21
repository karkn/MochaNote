/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Memopad.Internal.Core;

namespace Mkamo.Memopad.Internal.Utils {
    internal static class KeyActionUtil {
        public static void SelectOpenNotesNodeInWorkspace() {
            var app = MemopadApplication.Instance;
            var tree = app.MainForm.WorkspaceView.WorkspaceTree;
            tree.SelectedNode = tree._OpenMemosNode;
            tree.Select();
        }

        public static void FocusWorkspace() {
            var app = MemopadApplication.Instance;
            app.MainForm.WorkspaceView.WorkspaceTree.Select();
        }

        public static void FocusMemoList() {
            var app = MemopadApplication.Instance;
            app.MainForm._MemoListView.MemoListBox.Select();
        }

    }
}
