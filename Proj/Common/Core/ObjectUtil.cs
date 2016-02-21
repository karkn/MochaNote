/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Mkamo.Common.Core {
    public static class ObjectUtil {
        // ========================================
        // method
        // ========================================
        /// <summary>
        /// メンバ名を取得する．
        /// var sym = obj.GetSym(o => o.Foo); /// => "Foo"
        /// </summary>
        public static string Symbol<S, T>(this S obj, Expression<Func<S, T>> expr) {
            return ((MemberExpression) expr.Body).Member.Name;
        }

        /// <summary>
        /// Throws an ArgumentNullException if the given data item is null.
        /// </summary>
        public static void ThrowIfNull<T>(this T data, string name) where T: class {
            if (data == null) {
                throw new ArgumentNullException(name);
            }
        }

        /// <summary>
        /// Throws an ArgumentNullException if the given data item is null.
        /// No parameter name is specified.
        /// </summary>
        public static void ThrowIfNull<T>(this T data) where T: class {
            if (data == null) {
                throw new ArgumentNullException();
            }
        }
    }
}
