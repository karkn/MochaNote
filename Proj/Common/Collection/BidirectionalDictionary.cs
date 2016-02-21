/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Common.Collection {
    public class BidirectionalDictionary<TKey, TValue>: IDictionary<TKey, TValue> {
        // ========================================
        // field
        // ========================================
        private IDictionary<TKey, TValue> _forwardMap = new Dictionary<TKey, TValue>();
        private IDictionary<TValue, TKey> _reverseMap = new Dictionary<TValue, TKey>();

        // ========================================
        // constructor
        // ========================================

        // ========================================
        // property
        // ========================================
        public ICollection<TKey> Keys {
            get { return _forwardMap.Keys; }
        }

        public ICollection<TValue> Values {
            get { return _forwardMap.Values; }
        }

        public TValue this[TKey key] {
            get {
                return _forwardMap[key];
            }
            set {
                _forwardMap[key] = value;
                _reverseMap[value] = key;
            }
        }

        // === BidirectionalDictionary ==========
        public IDictionary<TValue, TKey> Reverse {
            get { return _reverseMap; }
        }

        // ========================================
        // method
        // ========================================
        public void Add(TKey key, TValue value) {
            _forwardMap.Add(key, value);
            _reverseMap.Add(value, key);
        }

        public bool ContainsKey(TKey key) {
            return _forwardMap.ContainsKey(key);
        }

        public bool Remove(TKey key) {
            _reverseMap.Remove(_forwardMap[key]);
            return _forwardMap.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value) {
            return _forwardMap.TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<TKey, TValue> item) {
            _forwardMap.Add(item);
            _reverseMap.Add(new KeyValuePair<TValue, TKey>(item.Value, item.Key));
        }

        public void Clear() {
            _forwardMap.Clear();
            _reverseMap.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) {
            return _forwardMap.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            _forwardMap.CopyTo(array, arrayIndex);
        }

        public int Count {
            get { return _forwardMap.Count; }
        }

        public bool IsReadOnly {
            get { return _forwardMap.IsReadOnly; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) {
            _reverseMap.Remove(item.Value);
            return _forwardMap.Remove(item);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            return _forwardMap.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return _forwardMap.GetEnumerator();
        }

        // === BidirectionalDictionary ==========
        public bool ContainsValue(TValue value){
            return _reverseMap.ContainsKey(value);
        }

        public TKey KeyFor(TValue value) {
            return _reverseMap[value];
        }

        public bool TryGetKey(TValue value, out TKey key) {
            return _reverseMap.TryGetValue(value, out key);
        }
    }
}
