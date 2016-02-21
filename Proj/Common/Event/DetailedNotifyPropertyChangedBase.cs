/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Mkamo.Common.Event;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Mkamo.Common.Event {
    /// <summary>
    /// IDetailedNotifyPropertyChangedを実装するクラスのための基底クラス．
    /// </summary>
    [Serializable]
    public class DetailedNotifyPropertyChangedBase: IDetailedNotifyPropertyChanged {
        // ========================================
        // field
        // ========================================
        [NonSerialized]
        private QualifiedEventHandlers<DetailedPropertyChangedEventArgs> _namedPropertyChanged; /// lazy load

        [NonSerialized]
        private bool _suppressNotification = false;

        // ========================================
        // constructor
        // ========================================
        protected DetailedNotifyPropertyChangedBase() {
        }

        
        // ========================================
        // property
        // ========================================
        // === IPropertyChangedNotifier ==========
        public virtual IQualifiedEventHandlers<DetailedPropertyChangedEventArgs> NamedPropertyChanged {
            get { return _NamedPropertyChanged; }
        }

        [XmlIgnore]
        public virtual bool SuppressNotification {
            get { return _suppressNotification; }
            set { _suppressNotification = value; }
        }

        /// <summary>
        /// イベントハンドラがまだ登録されていなければtrueを返す．
        /// </summary>
        public virtual bool IsEmpty {
            get { return _IsPropertyChangedEmpty; }
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected virtual QualifiedEventHandlers<DetailedPropertyChangedEventArgs> _NamedPropertyChanged {
            get {
                if (_namedPropertyChanged == null) {
                    _namedPropertyChanged = new QualifiedEventHandlers<DetailedPropertyChangedEventArgs>();
                }
                return _namedPropertyChanged;
            }
        }

        // ------------------------------
        // private
        // ------------------------------
        private bool _IsPropertyChangedEmpty {
            get { return (_namedPropertyChanged == null || _namedPropertyChanged.IsEmpty) && PropertyChanged == null; }
        }

        // ========================================
        // event
        // ========================================
        // === INotifyPropertyChanged ==========
        [field:NonSerialized]
        public virtual event PropertyChangedEventHandler PropertyChanged;

        // === IDetailedNotifyPropertyChanged ==========
        public virtual event EventHandler<DetailedPropertyChangedEventArgs> DetailedPropertyChanged {
            add { _NamedPropertyChanged.AddHandler(value); }
            remove { _NamedPropertyChanged.RemoveHandler(value); }
        }

        // ========================================
        // method
        // ========================================
        protected virtual void OnPropertyChanged(object sender, DetailedPropertyChangedEventArgs e) {
            if (!_suppressNotification) {
                var handler = PropertyChanged;
                if (handler != null) {
                    handler(sender, e);
                }

                if (_namedPropertyChanged != null) {
                    _NamedPropertyChanged.Notify(sender, e.PropertyName, e);
                }
            }
        }

        protected virtual void OnPropertyChanged<TValue>(
            object sender, string propertyName, PropertyChangeKind kind, TValue oldValue, TValue newValue
        ) {
            if (!_IsPropertyChangedEmpty) {
                var e = new DetailedPropertyChangedEventArgs<TValue>(propertyName, kind, oldValue, newValue);
                OnPropertyChanged(sender, e);
            }
        }

        protected virtual void OnPropertyChanged<TValue>(
            object sender, string propertyName, PropertyChangeKind kind, TValue oldValue, TValue newValue, int index
        ) {
            if (!_IsPropertyChangedEmpty) {
                var e = new DetailedPropertyChangedEventArgs<TValue>(propertyName, kind, oldValue, newValue, index);
                OnPropertyChanged(sender, e);
            }
        }

        // --- no indexed ---
        protected virtual void OnPropertySet<TValue>(object sender, string propertyName, TValue oldValue, TValue newValue) {
            OnPropertyChanged<TValue>(sender, propertyName, PropertyChangeKind.Set, oldValue, newValue);
        }

        protected virtual void OnPropertyUnset<TValue>(object sender, string propertyName, TValue oldValue) {
            OnPropertyChanged<TValue>(sender, propertyName, PropertyChangeKind.Unset, oldValue, default(TValue));
        }

        protected virtual void OnPropertyCleared<TValue>(object sender, string propertyName, TValue oldValue) {
            OnPropertyChanged<TValue>(sender, propertyName, PropertyChangeKind.Clear, oldValue, default(TValue));
        }

        // --- indexed ---
        protected virtual void OnPropertySet<TValue>(object sender, string propertyName, TValue oldValue, TValue newValue, int index) {
            OnPropertyChanged<TValue>(sender, propertyName, PropertyChangeKind.Set, oldValue, newValue, index);
        }

        protected virtual void OnPropertyUnset<TValue>(object sender, string propertyName, TValue oldValue, int index) {
            OnPropertyChanged<TValue>(sender, propertyName, PropertyChangeKind.Unset, oldValue, default(TValue), index);
        }

        protected virtual void OnPropertyAdded<TValue>(object sender, string propertyName, TValue newValue, int index) {
            OnPropertyChanged<TValue>(sender, propertyName, PropertyChangeKind.Add, default(TValue), newValue, index);
        }

        //protected virtual void OnPropertyAddedMany<TValue>(object sender, string propertyName, TValue newValues, int index) {
        //    OnPropertyChanged<TValue>(sender, propertyName, PropertyChangeKind.AddMany, default(TValue), newValues, index);
        //}

        protected virtual void OnPropertyRemoved<TValue>(object sender, string propertyName, TValue oldValue, int index) {
            OnPropertyChanged<TValue>(sender, propertyName, PropertyChangeKind.Remove, oldValue, default(TValue), index);
        }

        //protected virtual void OnPropertyRemovedMany<TValue>(object sender, string propertyName, TValue oldValues, int index) {
        //    OnPropertyChanged<TValue>(sender, propertyName, PropertyChangeKind.RemoveMany, oldValues, default(TValue), index);
        //}


    }
}
