/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Collection;
using Mkamo.Common.Event;
using Mkamo.Common.Core;
using System.Collections;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Mkamo.Common.Association {
    [Serializable]
    public class AssociationList<T>: IList<T>, IDetailedNotifyPropertyChanged, IDetailedNotifyPropertyChanging {
        // ========================================
        // static field
        // ========================================
        private static readonly IList<T> EmptyList = new EmptyList<T>();

        // ========================================
        // field
        // ========================================
        private IList<T> _list;
        private Action<T> _associator;
        private Action<T> _unassociator;

        private object _eventSender;
        private string _eventPropertyName;

        [NonSerialized]
        private DetailedNotifyPropertyChange _notifier;

        // ========================================
        // constructor
        // ========================================
        public AssociationList(IList<T> list, Action<T> associator, Action<T> unassociator) {
            if (list == null || associator == null || unassociator == null) {
                throw new ArgumentNullException("list or associator or unassociator");
            }

            _list = list;
            _associator = associator;
            _unassociator = unassociator;

            _eventSender = this;
            _eventPropertyName = Property.Associations;

            _notifier = new DetailedNotifyPropertyChange();
        }

        public AssociationList(Action<T> associator, Action<T> unassociator):
            this(EmptyList, associator, unassociator)
        {
        }


        [OnDeserialized]
        internal void InitDeserializedCompositeSupport(StreamingContext context) {
            _notifier = new DetailedNotifyPropertyChange();
        }

        // ========================================
        // event
        // ========================================
        public event PropertyChangingEventHandler PropertyChanging {
            add { _notifier.PropertyChanging += value;}
            remove { _notifier.PropertyChanging -= value;}
        }

        public event PropertyChangedEventHandler PropertyChanged {
            add { _notifier.PropertyChanged += value;}
            remove { _notifier.PropertyChanged -= value;}
        }

        public event EventHandler<DetailedPropertyChangedEventArgs> DetailedPropertyChanged {
            add { _notifier.DetailedPropertyChanged += value; }
            remove { _notifier.DetailedPropertyChanged -= value; }
        }
        public event EventHandler<DetailedPropertyChangingEventArgs> DetailedPropertyChanging {
            add { _notifier.DetailedPropertyChanging += value; }
            remove { _notifier.DetailedPropertyChanging -= value; }
        }

        // ========================================
        // property
        // ========================================
        public IQualifiedEventHandlers<DetailedPropertyChangedEventArgs> NamedPropertyChanged {
            get { return _notifier.NamedPropertyChanged; }
        }

        public IQualifiedEventHandlers<DetailedPropertyChangingEventArgs> NamedPropertyChanging {
            get { return _notifier.NamedPropertyChanging; }
        }

        // === IList ==========
        public int Count {
            get { return _list.Count; }
        }

        public bool IsReadOnly {
            get { return false; }
        }

        public T this[int index] {
            get { return _list[index]; }
            set {
                if (value == null) {
                    throw new ArgumentNullException("value");
                }

                T old = _list[index];
                _notifier.NotifyPropertySetting(_eventSender, _eventPropertyName, old, value, index);

                // remove at
                _list.RemoveAt(index);
                _unassociator(old);

                // insert
                if (!_list.Contains(value)) {
                    _list.Insert(index, value);
                    _associator(value);
                }

                _notifier.NotifyPropertySet(_eventSender, _eventPropertyName, old, value, index);
            }
        }

        // === AssociationList ==========
        public object EventSender {
            get { return _eventSender; }
            set { _eventSender = value; }
        }

        public string EventPropertyName {
            get { return _eventPropertyName; }
            set { _eventPropertyName = value; }
        }

        public bool SuppressNotification {
            get { return _notifier.SuppressNotification; }
            set { _notifier.SuppressNotification = value; }
        }

        // ========================================
        // method
        // ========================================
        // === IList ==========
        // --- write ---
        public void Add(T target) {
            if (target == null) {
                throw new ArgumentNullException("target");
            }

            if (!_list.Contains(target)) {
                if (_list.IsEmptyList()) {
                    _list = new List<T>(4);
                }
                _notifier.NotifyPropertyAdding(_eventSender, _eventPropertyName, target, _list.Count - 1);
                _list.Add(target);
                _associator(target);
                _notifier.NotifyPropertyAdded(_eventSender, _eventPropertyName, target, _list.Count - 1);
            }
        }

        public bool Remove(T target) {
            if (target == null) {
                throw new ArgumentNullException("target");
            }

            int index = _list.IndexOf(target);
            if (index > -1) {
                _notifier.NotifyPropertyRemoving(_eventSender, _eventPropertyName, target, index);
                if (_list.Remove(target)) {
                    _unassociator(target);
                    _notifier.NotifyPropertyRemoved(_eventSender, _eventPropertyName, target, index);
                    return true;
                }
            }
            return false;
        }

        public void Insert(int index, T target) {
            if (target == null) {
                throw new ArgumentNullException("target");
            }

            if (!_list.Contains(target)) {
                if (_list.IsEmptyList()) {
                    _list = new List<T>(4);
                }
                _notifier.NotifyPropertyAdding(_eventSender, _eventPropertyName, target, index);
                _list.Insert(index, target);
                _associator(target);
                _notifier.NotifyPropertyAdded(_eventSender, _eventPropertyName, target, index);
            }
        }

        public void RemoveAt(int index) {
            T target = _list[index];
            _notifier.NotifyPropertyRemoving(_eventSender, _eventPropertyName, target, index);
            _list.RemoveAt(index);
            _unassociator(target);
            _notifier.NotifyPropertyRemoved(_eventSender, _eventPropertyName, target, index);
        }

        public void Clear() {
            if (Count == 0) {
                return;
            }

            T[] old = new T[_list.Count];
            _list.CopyTo(old, 0);
            _notifier.NotifyPropertyClearing(_eventSender, _eventPropertyName, old);
            int len = _list.Count;
            for (int i = 0; i < len; ++i) {
                T target = _list[0];
                _list.RemoveAt(0);
                _unassociator(target);
            }
            _notifier.NotifyPropertyCleared(_eventSender, _eventPropertyName, old);
        }

        // --- read ---
        public int IndexOf(T item) {
            return _list.IndexOf(item);
        }

        public bool Contains(T item) {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex) {
            _list.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator() {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _list.GetEnumerator();
        }


        public static class Property {
            public static readonly string Associations = "Associations";
        }
    }
}
