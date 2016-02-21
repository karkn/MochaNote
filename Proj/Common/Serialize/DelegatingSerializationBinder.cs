/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Reflection;

namespace Mkamo.Common.Serialize {
    public class DelegatingSerializationBinder: SerializationBinder {
        private Func<string, string, Type> _binder;

        public DelegatingSerializationBinder(Func<string, string, Type> binder) {
            _binder = binder;
        }

        public override Type BindToType(string assemblyName, string typeName) {
            return _binder(assemblyName, typeName);
        }
    }
}
