/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Mkamo.Container.Core {
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
        Inherited = true)]
    public class EntityAttribute: Attribute {
        // ========================================
        // field
        // ========================================
        private bool _persist;
        private string _load;
        private string _save;

        private string _onLoading;
        private string _onLoaded;

        // ========================================
        // constructor
        // ========================================
        public EntityAttribute() {
            _persist = true;
        }

        // ========================================
        // property
        // ========================================
        public bool Persist {
            get { return _persist; }
            set { _persist = value; }
        }
        public string Load {
            get { return _load; }
            set { _load = value; }
        }
        public string Save {
            get { return _save; }
            set { _save = value; }
        }

        public string OnLoading {
            get { return _onLoading; }
            set { _onLoading = value; }
        }
        public string OnLoaded {
            get { return _onLoaded; }
            set { _onLoaded = value; }
        }
    }
}
