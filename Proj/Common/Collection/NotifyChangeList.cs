/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Mkamo.Common.Event;
using System.Collections;

namespace Mkamo.Common.Collection {
    [Serializable]
    public class NotifyChangeList<T>: DetailedNotifyPropertyChangeBase, IList<T> {
        // ========================================
        // field
        // ========================================
        private IList<T> _list;
        private object _eventSender;
        private string _eventPropertyName;

        // ========================================
        // constructor
        // ========================================
        public NotifyChangeList(IList<T> list): base() {
            _list = list;
            _eventSender = this;
            _eventPropertyName = "Items";
        }

        public NotifyChangeList(IList<T> list, object eventSender, string eventPropertyName): base() {
            _list = list;
            _eventSender = eventSender;
            _eventPropertyName = eventPropertyName;
        }


        // ========================================
        // property
        // ========================================
        public T this[int index] {
            get { return _list[index]; }
            set {
                T old = _list[index];
                if (EqualityComparer<T>.Default.Equals(value, old)) {
                    return;
                }
                OnPropertySetting(_eventSender, _eventPropertyName, old, value, index);
                _list[index] = value;
                OnPropertySet(_eventSender, _eventPropertyName, old, value, index);
            }
        }

        public int Count {
            get { return _list.Count; }
        }

        public bool IsReadOnly {
            get { return _list.IsReadOnly; }
        }

        public object EventSender {
            get { return _eventSender; }
            set { _eventSender = value; }
        }

        public string EventPropertyName {
            get { return _eventPropertyName; }
            set { _eventPropertyName = value; }
        }

        // ========================================
        // method
        // ========================================
        // === IList ==========
        public int IndexOf(T item) {
            return _list.IndexOf(item);
        }

        public void Add(T item) {
            int count = _list.Count;
            OnPropertyAdding(_eventSender, _eventPropertyName, item, count);
            _list.Add(item);
            OnPropertyAdded(_eventSender, _eventPropertyName, item, count);
        }

        public void Insert(int index, T item) {
            OnPropertyAdding(_eventSender, _eventPropertyName, item, index);
            _list.Insert(index, item);
            OnPropertyAdded(_eventSender, _eventPropertyName, item, index);
        }

        public bool Remove(T item) {
            int index = _list.IndexOf(item);
            if (index > -1) {
                OnPropertyRemoving(_eventSender, _eventPropertyName, item, index);
                bool ret = _list.Remove(item);
                if (ret) {
                    OnPropertyRemoved(_eventSender, _eventPropertyName, item, index);
                }
                return ret;
            }
            return false;
        }

        public void RemoveAt(int index) {
            T old = _list[index];
            OnPropertyRemoving(_eventSender, _eventPropertyName, old, index);
            _list.RemoveAt(index);
            OnPropertyRemoved(_eventSender, _eventPropertyName, old, index);
        }

        public void Clear() {
            T[] old = null;
            if (!IsEmpty) {
                old = new T[_list.Count];
                _list.CopyTo(old, 0);
                OnPropertyClearing(_eventSender, _eventPropertyName, old);
            }
            _list.Clear();
            if (!IsEmpty) {
                OnPropertyCleared(_eventSender, _eventPropertyName, old);
            }
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
    }
}
