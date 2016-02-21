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
    [Externalizable(Type = typeof(UmlDependency), FactoryMethodType = typeof(UmlFactory), FactoryMethod = "CreateDependency")]
    public class UmlDependency: UmlRelationship {
        // ========================================
        // field
        // ========================================
        private UmlNamedElement _client;
        private UmlNamedElement _supplier;

        // ========================================
        // constructor
        // ========================================
        protected internal UmlDependency() {
        }

        // ========================================
        // property
        // ========================================
        [Persist, External]
        public virtual UmlNamedElement Client {
            get { return _client; }
            set {
                var old = _client;
                var result = AssociationUtil.EnsureAssociation(
                    _client,
                    value,
                    elem => _client = elem,
                    elem => {
                        if (!elem.ClientDependencies.Contains(this)) {
                            elem.ClientDependencies.Add(this);
                        }
                    },
                    elem => elem.ClientDependencies.Remove(this)
                );
                if (result != AssociationUtil.EnsureResult.None) {
                    OnPropertySet(this, "Client", old, value);
                }
            }
        }

        [Persist, External]
        public virtual UmlNamedElement Supplier {
            get { return _supplier; }
            set {
                var old = _supplier;
                var result = AssociationUtil.EnsureAssociation(
                    _supplier,
                    value,
                    elem => _supplier = elem,
                    elem => {
                        if (!elem.SupplierDependencies.Contains(this)) {
                            elem.SupplierDependencies.Add(this);
                        }
                    },
                    elem => elem.SupplierDependencies.Remove(this)
                );
                if (result != AssociationUtil.EnsureResult.None) {
                    OnPropertySet(this, "Supplier", old, value);
                }
            }
        }

        // ========================================
        // method
        // ========================================

    }
}
