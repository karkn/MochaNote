/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Mkamo.Common.Core {
    public static class ICollectionUtil {
        /// <summary>
        /// colからpredがtrueになる要素を削除する。
        /// </summary>
        public static void Remove<T>(ICollection<T> col, Predicate<T> pred) {
            var removings = new List<T>();
            foreach (var elem in col) {
                if (pred(elem)) {
                    removings.Add(elem);
                }
            }

            foreach (var removing in removings) {
                col.Remove(removing);
            }
        }

        /// <summary>
        /// col1に含まれていてcol2には含まれていない要素を格納したコレクションを返す
        /// </summary>
        public static ICollection<T> Subtract<T>(ICollection<T> col1, ICollection<T> col2) {
            List<T> result = new List<T>();
            foreach (T item1 in col1) {
                if (!col2.Contains(item1)) {
                    result.Add(item1);
                }
            }
            return result;
        }

        /// <summary>
        /// col1だけに含まれている要素とcol2だけに含まれている要素を格納したコレクションを返す
        /// </summary>
        public static ICollection<T> Disjunction<T>(ICollection<T> col1, ICollection<T> col2) {
            List<T> ret = new List<T>();
            foreach (T item1 in col1) {
                if (!col2.Contains(item1)) {
                    ret.Add(item1);
                }
            }
            foreach (T item2 in col2) {
                if (!col1.Contains(item2)) {
                    ret.Add(item2);
                }
            }
            return ret;
        }

        /// <summary>
        /// col1とcol2の両方に含まれている要素を格納したコレクションを返す
        /// </summary>
        public static ICollection<T> Intersection<T>(ICollection<T> col1, ICollection<T> col2) {
            List<T> ret = new List<T>();
            foreach (T item1 in col1) {
                if (col2.Contains(item1)) {
                    ret.Add(item1);
                }
            }
            return ret;
        }

        /// <summary>
        /// col1とcol2に含まれるすべての要素を格納したコレクションを返す
        /// </summary>
        public static ICollection<T> Union<T>(ICollection<T> col1, ICollection<T> col2) {
            List<T> ret = new List<T>(col1);
            foreach (T item2 in col2) {
                if (!ret.Contains(item2)) {
                    ret.Add(item2);
                }
            }
            return ret;
        }

    }
}
