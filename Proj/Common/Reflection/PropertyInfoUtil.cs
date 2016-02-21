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
    public static class PropertyInfoUtil {
        public static IAccessor ToAccessor(this PropertyInfo pi) {
            var getterDelegateType = typeof(Func<,>).MakeGenericType(pi.DeclaringType, pi.PropertyType);
            var getter = pi.GetGetMethod();
            var getterDelegate = getter == null ? null : Delegate.CreateDelegate(getterDelegateType, getter);

            var setterDelegateType = typeof(Action<,>).MakeGenericType(pi.DeclaringType, pi.PropertyType);
            var setter = pi.GetSetMethod();
            var setterDelegate = setter == null ? null : Delegate.CreateDelegate(setterDelegateType, setter);

            var accessorType = typeof(Accessor<,>).MakeGenericType(pi.DeclaringType, pi.PropertyType);
            var accessor = (IAccessor) Activator.CreateInstance(accessorType, getterDelegate, setterDelegate);

            return accessor;
        }
    }
}
