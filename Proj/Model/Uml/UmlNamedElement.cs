/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Event;
using System.Collections.ObjectModel;
using Mkamo.Common.Association;
using Mkamo.Container.Core;
using Mkamo.Common.Externalize;

namespace Mkamo.Model.Uml {
    public abstract class UmlNamedElement: UmlElement {
        // ========================================
        // field
        // ========================================
        private string _name;
        private UmlVisibilityKind _visibility;

        private AssociationCollection<UmlDependency> _clientDependencies; /// lazy
        private AssociationCollection<UmlDependency> _supplierDependencies; /// lazy

        // ========================================
        // constructor
        // ========================================
        protected UmlNamedElement() {
            _visibility = UmlVisibilityKind.None;
        }
        
        // ========================================
        // property
        // ========================================
        [Persist, External]
        public virtual string Name {
            get { return _name; }
            set {
                if (_name == value) {
                    return;
                }
                var old = _name;
                _name = value;
                OnPropertySet(this, "Name", old, value);
            }
        }

        [Persist, External]
        public virtual UmlVisibilityKind Visibility {
            get { return _visibility; }
            set {
                if (_visibility == value) {
                    return;
                }
                var old = _visibility;
                _visibility = value;
                OnPropertySet(this, "Visibility", old, value);
            }
        }

        [Persist, External]
        public virtual Collection<UmlDependency> ClientDependencies {
            get {
                if (_clientDependencies == null) {
                    _clientDependencies = new AssociationCollection<UmlDependency>(
                        dep => {
                            if (dep.Client != this) {
                                dep.Client = this;
                            }
                        },
                        dep => dep.Client = null
                    );
                    _clientDependencies.EventSender = this;
                    _clientDependencies.EventPropertyName = "ClientDependencies";
                    _clientDependencies.DetailedPropertyChanged += (sender, e) => OnPropertyChanged(this, e);
                }
                return _clientDependencies;
            }
        }

        [Persist, External]
        public virtual Collection<UmlDependency> SupplierDependencies {
            get {
                if (_supplierDependencies == null) {
                    _supplierDependencies = new AssociationCollection<UmlDependency>(
                        dep => {
                            if (dep.Supplier != this) {
                                dep.Supplier = this;
                            }
                        },
                        dep => dep.Supplier = null
                    );
                    _supplierDependencies.EventSender = this;
                    _supplierDependencies.EventPropertyName = "SupplierDependencies";
                    _supplierDependencies.DetailedPropertyChanged += (sender, e) => OnPropertyChanged(this, e);
                }
                return _supplierDependencies;
            }
        }


        // ========================================
        // method
        // ========================================

    }
}
