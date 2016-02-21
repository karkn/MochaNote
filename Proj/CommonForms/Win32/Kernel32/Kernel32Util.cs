/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32.SafeHandles;
using System.IO;
using Mkamo.Common.String;
using Mkamo.Common.Core;

namespace Mkamo.Common.Win32.Kernel32 {
    public static class Kernel32Util {
        /// <summary>
        /// ファイルが存在する場合は上書き。
        /// </summary>
        public static SafeFileHandle CreateFileForWrite(string path) {
            var handle = Kernel32PI.CreateFile(
                path,
                DesiredAccess.GENERIC_WRITE,
                ShareMode.FILE_SHARE_READ,
                0,
                CreationDisposition.CREATE_ALWAYS,
                FlagsAndAttributes.FILE_ATTRIBUTE_NORMAL,
                IntPtr.Zero
            );
            return new SafeFileHandle(handle, true);
        }

        /// <summary>
        /// ファイルが存在しない場合はIsInvalidなハンドル。
        /// </summary>
        public static SafeFileHandle CreateFileForRead(string path) {
            var handle = Kernel32PI.CreateFile(
                path,
                DesiredAccess.GENERIC_READ,
                ShareMode.FILE_SHARE_READ,
                0,
                CreationDisposition.OPEN_EXISTING,
                FlagsAndAttributes.FILE_ATTRIBUTE_NORMAL,
                IntPtr.Zero
            );
            return new SafeFileHandle(handle, true);
        }

        public static bool SupportsAds(string path) {
            var drive = StringUtil.IsNullOrWhitespace(path) ? null : path.Substring(0, 1);
            if (drive == null) {
                return false;
            }

            var info = new DriveInfo(drive);
            return string.Equals(info.DriveFormat, "NTFS", StringComparison.OrdinalIgnoreCase);
        }

        public static bool HasAds(string path) {
            if (!SupportsAds(path)) {
                return false;
            }
            using (var sh = Kernel32Util.CreateFileForRead(path)) {
                return !sh.IsInvalid;
            }
        }

        public static bool UpdateAds(string path, string data) {
            if (!SupportsAds(path)) {
                return false;
            }

            using (var sh = Kernel32Util.CreateFileForWrite(path)) {
                if (sh.IsInvalid) {
                    return false;
                }
                using (var stream = new StreamWriter(new FileStream(sh, FileAccess.Write))) {
                    stream.Write(data);
                    return true;
                }
            }
        }

        public static string LoadAds(string path) {
            if (!SupportsAds(path)) {
                return null;
            }

            using (var sh = Kernel32Util.CreateFileForRead(path)) {
                if (sh.IsInvalid) {
                    return null;
                }
                using (var stream = new StreamReader(new FileStream(sh, FileAccess.Read))) {
                    return stream.ReadToEnd();
                }
            }
        }
    }
}
