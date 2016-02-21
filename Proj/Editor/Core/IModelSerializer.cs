/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Editor.Core {
    public interface IModelSerializer {
        /// <summary>
        /// modelの保存処理を行いロードするために必要な情報(hint)を返す．
        /// hintはSerializableでなければならない．
        /// </summary>
        object Save(object model);

        /// <summary>
        /// hintを元にmodelをロードして返す．
        /// </summary>
        object Load(object hint);
    }
}
