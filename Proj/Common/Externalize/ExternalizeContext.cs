/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Externalize.Internal;
using System.Reflection;
using Mkamo.Common.Diagnostics;
using Mkamo.Common.Core;

namespace Mkamo.Common.Externalize {
    public class ExternalizeContext {
        // ========================================
        // field
        // ========================================
        private Externalizer _owner;

        private Dictionary<object, IMemento> _externalizableToMemento; /// lazy
        private Dictionary<IMemento, object> _mementoToExternalizable; /// lazy

        private ExternalizableFilter _externalizableFilter;
        private MementoFilter _mementoFilter;

        // ========================================
        // constructor
        // ========================================
        internal ExternalizeContext(
            Externalizer owner,
            ExternalizableFilter externalizableFilter,
            MementoFilter mementoFilter
        ) {
            _owner = owner;
            _externalizableFilter = externalizableFilter;
            _mementoFilter = mementoFilter;
        }

        // ========================================
        // property
        // ========================================
        public Dictionary<string, object> ExtendedData {
            get { return _owner.ExtendedData; }
        }

        // ------------------------------
        // protected
        // ------------------------------
        internal Dictionary<object, IMemento> _ExternalizableToMemento {
            get {
                if (_externalizableToMemento == null) {
                    _externalizableToMemento = new Dictionary<object, IMemento>();
                }
                return _externalizableToMemento;
            }
        }

        internal Dictionary<IMemento, object> _MementoToExternalizable {
            get {
                if (_mementoToExternalizable== null) {
                    _mementoToExternalizable = new Dictionary<IMemento, object>();
                }
                return _mementoToExternalizable;
            }
        }

        // ========================================
        // method
        // ========================================
        /// <summary>
        /// externalizableのmementoを取得する．
        /// </summary>
        public IMemento GetMemento(string key, object externalizable) {
            if (externalizable == null) {
                return null;
            }

            if (!_ExternalizableToMemento.ContainsKey(externalizable)) {
                if (_externalizableFilter != null && !_externalizableFilter(key, externalizable)) {
                    return null;
                }
                return CreateMemento(externalizable);
            }
            return _ExternalizableToMemento[externalizable];
        }

        /// <summary>
        /// mementoからexternalizableを取得する．
        /// </summary>
        public object GetExternalizable(string key, IMemento memento) {
            if (memento == null) {
                return null;
            }

            if (!_MementoToExternalizable.ContainsKey(memento)) {
                if (_mementoFilter != null && !_mementoFilter(key, memento)) {
                    return null;
                }
                return CreateExternalizable((Memento) memento);
            }
            return _MementoToExternalizable[memento];
        }

        // ------------------------------
        // internal
        // ------------------------------
        internal Memento CreateMemento(object externalizable) {
            var service = TypeService.Instance;

            var type = externalizable.GetType();
            if (service.IsExternalizableDefined(type)) {
                var externalizableAttr = service.GetExternalizableAttribute(type);
                if (externalizableAttr.Type != null) {
                    type = externalizableAttr.Type;
                }
            }

            var ret = new Memento(type);
            ret.Context = this;
            _ExternalizableToMemento[externalizable] = ret;

            if (externalizable is IExternalizable) {
                ((IExternalizable) externalizable).WriteExternal(ret, this);
            } else if (service.IsExternalizableDefined(type)) {
                new PropertyExternalizableSupport(externalizable).SaveTo(ret, this);
            } else {
                throw new ArgumentException("externalizable");
            }

            return ret;
        }

        internal object CreateExternalizable(Memento memento) {
            memento.Context = this;

            var service = TypeService.Instance;

            var ret = default(object);
            var type = service.GetType(memento.TypeName, memento.AssemblyName);

            if (memento.FactoryMethodName != null && memento.FactoryMethodParamKeys != null) {
                /// factory methodで生成
                var paramKeys = memento.FactoryMethodParamKeys;
                var paras = new object[paramKeys.Length];
                for (int i = 0, len = paramKeys.Length; i < len; ++i) {
                    paras[i] = memento.Values[paramKeys[i]];
                }

                var factMethod = service.GetMethod(
                    memento.FactoryMethodName,
                    memento.FactoryTypeName,
                    memento.FactoryAssemblyName
                );

                ret = factMethod.Invoke(null, paras);

            } else if (memento.ConstructorParamKeys != null && memento.ConstructorParamKeys.Length > 0) {
                /// コンストラクタで生成
                var paramKeys = memento.ConstructorParamKeys;
                var paras = new object[paramKeys.Length];
                for (int i = 0, len = paramKeys.Length; i < len; ++i) {
                    paras[i] = memento.Values[paramKeys[i]];
                }
                ret = Activator.CreateInstance(type, paras);

            } else {
                /// デフォルトコンストラクタで生成
                //ret = Activator.CreateInstance(type);
                
                var act = service.GetDefaultActivator(type);
                ret = act();
            }

            _MementoToExternalizable[memento] = ret;

            if (typeof(IExternalizable).IsAssignableFrom(type)) {
                ((IExternalizable) ret).ReadExternal(memento, this);
            } else if (TypeService.Instance.IsExternalizableDefined(type)) {
                new PropertyExternalizableSupport(ret).LoadFrom(memento, this);
            } else {
                throw new ArgumentException("type");
            }

            if (typeof(IReplacingExternalizable).IsAssignableFrom(type)) {
                return ((IReplacingExternalizable) ret).GetReplaced();
            } else {
                return ret;
            }
        }

        //internal void LoadExternalizable(Memento memento, object target) {
        //    memento.Context = this;

        //    var service = TypeService.Instance;

        //    var type = service.GetType(memento.TypeName, memento.AssemblyName);

        //    Contract.Requires(type.IsAssignableFrom(target.GetType()));

        //    _MementoToExternalizable[memento] = target;

        //    if (typeof(IExternalizable).IsAssignableFrom(type)) {
        //        ((IExternalizable) target).ReadExternal(memento, this);
        //    } else if (TypeService.Instance.IsExternalizableDefined(type)) {
        //        new PropertyExternalizableSupport(target).LoadFrom(memento, this);
        //    } else {
        //        throw new ArgumentException("type");
        //    }

        //    //if (typeof(IReplacingExternalizable).IsAssignableFrom(type)) {
        //    //    return ((IReplacingExternalizable) ret).GetReplaced();
        //    //} else {
        //    //    return ret;
        //    //}
        //}
    }
}
