/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Diagnostics;

namespace Mkamo.Common.Core {
    public class StringProvider<T> {
        private T _object;
        private Func<T, string> _toString;

        public StringProvider(T obj, Func<T, string> toString) {
            Contract.Requires(obj != null);
            Contract.Requires(toString != null);

            _object = obj;
            _toString = toString;
        }

        // ========================================
        // property
        // ========================================
        public T Object {
            get { return _object; }
        }

        // ========================================
        // method
        // ========================================
        public override string ToString() {
            return _toString(_object);
        }
    }
}
