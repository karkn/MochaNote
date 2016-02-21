/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Association;
using Mkamo.Container.Core;
using Mkamo.Common.Externalize;

namespace Mkamo.Model.Uml {
    public abstract class UmlType: UmlNamedElement, UmlPackagableElement {
        // ========================================
        // field
        // ========================================
        private UmlPackage _owingPackage;

        // ========================================
        // constructor
        // ========================================
        protected UmlType() {
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        [Persist, External]
        public virtual UmlPackage OwingPackage {
            get { return _owingPackage; }
            set {
                var old = _owingPackage;
                var result = AssociationUtil.EnsureAssociation(
                    _owingPackage,
                    value,
                    pkg => _owingPackage = pkg,
                    pkg => {
                        if (!pkg.PackagedElements.Contains(this)) {
                            pkg.PackagedElements.Add(this);
                        }
                    },
                    pkg => pkg.PackagedElements.Remove(this)
                );
                if (result != AssociationUtil.EnsureResult.None) {
                    OnPropertySet(this, "OwingPackage", old, value);
                }
            }
        }
    }
}
