/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Crypto;
using Mkamo.Common.String;
using Mkamo.Common.DataType;
using Mkamo.Memopad.Core;
using Mkamo.Common.Core;

namespace Mkamo.Memopad.Internal.Utils {
    internal static class CryptoUtil {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        // ========================================
        // static method
        // ========================================
        internal static string EncryptBoolAndVersion(bool b, Version version, string pass) {
            var s = b.ToString() + "#" + version.ToString();
            return AesUtil.EncryptString(s, pass);
        }

        internal static Tuple<bool, Version> DecryptBoolAndVersion(string encrypted, string pass, Tuple<bool, Version> defaultValue) {
            if (StringUtil.IsNullOrWhitespace(encrypted)) {
                return defaultValue;
            }

            var decrypted = string.Empty;
            try {
                decrypted = AesUtil.DecryptString(encrypted, pass);
                var strs = decrypted.Split('#');
                if (strs == null || strs.Length != 2) {
                    return defaultValue;
                }

                return Tuple.Create(bool.Parse(strs[0]), new Version(strs[1]));

            } catch (Exception e) {
                Logger.Warn("Decrypt bool and version failed", e);
                return defaultValue;
            }
        }

        internal static string EncryptDateAndCount(DateTime date, int count, string pass) {
            var s = date.ToString("yyyy/MM/dd") + "#" + count.ToString();
            return AesUtil.EncryptString(s, pass);
        }

        /// <summary>
        /// Item1に日付，Item2にcountを格納して返す。
        /// encryptedが想定外の値ならItem1にはtoday，Item2には0を格納。
        /// </summary>
        internal static Tuple<DateTime, int> DecryptDateAndCount(string encrypted, DateTime today, string pass) {
            try {
                if (StringUtil.IsNullOrWhitespace(encrypted)) {
                    return Tuple.Create(today, 0);
                }

                var decrypted = string.Empty;
                try {
                    decrypted = AesUtil.DecryptString(encrypted, pass);
                } catch (Exception e) {
                    Logger.Warn("Can't decrypt date and count", e);
                    return Tuple.Create(today, 0);
                }

                var strs = decrypted.Split('#');
                if (strs == null || strs.Length != 2) {
                    return Tuple.Create(today, 0);
                }

                var date = default(DateTime);
                if (!DateTime.TryParse(strs[0], out date)) {
                    return Tuple.Create(today, 0);
                }

                var count = 0;
                if (!int.TryParse(strs[1], out count)) {
                    return Tuple.Create(today, 0);
                }

                return Tuple.Create(date, count);

            } catch (Exception) {
                return Tuple.Create(today, 0);
            }
        }


        internal static string EncryptNameAndValues(IDictionary<string, string> dict, string pass) {
            var buf = new StringBuilder();
            foreach (var pair in dict) {
                buf.Append(pair.Key + "=" + pair.Value + ";");
            }

            return AesUtil.EncryptString(buf.ToString(), pass);
        }

        internal static IDictionary<string, string> DecryptNameAndValues(string encrypted, string pass) {
            var ret = new Dictionary<string, string>();

            var dec = string.Empty;
            try {
                dec = AesUtil.DecryptString(encrypted, pass);
            } catch (Exception e) {
                Logger.Warn("Can't decrypt names and values", e);
                return ret;
            }
            var nvs = dec.Split(new[] { ";", }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var nv in nvs) {
                var strs = nv.Split(new[] { "=", }, StringSplitOptions.RemoveEmptyEntries);
                if (strs != null && strs.Length == 2) {
                    ret.Add(strs[0], strs[1]);
                }
            }

            return ret;
        }

        internal static string GetMemoInfosPassword(MemoInfoCollection memoInfos, string defaultPassword) {
            if (memoInfos.Count == 0) {
                return defaultPassword;
            } else if (memoInfos.Count == 1) {
                return GetHash(memoInfos[0].MemoId);

            } else if (memoInfos.Count == 2) {
                return GetHash(memoInfos[0].MemoId + memoInfos[1].MemoId);

            } else {
                /// memoInfos.Count >= 3

                var rand = new Random(memoInfos.Count);
                var buf = new StringBuilder();
                for (int i = 0; i < 3; ++i) {
                    var r = rand.Next(memoInfos.Count);
                    var info = memoInfos[r];
                    buf.Append(info.MemoId);
                }

                return GetHash(buf.ToString());
            }
        }

        internal static string DecryptSmtpPassword(string encrypted) {
            try {
                return AesUtil.DecryptString(encrypted, MemopadConsts.SmtpPasswordEncryptingPassword);
            } catch (Exception e) {
                Logger.Warn("Smtp password decryption failed.", e);
                return "";
            }
        }

        internal static string EncryptSmtpPassword(string raw) {
            try {
                return AesUtil.EncryptString(raw, MemopadConsts.SmtpPasswordEncryptingPassword);
            } catch (Exception e) {
                Logger.Warn("Smtp password encryption failed.", e);
                return "";
            }
        }



        private static string GetHash(string s) {
            var data = Encoding.UTF8.GetBytes(s);
            using (var md5 = System.Security.Cryptography.MD5.Create()) {
                var bs = md5.ComputeHash(data);
                var ret = new StringBuilder();
                foreach (byte b in bs) {
                    ret.Append(b.ToString("x2"));
                }
                return ret.ToString();
            }
        }
    }
}
