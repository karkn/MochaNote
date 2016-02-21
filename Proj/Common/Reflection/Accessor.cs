/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Common.Reflection {
    internal sealed class Accessor<TTarget, TProperty>: IAccessor {
        // ========================================
        // field
        // ========================================
        private readonly Func<TTarget, TProperty> _getter;
        private readonly Action<TTarget, TProperty> _setter;

        // ========================================
        // constructor
        // ========================================
        public Accessor(Func<TTarget, TProperty> getter, Action<TTarget, TProperty> setter) {
            _getter = getter;
            _setter = setter;
        }

        // ========================================
        // method
        // ========================================
        public object GetValue(object target) {
            return _getter((TTarget) target); ;
        }

        public void SetValue(object target, object value) {
            _setter((TTarget) target, (TProperty) value);
        }
    }
}
