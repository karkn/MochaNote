/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mkamo.Common.Crypto {
    public static class AesUtil {
        // ========================================
        // static field
        // ========================================
        public static string EncryptString(string str, string password) {
            var aes = new System.Security.Cryptography.AesCryptoServiceProvider();

            var data = System.Text.Encoding.UTF8.GetBytes(str);
        
            var passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
            aes.Key = ResizeBytesArray(passwordBytes, aes.Key.Length);
            aes.IV = ResizeBytesArray(passwordBytes, aes.IV.Length);

            var stream = new System.IO.MemoryStream();
            var encryptor = aes.CreateEncryptor();
            var cryptStream = new System.Security.Cryptography.CryptoStream(
                stream,
                encryptor,
                System.Security.Cryptography.CryptoStreamMode.Write
            );
            try {
                cryptStream.Write(data, 0, data.Length);
                cryptStream.FlushFinalBlock();
                var encrypted = stream.ToArray();
                return System.Convert.ToBase64String(encrypted);

            } finally {
                cryptStream.Close();
                stream.Close();
            }
        }
        
        /// <summary>
        /// 暗号化された文字列を復号化する
        /// </summary>
        public static string DecryptString(string str, string password) {
            var aes = new System.Security.Cryptography.AesCryptoServiceProvider();

            var passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
            aes.Key = ResizeBytesArray(passwordBytes, aes.Key.Length);
            aes.IV = ResizeBytesArray(passwordBytes, aes.IV.Length);
        
            var data = System.Convert.FromBase64String(str);
            var stream = new System.IO.MemoryStream(data);
            var decryptor = aes.CreateDecryptor();
            var cryptStreem = new System.Security.Cryptography.CryptoStream(
                stream,
                decryptor,
                System.Security.Cryptography.CryptoStreamMode.Read
            );
            var reader = new System.IO.StreamReader(
                cryptStreem, 
                System.Text.Encoding.UTF8
            );
            try {
                var result = reader.ReadToEnd();
                return result;
            } finally {
                reader.Close();
                cryptStreem.Close();
                stream.Close();
            }
        }
        
        /// <summary>
        /// 共有キー用に、バイト配列のサイズを変更する
        /// </summary>
        private static byte[] ResizeBytesArray(byte[] bytes, int newSize) {
            var ret = new byte[newSize];
            if (bytes.Length <= newSize) {
                for (int i = 0; i < bytes.Length; ++i) {
                    ret[i] = bytes[i];
                }
            } else {
                int pos = 0;
                for (int i = 0; i < bytes.Length; ++i) {
                    ret[pos] ^= bytes[i];
                    ++pos;
                    if (pos >= ret.Length) {
                        pos = 0;
                    }
                }
            }
            return ret;
        }
    }
}
