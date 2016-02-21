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
using Mkamo.Model.Core;
using Mkamo.Common.Externalize;

namespace Mkamo.Model.Uml {
    [Externalizable(Type = typeof(UmlAssociation), FactoryMethodType = typeof(UmlFactory), FactoryMethod = "CreateAssociation")]
    public class UmlAssociation: UmlRelationship {
        // ========================================
        // field
        // ========================================
        private string _name;

        private UmlProperty _sourceMemberEnd;
        private UmlProperty _targetMemberEnd;

        private bool _isSourceNavigable;
        private bool _isTargetNavigable;

        // ========================================
        // constructor
        // ========================================
        protected internal UmlAssociation() {
            _isSourceNavigable = true;
            _isTargetNavigable = true;
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

        [Persist(Cascade = true, Composite = typeof(UmlProperty)), External]
        public virtual UmlProperty SourceMemberEnd {
            get { return _sourceMemberEnd; }
            set {
                var old = _sourceMemberEnd;
                var result = AssociationUtil.EnsureAssociation(
                    _sourceMemberEnd,
                    value,
                    prop => _sourceMemberEnd = prop,
                    prop => {
                        if (prop.OutgoingAssociation != this) {
                            prop.OutgoingAssociation = this;
                        }
                    },
                    prop => prop.OutgoingAssociation = null
                );
                if (result != AssociationUtil.EnsureResult.None) {
                    OnPropertySet(this, "SourceMemberEnd", old, value);
                }
            }
        }

        [Persist(Cascade = true, Composite = typeof(UmlProperty)), External]
        public virtual UmlProperty TargetMemberEnd {
            get { return _targetMemberEnd; }
            set {
                var old = _targetMemberEnd;
                var result = AssociationUtil.EnsureAssociation(
                    _targetMemberEnd,
                    value,
                    prop => _targetMemberEnd = prop,
                    prop => {
                        if (prop.IncomingAssociation != this) {
                            prop.IncomingAssociation = this;
                        }
                    },
                    prop => prop.IncomingAssociation = null
                );
                if (result != AssociationUtil.EnsureResult.None) {
                    OnPropertySet(this, "TargetMemberEnd", old, value);
                }
            }
        }

        [Persist, External]
        public virtual bool IsSourceNavigable {
            get { return _isSourceNavigable; }
            set {
                if (_isSourceNavigable == value) {
                    return;
                }
                var old = _isSourceNavigable;
                _isSourceNavigable = value;
                OnPropertySet(this, "IsSourceNavigable", old, value);
            }
        }

        [Persist, External]
        public virtual bool IsTargetNavigable {
            get { return _isTargetNavigable; }
            set {
                if (_isTargetNavigable == value) {
                    return;
                }
                var old = _isTargetNavigable;
                _isTargetNavigable = value;
                OnPropertySet(this, "IsTargetNavigable", old, value);
            }
        }

        public virtual UmlType SourceEndType {
            get { return _sourceMemberEnd.Type; }
        }

        public virtual UmlType TargetEndType {
            get { return _targetMemberEnd.Type; }
        }

        // ========================================
        // method
        // ========================================

    }

}
