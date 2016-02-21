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
    public abstract class UmlRelationship: UmlElement {
        // ========================================
        // field
        // ========================================
        private UmlElement _source;
        private UmlElement _target;

        // ========================================
        // constructor
        // ========================================
        protected UmlRelationship() {
        }

        protected UmlRelationship(UmlElement source, UmlElement target) {
            Source = source;
            Target = target;
        }

        // ========================================
        // property
        // ========================================
        [Persist, External]
        public virtual UmlElement Source {
            get { return _source; }
            set {
                var old = _source;
                var result = AssociationUtil.EnsureAssociation(
                    _source,
                    value,
                    elem => _source = elem,
                    elem => {
                        if (!elem.OutgoingRelationships.Contains(this)) {
                            elem.OutgoingRelationships.Add(this);
                        }
                    },
                    elem => elem.OutgoingRelationships.Remove(this)
                );
                if (result != AssociationUtil.EnsureResult.None) {
                    OnPropertySet(this, "Source", old, value);
                }
            }
        }

        [Persist, External]
        public virtual UmlElement Target {
            get { return _target; }
            set {
                var old = _target;
                var result = AssociationUtil.EnsureAssociation(
                    _target,
                    value,
                    elem => _target = elem,
                    elem => {
                        if (!elem.IncomingRelationships.Contains(this)) {
                            elem.IncomingRelationships.Add(this);
                        }
                    },
                    elem => elem.IncomingRelationships.Remove(this)
                );
                if (result != AssociationUtil.EnsureResult.None) {
                    OnPropertySet(this, "Target", old, value);
                }
            }
        }

        // ========================================
        // method
        // ========================================
    }
}
