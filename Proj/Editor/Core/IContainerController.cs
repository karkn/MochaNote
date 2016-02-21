/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Editor.Core {
    public interface IContainerController: IController {
        // ========================================
        // property
        // ========================================
        /// <summary>
        /// 子要素を返す．
        /// </summary>
        IEnumerable<object> Children { get; }

        /// <summary>
        /// 子要素の数を返す．
        /// </summary>
        int ChildCount { get; }

        /// <summary>
        /// Refresh時にChildEditorを再構築するかどうかを返す．
        /// </summary>
        bool SyncChildEditors { get; }

        // ========================================
        // method
        // ========================================
        /// <summary>
        /// factoryによって生成されるオブジェクトを子として持てるかを返す．
        /// </summary>
        bool CanContainChild(IModelDescriptor descriptor);

        /// <summary>
        /// index位置にchildを追加する．
        /// </summary>
        void InsertChild(object child, int index);

        /// <summary>
        /// childを削除する．
        /// </summary>
        void RemoveChild(object child);

    }
}
