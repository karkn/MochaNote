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
using Mkamo.Common.Association;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Mkamo.Model.Memo {
    [Entity, Externalizable]
    [DataContract, Serializable]
    public abstract class MemoMarkableElement: MemoElement {
        // ========================================
        // field
        // ========================================
        [DataMember(Name = "Marks")]
        private AssociationCollection<MemoMark> _marks;

        // ========================================
        // constructor
        // ========================================
        protected internal MemoMarkableElement() {
            if (IsMarkable) {
                _marks = new AssociationCollection<MemoMark>(
                    mark => {
                        if (mark.Content != this) {
                            mark.Content = this;
                        }
                    },
                    mark => mark.Content = null,
                    this,
                    "Marks"
                );
                _marks.DetailedPropertyChanged += (sender, e) => OnPropertyChanged(this, e);
            }
        }

        // ========================================
        // property
        // ========================================
        [Persist(Cascade = true), External]
        public virtual Collection<MemoMark> Marks {
            get { return _marks; }
        }

        public virtual bool IsMarkable {
            get { return false; }
        }

    }
}
