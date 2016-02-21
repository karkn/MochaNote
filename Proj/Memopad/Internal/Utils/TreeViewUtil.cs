/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Container.Core;
using Mkamo.Model.Memo;
using System.Windows.Forms;
using Mkamo.Model.Core;
using Mkamo.Control.TreeNodeEx;

namespace Mkamo.Memopad.Internal.Utils {
    internal static class TreeViewUtil {
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
        public static void UpdateTagTreeView(TreeView treeView, Workspace workspace) {
            treeView.Nodes.Clear();
            BuildTagTree(treeView.Nodes, workspace, -1, null);
        }

        public static void UpdateTagTreeView(TreeView treeView, Workspace workspace, Predicate<MemoTag> pred) {
            treeView.Nodes.Clear();
            BuildTagTree(treeView.Nodes, workspace, -1, pred);
        }

        public static void UpdateTagTreeView(TreeNode root, Workspace workspace) {
            root.Nodes.Clear();
            BuildTagTree(root.Nodes, workspace, -1, null);
        }

        public static void UpdateTagTreeView(TreeNode root, Workspace workspace, int imageIndex) {
            root.Nodes.Clear();
            BuildTagTree(root.Nodes, workspace, imageIndex, null);
        }

        public static void BuildTagTree(TreeNode root, Workspace workspace, int imageIndex) {
            BuildTagTree(root.Nodes, workspace, imageIndex, null);
        }

        public static void BuildTagTree(TreeNodeCollection nodes, Workspace workspace, int imageIndex, Predicate<MemoTag> pred) {
            var tags = workspace.Tags;

            var targets = default(HashSet<MemoTag>);
            if (pred != null) {
                targets = new HashSet<MemoTag>();
                foreach (var tag in tags) {
                    if (pred(tag)) {
                        targets.Add(tag);
                        for (var ctag = tag; ctag.SuperTag != null; ctag = ctag.SuperTag) {
                            targets.Add(ctag.SuperTag);
                        }
                    }
                }
                tags = targets;
            }

            var addeds = new HashSet<MemoTag>();
            foreach (var tag in tags) {
                if (tag.SuperTag == null) {
                    AddNodeRecursively(tag, imageIndex, nodes, targets, addeds);
                }
            }
        }

        // ------------------------------
        // private
        // ------------------------------
        private static void AddNodeRecursively(
            MemoTag tag, int imageIndex, TreeNodeCollection parentCol, HashSet<MemoTag> targets, HashSet<MemoTag> addeds
        ) {
            if (addeds.Contains(tag)) {
                return;
            }
            if (targets != null && !targets.Contains(tag)) {
                return;
            }

            var node = new TreeNodeEx(tag.Name);
            node.Tag = tag;
            if (imageIndex > -1) {
                node.ImageIndex = imageIndex;
                node.SelectedImageIndex = imageIndex;
            }
            parentCol.Add(node);
            addeds.Add(tag);

            foreach (var child in tag.SubTags) {
                AddNodeRecursively(child, imageIndex, node.Nodes, targets, addeds);
            }
        }

    }
}
