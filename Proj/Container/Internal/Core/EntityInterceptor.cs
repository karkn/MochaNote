/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Reflection;
using Mkamo.Common.Event;
using Mkamo.Common.Reflection;
using System.ComponentModel;
using Mkamo.Container.Core;
using Mkamo.Common.DataType;
using Castle.DynamicProxy;

namespace Mkamo.Container.Internal.Core {
    internal class EntityInterceptor: IInterceptor, IEntity {
        // ========================================
        // static field
        // ========================================
        private static readonly List<MethodInfo> ProceedOnlyMethods = new List<MethodInfo>();
        private static readonly List<MethodInfo> ProceedAndDirtyMethods = new List<MethodInfo>();

        private static readonly ReturnValueWrapperFactoryRegistry WrapperTypeRegistry = new ReturnValueWrapperFactoryRegistry();

        // ========================================
        // field
        // ========================================
        private object _target;
        private Type _type;
        private string _id;
        private EntityContainer _container;
        
        private PersistenceMaintainer _persistenceMaintainer;

        private Dictionary<PropertyInfo, object> _listPropToListWrapperMap; /// lazy
        private bool _needRealInvocation = false;

        private bool _isExtendedDataDirty = false;
        private Dictionary<string, string> _extendedData; /// lazy

        private bool _isExtendedObjectDataDirty = false;
        private Dictionary<string, byte[]> _extendedObjectData; /// lazy

        // ========================================
        // constructor
        // ========================================
        public EntityInterceptor(
            Type type, EntityContainer container, PersistentState state
        ) {
            _type = type;
            _container = container;
            _persistenceMaintainer = new PersistenceMaintainer(this, container, state);
        }

        // ========================================
        // event
        // ========================================
        public event EventHandler<PersistentStateChangedEventArgs> PersistentStateChanged {
            add { _persistenceMaintainer.PersistentStateChanged += value; }
            remove { _persistenceMaintainer.PersistentStateChanged -= value; }
        }

        // ========================================
        // property
        // ========================================
        // === IEntity ==========
        public object Target {
            get { return _target; }
            set {
                _target = value;
                _persistenceMaintainer.Owner = value;
            }
        }
        
        public Type Type {
            get { return _type; }
        }

        public string Id {
            get { return _id; }
            set { _id = value; }
        }

        public PersistentState State {
            get { return _persistenceMaintainer.State; }
        }

        public IEntityContainer Container {
            get { return _container; }
        }

        public IDictionary<string, string> ExtendedData {
            get { 
                if (_extendedData == null) {
                    _extendedData = new Dictionary<string, string>();
                }
                return _extendedData;
            }
        }

        public bool IsExtendedDataDirty {
            get { return _isExtendedDataDirty; }
            set { _isExtendedDataDirty = value; }
        }

        public IDictionary<string, byte[]> ExtendedObjectData {
            get { 
                if (_extendedObjectData == null) {
                    _extendedObjectData = new Dictionary<string, byte[]>();
                }
                return _extendedObjectData;
            }
        }

        public bool IsExtendedObjectDataDirty {
            get { return _isExtendedObjectDataDirty; }
            set { _isExtendedObjectDataDirty = value; }
        }

        // ------------------------------
        // internal
        // ------------------------------
        internal bool NeedRealInvocation {
            get { return _needRealInvocation; }
            set { _needRealInvocation = value; }
        }

        // ------------------------------
        // private
        // ------------------------------
        private Dictionary<PropertyInfo, object> _ListPropToListWrapperMap {
            get {
                if (_listPropToListWrapperMap == null) {
                    _listPropToListWrapperMap = new Dictionary<PropertyInfo, object>();
                }
                return _listPropToListWrapperMap;
            }
        }

        // ========================================
        // method
        // ========================================
        // === IEntity ==========
        public void Persist() {
            _persistenceMaintainer.Persist();
        }

        public void Dirty() {
            _persistenceMaintainer.Dirty();
        }

        public void Remove() {
            _persistenceMaintainer.Remove();
        }

        public void Reflect() {
            _persistenceMaintainer.Reflect(_target);
        }

        public void Rollback() {
            _persistenceMaintainer.Rollback(_target);
        }

        // === IInterceptor ==========
        public void Intercept(IInvocation invocation) {
            if (_needRealInvocation) {
                invocation.Proceed();
                return;
            }

            var method = invocation.Method;

            _persistenceMaintainer.EnsureLoaded(_target);

            var service = TypeService.Instance;

            if (_persistenceMaintainer.State == PersistentState.Discarded) {
                throw new InvalidOperationException("Discarded entity must not be used");

            } else if (ProceedOnlyMethods.Contains(method)) {
                invocation.Proceed();

            } else if (ProceedAndDirtyMethods.Contains(method)) {
                if (_container.IsReadonly) {
                    throw new InvalidOperationException("readonly");
                }
                invocation.Proceed();
                _persistenceMaintainer.Dirty();

            } else if (service.IsDirtyDefined(method)) {
                if (_container.IsReadonly) {
                    throw new InvalidOperationException("readonly");
                }
                invocation.Proceed();
                _persistenceMaintainer.Dirty();
                ProceedAndDirtyMethods.Add(method);

            } else if (MethodInfoUtil.IsPropertySetter(method)) {
                if (_container.IsReadonly) {
                    throw new InvalidOperationException("readonly");
                }
                invocation.Proceed();
                _persistenceMaintainer.Dirty();
                ProceedAndDirtyMethods.Add(method);

            } else if (MethodInfoUtil.IsPropertyGetter(method)) {
                if (method.ReturnType.IsArray) {
                    /// ここで場合分けしておかないとICollectionの分岐条件にマッチしてしまう
                    invocation.Proceed();
                    ProceedOnlyMethods.Add(method);
                } else if (MethodInfoUtil.IsICollectionPropertyGetter(method)) {
                    ReturnCollectionWrapperIfPossible(invocation, _target);
                } else {
                    invocation.Proceed();
                    ProceedOnlyMethods.Add(method);
                }

            } else {
                invocation.Proceed();
                ProceedOnlyMethods.Add(method);
            }
        }

        // --- protected ---
        protected void ReturnCollectionWrapperIfPossible(IInvocation invocation, object target) {
            var prop = TypeService.Instance.GetPropertyInfo(invocation.Method);
            if (!_ListPropToListWrapperMap.ContainsKey(prop)) {
                /// originalなListをinvocation.ReturnTypeに格納
                _needRealInvocation = true;
                invocation.Proceed();
                _needRealInvocation = false;

                var retValue = invocation.ReturnValue;
                if (retValue == null) {
                    /// nullの場合何もせずそのままnullを返させる
                    return;
                }

                var wrapper = WrapperTypeRegistry.CreateWrapper(_container, target, retValue, prop);

                /// wapperが生成できなかった場合は直に格納
                _ListPropToListWrapperMap[prop] = wrapper ?? invocation.ReturnValue;
            }
            invocation.ReturnValue = _ListPropToListWrapperMap[prop];
        }

    }
}
