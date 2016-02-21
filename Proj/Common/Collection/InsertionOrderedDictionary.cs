/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections;

namespace Mkamo.Common.Collection {
    /// <summary>
    /// Addされた順序でKeyを走査可能なDictionary．
    /// </summary>
    public class InsertionOrderedDictionary<TKey, TValue>:
        IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>> {
        // ========================================
        // field
        // ========================================
        private List<TKey> _keys;
        private Dictionary<TKey, TValue> _keyToValue;

        // ========================================
        // constructor
        // ========================================
        public InsertionOrderedDictionary() {
        }

        // ========================================
        // property
        // ========================================
        // --- IDictionary ---
        public ICollection<TKey> Keys {
            get { return _Keys; }
        }

        public ICollection<TValue> Values {
            get { return _KeyToValue.Values; }
        }

        public TValue this[TKey key] {
            get { return _KeyToValue[key]; }
            set {
                if (_KeyToValue.ContainsKey(key)) {
                    Remove(key);
                }
                Add(key, value);
            }
        }

        // --- ICollection ---
        int ICollection<KeyValuePair<TKey, TValue>>.Count {
            get { return _KeyToValue.Count; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly {
            get { return _KeyToValueAsIDictionary.IsReadOnly; }
        }

        // --- ListOrderedDictionary ---
        public IList<TKey> InsertionOrderedKeys {
            get { return _Keys; }
        }

        public int Count {
            get { return _keyToValue.Count; }
        }

        public TValue this[int index] {
            get { return this[_Keys[index]]; }
        }

        // --- private ---
        private Dictionary<TKey, TValue> _KeyToValue {
            get {
                if (_keyToValue == null) {
                    _keyToValue = new Dictionary<TKey,TValue>();
                }
                return _keyToValue;
            }
        }
        private List<TKey> _Keys {
            get {
                if (_keys == null) {
                    _keys = new List<TKey>();
                }
                return _keys;
            }
        }
        private IDictionary<TKey, TValue> _KeyToValueAsIDictionary {
            get { return _KeyToValue as IDictionary<TKey, TValue>; }
        }

        // ========================================
        // method
        // ========================================
        // --- IDictionary ---
        public void Add(TKey key, TValue value) {
            if (_KeyToValue.ContainsKey(key)) {
                throw new ArgumentException(key + " is already contained");
            }
            _Keys.Add(key);
            _KeyToValue.Add(key, value);
        }

        public bool ContainsKey(TKey key) {
            return _KeyToValue.ContainsKey(key);
        }

        public bool Remove(TKey key) {
            if (_KeyToValue.ContainsKey(key)) {
                _Keys.Remove(key);
                _KeyToValue.Remove(key);
                return true;
            } else {
                return false;
            }
        }

        public bool TryGetValue(TKey key, out TValue value) {
            return _KeyToValue.TryGetValue(key, out value);
        }


        // --- ICollection ---
        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) {
            Add(item.Key, item.Value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear() {
            _Keys.Clear();
            _KeyToValue.Clear();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) {
            return _KeyToValueAsIDictionary.Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            _KeyToValueAsIDictionary.CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) {
            return Remove(item.Key);
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() {
            return _KeyToValueAsIDictionary.GetEnumerator();
        }

        // --- IEnumerable ---
        IEnumerator IEnumerable.GetEnumerator() {
            return _KeyToValueAsIDictionary.GetEnumerator();
        }
    }
}
