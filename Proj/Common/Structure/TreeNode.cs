/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Mkamo.Common.Structure {
    [Serializable]
    public class TreeNode<TValue>: IStructured<TreeNode<TValue>> {
        // ========================================
        // field
        // ========================================
        private StructuredSupport<TreeNode<TValue>> _support;
        private TValue _value;

        // ========================================
        // constructor
        // ========================================
        public TreeNode(TValue value) {
            _support = new StructuredSupport<TreeNode<TValue>>(this);
            _value = value;
        }

        public TreeNode(): this(default(TValue)) {
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public TreeNode<TValue> Parent {
            get { return _support.Parent; }
            set { _support.Parent = value; }
        }

        public Collection<TreeNode<TValue>> Children {
            get { return _support.Children; }
        }

        public TValue Value {
            get { return _value; }
            set {
                if (EqualityComparer<TValue>.Default.Equals(value, _value)) {
                    return;
                }
                _value = value;
            }
        }
    }
}
