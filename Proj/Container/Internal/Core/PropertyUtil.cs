/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Reflection;
using System.Collections;
using System.Reflection;

namespace Mkamo.Container.Internal.Core {
    internal static class PropertyUtil {
        // ========================================
        // static field
        // ========================================
        public static void ProcessProperty(
            object obj,
            PropertyInfo prop,
            Action<PropertyKind, object> pre,
            Action<PropertyKind, object> post,
            Action<PropertyKind, object, object> each
        ) {
            var propValue = prop.GetValue(obj, null);
            if (propValue != null) {
                var propType = TypeService.Instance.GetRealType(propValue);
                var persistAttr = TypeService.Instance.GetPersistAttribute(prop);

                if (propType.IsArray) {
                    if (HasPublicGetter(prop) && (HasPublicSetter(prop) || persistAttr.Add != null)) {
                        ProcessArrayProperty(propValue, propType, pre, post, each);
                    }

                } else if (GenericTypeUtil.IsGenericIDictionary(propType)) {
                    if (HasPublicGetter(prop)) {
                        ProcessDictionaryProperty(propValue, propType, pre, post, each);
                    }

                } else if (GenericTypeUtil.IsGenericICollection(propType)) {
                    if (HasPublicGetter(prop)) {
                        ProcessEnumerableProperty(propValue, propType, pre, post, each);
                    }

                } else if (GenericTypeUtil.IsGenericIEnumerable(propType) && persistAttr.Add != null) {
                    if (HasPublicGetter(prop)) {
                        ProcessEnumerableProperty(propValue, propType, pre, post, each);
                    }

                } else {
                    if (HasPublicGetter(prop) && HasPublicSetter(prop)) {
                        if (pre != null) {
                            pre(PropertyKind.Single, propValue);
                        }
                        if (each != null) {
                            each(PropertyKind.Single, propValue, null);
                        }
                        if (post != null) {
                            post(PropertyKind.Single, propValue);
                        }
                    }
                }
            }
        }

        // ------------------------------
        // private
        // ------------------------------
        private static void ProcessArrayProperty(
            object propValue,
            Type propType,
            Action<PropertyKind, object> pre,
            Action<PropertyKind, object> post,
            Action<PropertyKind, object, object> each
        ) {
            if (pre != null) {
                pre(PropertyKind.Array, propValue);
            }

            if (each != null) {
                var arrayPropValue = propValue as Array;
                for (int i = 0; i < arrayPropValue.Length; ++i) {
                    each(PropertyKind.Array, arrayPropValue.GetValue(i), null);
                }
            }

            if (post != null) {
                post(PropertyKind.Array, propValue);
            }
        }

        private static void ProcessDictionaryProperty(
            object propValue,
            Type propType,
            Action<PropertyKind, object> pre,
            Action<PropertyKind, object> post,
            Action<PropertyKind, object, object> each
        ) {
            if (pre != null) {
                pre(PropertyKind.Dictionary, propValue);
            }

            if (each != null) {
                var elemTypes = GenericTypeUtil.GetGenericArgumentOfGenericIDictionary(propType);
                var dictType = typeof(IDictionary<,>).MakeGenericType(elemTypes);
                var getKeysMethod = dictType.GetMethod("get_Keys");
                var getItemMethod = dictType.GetMethod("get_Item");

                var keys = getKeysMethod.Invoke(propValue, null) as ICollection;
                foreach (var key in keys) {
                    var value = getItemMethod.Invoke(propValue, new object[] { key });
                    each(PropertyKind.Dictionary, value, key);
                }
            }

            if (post != null) {
                post(PropertyKind.Dictionary, propValue);
            }
        }

        private static void ProcessEnumerableProperty(
            object propValue,
            Type propType,
            Action<PropertyKind, object> pre,
            Action<PropertyKind, object> post,
            Action<PropertyKind, object, object> each
        ) {
            if (pre != null) {
                pre(PropertyKind.Enumerable, propValue);
            }

            if (each != null) {
                var elemType = GenericTypeUtil.GetGenericArgumentOfGenericIEnumerable(propType);
                var getEnumeratorMethod = typeof(IEnumerable<>).MakeGenericType(elemType).GetMethod("GetEnumerator");
                var enumerator = getEnumeratorMethod.Invoke(propValue, null) as IEnumerator;
                while (enumerator.MoveNext()) {
                    var value = enumerator.Current;
                    each(PropertyKind.Enumerable, value, null);
                }
            }

            if (post != null) {
                post(PropertyKind.Enumerable, propValue);
            }
        }

        private static bool HasPublicGetter(PropertyInfo prop) {
            return TypeService.Instance.HasPublicGetter(prop);
        }

        private static bool HasPublicSetter(PropertyInfo prop) {
            return TypeService.Instance.HasPublicSetter(prop);
        }


        // ========================================
        // type
        // ========================================
        [Serializable]
        public enum PropertyKind {
            Single,
            Array,
            Dictionary,
            Enumerable,
        }

    }
}
