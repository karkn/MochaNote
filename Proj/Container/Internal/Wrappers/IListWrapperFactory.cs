/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Mkamo.Container.Internal.Core;
using Mkamo.Common.Reflection;
using System.Reflection;
using Mkamo.Container.Core;
using Mkamo.Common.Event;

namespace Mkamo.Container.Internal.Wrappers {
    internal class IListWrapperFactory: IReturnValueWrapperFactory {
        public object CreateWrapper(IEntityContainer container, object owner, object real, Type type) {
            var elemType = GenericTypeUtil.GetGenericArgumentOfGenericIList(type);
            var specifiedIListWrapperType = typeof(IListWrapper<>).MakeGenericType(elemType);
            var con =
                specifiedIListWrapperType.GetConstructor(
                    new Type[] {
                        typeof(IEntityContainer),
                        typeof(object),
                        typeof(IList<>).MakeGenericType(elemType)
                    }
                );
            return con.Invoke(new object[] { container, owner, real });
        }
    }
}
