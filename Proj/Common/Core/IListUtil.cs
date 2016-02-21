/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Collection;
using Mkamo.Common.Collection.Internal;
using System.Collections;

namespace Mkamo.Common.Core {
    public static class IListUtil {
        public static int IndexOf<T>(this IList<T> list, Predicate<T> finder) {
            for (int i = 0; i < list.Count; ++i) {
                if (finder(list[i])) {
                    return i;
                }
            }
            return -1;
        }

        public static void Reorder<T>(this IList<T> list, T elem, int index) {
            list.Remove(elem);
            list.Insert(index, elem);
        }

        public static RangedList<T> GetRangedList<T>(this IList<T> list, int index, int count) {
            return new RangedList<T>(list, index, count);
        }

        public static bool IsEmptyList<T>(this IList<T> list) {
            return list is EmptyList<T>;
        }

        public static IList<T> Cast<T>(IList list) {
            return new AsIListWrapper<T>(list);
        }
    }
}
