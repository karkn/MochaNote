/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Association;
using Mkamo.StyledText.Core;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Common.Event;
using System.Collections.ObjectModel;
using Mkamo.Container.Core;
using Mkamo.Common.Externalize;
using Mkamo.Model.Core;
using Core = Mkamo.StyledText.Core;
using System.Runtime.Serialization;

namespace Mkamo.Model.Memo {
    [Entity, Externalizable]
    [DataContract, Serializable]
    public class MemoNode: MemoStyledText {
        // ========================================
        // field
        // ========================================
        [DataMember(Name = "Outgoings")]
        private AssociationCollection<MemoEdge> _outgoings;
        [DataMember(Name = "Incomings")]
        private AssociationCollection<MemoEdge> _incomings;

        // ========================================
        // constructor
        // ========================================
        protected internal MemoNode() {
            _outgoings = new AssociationCollection<MemoEdge>(
                edge => {
                    if (edge.Source != this) {
                        edge.Source = this;
                    }
                },
                edge => edge.Source = null,
                this,
                "Outgoings"
            );
            _outgoings.DetailedPropertyChanged += (sender, e) => OnPropertyChanged(this, e);

            _incomings = new AssociationCollection<MemoEdge>(
                edge => {
                    if (edge.Target != this) {
                        edge.Target = this;
                    }
                },
                edge => edge.Target = null,
                this,
                "Incomings"
            );
            _incomings.DetailedPropertyChanged += (sender, e) => OnPropertyChanged(this, e);

        }

        // ========================================
        // property
        // ========================================
        public override bool IsMarkable {
            get { return true; }
        }

        [Persist, External]
        public virtual Collection<MemoEdge> Outgoings {
            get { return _outgoings; }
        }

        [Persist, External]
        public virtual Collection<MemoEdge> Incomings {
            get { return _incomings; }
        }
    }

}
