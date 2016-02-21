/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Mkamo.Common.Core {
    public class ObjectActivatorCache<T> {
        // ========================================
        // static field
        // ========================================
        private Type[] EmptyTypeArray = new Type[0];

        // ========================================
        // field
        // ========================================
        private Dictionary<ConstructorInfo, ObjectActivator<T>> _ctorToActivators = new Dictionary<ConstructorInfo, ObjectActivator<T>>();
        private Dictionary<Type, ObjectActivator<T>> _typeToDefaultActivator = new Dictionary<Type, ObjectActivator<T>>();

        // ========================================
        // constructor
        // ========================================

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public ObjectActivator<T> GetActivator(ConstructorInfo ctor) {
            var ret = default(ObjectActivator<T>);

            if (!_ctorToActivators.TryGetValue(ctor, out ret)) {
                ret = ExpressionUtil.GetActivator<T>(ctor);
                _ctorToActivators.Add(ctor, ret);
            }

            return ret;
        }

        public ObjectActivator<T> GetDefaultActivator(Type type) {
            var ret = default(ObjectActivator<T>);

            if (!_typeToDefaultActivator.TryGetValue(type, out ret)) {
                var ctor = type.GetConstructor(EmptyTypeArray);
                ret = GetActivator(ctor);
                _typeToDefaultActivator.Add(type, ret);
            }

            return ret;
        }
    }
}
