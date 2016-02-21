/*
 * Copyright (c) 2010, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Mkamo.Common.Core {
    [Serializable]
    public class WeakReference<T>: WeakReference {
        public WeakReference(T target): base(target) {
        }

        public WeakReference(T target, bool trackResurrection): base(target, trackResurrection) {
        }

        protected WeakReference(SerializationInfo info, StreamingContext context): base(info, context) {
        }

        public new T Target {
            get { return (T) base.Target; }
        }
    }
}
