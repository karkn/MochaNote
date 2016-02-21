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
    /// falseを返すkey，externalizableのときたどらない．
    /// </summary>
    public delegate bool ExternalizableFilter(string key, object externalizable);

    /// <summary>
    /// falseを返すkey，mementoのときたどらない．
    /// </summary>
    public delegate bool MementoFilter(string key, IMemento memento);
}
