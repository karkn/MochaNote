/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;

namespace Mkamo.Editor.Core {
    public interface IModelFactory {
        // ========================================
        // property
        // ========================================
        IModelDescriptor ModelDescriptor { get; }

        // ========================================
        // method
        // ========================================
        object CreateModel();
    }

    public class DefaultModelFactory<T>: IModelFactory where T: new() {
        // ========================================
        // field
        // ========================================
        private Action<T> _initializer;

        // ========================================
        // constructor
        // ========================================
        public DefaultModelFactory(): this(null) {
        }

        public DefaultModelFactory(Action<T> initializer) {
            _initializer = initializer;
        }

        // ========================================
        // property
        // ========================================
        public IModelDescriptor ModelDescriptor {
            get { return new DefaultModelDescriptor(typeof(T)); }
        }

        // ========================================
        // method
        // ========================================
        public object CreateModel() {
            var ret = new T();
            if (_initializer != null) {
                _initializer(ret);
            }
            return ret;
        }

    }

    public class DelegatingModelFactory<T>: IModelFactory {
        // ========================================
        // field
        // ========================================
        private Func<T> _constructor;

        // ========================================
        // constructor
        // ========================================
        public DelegatingModelFactory(): this(null) {
        }

        public DelegatingModelFactory(Func<T> constructor) {
            _constructor = constructor;
        }

        // ========================================
        // property
        // ========================================
        public IModelDescriptor ModelDescriptor {
            get { return new DefaultModelDescriptor(typeof(T)); }
        }

        // ========================================
        // method
        // ========================================
        public object CreateModel() {
            return _constructor();
        }

    }
}
