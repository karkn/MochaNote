/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Mkamo.Common.Event;

namespace Mkamo.Common.Event {
    [Serializable]
    public class DetailedNotifyPropertyChange: DetailedNotifyPropertyChangeBase, IDetailedNotifyPropertyChanging {
        // ========================================
        // constructor
        // ========================================
        public DetailedNotifyPropertyChange() {
        }

        // ========================================
        // method
        // ========================================    
        // === PropertyChangedNotifierBase ==========
        public virtual void NotifyPropertyChanged(object sender, DetailedPropertyChangedEventArgs e) {
            base.OnPropertyChanged(sender, e);
        }

        public virtual void NotifyPropertyChanged<TValue>(
            object sender, string propertyName, PropertyChangeKind kind, TValue oldValue, TValue newValue
        ) {
            base.OnPropertyChanged<TValue>(sender, propertyName, kind, oldValue, newValue);
        }

        public virtual void NotifyPropertyChanged<TValue>(
            object sender, string propertyName, PropertyChangeKind kind, TValue oldValue, TValue newValue, int index
        ) {
            base.OnPropertyChanged<TValue>(sender, propertyName, kind, oldValue, newValue, index);
        }


        // --- no indexed ---
        public virtual void NotifyPropertySet<TValue>(object sender, string propertyName, TValue oldValue, TValue newValue) {
            NotifyPropertyChanged<TValue>(sender, propertyName, PropertyChangeKind.Set, oldValue, newValue);
        }

        public virtual void NotifyPropertyUnset<TValue>(object sender, string propertyName, TValue oldValue) {
            NotifyPropertyChanged<TValue>(sender, propertyName, PropertyChangeKind.Unset, oldValue, default(TValue));
        }

        public virtual void NotifyPropertyCleared<TValue>(object sender, string propertyName, TValue oldValue) {
            NotifyPropertyChanged<TValue>(sender, propertyName, PropertyChangeKind.Clear, oldValue, default(TValue));
        }

        // --- indexed ---
        public virtual void NotifyPropertySet<TValue>(object sender, string propertyName, TValue oldValue, TValue newValue, int index) {
            NotifyPropertyChanged<TValue>(sender, propertyName, PropertyChangeKind.Set, oldValue, newValue, index);
        }

        public virtual void NotifyPropertyUnset<TValue>(object sender, string propertyName, TValue oldValue, int index) {
            NotifyPropertyChanged<TValue>(sender, propertyName, PropertyChangeKind.Unset, oldValue, default(TValue), index);
        }

        public virtual void NotifyPropertyAdded<TValue>(object sender, string propertyName, TValue newValue, int index) {
            NotifyPropertyChanged<TValue>(sender, propertyName, PropertyChangeKind.Add, default(TValue), newValue, index);
        }

        //public virtual void NotifyPropertyAddedMany<TValue>(object sender, string propertyName, TValue newValue, int index) {
        //    NotifyPropertyChanged<TValue>(sender, propertyName, PropertyChangeKind.AddMany, default(TValue), newValue, index);
        //}

        public virtual void NotifyPropertyRemoved<TValue>(object sender, string propertyName, TValue oldValue, int index) {
            NotifyPropertyChanged<TValue>(sender, propertyName, PropertyChangeKind.Remove, oldValue, default(TValue), index);
        }

        //public virtual void NotifyPropertyRemovedMany<TValue>(object sender, string propertyName, TValue oldValue, int index) {
        //    NotifyPropertyChanged<TValue>(sender, propertyName, PropertyChangeKind.RemoveMany, oldValue, default(TValue), index);
        //}

        // === PropertyChangeNotifierBase ==========
        public virtual void NotifyPropertyChanging(object sender, DetailedPropertyChangingEventArgs e) {
            base.OnPropertyChanging(sender, e);
        }

        public virtual void NotifyPropertyChanging<TValue>(
            object sender, string propertyName, PropertyChangeKind kind, TValue oldValue, TValue newValue
        ) {
            base.OnPropertyChanging<TValue>(sender, propertyName, kind, oldValue, newValue);
        }

        public virtual void NotifyPropertyChanging<TValue>(
            object sender, string propertyName, PropertyChangeKind kind, TValue oldValue, TValue newValue, int index
        ) {
            base.OnPropertyChanging<TValue>(sender, propertyName, kind, oldValue, newValue, index);
        }

        // --- no indexed ---
        public virtual void NotifyPropertySetting<TValue>(object sender, string propertyName, TValue oldValue, TValue newValue) {
            NotifyPropertyChanging<TValue>(sender, propertyName, PropertyChangeKind.Set, oldValue, newValue);
        }

        public virtual void NotifyPropertyUnsetting<TValue>(object sender, string propertyName, TValue oldValue) {
            NotifyPropertyChanging<TValue>(sender, propertyName, PropertyChangeKind.Unset, oldValue, default(TValue));
        }

        public virtual void NotifyPropertyClearing<TValue>(object sender, string propertyName, TValue oldValue) {
            NotifyPropertyChanging<TValue>(sender, propertyName, PropertyChangeKind.Clear, oldValue, default(TValue));
        }

        // --- indexed ---
        public virtual void NotifyPropertySetting<TValue>(
            object sender, string propertyName, TValue oldValue, TValue newValue, int index
        ) {
            NotifyPropertyChanging<TValue>(sender, propertyName, PropertyChangeKind.Set, oldValue, newValue, index);
        }

        public virtual void NotifyPropertyUnsetting<TValue>(object sender, string propertyName, TValue oldValue, int index) {
            NotifyPropertyChanging<TValue>(sender, propertyName, PropertyChangeKind.Unset, oldValue, default(TValue), index);
        }

        public virtual void NotifyPropertyAdding<TValue>(object sender, string propertyName, TValue newValue, int index) {
            NotifyPropertyChanging<TValue>(sender, propertyName, PropertyChangeKind.Add, default(TValue), newValue, index);
        }

        //public virtual void NotifyPropertyAddingMany<TValue>(object sender, string propertyName, TValue newValue, int index) {
        //    NotifyPropertyChanging<TValue>(sender, propertyName, PropertyChangeKind.AddMany, default(TValue), newValue, index);
        //}

        public virtual void NotifyPropertyRemoving<TValue>(object sender, string propertyName, TValue oldValue, int index) {
            NotifyPropertyChanging<TValue>(sender, propertyName, PropertyChangeKind.Remove, oldValue, default(TValue), index);
        }

        //public virtual void NotifyPropertyRemovingMany<TValue>(object sender, string propertyName, TValue oldValue, int index) {
        //    NotifyPropertyChanging<TValue>(sender, propertyName, PropertyChangeKind.RemoveMany, oldValue, default(TValue), index);
        //}

    }
}
