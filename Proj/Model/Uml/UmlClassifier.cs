/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Association;
using System.Collections.ObjectModel;
using Mkamo.Container.Core;
using Mkamo.Common.Externalize;

namespace Mkamo.Model.Uml {
    public abstract class UmlClassifier: UmlType {
        // ========================================
        // field
        // ========================================
        private AssociationCollection<UmlGeneralization> _outgoingGeneralizations; /// lazy
        private AssociationCollection<UmlGeneralization> _incomingGeneralizations; /// lazy

        private UmlPropertyCollection _attributes;
        private UmlOperationCollection _operations;

        private bool _isAbstract;

        // ========================================
        // constructor
        // ========================================
        protected UmlClassifier() {
            _isAbstract = false;

            _attributes = new UmlPropertyCollection(this);
            _operations = new UmlOperationCollection(this);
        }

        // ========================================
        // property
        // ========================================
        [Persist, External]
        public virtual Collection<UmlGeneralization> OutgoingGeneralizations {
            get {
                if (_outgoingGeneralizations == null) {
                    _outgoingGeneralizations = new AssociationCollection<UmlGeneralization>(
                        edge => {
                            if (edge.Specific != this) {
                                edge.Specific = this;
                            }
                        },
                        edge => edge.Specific = null,
                        this,
                        "OutgoingGeneralizations"
                    );
                    _outgoingGeneralizations.DetailedPropertyChanged += (sender, e) => OnPropertyChanged(this, e);
                }
                return _outgoingGeneralizations;
            }
        }

        [Persist, External]
        public virtual Collection<UmlGeneralization> IncomingGeneralizations {
            get {
                if (_incomingGeneralizations == null) {
                    _incomingGeneralizations = new AssociationCollection<UmlGeneralization>(
                        edge => {
                            if (edge.General != this) {
                                edge.General = this;
                            }
                        },
                        edge => edge.General = null,
                        this,
                        "IncomingGeneralizations"
                    );
                    _incomingGeneralizations.DetailedPropertyChanged += (sender, e) => OnPropertyChanged(this, e);
                }
                return _incomingGeneralizations;
            }
        }

        [Persist(Add = "AddAttribute", Cascade = true), External(Add = "AddAttribute")]
        public virtual IEnumerable<UmlProperty> Attributes {
            get { return _attributes; }
        }

        [Persist(Add = "AddOperation", Cascade = true), External(Add = "AddOperation")]
        public virtual IEnumerable<UmlOperation> Operations {
            get { return _operations; }
        }

        [Persist, External]
        public virtual bool IsAbstract {
            get { return _isAbstract; }
            set {
                if (_isAbstract == value) {
                    return;
                }
                var old = _isAbstract;
                _isAbstract = value;
                OnPropertySet(this, "IsAbstract", old, value);
            }
        }

        // ========================================
        // method
        // ========================================
        // --- attribute ---
        [Dirty]
        public virtual void AddAttribute(UmlProperty attr) {
            _attributes.Add(attr);
        }

        [Dirty]
        public virtual void InsertAttribute(int index, UmlProperty attr) {
            _attributes.Insert(index, attr);
        }

        [Dirty]
        public virtual bool RemoveAttribute(UmlProperty attr) {
            return _attributes.Remove(attr);
        }

        [Dirty]
        public virtual void ClearAttributes() {
            _attributes.Clear();
        }

        // --- operation ---
        [Dirty]
        public virtual void AddOperation(UmlOperation ope) {
            _operations.Add(ope);
        }

        [Dirty]
        public virtual void InsertOperation(int index, UmlOperation ope) {
            _operations.Insert(index, ope);
        }

        [Dirty]
        public virtual bool RemoveOperation(UmlOperation ope) {
            return _operations.Remove(ope);
        }

        [Dirty]
        public virtual void ClearOperations() {
            _operations.Clear();
        }

    }
}
