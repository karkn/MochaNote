/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Mkamo.Common.Reflection {
    public class AccessorCache {
        // ========================================
        // field
        // ========================================
        private Dictionary<PropertyInfo, IAccessor> _propToAccessor;

        // ========================================
        // constructor
        // ========================================
        public AccessorCache() {
            _propToAccessor = new Dictionary<PropertyInfo, IAccessor>();
        }

        // ========================================
        // method
        // ========================================
        public object GetValue(object target, PropertyInfo prop) {
            IAccessor acc;
            if (!_propToAccessor.TryGetValue(prop, out acc)) {
                acc = Mkamo.Common.Reflection.PropertyInfoUtil.ToAccessor(prop);
                _propToAccessor[prop] = acc;
            }
            return acc.GetValue(target);
        }

        public void SetValue(object target, PropertyInfo prop, object value) {
            IAccessor acc;
            if (!_propToAccessor.TryGetValue(prop, out acc)) {
                acc = Mkamo.Common.Reflection.PropertyInfoUtil.ToAccessor(prop);
                _propToAccessor[prop] = acc;
            }
            acc.SetValue(target, value);
        }

    }
}
