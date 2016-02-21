/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Container.Core;
using Mkamo.Model.Uml;
using Mkamo.Common.DataType;
using Mkamo.Editor.Core;
using Mkamo.Model.Memo;

namespace Mkamo.Memopad.Internal.Core {
    internal class MemoModelSerializer: IModelSerializer {
        /// <summary>
        /// serializeしたときに特殊な型名が入るのが嫌なのでEnumにせず，string定数の定義にしてある．
        /// </summary>
        private const string NormalKind = "Normal";
        private const string UmlPropertyCollectionKind = "UmlPropertyCollection";
        private const string UmlOperationCollectionKind = "UmlOperationCollection";

        public object Save(object model) {
            //if (model is Memo) {
            //    var container = MemopadApplication.Instance.Container;
            //    return container.GetId(model);
            //}

            //return model;

            var container = MemopadApplication.Instance.Container;
            var type = container.GetRealType(model);

            /// UmlPropertyCollection/UmlOperationCollectionはEntityでないので
            /// UmlClassifierから引き出せるようにしておく
            if (type.IsAssignableFrom(typeof(UmlPropertyCollection)))
            {
                var col = model as UmlPropertyCollection;
                var owner = col.Owner;
                var ownerType = container.GetRealType(owner);
                var typeName = ownerType.FullName + "," + ownerType.Assembly.GetName().Name;
                return Tuple.Create(UmlPropertyCollectionKind, typeName, container.GetId(owner));

            }
            else if (type.IsAssignableFrom(typeof(UmlOperationCollection)))
            {
                var col = model as UmlOperationCollection;
                var owner = col.Owner;
                var ownerType = container.GetRealType(owner);
                var typeName = ownerType.FullName + "," + ownerType.Assembly.GetName().Name;
                return Tuple.Create(UmlOperationCollectionKind, typeName, container.GetId(owner));

            }
            else
            {
                var typeName = type.FullName + "," + type.Assembly.GetName().Name;
                return Tuple.Create(NormalKind, typeName, container.GetId(model));
            }
        }

        public object Load(object hint) {
            //if (hint is string) {
            //    var container = MemopadApplication.Instance.Container;
            //    return container.Find<Memo>((string) hint);
            //}

            //return hint;

            var container = MemopadApplication.Instance.Container;

            var tuple = (Tuple<string, string, string>)hint;
            var kind = tuple.Item1;
            var typeName = tuple.Item2;
            var id = tuple.Item3;

            var type = Type.GetType(typeName);

            /// UmlPropertyCollection/UmlOperationCollectionはEntityでないので
            /// UmlClassifierから引き出す
            switch (kind)
            {
                case NormalKind:
                    {
                        return container.Find(type, id);
                    }
                case UmlPropertyCollectionKind:
                    {
                        var cls = container.Find(type, id) as UmlClassifier;
                        return cls.Attributes;
                    }
                case UmlOperationCollectionKind:
                    {
                        var cls = container.Find(type, id) as UmlClassifier;
                        return cls.Operations;
                    }
                default:
                    {
                        throw new ArgumentException("kind");
                    }
            }
        }
    }


}
