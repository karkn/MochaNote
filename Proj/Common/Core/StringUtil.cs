/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.DataType;
using System.Text.RegularExpressions;
using System.IO;

namespace Mkamo.Common.Core {
    public static class StringUtil {
        // ========================================
        // static method
        // ========================================
        public static string With(this string s, params object[] args) {
            return string.Format(s, args);
        }

        public static bool IsNullOrWhitespace(this string s) {
            var empty = string.IsNullOrEmpty(s);
            if (empty) {
                return true;
            } else {
                foreach (char ch in s) {
                    if (!char.IsWhiteSpace(ch)) {
                        return false;
                    }
                }
                return true;
            }
        }

        public static bool IsNullOrDefault(this string s, string defaultValue) {
            return string.IsNullOrEmpty(s) || s == defaultValue;
        }

        public static string Repeat(this string s, int count) {
            var ret = new StringBuilder();
            for (int i = 0; i < count; ++i) {
                ret.Append(s);
            }
            return ret.ToString();
        }

        /// <summary>
        /// sの頭maxLineCount分の文字列を返す。空白行は無視する。
        /// </summary>
        public static string GetHead(this string s, int maxLineCount) {
            if (string.IsNullOrEmpty(s)) {
                return string.Empty;
            }

            var ret = new StringBuilder();
            var cLineCount = 0;
            var lines = s.Split(new[] { '\r', '\n', }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines) {
                if (!IsNullOrWhitespace(line)) {
                    ret.AppendLine(line);
                    ++cLineCount;
                }
                if (cLineCount >= maxLineCount) {
                    break;
                }
            }

            return ret.ToString();
        }


        public static Range GetUrlRange(string s) {
            //var r = new Regex(@"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
            var r = new Regex(@"http(s)?://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?"); /// スペースはだめにしておく
            var m = r.Match(s);
            if (m.Success) {
                return Range.FromStartAndLength(m.Index, m.Length);
            } else {
                return Range.Empty;
            }
        }

        public static bool IsUrl(string s) {
            //var r = new Regex(@"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
            var r = new Regex(@"http(s)?://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?"); /// スペースはだめにしておく
            var m = r.Match(s);
            if (m.Success) {
                return m.Index == 0 && m.Length == s.Length;
            } else {
                return false;
            }
        }

        public static IEnumerable<Range> GetUrlRanges(string s) {
            //var r = new Regex(@"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
            var r = new Regex(@"http(s)?://([\w-]+\.)+[\w-]+(/[\w-./?%&=]*)?"); /// スペースはだめにしておく
            var ms = r.Matches(s);
            if (ms.Count > 0) {
                var ret = new List<Range>();
                foreach (Match m in ms) {
                    ret.Add(Range.FromStartAndLength(m.Index, m.Length));
                }
                return ret;
                //return new Range(m.Index, m.Length);
            } else {
                return new Range[0];
            }
        }


        /// <summary>
        /// sのindex番目の文字を含むwordの範囲を返す．
        /// </summary>
        public static Range GetWordRange(string s, int index) {
            var ch = s[index];
            var kind = GetCharKind(ch);

            switch (kind) {
                case CharKind.Control:
                    return Range.FromStartAndLength(index, 1);

                case CharKind.Whitespace: {
                    /// whitepace全体と直前のwhitespace以外のwordを含める
                    var start = GetBackwardWordBound(s, index);
                    if (start > 0) {
                        start = GetBackwardWordBound(s, start - 1);
                    }
                    var end = GetForwardWordBound(s, index);
                    return Range.FromStartAndEnd(start, end);
                }

                case CharKind.Punctuation: {
                    /// punc全体と次のwordがWhitespaceのときのみそのwordを含める
                    var start = GetBackwardWordBound(s, index);
                    var end = GetForwardWordBound(s, index);
                    if (end < s.Length - 1) {
                        if (GetCharKind(s[end + 1]) == CharKind.Whitespace) {
                            end = GetForwardWordBound(s, end + 1);
                        }
                    }
                    return Range.FromStartAndEnd(start, end);
                }

                case CharKind.Digit:
                case CharKind.Letter:
                case CharKind.Number:
                case CharKind.Hiragana:
                case CharKind.Katakana:
                case CharKind.Kanji: {
                    /// wordを選択
                    var start = GetBackwardWordBound(s, index);
                    var end = GetForwardWordBound(s, index);
                    return Range.FromStartAndEnd(start, end);
                }

                case CharKind.Others: {
                    /// 同じ文字が続く分だけwordとして選択
                    var start = GetBackwardSameCharBound(s, index);
                    var end = GetForwardSameCharBound(s, index);
                    return Range.FromStartAndEnd(start, end);
                }
            }

            return Range.FromStartAndLength(index, 1);
        }

