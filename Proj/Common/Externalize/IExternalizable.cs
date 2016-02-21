/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Common.Externalize {
    /// <summary>
    /// mementoへの保存や復元が可能なオブジェクト．
    /// Serializableでは保存しにくいオブジェクトを一旦mementにして保存しやすくする．
    /// Serializableではできない参照をたどるかどうかを動的な指定ができる．
    /// </summary>
    public interface IExternalizable {
        /// <summary>
        /// 渡されたmementoに保存すべき状態を格納する．
        /// </summary>
        void WriteExternal(IMemento memento, ExternalizeContext context);

        /// <summary>
        /// 渡されたmementoの情報を使って状態を復元する．
        /// </summary>
        void ReadExternal(IMemento memento, ExternalizeContext context);
    }
}
