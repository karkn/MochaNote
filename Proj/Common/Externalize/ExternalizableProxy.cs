/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Diagnostics;

namespace Mkamo.Common.Externalize {
    public class ExternalizableProxy<T>: IExternalizable {
        // ========================================
        // field
        // ========================================
        private Action<T, IMemento, ExternalizeContext> _saveTo;
        private Func<IMemento, ExternalizeContext, T> _loadFrom;

        private T _real;

        // ========================================
        // constructor
        // ========================================
        public ExternalizableProxy(
            Action<T, IMemento, ExternalizeContext> saveTo,
            Func<IMemento, ExternalizeContext, T> loadFrom
        )
            : this(default(T), saveTo, loadFrom) {
        }

        public ExternalizableProxy(
            T real,
            Action<T, IMemento, ExternalizeContext> saveTo,
            Func<IMemento, ExternalizeContext, T> loadFrom
        ) {
            Contract.Requires(saveTo != null);
            Contract.Requires(loadFrom != null);

            _real = real;
            _saveTo = saveTo;
            _loadFrom = loadFrom;
        }

        // ========================================
        // property
        // ========================================
        public T Real {
            get { return _real; }
        }

        // ========================================
        // method
        // ========================================
        public void WriteExternal(IMemento memento, ExternalizeContext context) {
            memento.SetConstructorParamKeys(new[] { "ExternalizableProxy.SaveTo", "ExternalizableProxy.LoadFrom" });
            memento.WriteSerializable("ExternalizableProxy.SaveTo", _saveTo);
            memento.WriteSerializable("ExternalizableProxy.LoadFrom", _loadFrom);

            _saveTo(_real, memento, context);
        }

        public void ReadExternal(IMemento memento, ExternalizeContext context) {
            _saveTo = memento.ReadSerializable("ExternalizableProxy.SaveTo") as Action<T, IMemento, ExternalizeContext>;
            _loadFrom = memento.ReadSerializable("ExternalizableProxy.LoadFrom") as Func<IMemento, ExternalizeContext, T>;

            _real = _loadFrom(memento, context);
        }
    }
}
