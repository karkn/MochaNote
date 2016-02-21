/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Common.Core {
    public class Lazy<T> {
        // ========================================
        // field
        // ========================================
        private Func<T> _factory;
        private T _value;
        private bool _isValueCreated = false;

        // ========================================
        // constructor
        // ========================================
        public Lazy(Func<T> factory) {
            _factory = factory;
        }

        // ========================================
        // event
        // ========================================
        public event EventHandler ValueCreated;
        public event EventHandler ValueCleared;

        // ========================================
        // property
        // ========================================
        public T Value {
            get {
                if (!_isValueCreated) {
                    _value = _factory();
                    _isValueCreated = true;
                    OnValueCreated();
                }
                return _value;
            }
        }

        public bool IsValueCreated {
            get { return _isValueCreated; }
        }

        // ========================================
        // method
        // ========================================
        public void ClearValue() {
            _value = default(T);
            _isValueCreated = false;
            OnValueCleared();
        }

        public override string ToString() {
            return Value.ToString();
        }

        protected virtual void OnValueCreated() {
            var handler = ValueCreated;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnValueCleared() {
            var handler = ValueCleared;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
