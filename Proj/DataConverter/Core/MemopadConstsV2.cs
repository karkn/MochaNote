/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Deployment.Application;
using System.Configuration;
using System.Drawing;
using Mkamo.Common.IO;
using System.Runtime.Serialization;
using Mkamo.Memopad.Core;

namespace Mkamo.DataConverter.Core {
    public static class MemopadConstsV2 {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static readonly string MemoRoot;
        public static readonly string LogRootName = "log";
        public static readonly string LogRoot;
        public static readonly string LockFileName;
        public static readonly string LockFilePath;
        public static readonly BootstrapSettings BootstrapSettings;

        internal static readonly string EmbeddedRootName = "embedded";
        internal static readonly string EmbeddedRoot;
        internal static readonly string MemoFileName = "note.sdf";
        internal static readonly string MemoFilePath;
        internal static readonly string ExtendedDataFileName= "exdata.sdf";
        internal static readonly string ExtendedDataFilePath;

        internal static readonly string EmbeddedFileRoot;
        internal static readonly string EmbeddedImageRoot;

        internal static readonly string BootstrapSettingsFilePath;
        internal static readonly string SettingsFilePath;
        internal static readonly string WindowSettingsFilePath;

        internal static readonly string RecentlyClosedMemoIdsFilePath;
        internal static readonly string RecentlyCreatedMemoIdsFilePath;
        internal static readonly string RecentlyModifiedMemoIdsFilePath;


        // ========================================
        // static method
        // ========================================
        static MemopadConstsV2() {
            var localAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appDataRoot = Path.Combine(localAppDataFolder, @"mocha\MochaNote\2.0");

            BootstrapSettingsFilePath = Path.Combine(appDataRoot, @"bootstrap.xml");
            BootstrapSettings = BootstrapSettings.LoadBootstrapSettings(BootstrapSettingsFilePath);


            /// MemoRootの初期化
            /// bootstrap.xmlからMemoRoot取得
            if (string.IsNullOrEmpty(BootstrapSettings.MemoRoot)) {
                BootstrapSettings.MemoRoot = Path.Combine(appDataRoot, @"note");
            }
            MemoRoot = BootstrapSettings.MemoRoot;

            try {
                PathUtil.EnsureDirectoryExists(MemoRoot);
            } catch (Exception e) {
                Logger.Error("Can't create MemoRoot folder", e);
                try {
                    /// デフォルトの設定でやり直す
                    BootstrapSettings.MemoRoot = Path.Combine(appDataRoot, @"note");
                    MemoRoot = BootstrapSettings.MemoRoot;
                    //UseCommandLineMemoRoot = false;
                    PathUtil.EnsureDirectoryExists(MemoRoot);
                } catch (Exception ex) {
                    Logger.Error("Can't create Default MemoRoot folder", ex);
                    throw;
                }
            }

            LockFileName = "lock";
            LockFilePath = Path.Combine(MemoRoot, LockFileName);

            LogRoot = Path.Combine(MemoRoot, LogRootName);
            EmbeddedRoot = Path.Combine(MemoRoot, EmbeddedRootName);
            EmbeddedFileRoot = Path.Combine(EmbeddedRoot, "file");
            EmbeddedImageRoot = Path.Combine(EmbeddedRoot, "image");
            MemoFilePath = Path.Combine(MemoRoot, MemoFileName);
            ExtendedDataFilePath = Path.Combine(MemoRoot, ExtendedDataFileName);

            SettingsFilePath = Path.Combine(MemoRoot, "settings.xml");
            WindowSettingsFilePath = Path.Combine(MemoRoot, "window." + Environment.MachineName + ".xml");

            RecentlyClosedMemoIdsFilePath = Path.Combine(MemoRoot, "recentlyclosed.xml");
            RecentlyCreatedMemoIdsFilePath = Path.Combine(MemoRoot, "recentlycreated.xml");
            RecentlyModifiedMemoIdsFilePath = Path.Combine(MemoRoot, "recentlymodified.xml");
        }

    }
}
