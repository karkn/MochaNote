/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using Mkamo.Common.Forms.Descriptions;
using System.Runtime.Serialization;
using System.IO;
using Mkamo.Common.Forms.Drawing;
using System.Xml;

namespace Mkamo.Memopad.Core {
    [Serializable, DataContract]
    public class MemopadWindowSettings {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // ========================================
        // static method
        // ========================================
        internal static MemopadWindowSettings LoadWindowSettings() {
            var ret = default(MemopadWindowSettings);

            try {
                if (File.Exists(MemopadConsts.WindowSettingsFilePath)) {
                    var serializer = new DataContractSerializer(typeof(MemopadWindowSettings));
                    using (
                        var stream = new FileStream(MemopadConsts.WindowSettingsFilePath, FileMode.Open, FileAccess.Read)
                    )
                    using (var reader = XmlReader.Create(stream)) {
                        ret = serializer.ReadObject(reader) as MemopadWindowSettings;
                    }
                }
            } catch (Exception e) {
                Logger.Warn("Window settings load failed", e);
            }

            if (ret == null) {
                ret = new MemopadWindowSettings();
            }

            return ret;
        }

        internal static void SaveWindowSettings(MemopadWindowSettings windowSettings) {
            var serializer = new DataContractSerializer(typeof(MemopadWindowSettings));
            using (var stream = new FileStream(MemopadConsts.WindowSettingsFilePath, FileMode.Create, FileAccess.Write))
            using (var writer = XmlWriter.Create(stream)) {
                serializer.WriteObject(writer, windowSettings);
            }
        }


        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================
        public MemopadWindowSettings() {
            Theme = ThemeKind.Silver;

            WindowLocation = Point.Empty;
            WindowSize = new Size(900, 700);
            CompactWindowLocation = Point.Empty;
            CompactWindowSize = new Size(540, 380);
            WindowTopMost = false;
            WindowMaximized = false;

            WorkspaceSplitterDistance = 200;
            WorkspaceSplitterCollapsed = false;
            MemoListSplitterDistance = 200;
            MemoListSplitterCollapsed = false;

            ShowLineBreak = false;
            ShowBlockBreak = false;

            ShowDescendantTagsMemo = false;

            MemoListBoxDisplayItems = new[] {
                MemoListBoxDisplayItem.Title,
                MemoListBoxDisplayItem.CreatedDate,
                MemoListBoxDisplayItem.Tag,
            };
            MemoListBoxSortKey = MemoListBoxDisplayItem.Title;
            MemoListBoxSortsAscendingOrder = true;
            MemoListBoxSortsImportanceOrder = true;

            ShowStartPageOnStart = true;

            CreateMemoHotKey = "Control+Alt+A";
            CreateMemoHotKey = "Control+Alt+N";
            ClipMemoHotKey = "Control+Alt+C";
            CaptureScreenHotKey = "Control+Alt+D";

            SubInfoShown = false;

            MemoEditorBackgroundImageEnabled = false;
            MemoEditorBackgroundImageFilePath = "";
            MemoEditorBackgroundImageOpacityPercent = 0;
            MemoEditorBackgroundImageScalePercent = 100;

            MemoTextDefaultMaxWidth = -1;

            MinimizeToTaskTray = false;
            MinimizeOnStartUp = false;

            ReplaceMeiryoWithMeiryoUI = true;
        }


        // ========================================
        // property
        // ========================================
        // --- init ---
        [DataMember]
        public string Version { get; set; }

        // --- theme ---
        [DataMember]
        public ThemeKind Theme { get; set; }

        // --- backup ---
        [DataMember]
        public string BackupRoot { get; set; }

        // --- form ---
        [DataMember]
        public Point WindowLocation { get; set; }

        [DataMember]
        public Size WindowSize { get; set; }

        [DataMember]
        public Point CompactWindowLocation { get; set; }

        [DataMember]
        public Size CompactWindowSize { get; set; }

        [DataMember]
        public bool WindowTopMost { get; set; }

        [DataMember]
        public bool WindowMaximized { get; set; }

        // --- splitter ---
        [DataMember]
        public int MemoListSplitterDistance { get; set; }

        [DataMember]
        public int WorkspaceSplitterDistance { get; set; }
        
        [DataMember]
        public bool WorkspaceSplitterCollapsed { get; set; }

        [DataMember]
        public bool MemoListSplitterCollapsed { get; set; }

        // --- editor ---
        [DataMember]
        public bool ShowLineBreak { get; set; }
        [DataMember]
        public bool ShowBlockBreak { get; set; }

        // --- workspace ---
        [DataMember]
        public bool ShowDescendantTagsMemo { get; set; }
        
