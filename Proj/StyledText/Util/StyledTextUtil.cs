/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Mkamo.StyledText.Util {
    public static class StyledTextUtil {
        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        /// <summary>
        /// sを改行で分割したstring[]の配列を返す．
        /// 改行そのものは配列に格納されない．
        /// </summary>
        public static string[] SplitWithBlockBreak(string s) {
            if (string.IsNullOrEmpty(s)) {
                return new string[0];
            }

            s = NormalizeLineBreak(s, "\r");

            var ret = new List<string>();

            int start = 0;
            int end = 0;
            while ((end =  s.IndexOf('\r', start)) > -1) {
                ret.Add(s.Substring(start, end - start));
                start = end + 1;
            }
            if (start != s.Length) {
                ret.Add(s.Substring(start));
            }

            return ret.ToArray();
        }


        /// <summary>
        /// sの改行をlineBreakに統一したstringを返す．
        /// </summary>
        /// <returns></returns>
        public static string NormalizeLineBreak(string s, string lineBreak) {
            if (string.IsNullOrEmpty(s)) {
                return string.Empty;
            }

            return Regex.Replace(s, "\\r\\n|\\r|\\n", lineBreak);

            //var ret = s;
            //if ("\r\n" != lineBreak) {
            //    ret = ret.Replace("\r\n", lineBreak);
            //}
            //if ("\n" != lineBreak) {
            //    ret = ret.Replace("\n", lineBreak);
            //}
            //if ("\r" != lineBreak) {
            //    ret = ret.Replace("\r", lineBreak);
            //}
            //return ret;
        }
    }
}
