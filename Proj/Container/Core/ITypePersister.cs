/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Mkamo.Container.Core {
    public interface ITypePersister {
        /// <summary>
        /// valueの保存すべき内容をvaluesに格納する．
        /// </summary>
        void Save(object value, IDictionary<string, string> values);

        /// <summary>
        /// valuesの情報をもとにobjectを復元して返す．
        /// </summary>
        object Load(IDictionary<string, string> values);
    }
}
