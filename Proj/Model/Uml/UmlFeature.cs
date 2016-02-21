/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Container.Core;
using Mkamo.Common.Externalize;

namespace Mkamo.Model.Uml {
    public abstract class UmlFeature: UmlTypedElement {
        // ========================================
        // field
        // ========================================
        private bool _isStatic;

        // ========================================
        // constructor
        // ========================================
        protected UmlFeature() {
            _isStatic = false;
        }

        // ========================================
        // property
        // ========================================
        [Persist, External]
        public virtual bool IsStatic {
            get { return _isStatic; }
            set {
                if (_isStatic == value) {
                    return;
                }
                var old = _isStatic;
                _isStatic = value;
                OnPropertySet(this, "IsStatic", old, value);
            }
        }
    }
}
