/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Mkamo.Common.IO {
    public static class DirectoryUtil {
        /// <summary>
        /// sourceをtargetにコピーする。
        /// </summary>
        public static void Copy(string source, string target) {
            if (string.Equals(source , target, StringComparison.OrdinalIgnoreCase)) {
                return;
            }

            var sDir = new DirectoryInfo(source);
            var tDir = new DirectoryInfo(target);
            Copy(sDir, tDir);
        }

        public static void Copy(DirectoryInfo sourceDir, DirectoryInfo targetDir) {
            if (sourceDir == null || targetDir == null || string.Equals(sourceDir.FullName, targetDir.FullName, StringComparison.OrdinalIgnoreCase)) {
                throw new ArgumentException("sourceDir or targetDir");
            }

            if (!sourceDir.Exists) {
                throw new ArgumentException("sourceDir");
            }

            if (!targetDir.Exists) {
                targetDir.Create();
                targetDir.Attributes = sourceDir.Attributes;
            }

            var sFiles = sourceDir.GetFiles();
            foreach (var sFile in sFiles) {
                var tFile = new FileInfo(Path.Combine(targetDir.FullName, sFile.Name));
                sFile.CopyTo(tFile.FullName, true);
            }

            var dirs = sourceDir.GetDirectories();
            foreach (var dir in dirs) {
                Copy(dir, new DirectoryInfo(Path.Combine(targetDir.FullName, dir.Name)));
            }
        }
    }
}
