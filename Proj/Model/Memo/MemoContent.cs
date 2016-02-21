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
    [DataContract]
    [Serializable]
    public class MemoContent: MemoMarkableElement {
        // ========================================
        // field
        // ========================================
        [DataMember(Name = "Keywords")]
        private string _keywords;

        // ========================================
        // constructor
        // ========================================
        protected internal MemoContent() {
        }

        // ========================================
        // property
        // ========================================
        /// <summary>
        /// 検索のための付加キーワード
        /// </summary>
        [Persist, External]
        public virtual string Keywords {
            get { return _keywords; }
            set { _keywords = value; }
        }

        // ========================================
        // method
        // ========================================

    }
}
