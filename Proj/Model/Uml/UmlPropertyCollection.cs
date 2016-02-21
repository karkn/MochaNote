/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Mkamo.Common.Externalize;

namespace Mkamo.Model.Uml {
    public class UmlPropertyCollection: Collection<UmlProperty>, INotifyPropertyChanged {
        // ========================================
        // field
        // ========================================
        private UmlClassifier _owner;

        // ========================================
        // constructor
        // ========================================
        protected internal UmlPropertyCollection(UmlClassifier owner) {
            _owner = owner;
        }

        protected internal UmlPropertyCollection() {
        }

        // ========================================
        // event
        // ========================================
        public virtual event PropertyChangedEventHandler PropertyChanged;

        // ========================================
        // property
        // ========================================
        public UmlClassifier Owner {
            get { return _owner; }
        }

        // ========================================
        // method
        // ========================================
        protected override void InsertItem(int index, UmlProperty item) {
            base.InsertItem(index, item);
            OnPropertiesChanged();
        }

        protected override void RemoveItem(int index) {
            base.RemoveItem(index);
            OnPropertiesChanged();
        }

        protected override void SetItem(int index, UmlProperty item) {
            base.SetItem(index, item);
            OnPropertiesChanged();
        }

        protected override void ClearItems() {
            base.ClearItems();
            OnPropertiesChanged();
        }

        protected virtual void OnPropertiesChanged() {
            var handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs("Properties"));
            }
        }
    }
}
