/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Model.Memo;
using System.IO;
using Mkamo.Memopad.Core;

namespace Mkamo.Memopad.Internal.Helpers {
    internal static class MemoFileEditorHelper {
        // ========================================
        // static field
        // ========================================
        public static string GetFullPath(MemoFile memoFile) {
            return memoFile.IsEmbedded?
                Path.Combine(MemopadConsts.EmbeddedFileRoot, memoFile.Path):
                memoFile.Path;
        }

        public static string GetEmbeddedFileDirectoryPath(MemoFile file) {
            return Path.Combine(MemopadConsts.EmbeddedFileRoot, file.EmbeddedId);
        }
    }
}
