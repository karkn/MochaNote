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
    [Externalizable(Type = typeof(UmlParameter), FactoryMethodType = typeof(UmlFactory), FactoryMethod = "CreateParameter")]
    public class UmlParameter: UmlTypedElement, UmlMultiplicityElement {
        // ========================================
        // field
        // ========================================
        private bool _isOrdered;
        private bool _isUnique;
        private bool _isUpperUnlimited;
        private int _upper;
        private int _lower;

        //private UmlOperation _operation;

        private string _default;
        private UmlParameterDirectionKind _direction;

        // ========================================
        // constructor
        // ========================================
        protected internal UmlParameter() {
            Visibility = UmlVisibilityKind.Private;

            _isOrdered = false;
            _isUnique = false;
            _isUpperUnlimited = false;
            _upper = 1;
            _lower = 1;

            _default = null;
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
                OnPropertySet(this, "UpperValue", old, value);
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
                OnPropertySet(this, "LowerValue", old, value);
            }
        }

        // === UmlProperty ==========
        //[Persist, External]
        //public UmlOperation Operation {
        //    get { return _operation; }
        //    set {
        //        var old = _operation;
        //        var result = AssociationUtil.EnsureAssociation(
        //            _operation,
        //            value,
        //            ope => _operation = ope,
        //            ope => ope.OwnedParameters.Add(this),
        //            ope => ope.OwnedParameters.Remove(this)
        //        );
        //        if (result != AssociationUtil.EnsureResult.None) {
        //            OnPropertySet(this, "Operation", old, value);
        //        }
        //    }
        //}

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
        public UmlParameterDirectionKind Direction {
            get { return _direction; }
            set {
                if (_direction == value) {
                    return;
                }
                var old = _direction;
                _direction = value;
                OnPropertySet(this, "Direction", old, value);
            }
        }


        // ========================================
        // method
        // ========================================

        // ========================================
        // class
        // ========================================
    }
}
