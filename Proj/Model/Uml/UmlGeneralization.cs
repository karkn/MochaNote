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
using Mkamo.Model.Core;

namespace Mkamo.Model.Uml {
    [Externalizable(Type = typeof(UmlGeneralization), FactoryMethodType = typeof(UmlFactory), FactoryMethod = "CreateGeneralization")]
    public class UmlGeneralization: UmlRelationship {
        // ========================================
        // field
        // ========================================
        private UmlClassifier _specific;
        private UmlClassifier _general;

        // ========================================
        // constructor
        // ========================================
        protected internal UmlGeneralization() {
        }

        // ========================================
        // property
        // ========================================
        [Persist, External]
        public virtual UmlClassifier Specific {
            get { return _specific; }
            set {
                var old = _specific;
                var result = AssociationUtil.EnsureAssociation(
                    _specific,
                    value,
                    cls => _specific = cls,
                    cls => {
                        if (!cls.OutgoingGeneralizations.Contains(this)) {
                            cls.OutgoingGeneralizations.Add(this);
                        }
                    },
                    cls => cls.OutgoingGeneralizations.Remove(this)
                );
                if (result != AssociationUtil.EnsureResult.None) {
                    OnPropertySet(this, "Specific", old, value);
                }
            }
        }

        [Persist, External]
        public virtual UmlClassifier General {
            get { return _general; }
            set {
                var old = _general;
                var result = AssociationUtil.EnsureAssociation(
                    _general,
                    value,
                    cls => _general = cls,
                    cls => {
                        if (!cls.IncomingGeneralizations.Contains(this)) {
                            cls.IncomingGeneralizations.Add(this);
                        }
                    },
                    cls => cls.IncomingGeneralizations.Remove(this)
                );
                if (result != AssociationUtil.EnsureResult.None) {
                    OnPropertySet(this, "General", old, value);
                }
            }
        }
    }
}
