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
using Mkamo.Model.Core;
using Mkamo.Common.Association;
using System.Collections.ObjectModel;

namespace Mkamo.Model.Uml {
    [Externalizable(Type = typeof(UmlOperation), FactoryMethodType = typeof(UmlFactory), FactoryMethod = "CreateOperation")]
    public class UmlOperation: UmlFeature {
        // ========================================
        // field
        // ========================================
        private bool _isAbstract;
        private string _parameters;

        //private AssociationCollection<UmlParameter> _ownedParameters; /// lazy
        
        // ========================================
        // constructor
        // ========================================
        protected internal UmlOperation() {
            _isAbstract = false;
            Visibility = UmlVisibilityKind.Public;
        }

        // ========================================
        // property
        // ========================================
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

        [Persist, External]
        public virtual string Parameters {
            get { return _parameters; }
            set {
                if (_parameters == value) {
                    return;
                }
                var old = _parameters;
                _parameters = value;
                OnPropertySet(this, "Parameters", old, value);
            }
        }

        //[Persist(Cascade = true), External]
        //public virtual Collection<UmlParameter> OwnedParameters {
        //    get {
        //        if (_ownedParameters == null) {
        //            _ownedParameters = new AssociationCollection<UmlParameter>(
        //                para => para.Operation = this,
        //                para => para.Operation = null,
        //                this,
        //                "OwnedParameters"
        //            );
        //            _ownedParameters.DetailedPropertyChanged += (sender, e) => OnPropertyChanged(this, e);
        //        }
        //        return _ownedParameters;
        //    }
        //}

        // ========================================
        // method
        // ========================================

    }
}
