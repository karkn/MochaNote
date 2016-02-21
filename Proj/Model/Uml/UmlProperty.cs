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
using Mkamo.Common.Core;
using Mkamo.Container.Core;
using Mkamo.Common.Externalize;
using Mkamo.Model.Core;

namespace Mkamo.Model.Uml {
    [Externalizable(Type = typeof(UmlProperty), FactoryMethodType = typeof(UmlFactory), FactoryMethod = "CreateProperty")]
    public class UmlProperty: UmlFeature, UmlMultiplicityElement {
        // ========================================
        // field
        // ========================================
        private bool _isOrdered;
        private bool _isUnique;
        private bool _isUpperUnlimited;
        private int _upper;
        private int _lower;

        private string _default;
        private bool _isReadOnly;
        private bool _isDerived;

        private UmlAggregationKind _aggregation;

        private UmlAssociation _owingAssociation;

        // ========================================
        // constructor
        // ========================================
        protected internal UmlProperty() {
            Visibility = UmlVisibilityKind.Private;

            _isOrdered = false;
            _isUnique = false;
            _isUpperUnlimited = false;
            _upper = 1;
            _lower = 1;

            _default = null;
            _isReadOnly = false;
            _isDerived = false;

            _aggregation = UmlAggregationKind.None;
        }

        // ========================================
        // property
        // ========================================
        // === UmlMultiplicityElement ==========
        [Persist, External]
        public virtual bool IsOrdered {
            get { return _isOrdered; }
            set {
                if (_isOrdered == value) {
                    return;
                }
                var old = _isOrdered;
                _isOrdered = value;
                OnPropertySet(this, "IsOrdered", old, value);
            }
        }

        [Persist, External]
        public virtual bool IsUnique {
            get { return _isUnique; }
            set {
                if (_isUnique == value) {
                    return;
                }
                var old = _isUnique;
                _isUnique = value;
                OnPropertySet(this, "IsUnique", old, value);
            }
        }

        [Persist, External]
        public virtual bool IsUpperUnlimited {
            get { return _isUpperUnlimited; }
            set {
                if (_isUpperUnlimited == value) {
                    return;
                }
                var old = _isUpperUnlimited;
                _isUpperUnlimited = value;
                OnPropertySet(this, "IsUpperUnlimited", old, value);
            }
        }


        [Persist, External]
        public virtual int Upper {
            get { return _upper; }
            set {
                if (_upper == value) {
                    return;
                }
                var old = _upper;
                _upper = value;
                OnPropertySet(this, "Upper", old, value);
            }
        }

        [Persist, External]
        public virtual int Lower {
            get { return _lower; }
            set {
                if (_lower == value) {
                    return;
                }
                var old = _lower;
                _lower = value;
                OnPropertySet(this, "Lower", old, value);
            }
        }

        // === UmlProperty ==========
        [Persist, External]
        public virtual UmlAssociation OwingAssociation {
            get { return _owingAssociation; }
            set {
                if (value == _owingAssociation) {
                    return;
                }
                var old = _owingAssociation;
                _owingAssociation = value;
                OnPropertySet(this, "OwingAssociation", old, value);
            }
        }

        public virtual UmlAssociation OutgoingAssociation {
            get { return _owingAssociation; }
            set {
                var old = _owingAssociation;
                var result = AssociationUtil.EnsureAssociation(
                    _owingAssociation,
                    value,
                    assoc => _owingAssociation = assoc,
                    assoc => {
                        if (assoc.SourceMemberEnd != this) {
                            assoc.SourceMemberEnd = this;
                        }
                    },
                    assoc => assoc.SourceMemberEnd = null
                );
                if (result != AssociationUtil.EnsureResult.None) {
                    OnPropertySet(this, "OwingAssociation", old, value);
                }
            }
        }

        public virtual UmlAssociation IncomingAssociation {
            get { return _owingAssociation; }
            set {
                var old = _owingAssociation;
                var result = AssociationUtil.EnsureAssociation(
                    _owingAssociation,
                    value,
                    assoc => _owingAssociation = assoc,
                    assoc => {
                        if (assoc.TargetMemberEnd != this) {
                            assoc.TargetMemberEnd = this;
                        }
                    },
                    assoc => assoc.TargetMemberEnd = null
                );
                if (result != AssociationUtil.EnsureResult.None) {
                    OnPropertySet(this, "OwingAssociation", old, value);
                }
            }
        }


        [Persist, External]
        public virtual string Default {
            get { return _default; }
            set {
                if (_default == value) {
                    return;
                }
                var old = _default;
                _default = value;
                OnPropertySet(this, "Default", old, value);
            }
        }

        [Persist, External]
        public virtual bool IsReadOnly {
            get { return _isReadOnly; }
            set {
                if (_isReadOnly == value) {
                    return;
                }
                var old = _isReadOnly;
                _isReadOnly = value;
                OnPropertySet(this, "IsReadOnly", old, value);
            }
        }

        [Persist, External]
        public virtual bool IsDerived {
            get { return _isDerived; }
            set {
                if (_isDerived == value) {
                    return;
                }
                var old = _isDerived;
                _isDerived = value;
                OnPropertySet(this, "IsDerived", old, value);
            }
        }

        [Persist, External]
        public virtual UmlAggregationKind Aggregation {
            get { return _aggregation; }
            set {
                if (_aggregation== value) {
                    return;
                }
                var old = _aggregation;
                _aggregation = value;
                OnPropertySet(this, "Aggregation", old, value);
            }
        }

        public virtual bool IsComposite {
            get { return _aggregation == UmlAggregationKind.Composite; }
        }


        // ========================================
        // method
        // ========================================

        // ========================================
        // class
        // ========================================
        public static class Property {
            public static readonly string OutgoingAssociations = "OutgoingAssociations";
            public static readonly string IncomingAssociations = "IncomingAssociations";
        }

    }
}
