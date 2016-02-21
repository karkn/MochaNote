/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Mkamo.Control.TreeNodeEx {
    public class TreeNodeEx: TreeNode {

        // ========================================
        // field
        // ========================================
        private bool _isSubNodesLoaded;

        // ========================================
        // constructor
        // ========================================
        public TreeNodeEx(string text): base(text) {
            _isSubNodesLoaded = false;
        }

        public TreeNodeEx(): base() {
            _isSubNodesLoaded = false;
        }

        // ========================================
        // property
        // ========================================
        public bool IsSubNodesLoaded {
            get { return _isSubNodesLoaded; }
            set { _isSubNodesLoaded = value; }
        }

        // ========================================
        // method
        // ========================================

    }

}
