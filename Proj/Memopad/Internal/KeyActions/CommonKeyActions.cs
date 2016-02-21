/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Mkamo.Common.Forms.KeyMap;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Editor.Core;

namespace Mkamo.Memopad.Internal.KeyActions {
    [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = true)]
    internal class CommonKeyActions<T> {
        [KeyAction("ワークスペースの開いているノートを選択")]
        public static void SelectOpenNotesNodeInWorkspace(T obj) {
            var app = MemopadApplication.Instance;
            var tree = app.MainForm.WorkspaceView.WorkspaceTree;
            tree.SelectedNode = tree._OpenMemosNode;
            tree.Select();
        }

        [KeyAction("ワークスペースを選択")]
        public static void FocusWorkspaceView(T obj) {
            var app = MemopadApplication.Instance;
            app.MainForm.WorkspaceView.WorkspaceTree.Select();
        }

        [KeyAction("メモリストを選択")]
        public static void FocusMemoListView(T obj) {
            var app = MemopadApplication.Instance;
            app.MainForm._MemoListView.MemoListBox.Select();
        }
    }
}
