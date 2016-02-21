/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Externalize;

namespace Mkamo.Container.Core {
    [Entity, Serializable, Externalizable]
    public class EntityCollection<T> {
        // ========================================
        // field
        // ========================================
        private List<T> _items;

        // ========================================
        // constructor
        // ========================================
        protected EntityCollection() {
            _items = new List<T>();
        }

        // ========================================
        // property
        // ========================================
        [Persist(Cascade = true, Add = "Add"), External(Add = "Add")]
        public virtual IEnumerable<T> Items {
            get { return _items; }
        }

        public virtual int Count {
            get { return _items.Count; }
        }

        // ========================================
        // method
        // ========================================
        [Dirty]
        public virtual void Add(T item) {
            _items.Add(item);
        }

        [Dirty]
        public virtual void Insert(int index, T item) {
            _items.Insert(index, item);
        }

        [Dirty]
        public virtual bool Remove(T item) {
            return _items.Remove(item);
        }

        [Dirty]
        public virtual void Clear() {
            _items.Clear();
        }

    }
}
