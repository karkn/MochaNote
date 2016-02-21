/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Mkamo.Container.Core;
using Mkamo.Container.Internal.Core;

namespace Mkamo.Container.Internal.Wrappers {
    internal class CollectionWrapper<T>: Collection<T> {
        // ========================================
        // field
        // ========================================
        private IEntityContainer _container;
        private object _owner;
        private IEntity _entity;
        private Collection<T> _real;

        private bool _inited;

        // ========================================
        // constructor
        // ========================================
        public CollectionWrapper(IEntityContainer container, object owner, Collection<T> real) {
            _container = container;
            _owner = owner;
            _entity = container.AsEntity(owner);
            _real = real;

            _inited = false;
            foreach (var item in _real) {
                Add(item);
            }
            _inited = true;
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        protected override void InsertItem(int index, T item) {
            if (_container.IsReadonly) {
                throw new InvalidOperationException("readonly");
            }
            base.InsertItem(index, item);
            if (_inited) {
                _real.Insert(index, item);
                _entity.Dirty();
            }
        }

        protected override void RemoveItem(int index) {
            if (_container.IsReadonly) {
                throw new InvalidOperationException("readonly");
            }
            base.RemoveItem(index);
            _real.RemoveAt(index);
            _entity.Dirty();
        }

        protected override void ClearItems() {
            if (_container.IsReadonly) {
                throw new InvalidOperationException("readonly");
            }
            base.ClearItems();
            _real.Clear();
            _entity.Dirty();
        }

        protected override void SetItem(int index, T item) {
            if (_container.IsReadonly) {
                throw new InvalidOperationException("readonly");
            }
            base.SetItem(index, item);
            _real[index] = item;
            _entity.Dirty();
        }
    }
}
