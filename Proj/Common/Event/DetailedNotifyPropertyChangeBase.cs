/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Mkamo.Common.Event;
using System.ComponentModel;

namespace Mkamo.Common.Event {
    [Serializable]
    public class DetailedNotifyPropertyChangeBase: DetailedNotifyPropertyChangedBase, IDetailedNotifyPropertyChanging {
        // ========================================
        // field
        // ========================================
        [NonSerialized]
        private QualifiedEventHandlers<DetailedPropertyChangingEventArgs> _namedPropertyChanging;

        // ========================================
        // constructor
        // ========================================
        public DetailedNotifyPropertyChangeBase() {
            _namedPropertyChanging = null; // lazy load
        }

        // ========================================
        // property
        // ========================================
        public virtual IQualifiedEventHandlers<DetailedPropertyChangingEventArgs> NamedPropertyChanging {
            get { return _NamedPropertyChanging; }
        }

        /// <summary>
        /// イベントハンドラがまだ登録されていなければtrueを返す．
        /// </summary>
        public override bool IsEmpty {
            get { return base.IsEmpty && _IsPropertyChangingEmpty; }
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected virtual QualifiedEventHandlers<DetailedPropertyChangingEventArgs> _NamedPropertyChanging {
            get {
                if (_namedPropertyChanging == null) {
                    _namedPropertyChanging = new QualifiedEventHandlers<DetailedPropertyChangingEventArgs>();
                }
                return _namedPropertyChanging;
            }
        }

        // ------------------------------
        // private
        // ------------------------------
        private bool _IsPropertyChangingEmpty {
            get { return (_namedPropertyChanging == null || _namedPropertyChanging.IsEmpty) && PropertyChanging == null; }
        }

        // ========================================
        // event
        // ========================================
        // === INotifyPropertyChanging ==========
        [field:NonSerialized]
        public virtual event PropertyChangingEventHandler PropertyChanging;

        // === IDetailedNotifyPropertyChanging ==========
        public virtual event EventHandler<DetailedPropertyChangingEventArgs> DetailedPropertyChanging {
            add { _NamedPropertyChanging.AddHandler(value); }
            remove { _NamedPropertyChanging.RemoveHandler(value); }
        }


        // ========================================
        // method
        // ========================================    
        protected virtual void OnPropertyChanging(object sender, DetailedPropertyChangingEventArgs e) {
            if (!SuppressNotification) {
                var tmp = PropertyChanging;
                if (tmp != null) {
                    tmp(sender, e);
                }

                if(_namedPropertyChanging != null) {
                    _NamedPropertyChanging.Notify(sender, e.PropertyName, e);
                }
            }
        }

        protected virtual void OnPropertyChanging<TValue>(
            object sender, string propertyName, PropertyChangeKind kind, TValue oldValue, TValue newValue
        ) {
            if (!_IsPropertyChangingEmpty) {
                var e = new DetailedPropertyChangingEventArgs<TValue>(propertyName, kind, oldValue, newValue);
                OnPropertyChanging(sender, e);
            }
        }

        protected virtual void OnPropertyChanging<TValue>(
            object sender, string propertyName, PropertyChangeKind kind, TValue oldValue, TValue newValue, int index
        ) {
            if (!_IsPropertyChangingEmpty) {
                var e = new DetailedPropertyChangingEventArgs<TValue>(propertyName, kind, oldValue, newValue, index);
                OnPropertyChanging(sender, e);
            }
        }

        // --- no indexed ---
        protected virtual void OnPropertySetting<TValue>(object sender, string propertyName, TValue oldValue, TValue newValue) {
            OnPropertyChanging<TValue>(sender, propertyName, PropertyChangeKind.Set, oldValue, newValue);
        }

        protected virtual void OnPropertyUnsetting<TValue>(object sender, string propertyName, TValue oldValue) {
            OnPropertyChanging<TValue>(sender, propertyName, PropertyChangeKind.Unset, oldValue, default(TValue));
        }

        protected virtual void OnPropertyClearing<TValue>(object sender, string propertyName, TValue oldValue) {
            OnPropertyChanging<TValue>(sender, propertyName, PropertyChangeKind.Clear, oldValue, default(TValue));
        }

        // --- indexed ---
        protected virtual void OnPropertySetting<TValue>(object sender, string propertyName, TValue oldValue, TValue newValue, int index) {
            OnPropertyChanging<TValue>(sender, propertyName, PropertyChangeKind.Set, oldValue, newValue, index);
        }

        protected virtual void OnPropertyUnsetting<TValue>(object sender, string propertyName, TValue oldValue, int index) {
            OnPropertyChanging<TValue>(sender, propertyName, PropertyChangeKind.Unset, oldValue, default(TValue), index);
        }

        protected virtual void OnPropertyAdding<TValue>(object sender, string propertyName, TValue newValue, int index) {
            OnPropertyChanging<TValue>(sender, propertyName, PropertyChangeKind.Add, default(TValue), newValue, index);
        }

        //protected virtual void OnPropertyAddingMany<TValue>(object sender, string propertyName, TValue newValue, int index) {
        //    OnPropertyChanging<TValue>(sender, propertyName, PropertyChangeKind.AddMany, default(TValue), newValue, index);
        //}

        protected virtual void OnPropertyRemoving<TValue>(object sender, string propertyName, TValue oldValue, int index) {
            OnPropertyChanging<TValue>(sender, propertyName, PropertyChangeKind.Remove, oldValue, default(TValue), index);
        }

        //protected virtual void OnPropertyRemovingMany<TValue>(object sender, string propertyName, TValue oldValue, int index) {
        //    OnPropertyChanging<TValue>(sender, propertyName, PropertyChangeKind.RemoveMany, oldValue, default(TValue), index);
        //}

    }
}
