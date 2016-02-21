/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Memopad.Internal.Controls;
using Mkamo.Common.Forms.KeyMap;
using System.Reflection;

namespace Mkamo.Memopad.Internal.KeyActions {
    [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = true)]
    internal static class MemoListViewKeyActions {
        // ========================================
        // method
        // ========================================
        [KeyAction("")]
        public static void LoadSelectedMemos(MemoListView view) {
            view.LoadSelectedMemos();
        }

        [KeyAction("")]
        public static void RemoveSelectedMemos(MemoListView view) {
            /// 削除確認ダイアログがプレビューに隠れてしまわないようにする
            view.MemoListBox.ClosePreviewPopup();

            switch (view.TargetKind) {
                case Mkamo.Memopad.Core.MemoListTargetKind.SmartFolder:
                case Mkamo.Memopad.Core.MemoListTargetKind.Tag:
                case Mkamo.Memopad.Core.MemoListTargetKind.QueryBuilder:
                case Mkamo.Memopad.Core.MemoListTargetKind.OpenMemos:
                    view.RemoveSelectedMemos();
                    break;
                case Mkamo.Memopad.Core.MemoListTargetKind.Folder:
                    view.RemoveSelectedMemosFromFolder();
                    break;
                case Mkamo.Memopad.Core.MemoListTargetKind.TrashBox:
                    view.RemoveSelectedMemosCompletely();
                    break;
            }
        }

        [KeyAction("")]
        public static void SelectNextItem(MemoListView view) {
            view.SelectNextItem();
        }

        [KeyAction("")]
        public static void SelectPreviousItem(MemoListView view) {
            view.SelectPreviousItem();
        }

        [KeyAction("")]
        public static void SelectAll(MemoListView view) {
            view.SelectAll();
        }

    }
}
