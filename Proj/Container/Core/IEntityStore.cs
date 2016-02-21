/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using Mkamo.Container.Internal.Core;
using Mkamo.Common.DataType;

namespace Mkamo.Container.Core {
    public interface IEntityStore {
        // ========================================
        // property
        // ========================================
        IEntityContainer EntityContainer { set; }
        int ValueQuota { get; set; }

        //string[] Ids { get; }
        //string[] RegisteredNames { get; }

        IDictionary<Type, ITypePersister> TypePersisters { get; }

        // ========================================
        // method
        // ========================================
        string CreateId();
        void Load(object target);
        void Insert(object target);
        void Update(object target);
        bool Remove(object target);

        void Begin();
        void Commit();

        /// <summary>
        /// finder(T value)の結果がtrueとなるEntityのIdのIEnumerableを返す．
        /// </summary>
        //IEnumerable<string> FindIds<T>(Type type, Func<T, bool> finder);

        bool IsEntityExists(Type targetType, string id);
        IEnumerable<string> GetIds(Type type);
        IEnumerable<string> GetIdsLike(Type type, string likeParam);

        string LoadExtendedTextData(Type type, string id, string key);
        void SaveExtendedTextData(Type type, string id, string key, string value);
        byte[] LoadExtendedBinaryData(Type type, string id, string key);
        void SaveExtendedBinaryData(Type type, string id, string key, byte[] value);

        string LoadRawData(Type type, string id);
        
        //void RegisterName(string name, IEntityProxy target);
        //void UnregisterName(string name);
        //IEntityProxy GetNamedEntity(string name);
    }
}
