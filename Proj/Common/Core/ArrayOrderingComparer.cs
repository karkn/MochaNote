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
    public class ArrayOrderingComparer<T>: IComparer<T> {
        // ========================================
        // field
        // ========================================
        private T[] _array;

        // ========================================
        // constructor
        // ========================================
        public ArrayOrderingComparer(T[] array) {
            Contract.Requires(array != null);

            _array = array;
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public int Compare(T x, T y) {
            if (EqualityComparer<T>.Default.Equals(x, y)) {
                return 0;
            } else {
                var xIndex = Array.IndexOf(_array, x);
                var yIndex = Array.IndexOf(_array, y);
                var xContained = xIndex > -1;
                var yContained = yIndex > -1;

                if (xContained) {
                    if (yContained) {
                        return xIndex - yIndex;
                    } else {
                        return -1;
                    }
                } else {
                    if (yContained) {
                        return 1;
                    } else {
                        return 0;
                    }
                }
            }
        }
    }
}
