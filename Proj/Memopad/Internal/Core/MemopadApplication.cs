/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Memopad.Core;
using System.IO;
using Mkamo.Container.Core;
using System.Windows.Forms;
using Mkamo.Common.DataType;
using Mkamo.Editor.Core;
using Mkamo.Memopad.Internal.Controls;
using System.Runtime.Serialization.Formatters.Binary;
using Mkamo.Common.Externalize;
using Mkamo.Model.Memo;
using Mkamo.Common.Event;
using System.ComponentModel;
using Mkamo.Memopad.Internal.Forms;
using System.Xml.Serialization;
using Mkamo.Model.Core;
using System.Drawing;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Common.Forms.Themes;
using Mkamo.Control.Progress;
using System.Threading;
using log4net.Config;
using System.Runtime.Serialization;
using System.Xml;
using Mkamo.Memopad.Internal.KeySchemes;
using Mkamo.Memopad.Internal.Helpers;
using Mkamo.Memopad.Internal.Utils;
using ComponentFactory.Krypton.Toolkit;
using System.Reflection;
using System.Drawing.Imaging;
using System.Diagnostics;
using Mkamo.Common.Core;
using Mkamo.Common.Win32.User32;
using Mkamo.Common.Util;
using Mkamo.Common.Forms.Clipboard;
using Mkamo.Common.String;
using Mkamo.Common.Forms.ScreenCapture;
using System.Net.NetworkInformation;
using Mkamo.Control.HotKey;
using Mkamo.Common.Forms.Message;
using Mkamo.Figure.Figures;

