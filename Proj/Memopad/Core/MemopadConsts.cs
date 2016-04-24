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
using System.Configuration;
using System.Drawing;
using Mkamo.Common.IO;
using System.Runtime.Serialization;
using Mkamo.Common.Forms.Drawing;

namespace Mkamo.Memopad.Core {
    public static class MemopadConsts {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        internal static object DataLock = new object();

        public static readonly string AppDataRoot;

        public static readonly string MemoRoot;
        public static readonly string LogRootName = "log";
        public static readonly string LogRoot;
        public static readonly string LockFileName;
        public static readonly string LockFilePath;
        public static readonly BootstrapSettings BootstrapSettings;

        internal static readonly string LicenseFileExtension = ".cnflic2";

        internal static readonly string RegistryRootKey = @"SOFTWARE\mocha\mochanote\3.0";

        internal static readonly string MyDocumentsRootPath;
        internal static readonly string AutoFileImportDirPath;

        internal static readonly bool UseCommandLineMemoRoot;

        /// <summary>
        /// 表示のみに使う。実際の値はTicketServiceで定義。
        /// </summary>
        internal static readonly int UnlicensedMaxMemoCreationCount;
        internal static readonly int UnlicensedMaxTagCreationCount;
        internal static readonly int UnlicensedMaxMemoLoadCount;

        internal static readonly string ConnectionString = @"Data Source={0}; LCID=1033; Max Database Size = 2048; Max Buffer Size = 2048";

        internal static readonly string EmbeddedRootName = "embedded";
        internal static readonly string EmbeddedRoot;

        internal static readonly string MemoSdfFilePath;
        internal static readonly string ExtendedDataSdfFilePath;

        internal static readonly string EmbeddedFileRoot;
        internal static readonly string EmbeddedImageRoot;
        internal static readonly string ProxyRoot;

        internal static readonly string BootstrapSettingsFilePath;
        internal static readonly string SettingsFilePath;
        internal static readonly string WindowSettingsFilePath;
        internal static readonly string BootstrapSettingsV1FilePath;

        internal static readonly string RecentlyClosedMemoIdsFilePath;
        internal static readonly string RecentlyCreatedMemoIdsFilePath;
        internal static readonly string RecentlyModifiedMemoIdsFilePath;

        internal static readonly string FusenMemoIdsFilePath;

        internal static readonly string AbbrevWordDictionaryPath;

        internal static readonly string StartUpShortcutFilePath;

        internal static readonly string ProxyAssemblyName = "Mkamo.Model.Proxies";
        internal static readonly string ProxyAssemblyFilePath;

        internal static readonly string SmtpPasswordEncryptingPassword = "@V8mJ@mUkK~6-(0!";

        internal static readonly string TutorialPath = Path.Combine(Application.StartupPath, @"Help\Tutorial\start_tutorial.html");

#if DEBUG
        internal static readonly string LatestUrl = "http://www.confidante.jp/file/latest2_debug";
#else
        internal static readonly string LatestUrl = "http://www.confidante.jp/file/latest2";
#endif

        /// MemoTextの図形のLocationと一番最初の文字の左上の点との差
        /// MemoTextのPaddingとBlockのMargin/Paddingで値を足したものの符号をマイナスにしたもの
        /// Y軸はBlockのLineSpaceも引いておく
        internal static readonly Size MemoTextFirstCharDelta = new Size(12, 8); /// (10 + 2, 4 + 2 + 2)

        internal static readonly int BlockLineSpace = 2;

        /// <summary>
        /// caret初期位置．
        /// </summary>
        internal static readonly Point DefaultCaretPosition = new Point(8, 16) + MemoTextFirstCharDelta;

        /// <summary>
        /// caret移動量
        /// </summary>
        internal const int CaretMoveDelta = 16;

        /// <summary>
        /// ノート要素間のスペース
        /// </summary>
        internal const int DefaultElementSpace = 16;

        /// <summary>
        /// EditorCanvas -> Form参照のキー。
        /// </summary>
        internal static readonly string FormEditorTransientDataKey = "Form";

        internal static readonly Insets UmlFeaturePadding = new Insets(2, 1, 2, 1);

        // ========================================
        // static method
        // ========================================
        static MemopadConsts() {
            UnlicensedMaxMemoCreationCount = 30;
            UnlicensedMaxMemoLoadCount = 200;
            UnlicensedMaxTagCreationCount = 10;

            var localAppDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            AppDataRoot = Path.Combine(localAppDataFolder, @"mocha\MochaNote\4.0");

            LogRoot = Path.Combine(AppDataRoot, LogRootName);

            BootstrapSettingsFilePath = Path.Combine(AppDataRoot, @"bootstrap.xml");
            BootstrapSettings = BootstrapSettings.LoadBootstrapSettings(BootstrapSettingsFilePath);

            BootstrapSettingsV1FilePath = Path.Combine(localAppDataFolder, @"mocha\MochaNote\1.0\bootstrap.xml");

            var myDocPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            MyDocumentsRootPath = Path.Combine(myDocPath, @"MochaNote");
            AutoFileImportDirPath = Path.Combine(myDocPath, @"MochaNote\Import");

            /// MemoRootの初期化
            var clArgs = System.Environment.GetCommandLineArgs();
            if (clArgs != null && clArgs.Length > 1) {
                /// コマンドライン引数からMemoRoot取得
                MemoRoot = clArgs[1];
                UseCommandLineMemoRoot = true;

            } else {
                /// bootstrap.xmlからMemoRoot取得
                if (string.IsNullOrEmpty(BootstrapSettings.MemoRoot)) {
                    BootstrapSettings.MemoRoot = Path.Combine(AppDataRoot, @"note");
                }
                MemoRoot = BootstrapSettings.MemoRoot;
                UseCommandLineMemoRoot = false;
            }

            LockFileName = "lock";
            LockFilePath = Path.Combine(MemoRoot, LockFileName);

            EmbeddedRoot = Path.Combine(MemoRoot, EmbeddedRootName);
            EmbeddedFileRoot = Path.Combine(EmbeddedRoot, "file");
            EmbeddedImageRoot = Path.Combine(EmbeddedRoot, "image");

            SettingsFilePath = Path.Combine(MemoRoot, "settings.xml");
            WindowSettingsFilePath = Path.Combine(MemoRoot, "window." + Environment.MachineName + ".xml");

            MemoSdfFilePath = Path.Combine(MemoRoot, "note.sdf");
            ExtendedDataSdfFilePath = Path.Combine(MemoRoot, "exdata.sdf");
            
            RecentlyClosedMemoIdsFilePath = Path.Combine(MemoRoot, "recentlyclosed.xml");
            RecentlyCreatedMemoIdsFilePath = Path.Combine(MemoRoot, "recentlycreated.xml");
            RecentlyModifiedMemoIdsFilePath = Path.Combine(MemoRoot, "recentlymodified.xml");

            FusenMemoIdsFilePath = Path.Combine(MemoRoot, "fusen.xml");

            AbbrevWordDictionaryPath = Path.Combine(MemoRoot, "abbrev.txt");

            ProxyRoot = Path.Combine(AppDataRoot, "proxy");
            ProxyAssemblyFilePath = Path.Combine(ProxyRoot, ProxyAssemblyName + ".dll");

            StartUpShortcutFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Startup),
                "MochaNote.lnk"
            );
        }

    }
}