        public static int GetBackwardSameCharBound(string s, int index) {
            var ch = s[index];
            
            var i = index;
            var c = ch;
            while (i > 0 && c == ch) {
                --i;
                c = s[i];
            }

            return (i == 0 && c == ch)? i: i + 1;
        }

        public static int GetForwardSameCharBound(string s, int index) {
            var len = s.Length;
            var ch = s[index];
            
            var i = index;
            var c = ch;
            while (i < len - 1 && c == ch) {
                ++i;
                c = s[i];
            }

            return (i == len - 1 && c == ch)? i: i - 1;
        }

        public static int GetBackwardWordBound(string s, int index) {
            var ch = s[index];
            var kind = GetCharKind(ch);
            
            var i = index;
            var k = kind;
            while (i > 0 && k == kind) {
                --i;
                k = GetCharKind(s[i]);
            }

            return (i == 0 && k == kind)? i: i + 1;
        }

        public static int GetForwardWordBound(string s, int index) {
            var len = s.Length;
            var ch = s[index];
            var kind = GetCharKind(ch);
            
            var i = index;
            var k = kind;
            while (i < len - 1 && k == kind) {
                ++i;
                k = GetCharKind(s[i]);
            }

            return (i == len - 1 && k == kind)? i: i - 1;
        }

        public static CharKind GetCharKind(char ch) {
            if (char.IsWhiteSpace(ch)) {
                return CharKind.Whitespace;
            } else if (char.IsPunctuation(ch)) {
                return CharKind.Punctuation;

            } else if (char.IsControl(ch)) {
                return CharKind.Control;

            } else if (char.IsDigit(ch)) {
                return CharKind.Digit;
            } else if (char.IsNumber(ch)) {
                return CharKind.Number;

            } else if(ch >= 0x3040 && ch<= 0x309f) {
                return CharKind.Hiragana;
            } else if(ch >= 0x30a0 && ch<= 0x30ff) {
                return CharKind.Katakana;
            } else if (ch >= 0x4e00 && ch <= 0x9fff) {
                return CharKind.Kanji;
            } else if (char.IsLetter(ch)) {
                return CharKind.Letter;


            } else {
                return CharKind.Others;
            }            
        }

        /// <summary>
        /// 空行や最後の行も残る。
        /// </summary>
        public static string[] SplitLines(string s) {
            return Regex.Split(s, "\\r\\n|\\r|\\n");
        }

        public static string ReadAllText(string filePath) {
            var bytes = File.ReadAllBytes(filePath);
            var enc = StringUtil.GetEncoding(bytes);
            return enc.GetString(bytes);
        }

        public static Encoding GetEncoding(byte[] bytes) {
            return GetEncodingInternal(bytes);
        }

