/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Mkamo.Common.Association;
using Mkamo.Container.Core;
using Mkamo.Common.Externalize;
using Mkamo.Model.Core;

namespace Mkamo.Model.Uml {
    [Externalizable(Type = typeof(UmlPackage), FactoryMethodType = typeof(UmlFactory), FactoryMethod = "CreatePackage")]
    public class UmlPackage: UmlNamedElement, UmlPackagableElement {
        // ========================================
        // field
        // ========================================
        private AssociationCollection<UmlPackagableElement> _packagedElements; /// lazy
        private UmlPackage _owingPackage;

        // ========================================
        // constructor
        // ========================================
        protected internal UmlPackage() {
            
        }

        // ========================================
        // property
        // ========================================
        [Persist(Cascade = true), External]
        public virtual Collection<UmlPackagableElement> PackagedElements {
            get {
                if (_packagedElements == null) {
                    _packagedElements = new AssociationCollection<UmlPackagableElement>(
                        elem => {
                            if (elem.OwingPackage != this) {
                                elem.OwingPackage = this;
                            }
                        },
                        elem => elem.OwingPackage = null
                    );
                    _packagedElements.EventSender = this;
                    _packagedElements.EventPropertyName = "PackagedElements";
                    _packagedElements.DetailedPropertyChanged += (sender, e) => OnPropertyChanged(this, e);
                }
                return _packagedElements;
            }
        }

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

        // ========================================
        // method
        // ========================================

    }
}
