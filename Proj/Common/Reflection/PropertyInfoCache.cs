/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Mkamo.Common.Reflection {
    public class PropertyInfoCache {
        // ========================================
        // field
        // ========================================
        private Dictionary<MethodInfo, PropertyInfo> _methodToPropMap; /// lazy

        // ========================================
        // constructor
        // ========================================
        public PropertyInfoCache() {
        }

        // ========================================
        // property
        // ========================================
        protected Dictionary<MethodInfo, PropertyInfo> _MethodToPropMap {
            get {
                if (_methodToPropMap == null) {
                    _methodToPropMap = new Dictionary<MethodInfo,PropertyInfo>();
                }
                return _methodToPropMap;
            }
        }

        // ========================================
        // method
        // ========================================
        public PropertyInfo GetPropertyInfo(MethodInfo method) {
            PropertyInfo ret;

            if (!_MethodToPropMap.TryGetValue(method, out ret)) {
                if (MethodInfoUtil.IsPropertyGetter(method)) {
                    ret = MethodInfoUtil.GetPropertyForGetter(method);
                    _MethodToPropMap.Add(method, ret);
                } else if (MethodInfoUtil.IsPropertySetter(method)) {
                    ret = MethodInfoUtil.GetPropertyForSetter(method);
                    _MethodToPropMap.Add(method, ret);
                } else {
                    _MethodToPropMap.Add(method, null);
                }
            }

            return ret;
        }

    }
}
