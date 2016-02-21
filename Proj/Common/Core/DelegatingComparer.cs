/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Diagnostics;

namespace Mkamo.Common.Core {
    public class DelegatingComparer<T>: IComparer<T> {
        // ========================================
        // field
        // ========================================
        private Func<T, T, int> _comparer;

        // ========================================
        // constructor
        // ========================================
        public DelegatingComparer(Func<T, T, int> comparer) {
            Contract.Requires(comparer != null);
            _comparer = comparer;
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public int Compare(T x, T y) {
            return _comparer(x, y);
        }
    }
}
