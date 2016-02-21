/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Mkamo.Common.Event {
    /// <summary>
    /// イベントハンドラをkeyごとに管理する．
    /// </summary>
    public interface IQualifiedEventHandlers<TEventArgs> where TEventArgs: EventArgs {
        // ========================================
        // property
        // ========================================
        /// <summary>
        /// イベントハンドラがまだ登録されていなければtrueを返す．
        /// </summary>
        bool IsEmpty { get; }

        // ========================================
        // method
        // ========================================
        /// <summary>
        /// すべてのキーに対して呼び出すイベントハンドラを登録する．
        /// </summary>
        void AddHandler(EventHandler<TEventArgs> handler);

        /// <summary>
        /// keyに対して呼び出すイベントハンドラを登録する．
        /// </summary>
        void AddHandler(object key, EventHandler<TEventArgs> handler);

        /// <summary>
        /// すべてのキーに対して呼び出すイベントハンドラを削除する．
        /// </summary>
        void RemoveHandler(EventHandler<TEventArgs> handler);

        /// <summary>
        /// keyに対して呼び出すイベントハンドラを削除する．
        /// </summary>
        void RemoveHandler(object key, EventHandler<TEventArgs> handler);
    }
}