        // --- memo list box ---
        [DataMember]
        public MemoListBoxDisplayItem[] MemoListBoxDisplayItems { get; set; }

        [DataMember]
        public MemoListBoxDisplayItem MemoListBoxSortKey { get; set; }

        [DataMember]
        public bool MemoListBoxSortsAscendingOrder { get; set; }

        [DataMember]
        public bool MemoListBoxSortsImportanceOrder { get; set; }

        // --- start page ---
        [DataMember]
        public bool ShowStartPageOnStart { get; set; }

        // --- hot key ---
        [DataMember]
        public string ActivateHotKey { get; set; }
        [DataMember]
        public string CreateMemoHotKey { get; set; }
        [DataMember]
        public string ClipMemoHotKey { get; set; }
        [DataMember]
        public string CaptureScreenHotKey { get; set; }

        // --- page content ---
        [DataMember]
        public bool SubInfoShown { get; set; }

        // --- background ---
        [DataMember]
        public bool MemoEditorBackgroundImageEnabled { get; set; }

        [DataMember]
        public string MemoEditorBackgroundImageFilePath { get; set; }

        [DataMember]
        public int MemoEditorBackgroundImageOpacityPercent { get; set; }

        [DataMember]
        public int MemoEditorBackgroundImageScalePercent { get; set; }

        // --- memotext ---
        [DataMember]
        public int MemoTextDefaultMaxWidth { get; set; }

        // --- minimamize ---
        [DataMember]
        public bool MinimizeToTaskTray { get; set; }

        [DataMember]
        public bool MinimizeOnStartUp { get; set; }

        // --- font ---
        [DataMember]
        public bool ReplaceMeiryoWithMeiryoUI { get; set; }

        // --- mail ---
        [DataMember]
        public string SmtpServer { get; set; }

        [DataMember]
        public int SmtpPort { get; set; }

        [DataMember]
        public bool SmtpEnableAuth { get; set; }

        [DataMember]
        public string SmtpUserName { get; set; }

        [DataMember]
        public string SmtpPassword { get; set; }

        [DataMember]
        public bool SmtpEnableSsl { get; set; }

        [DataMember]
        public string MailTo { get; set; }

        [DataMember]
        public string MailFrom { get; set; }

        // ========================================
        // method
        // ========================================
        internal string GetDailyBackupDirPath() {
            return Path.Combine(BackupRoot, "daily");
        }

        internal string GetWeeklyBackupDirPath() {
            return Path.Combine(BackupRoot, "weekly");
        }

        internal string GetMonthlyBackupDirPath() {
            return Path.Combine(BackupRoot, "monthly");
        }

        internal Image CreateMemoEditorBackgroundImage() {
            if (!MemoEditorBackgroundImageEnabled) {
                return null;
            }

            try {
                if (!File.Exists(MemoEditorBackgroundImageFilePath)) {
                    return null;
                }
                if (MemoEditorBackgroundImageOpacityPercent == 0) {
                    return null;
                }
                if (MemoEditorBackgroundImageScalePercent == 0) {
                    return null;
                }

                using (var img = Image.FromFile(MemoEditorBackgroundImageFilePath)) {
                    var imgAttrs = MemoEditorBackgroundImageOpacityPercent == 100 ?
                        null :
                        ImageUtil.GetImageAttributes((float) MemoEditorBackgroundImageOpacityPercent / 100);
    
                    var imgScale = MemoEditorBackgroundImageScalePercent == 100 ?
                        1:
                        (float) MemoEditorBackgroundImageScalePercent / 100;
    
                    var imgSize = imgScale == 1 ?
                        img.Size :
                        new Size((int) (img.Width * imgScale), (int) (img.Height * imgScale));
    
                    var ret = new Bitmap(imgSize.Width, imgSize.Height);
                    using (var g = Graphics.FromImage(ret)) {
                        GraphicsUtil.SetupGraphics(g, GraphicQuality.MaxQuality);
                        if (imgAttrs == null) {
                            g.DrawImage(
                                img,
                                new Rectangle(0, 0, imgSize.Width, imgSize.Height),
                                0,
                                0,
                                img.Width,
                                img.Height,
                                GraphicsUnit.Pixel
                            );
                        } else {
                            g.DrawImage(
                                img,
                                new Rectangle(0, 0, imgSize.Width, imgSize.Height),
                                0,
                                0,
                                img.Width,
                                img.Height,
                                GraphicsUnit.Pixel,
                                imgAttrs
                            );
                            imgAttrs.Dispose();
                        }
                    }
                    return ret;
                }

            } catch (Exception e) {
                Logger.Warn("Can't load background image.", e);
                return null;
            }
        }
    }
}
