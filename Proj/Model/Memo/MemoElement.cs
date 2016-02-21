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
using System.Drawing;
using Mkamo.Model.Core;
using System.Runtime.Serialization;

namespace Mkamo.Model.Memo {
    [Entity, Externalizable, DataContract, Serializable]
    public abstract class MemoElement: DetailedNotifyPropertyChangedBase {
        // ========================================
        // field
        // ========================================
        [DataMember(Name = "CreatedDate")]
        private DateTime _createdDate;
        [DataMember(Name = "ModifiedDate")]
        private DateTime _modifiedDate;

        // ========================================
        // constructor
        // ========================================
        protected internal MemoElement() {
            _createdDate = DateTime.Now;
            _modifiedDate = _createdDate;
        }

        // ========================================
        // property
        // ========================================
        /// <summary>
        /// 作成日時
        /// </summary>
        [Persist, External]
        public virtual DateTime CreatedDate {
            get { return _createdDate; }
            set { _createdDate = value; }
        }

        /// <summary>
        /// 更新日時
        /// </summary>
        [Persist, External]
        public virtual DateTime ModifiedDate {
            get { return _modifiedDate; }
            set { _modifiedDate = value; }
        }

        // ========================================
        // method
        // ========================================
        [Dirty]
        public virtual void Touch() {
        }

        protected override void OnPropertyChanged(object sender, DetailedPropertyChangedEventArgs e) {
            if (!SuppressNotification) {
                _modifiedDate = DateTime.Now;
            }
            base.OnPropertyChanged(sender, e);
        }

        protected virtual void OnPropertyChanged(object sender, DetailedPropertyChangedEventArgs e, bool isModification) {
            if (!SuppressNotification && isModification) {
                _modifiedDate = DateTime.Now;
            }
            base.OnPropertyChanged(sender, e);
        }
    }
}
