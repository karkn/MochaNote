/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Common.Reflection {
    public interface IActionInvoker {
        void Invoke(object target, object arg);
    }


    internal class ActionInvoker<TTarget, TArg>: IActionInvoker {
        private Action<TTarget, TArg> _action;

        public ActionInvoker(Action<TTarget, TArg> action) {
            _action = action;
        }

        public void Invoke(object target, object arg) {
            _action((TTarget) target, (TArg) arg);
        }
    }


    //public interface IStaticActionInvoker {
    //    void Invoke(object arg);
    //}

    internal class StaticActionInvoker<TArg> {//: IStaticActionInvoker {
        private Action<TArg> _action;

        public StaticActionInvoker(Action<TArg> action) {
            _action = action;
        }

        public void Invoke(TArg arg) {
            _action((TArg) arg);
        }
    }
}
