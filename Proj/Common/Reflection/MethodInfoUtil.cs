/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Mkamo.Common.Reflection {
    public static class MethodInfoUtil {
        private static readonly string SetterPrefix = "set_";
        private static readonly int SetterPrefixLength = 4;
        private static readonly string GetterPrefix = "get_";
        private static readonly int GetterPrefixLength = 4;

        public static bool IsPropertySetter(MethodInfo method) {
            return method.IsSpecialName && method.Name.StartsWith(SetterPrefix);
        }

        public static bool IsPropertyGetter(MethodInfo method) {
            return method.IsSpecialName && method.Name.StartsWith(GetterPrefix);
        }

        public static PropertyInfo GetPropertyForAccessor(MethodInfo accessorMethod) {
            if (IsPropertyGetter(accessorMethod)) {
                return GetPropertyForGetter(accessorMethod);
            } else if (IsPropertySetter(accessorMethod)) {
                return GetPropertyForSetter(accessorMethod);
            } else {
                return null;
            }
        }

        public static PropertyInfo GetPropertyForGetter(MethodInfo getterMethod) {
            if (getterMethod == null) {
                return null;
            }
            Type type = getterMethod.DeclaringType;
            return type.GetProperty(
                getterMethod.Name.Substring(GetterPrefixLength),
                GetPropertyParamTypesForGetter(getterMethod)
            ); 
        }

        public static PropertyInfo GetPropertyForSetter(MethodInfo setterMethod) {
            if (setterMethod == null) {
                return null;
            }
            Type type = setterMethod.DeclaringType;
            return type.GetProperty(
                setterMethod.Name.Substring(SetterPrefixLength),
                GetPropertyParamTypesForSetter(setterMethod)
            ); 
        }

        public static Type[] GetPropertyParamTypesForGetter(MethodInfo getterMethod) {
            ParameterInfo[] paramInfos = getterMethod.GetParameters();
            Type[] paramTypes = new Type[paramInfos.Length];
            for (int i = 0, len = paramInfos.Length; i < len; ++i) {
                paramTypes[i] = paramInfos[i].ParameterType;
            }
            return paramTypes;
        }

        public static Type[] GetPropertyParamTypesForSetter(MethodInfo setterMethod) {
            ParameterInfo[] paramInfos = setterMethod.GetParameters();
            // setterメソッドの最後のパラメタはvalueなので捨てる
            Type[] paramTypes = new Type[paramInfos.Length - 1];
            for (int i = 0, len = paramInfos.Length; i < len - 1; ++i) {
                paramTypes[i] = paramInfos[i].ParameterType;
            }
            return paramTypes;
        }

        public static bool IsICollectionPropertyGetter(MethodInfo method) {
            return IsPropertyGetter(method) && GenericTypeUtil.IsGenericICollection(method.ReturnType);
        }

        public static IActionInvoker ToActionInvoker(this MethodInfo method) {
            var argType = method.GetParameters()[0].ParameterType;;

            var methodDelegateType = typeof(Action<,>).MakeGenericType(method.DeclaringType, argType);
            var methodDelegate = Delegate.CreateDelegate(methodDelegateType, method);

            var invokerType = typeof(ActionInvoker<,>).MakeGenericType(method.DeclaringType, argType);
            var invoker = (IActionInvoker) Activator.CreateInstance(invokerType, methodDelegate);

            return invoker;
        }

        public static Action<T> ToStaticAction<T>(this MethodInfo method) {
            var methodDelegate = Delegate.CreateDelegate(typeof(Action<T>), method);
            return (Action<T>) methodDelegate;
        }
    }
}
