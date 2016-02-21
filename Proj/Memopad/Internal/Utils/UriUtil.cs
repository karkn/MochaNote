/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Memopad.Core;
using Mkamo.Memopad.Internal.Core;

namespace Mkamo.Memopad.Internal.Utils {
    internal static class UriUtil {
        // ========================================
        // method
        // ========================================
        public static bool IsHttpUri(string uri) {
            return !string.IsNullOrEmpty(uri) && uri.StartsWith("http://");
        }

        public static bool IsFileUri(string uri) {
            return !string.IsNullOrEmpty(uri) && uri.StartsWith("file://");
        }

        public static bool IsMemoUri(string uri) {
            return !string.IsNullOrEmpty(uri) && uri.StartsWith("memo://");
        }

        public static MemoInfo GetMemoInfo(string uri) {
            if (string.IsNullOrEmpty(uri) || !uri.StartsWith("memo:///")) {
                return null;
            }

            var memoId = uri.Substring("memo:///".Length);
            return MemopadApplication.Instance.FindMemoInfoByMemoId(memoId);
        }

        public static string GetUri(MemoInfo info) {
            return "memo:///" + info.MemoId;
        }
    }
}
