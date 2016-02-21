/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using Mkamo.Common.Event;
using Mkamo.Container.Internal.Core;
using System.Reflection;

namespace Mkamo.Container.Core {
    public interface IEntityContainer {
        event EventHandler<EntityEventArgs> EntityCreated;
        event EventHandler<EntityEventArgs> EntityRemoving;
        event EventHandler<EntityEventArgs> EntityRemoved;
        event EventHandler<EntityEventArgs> EntityPersisted;

        // ========================================
        // property
        // ========================================
        IEntityStore Store { get; }
        bool IsReadonly { get; }

        // ========================================
        // method
        // ========================================
        // --- proxy assembly ---
        void SaveProxyAssembly(Assembly[] assems);
        //void SaveProxyAssembly(Assembly assem, string[] typeNames);
        void LoadProxyAssembly(string proxyAssemblyPath);

        // --- find ---
        T Find<T>(string id) where T: class;
        object Find(Type type, string id);

        IEnumerable<T> FindAll<T>() where T: class;

        // --- ids ---
        IEnumerable<string> GetIds<T>();
        IEnumerable<string> GetIds(Type type);
        /// <summary>
        /// finder(T value)の結果がtrueとなるEntityのIdのIEnumerableを返す．
        /// </summary>
        //IEnumerable<string> FindIds<T>(Type type, Func<T, bool> finder);
        IEnumerable<string> GetIdsWhereWithEntityAndStoreRawData<T>(Predicate<T> entityWhere, Predicate<string> rawWhere);
        IEnumerable<string> GetIdsWhereWithEntityAndId<T>(Predicate<T> entityWhere, Predicate<string> idWhere);
        //IEnumerable<string> GetIdsLike<T>(Predicate<T> entityWhere, string likeParam);
        

        // --- extended data ---
        /// <summary>
        /// type，idに対応するentityの拡張データをロードする．
        /// entityがロードされていない場合，entityをロードせずに値を取れる．
        /// </summary>
        string LoadExtendedTextData(Type type, string id, string key);
        string LoadExtendedTextData(object obj, string key);

        /// <summary>
        /// type，idに対応するentityの拡張データを保存する．
        /// entityがロードされていない場合，entityをロードせずに値を保存できる．
        /// </summary>
        void SaveExtendedTextData(Type type, string id, string key, string value);
        void SaveExtendedTextData(object obj, string key, string value);

        byte[] LoadExtendedBinaryData(object obj, string key);
        byte[] LoadExtendedBinaryData(Type type, string id, string key);

        void SaveExtendedBinaryData(object obj, string key, byte[] value);
        void SaveExtendedBinaryData(Type type, string id, string key, byte[] value);
        
        // --- exist ---
        bool IsExist<T>(string id);
        bool IsExist(Type type, string id);

 
        // --- create, persist ---
        T Create<T>() where T: class;
        object Create(Type type);

        T CreateTransient<T>() where T: class;
        object CreateTransient(Type type);

        void Persist(object entity);

        // --- dirty ---
        void Dirty(object entity);

        // --- remove ---
        void Remove(object entity);

        // --- commit, rollback ---
        void Commit();
        void Rollback();

        // --- info ---
        bool IsEntity(object entity);

        string GetId(object entity);

        IEntity AsEntity(object entity);
        Type GetRealType(object entity);

        // --- readonly ---
        void ForceReadonly(string key);
        void CancelReadonly(string key);
    }
}
