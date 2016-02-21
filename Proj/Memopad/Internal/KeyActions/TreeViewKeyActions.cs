/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Diagnostics;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Memopad.Core;
using System.Windows.Forms;
using Mkamo.Common.Forms.Input;
using Mkamo.Common.Forms.KeyMap;
using System.Reflection;

namespace Mkamo.Memopad.Internal.KeyActions {
    [Obfuscation(Feature = "renaming", Exclude = true, ApplyToMembers = true)]
    internal static class TreeViewKeyActions {
        // ========================================
        // method
        // ========================================
        [KeyAction("")]
        public static void BeginEdit(TreeView treeView) {
            var node = treeView.SelectedNode;
            if (node != null && treeView.LabelEdit) {
                node.BeginEdit();
            }
        }

        [KeyAction("")]
        public static void MoveBeginningOfLine(TreeView treeView) {
            if (treeView.Nodes.Count > 0) {
                treeView.SelectedNode = treeView.Nodes[0];
            }
        }

        [KeyAction("")]
        public static void MoveEndOfLine(TreeView treeView) {
            if (treeView.Nodes.Count > 0) {
                treeView.SelectedNode = treeView.Nodes[treeView.Nodes.Count - 1];
            }
        }

        [KeyAction("")]
        public static void MoveForward(TreeView treeView) {
            var node = treeView.SelectedNode;
            if (node == null) {
                MoveBeginningOfLine(treeView);
            } else {
                if (node.IsExpanded) {
                    if (node.Nodes.Count > 0) {
                        treeView.SelectedNode = node.Nodes[0];
                    }
                } else {
                    node.Expand();
                }
            }
        }

        [KeyAction("")]
        public static void MoveBackward(TreeView treeView) {
            var node = treeView.SelectedNode;
            if (node == null) {
                MoveBeginningOfLine(treeView);
            } else {
                if (node.IsExpanded) {
                    node.Collapse();
                } else {
                    if (node.Parent == null) {
                        MoveBeginningOfLine(treeView);
                    } else {
                        treeView.SelectedNode = node.Parent;
                    }
                }
            }
        }

        [KeyAction("")]
        public static void MoveNextLine(TreeView treeView) {
            var node = treeView.SelectedNode;
            if (node == null) {
                MoveBeginningOfLine(treeView);
            } else {
                if (node.NextVisibleNode != null) {
                    treeView.SelectedNode = node.NextVisibleNode;
                }
            }
        }

        [KeyAction("")]
        public static void MovePreviousLine(TreeView treeView) {
            var node = treeView.SelectedNode;
            if (node == null) {
                MoveBeginningOfLine(treeView);
            } else {
                if (node.PrevVisibleNode != null) {
                    treeView.SelectedNode = node.PrevVisibleNode;
                }
            }
        }

        [KeyAction("")]
        public static void ToggleChecked(TreeView treeView) {
            if (treeView.CheckBoxes) {
                var selected = treeView.SelectedNode;
                if (selected != null) {
                    selected.Checked = !selected.Checked;
                }
            }
        }
    }
}
