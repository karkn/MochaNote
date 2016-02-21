/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Reflection;
using Mkamo.Common.Externalize;
using System.Reflection;
using Mkamo.Common.DataType;
using Mkamo.Common.Core;

namespace Mkamo.Common.Externalize.Internal {
    internal class TypeService {
        // ========================================
        // static field
        // ========================================
        internal static readonly TypeService Instance = new TypeService();

        // ========================================
        // field
        // ========================================
        private AttributeCache _attrCache = new AttributeCache();

        private Dictionary<string, Type> _typeCache = new Dictionary<string, Type>();
        private Dictionary<string, MethodInfo> _methodCache = new Dictionary<string, MethodInfo>();

        private ObjectActivatorCache<object> _activatorCache = new ObjectActivatorCache<object>();

        // ========================================
        // constructor
        // ========================================
        public TypeService() {
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public bool IsExternalizableDefined(Type type) {
            return _attrCache.IsDefined<ExternalizableAttribute>(type);
        }

        public bool IsExternalDefined(PropertyInfo prop) {
            return _attrCache.IsDefined<ExternalAttribute>(prop);
        }

        public ExternalizableAttribute GetExternalizableAttribute(Type type) {
            return _attrCache.GetCustomAttribute<ExternalizableAttribute>(type);
        }

        public ExternalAttribute GetExternalAttribute(PropertyInfo prop) {
            return _attrCache.GetCustomAttribute<ExternalAttribute>(prop);
        }

        public Type GetType(string typeName, string assemName) {
            var s = typeName + "#" + assemName;
            var ret = default(Type);
            if (!_typeCache.TryGetValue(s, out ret)) {
                var name = new AssemblyName(assemName);
                var assem = default(Assembly);
                try {
                    assem = Assembly.Load(name.Name);
                } catch {
                    assem = Assembly.Load(name);
                }
                ret = assem.GetType(typeName);
                _typeCache[s] = ret;
            }
            return ret;
        }

        public MethodInfo GetMethod(string methodName, string typeName, string assemName) {
            var s = methodName + "#" + typeName + "#" + assemName;
            var ret = default(MethodInfo);
            if (!_methodCache.TryGetValue(s, out ret)) {
                var type = GetType(typeName, assemName);
                ret = type.GetMethod(methodName);
                _methodCache[s] = ret;
            }
            return ret;
        }

        public ObjectActivator<object> GetDefaultActivator(Type type) {
            return _activatorCache.GetDefaultActivator(type);
        }
    }
}
