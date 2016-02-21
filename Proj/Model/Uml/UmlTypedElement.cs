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
    public abstract class UmlTypedElement: UmlNamedElement {
        // ========================================
        // field
        // ========================================
        private UmlType _type;
        private string _typeName;

        // ========================================
        // constructor
        // ========================================
        protected UmlTypedElement() {
            _type = null;
            _typeName = "";
        }

        // ========================================
        // property
        // ========================================
        [Persist, External]
        public virtual UmlType Type {
            get { return _type; }
            set {
                if (_type == value) {
                    return;
                }
                var old = _type;
                _type = value;
                _typeName = _type == null? "": _type.Name;
                OnPropertySet(this, "Type", old, value);
            }
        }

        [Persist, External]
        public virtual string TypeName {
            get { return _type == null? _typeName: _type.Name; }
            set {
                if (_typeName == value || value == null) {
                    return;
                }

                var old = _typeName;
                _typeName = value;
                _type = null;
                OnPropertySet(this, "TypeName", old, value);
            }
        }

        // ========================================
        // method
        // ========================================

    }
}
