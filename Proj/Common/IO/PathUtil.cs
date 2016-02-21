/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Mkamo.Common.IO {
    public static class PathUtil {
        public static bool IsValidPath(string path) {
            if (path == null) {
                return false;
            }
            return path.IndexOfAny(Path.GetInvalidPathChars()) < 0;
        }

        public static string GetUniqueFilePathByCounting(string path) {
            var ret = path;
            var i = 2;
            while (File.Exists(ret)) {
                ret = path + "_" + i.ToString();
                ++i;
            }
            return ret;
        }

        public static string GetUniqueDirectoryPathByCounting(string path) {
            var ret = path;
            var i = 2;
            while (Directory.Exists(ret)) {
                ret = path + "_" + i.ToString();
                ++i;
            }
            return ret;
        }

        public static string GetValidFilename(string path, string replacing) {
            var ret = new StringBuilder();

            var invalids = Path.GetInvalidFileNameChars();
            foreach (var ch in path) {
                if (Array.Exists(invalids, c => c == ch)) {
                    ret.Append(replacing);
                } else {
                    ret.Append(ch);
                }
            }

            return ret.ToString();
        }

        public static string GetValidRelativePath(string path, string replacing) {
            var ret = new StringBuilder();

            var invalids = Path.GetInvalidFileNameChars();
            var dirSep = Path.DirectorySeparatorChar;
            var altDirSep = Path.AltDirectorySeparatorChar;
            //var pathSep = Path.PathSeparator;
            //var volSep = Path.VolumeSeparatorChar;

            foreach (var ch in path) {
                if (ch != dirSep && ch != altDirSep && Array.Exists(invalids, c => c == ch)) {
                    ret.Append(replacing);
                } else {
                    ret.Append(ch);
                }
            }

            return ret.ToString();
        }

        //public static string GetValidPath(string path, string replacing) {
        //    var ret = new StringBuilder();

        //    //var invalids = Path.GetInvalidPathChars();
        //    var invalids = Path.GetInvalidFileNameChars();
        //    var dirSep = Path.DirectorySeparatorChar;
        //    var altDirSep = Path.AltDirectorySeparatorChar;
        //    var pathSep = Path.PathSeparator;
        //    var volSep = Path.VolumeSeparatorChar;

        //    foreach (var ch in path) {
        //        if (ch != dirSep && ch != altDirSep && ch != pathSep && ch != volSep && Array.Exists(invalids, c => c == ch)) {
        //            ret.Append(replacing);
        //        } else {
        //            ret.Append(ch);
        //        }
        //    }

        //    return ret.ToString();
        //}

        public static void EnsureDirectoryExists(string path) {
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
        }

    }
}
