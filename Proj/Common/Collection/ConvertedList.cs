/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Mkamo.Common.Collection {
    public class ConvertedList<TOrigin, TConverted>: IList<TConverted> {
        // ========================================
        // field
        // ========================================
        private IList<TOrigin> _origin;
        private Converter<TOrigin, TConverted> _originToConverted;
        private Converter<TConverted, TOrigin> _convertedToOrigin;

        // ========================================
        // constructor
        // ========================================
        public ConvertedList(
            IList<TOrigin> origin,
            Converter<TOrigin, TConverted> originToConverted,
            Converter<TConverted, TOrigin> convertedToOrigin
        ) {
            if (origin == null || originToConverted == null) {
                throw new ArgumentNullException();
            }
            _origin = origin;
            _originToConverted = originToConverted;
            _convertedToOrigin = convertedToOrigin;
        }

        public ConvertedList(
            IList<TOrigin> origin,
            Converter<TOrigin, TConverted> originToConverted
        )
            : this(origin, originToConverted, null)
        {
        }

        // ========================================
        // property
        // ========================================
        public virtual TConverted this[int index] {
            get { return _originToConverted(_origin[index]); }
            set {
                if (_convertedToOrigin == null) {
                    throw new NotSupportedException("Converted to origin Converter is not set");
                }
                _Origin[index] = _convertedToOrigin(value);
            }
        }

        public int Count {
            get { return _origin.Count; }
        }

        public virtual bool IsReadOnly {
            get { return _convertedToOrigin != null; }
        }

        // --- protected ---
        protected IList<TOrigin> _Origin {
            get { return _origin; }
        }

        protected Converter<TOrigin, TConverted> _Converter {
            get { return _originToConverted; }
        }

        // ========================================
        // method
        // ========================================
        public virtual void Add(TConverted item) {
            if (_convertedToOrigin == null) {
                throw new NotSupportedException("Converted to origin Converter is not set");
            }
            _Origin.Add(_convertedToOrigin(item));
        }

        public virtual void Insert(int index, TConverted item) {
            if (_convertedToOrigin == null) {
                throw new NotSupportedException("Converted to origin Converter is not set");
            }
            _Origin.Insert(index, _convertedToOrigin(item));
        }

        public virtual bool Remove(TConverted item) {
            if (_convertedToOrigin == null) {
                throw new NotSupportedException("Converted to origin Converter is not set");
            }
            return _Origin.Remove(_convertedToOrigin(item));
        }

        public virtual void RemoveAt(int index) {
            if (_convertedToOrigin == null) {
                throw new NotSupportedException("Converted to origin Converter is not set");
            }
            _Origin.RemoveAt(index);
        }

        public virtual void Clear() {
            _Origin.Clear();
        }

        public bool Contains(TConverted item) {
            return IndexOf(item) > -1;
        }

        public int IndexOf(TConverted item) {
            for (int i = 0; i < _origin.Count - 1; ++i) {
                TConverted converted = _originToConverted(_origin[i]);
                if (EqualityComparer<TConverted>.Default.Equals(converted, item)) {
                    return i;
                }
            }
            return -1;
        }

        public void CopyTo(TConverted[] array, int arrayIndex) {
            if (array == null) {
                throw new ArgumentNullException("array is null");
            }
            if (arrayIndex < 0) {
                throw new ArgumentOutOfRangeException("arrayIndex is less than 0");
            }
            if (arrayIndex + _origin.Count > array.Length) {
                throw new ArgumentOutOfRangeException("array length is not enough to be copied");
            }

            for (int i = 0; i < _origin.Count - 1; ++i) {
                array[i + arrayIndex] = _originToConverted(_origin[i]);
            }
        }

        public IEnumerator<TConverted> GetEnumerator() {
            foreach (TOrigin elem in _origin) {
                yield return _originToConverted(elem);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
