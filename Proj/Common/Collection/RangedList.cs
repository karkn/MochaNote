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
    /// <summary>
    /// originの一部だけを指すリスト．
    /// </summary>
    [Serializable]
    public class RangedList<T>: IList<T> {
        // ========================================
        // field
        // ========================================
        private IList<T> _origin;
        private int _offset;
        private int _length;

        // ========================================
        // constructor
        // ========================================
        public RangedList(IList<T> origin, int offset, int length) {
            if (origin == null) {
                throw new ArgumentNullException("origin");
            }
            _origin = origin;
            SetRange(offset, length);
        }

        public RangedList(IList<T> origin): this(origin, 0, origin.Count) {
        }

        // ========================================
        // property
        // ========================================
        // === IList ==========
        public T this[int index] {
            get {
                if (index < 0 || index > _length - 1) {
                    throw new ArgumentOutOfRangeException("index");
                }
                return _origin[_offset + index];
            }
            set {
                if (index < 0 || index > _length - 1) {
                    throw new ArgumentOutOfRangeException("index");
                }
                _origin[_offset + index] = value;
            }
        }

        public int Count {
            get { return Length; }
            set { Length = value; }
        }

        public bool IsReadOnly {
            get { return false; }
        }

        // === RangedList ==========
        public int Offset {
            get { return _offset; }
            set { SetRange(value, _length); }
        }

        public int Length {
            get { return _length; }
            set { SetRange(_offset, value); }
        }

        // ========================================
        // method
        // ========================================
        // === IList ==========
        public void Add(T item) {
            _origin.Insert(_offset + _length, item);
            ++_length;
        }

        public void Insert(int index, T item) {
            if (index < 0 || index > _offset + _length) {
                throw new ArgumentOutOfRangeException("index");
            }
            _origin.Insert(_offset + index, item);
            ++_length;
        }

        public bool Remove(T item) {
            int index = IndexOf(item);
            if (index > -1) {
                RemoveAt(index);
                return true;
            } else {
                return false;
            }
        }

        public void RemoveAt(int index) {
            if (index < 0 || index > _offset + _length) {
                throw new ArgumentOutOfRangeException("index");
            }
            _origin.RemoveAt(_offset + index);
            --_length;
        }

        public void Clear() {
            for (int i = 0; i < _length; ++i) {
                _origin.RemoveAt(_offset);
            }
            _length = 0;
        }

        public int IndexOf(T item) {
            for (int i = 0; i < _length; ++i) {
                if (item.Equals(_origin[_offset + i])) {
                    return i;
                }
            }
            return -1;
        }

        public bool Contains(T item) {
            return IndexOf(item) > -1;
        }

        public void CopyTo(T[] array, int arrayIndex) {
            if (array == null) {
                throw new ArgumentNullException("array");
            }
            if (arrayIndex < 0 || arrayIndex + _length < array.Length) {
                throw new ArgumentOutOfRangeException("arrayIndex");
            }

            for (int i = 0; i < _length; ++i) {
                array[arrayIndex + i] = _origin[_offset + i];
            }
        }

        public IEnumerator<T> GetEnumerator() {
            for (int i = 0; i < _length; ++i) {
                yield return _origin[_offset + i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        // === RangedList ==========
        public void SetRange(int offset, int length) {
            if (offset + length > _origin.Count) {
                throw new ArgumentOutOfRangeException("offset, length");
            }
            _offset = offset;
            _length = length;
        }
    }
}
