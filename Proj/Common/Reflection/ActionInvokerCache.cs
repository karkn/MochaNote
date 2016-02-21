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
    public class ActionInvokerCache {
        private Dictionary<MethodInfo, IActionInvoker> _methodToActionInvoker;

        public ActionInvokerCache() {
            _methodToActionInvoker = new Dictionary<MethodInfo, IActionInvoker>();
        }

        public void Invoke(object target, MethodInfo method, object arg) {
            IActionInvoker invoker;
            if (!_methodToActionInvoker.TryGetValue(method, out invoker)) {
                invoker = MethodInfoUtil.ToActionInvoker(method);
                _methodToActionInvoker[method] = invoker;
            }
            invoker.Invoke(target, arg);
        }
    }
}
