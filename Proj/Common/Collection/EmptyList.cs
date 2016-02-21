/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Mkamo.Common.Collection {
    [Serializable]
    public class EmptyList<T>: IList<T> {
        // ========================================
        // static field
        // ========================================
        public static readonly EmptyList<T> Empty = new EmptyList<T>();

        // ========================================
        // property
        // ========================================
        public T this[int index] {
            get { throw new ArgumentOutOfRangeException("index"); }
            set { throw new ArgumentOutOfRangeException("index"); }
        }

        public int Count {
            get { return 0; }
        }

        public bool IsReadOnly {
            get { return true; }
        }

        // ========================================
        // method
        // ========================================
        // === EmptyList ==========
        void ICollection<T>.Add(T item) {
            throw new InvalidOperationException("EmptyList is read only");
        }

        bool ICollection<T>.Remove(T item) {
            throw new InvalidOperationException("EmptyList is read only");
        }

        void ICollection<T>.Clear() {
            throw new InvalidOperationException("EmptyList is read only");
        }

        void IList<T>. Insert(int index, T item) {
            throw new InvalidOperationException("EmptyList is read only");
        }

        void IList<T>.RemoveAt(int index) {
            throw new InvalidOperationException("EmptyList is read only");
        }

        public bool Contains(T item) {
            return false;
        }

        public int IndexOf(T item) {
            return -1;
        }

        public void CopyTo(T[] array, int arrayIndex) {

        }

        public IEnumerator<T> GetEnumerator() {
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
