/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Mkamo.Container.Internal.Core;
using Mkamo.Container.Core;
using System.Reflection;
using Mkamo.Common.Event;
using Mkamo.Common.Reflection;

namespace Mkamo.Container.Internal.Wrappers {
    internal class IListWrapper<T>: IList<T> {
        // ========================================
        // field
        // ========================================
        private IEntityContainer _container;
        private object _owner;
        private IEntity _entity;
        private IList<T> _real;

        // ========================================
        // constructor
        // ========================================
        public IListWrapper(IEntityContainer container, object owner, IList<T> real) {
            _container = container;
            _owner = owner;
            _entity = container.AsEntity(owner);
            _real = real;
        }


        // ========================================
        // property
        // ========================================
        public T this[int index] {
            get { return _real[index]; }
            set {
                if (_container.IsReadonly) {
                    throw new InvalidOperationException("readonly");
                }
                _real[index] = value;
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
        public void Add(T item) {
            if (_container.IsReadonly) {
                throw new InvalidOperationException("readonly");
            }
            if (item == null) {
                throw new ArgumentNullException("item");
            }

            _real.Add(item);
            _entity.Dirty();
        }

        public void Insert(int index, T item) {
            if (_container.IsReadonly) {
                throw new InvalidOperationException("readonly");
            }
            if (item == null) {
                throw new ArgumentNullException("item");
            }

            _real.Insert(index, item);
            _entity.Dirty();
        }

        public bool Remove(T item) {
            if (_container.IsReadonly) {
                throw new InvalidOperationException("readonly");
            }
            if (item == null) {
                throw new ArgumentNullException("target");
            }

            var ret = _real.Remove(item);
            if (ret) {
                _entity.Dirty();
            }
            return ret;
        }

        public void RemoveAt(int index) {
            if (_container.IsReadonly) {
                throw new InvalidOperationException("readonly");
            }
            _real.RemoveAt(index);
            _entity.Dirty();
        }

        public void Clear() {
            if (_container.IsReadonly) {
                throw new InvalidOperationException("readonly");
            }
            _real.Clear();
            _entity.Dirty();
        }


        public bool Contains(T item) {
            return _real.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex) {
            _real.CopyTo(array, arrayIndex);
        }

        public int IndexOf(T item) {
            return _real.IndexOf(item);
        }

        public IEnumerator<T> GetEnumerator() {
            return _real.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
