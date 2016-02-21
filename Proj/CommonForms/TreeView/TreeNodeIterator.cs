/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Mkamo.Common.Forms.TreeView {
    using TreeView = global::System.Windows.Forms.TreeView;
    using Mkamo.Common.Diagnostics;

    /// <summary>
    /// TreeViewに含まれるすべてのTreeNodeを走査するIterator．
    /// GetEnumerator()があるのでforeachで使える．
    /// 例．
    ///     public IEnumerable<MemoTag> GetCheckedTags() {
    ///         var ite = new TreeNodeIterator(_tagTreeView);
    ///         foreach (var node in ite) {
    ///             if (node.Checked) {
    ///                 yield return node.Tag as MemoTag;
    ///             }
    ///         }
    ///     }
    /// </summary>
    public class TreeNodeIterator {
        // ========================================
        // field
        // ========================================
        private TreeView _treeView;
        private TreeNode _rootNode;

        // ========================================
        // constructor
        // ========================================
        public TreeNodeIterator(TreeView treeView) {
            Contract.Requires(treeView != null);

            _treeView = treeView;
        }

        public TreeNodeIterator(TreeNode rootNode) {
            Contract.Requires(rootNode != null);

            _rootNode = rootNode;
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public IEnumerator<TreeNode> GetEnumerator() {
            if (_treeView != null) {
                foreach (var node in GetNodes(_treeView.Nodes)) {
                    Contract.Ensures(node != null);
                    yield return node;
                }

            } else if (_rootNode != null) {
                Contract.Ensures(_rootNode != null);
                yield return _rootNode;

                foreach (var node in GetNodes(_rootNode.Nodes)) {
                    Contract.Ensures(node != null);
                    yield return node;
                }
            }
        }

        // ------------------------------
        // private
        // ------------------------------
        private IEnumerable<TreeNode> GetNodes(TreeNodeCollection nodes) {
            foreach (TreeNode node in nodes) {
                if (node != null) {
                    Contract.Ensures(node != null);
                    yield return node;

                    foreach (var child in GetNodes(node.Nodes)) {
                        Contract.Ensures(child != null);
                        yield return child;
                    }
                }
            }
        }
    }
}
