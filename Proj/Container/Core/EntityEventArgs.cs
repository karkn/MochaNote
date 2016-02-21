/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Container.Core {
    public class EntityEventArgs: EventArgs {
        // ========================================
        // field
        // ========================================
        private object _entity;
        private Type _entityType;

        // ========================================
        // constructor
        // ========================================
        public EntityEventArgs(object entity, Type entityType) {
            _entity = entity;
            _entityType = entityType;
        }

        // ========================================
        // property
        // ========================================
        public object Entity {
            get { return _entity; }
        }

        public Type EntityType {
            get { return _entityType; }
        }

    }
}
