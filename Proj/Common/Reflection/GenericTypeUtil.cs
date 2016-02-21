/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Mkamo.Common.Diagnostics;
using System.Collections.ObjectModel;
using Mkamo.Common.Association;

namespace Mkamo.Common.Reflection {
    public static class GenericTypeUtil {
        public static bool EqualsGenericIEnumerable(Type type) {
            Contract.Requires(type != null);

            if (!type.IsGenericType) {
                return false;
            }
            return type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
        }

        public static bool EqualsGenericICollection(Type type) {
            Contract.Requires(type != null);

            if (!type.IsGenericType) {
                return false;
            }
            return type.GetGenericTypeDefinition() == typeof(ICollection<>);
        }

        public static bool EqualsGenericCollection(Type type) {
            Contract.Requires(type != null);

            if (!type.IsGenericType) {
                return false;
            }
            return type.GetGenericTypeDefinition() == typeof(Collection<>);
        }

        public static bool EqualsGenericAssociationCollection(Type type) {
            Contract.Requires(type != null);

            if (!type.IsGenericType) {
                return false;
            }
            return type.GetGenericTypeDefinition() == typeof(AssociationCollection<>);
        }

        public static bool EqualsGenericIList(Type type) {
            Contract.Requires(type != null);

            if (!type.IsGenericType) {
                return false;
            }
            return type.GetGenericTypeDefinition() == typeof(IList<>);
        }

        public static bool EqualsGenericIDictionary(Type type) {
            Contract.Requires(type != null);

            if (!type.IsGenericType) {
                return false;
            }
            return type.GetGenericTypeDefinition() == typeof(IDictionary<,>);
        }

        /// <summary>
        /// typeがcompGenTypeDefそのものまたは継承または実装したタイプかどうかを返す．
        /// </summary>
        public static bool IsGeneric(Type type, Type compGenTypeDef) {
            Contract.Requires(type != null);
            Contract.Requires(compGenTypeDef != null);
            Contract.Requires(compGenTypeDef.IsGenericTypeDefinition);
            
            if (type == compGenTypeDef) {
                return true;
            }
            if (type.IsGenericType) {
                var genTypeDef = type.GetGenericTypeDefinition();
                return
                    compGenTypeDef == genTypeDef ||
                    genTypeDef.GetInterface(compGenTypeDef.FullName) != null;
            } else {
                return type.GetInterface(compGenTypeDef.FullName) != null;
            }
        }

        public static bool IsGenericIEnumerable(Type type) {
            return IsGeneric(type, typeof(IEnumerable<>));
        }

        public static bool IsGenericICollection(Type type) {
            return IsGeneric(type, typeof(ICollection<>));
        }

        public static bool IsGenericCollection(Type type) {
            return IsGeneric(type, typeof(Collection<>));
        }

        public static bool IsGenericAssociationCollection(Type type) {
            return IsGeneric(type, typeof(AssociationCollection<>));
        }

        public static bool IsGenericIList(Type type) {
            return IsGeneric(type, typeof(IList<>));
        }

        public static bool IsGenericIDictionary(Type type) {
            return IsGeneric(type, typeof(IDictionary<,>));
        }

        public static Type[] GetGenericArguments(Type type, Type genTypeDefHasArgs) {
            Contract.Requires(type != null);

            if (type.IsGenericType) {
                var genTypeDef = type.GetGenericTypeDefinition();
                if (genTypeDef == genTypeDefHasArgs) {
                    return type.GetGenericArguments();
                }
            }
            var found = type.GetInterface(genTypeDefHasArgs.FullName);
            Contract.Ensures(found != null, "type doesn't implement getTypeDefHasArgs");

            return found.GetGenericArguments();
        }

        public static Type GetGenericArgumentOfGenericIEnumerable(Type type) {
            var ret = GetGenericArguments(type, typeof(IEnumerable<>));
            Contract.Ensures(ret != null && ret.Length > 0, "type");
            return ret[0];
        }

        public static Type GetGenericArgumentOfGenericICollection(Type type) {
            var ret = GetGenericArguments(type, typeof(ICollection<>));
            Contract.Ensures(ret != null && ret.Length > 0);
            return ret[0];
        }

        public static Type GetGenericArgumentOfGenericCollection(Type type) {
            var ret = GetGenericArguments(type, typeof(Collection<>));
            Contract.Ensures(ret != null && ret.Length > 0);
            return ret[0];
        }

        public static Type GetGenericArgumentOfGenericAssociationCollection(Type type) {
            var ret = GetGenericArguments(type, typeof(AssociationCollection<>));
            Contract.Ensures(ret != null && ret.Length > 0);
            return ret[0];
        }

        public static Type GetGenericArgumentOfGenericIList(Type type) {
            var ret = GetGenericArguments(type, typeof(IList<>));
            Contract.Ensures(ret != null && ret.Length > 0);
            return ret[0];
        }

        public static Type[] GetGenericArgumentOfGenericIDictionary(Type type) {
            return GetGenericArguments(type, typeof(IDictionary<,>));
        }

    }
}
