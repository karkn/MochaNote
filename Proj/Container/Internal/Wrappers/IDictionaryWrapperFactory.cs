/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Mkamo.Container.Internal.Core;
using Mkamo.Container.Core;
using System.Reflection;
using Mkamo.Common.Event;
using Mkamo.Common.Reflection;

namespace Mkamo.Container.Internal.Wrappers {
    internal class IDictionaryWrapperFactory: IReturnValueWrapperFactory {
        public object CreateWrapper(IEntityContainer container, object owner, object real, Type type) {
            var elemTypes = GenericTypeUtil.GetGenericArgumentOfGenericIDictionary(type);
            var specifiedIDictionaryWrapperType = typeof(IDictionaryWrapper<,>).MakeGenericType(elemTypes);
            var con =
                specifiedIDictionaryWrapperType.GetConstructor(
                    new Type[] {
                        typeof(IEntityContainer),
                        typeof(object),
                        typeof(IDictionary<,>).MakeGenericType(elemTypes)
                    }
                );
            return con.Invoke(new object[] { container, owner, real });
        }
    }
}