namespace Mkamo.Memopad.Internal.Core {
    /// <summary>
    /// MemopadForm，MemoInfoCollectionの管理．
    /// </summary>
    internal class MemopadApplication {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(global::System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// 呼び出し順序の関係で
        ///   internal static readonly MemopadFacade Instance = new MemopadFacade();
        /// はだめ．
        private static MemopadApplication _instance;
        internal static MemopadApplication Instance {
            get { return _instance?? (_instance = new MemopadApplication()); }
        }


        // ========================================
        // field
        // ========================================
        private object _lock;

        private MemopadAppContext _context;
        private SqlServerAccessor _memoAccessor;

        private MemopadForm _mainForm;
        private FusenManager _fusenManager;

        private MemoInfoCollection _memoInfos;
        private MemoInfoCollection _removedMemoInfos;

        private Lazy<MemoIdCollection> _recentlyClosedMemoIds;
        private Lazy<MemoIdCollection> _recentlyCreatedMemoIds;
        private Lazy<MemoIdCollection> _recentlyModifiedMemoIds;

        private Lazy<MemoIdCollection> _removedEmbeddedFileIds;
        private Lazy<MemoIdCollection> _removedEmbeddedImageIds;

        private Lazy<AbbrevWordPersister> _abbrevWordPersister;

        private MemopadSettings _settings;
        private MemopadWindowSettings _windowSettings;

        private IKeyScheme _keySchema;

        private MemoFolder _activeFolder;
        private MemoSmartFilter _activeSmartFilter;

        private IEntityContainer _container;
        private Workspace _workspace;
        private ITheme _theme;

        private bool _needRecoverMemoInfos;
        private bool _illegalSettings;

        /// <summary>
        /// restore時はSaveされると困るのでそれを防ぐ。
        /// </summary>
        private bool _preventSaveAll;

        private BackupExecutor _backupExecutor;

        private AutoFileImporter _autoFileImporter;

        // --- supplementary ui ---
        private KryptonPalette _kryptonPalette;
        private KryptonManager _kryptonManager;
        private HotKey _hotKey;
        private MemopadNotifyIcon _notifyIcon;

        private bool _inCaptureScreen = false;

        private ToolStripRenderer _miniToolBarRenderer;

        // ========================================
        // constructor
        // ========================================
        internal MemopadApplication() {
            _lock = new object();
            _preventSaveAll = false;

            /// design時にも動くようにsurelyを使う
            _container = ContainerFactory.GetContainerSurely();

            _workspace = new Workspace(_container);

            _container.EntityPersisted += HandleEntityPersisted;
            _container.EntityRemoving += HandleEntityRemoving;
            _workspace.MemoFolderRemoving += HandleMemoFolderRemoving;
            _workspace.MemoSmartFilterRemoving += HandleMemoSmartFilterRemoving;

            _illegalSettings = false;

            _needRecoverMemoInfos = false;
        }

        /// <summary>
        /// MemopadAppContextから呼ばれる。
        /// </summary>
        internal void Init(MemopadAppContext context) {
            var currentVersion = new Version("4.0.0");

            _context = context;
            _context.ThreadExit += (sender, ev) => {
                if (_notifyIcon != null) {
                    _notifyIcon.Dispose();
                }
                if (_hotKey != null) {
                    _hotKey.RemoveAllHotKeys();
                }
                if (_autoFileImporter != null) {
                    _autoFileImporter.Dispose();
                }
                if (_backupExecutor != null) {
                    _backupExecutor.Dispose();
                }
            };

            _memoAccessor = new SqlServerAccessor(_context.MemoConnection);
            try {
                _memoInfos = MemoInfo.LoadMemoInfos(_memoAccessor);
                foreach (var info in _memoInfos) {
                    info.PropertyChanged += HandleMemoInfoPropChanged;
                }
            } catch (Exception) {
                _needRecoverMemoInfos = true;
            }

            if (!_needRecoverMemoInfos) {
                _fusenManager = new FusenManager();

                _settings = MemopadSettings.LoadSettings();
                _windowSettings = MemopadWindowSettings.LoadWindowSettings();
                InitSettings(currentVersion, MemopadConsts.BootstrapSettings, _settings, _windowSettings);

                switch (_settings.KeyScheme) {
                    case KeySchemeKind.Emacs:
                        _keySchema = new EmacsKeyScheme();
                        break;
                    default:
                        _keySchema = new DefaultKeyScheme();
                        break;
                }

                _recentlyClosedMemoIds = new Lazy<MemoIdCollection>(
                    () => MemoIdCollection.LoadIdsFromFile(MemopadConsts.RecentlyClosedMemoIdsFilePath)
                );
                _recentlyCreatedMemoIds = new Lazy<MemoIdCollection>(
                    () => MemoIdCollection.LoadIdsFromFile(MemopadConsts.RecentlyCreatedMemoIdsFilePath)
                );
                _recentlyModifiedMemoIds = new Lazy<MemoIdCollection>(
                    () => MemoIdCollection.LoadIdsFromFile(MemopadConsts.RecentlyModifiedMemoIdsFilePath)
                );
                
                _removedMemoInfos = MemoInfo.LoadRemovedMemoInfos(_memoAccessor);

                _removedEmbeddedFileIds = new Lazy<MemoIdCollection>(
                    () => MemoIdCollection.LoadIdsFromSdf("RemovedEmbeddedFileId", _memoAccessor)
                );
                _removedEmbeddedImageIds = new Lazy<MemoIdCollection>(
                    () => MemoIdCollection.LoadIdsFromSdf("RemovedEmbeddedImageId", _memoAccessor)
                );

                _abbrevWordPersister = new Lazy<AbbrevWordPersister>(
                    () => {
                        var ret = new AbbrevWordPersister();
                        ret.Load(MemopadConsts.AbbrevWordDictionaryPath);
                        return ret;
                    }
                );

                // --- notify icon ---
                _notifyIcon = new MemopadNotifyIcon();
    
                // --- hotkey ---
                _hotKey = new HotKey();
                _hotKey.AddHotKey(
                    _windowSettings.ActivateHotKey,
                    HotKeyUtil.ActivateHotKeyPressed
                );
                _hotKey.AddHotKey(
                    _windowSettings.ClipMemoHotKey,
                    HotKeyUtil.ClipMemoHotKeyPressed
                );
                _hotKey.AddHotKey(
                    _windowSettings.CreateMemoHotKey,
                    HotKeyUtil.CreateMemoHotKeyPressed
                );
                _hotKey.AddHotKey(
                    _windowSettings.CaptureScreenHotKey,
                    HotKeyUtil.CaptureScreenHotKeyPressed
                );

                _kryptonPalette = new KryptonPalette();
                _kryptonManager = new KryptonManager();
                _kryptonManager.GlobalPalette = _kryptonPalette;

                _theme = CreateTheme(_windowSettings.ReplaceMeiryoWithMeiryoUI);

                _backupExecutor = new BackupExecutor(
                    _windowSettings.GetDailyBackupDirPath(),
                    _windowSettings.GetWeeklyBackupDirPath(),
                    _windowSettings.GetMonthlyBackupDirPath()
                );
#if !DEBUG
                _backupExecutor.Start();
#endif

                _autoFileImporter = new AutoFileImporter();

                //if (_ticketService.GetUpgradeStatus() == UpgradeTicketService.UpgradeStatus.JustInvalidFirst) {
                //    MessageUtil.IntroducePremiumLicenseExtension();
                //}
            }
        }

        internal bool IllegalSettings{
            get { return _illegalSettings; }
        }

        internal bool NeedRecoverMemoInfos {
            get { return _needRecoverMemoInfos; }
        }

        /// memファイルとMemoオブジェクトからMemoInfoを復元する
        internal void RecoverMemoInfos() {
            var recovered = new MemoInfoCollection();

            var form = new ProgressForm();
            form.Font = _theme.CaptionFont;
            form.Text = "ノートデータの復元";
            form.Message = 
                "ノートデータの破損を検知しました。" + System.Environment.NewLine +
                "ノートデータを復元しています。";

            form.Show(_mainForm);
            form.Refresh();
            Application.DoEvents();

            var currentProgress = 0;
            var recoveredFromMemento = MemoSerializeUtil.RecoverMemoInfosFromMemento(
                (progress) => {
                    if (progress != currentProgress) {
                        form.Progress = progress;
                        form.Refresh();
                        Application.DoEvents();
                    }
                    currentProgress = progress;
                }
            );
            form.Refresh();
            Application.DoEvents();

            if (recoveredFromMemento.Any()) {
                foreach (var info in recoveredFromMemento) {
                    recovered.Add(info);
                }
            }

            //_memoInfosValidated = true;
            RemoveInvalidMemoInfos(recovered);
            foreach (var info in recovered) {
                info.PropertyChanged += HandleMemoInfoPropChanged;
            }

            _memoInfos = recovered;

            /// どれが削除済みかわからないので
            /// removed memo infosは空にしておく
            _removedMemoInfos = new MemoInfoCollection();

            form.Close();
            form.Dispose();

            try {
                MemoInfo.SaveMemoInfos(_MemoInfos, _memoAccessor);
                MemoInfo.SaveRemovedMemoInfos(_removedMemoInfos, _memoAccessor);
            } catch (Exception e) {
                Logger.Warn("Save memo infos failed", e);
            }

            _preventSaveAll = true;
            Application.Restart();
        }

        private ITheme CreateTheme(bool replaceMeiryo) {
            _theme = new DefaultTheme();
            if (replaceMeiryo) {
                _theme.ReplaceFont("Meiryo", "Meiryo UI");
                _theme.ReplaceFont("メイリオ", "Meiryo UI");
            }
            _theme.DescriptionFont = new Font(
                _theme.CaptionFont.Name, _theme.CaptionFont.SizeInPoints - 1
            );

            switch (_windowSettings.Theme) {
                case ThemeKind.Default: {
                    _kryptonPalette.BasePaletteMode = PaletteMode.ProfessionalSystem;
                    _theme.DarkBackColor = Color.WhiteSmoke;
                    break;
                }
                case ThemeKind.Blue: {
                    _kryptonPalette.BasePaletteMode = PaletteMode.Office2010Blue;
                    _theme.DarkBackColor = Color.FromArgb(210, 230, 250);
                    break;
                }
                case ThemeKind.Silver: {
                    _kryptonPalette.BasePaletteMode = PaletteMode.Office2010Silver;
                    _theme.DarkBackColor = Color.WhiteSmoke;
                    break;
                }
                case ThemeKind.Black: {
                    _kryptonPalette.BasePaletteMode = PaletteMode.Office2010Black;
                    _theme.DarkBackColor = Color.LightGray;
                    break;
                }
                default: {
                    _kryptonPalette.BasePaletteMode = PaletteMode.ProfessionalSystem;
                    _theme.DarkBackColor = Color.WhiteSmoke;
                    break;
                }
            }

            var captionFont = _theme.CaptionFont;
            _kryptonPalette.ToolMenuStatus.ToolStrip.ToolStripFont = captionFont;
            _kryptonPalette.Common.StateCommon.Content.LongText.Font = captionFont;
            _kryptonPalette.Common.StateCommon.Content.ShortText.Font = captionFont;

            var menuFont = _theme.MenuFont;
            _kryptonPalette.ToolMenuStatus.MenuStrip.MenuStripFont = menuFont;

            _kryptonPalette.ToolMenuStatus.StatusStrip.StatusStripFont = _theme.StatusFont;


            /// toolstrip color
            if (_windowSettings.Theme == ThemeKind.Blue || _windowSettings.Theme == ThemeKind.Silver|| _windowSettings.Theme == ThemeKind.Black) {
                var color = _kryptonPalette.GetBackColor1(PaletteBackStyle.PanelClient, PaletteState.Normal);
                _kryptonPalette.ToolMenuStatus.ToolStrip.ToolStripGradientBegin = color;
                _kryptonPalette.ToolMenuStatus.ToolStrip.ToolStripGradientEnd = color;
                _kryptonPalette.ToolMenuStatus.StatusStrip.StatusStripGradientBegin = color;
                _kryptonPalette.ToolMenuStatus.StatusStrip.StatusStripGradientEnd = color;
                _kryptonPalette.BaseRenderer = new CustomOffice2010Renderer(color);
            }

            return _theme;
        }

        private void InitSettings(
            Version currentVersion,
            BootstrapSettings bootstrapSettings,
            MemopadSettings settings,
            MemopadWindowSettings windowSettings
        ) {
            /// Versionは1.1.0で導入，それ以前はIsInitializedを使っていたための処理
            var version = default(Version);
            if (string.IsNullOrEmpty(settings.Version)) {
                if (settings.IsInitialized) {
                    version = new Version("1.0.0");
                } else {
                    /// 初めての起動
                    version = new Version("0.0.0");
                }
            } else {
                version = new Version(settings.Version);
            }

            var winVersion = default(Version);
            if (string.IsNullOrEmpty(windowSettings.Version)) {
                /// 初めての起動
                winVersion = new Version("0.0.0");
            } else {
                winVersion = new Version(windowSettings.Version);
            }

            /// バージョンチェック
            if (version > currentVersion) {
                MessageBox.Show(
                    "ご利用のMochaNoteのバージョンでは対応していないノートデータです。" + Environment.NewLine +
                    "最新版のMochaNoteをインストールしてください。",
                    "バージョンエラー"
                );
                _illegalSettings = true;
                _preventSaveAll = true;
                return;
            }

            var ver100 = new Version("1.0.0");
            if (version < ver100) {
                /// デフォルトのフォントをフォントインストール状況から決定
                var families = FontFamily.Families;

                var textFontName = default(string);
                if (families.Any(ff => ff.Name == "MeiryoKe_PGothic")) {
                    textFontName = "MeiryoKe_PGothic";

                } else if (families.Any(ff => ff.Name == "メイリオ")) {
                    textFontName = "メイリオ";
                } else if (families.Any(ff => ff.Name == "ＭＳ Ｐゴシック")) {
                    textFontName = "ＭＳ Ｐゴシック";

                } else if (families.Any(ff => ff.Name == "Meiryo")) {
                    textFontName = "Meiryo";
                } else {
                    textFontName = "MS PGothic";
                }

                settings.DefaultMemoTextFontName = textFontName;
                settings.DefaultMemoContentFontName = textFontName;
                settings.DefaultUmlFontName = textFontName;
                settings.DefaultMemoTextFontSize = 9;
                settings.DefaultMemoContentFontSize = 9;
                settings.DefaultUmlFontSize = 9;
            }

            //var ver110 = new Version("1.1.0");
            //if (version < ver110) {
            //    settings.DefaultShapeId = "rect";
            //}
            
            var ver120 = new Version("1.2.0");
            if (version < ver120) {
                settings.ConfirmFolderRemoval = true;
                settings.ConfirmSmartFolderRemoval = true;
            }
            if (winVersion < ver120) {
                windowSettings.Theme = ThemeKind.Default;
            }

            var ver161 = new Version("1.6.1");
            if (winVersion < ver161) {
                windowSettings.ShowStartPageOnStart = true;
            }

            var ver170 = new Version("1.7.0");
            if (winVersion < ver170) {
                windowSettings.ShowDescendantTagsMemo = false;
            }

            //var ver190 = new Version("1.9.0");
            //if (winVersion < ver190) {
            //    var items = new List<MemoListBoxDisplayItem>(windowSettings.MemoListBoxDisplayItems);
            //    if (!items.Contains(MemoListBoxDisplayItem.SummaryText)) {
            //        items.Add(MemoListBoxDisplayItem.SummaryText);
            //    }
            //    windowSettings.MemoListBoxDisplayItems = items.ToArray();
            //}

            var ver1100 = new Version("1.10.0");
            if (winVersion < ver1100) {
                windowSettings.ClipMemoHotKey = "Control+Alt+C";
                windowSettings.CreateMemoHotKey = "Control+Alt+M";
                windowSettings.CaptureScreenHotKey = "Control+Alt+D";
                windowSettings.SubInfoShown = false;
            }

            var ver1110 = new Version("1.11.0");
            if (version < ver1110) {
                MemoDataFolderSync.MakeMemoDataFolderIcon(MemopadConsts.MemoRoot);
            }

            var ver1120 = new Version("1.12.0");
            if (version < ver1120) {
                settings.MemoTextFrameVisiblePolicy = (int) HandleStickyKind.MouseOver;
            }

            var ver1130 = new Version("1.13.0");
            if (version < ver1130) {
                windowSettings.MemoTextDefaultMaxWidth = -1;
                windowSettings.MemoEditorBackgroundImageOpacityPercent = 20;
                windowSettings.MemoEditorBackgroundImageScalePercent = 100;
            }

            var ver200 = new Version("2.0.0");
            if (version < ver200) {
                settings.NotifyIconClickAction = NotifyIconActionKind.ShowMainForm;
                settings.NotifyIconDoubleClickAction = NotifyIconActionKind.CreateMemo;
                settings.ConfirmDataConversionFromV1 = true;

                try {
                    if (File.Exists(MemopadConsts.TutorialPath)) {
                        Process.Start(MemopadConsts.TutorialPath);
                    }
                } catch (Exception e) {
                    Logger.Warn("Can't load tutorial", e);
                }
            }
            if (winVersion < ver200) {
                windowSettings.MemoListBoxSortsImportanceOrder = true;
                windowSettings.ReplaceMeiryoWithMeiryoUI = true;
                windowSettings.MinimizeToTaskTray = false;
                windowSettings.MinimizeOnStartUp = false;
            }

            var ver220 = new Version("2.2.0");
            if (winVersion < ver220) {
                windowSettings.ActivateHotKey = "Control+Alt+A";
            }

            var ver240 = new Version("2.4.0");
            if (winVersion < ver240) {
                windowSettings.BackupRoot = Path.Combine(MemopadConsts.AppDataRoot, "backup");
            }
//#if DEBUG
//            windowSettings.BackupRoot = Path.Combine(MemopadConsts.AppDataRoot, "backup_debug");
//#endif

            var ver300 = new Version("3.0.0");
            if (version < ver300) {
                settings.UseClearType = true;
                settings.EditorCanvasImeOn = true;

                windowSettings.SmtpPort = 25;
                windowSettings.SmtpEnableAuth = false;
                windowSettings.SmtpEnableSsl = false;
            }
            
            var ver302 = new Version("3.0.2");
            if (version < ver302) {
                settings.CheckLatestOnStart = true;
            }

            if (settings.ConfirmDataConversionFromV1) {
                if (File.Exists(MemopadConsts.BootstrapSettingsV1FilePath)) {
                    /// データ変換のお知らせ
                    using (var dialog = new ConfirmDataConversionForm()) {
                        dialog.ShowDialog();
                        settings.ConfirmDataConversionFromV1 = !dialog.DontShow;
                    }
                } else {
                    settings.ConfirmDataConversionFromV1 = false;
                }
            }

            /// 現在のバージョン
            settings.Version = currentVersion.ToString();
            windowSettings.Version = currentVersion.ToString();

            /// Mkamo.Model.dllがロックされてしまうのを直せないのでproxyの保存はやめる
            var assemPath = MemopadConsts.ProxyAssemblyFilePath;
            if (File.Exists(assemPath)) {
                File.Delete(assemPath);
            }

//#if !DEBUG
#if false
            /// proxy cache
            var assemPath = MemopadConsts.ProxyAssemblyFilePath;
            if (version != currentVersion) {
                /// delete old proxy cache
                if (File.Exists(assemPath)) {
                    File.Delete(assemPath);
                }
            }

            if (File.Exists(assemPath)) {
                /// load proxy cache
                var refAssemVerStr = bootstrapSettings.ModelAssemblyVersionReferencedByProxy;
                if (string.IsNullOrEmpty(refAssemVerStr)) {
                    File.Delete(assemPath);
                } else {
                    var refAssemVer = new Version(refAssemVerStr);
                    //var refAssem = Assembly.Load("Mkamo.Model");
                    var refAssem = Assembly.ReflectionOnlyLoad("Mkamo.Model");

                    if (refAssemVer != refAssem.GetName().Version) {
                        File.Delete(assemPath);
                    } else {
                        try {
                            _container.LoadProxyAssembly(assemPath);
                        } catch (Exception e) {
                            Logger.Warn("Can't load proxy assembly.", e);
                            MessageBox.Show(
                                "古いプロキシアセンブリが見つかりました。" + Environment.NewLine +
                                "MochaNote終了後に" + assemPath + "を削除してください。",
                                "プロキシアセンブリロードエラー"
                            );
                            throw;
                        }
                    }
                }
            }
#endif
        }

        // ========================================
        // destructor
        // ========================================
        /// <summary>
        /// 終了処理をして終了する。
        /// </summary>
        internal void Exit() {
            lock (_lock) {
                if (IsMainFormLoaded && _mainForm.IsShown) {
                    /// Close()内でExitInternal()を呼ぶ
                    _mainForm.Cursor = Cursors.WaitCursor;
                    _mainForm.Close();
                } else {
                    ExitInternal();
                }
            }
        }

        /// <summary>
        /// Exit()とMainFormのOnFormClosed()以外から呼ばれないようにする。
        /// </summary>
        internal void ExitInternal() {
//#if !DEBUG
#if false
            var assem = Assembly.Load("Mkamo.Model");
            var assemPath = MemopadConsts.ProxyAssemblyFilePath;
            if (!File.Exists(assemPath)) {
                /// save proxy referencing assem version
                BootstrapSettings.ModelAssemblyVersionReferencedByProxy = assem.GetName().Version.ToString();
            }
#endif

            if (!_preventSaveAll) {
                SaveAll();
                CloseConnections();
            }

//#if !DEBUG
#if false
            if (!File.Exists(assemPath)) {
                /// create new proxy cache
                MessageBoxUtil.Show(
                    (IsMainFormLoaded && _mainForm.IsShown) ? _mainForm : null,
                    _theme.CaptionFont,
                    "キャッシュを作成しています。\r\n" +
                    "しばらくお待ちください。\r\n" +
                    "この処理は初めてのMochaNoteの終了時にのみ行われます。",
                    "キャッシュの作成",
                    () => _container.SaveProxyAssembly(new[] { assem })
                );
            }
#endif
            _context.ExitThread();
        }


        // ========================================
        // event
        // ========================================
        /// <summary>
        /// MemoInfosに何らかの変更が行われたときに発火するイベント．
        /// </summary>
        public event EventHandler<MemoInfoEventArgs> MemoInfoAdded;
        public event EventHandler<MemoInfoEventArgs> MemoInfoRemoving;
        public event EventHandler<MemoInfoEventArgs> MemoInfoRemoved;
        public event EventHandler<MemoInfoEventArgs> MemoInfoChanged;

        public event EventHandler<MemoInfoEventArgs> MemoInfoRecovered;
        public event EventHandler<MemoInfoEventArgs> MemoInfoRemovedCompletely;

        public event EventHandler ActiveFolderChanging;
        public event EventHandler ActiveFolderChanged;

        public event EventHandler ActiveSmartFilterChanged;

        public event EventHandler RecentlyClosedMemoInfosChanged;

        // ========================================
        // property
        // ========================================
        public Workspace Workspace {
            get { return _workspace; }
        }

        public SqlServerAccessor MemoAccessor {
            get { return _memoAccessor; }
        }

        public IEntityContainer Container {
            get { return _container; }
        }

        public MemopadSettings Settings {
            get { return _settings; }
        }

        public MemopadWindowSettings WindowSettings {
            get { return _windowSettings; }
        }

        public ITheme Theme {
            get { return _theme; }
        }

        public KryptonPalette KryptonPalette {
            get { return _kryptonPalette; }
        }

        /// <summary>
        /// コマンドラインでMemoRootをわたされたときはMemoRoot == null。
        /// </summary>
        public BootstrapSettings BootstrapSettings {
            get { return MemopadConsts.BootstrapSettings; }
        }

        public IKeyScheme KeySchema {
            get { return _keySchema; }
        }

        public IEnumerable<MemoInfo> MemoInfos {
            get { return _MemoInfos; }
        }

        public int MemoInfoCount {
            get { return _MemoInfos.Count; }
        }

        public IEnumerable<MemoInfo> RemovedMemoInfos {
            get { return _removedMemoInfos; }
        }

        public MemopadForm MainForm {
            get { return _MainForm; }
        }

        public bool IsMainFormLoaded {
            get { return _mainForm != null && _mainForm.IsHandleCreated; }
        }

        public FusenManager FusenManager {
            get { return _fusenManager; }
        }

        public HotKey HotKey {
            get { return _hotKey; }
        }

        public MemopadAppContext Context {
            get { return _context; }
        }

        public IEnumerable<MemoInfo> OpenMemoInfos {
            get {
                if (IsMainFormLoaded) {
                    foreach (var tabPage in _MainForm.MemoTabPages) {
                        var content = tabPage.Tag as PageContent;
                        yield return content.MemoInfo;
                    }
                }
                foreach (var info in _fusenManager.OpenMemoInfos) {
                    yield return info;
                }
            }
        }

        public IEnumerable<MemoInfo> RecentlyClosedMemoInfos {
            get {
                var ids = _recentlyClosedMemoIds.Value.ToArray();
                foreach (var id in ids) {
                    var ret = FindMemoInfoByMemoId(id);
                    if (ret == null) {
                        _recentlyClosedMemoIds.Value.Remove(id);
                    } else {
                        yield return ret;
                    }
                }
            }
        }

        public IEnumerable<MemoInfo> RecentlyCreatedMemoInfos {
            get {
                var ids = _recentlyCreatedMemoIds.Value.ToArray();
                foreach (var id in ids) {
                    var ret = FindMemoInfoByMemoId(id);
                    if (ret == null) {
                        _recentlyCreatedMemoIds.Value.Remove(id);
                    } else {
                        yield return ret;
                    }
                }
            }
        }

        public IEnumerable<MemoInfo> RecentlyModifiedMemoInfos {
            get {
                var ids = _recentlyModifiedMemoIds.Value.ToArray();
                foreach (var id in ids) {
                    var ret = FindMemoInfoByMemoId(id);
                    if (ret == null) {
                        _recentlyModifiedMemoIds.Value.Remove(id);
                    } else {
                        yield return ret;
                    }
                }
            }
        }

        public MemoFolder ActiveFolder {
            get { return _activeFolder; }
            set {
                if (value == _activeFolder) {
                    return;
                }
                OnActiveFolderChanging();
                _activeFolder = value;
                SyncTabsWithActiveFolder(value);
                OnActiveFolderChanged();
            }
        }

        public MemoSmartFilter ActiveSmartFilter {
            get { return _activeSmartFilter; }
            set {
                if (value == _activeSmartFilter) {
                    return;
                }
                _activeSmartFilter = value;
                OnActiveSmartFilterChanged();
            }
        }


        // ------------------------------
        // protected
        // ------------------------------
        protected MemoInfoCollection _MemoInfos {
            get {
                return _memoInfos;
            }
        }

        // ------------------------------
        // internal
        // ------------------------------
        internal object _Lock {
            get { return _lock; }
        }

        internal bool _PreventSaveAll {
            get { return _preventSaveAll; }
            set {_preventSaveAll = value; }
        }

        internal MemopadSettings _Settings {
            get { return _settings; }
        }

        internal MemopadWindowSettings _WindowSettings {
            get { return _windowSettings; }
        }

        internal ToolStripRenderer _MiniToolBarRenderer {
            get {
                if (_miniToolBarRenderer == null) {
                    using (var rend = new RenderOffice2010())
                    using (var pal = new KryptonPalette()) {
                        pal.BasePaletteMode = _kryptonPalette.BasePaletteMode;

                        var captionFont = _theme.CaptionFont;
                        //pal.ToolMenuStatus.ToolStrip.ToolStripFont = captionFont;
                        //pal.Common.StateCommon.Content.LongText.Font = captionFont;
                        //pal.Common.StateCommon.Content.ShortText.Font = captionFont;

                        var menuFont = _theme.MenuFont;
                        pal.ToolMenuStatus.MenuStrip.MenuStripFont = menuFont;

                        pal.ToolMenuStatus.ToolStrip.ToolStripGradientBegin = Color.White;
                        pal.ToolMenuStatus.ToolStrip.ToolStripGradientEnd = Color.White;
                        _miniToolBarRenderer = rend.RenderToolStrip(pal);
                    }
                }
                return _miniToolBarRenderer;
            }
        }

        internal AbbrevWordPersister _AbbrevWordPersister {
            get { return _abbrevWordPersister.Value; }
        }

        internal Func<IEnumerable<string>> _AdditionalAbbrevWordProvider {
            get { return _abbrevWordPersister.Value.GetWords; }
        }


        // ------------------------------
        // private
        // ------------------------------
        private MemopadForm _MainForm {
            get {
                if (_mainForm == null) {
                    _mainForm = new MemopadForm();
                    _mainForm.Theme = _theme;
                    _mainForm.Activated += (sender, e) => _fusenManager.HideAllToolStripForms();

                    _mainForm.Resize += (s, e) => {
                        if (_mainForm.WindowState == FormWindowState.Minimized) {
                            _backupExecutor.LockIdle = true;
                        } else {
                            _backupExecutor.LockIdle = false;
                        }
                    };

                    _backupExecutor.DailyBackupStarted += (s, e) => SetMainFormStatusMessageSync("バックアップを作成しています");
                    _backupExecutor.WeeklyBackupStarted += (s, e) => SetMainFormStatusMessageSync("バックアップを作成しています");
                    _backupExecutor.MonthlyBackupStarted += (s, e) => SetMainFormStatusMessageSync("バックアップを作成しています");

                    _backupExecutor.DailyBackupFinished += (s, e) => SetMainFormStatusMessageSync("バックアップを作成しました");
                    _backupExecutor.WeeklyBackupFinished += (s, e) => SetMainFormStatusMessageSync("バックアップを作成しました");
                    _backupExecutor.MonthlyBackupFinished += (s, e) => SetMainFormStatusMessageSync("バックアップを作成しました");
                }
                return _mainForm;
            }
        }

        private void SetMainFormStatusMessageSync(string message) {
            if (_mainForm != null && _mainForm.IsHandleCreated && !_mainForm.IsDisposed) {
                Action act = () => _mainForm.ShowStatusMessage(message);
                _mainForm.Invoke(act);
            }
        }

        // ========================================
        // method
        // ========================================
        public bool EnsureMemoRootExists() {
            if (Directory.Exists(MemopadConsts.MemoRoot)) {
                return true;
            } else {
                Logger.Error("MemoRoot not exists.");
                var msg =
                    "ノート格納フォルダが見つかりません。\r\n" +
                    "ノートの保存を中止します。";
                MessageBox.Show(_mainForm, msg, "ノート保存エラー");
                return false;
            }
        }

        public void SaveAllMemos() {
            lock (_lock) {
                lock (MemopadConsts.DataLock) {
                    if (!EnsureMemoRootExists()) {
                        return ;
                    }
    
                    // todo: tranはCommandにセットしてやらないと有効にならない
                    //var memoTran = _context.MemoConnection.BeginTransaction();
                    //var exDataTran = _context.ExtendedDataConnection.BeginTransaction();
    
                    try {
    
                        if (_mainForm != null) {
                            try {
                                _mainForm.EnsureFocusCommited();
                            } catch (Exception e) {
                                Logger.Warn("Ensure focus commited failed", e);
                            }
        
                            foreach (var page in _mainForm.MemoTabPages) {
                                try {
                                    _mainForm.SavePage(page);
                                } catch (Exception e) {
                                    Logger.Warn("Save page failed: " + ((PageContent) page.Tag).MemoInfo.Title, e);
                                }
                            }
                        }
    
                        foreach (var fusen in _fusenManager.RegisteredForms) {
                            try {
                                fusen.EnsureFocusCommited();
                                fusen.Save();
                            } catch (Exception e) {
                                Logger.Warn("Save fusen failed: " + fusen.PageContent.MemoInfo.Title, e);
                            }
                        }

                        _container.Commit();
                        MemoInfo.SaveMemoInfos(_MemoInfos, _memoAccessor);
                        MemoInfo.SaveRemovedMemoInfos(_removedMemoInfos, _memoAccessor);
                        MemoIdCollection.SaveIdsToSdf(_removedEmbeddedFileIds.Value, "RemovedEmbeddedFileId", _memoAccessor);
                        MemoIdCollection.SaveIdsToSdf(_removedEmbeddedImageIds.Value, "RemovedEmbeddedImageId", _memoAccessor);
    
                        //memoTran.Commit();
                        //exDataTran.Commit();
    
                    } catch (Exception e) {
                        Logger.Error("Save all memos failed. ", e);
                        //memoTran.Rollback();
                        //exDataTran.Rollback();
                    }
                }
            }
        }

        public void CheckForUpdates() {
            lock (_lock) {
                if (!NetworkInterface.GetIsNetworkAvailable()) {
                    MessageBox.Show(
                        _mainForm,
                        "ネットワークに接続されていないため最新版の確認ができません。",
                        "ネットワークエラー",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }
    
                try {
                    var updater = new Updater();
                    if (updater.IsLatest(_settings.Version)) {
                        MessageBox.Show(
                            _mainForm,
                            "ご使用のMochaNoteは最新です。",
                            "最新版の確認",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
    
                    } else {
                        var ret = MessageBox.Show(
                            _mainForm,
                            "新しいバージョンのMochaNoteが見つかりました。" + Environment.NewLine +
                            "ダウンロードしますか?",
                            "最新版の確認",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question
                        );
                        if (ret == DialogResult.Yes) {
                            updater.DownloadLatest();
                        }
                    }
    
                } catch (Exception e) {
                    Logger.Warn("Update check failed.", e);
                    MessageBox.Show(
                        _mainForm,
                        "最新版の確認中にエラーが発生しました。" + Environment.NewLine +
                        "インターネットへの接続が可能かどうか確認してください。",
                        "最新版の確認エラー",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        public void CheckForUpdatesAsync() {
                if (!NetworkInterface.GetIsNetworkAvailable()) {
                    return;
                }
    
                try {
                    var updater = new Updater();

                    Action update = () => {
                        var ret = MessageBox.Show(
                            _mainForm,
                            "新しいバージョンのMochaNoteが見つかりました。" + Environment.NewLine +
                            "ダウンロードしますか?",
                            "最新版の確認",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question
                        );
                        if (ret == DialogResult.Yes) {
                            updater.DownloadLatest();
                        }
                    };

                    updater.IsLatestAsync(_settings.Version, update);
    
                } catch (Exception e) {
                    Logger.Warn("Update check async failed.", e);
                }
        }

        // --- form ---
        public void ShowMainForm() {
            ShowMainForm(true);
        }

        public void ShowMainForm(bool restore) {
            lock (_lock) {
                _MainForm.Show();
                if (restore && (_MainForm.WindowState == FormWindowState.Minimized)) {
                    _MainForm.ShowInTaskbar = true;
                    _MainForm.WindowState = FormWindowState.Normal;
                }
            }
        }

        public void LoadFusenForms() {
            var ids = MemoIdCollection.LoadIdsFromFile(MemopadConsts.FusenMemoIdsFilePath);
            if (ids != null) {
                foreach (var id in ids) {
                    var info = FindMemoInfoByMemoId(id);
                    LoadMemoAsFusen(info, true);
                }
            }
        }

        public void ActivateMainForm() {
            lock (_lock) {
                _MainForm.Activate();
            }
        }

        public void ShowFusenForms(bool useDummy) {
            lock (_lock) {
                _fusenManager.ShowAll(useDummy);
            }
        }

        public void ShowWorkspaceView() {
            lock (_lock) {
                _MainForm.ShowWorkspaceView();
            }
        }

        // --- memo (== tab page) ---
        public bool IsLoadedMemo(MemoInfo info) {
            return OpenMemoInfos.Any(i => i == info);
        }

        public MemoInfo CreateMemo() {
            return CreateMemo("新しいノート");
        }

        public MemoInfo CreateMemo(bool asFusen) {
            return CreateMemo("新しいノート", asFusen);
        }

        public MemoInfo CreateMemo(string title) {
            return CreateMemo(title, false);
        }

        public MemoInfo CreateMemo(string title, bool asFusen) {
            lock (_lock) {
                if (!EnsureMemoRootExists()) {
                    return null;
                }

                var memo = _workspace.CreateMemo();
                var info = new MemoInfo();

                memo.Title = title;
                info.Title = memo.Title;

                var id = Guid.NewGuid().ToString();
                info.MemoId = _container.GetId(memo);
                info.MementoId = id;

                info.PropertyChanged += HandleMemoInfoPropChanged;
                _MemoInfos.Add(info);

                if (asFusen) {
                    var fusen = new FusenForm();
                    fusen.FormClosed += HandleFusenFormFormClosed;
                    _fusenManager.RegisterForm(info, fusen);
                    fusen.NewMemo(info, memo);

                } else {
                    _MainForm.NewPage(info, memo);
                    if (_activeFolder != null) {
                        _activeFolder.AddContainingMemo(memo);
                    }
                }

                AddRecentlyCreatedMemoInfo(info);

                OnMemoInfoAdded(info);

                return info;
            }
        }

        public MemoInfo ClipAndCreateMemo() {
            lock (_lock) {
                var old = string.Empty;
                if (Clipboard.ContainsText()) {
                    old = Clipboard.GetText();
                }
                Clipboard.Clear();
    
                var winText = User32Util.GetActiveWindowText();
    
                var isWinXP = EnvironmentUtil.IsWinXP();
    
                /// Ctrl+C送信
                if (isWinXP) {
                    /// この実装はWin Vista/7でうまく動かない
                    /// SendWait()の内部動作が変わるため
                    SendKeys.SendWait("^c");
                } else {
                    /// Win XPでもVista/7でも動くが
                    /// 失敗したり，waitが必要だったりする
                    User32Util.SendCtrlC(_windowSettings.ClipMemoHotKey);
                }
    
                var source = string.Empty;
                try {
                    /// SendCtrlC()の場合少し待たないと
                    /// Clipboard.GetData("Html Format")が成功しないことがある
                    //if (!isWinXP) {
                        ClipboardUtil.Wait("Text", 1000);
                    //}
                    source = ClipboardUtil.GetHtmlSourceUrlFromClipboard();
                } catch (Exception) {
                }
    
                ShowMainForm();
                ActivateMainForm();
    
                var ret = default(MemoInfo);
                if (string.IsNullOrEmpty(winText)) {
                    ret = CreateMemo();
                } else {
                    ret = CreateMemo(winText);
                }
    
                if (ret != null && !StringUtil.IsNullOrWhitespace(source)) {
                    var memo = Container.Find<Memo>(ret.MemoId);
                    if (memo != null) {
                        memo.Source = source;
                    }
                }
    
                if (_MainForm.CurrentEditorCanvas != null) {
                    MemoEditorHelper.Paste(_MainForm.CurrentEditorCanvas.RootEditor.Children.First(), false);
                }
    
                if (!string.IsNullOrEmpty(old)) {
                    Clipboard.Clear();
                    ClipboardUtil.SetText(old);
                }
    
                return ret;
            }
        }

        public MemoInfo CaptureAndCreateMemo() {
            var ret = default(MemoInfo);

            if (_inCaptureScreen) {
                return null;
            }

            _inCaptureScreen = true;
            using (var capture = new ScreenCaptureForm()) {
                capture.Font = _theme.CaptionFont;
                capture.Setup();
                capture.ShowDialog(_MainForm);

                if (capture.IsCaptured) {
                    ShowMainForm();
                    ActivateMainForm();
                    ret = CreateMemo("画面の取り込み");

                    using (var img = capture.CreateCaptured()) {

                        if (_MainForm.CurrentEditorCanvas != null) {
                            MemoEditorHelper.AddImage(_MainForm.CurrentEditorCanvas.RootEditor.Content, new Point(8, 8), img, true, true);
                        }
                    }

                }
                capture.Close();
            }
            _inCaptureScreen = false;

            return ret;
        }

        public bool LoadMemo(MemoInfo info) {
            return LoadMemo(info, false);
        }

        public bool LoadMemo(MemoInfo info, bool background) {
            lock (_lock) {
                try {
                    if (!ValidateMemoInfo(info)) {
                        return false;
                    }

                    /// ロードには自分で保持しているMemoInfoを必ず使う
                    info = _MemoInfos.First(i => i == info);
                    if (info == null) {
                        return false;
                    }

                    if (_mainForm != null) {
                        _mainForm.Cursor = Cursors.WaitCursor;
                    }

                    if (_fusenManager.IsRegistered(info)) {
                        var fusen = _fusenManager.GetRegisteredForm(info);
                        fusen.Activate();
                        return true;
                    }

                    var tab = _MainForm.LoadPage(info, background, true);

                    /// sync active folder
                    if (_activeFolder != null && tab != null) {
                        var memo = _container.Find<Memo>(info.MemoId);
                        _activeFolder.AddContainingMemo(memo);
                    }

                    /// 履歴から削除
                    if (tab != null) {
                        RemoveRecentlyClosedMemoInfos(new[] { info });
                    }

                        return tab != null;

                } catch (Exception e) {
                    Logger.Warn("Load memo failed: " + info.Title + ", " + info.MementoId, e);
                    return false;
                } finally {
                    if (_mainForm != null) {
                        _mainForm.Cursor = Cursors.Default;
                    }
                }
            }
        }

        public bool LoadMemoAsFusen(MemoInfo info, bool background) {
            lock (_lock) {
                try {
                    if (!ValidateMemoInfo(info)) {
                        return false;
                    }

                    /// ロードには自分で保持しているMemoInfoを必ず使う
                    info = _MemoInfos.First(i => i == info);
                    if (info == null) {
                        return false;
                    }

                    if (_fusenManager.IsRegistered(info)) {
                        /// すでに開いていればアクティブに
                        var registered = _fusenManager.GetRegisteredForm(info);
                        registered.Show();
                        registered.Activate();
                        return true;
                    }

                    if (_mainForm != null) {
                        _mainForm.Cursor = Cursors.WaitCursor;
                    }

                    if (_MainForm.IsMemoOpened(info)) {
                        CloseMemo(info);
                    }

                    var fusen = new FusenForm();
                    fusen.FormClosed += HandleFusenFormFormClosed;
                    _fusenManager.RegisterForm(info, fusen);
                    fusen.OpenMemo(info, background, true);

                    /// 履歴から削除
                    RemoveRecentlyClosedMemoInfos(new[] { info });

                    return true;
                    

                } catch (Exception e) {
                    Logger.Warn("Load memo as fusen failed: " + info.Title + ", " + info.MementoId, e);
                    return false;
                } finally {
                    if (_mainForm != null) {
                        _mainForm.Cursor = Cursors.Default;
                    }
                }
            }
        }

        public void LoadAllMemos(IEnumerable<MemoInfo> infos) {
            lock (_lock) {
                if (infos == null) {
                    return;
                }
                if (!EnsureMemoRootExists()) {
                    return;
                }

                var first = true;
                foreach (var info in infos) {
                    if (first) {
                        first = false;
                        LoadMemo(info, false);
                    } else {
                        LoadMemo(info, true);
                    }
                }
            }
        }
        
        public bool LoadRemovedMemo(MemoInfo info) {
            return LoadRemovedMemo(info, false);
        }

        public bool LoadRemovedMemo(MemoInfo info, bool background) {
            lock (_lock) {
                if (!EnsureMemoRootExists()) {
                    return false;
                }

                try {
                    if (!ValidateMemoInfo(info)) {
                        return false;
                    }

                    info = _removedMemoInfos.First(i => i == info);
                    if (info == null) {
                        return false;
                    }

                    if (_mainForm != null) {
                        _mainForm.Cursor = Cursors.WaitCursor;
                    }
                    var tab = _MainForm.LoadPage(info, background, false);

                    return tab != null;

                } catch (Exception e) {
                    Logger.Warn("Load memo failed: " + info.Title + ", " + info.MementoId, e);
                    return false;
                } finally {
                    if (_mainForm != null) {
                        _mainForm.Cursor = Cursors.Default;
                    }
                }
            }
        }

        public bool RemoveMemo(MemoInfo info) {
            lock (_lock) {
                if (!ValidateMemoInfo(info) || !_MemoInfos.Contains(info)) {
                    return false;
                }

                if (!EnsureMemoRootExists()) {
                    return false;
                }

                try {
                    if (_mainForm != null) {
                        _mainForm.Cursor = Cursors.WaitCursor;
                    }

                    var tab = default(TabPage);
                    var isFusen = false;
                    if (_fusenManager.IsRegistered(info)) {
                        isFusen = true;
                        var fusen = _fusenManager.GetRegisteredForm(info);
                        fusen.Close();

                    } else {
                        tab = _MainForm.FindMemoTabPage(
                            tp => info == ((PageContent) tp.Tag).MemoInfo
                        );
                        if (tab != null) {
                            /// コミットしておく
                            _MainForm.EnsureFocusCommited(tab);
                        }
                    }

                    /// 履歴から削除
                    RemoveRecentlyClosedMemoInfos(new[] { info });

                    info.PropertyChanged -= HandleMemoInfoPropChanged;
                    OnMemoInfoRemoving(info);

                    /// _memoInfosから_removedMemoInfosに移動するだけ
                    var ret = _MemoInfos.Remove(info);
                    _removedMemoInfos.Add(info);

                    SaveAllMemos();

                    if (isFusen) {
                        /// do nothing
                    } else {
                        if (tab != null) {
                            _MainForm.ClosePage(tab);
                        }
                        if (_activeFolder != null) {
                            var memo = _container.Find<Memo>(info.MemoId);
                            if (_activeFolder.ContainingMemos.Contains(memo)) {
                                _activeFolder.RemoveContainingMemo(memo);
                            }
                        }
                    }

                    OnMemoInfoRemoved(info);

                    return ret;

                } finally {
                    if (_mainForm != null) {
                        _mainForm.Cursor = Cursors.Default;
                    }
                }
            }
        }

        public bool RecoverMemoFromTrashBox(MemoInfo info) {
            lock (_lock) {
                if (!ValidateMemoInfo(info) || !_removedMemoInfos.Contains(info)) {
                    return false;
                }

                if (!EnsureMemoRootExists()) {
                    return false;
                }

                try {
                    if (_mainForm != null) {
                        _mainForm.Cursor = Cursors.WaitCursor;
                    }

                    var tabPage = _MainForm.FindMemoTabPage(
                        tp => info == ((PageContent) tp.Tag).MemoInfo
                    );
                    if (tabPage != null) {
                        _MainForm.ClosePage(tabPage);
                    }

                    /// _removedMemoInfosから_memoInfosに移動
                    var ret = _removedMemoInfos.Remove(info);
                    _MemoInfos.Add(info);

                    SaveAllMemos();

                    OnMemoInfoRecovered(info);
                    info.PropertyChanged += HandleMemoInfoPropChanged;

                    return ret;

                } finally {
                    if (_mainForm != null) {
                        _mainForm.Cursor = Cursors.Default;
                    }
                }
            }
        }

        public bool RemoveCompletelyMemoFromTrashBox(MemoInfo info) {
            lock (_lock) {
                if (!ValidateMemoInfo(info) || !_removedMemoInfos.Contains(info)) {
                    return false;
                }

                if (!EnsureMemoRootExists()) {
                    return false;
                }

                try {
                    if (_mainForm != null) {
                        _mainForm.Cursor = Cursors.WaitCursor;
                    }

                    var tabPage = _MainForm.FindMemoTabPage(
                        tp => info == ((PageContent) tp.Tag).MemoInfo
                    );

                    var ret = _removedMemoInfos.Remove(info);

                    _memoAccessor.RemoveMemento(info.MementoId);
        
                    if (tabPage != null) {
                        _MainForm.ClosePage(tabPage);
                    }

                    var memo = _container.Find<Memo>(info.MemoId);
                    _workspace.RemoveMemo(memo);
        
                    /// RemoveMemo()の後で保存しないとentityがDiscardedになっている可能性がある
                    SaveAllMemos();

                    OnMemoInfoRemovedCompletely(info);

                    return ret;

                } finally {
                    if (_mainForm != null) {
                        _mainForm.Cursor = Cursors.Default;
                    }
                }
            }
        }

        public void EmptyMemosFromTrashBox() {
            lock (_lock) {
                if (!EnsureMemoRootExists()) {
                    return;
                }

                try {
                    if (_mainForm != null) {
                        _mainForm.Cursor = Cursors.WaitCursor;
                    }

                    var removeds = _removedMemoInfos.ToArray();
                    _removedMemoInfos.Clear();
                    /// 先にClear()しておかないと_workspace.RemoveMemo(memo)の副作用で
                    /// MemopadFormMediator.HandleMemoTagChanged()が呼ばれたときに
                    /// すでに削除されたMemoに対するMemoInfoをTrashBoxPresenterが返すことになり，
                    /// 例外が起こってしまう。

                    foreach (var info in removeds) {
                        if (!ValidateMemoInfo(info, false)) {
                            continue;
                        }

                        if (_MainForm != null) {
                            var tabPage = _MainForm.FindMemoTabPage(
                                tp => info == ((PageContent) tp.Tag).MemoInfo
                            );
                            if (tabPage != null) {
                                _MainForm.ClosePage(tabPage);
                            }
                        }

                        _memoAccessor.RemoveMemento(info.MementoId);
            
                        var memo = _container.Find<Memo>(info.MemoId);
                        _workspace.RemoveMemo(memo);
                    }


                    foreach (var info in removeds) {
                        OnMemoInfoRemovedCompletely(info);
                    }

                    /// RemoveMemo()の後で保存しないとentityがDiscardedになっている可能性がある
                    SaveAllMemos();

                } finally {
                    if (_mainForm != null) {
                        _mainForm.Cursor = Cursors.Default;
                    }
                }
            }
        }

        private void HandleFusenFormFormClosed(object sender, EventArgs e) {
            var fusen = (FusenForm) sender;
            fusen.EnsureFocusCommited();

            SaveAllMemos();

            var closed = fusen.PageContent.MemoInfo;
            if (closed != null) {
                AddRecentlyClosedMemoInfo(closed);
            }

            _fusenManager.UnregisterForm(fusen.PageContent.MemoInfo);
            fusen.FormClosed -= HandleFusenFormFormClosed;
            fusen.Dispose();
        }

        public void CloseMemo(MemoInfo info) {
            lock (_lock) {
                if (!EnsureMemoRootExists()) {
                    return;
                }

                try {
                    //if (!ValidateMemoInfo(info)) {
                    //    return;
                    //}

                    if (_fusenManager.IsRegistered(info)) {
                        var fusen = _fusenManager.GetRegisteredForm(info);
                        fusen.Close();

                    } else {

                        if (_mainForm != null) {
                            _mainForm.Cursor = Cursors.WaitCursor;
                        }
    
                        var tabPage = _MainForm.FindMemoTabPage(
                            tp => info == ((PageContent) tp.Tag).MemoInfo
                        );
                        if (tabPage != null) {
                            _MainForm.EnsureFocusCommited(tabPage);
                        }
        
                        /// 先に保存しないとtabPageが閉じられて保存対象にならない
                        SaveAllMemos();
        
                        if (tabPage != null) {
                            var closed = _MainForm.ClosePage(tabPage);
                            if (closed != null) {
                                AddRecentlyClosedMemoInfo(closed);
                            }
                        }
                        if (_activeFolder != null) {
                            var memo = _container.Find<Memo>(info.MemoId);
                            if (_activeFolder.ContainingMemos.Contains(memo)) {
                                _activeFolder.RemoveContainingMemo(memo);
                            }
                        }
                    }

                } finally {
                    if (_mainForm != null) {
                        _mainForm.Cursor = Cursors.Default;
                    }
                }
            }
        }

        /// <summary>
        /// 付箋は閉じない。
        /// </summary>
        public void CloseAllMemos() {
            lock (_lock) {
                if (!EnsureMemoRootExists()) {
                    return;
                }

                try {
                    if (_mainForm != null) {
                        _mainForm.Cursor = Cursors.WaitCursor;
                    }

                    _MainForm.EnsureFocusCommited();
    
                    /// 先に保存しないとtabPageが閉じられて保存対象にならない
                    SaveAllMemos();

                    var closeds = _MainForm.CloseAllPages();

                    if (_activeFolder != null) {
                        _activeFolder.ClearContainingMemos();
                    }
                    AddRecentlyClosedMemoInfos(closeds);

                } finally {
                    if (_mainForm != null) {
                        _mainForm.Cursor = Cursors.Default;
                    }
                }
            }
        }

        /// <summary>
        /// 付箋は閉じない。
        /// </summary>
        public void CloseOtherMemos() {
            lock (_lock) {
                if (!EnsureMemoRootExists()) {
                    return;
                }

                try {
                    if (_mainForm != null) {
                        _mainForm.Cursor = Cursors.WaitCursor;
                    }

                    _MainForm.EnsureFocusCommited();
    
                    /// 先に保存しないとtabPageが閉じられて保存対象にならない
                    SaveAllMemos();

                    var closeds = _MainForm.CloseOtherPages();
                    if (_activeFolder != null) {
                        var memos = closeds.Select(info => _container.Find<Memo>(info.MemoId));
                        foreach (var memo in memos) {
                            if (_activeFolder.ContainingMemos.Contains(memo)) {
                                _activeFolder.RemoveContainingMemo(memo);
                            }
                        }
                    }
                    AddRecentlyClosedMemoInfos(closeds);

                } finally {
                    if (_mainForm != null) {
                        _mainForm.Cursor = Cursors.Default;
                    }
                }
            }
        }

        public MemoInfo FindMemoInfo(Memo memo) {
            lock (_lock) {
                var memoId = _container.GetId(memo);
                foreach (var info in _MemoInfos) {
                    if (memoId == info.MemoId) {
                        return ValidateMemoInfo(info) ? info : null;
                    }
                }
                return null;
            }
        }

        public MemoInfo FindMemoInfoByMementoId(string mementoId) {
            lock (_lock) {
                foreach (var info in _MemoInfos) {
                    if (mementoId == info.MementoId) {
                        return info;
                    }
                }
                return null;
            }
        }

        public MemoInfo FindMemoInfoByMemoId(string memoId) {
            lock (_lock) {
                foreach (var info in _MemoInfos) {
                    if (memoId == info.MemoId) {
                        return info;
                    }
                }
                return null;
            }
        }

        public PageContent FindPageContent(MemoInfo info) {
            lock (_lock) {
                var opened = _MainForm.FindMemoTabPage(
                    page => {
                        var pageContent = (PageContent) page.Tag;
                        return info == pageContent.MemoInfo;
                    }
                );
                if (opened != null) {
                    return _MainForm.GetPageContent(opened);
                }

                if (_fusenManager.IsRegistered(info)) {
                    return _fusenManager.GetRegisteredForm(info).PageContent;
                }

                return null;
            }
        }

        // --- recent used memos ---
        public void AddRecentlyModifiedMemoInfo(MemoInfo info) {
            var id = info.MemoId;
            var ids = _recentlyModifiedMemoIds.Value;
            if (ids.Any() && ids.Last() == id) {
                return;
            }

            lock (_lock) {
                if (ids.Contains(id)) {
                    ids.Remove(id);
                }
                ids.Add(id);
                if (ids.Count > _settings.RecentMax) {
                    for (int i = ids.Count; i > _settings.RecentMax; --i) {
                        ids.RemoveAt(0);
                    }
                }
            }
        }
        // ------------------------------
        // protected
        // ------------------------------
        protected virtual void OnMemoInfoChanged(MemoInfo memoInfo) {
            var handler = MemoInfoChanged;
            if (handler != null) {
                handler(this, new MemoInfoEventArgs(memoInfo));
            }
        }

        protected virtual void OnMemoInfoAdded(MemoInfo memoInfo) {
            var handler = MemoInfoAdded;
            if (handler != null) {
                handler(this, new MemoInfoEventArgs(memoInfo));
            }
        }
        
        protected virtual void OnMemoInfoRemoving(MemoInfo memoInfo) {
            var handler = MemoInfoRemoving;
            if (handler != null) {
                handler(this, new MemoInfoEventArgs(memoInfo));
            }
        }
        
        protected virtual void OnMemoInfoRemoved(MemoInfo memoInfo) {
            var handler = MemoInfoRemoved;
            if (handler != null) {
                handler(this, new MemoInfoEventArgs(memoInfo));
            }
        }
        
        protected virtual void OnMemoInfoRemovedCompletely(MemoInfo memoInfo) {
            var handler = MemoInfoRemovedCompletely;
            if (handler != null) {
                handler(this, new MemoInfoEventArgs(memoInfo));
            }
        }
        
        protected virtual void OnMemoInfoRecovered(MemoInfo memoInfo) {
            var handler = MemoInfoRecovered;
            if (handler != null) {
                handler(this, new MemoInfoEventArgs(memoInfo));
            }
        }

        protected virtual void OnActiveFolderChanging() {
            var handler = ActiveFolderChanging;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }
        
        protected virtual void OnActiveFolderChanged() {
            var handler = ActiveFolderChanged;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }
        
        protected void OnActiveSmartFilterChanged() {
            var handler = ActiveSmartFilterChanged;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnRecentlyClosedMemoInfosChanged() {
            var handler = RecentlyClosedMemoInfosChanged;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        // ------------------------------
        // internal
        // ------------------------------
        internal void OpenConnections() {
            _context.OpenConnections();
        }

        internal void CloseConnections() {
            _context.CloseConnections();
        }


        internal void SaveBootstrapSettings() {
            BootstrapSettings.SaveBootstrapSettings(
                MemopadConsts.BootstrapSettings,
                MemopadConsts.BootstrapSettingsFilePath
            );
        }

        internal void SaveRecentIds() {
            if (_recentlyClosedMemoIds.IsValueCreated) {
                MemoIdCollection.SaveIdsToFile(_recentlyClosedMemoIds.Value, MemopadConsts.RecentlyClosedMemoIdsFilePath);
            }
            if (_recentlyCreatedMemoIds.IsValueCreated) {
                MemoIdCollection.SaveIdsToFile(_recentlyCreatedMemoIds.Value, MemopadConsts.RecentlyCreatedMemoIdsFilePath);
            }
            if (_recentlyModifiedMemoIds.IsValueCreated) {
                MemoIdCollection.SaveIdsToFile(_recentlyModifiedMemoIds.Value, MemopadConsts.RecentlyModifiedMemoIdsFilePath);
            }
        }

        internal void SaveFusenFormIds() {
            var infos = _fusenManager.OpenMemoInfos;
            var ids = new MemoIdCollection();
            foreach (var info in infos) {
                ids.Add(info.MemoId);
            }
            MemoIdCollection.SaveIdsToFile(ids, MemopadConsts.FusenMemoIdsFilePath);
        }

        // ------------------------------
        // private
        // ------------------------------
        private void SaveAll() {
            lock (_lock) {
                lock (MemopadConsts.DataLock) {
                    if (!EnsureMemoRootExists()) {
                        return ;
                    }
    
                    try {
                        if (_settings.EmptyTrashBoxOnExit) {
                            EmptyMemosFromTrashBox();
                            /// EmptyMemosFromTrashBox()の最後でSaveAllMemos()が呼び出される
                        } else {
                            SaveAllMemos();
                        }
                    } catch (Exception e) {
                        Logger.Warn("Empty trash box failed", e);
                    }
    
                    AddRecentlyClosedMemoInfos(OpenMemoInfos);
    
                    SaveRecentIds();
    
                    if (_mainForm != null) {
                        _mainForm.SaveFormSettings(_windowSettings);
                    }
                    MemopadSettings.SaveSettings(_settings);
                    MemopadWindowSettings.SaveWindowSettings(_windowSettings);
    
                    ClearRemovedEmbeddedFiles();
                    MemoIdCollection.SaveIdsToSdf(_removedEmbeddedFileIds.Value, "RemovedEmbeddedFileId", _memoAccessor);
    
                    ClearRemovedEmbeddedImages();
                    MemoIdCollection.SaveIdsToSdf(_removedEmbeddedImageIds.Value, "RemovedEmbeddedImageId", _memoAccessor);
    
                    SaveFusenFormIds();
    
                    SaveBootstrapSettings();

                    if (_abbrevWordPersister.IsValueCreated) {
                        _abbrevWordPersister.Value.Save(MemopadConsts.AbbrevWordDictionaryPath);
                    }
                }
            }
        }

        private bool ValidateMemoInfo(MemoInfo info) {
            return ValidateMemoInfo(info, true);
        }

        private bool ValidateMemoInfo(MemoInfo info, bool removeInvalidate) {
            if (info == null) {
                return false;
            }

            try {
                var memo = _container.Find<Memo>(info.MemoId);
                var isValidMementoId = OpenMemoInfos.Contains(info) || _memoAccessor.IsMementoExists(info.MementoId);
                if (memo != null && isValidMementoId) {
                    return true;
                }
            } catch (Exception e) {
                Logger.Warn("Find memo or test file exist failed", e);
            }

            info.PropertyChanged -= HandleMemoInfoPropChanged;
            if (removeInvalidate) {
                if (_memoInfos.Contains(info)) {
                    _memoInfos.Remove(info);
                }
                if (_removedMemoInfos.Contains(info)) {
                    _removedMemoInfos.Remove(info);
                }
            }

            return false;
        }

        /// <summary>
        /// Memoオブジェクトがとれなかったりmementoファイルが存在しなかったinfoを削除する。
        /// </summary>
        private void RemoveInvalidMemoInfos(MemoInfoCollection infos) {
            if (infos == null || !infos.Any()) {
                return;
            }

            var invalids = new List<MemoInfo>();
            foreach (var info in infos) {
                try {
                    var memo = _container.Find<Memo>(info.MemoId);
                    if (memo == null || _memoAccessor.IsMementoExists(info.MementoId)) {
                        invalids.Add(info);
                    }
                } catch (Exception e) {
                    Logger.Warn("Find memo or test file exist failed", e);
                    if (!invalids.Contains(info)) {
                        invalids.Add(info);
                    }
                }
            }

            /// Memoオブジェクトがとれなかったりmementoファイルが存在しなかったinfoは削除
            if (invalids.Any()) {
                foreach (var invalid in invalids) {
                    infos.Remove(invalid);
                    Logger.Warn("Remove invalid memo info: " + invalid.Dump());
                }
            }
        }

        private void AddRecentlyCreatedMemoInfo(MemoInfo info) {
            lock (_lock) {
                var id = info.MemoId;
                var ids = _recentlyCreatedMemoIds.Value;
                if (ids.Contains(id)) {
                    ids.Remove(id);
                }
                ids.Add(id);
                if (ids.Count > _settings.RecentMax) {
                    for (int i = ids.Count; i > _settings.RecentMax; --i) {
                        ids.RemoveAt(0);
                    }
                }
            }
        }

        private void AddRecentlyClosedMemoInfo(MemoInfo info) {
            var id = info.MemoId;
            var ids = _recentlyClosedMemoIds.Value;
            if (ids.Contains(id)) {
                ids.Remove(id);
            }
            if (_MemoInfos.Contains(info)) {
                ids.Add(id);
            }
            if (ids.Count > _settings.RecentMax) {
                for (int i = ids.Count; i > _settings.RecentMax; --i) {
                    ids.RemoveAt(0);
                }
            }
            OnRecentlyClosedMemoInfosChanged();
        }

        private void AddRecentlyClosedMemoInfos(IEnumerable<MemoInfo> infos) {
            var ids = _recentlyClosedMemoIds.Value;
            foreach (var info in infos) {
                var id = info.MemoId;
                if (ids.Contains(id)) {
                    ids.Remove(id);
                }
                if (_MemoInfos.Contains(info)) {
                    ids.Add(id);
                }
            }
            if (ids.Count > _settings.RecentMax) {
                for (int i = ids.Count; i > _settings.RecentMax; --i) {
                    ids.RemoveAt(0);
                }
            }
            OnRecentlyClosedMemoInfosChanged();
        }

        private void RemoveRecentlyClosedMemoInfos(IEnumerable<MemoInfo> infos) {
            var ids = _recentlyClosedMemoIds.Value;
            foreach (var info in infos) {
                var id = info.MemoId;
                if (ids.Contains(id)) {
                    ids.Remove(id);
                }
            }
            OnRecentlyClosedMemoInfosChanged();
        }

        private void ClearRemovedEmbeddedFiles() {
            var succeeded = new List<string>();
            foreach (var embeddedId in _removedEmbeddedFileIds.Value) {
                try {
                    var filePath = Path.Combine(MemopadConsts.EmbeddedFileRoot, embeddedId);
                    if (Directory.Exists(filePath)) {
                        var info = new DirectoryInfo(filePath);
                        foreach (var file in info.GetFiles()) {
                            file.IsReadOnly = false;
                        }
                        Directory.Delete(filePath, true);
                    }
                    succeeded.Add(embeddedId);

                } catch (Exception e) {
                    Logger.Warn("Can't Delete: embeddedId=" + embeddedId, e);
                }
            }

            foreach (var embeddedId in succeeded) {
                if (_removedEmbeddedFileIds.Value.Contains(embeddedId)) {
                    _removedEmbeddedFileIds.Value.Remove(embeddedId);
                }
            }
        }

        //private IEnumerable<string> GetMemoFileEmbeddedIds() {
        //    var values = _container.Store.GetPropertyValues(typeof(Mkamo.Model.Memo.MemoFile), "EmbeddedId");
        //    foreach (var val in values) {
        //        if (!string.IsNullOrEmpty(val)) {
        //            yield return val;
        //        }
        //    }
        //}

        private void ClearRemovedEmbeddedImages() {
            var succeeded = new List<string>();
            foreach (var embeddedId in _removedEmbeddedImageIds.Value) {
                try {
                    var imagePath = Path.Combine(MemopadConsts.EmbeddedImageRoot, embeddedId);
                    if (File.Exists(imagePath)) {
                        var file = new FileInfo(imagePath);
                        file.IsReadOnly = false;
                        file.Delete();
                    }
                    succeeded.Add(embeddedId);

                } catch (Exception e) {
                    Logger.Warn("Can't Delete: embeddedId=" + embeddedId, e);
                }
            }

            foreach (var embeddedId in succeeded) {
                if (_removedEmbeddedImageIds.Value.Contains(embeddedId)) {
                    _removedEmbeddedImageIds.Value.Remove(embeddedId);
                }
            }
        }

        private void SyncTabsWithActiveFolder(MemoFolder folder) {
            lock (_lock) {
                _MainForm.EnsureFocusCommited();

                /// 先に保存しないとtabPageが閉じられて保存対象にならない
                SaveAllMemos();

                var closeds = _MainForm.CloseAllPages();
                AddRecentlyClosedMemoInfos(closeds);

                if (folder != null) {
                    var infos = folder.ContainingMemos.Select(memo => FindMemoInfo(memo));
                    LoadAllMemos(infos);
                }
            }
        }

        // --- event handler for info ---
        private void HandleMemoInfoPropChanged(object sender, PropertyChangedEventArgs e) {
            OnMemoInfoChanged((MemoInfo) sender);
        }

        private void HandleMemoFolderRemoving(object sender, MemoFolderEventArgs e) {
            if (e.Folder == _activeFolder) {
                ActiveFolder = null;
            }
        }

        private void HandleMemoSmartFilterRemoving(object sender, MemoSmartFilterEventArgs e) {
            if (e.SmartFilter == _activeSmartFilter) {
                ActiveSmartFilter = null;
            }
        }

        private void HandleEntityPersisted(object sender, EntityEventArgs e) {
            if (e.EntityType == typeof(MemoFile)) {
                var file = e.Entity as MemoFile;
                if (file != null && file.IsEmbedded) {
                    /// 埋め込まれたMemoFileのファイルを復活
                    /// Create時に呼ばれた時はまだIsEmbeddedがfalseなので処理されないようになっている
                    var fileId = file.EmbeddedId;
                    if (_removedEmbeddedFileIds.Value.Contains(fileId)) {
                        _removedEmbeddedFileIds.Value.Remove(fileId);
                    }
                }
            } else if (e.EntityType == typeof(MemoImage)) {
                var image = e.Entity as MemoImage;
                if (image != null) {
                    /// 埋め込まれたMemoImageのファイルを復活
                    /// Create時に呼ばれた時はまだdescがnullなので処理されないようになっている
                    var desc = image.Image as FileImageDescription;
                    if (desc != null) {
                        var imageId = Path.GetFileName(desc.Filename);
                        if (_removedEmbeddedImageIds.Value.Contains(imageId)) {
                            _removedEmbeddedImageIds.Value.Remove(imageId);
                        }
                    }
                }
            }
        }

        private void HandleEntityRemoving(object sender, EntityEventArgs e) {
            if (e.EntityType == typeof(MemoFile)) {
                var file = e.Entity as MemoFile;
                if (file != null && file.IsEmbedded) {
                    /// 埋め込まれたMemoFileのファイルを削除予約
                    _removedEmbeddedFileIds.Value.Add(file.EmbeddedId);
                }
            } else if (e.EntityType == typeof(MemoImage)) {
                var image = e.Entity as MemoImage;
                var desc = image.Image as FileImageDescription;
                var imageId = Path.GetFileName(desc.Filename);
                if (image != null) {
                    /// 埋め込まれたMemoFileのファイルを削除予約
                    _removedEmbeddedImageIds.Value.Add(imageId);
                }
            }
        }

        // ========================================
        // type
        // ========================================
        private class CustomOffice2010Renderer: RenderOffice2010 {
            private Color _panelColor;

            public CustomOffice2010Renderer(Color panelColor) {
                _panelColor = panelColor;
            }

            public override ToolStripRenderer RenderToolStrip(IPalette colorPalette) {
                var ret = base.RenderToolStrip(colorPalette);
                ret.RenderToolStripBorder += RenderToolStripBorder;
                return ret;
            }

            private void RenderToolStripBorder(object sender, ToolStripRenderEventArgs e) {
                if (e.ToolStrip.GetType() == typeof(ToolStrip)) {
                    /// ToolStripの下辺，右辺の線が見えなくなるようにパネルの色で上書きする。
                    /// パレットの色の設定だけでは，StatusStripのボーダーもいっしょに消えたりしてしまうなど，
                    /// ToolStripのボーダーだけを消すことができないので。
                    using (var pen = new Pen(_panelColor)) {
                        e.Graphics.DrawLine(pen, e.AffectedBounds.Right - 1, e.AffectedBounds.Top, e.AffectedBounds.Right - 1, e.AffectedBounds.Bottom - 1);
                        e.Graphics.DrawLine(pen, e.AffectedBounds.Left, e.AffectedBounds.Bottom - 1, e.AffectedBounds.Right - 1, e.AffectedBounds.Bottom - 1);
                        e.Graphics.DrawLine(pen, e.AffectedBounds.Right - 2, e.AffectedBounds.Bottom - 2, e.AffectedBounds.Right - 2, e.AffectedBounds.Bottom - 1);
                    }
                }
            }
        }


    }
}
