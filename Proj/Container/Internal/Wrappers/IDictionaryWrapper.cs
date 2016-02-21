/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Mkamo.Container.Core;
using Mkamo.Container.Internal.Core;

namespace Mkamo.Container.Internal.Wrappers {
    internal class IDictionaryWrapper<TKey, TValue>: IDictionary<TKey, TValue> {
        // ========================================
        // field
        // ========================================
        private IEntityContainer _container;
        private object _owner;
        private IEntity _entity;
        private IDictionary<TKey, TValue> _real;

        // ========================================
        // constructor
        // ========================================
        public IDictionaryWrapper(IEntityContainer container, object owner, IDictionary<TKey, TValue> real) {
            _container = container;
            _owner = owner;
            _entity = container.AsEntity(owner);
            _real = real;
        }

        // ========================================
        // property
        // ========================================
        public ICollection<TKey> Keys {
            get { return _real.Keys; }
        }

        public ICollection<TValue> Values {
            get { return _real.Values; }
        }

        public TValue this[TKey key] {
            get { return _real[key]; }
            set {
                if (_container.IsReadonly) {
                    throw new InvalidOperationException("readonly");
                }
                _real[key] = value;
                _entity.Dirty();
            }
        }

        public int Count {
            get { return _real.Count; }
        }

        public bool IsReadOnly {
            get { return _real.IsReadOnly; }
        }


        // ========================================
        // method
        // ========================================
        public void Add(TKey key, TValue value) {
            if (_container.IsReadonly) {
                throw new InvalidOperationException("readonly");
            }
            _real.Add(key, value);
            _entity.Dirty();
        }

        public bool ContainsKey(TKey key) {
            return _real.ContainsKey(key);
        }

        public bool Remove(TKey key) {
            if (_container.IsReadonly) {
                throw new InvalidOperationException("readonly");
            }
            var ret = _real.Remove(key);
            if (ret) {
                _entity.Dirty();
            }
            return ret;
        }

        public bool TryGetValue(TKey key, out TValue value) {
            return _real.TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<TKey, TValue> item) {
            if (_container.IsReadonly) {
                throw new InvalidOperationException("readonly");
            }
            _real.Add(item);
            _entity.Dirty();
        }

        public void Clear() {
            if (_container.IsReadonly) {
                throw new InvalidOperationException("readonly");
            }
            _real.Clear();
            _entity.Dirty();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) {
            return _real.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            _real.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) {
            if (_container.IsReadonly) {
                throw new InvalidOperationException("readonly");
            }
            var ret = _real.Remove(item);
            if (ret) {
                _entity.Dirty();
            }
            return ret;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            return _real.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return _real.GetEnumerator();
        }
    }
}
