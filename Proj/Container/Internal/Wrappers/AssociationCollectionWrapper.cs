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
    internal class AssociationCollectionWrapper<T>: CollectionWrapper<T> {
        // ========================================
        // constructor
        // ========================================
        public AssociationCollectionWrapper(IEntityContainer container, object owner, Collection<T> real)
            : base(container, owner, real) {

        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        protected override void InsertItem(int index, T item) {
            if (!Contains(item)) {
                base.InsertItem(index, item);
            }
        }

        protected override void SetItem(int index, T item) {
            if (!Contains(item)) {
                base.SetItem(index, item);
            }
        }
    }
}
