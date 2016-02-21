/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Common.Collection {
    /// <summary>
    /// rootからnextProviderを使って走査する．
    /// </summary>
    public class Iterator<T>: IEnumerable<T> {
        // ========================================
        // field
        // ========================================
        private T _root;
        private Func<T, IEnumerable<T>> _nextsProvider;

        // ========================================
        // constructor
        // ========================================
        public Iterator(T root, Func<T, IEnumerable<T>> nextsProvider) {
            _root = root; 
            _nextsProvider = nextsProvider;
        }

        // ========================================
        // method
        // ========================================
        public IEnumerator<T> GetEnumerator() {
            yield return _root;
            foreach (var elem in Iterate(_nextsProvider(_root))) {
                yield return elem;
            }
        }

        global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        // ------------------------------
        // private
        // ------------------------------
        private IEnumerable<T> Iterate(IEnumerable<T> enumerable) {
            foreach (var elem in enumerable) {
                yield return elem;

                foreach (var child in Iterate(_nextsProvider(elem))) {
                    yield return child;
                }
            }
        }

    }
}
