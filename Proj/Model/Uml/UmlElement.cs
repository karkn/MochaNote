/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Event;
using Mkamo.Common.Association;
using System.Collections.ObjectModel;
using Mkamo.Container.Core;
using Mkamo.Common.Externalize;
using Mkamo.Model.Core;
using Mkamo.Model.Memo;

namespace Mkamo.Model.Uml {
    [Entity, Externalizable]
    public abstract class UmlElement: MemoContent {
        // ========================================
        // field
        // ========================================
        private AssociationCollection<UmlRelationship> _outgoingRelationships; /// lazy
        private AssociationCollection<UmlRelationship> _incomingRelationships; /// lazy

        private string _stereotype;

        // ========================================
        // constructor
        // ========================================
        protected UmlElement() {
        }

        // ========================================
        // property
        // ========================================
        [Persist, External]
        public virtual Collection<UmlRelationship> OutgoingRelationships {
            get {
                if (_outgoingRelationships == null) {
                    _outgoingRelationships = new AssociationCollection<UmlRelationship>(
                        edge => {
                            if (edge.Source != this) {
                                edge.Source = this;
                            }
                        },
                        edge => edge.Source = null
                    );
                    _outgoingRelationships.EventSender = this;
                    _outgoingRelationships.EventPropertyName = "OutgoingRelationships";
                    _outgoingRelationships.DetailedPropertyChanged += (sender, e) => OnPropertyChanged(this, e);
                }
                return _outgoingRelationships;
            }
        }

        [Persist, External]
        public virtual Collection<UmlRelationship> IncomingRelationships {
            get {
                if (_incomingRelationships == null) {
                    _incomingRelationships = new AssociationCollection<UmlRelationship>(
                        edge => {
                            if (edge.Target != this) {
                                edge.Target = this;
                            }
                        },
                        edge => edge.Target = null
                    );
                    _incomingRelationships.EventSender = this;
                    _incomingRelationships.EventPropertyName = "IncomingRelationships";
                    _incomingRelationships.DetailedPropertyChanged += (sender, e) => OnPropertyChanged(this, e);
                }
                return _incomingRelationships;
            }
        }

        [Persist, External]
        public virtual string Stereotype {
            get { return _stereotype; }
            set {
                if (_stereotype == value) {
                    return;
                }
                var old = _stereotype;
                _stereotype = value;
                OnPropertySet(this, "Stereotype", old, value);
            }
        }


        // ========================================
        // method
        // ========================================

    }
}
