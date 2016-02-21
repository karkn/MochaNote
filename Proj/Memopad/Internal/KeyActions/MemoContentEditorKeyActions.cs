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
using Mkamo.Model.Memo;
using Mkamo.Common.Command;
using Mkamo.Common.Forms.KeyMap;
using System.Reflection;

namespace Mkamo.Memopad.Internal.KeyActions {
    [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = true)]
    internal static class MemoContentEditorKeyActions {
        // ========================================
        // static field
        // ========================================
        public const int MoveDelta = 8;

        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        // --- undo ---
        [KeyAction("")]
        public static void Undo(IEditor editor) {
            var canvas = editor.Site.EditorCanvas;
            if (canvas != null) {
                canvas.CommandExecutor.Undo();
            }
        }

        [KeyAction("")]
        public static void Redo(IEditor editor) {
            var canvas = editor.Site.EditorCanvas;
            if (canvas != null) {
                canvas.CommandExecutor.Redo();
            }
        }

        // --- move ---
        [KeyAction("")]
        public static void MoveLeft(IEditor editor) {
            editor.RequestMove(new Size(-MoveDelta, 0));
        }
        
        [KeyAction("")]
        public static void MoveRight(IEditor editor) {
            editor.RequestMove(new Size(MoveDelta, 0));
        }
        
        [KeyAction("")]
        public static void MoveUp(IEditor editor) {
            editor.RequestMove(new Size(0, -MoveDelta));
        }
        
        [KeyAction("")]
        public static void MoveDown(IEditor editor) {
            editor.RequestMove(new Size(0, MoveDelta));
        }
        
        [KeyAction("")]
        public static void Remove(IEditor editor) {
            var parent = editor.Parent;
            var loc = CaretUtil.GetExpectedCaretPosition(editor.Figure.Location, editor.Site.GridService);
            var caret = editor.Site.Caret;
            editor.RequestRemove();
            caret.Position = loc;
            parent.RequestSelect(SelectKind.True, true);
        }
        
        [KeyAction("")]
        public static void Cut(IEditor editor) {
            var selecteds = editor.Site.SelectionManager.SelectedEditors;
            Copy(selecteds, editor.Site.CommandExecutor);
            foreach (var selected in selecteds.ToArray()) {
                selected.RequestRemove();
            }
        }

        [KeyAction("")]
        public static void Copy(IEditor editor) {
            Copy(editor.Site.SelectionManager.SelectedEditors, editor.Site.CommandExecutor);
        }

        // --- select ---
        [KeyAction("次の要素を選択")]
        public static void SelectNextElement(IEditor editor) {
            var parent = editor.Parent;
            if (parent != null) {
                var found = false;
                var next = default(IEditor);
                foreach (var child in parent.GetChildrenByPosition()) {
                    if (child == editor) {
                        found = true;
                        continue;
                    }
                    if (found) {
                        next = child;
                        break;
                    }
                }
                if (next == null && found) {
                    next = parent.GetChildrenByPosition().First();
                }

                if (next != null) {
                    next.RequestSelect(SelectKind.True, true);
                    editor.Site.EditorCanvas.EnsureVisible(next);
                }
            }
        }

        [KeyAction("前の要素を選択")]
        public static void SelectPreviousElement(IEditor editor) {
            var parent = editor.Parent;
            if (parent != null) {
                var found = false;
                var prev = default(IEditor);
                foreach (var child in parent.GetChildrenByPosition().Reverse()) {
                    if (child == editor) {
                        found = true;
                        continue;
                    }
                    if (found) {
                        prev = child;
                        break;
                    }
                }
                if (prev == null && found) {
                    prev = parent.GetChildrenByPosition().Last();
                }

                if (prev != null) {
                    prev.RequestSelect(SelectKind.True, true);
                    editor.Site.EditorCanvas.EnsureVisible(prev);
                }
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


        // --- move out ---
        [KeyAction("")]
        public static void MoveOutDown(IEditor editor) {
            var bounds = editor.Figure.Bounds;

            editor.Parent.RequestSelect(SelectKind.True, true);
            editor.Site.Caret.Position = CaretUtil.GetExpectedCaretPosition(
                new Point(bounds.Left, bounds.Bottom - 1), editor.Site.GridService
            );
        }

        [KeyAction("")]
        public static void MoveOutUp(IEditor editor) {
            var bounds = editor.Figure.Bounds;

            editor.Parent.RequestSelect(SelectKind.True, true);
            editor.Site.Caret.Position = CaretUtil.GetExpectedCaretPosition(
                new Point(bounds.Left, bounds.Top - 20), editor.Site.GridService
            );
        }

        [KeyAction("")]
        public static void MoveOutLeft(IEditor editor) {
            var bounds = editor.Figure.Bounds;

            editor.Parent.RequestSelect(SelectKind.True, true);
            editor.Site.Caret.Position = CaretUtil.GetExpectedCaretPosition(
                new Point(bounds.Left - 16, bounds.Top), editor.Site.GridService
            );
        }

        [KeyAction("")]
        public static void MoveOutRight(IEditor editor) {
            var bounds = editor.Figure.Bounds;

            editor.Parent.RequestSelect(SelectKind.True, true);
            editor.Site.Caret.Position = CaretUtil.GetExpectedCaretPosition(
                new Point(bounds.Right - 1, bounds.Top), editor.Site.GridService
            );
        }

        // --- misc ---
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

        [KeyAction("")]
        public static void ScrollRecenter(IEditor editor) {
            var canvas = editor.Site.EditorCanvas;
            canvas.ScrollRecenter();
        }

        // --- focus ---
        [KeyAction("")]
        public static void BeginFocus(IEditor editor) {
            editor.RequestFocus(FocusKind.Begin, null);
        }

        // ------------------------------
        // private
        // ------------------------------
        private static void Copy(IEnumerable<IEditor> targets, ICommandExecutor commandExecutor) {
            var req = new CopyRequest(targets);
            var list = new EditorBundle(targets);
            list.PerformGroupRequest(req, commandExecutor);
        }

        private static void FocusMemoText(IEditor editor, Point pt) {
            foreach (var child in editor.Children) {
                if (child.Model is MemoText && child.Figure.ContainsPoint(pt)) {
                    child.RequestFocus(FocusKind.Begin, pt);
                }
            }
        }

    }
}
