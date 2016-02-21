using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Externalize;
using Mkamo.Model.Core;
using Mkamo.Container.Core;
using System.Runtime.Serialization;

namespace Mkamo.Model.Memo {
    [Externalizable(Type = typeof(MemoAnchorReference), FactoryMethodType = typeof(MemoFactory), FactoryMethod = "CreateAnchorReference")]
    [DataContract]
    [Serializable]
    public class MemoAnchorReference: MemoEdge {
        // ========================================
        // field
        // ========================================
        [DataMember(Name = "AnchorId")]
        private string _anchorId;

        // ========================================
        // constructor
        // ========================================
        protected internal MemoAnchorReference() {
        }


        // ========================================
        // property
        // ========================================
        [Persist, External]
        public virtual string AnchorId {
            get { return _anchorId; }
            set{
                if (_anchorId == value) {
                    return;
                }
                var old = _anchorId;
                _anchorId = value;
                OnPropertySet(this, "AnchorId", old, value);
            }
        }
    }
}