        private static Encoding GetEncodingInternal(byte[] bytes) {
            /// http://dobon.net/vb/dotnet/string/detectcode.html
            const byte bEscape = 0x1B;
            const byte bAt = 0x40;
            const byte bDollar = 0x24;
            const byte bAnd = 0x26;
            const byte bOpen = 0x28;    //'('
            const byte bB = 0x42;
            const byte bD = 0x44;
            const byte bJ = 0x4A;
            const byte bI = 0x49;
    
            int len = bytes.Length;
            byte b1, b2, b3, b4;

            //Encode::is_utf8 は無視

            bool isBinary = false;
            for (int i = 0; i < len; ++i) {
                b1 = bytes[i];
                if (b1 <= 0x06 || b1 == 0x7F || b1 == 0xFF) {
                    //'binary'
                    isBinary = true;
                    if (b1 == 0x00 && i < len - 1 && bytes[i + 1] <= 0x7F) {
                        //smells like raw unicode
                        return System.Text.Encoding.Unicode;
                    }
                }
            }
            if (isBinary) {
                return null;
            }

            //not Japanese
            bool notJapanese = true;
            for (int i = 0; i < len; ++i) {
                b1 = bytes[i];
                if (b1 == bEscape || 0x80 <= b1) {
                    notJapanese = false;
                    break;
                }
            }
            if (notJapanese) {
                return System.Text.Encoding.ASCII;
            }

            for (int i = 0; i < len - 2; ++i) {
                b1 = bytes[i];
                b2 = bytes[i + 1];
                b3 = bytes[i + 2];

                if (b1 == bEscape) {
                    if (b2 == bDollar && b3 == bAt) {
                        //JIS_0208 1978
                        //JIS
                        return System.Text.Encoding.GetEncoding(50220);
                    } else if (b2 == bDollar && b3 == bB) {
                        //JIS_0208 1983
                        //JIS
                        return System.Text.Encoding.GetEncoding(50220);
                    } else if (b2 == bOpen && (b3 == bB || b3 == bJ)) {
                        //JIS_ASC
                        //JIS
                        return System.Text.Encoding.GetEncoding(50220);
                    } else if (b2 == bOpen && b3 == bI) {
                        //JIS_KANA
                        //JIS
                        return System.Text.Encoding.GetEncoding(50220);
                    }
                    if (i < len - 3) {
                        b4 = bytes[i + 3];
                        if (b2 == bDollar && b3 == bOpen && b4 == bD) {
                            //JIS_0212
                            //JIS
                            return System.Text.Encoding.GetEncoding(50220);
                        }
                        if (i < len - 5 &&
                            b2 == bAnd && b3 == bAt && b4 == bEscape &&
                            bytes[i + 4] == bDollar && bytes[i + 5] == bB) {
                            //JIS_0208 1990
                            //JIS
                            return System.Text.Encoding.GetEncoding(50220);
                        }
                    }
                }
            }

            //should be euc|sjis|utf8
            //use of (?:) by Hiroki Ohzaki <ohzaki@iod.ricoh.co.jp>
            int sjis = 0;
            int euc = 0;
            int utf8 = 0;
            for (int i = 0; i < len - 1; ++i) {
                b1 = bytes[i];
                b2 = bytes[i + 1];
                if (((0x81 <= b1 && b1 <= 0x9F) || (0xE0 <= b1 && b1 <= 0xFC)) &&
                    ((0x40 <= b2 && b2 <= 0x7E) || (0x80 <= b2 && b2 <= 0xFC))) {
                    //SJIS_C
                    sjis += 2;
                    ++i;
                }
            }
            for (int i = 0; i < len - 1; ++i) {
                b1 = bytes[i];
                b2 = bytes[i + 1];
                if (((0xA1 <= b1 && b1 <= 0xFE) && (0xA1 <= b2 && b2 <= 0xFE)) ||
                    (b1 == 0x8E && (0xA1 <= b2 && b2 <= 0xDF))) {
                    //EUC_C
                    //EUC_KANA
                    euc += 2;
                    ++i;
                } else if (i < len - 2) {
                    b3 = bytes[i + 2];
                    if (b1 == 0x8F && (0xA1 <= b2 && b2 <= 0xFE) &&
                        (0xA1 <= b3 && b3 <= 0xFE)) {
                        //EUC_0212
                        euc += 3;
                        i += 2;
                    }
                }
            }
            for (int i = 0; i < len - 1; ++i) {
                b1 = bytes[i];
                b2 = bytes[i + 1];
                if ((0xC0 <= b1 && b1 <= 0xDF) && (0x80 <= b2 && b2 <= 0xBF)) {
                    //UTF8
                    utf8 += 2;
                    ++i;
                } else if (i < len - 2) {
                    b3 = bytes[i + 2];
                    if ((0xE0 <= b1 && b1 <= 0xEF) && (0x80 <= b2 && b2 <= 0xBF) &&
                        (0x80 <= b3 && b3 <= 0xBF)) {
                        //UTF8
                        utf8 += 3;
                        i += 2;
                    }
                }
            }
            //M. Takahashi's suggestion
            //utf8 += utf8 / 2;

            System.Diagnostics.Debug.WriteLine(
                string.Format("sjis = {0}, euc = {1}, utf8 = {2}", sjis, euc, utf8));
            if (euc > sjis && euc > utf8) {
                //EUC
                return System.Text.Encoding.GetEncoding(51932);
            } else if (sjis > euc && sjis > utf8) {
                //SJIS
                return System.Text.Encoding.GetEncoding(932);
            } else if (utf8 > euc && utf8 > sjis) {
                //UTF8
                return System.Text.Encoding.UTF8;
            }

            return null;
        }
    }

    [Serializable]
    public enum CharKind {
        Control,

        Whitespace,
        Punctuation,

        Digit,
        Letter,

        Number,
        Hiragana,
        Katakana,
        Kanji,

        Others,
    }
}
