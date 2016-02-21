/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Mkamo.Common.Event {
    public abstract class DetailedPropertyChangedEventArgs: PropertyChangedEventArgs {
        // ========================================
        // field
        // ========================================
        private PropertyChangeKind _kind;
        private int _index;
        private bool _isIndexed;

        // ========================================
        // constructor
        // ========================================
        public DetailedPropertyChangedEventArgs(string propertyName, PropertyChangeKind kind): base(propertyName) {
            _kind = kind;
            _index = -1;
            _isIndexed = false;
        }

        public DetailedPropertyChangedEventArgs(string propertyName, PropertyChangeKind kind, int index): base(propertyName) {
            _kind = kind;
            _index = index;
            _isIndexed = true;
        }

        // ========================================
        // property
        // ========================================
        public PropertyChangeKind Kind {
            get { return _kind; }
        }
        public int Index {
            get { return _index; }
        }

        public abstract object OldValue { get; }
        public abstract object NewValue { get; }

        public bool IsIndexed {
            get { return _isIndexed; }
        }
    }

    /// <summary>
    /// TValueがstructのときに余計なboxingがおこらないようにする．
    /// </summary>
    public class DetailedPropertyChangedEventArgs<TValue>: DetailedPropertyChangedEventArgs {
        // ========================================
        // field
        // ========================================
        private TValue _oldValue;
        private TValue _newValue;

        // ========================================
        // constructor
        // ========================================
        public DetailedPropertyChangedEventArgs(
            string propertyName, PropertyChangeKind kind, TValue oldValue, TValue newValue
        ): base(propertyName, kind) {
            _oldValue = oldValue;
            _newValue = newValue;
        }
        public DetailedPropertyChangedEventArgs(
            string propertyName, PropertyChangeKind kind, TValue oldValue, TValue newValue, int index
        ): base(propertyName, kind, index) {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        // ========================================
        // property
        // ========================================
        public override object OldValue {
            get { return _oldValue; }
        }
        public override object NewValue {
            get { return _newValue; }
        }

        public TValue TypedOldValue {
            get { return _oldValue; }
        }
        public TValue TypedNewValue {
            get { return _newValue; }
        }
    }

}
