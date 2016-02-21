/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mkamo.Memopad.Internal.Controls;
using Mkamo.Editor.Core;
using System.Drawing;
using ComponentFactory.Krypton.Toolkit;
using Mkamo.Memopad.Internal.Controllers;
using Mkamo.Editor.Tools;
using Mkamo.Memopad.Core;
using Mkamo.Figure.Core;
using System.Text.RegularExpressions;
using Mkamo.Editor.Focuses;
using Mkamo.Editor.Requests;

namespace Mkamo.Memopad.Internal.Core {
    internal class MemopadUILogic {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // ========================================
        // field
        // ========================================
        MemopadApplication _facade;

        // ========================================
        // constructor
        // ========================================
        public MemopadUILogic() {
            _facade = MemopadApplication.Instance;
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public void InitFontNameToolStripComboBox(ComboBox comboBox) {
            comboBox.BeginUpdate();
            comboBox.Items.Add("");

            comboBox.Items.AddRange(RegularFontNames.Instance.FontNames);

            comboBox.EndUpdate();
            comboBox.SelectedIndex = 0;
        }

        public void InitFontSizeToolStripComboBox(ComboBox comboBox) {
            comboBox.BeginUpdate();
            comboBox.Items.AddRange(
                new[] {
                    "",
                    "8",
                    "9",
                    "10",
                    "11",
                    "12",
                    "14",
                    "16",
                    "18",
                    "20",
                    "24",
                    "28",
                    "32",
                    "36",
                    "40",
                    "44",
                    "48",
                }
            );
            comboBox.EndUpdate();
            comboBox.SelectedIndex = 0;
        }

        // --- tool strip ---
        public void DoUndo(EditorCanvas editorCanvas) {
            if (editorCanvas == null) {
                return;
            }

            if (editorCanvas.FocusManager.IsEditorFocused) {
                var focus = editorCanvas.FocusManager.Focus as StyledTextFocus;
                if (focus != null) {
                    focus.Undo();
                }
                
            } else if (editorCanvas.CommandExecutor.CanUndo) {
                using (editorCanvas.DirtManager.BeginDirty()) {
                    editorCanvas.CommandExecutor.Undo();
                }
            }
        }

        public void DoRedo(EditorCanvas editorCanvas) {
            if (editorCanvas == null) {
                return;
            }

            if (editorCanvas.FocusManager.IsEditorFocused) {
                var focus = editorCanvas.FocusManager.Focus as StyledTextFocus;
                if (focus != null && focus.CommandExecutor.CanRedo) {
                    focus.Redo();
                }
                
            } else if (editorCanvas.CommandExecutor.CanRedo) {
                using (editorCanvas.DirtManager.BeginDirty()) {
                    editorCanvas.CommandExecutor.Redo();
                }
            }
        }

        public void DoCut(EditorCanvas editorCanvas) {
            if (editorCanvas == null) {
                return;
            }

            if (editorCanvas.FocusManager.IsEditorFocused) {
                var focus = editorCanvas.FocusManager.Focus as StyledTextFocus;
                if (focus != null) {
                    focus.Cut();
                }
                

            } else if (editorCanvas.CanCut()) {
                var targets = editorCanvas.SelectionManager.SelectedEditors;
                var req = new CopyRequest(targets);
                var bundle = new EditorBundle(targets);
                var copy = bundle.GetGroupCommand(req);
                if (copy == null) {
                    return;
                }
                var remove = bundle.GetCompositeCommand(new RemoveRequest());
                if (remove == null) {
                    return;
                }
                var cmd = copy.Chain(remove);
                editorCanvas.CommandExecutor.Execute(cmd);
            }
        }

        public void DoCopy(EditorCanvas editorCanvas) {
            if (editorCanvas == null) {
                return;
            }

            if (editorCanvas.FocusManager.IsEditorFocused) {
                var focus = editorCanvas.FocusManager.Focus as StyledTextFocus;
                if (focus != null) {
                    focus.Copy();
                }
                

            } else if (editorCanvas.CanCopy()) {
                var targets = editorCanvas.SelectionManager.SelectedEditors;
                var bundle = new EditorBundle(targets);
                var copy = new CopyRequest(targets);
                bundle.PerformGroupRequest(copy, editorCanvas.CommandExecutor);
            }
        }

        public void DoPaste(EditorCanvas editorCanvas) {
            if (editorCanvas == null) {
                return;
            }

            if (editorCanvas.FocusManager.IsEditorFocused) {
                var focus = editorCanvas.FocusManager.Focus as StyledTextFocus;
                if (focus != null) {
                    focus.PasteInlinesOrText(false);
                }
                

            } else if (editorCanvas.CanPaste()) {
                var selecteds = editorCanvas.SelectionManager.SelectedEditors;
                var bundle = new EditorBundle(selecteds);
                var req = new PasteRequest();
                req.Location = editorCanvas.Caret.Position;
                bundle.PerformCompositeRequest(req, editorCanvas.CommandExecutor);
            }
        }

        public void DoSelectAll(EditorCanvas editorCanvas) {
            if (editorCanvas == null) {
                return;
            }

            if (editorCanvas.FocusManager.IsEditorFocused) {
                var focus = editorCanvas.FocusManager.Focus as StyledTextFocus;
                if (focus != null) {
                    focus.SelectAll();
                }
                
            } else {
                var editor = editorCanvas.RootEditor.Content;
                foreach (var child in editor.Children) {
                    child.RequestSelect(SelectKind.True, false);
                }
            }
        }

        public void DoDelete(EditorCanvas editorCanvas) {
            if (editorCanvas == null) {
                return;
            }

            if (editorCanvas.FocusManager.IsEditorFocused) {
                var focus = editorCanvas.FocusManager.Focus as StyledTextFocus;
                if (focus != null) {
                    focus.RemoveForward();
                }
                

            } else if (editorCanvas.CanDelete()) {
                var selecteds = editorCanvas.SelectionManager.SelectedEditors;
                var bundle = new EditorBundle(selecteds);
                var req = new RemoveRequest();
                bundle.PerformCompositeRequest(req, editorCanvas.CommandExecutor);
            }
        }

        public void DoSearchInMemo(EditorCanvas editorCanvas, PageContent content) {
            if (editorCanvas == null) {
                return;
            }

            if (content != null) {
                if (content.IsInMemoSearcherShown) {
                    content.FinishSearch(false);
                } else {
                    content.StartSearch(true);
                }
            }
        }

    }
}
