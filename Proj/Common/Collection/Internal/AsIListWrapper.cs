/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Mkamo.Common.Collection.Internal {
    internal class AsIListWrapper<T>: IList<T> {
        // ========================================
        // field
        // ========================================
        private IList _origin;

        // ========================================
        // constructor
        // ========================================
        public AsIListWrapper(IList origin) {
            _origin = origin;
        }

        // ========================================
        // property
        // ========================================
        public T this[int index] {
            get { return (T) _origin[index]; }
            set { _origin[index] = value; }
        }

        public int Count {
            get { return _origin.Count; }
        }

        public bool IsReadOnly {
            get { return _origin.IsReadOnly; }
        }


        // ========================================
        // method
        // ========================================
        public int IndexOf(T item) {
            return _origin.IndexOf(item);
        }

        public void Insert(int index, T item) {
            _origin.Insert(index, item);
        }

        public void RemoveAt(int index) {
            _origin.RemoveAt(index);
        }

        public void Add(T item) {
            _origin.Add(item);
        }

        public void Clear() {
            _origin.Clear();
        }

        public bool Contains(T item) {
            return _origin.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex) {
            _origin.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item) {
            var i = _origin.IndexOf(item);
            if (i > -1) {
                _origin.Remove(item);
                return true;
            } else {
                return false;
            }
        }

        public IEnumerator<T> GetEnumerator() {
            return Enumerable.Cast<T>(_origin).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _origin.GetEnumerator();
        }
    }
}
