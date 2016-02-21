/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Shell32;
using System.IO;
using Mkamo.Common.Diagnostics;

namespace Mkamo.Memopad.Internal.Utils {
    internal class ShortcutUtil {
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
        public static bool CreateShortcut(string shortcutPath, string targetPath) {
            Contract.Requires(!string.IsNullOrEmpty(shortcutPath), "shortcutPath");
            Contract.Requires(!string.IsNullOrEmpty(targetPath), "targetPath");
            if (!shortcutPath.EndsWith(".lnk")) {
                return false;
            }


            if (File.Exists(shortcutPath) || Directory.Exists(shortcutPath)) {
                return false;
            }

            return false;

            /*            var shell = new ShellClass();

                        using (var stream = File.Create(shortcutPath)) {
                            stream.Close();
                        }

                        var folder = shell.NameSpace(Path.GetDirectoryName(shortcutPath));
                        var folderItem = folder.ParseName(Path.GetFileName(shortcutPath));

                        if (folderItem.IsLink) {
                            var shortcut = folderItem.GetLink as ShellLinkObject;
                            shortcut.Path = targetPath;
                            shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);
                            shortcut.SetIconLocation(targetPath, 0);
                            shortcut.Save(null);
                        }

                        return true;*/
        }
    }
}
