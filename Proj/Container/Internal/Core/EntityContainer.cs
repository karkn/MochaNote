/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Linq;
using System.Collections.Generic;
using Castle.DynamicProxy;
using Mkamo.Common.Event;
using Mkamo.Common.Reflection;
using Mkamo.Container.Core;
using System.IO;
using System.Reflection.Emit;
using System.Reflection;
using Mkamo.Container.Query;
using Mkamo.Common.Diagnostics;
using Mkamo.Common.Visitor;
using Mkamo.Common.IO;
using Mkamo.Common.DataType;

namespace Mkamo.Container.Internal.Core {
    internal class EntityContainer: IEntityContainer {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(global::System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        // ========================================
        // field
        // ========================================
        private ProxyGenerator _proxyGenerator;
        private ModuleScope _scope;
        private IEntityStore _store;

        private Dictionary<string, WeakReference> _idToObjMap;
        private HashSet<object> _strongReferenced;

        private string _readonlyKey;

        // ========================================
        // constructor
        // ========================================
        public EntityContainer() {
            _proxyGenerator = new ProxyGenerator();
            Init(new XmlFileEntityStore());
        }

        public EntityContainer(IEntityStore entityStore) {
            _proxyGenerator = new ProxyGenerator();
            Init(entityStore);
        }

        public EntityContainer(
            IEntityStore entityStore,
            string proxyAssemblyName,
            string proxyAssemblyPath
        ) {
            _proxyGenerator = CreateProxyGenerator(proxyAssemblyName, proxyAssemblyPath);
            Init(entityStore);
        }

        private void Init(IEntityStore entityStore) {
            _store = entityStore;
            _store.EntityContainer = this;
            _idToObjMap = new Dictionary<string, WeakReference>();
            _strongReferenced = new HashSet<object>();
        }

        private ProxyGenerator CreateProxyGenerator(string proxyAssemblyName, string proxyAssemblyPath) {
            var strongAssemblyName = proxyAssemblyName;
            var strongModulePath = proxyAssemblyPath;
            var weakAssemblyName = ModuleScope.DEFAULT_ASSEMBLY_NAME;
            var weakModulePath = ModuleScope.DEFAULT_FILE_NAME;
            _scope = new ModuleScope(
                true,
                false,
                strongAssemblyName,
                strongModulePath,
                weakAssemblyName,
                weakModulePath
            );

            var builder = new DefaultProxyBuilder(_scope);
            return new ProxyGenerator(builder);
        }

        // ========================================
        // event
        // ========================================
        public event EventHandler<EntityEventArgs> EntityCreated;
        public event EventHandler<EntityEventArgs> EntityPersisted;
        public event EventHandler<EntityEventArgs> EntityRemoving;
        public event EventHandler<EntityEventArgs> EntityRemoved;


        // ========================================
        // property
        // ========================================
        public IEntityStore Store {
            get { return _store; }
        }

        public bool IsReadonly {
            get { return _readonlyKey != null; }
        }

        // ========================================
        // method
        // ========================================
        // === IEntityPersister ==========
        // --- proxy assembly ---
        public void SaveProxyAssembly(Assembly[] assems) {
            Contract.Requires(assems != null);

            var service = TypeService.Instance;
            foreach (var assem in assems) {
                var types = assem.GetTypes();
                foreach (var type in types) {
                    if (service.IsEntityDefined(type)) {
                        /// proxyをとりあえず作る
                        CreateTransientEntity(type);
                    }
                }
            }

            PathUtil.EnsureDirectoryExists(_scope.StrongNamedModuleDirectory);
            _scope.SaveAssembly(true);
        }

        /// ここでCreateTransientEntityした型以外proxyを作れなくなる?
        //public void SaveProxyAssembly(Assembly assem, string[] typeNames) {
        //    Contract.Requires(assem != null && typeNames != null);

        //    var service = TypeService.Instance;
        //    foreach (var typeName in typeNames) {
        //        var type = assem.GetType(typeName);
        //        if (type != null && service.IsEntityDefined(type)) {
        //            /// proxyをとりあえず作る
        //            CreateTransientEntity(type);
        //        }
        //    }

        //    PathUtil.EnsureDirectoryExists(_scope.StrongNamedModuleDirectory);
        //    _scope.SaveAssembly(true);
        //}

        public void LoadProxyAssembly(string proxyAssemblyPath) {
            Contract.Requires(proxyAssemblyPath != null);

            if (File.Exists(proxyAssemblyPath)) {
                //var assem = Assembly.LoadFile(proxyAssemblyPath);
                var assem = Assembly.LoadFrom(proxyAssemblyPath);
                _scope.LoadAssemblyIntoCache(assem);
            }
        }

        
        // --- find ---
        public object Find(Type type, string id) {
            if (!IsEntityDefined(type)) {
                throw new ArgumentException("Type attribute must be defined as Entity");
            }

            var entity = GetLoadedEntity(type, id, true);
            if (entity != null) {
                if (entity.State == PersistentState.Removed) {
                    return null;
                } else {
                    return entity.Target;
                }
            } else {
                if (!_store.IsEntityExists(type, id)) {
                    Logger.Warn("Entity not exists type=" + type.FullName + ",id=" + id); 
                    return null;
                } else {
                    return CreateEntity(type, id, PersistentState.Hollow);
                }
            }
        }

        public T Find<T>(string id) where T: class {
            return (T) Find(typeof(T), id);
        }

        public IEnumerable<T> FindAll<T>() where T: class {
            var ret = new List<T>();

            var ids = GetIds<T>();
            foreach (var id in ids) {
                var entity = Find<T>(id);
                if (entity != null) {
                    ret.Add(entity);
                }
            }

            return ret;
        }

        //public IEnumerable<T>  Find<T>(Condition cond) where T: class {
        //    Contract.Requires(cond != null);

        //    var ret = new List<T>();
        //    // todo: condに合うTをretに格納

        //    if (cond.IsComposite) {
        //        var compo = cond as CompositeCondition;
        //        compo.Accept(
        //            (c) => false,
        //            (c) => {

        //            },
        //            NextVisitOrder.PositiveOrder
        //        );

        //    }

        //    return ret;
        //}

        // --- id ---
        public IEnumerable<string> GetIds(Type type) {
            var ret = new HashSet<string>();

            var gcedIds = new HashSet<string>();
            var removedIds = new HashSet<string>();

            /// container管理下idからremoved/discarded以外を追加
            foreach (var pair in _idToObjMap) {
                var obj = pair.Value.Target;
                if (obj == null) {
                    /// GCされていたId
                    gcedIds.Add(pair.Key);
                } else {
                    var entity = AsEntity(obj);
                    if (type == entity.Type) {
                        if (entity.State == PersistentState.Removed || entity.State == PersistentState.Discarded) {
                            removedIds.Add(pair.Key);
                        } else {
                            ret.Add(pair.Key);
                        }
                    }
                }
            }

            /// store管理化idからremove/discarded以外を追加
            foreach (var idInStore in _store.GetIds(type)) {
                if (!removedIds.Contains(idInStore)) {
                    ret.Add(idInStore);
                }
            }

            /// ガベコレされていたidを削除
            foreach (var id in gcedIds) {
                _idToObjMap.Remove(id);
            }

            return ret;
        }

        public IEnumerable<string> GetIds<T>() {
            return GetIds(typeof(T));
        }

        public IEnumerable<string> GetIdsWhereWithEntityAndStoreRawData<T>(Predicate<T> entityWhere, Predicate<string> rawWhere) {
            var ret = new List<string>();

            var ids = GetIds<T>();
            foreach (var id in ids) {
                var done = false;

                if (_idToObjMap.ContainsKey(id)) {
                    var obj = _idToObjMap[id].Target;
                    if (obj != null) {
                        if (entityWhere((T) obj)) {
                            ret.Add(id);
                        }
                        done = true;
                    }
                }

                if (!done) {
                    /// store
                    var raw = _store.LoadRawData(typeof(T), id);
                    if (rawWhere(raw)) {
                        ret.Add(id);
                    }
                }
            }

            return ret;
        }

        public IEnumerable<string> GetIdsWhereWithEntityAndId<T>(Predicate<T> entityWhere, Predicate<string> idWhere) {
            var ret = new List<string>();

            var ids = GetIds<T>();
            foreach (var id in ids) {
                var done = false;

                if (_idToObjMap.ContainsKey(id)) {
                    var obj = _idToObjMap[id].Target;
                    if (obj != null) {
                        if (entityWhere((T) obj)) {
                            ret.Add(id);
                        }
                        done = true;
                    }
                }

                if (!done) {
                    if (idWhere(id)) {
                        ret.Add(id);
                    }
                }
            }

            return ret;
        }

        //public IEnumerable<string> GetIdsLike<T>(Predicate<T> entityWhere, string likeParam) {
        //    var type = typeof(T);

        //    var storeIds = _store.GetIdsLike(type, likeParam);

        //    var entityIds = new List<string>();
        //    Action<string, T> entityAction = (id, obj) => {
        //        if (entityWhere(obj)) {
        //            entityIds.Add(id);
        //        }
        //    };
        //    var removedIds = ForEachLoadedIdsAndGetRemovedIds<T>(entityAction);

        //    return storeIds.Except(removedIds).Concat(entityIds);
        //}

        // --- extended data ---
        public string LoadExtendedTextData(object obj, string key) {
            var entity = AsEntity(obj);
            if (entity == null) {
                return null;
            }
            return LoadExtendedTextData(entity.Type, entity.Id, key);
        }

        public void SaveExtendedTextData(object obj, string key, string value) {
            var entity = AsEntity(obj);
            if (entity == null) {
                return;
            }
            SaveExtendedTextData(entity.Type, entity.Id, key, value);
        }

        public string LoadExtendedTextData(Type type, string id, string key) {
            var entity = GetLoadedEntity(type, id, true);
            if (entity != null) {
                if (entity.State == PersistentState.Removed) {
                    return null;
                }

                var data = entity.ExtendedData;
                if (data.ContainsKey(key)) {
                    return data[key];
                } else {
                    var ret = _store.LoadExtendedTextData(type, id, key);
                    data[key] = ret;
                    return ret;
                }
            } else {
                return _store.LoadExtendedTextData(type, id, key);
            }
        }

        public void SaveExtendedTextData(Type type, string id, string key, string value) {
            var entity = GetLoadedEntity(type, id, true);
            if (entity != null) {
                if (entity.State == PersistentState.Removed) {
                    /// do nothing
                } else {
                    var data = entity.ExtendedData;
                    data[key] = value;
                    entity.IsExtendedDataDirty = true;
                    MaintainStrongReference(entity);
                }
            } else {
                _store.SaveExtendedTextData(type, id, key, value);
            }
        }


        public byte[] LoadExtendedBinaryData(object obj, string key) {
            var entity = AsEntity(obj);
            if (entity == null) {
                return null;
            }
            return LoadExtendedBinaryData(entity.Type, entity.Id, key);
        }

        public void SaveExtendedBinaryData(object obj, string key, byte[] value) {
            var entity = AsEntity(obj);
            if (entity == null) {
                return;
            }
            SaveExtendedBinaryData(entity.Type, entity.Id, key, value);
        }

        public byte[] LoadExtendedBinaryData(Type type, string id, string key) {
            var entity = GetLoadedEntity(type, id, true);
            if (entity != null) {
                if (entity.State == PersistentState.Removed) {
                    return null;
                }

                var data = entity.ExtendedObjectData;
                if (data.ContainsKey(key)) {
                    return data[key];

                } else {
                    var ret = _store.LoadExtendedBinaryData(type, id, key);

                    /// メモリを使いすぎるのでキャッシュはしない
                    /// Saveされて，かつ，commit/rollback前のときだけdataに値が入っている
                    /// data[key] = ret;

                    return ret;
                }
            } else {
                return _store.LoadExtendedBinaryData(type, id, key);
            }
        }

        public void SaveExtendedBinaryData(Type type, string id, string key, byte[] value) {
            var entity = GetLoadedEntity(type, id, true);
            if (entity != null) {
                if (entity.State == PersistentState.Removed) {
                    /// do nothing
                } else {
                    var data = entity.ExtendedObjectData;
                    data[key] = value;
                    entity.IsExtendedObjectDataDirty = true;
                    MaintainStrongReference(entity);
                }
            } else {
                _store.SaveExtendedBinaryData(type, id, key, value);
            }
        }

        // --- exist ---
        public bool IsExist(Type type, string id) {
            if (!IsEntityDefined(type)) {
                throw new ArgumentException("Type attribute must be defined as Entity");
            }

            var entity = GetLoadedEntity(type, id, true);
            if (entity != null) {
                if (entity.State == PersistentState.Removed) {
                    return false;
                } else {
                    return true;
                }
            } else {
                return _store.IsEntityExists(type, id);
            }
        }

        public bool IsExist<T>(string id) {
            return IsExist(typeof(T), id);
        }


        // --- create, persist ---
        public object Create(Type type) {
            if (!IsEntityDefined(type)) {
                throw new ArgumentException("Type attribute must be defined as Entity");
            }

            var newId = _store.CreateId();
            var ret = CreateEntity(type, newId, PersistentState.New);

            /// typeのpropを見てCompositeなものはさらにCreate()
            var service = TypeService.Instance;
            var props = service.GetCompositePersistProperties(type);
            foreach (var prop in props) {
                var attr = service.GetPersistAttribute(prop);
                var compType = attr.Composite;
                var compValue = Create(compType);
                prop.SetValue(ret, compValue, null);
            }

            OnEntityCreated(ret, type);
            OnEntityPersisted(ret, type);
            return ret;
        }

        public T Create<T>() where T: class {
            return (T) Create(typeof(T));
        }

        public object CreateTransient(Type type) {
            if (!IsEntityDefined(type)) {
                throw new ArgumentException("Type attribute must be defined as Entity");
            }

            var ret = CreateTransientEntity(type);

            /// typeのpropを見てCompositeなものはさらにCreateTransient()
            var service = TypeService.Instance;
            var props = service.GetCompositePersistProperties(type);
            foreach (var prop in props) {
                var attr = service.GetPersistAttribute(prop);
                var compType = attr.Composite;
                var compValue = CreateTransient(compType);
                prop.SetValue(ret, compValue, null);
            }

            OnEntityCreated(ret, type);
            return ret;
        }

        public T CreateTransient<T>() where T: class {
            return (T) CreateTransient(typeof(T));
        }

        public void Persist(object entity) {
            var e = AsEntity(entity);
            if (e == null) {
                throw new ArgumentException("entity");
            }

            e.Persist();
            if (!_idToObjMap.ContainsKey(e.Id)) {
                _idToObjMap.Add(e.Id, new WeakReference(entity));
            }
            PersistCascade(entity);
            OnEntityPersisted(entity, e.Type);
        }

        // --- dirty ---
        public void Dirty(object entity) {
            var e = AsEntity(entity);
            if (e == null) {
                throw new ArgumentException("entity");
            }
            e.Dirty();
        }

        // --- remove ---
        public void Remove(object entity) {
            var e = AsEntity(entity);
            if (e == null) {
                throw new ArgumentException("entity");
            }
            OnEntityRemoving(entity, e.Type);

            /// cascade先から削除しないとdiscardedになってpropertyにアクセスできない
            RemoveCascade(entity);
            e.Remove();

            OnEntityRemoved(entity, e.Type);
        }

        // --- commit, rollback ---
        public void Commit() {
            _store.Begin();
            var gcedId = new List<string>();
            foreach (var pair in _idToObjMap) {
                var obj = pair.Value.Target;
                if (obj == null) {
                    gcedId.Add(pair.Key);
                } else {
                    var entity = AsEntity(obj);
                    if (entity != null && IsPersistTarget(entity.Type)) {

                        try {
                            entity.Reflect();
                        } catch (Exception e) {
                            Logger.Warn("Can't reflect entity. type=" + entity.Type + ",id=" + entity.Id, e);
                        }

                        /// Reflect()後でないとStateがNewのときに保存場所が作られていない
                        if (entity.IsExtendedDataDirty) {
                            var data = entity.ExtendedData;
                            foreach (var datapair in data) {
                                var value = datapair.Value;
                                if (value != null) {
                                    _store.SaveExtendedTextData(entity.Type, entity.Id, datapair.Key, value);
                                }
                            }
                            entity.IsExtendedDataDirty = false;
                        }

                        if (entity.IsExtendedObjectDataDirty) {
                            var data = entity.ExtendedObjectData;
                            foreach (var datapair in data) {
                                var bytes = datapair.Value;
                                if (bytes != null) {
                                    _store.SaveExtendedBinaryData(entity.Type, entity.Id, datapair.Key, bytes);
                                }
                            }
                            data.Clear();
                            entity.IsExtendedObjectDataDirty = false;
                        }

                        MaintainStrongReference(entity);
                    }
                }
            }
            gcedId.ForEach(id => _idToObjMap.Remove(id));
            _store.Commit();
        }

        public void Rollback() {
            var gcedId = new List<string>();
            foreach (var pair in _idToObjMap) {
                var obj = pair.Value.Target;
                if (obj == null) {
                    gcedId.Add(pair.Key);
                } else {
                    var entity = AsEntity(obj);
                    if (entity != null && IsPersistTarget(entity.Type)) {
                        entity.Rollback();

                        if (entity.IsExtendedDataDirty) {
                            entity.ExtendedData.Clear();
                            entity.IsExtendedDataDirty = false;
                        }

                        if (entity.IsExtendedObjectDataDirty) {
                            entity.ExtendedObjectData.Clear();
                            entity.IsExtendedObjectDataDirty = false;
                        }

                        MaintainStrongReference(entity);
                    }
                }
            }
            gcedId.ForEach(id => _idToObjMap.Remove(id));
        }


        // --- entity info ---        
        public bool IsEntity(object obj) {
            return AsEntity(obj) != null;
        }

        public string GetId(object obj) {
            var entity = AsEntity(obj);
            if (entity == null) {
                return null;
                //throw new ArgumentException("Entity is not IEntityProxy");
            }
            return entity.Id;
        }

        public IEntity AsEntity(object entity) {
            return TypeService.Instance.AsEntity(entity);
        }

        public Type GetRealType(object entity) {
            return TypeService.Instance.GetRealType(entity);
        }


        // --- readonly ---
        public void ForceReadonly(string key) {
            if (_readonlyKey == null) {
                _readonlyKey = key;
            }
        }

        public void CancelReadonly(string key) {
            if (key == _readonlyKey) {
                _readonlyKey = null;
            }
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected virtual void OnEntityCreated(object created, Type createdType) {
            var handler = EntityCreated;
            if (handler != null) {
                handler(this, new EntityEventArgs(created, createdType));
            }
        }

        protected virtual void OnEntityPersisted(object persisted, Type persistedType) {
            var handler = EntityPersisted;
            if (handler != null) {
                handler(this, new EntityEventArgs(persisted, persistedType));
            }
        }

        protected virtual void OnEntityRemoving(object removing, Type removingType) {
            var handler = EntityRemoving;
            if (handler != null) {
                handler(this, new EntityEventArgs(removing, removingType));
            }
        }

        protected virtual void OnEntityRemoved(object removed, Type removedType) {
            var handler = EntityRemoved;
            if (handler != null) {
                handler(this, new EntityEventArgs(removed, removedType));
            }
        }

        // ------------------------------
        // internal
        // ------------------------------
        /// <summary>
        /// Commit()時に処理が必要なentityがGCされないようにする．
        /// </summary>
        internal void MaintainStrongReference(object entity, PersistentState state) {
            switch (state) {
                /// インスタンスが削除されても復元可能なもの
                case PersistentState.Hollow:
                case PersistentState.Discarded:
                case PersistentState.Latest: {
                    var ent = AsEntity(entity);
                    if (!ent.IsExtendedDataDirty && !ent.IsExtendedObjectDataDirty) {
                        _strongReferenced.Remove(entity);
                    }
                    break;
                }

                /// インスタンスが削除されると復元できないもの
                case PersistentState.Removed:
                case PersistentState.Updated:
                case PersistentState.New: {
                    _strongReferenced.Add(entity);
                    break;
                }

                /// 管理対象外
                case PersistentState.Transient: {
                    /// do nothing
                    break;
                }
            }
        }

        internal void MaintainStrongReference(IEntity entity) {
            var state = entity.State;
            switch (state) {
                /// インスタンスが削除されても復元可能なもの
                case PersistentState.Hollow:
                case PersistentState.Discarded:
                case PersistentState.Latest: {
                    if (!entity.IsExtendedDataDirty && !entity.IsExtendedObjectDataDirty) {
                        _strongReferenced.Remove(entity.Target);
                    } else {
                        _strongReferenced.Add(entity.Target);
                    }
                    break;
                }

                /// インスタンスが削除されると復元できないもの
                case PersistentState.Removed:
                case PersistentState.Updated:
                case PersistentState.New: {
                    _strongReferenced.Add(entity.Target);
                    break;
                }

                /// 管理対象外
                case PersistentState.Transient: {
                    /// do nothing
                    break;
                }
            }
        }

        // ------------------------------
        // private
        // ------------------------------
        private bool IsEntityDefined(Type type) {
            return TypeService.Instance.IsEntityDefined(type);
        }

        private bool IsPersistTarget(Type type) {
            return TypeService.Instance.IsPersistTarget(type);
        }

        /// <summary>
        /// type，idに対応するentity状態をしらべロード済みならEntityを返す．
        /// そうでなければnullを返す．
        /// returnRemovedがtrueのとき，RemovedなEntityも返す．
        /// </summary>
        private IEntity GetLoadedEntity(Type type, string id, bool returnRemoved) {
            if (_idToObjMap.ContainsKey(id)) {
                var obj = _idToObjMap[id].Target;
                if (obj == null) {
                    /// GCされていたらmapから削除しておく
                    _idToObjMap.Remove(id);
                } else {
                    var entity = AsEntity(obj);
                    if (entity.State == PersistentState.Discarded) {
                        return null;
                    } else if (entity.State == PersistentState.Removed && !returnRemoved) {
                        return null;
                    } else {
                        return entity;
                    }
                }
            }
            return null;
        }

        private object CreateEntity(Type type, string id, PersistentState state) {
            var interceptor = new EntityInterceptor(type, this, state);
            interceptor.Id = id;

            interceptor.NeedRealInvocation = true;
            var ret = _proxyGenerator.CreateClassProxy(type, interceptor);
            interceptor.Target = ret;
            interceptor.NeedRealInvocation = false;

            _idToObjMap.Add(id, new WeakReference(ret));
            MaintainStrongReference(ret, state);
            return ret;
        }

        private object CreateTransientEntity(Type type) {
            var interceptor = new EntityInterceptor(type, this, PersistentState.Transient);

            interceptor.NeedRealInvocation = true;
            var ret = _proxyGenerator.CreateClassProxy(type, interceptor);
            interceptor.Target = ret;
            interceptor.NeedRealInvocation = false;

            /// 呼んでも何もしないので呼ばない
            /// MaintainStrongReference(ret, PersistentState.Transient);
            return ret;
        }

        private void RemoveCascade(object entity) {
            var service = TypeService.Instance;
            var type = service.GetRealType(entity);
            var props = service.GetCascadingPersistProperties(type);

            foreach (var prop in props) {
                PropertyUtil.ProcessProperty(
                    entity,
                    prop,
                    null,
                    null,
                    (kind, value, key) => {
                        if (value != null) {
                            var valueType = service.GetRealType(value);
                            if (service.IsEntityDefined(valueType)) {
                                var valueEntity = service.AsEntity(value);
                                if (valueEntity != null) {
                                    Remove(value);
                                }
                            }
                        }
                        if (key != null) {
                            var keyType = service.GetRealType(key);
                            if (service.IsEntityDefined(keyType)) {
                                var keyEntity = service.AsEntity(key);
                                if (keyEntity != null) {
                                    Remove(key);
                                }
                            }
                        }
                    }
                );
            }
        }

        private void PersistCascade(object entity) {
            var service = TypeService.Instance;
            var type = service.GetRealType(entity);
            var props = service.GetCascadingPersistProperties(type);

            foreach (var prop in props) {
                PropertyUtil.ProcessProperty(
                    entity,
                    prop,
                    null,
                    null,
                    (kind, value, key) => {
                        if (value != null) {
                            var valueType = service.GetRealType(value);
                            if (service.IsEntityDefined(valueType)) {
                                var valueEntity = service.AsEntity(value);
                                if (valueEntity != null) {
                                    Persist(value);
                                }
                            }
                        }
                        if (key != null) {
                            var keyType = service.GetRealType(key);
                            if (service.IsEntityDefined(keyType)) {
                                var keyEntity = service.AsEntity(key);
                                if (keyEntity != null) {
                                    Persist(key);
                                }
                            }
                        }
                    }
                );
            }
        }

        /// <summary>
        /// loaded idに対してentityActionを実行する。
        /// 戻り値にはloadedなidのうち，Removed/Discardedなentityのidを返す。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityAction"></param>
        /// <returns></returns>
        private IEnumerable<string> ForEachLoadedIdsAndGetRemovedIds<T>(Action<string, T> entityAction) {
            var type = typeof(T);

            var gcedIds = new HashSet<string>();
            var removedIds = new HashSet<string>();

            /// container管理下idからremoved/discarded以外を追加
            foreach (var pair in _idToObjMap.ToArray()) {
                var obj = pair.Value.Target;
                if (obj == null) {
                    /// GCされていたId
                    gcedIds.Add(pair.Key);
                } else {
                    var entity = AsEntity(obj);
                    if (type == entity.Type) {
                        if (entity.State == PersistentState.Removed || entity.State == PersistentState.Discarded) {
                            removedIds.Add(pair.Key);
                        } else {
                            entityAction(pair.Key, (T) obj);
                        }
                    }
                }
            }

            /// store管理化idからremove/discarded以外を追加
            //foreach (var idInStore in _store.GetIds(type)) {
            //    if (!removedIds.Contains(idInStore)) {
            //        storeAction(idInStore);
            //    }
            //}

            /// ガベコレされていたidを削除
            foreach (var id in gcedIds) {
                _idToObjMap.Remove(id);
            }

            return removedIds;
        }

    }

}
