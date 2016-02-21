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

namespace Mkamo.Model.Memo {
    [Entity, Externalizable]
    public abstract class MemoQueryHolder: MemoElement {
        // ========================================
        // field
        // ========================================
        private string _name;
        private MemoQuery _query;

        // ========================================
        // constructor
        // ========================================
        protected internal MemoQueryHolder() {
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


        [Persist(Cascade = true), External]
        public virtual MemoQuery Query {
            get { return _query; }
            set {
                if (_query == value) {
                    return;
                }
                var old = _query;
                _query = value;
                OnPropertySet(this, "Query", old, value);
            }
        }


        // ========================================
        // method
        // ========================================

    }
}
