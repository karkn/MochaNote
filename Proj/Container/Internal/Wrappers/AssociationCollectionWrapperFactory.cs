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
using System.Collections.ObjectModel;
using Mkamo.Common.Association;

namespace Mkamo.Container.Internal.Wrappers {
    internal class AssociationCollectionWrapperFactory: IReturnValueWrapperFactory {
        public object CreateWrapper(IEntityContainer container, object owner, object real, Type type) {
            var elemType = GenericTypeUtil.GetGenericArgumentOfGenericAssociationCollection(type);
            var specifiedWrapperType = typeof(AssociationCollectionWrapper<>).MakeGenericType(elemType);
            var con =
                specifiedWrapperType.GetConstructor(
                    new Type[] {
                        typeof(IEntityContainer),
                        typeof(object),
                        typeof(AssociationCollection<>).MakeGenericType(elemType)
                    }
                );
            return con.Invoke(new object[] { container, owner, real });
        }
    }
}
