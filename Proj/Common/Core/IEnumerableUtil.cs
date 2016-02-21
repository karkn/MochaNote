/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Diagnostics;
using Mkamo.Common.Collection;

namespace Mkamo.Common.Core {
    public static class IEnumerableUtil {

        public static bool IsNullOrEmpty<T>(IEnumerable<T> enumerable) {
            return enumerable == null || enumerable.Count() == 0;
        }


        /// <summary>
        /// IEnumerable<TIn>をIEnumerable<TReturn>として返す
        /// </summary>
        public static IEnumerable<TReturn> As<TIn, TReturn>(this IEnumerable<TIn> enumerable)
            where TIn: TReturn
        {
            foreach (var elem in enumerable) {
                yield return elem;
            }
        }

        /// <summary>
        /// enumerableからfinderがtrueを返す要素のうち最初の物を返す．
        /// 見つからなければdefault(T)を返す．
        /// </summary>
        public static T Find<T>(this IEnumerable<T> enumerable, Predicate<T> finder) {
            foreach (T item in enumerable) {
                if (finder(item)) {
                    return item;
                }
            }
            return default(T);
        }

        /// <summary>
        /// enumerableからfinderがtrueを返す要素のうち最初の物を返す．
        /// 見つからなければdefaultValueを返す．
        /// </summary>
        public static T FindDefault<T>(this IEnumerable<T> enumerable, Predicate<T> finder, T defaultValue) {
            foreach (T item in enumerable) {
                if (finder(item)) {
                    return item;
                }
            }
            return defaultValue;
        }

        /// <summary>
        /// enumerableの各要素を引数にしてactionを呼び出す．
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action) {
            foreach (T item in enumerable) {
                action(item);
            }
        }

        /// <summary>
        /// enumerableがvaluesの要素のどれか一つを含むかどうかを返す．
        /// </summary>
        public static bool ContainsAny<T>(this IEnumerable<T> enumerable, IEnumerable<T> values) {
            if (enumerable == null || values == null) {
                return false;
            }

            foreach (T value in values) {
                if (enumerable.Contains(value)) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// enumerableがvaluesの要素のすべてを含むかどうかを返す．
        /// </summary>
        public static bool ContainsAll<T>(this IEnumerable<T> enumerable, IEnumerable<T> values) {
            if (!values.Any()) {
                return false;
            }

            foreach (T value in values) {
                if (!enumerable.Contains(value)) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// enumerableで最初に見つかったelemのindexを返す．
        /// </summary>
        public static int IndexOf<T>(this IEnumerable<T> enumerable, T elem) {
            return IndexOf(enumerable, elem, null);
        }

        /// <summary>
        /// enumerableで最初に見つかったelemのindexを返す．
        /// 要素の同値判定にはcomparerを使う．
        /// </summary>
        public static int IndexOf<T>(this IEnumerable<T> enumerable, T elem, IEqualityComparer<T> comparer) {
            if (comparer == null) {
                comparer = EqualityComparer<T>.Default;
            }

            int ret = 0;
            foreach (var e in enumerable) {
                if (comparer.Equals(e, elem)) {
                    return ret;
                }
                ++ret;
            }
            return -1; 
        }

        /// <summary>
        /// enumerableのfirstからlastの間の要素を格納した配列を返す．
        /// firstとlastも配列に含まれる．
        /// </summary>
        public static T[] Range<T>(this IEnumerable<T> enumerable, T first, T last) where T: class {
            var ret = new List<T>();

            var finished = false;
            var inRange = false;
            foreach (T elem in enumerable) {
                if (elem == first) {
                    inRange = true;
                }

                if (inRange) {
                    ret.Add(elem);

                    if (elem == last) {
                        finished = true;
                        inRange = false;
                        break;
                    }
                }
            }

            Contract.Ensures(!inRange, "last");
            Contract.Ensures(finished, "first, last");

            return ret.ToArray();
        }

        /// <summary>
        /// enumerableのindex番目からlength個の要素を格納したIEnumerableを返す．
        /// </summary>
        public static IEnumerable<T> Range<T>(this IEnumerable<T> enumerable, int index, int length) {
            if (index < 0 || length < 1) {
                yield break;
            }

            var i = 0;
            foreach (T elem in enumerable) {
                if (i == index + length) {
                    yield break;
                }

                if (i >= index) {
                    yield return elem;
                }

                ++i;
            }
        }

        /// <summary>
        /// enumerableからfuncでIComparableを取得し，それが最小の要素を返す．
        /// </summary>
        public static T FindMin<T, S>(this IEnumerable<T> enumerable, Func<T, S> func) where S: IComparable {
            Contract.Requires(!IEnumerableUtil.IsNullOrEmpty(enumerable));

            var ret = enumerable.First();
            var value = func(ret);

            foreach (var elem in enumerable) {
                S v = func(elem);
                if (v.CompareTo(value) < 0) {
                    value = v;
                    ret = elem;
                }
            }

            return ret;
        }

    
        /// <summary>
        /// col1に含まれていてcol2には含まれていない要素を格納したListを返す
        /// </summary>
        public static List<T> Subtract<T>(this IEnumerable<T> e1, IEnumerable<T> e2) {
            List<T> result = new List<T>();
            foreach (T item1 in e1) {
                if (!e2.Contains(item1)) {
                    result.Add(item1);
                }
            }
            return result;
        }

        /// <summary>
        /// col1だけに含まれている要素とcol2だけに含まれている要素を格納したListを返す
        /// </summary>
        public static List<T> Disjunction<T>(this IEnumerable<T> e1, IEnumerable<T> e2) {
            List<T> ret = new List<T>();
            foreach (T item1 in e1) {
                if (!e2.Contains(item1)) {
                    ret.Add(item1);
                }
            }
            foreach (T item2 in e2) {
                if (!e1.Contains(item2)) {
                    ret.Add(item2);
                }
            }
            return ret;
        }

        /// <summary>
        /// col1とcol2の両方に含まれている要素を格納したListを返す
        /// </summary>
        public static List<T> Intersection<T>(this IEnumerable<T> e1, IEnumerable<T> e2) {
            List<T> ret = new List<T>();
            foreach (T item1 in e1) {
                if (e2.Contains(item1)) {
                    ret.Add(item1);
                }
            }
            return ret;
        }

        /// <summary>
        /// col1とcol2に含まれるすべての要素を格納したListを返す
        /// </summary>
        public static List<T> Union<T>(this IEnumerable<T> e1, IEnumerable<T> e2) {
            List<T> ret = new List<T>(e1);
            foreach (T item2 in e2) {
                if (!ret.Contains(item2)) {
                    ret.Add(item2);
                }
            }
            return ret;
        }

        public static bool EqualsEachInOrder<T>(this IEnumerable<T> e1, IEnumerable<T> e2) {
            var enumerator = e2.GetEnumerator();
            foreach (var elem1 in e1) {
                if (!enumerator.MoveNext()) {
                    return false;
                }
                var elem2 = enumerator.Current;
                if (!EqualityComparer<T>.Default.Equals(elem1, elem2)) {
                    return false;
                }
            }
            return !enumerator.MoveNext();
        }

        public static bool EqualsEachInRandomOrder<T>(this IEnumerable<T> e1, IEnumerable<T> e2) {
            var set1 = new HashSet<T>(e1);
            var set2 = new HashSet<T>(e2);
            foreach (var elem1 in set1) {
                if (set2.Count == 0) {
                    return false;
                }
                if (!set2.Remove(elem1)) {
                    return false;
                }
            }
            return set2.Count == 0;
        }

    }
}
