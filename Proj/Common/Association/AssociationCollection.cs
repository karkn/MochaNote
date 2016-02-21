/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Mkamo.Common.Event;
using Mkamo.Common.Collection;
using System.ComponentModel;
using Mkamo.Common.Diagnostics;
using System.Runtime.Serialization;

namespace Mkamo.Common.Association {
    /// <summary>
    /// 双方向の参照を保証するCollection．
    /// 標準のCollectionの振る舞いとは違い，Add()，Insert()ではContains()がtrueであるitemを渡されたときに何もしない．
    /// </summary>
    [Serializable]
    public class AssociationCollection<T>:
        Collection<T>, IDetailedNotifyPropertyChanged, IDetailedNotifyPropertyChanging
        where T: class {

        // ========================================
        // field
        // ========================================
        private Action<T> _associator;
        private Action<T> _unassociator;

        private object _eventSender;
        private string _eventPropertyName;

        [NonSerialized]
        private bool _disableAssociator;

        [NonSerialized]
        private DetailedNotifyPropertyChange _notifier; /// lazy


        // ========================================
        // constructor
        // ========================================
        public AssociationCollection(Action<T> associator, Action<T> unassociator) { 
            Contract.Requires(associator != null);
            Contract.Requires(unassociator != null);

            _associator = associator;
            _unassociator = unassociator;

            _eventSender = this;
            _eventPropertyName = "Associations";
        }

        public AssociationCollection(
            Action<T> associator, Action<T> unassociator, object eventSender, string eventPropertyName
        ) {
            Contract.Requires(associator != null);
            Contract.Requires(unassociator != null);

            _associator = associator;
            _unassociator = unassociator;

            _eventSender = eventSender;
            _eventPropertyName = eventPropertyName;
        }


        // ========================================
        // event
        // ========================================
        public event PropertyChangingEventHandler PropertyChanging {
            add { _Notifier.PropertyChanging += value;}
            remove { _Notifier.PropertyChanging -= value;}
        }

        public event PropertyChangedEventHandler PropertyChanged {
            add { _Notifier.PropertyChanged += value;}
            remove { _Notifier.PropertyChanged -= value;}
        }

        public event EventHandler<DetailedPropertyChangedEventArgs> DetailedPropertyChanged {
            add { _Notifier.DetailedPropertyChanged += value; }
            remove { _Notifier.DetailedPropertyChanged -= value; }
        }

        public event EventHandler<DetailedPropertyChangingEventArgs> DetailedPropertyChanging {
            add { _Notifier.DetailedPropertyChanging += value; }
            remove { _Notifier.DetailedPropertyChanging -= value; }
        }

        // ========================================
        // property
        // ========================================
        public IQualifiedEventHandlers<DetailedPropertyChangedEventArgs> NamedPropertyChanged {
            get { return _Notifier.NamedPropertyChanged; }
        }

        public IQualifiedEventHandlers<DetailedPropertyChangingEventArgs> NamedPropertyChanging {
            get { return _Notifier.NamedPropertyChanging; }
        }

        // === AssociationCollection ==========
        public object EventSender {
            get { return _eventSender; }
            set { _eventSender = value; }
        }

        public string EventPropertyName {
            get { return _eventPropertyName; }
            set { _eventPropertyName = value; }
        }

        public bool SuppressNotification {
            get { return _Notifier.SuppressNotification; }
            set { _Notifier.SuppressNotification = value; }
        }

        public bool DisableAssociator {
            get { return _disableAssociator; }
            set { _disableAssociator = value; }
        }


        protected DetailedNotifyPropertyChange _Notifier {
            get { return _notifier?? (_notifier = new DetailedNotifyPropertyChange()); }
        }

        // ========================================
        // method
        // ========================================
        protected override void SetItem(int index, T item) {
            Contract.Requires(item != null);
            Contract.Requires(index >= 0 && index <= Count - 1, "index");

            T old = this[index];
            if (item == old) {
                return;
            }
            if (Contains(item)) {
                return;
            }

            _Notifier.NotifyPropertySetting(_eventSender, _eventPropertyName, old, item, index);

            if (old != null) {
                if (!_disableAssociator) {
                    _unassociator(old);
                    base.SetItem(index, null);
                }
            }

            base.SetItem(index, item);

            if (item != null && !_disableAssociator) {
                _associator(item);
            }
            _Notifier.NotifyPropertySet(_eventSender, _eventPropertyName, old, item, index);
        }

        protected override void InsertItem(int index, T item) {
            Contract.Requires(item != null);
            Contract.Requires(index >= 0 && index <= Count);

            if (!Contains(item)) {
                _Notifier.NotifyPropertyAdding(_eventSender, _eventPropertyName, item, index);
                base.InsertItem(index, item);
                if (!_disableAssociator) {
                    _associator(item);
                }
                _Notifier.NotifyPropertyAdded(_eventSender, _eventPropertyName, item, index);
            }
        }

        protected override void RemoveItem(int index) {
            Contract.Requires(index >= 0 && index <= Count - 1);

            T target = this[index];
            _Notifier.NotifyPropertyRemoving(_eventSender, _eventPropertyName, target, index);
            base.RemoveItem(index);
            if (!_disableAssociator) {
                _unassociator(target);
            }
            _Notifier.NotifyPropertyRemoved(_eventSender, _eventPropertyName, target, index);
        }

        protected override void ClearItems() {
            if (Count == 0) {
                return;
            }

            T[] old = null;

            if (!(_Notifier.IsEmpty || _Notifier.SuppressNotification)) {
                old = new T[Count];
                CopyTo(old, 0);
                _Notifier.NotifyPropertyClearing(_eventSender, _eventPropertyName, old);
            }

            int len = Count;
            for (int i = 0; i < len; ++i) {
                T target = this[0];
                base.RemoveItem(0);
                if (!_disableAssociator) {
                    _unassociator(target);
                }
            }

            if (!(_Notifier.IsEmpty || _Notifier.SuppressNotification)) {
                _Notifier.NotifyPropertyCleared(_eventSender, _eventPropertyName, old);
            }
        }
    }
}
