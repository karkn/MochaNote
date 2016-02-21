/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Externalize;
using Mkamo.Model.Core;
using Mkamo.Container.Core;
using Mkamo.Common.Association;
using System.Runtime.Serialization;

namespace Mkamo.Model.Memo {
    [Externalizable(Type = typeof(MemoMark), FactoryMethodType = typeof(MemoFactory), FactoryMethod = "CreateMark")]
    [DataContract, Serializable]
    public class MemoMark: MemoElement {
        // ========================================
        // field
        // ========================================
        //private MemoMarkDefinition _definition;
        [DataMember(Name = "Kind")]
        private MemoMarkKind _kind;

        [DataMember(Name = "Value")]
        private string _value;
        [DataMember(Name = "Content")]
        private MemoMarkableElement _content;

        // ========================================
        // constructor
        // ========================================
        protected internal MemoMark() {
        }

        // ========================================
        // property
        // ========================================
        //[Persist, External]
        //public virtual MemoMarkDefinition Definition {
        //    get { return _definition; }
        //    set {
        //        if (_definition == value) {
        //            return;
        //        }
        //        var old = _definition;
        //        _definition = value;
        //        OnPropertySet(this, "Definition", old, value);
        //    }
        //}

        [Persist, External]
        public virtual MemoMarkKind Kind {
            get { return _kind; }
            set {
                if (_kind == value) {
                    return;
                }
                var old = _kind;
                _kind = value;
                OnPropertySet(this, "Kind", old, value);
            }
        }

        [Persist, External]
        public virtual string Value {
            get { return _value; }
            set {
                if (_value == value) {
                    return;
                }
                var old = _value;
                _value = value;
                OnPropertySet(this, "Value", old, value);
            }
        }

        [Persist, External]
        public virtual MemoMarkableElement Content {
            get { return _content; }
            set {
                var old = _content;
                var result = AssociationUtil.EnsureAssociation(
                    _content,
                    value,
                    content => _content = content,
                    content => {
                        if (!content.Marks.Contains(this)) {
                            content.Marks.Add(this);
                        }
                    },
                    content => content.Marks.Remove(this)
                );
                if (result != AssociationUtil.EnsureResult.None) {
                    OnPropertySet(this, "Content", old, value);
                }
            }
        }

        // ========================================
        // method
        // ========================================

    }
}
