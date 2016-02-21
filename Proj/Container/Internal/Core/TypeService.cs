/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Reflection;
using Mkamo.Container.Core;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using Castle.DynamicProxy;
using Mkamo.Common.Core;

namespace Mkamo.Container.Internal.Core {
    internal class TypeService {
        // ========================================
        // static field
        // ========================================
        internal static readonly TypeService Instance = new TypeService();

        // ========================================
        // field
        // ========================================
        private AttributeCache _attrCache;
        private PropertyInfoCache _propInfoCache;
        private AccessorCache _accessorCache;
        private ActionInvokerCache _actionInvokerCache;

        private Dictionary<string, Type> _nameToType;

        private Dictionary<Type, bool> _stringConvertiblities;

        private Dictionary<string, MethodInfo> _addMethodNameToMethod;
        private Dictionary<string, PropertyInfo> _nameToPropInfo;
        private Dictionary<Type, PropertyInfo[]> _typeToPublicInstancePropInfo;

        private ObjectActivatorCache<object> _activatorCache = new ObjectActivatorCache<object>();

        // ========================================
        // constructor
        // ========================================
        private TypeService() {
            _attrCache = new AttributeCache();
            _propInfoCache = new PropertyInfoCache();
            _accessorCache = new AccessorCache();
            _actionInvokerCache = new ActionInvokerCache();

            _nameToType = new Dictionary<string, Type>();
            _stringConvertiblities = new Dictionary<Type, bool>();
            _addMethodNameToMethod = new Dictionary<string, MethodInfo>();
            _nameToPropInfo = new Dictionary<string, PropertyInfo>();
            _typeToPublicInstancePropInfo = new Dictionary<Type, PropertyInfo[]>();
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        // --- attribute ---
        public bool IsEntityDefined(Type type) {
            return _attrCache.IsDefined(type, typeof(EntityAttribute));
        }

        public bool IsPersistTarget(Type type) {
            var entityAttr = _attrCache.GetCustomAttribute<EntityAttribute>(type);
            return entityAttr == null? false: entityAttr.Persist;
        }

        public bool IsSerializableDefined(Type type) {
            return _attrCache.IsDefined(type, typeof(SerializableAttribute));
        }

        public bool IsDirtyDefined(MethodInfo method) {
            return _attrCache.IsDefined(method, typeof(DirtyAttribute));
        }

        public EntityAttribute GetEntityAttribute(Type type) {
            return _attrCache.GetCustomAttribute<EntityAttribute>(type);
        }

        public PersistAttribute GetPersistAttribute(PropertyInfo prop) {
            return _attrCache.GetCustomAttribute<PersistAttribute>(prop);
        }

        // --- method ---
        public void InvokeAction(object target, MethodInfo method, object arg) {
            _actionInvokerCache.Invoke(target, method, arg);
        }

        public MethodInfo GetAddMethod(Type type, string methodName, Type elemType) {
            var s = type.FullName + "#" + methodName + "#" + elemType.FullName;
            if (!_addMethodNameToMethod.ContainsKey(s)) {
                var ret = type.GetMethod(methodName, new Type[] { elemType });
                _addMethodNameToMethod[s] = ret;
                return ret;
            }
            return _addMethodNameToMethod[s];
        }

        public MethodInfo GetOnLoading(Type type) {
            var attr = GetEntityAttribute(type);
            if (attr == null || attr.OnLoading == null) {
                return null;
            }

            var ret = type.GetMethod(attr.OnLoading, Type.EmptyTypes);
            return ret;
        }

        public MethodInfo GetOnLoaded(Type type) {
            var attr = GetEntityAttribute(type);
            if (attr == null || attr.OnLoaded == null) {
                return null;
            }

            var ret = type.GetMethod(attr.OnLoaded, Type.EmptyTypes);
            return ret;
        }

        // --- property ---
        public object GetValue(object target, PropertyInfo prop) {
            return _accessorCache.GetValue(target, prop);
        }

        public void SetValue(object target, PropertyInfo prop, object value) {
            _accessorCache.SetValue(target, prop, value);
        }

        public bool HasPublicGetter(PropertyInfo prop) {
            return prop.GetGetMethod() != null && prop.GetGetMethod().IsPublic;
        }

        public bool HasPublicSetter(PropertyInfo prop) {
            return prop.GetSetMethod() != null && prop.GetSetMethod().IsPublic;
        }

        public PropertyInfo GetPropertyInfo(MethodInfo method) {
            return _propInfoCache.GetPropertyInfo(method);
        }

        public PropertyInfo GetPublicInstanceProperty(Type type, string propName) {
            var s = type.FullName + "#" + propName;
            if (!_nameToPropInfo.ContainsKey(s)) {
                var ret = type.GetProperty(propName, BindingFlags.Instance | BindingFlags.Public);
                _nameToPropInfo[s] = ret;
                return ret;
            }
            return _nameToPropInfo[s];
        }

        public PropertyInfo[] GetPublicInstanceProperties(Type type) {
            if (!_typeToPublicInstancePropInfo.ContainsKey(type)) {
                var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                _typeToPublicInstancePropInfo[type] = props;
                return props;
            }
            return _typeToPublicInstancePropInfo[type];
        }

        public IEnumerable<PropertyInfo> GetPersistProperties(Type type) {
            var props = GetPublicInstanceProperties(type);
            return props.Where(
                prop => {
                    var attr = GetPersistAttribute(prop);
                    return attr != null && attr.Enabled;
                }
            );
        }

        public IEnumerable<PropertyInfo> GetCascadingPersistProperties(Type type) {
            var props = GetPublicInstanceProperties(type);
            return props.Where(
                prop => {
                    var attr = GetPersistAttribute(prop);
                    return attr != null && attr.Enabled && attr.Cascade;
                }
            );
        }

        public IEnumerable<PropertyInfo> GetCompositePersistProperties(Type type) {
            var props = GetPublicInstanceProperties(type);
            return props.Where(
                prop => {
                    var attr = GetPersistAttribute(prop);
                    return attr != null && attr.Enabled && attr.Composite != null;
                }
            );
        }

        public Type GetTypeObject(string typeStr, string assemStr) {
            var name = typeStr + "," + assemStr;

            if (!_nameToType.ContainsKey(name)) {
                var assemName = new AssemblyName(assemStr);
                var assem = default(Assembly);
                try {
                    assem = Assembly.Load(assemName.Name);
                } catch {
                    assem = Assembly.Load(assemName);
                }
                return _nameToType[name] = assem.GetType(typeStr);
            }

            return _nameToType[name];
        }

        public IEntity AsEntity(object entity) {
            if (entity == null) {
                return null;
            }
            var accessor = entity as IProxyTargetAccessor;
            if (accessor == null) {
                return null;
            }
            return accessor.GetInterceptors().FirstOrDefault(i => i is IEntity) as IEntity;
        }

        public Type GetRealType(object obj) {
            var entity = AsEntity(obj);
            return entity == null? obj.GetType(): entity.Type;
        }

        public bool HasStringInterconvertibility(Type type) {
            bool ret;
            if (_stringConvertiblities.TryGetValue(type, out ret)) {
                return ret;
            }

            var converter = TypeDescriptor.GetConverter(type);
            if (converter == null) {
                ret = false;
            } else {
                ret = converter.CanConvertFrom(typeof(string)) && converter.CanConvertTo(typeof(string));
            }
            _stringConvertiblities[type] = ret;
            return ret;
        }

        public ObjectActivator<object> GetDefaultActivator(Type type) {
            return _activatorCache.GetDefaultActivator(type);
        }
    }
}
