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

namespace Mkamo.Model.Uml {
    public class UmlOperationCollection: Collection<UmlOperation>, INotifyPropertyChanged {
        // ========================================
        // field
        // ========================================
        private UmlClassifier _owner;

        // ========================================
        // constructor
        // ========================================
        protected internal UmlOperationCollection(UmlClassifier owner) {
            _owner = owner;
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
        protected override void InsertItem(int index, UmlOperation item) {
            base.InsertItem(index, item);
            OnOperationsChanged();
        }

        protected override void RemoveItem(int index) {
            base.RemoveItem(index);
            OnOperationsChanged();
        }

        protected override void SetItem(int index, UmlOperation item) {
            base.SetItem(index, item);
            OnOperationsChanged();
        }

        protected override void ClearItems() {
            base.ClearItems();
            OnOperationsChanged();
        }

        protected virtual void OnOperationsChanged() {
            var handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs("Operations"));
            }
        }
    }
}
