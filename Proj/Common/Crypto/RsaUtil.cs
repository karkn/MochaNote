/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Mkamo.Common.Crypto {
    public static class RsaUtil {
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
        public static void CreateKeys(out string publicKey, out string privateKey) {
            var rsa = new RSACryptoServiceProvider();
            publicKey = rsa.ToXmlString(false);
            privateKey = rsa.ToXmlString(true);
        }

        public static string Encrypt(string str, string publicKey) {
            var rsa = new RSACryptoServiceProvider();

            rsa.FromXmlString(publicKey);

            var data = System.Text.Encoding.UTF8.GetBytes(str);
            var encryptedData = rsa.Encrypt(data, false);

            return Convert.ToBase64String(encryptedData);
        }

        public static string Decrypt(string str, string privateKey) {
            var rsa = new RSACryptoServiceProvider();

            rsa.FromXmlString(privateKey);

            var data = System.Convert.FromBase64String(str);
            var decryptedData = rsa.Decrypt(data, false);
            return Encoding.UTF8.GetString(decryptedData);
        }
    }
}
