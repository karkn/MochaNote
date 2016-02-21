/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Common.Structure {
    [Serializable]
    public class Tree<TValue> {
        // ========================================
        // field
        // ========================================
        private TreeNode<TValue> _root;

        // ========================================
        // constructor
        // ========================================
        public Tree(TreeNode<TValue> root) {
            _root = root;
        }

        public Tree(TValue rootValue): this(new TreeNode<TValue>(rootValue)) {
        }

        public Tree(): this(default(TValue)) {
        }

        // ========================================
        // property
        // ========================================
        public TreeNode<TValue> Root {
            get { return _root; }
        }

        // ========================================
        // method
        // ========================================

    }
}
