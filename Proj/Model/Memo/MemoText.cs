/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Event;
using Mkamo.Container.Core;
using Mkamo.Common.Externalize;
using Mkamo.Model.Core;
using Core = Mkamo.StyledText.Core;
using System.Runtime.Serialization;

namespace Mkamo.Model.Memo {
    [Externalizable(Type = typeof(MemoText), FactoryMethodType = typeof(MemoFactory), FactoryMethod = "CreateText")]
    [DataContract, Serializable]
    public class MemoText: MemoNode {
        // ========================================
        // field
        // ========================================
        [DataMember(Name = "IsSticky")]
        private bool _isSticky = false;

        // ========================================
        // constructor
        // ========================================
        protected internal MemoText() {
        }

        // ========================================
        // property
        // ========================================
        [Persist, External]
        public virtual bool IsSticky {
            get { return _isSticky; }
            set {
                if (_isSticky == value) {
                    return;
                }
                var old = _isSticky;
                _isSticky = value;
                OnPropertySet(this, "IsSticky", old, value);
            }
        }
        

        // ========================================
        // method
        // ========================================
    }
}
