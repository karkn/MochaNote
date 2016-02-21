/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Reflection;
using Mkamo.Common.Event;
using Mkamo.Container.Core;

namespace Mkamo.Container.Internal.Core {
    internal interface IReturnValueWrapperFactory {
        object CreateWrapper(IEntityContainer container, object owner, object real, Type type);
    }
}
