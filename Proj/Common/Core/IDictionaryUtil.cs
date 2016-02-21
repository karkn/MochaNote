/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Common.Core {
    public static class IDictionaryUtil {
        /// <summary>
        /// dictからkeysを削除する。
        /// </summary>
        public static void RemoveKeys<TKey, TValue>(this IDictionary<TKey, TValue> dict, IEnumerable<TKey> keys) {
            foreach (var key in keys) {
                if (dict.ContainsKey(key)) {
                    dict.Remove(key);
                }
            }
        }


        /// <summary>
        /// dictからpredがtrueになるkeyを削除する。
        /// </summary>
        public static void RemoveKeys<TKey, TValue>(this IDictionary<TKey, TValue> dict, Predicate<TKey> pred) {
            var removings = new List<TKey>();
            foreach (var key in dict.Keys) {
                if (pred(key)) {
                    removings.Add(key);
                }
            }

            foreach (var removing in removings) {
                dict.Remove(removing);
            }
        }
    }
}
