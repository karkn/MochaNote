/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Reflection;
using Mkamo.Common.Event;
using Mkamo.Container.Core;
using System.Collections.Generic;
using Mkamo.Common.DataType;

namespace Mkamo.Container.Core {
    /// Castle.DynamicProxy2を使うためにpublicでなければならない
    public interface IEntity {
        // ========================================
        // property
        // ========================================
        object Target { get; set; }
        Type Type { get; }
        string Id { get; set; }
        PersistentState State { get; }
        IEntityContainer Container { get; }

        IDictionary<string, string> ExtendedData { get; }
        bool IsExtendedDataDirty { get; set; }

        IDictionary<string, byte[]> ExtendedObjectData { get; }
        bool IsExtendedObjectDataDirty { get; set; }

        // ========================================
        // event
        // ========================================
        event EventHandler<PersistentStateChangedEventArgs> PersistentStateChanged;

        // ========================================
        // method
        // ========================================
        void Persist();
        void Dirty();
        void Remove();
        void Reflect();
        void Rollback();
    }

    [Serializable]
    public enum PersistentState {
        /// <summary>
        /// Entityが作成されたが保存対象になっていない状態
        /// </summary>
        Transient,

        /// <summary>
        /// Entityが作成されて保存対象であるが，まだStoreに反映されていない状態
        /// </summary>
        New,

        /// <summary>
        /// Findされた後にメンバにアクセスされておらずメンバがロードされていない状態
        /// </summary>
        Hollow,

        /// <summary>
        /// メンバがStoreに格納されている値と同期されている状態
        /// </summary>
        Latest,

        /// <summary>
        /// Entityが削除されてStoreにまだ反映されていない状態
        /// </summary>
        Removed,

        /// <summary>
        /// Entityのメンバが更新されてStoreにまだ反映されていない状態
        /// </summary>
        Updated,

        /// <summary>
        /// Entityが廃棄済みである状態．
        /// NewなEntityがRemoveされたり，RemovedなEntityがReflectされるとDiscardedになる．
        /// </summary>
        Discarded,
    }

    public class PersistentStateChangedEventArgs: EventArgs {
        private PersistentState _state;
        public PersistentStateChangedEventArgs(PersistentState state) {
            _state = state;
        }
        public PersistentState State {
            get { return _state; }
        }
    }
}
