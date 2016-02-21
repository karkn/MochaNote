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
using Mkamo.Editor.Core;
using System.IO;
using System.Xml;

namespace Mkamo.Memopad.Core {
    [Serializable, DataContract]
    public class MemopadSettings {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // ========================================
        // static method
        // ========================================
        internal static MemopadSettings LoadSettings() {
            var ret = default(MemopadSettings);

            try {
                if (File.Exists(MemopadConsts.SettingsFilePath)) {
                    var serializer = new DataContractSerializer(typeof(MemopadSettings));
                    using (var stream = new FileStream(MemopadConsts.SettingsFilePath, FileMode.Open, FileAccess.Read))
                    using (var reader = XmlReader.Create(stream)) {
                        ret = serializer.ReadObject(reader) as MemopadSettings;
                    }
                }
            } catch (Exception e) {
                Logger.Warn("Settings load failed", e);
            }

            if (ret == null) {
                ret = new MemopadSettings();
            }

            return ret;
        }

        internal static void SaveSettings(MemopadSettings settings ) {
            var serializer = new DataContractSerializer(typeof(MemopadSettings));
            using (var stream = new FileStream(MemopadConsts.SettingsFilePath, FileMode.Create, FileAccess.Write))
            using (var writer = XmlWriter.Create(stream)) {
                serializer.WriteObject(writer, settings);
            }
        }


        // ========================================
        // field
        // ========================================
        private int _defaultMemoTextFontHeightCache;

        private string _defaultMemoTextFontName;
        private int _defaultMemoTextFontSize;

        // ========================================
        // constructor
        // ========================================
        public MemopadSettings() {
            IsInitialized = false;
            Version = null;

            KeyScheme = KeySchemeKind.Default;

            //DefaultShapeId = "rect";

            DefaultMemoTextFontName = "MeiryoKe_PGothic";
            DefaultMemoTextFontSize = 9;
            DefaultMemoContentFontName = "MeiryoKe_PGothic";
            DefaultMemoContentFontSize = 9;
            DefaultUmlFontName = "MeiryoKe_PGothic";
            DefaultUmlFontSize = 9;

            UseClearType = true;

            ConfirmMemoRemoval = true;
            ConfirmTagRemoval = true;
            ConfirmFolderRemoval = true;
            ConfirmSmartFolderRemoval = true;

            RecentMax = 10;

            EmptyTrashBoxOnExit = false;

            _defaultMemoTextFontHeightCache = 0;

            MemoTextFrameVisiblePolicy = (int) HandleStickyKind.MouseEnter;

            NotifyIconClickAction = NotifyIconActionKind.ShowFusenForms;
            NotifyIconClickAction = NotifyIconActionKind.CreateFusenMemo;

            ConfirmDataConversionFromV1 = true;

            EditorCanvasImeOn = true;

            CheckLatestOnStart = true;
        }


        // ========================================
        // property
        // ========================================
        // --- init ---
        [DataMember]
        public bool IsInitialized { get; set; }

        [DataMember]
        public string Version { get; set; }

        // --- keymap ---
        [DataMember]
        public KeySchemeKind KeyScheme { get; set; }

        // --- memo ---
        /// <summary>
        /// ノートの空白部ダブルクリック時に作成する図形のID
        /// </summary>
        //[DataMember]
        //public string DefaultShapeId { get; set; }

        // --- editor font ---
        [DataMember]
        public string DefaultMemoTextFontName {
            get { return _defaultMemoTextFontName; }
            set {
                if (value == _defaultMemoTextFontName) {
                    return;
                }
                _defaultMemoTextFontName = value;
                _defaultMemoTextFontHeightCache = 0;
            }
        }

        [DataMember]
        public int DefaultMemoTextFontSize {
            get { return _defaultMemoTextFontSize; }
            set {
                if (value == _defaultMemoTextFontSize) {
                    return;
                }
                _defaultMemoTextFontSize = value;
                _defaultMemoTextFontHeightCache = 0;
            }
        }

        [DataMember]
        public string DefaultMemoContentFontName { get; set; }
        
        [DataMember]
        public int DefaultMemoContentFontSize { get; set; }
        
        [DataMember]
        public string DefaultUmlFontName { get; set; }
        
        [DataMember]
        public int DefaultUmlFontSize { get; set; }
        
        [DataMember]
        public bool UseClearType { get; set; }

        // --- confirm ---
        [DataMember]
        public bool ConfirmMemoRemoval { get; set; }
        [DataMember]
        public bool ConfirmTagRemoval { get; set; }
        [DataMember]
        public bool ConfirmFolderRemoval { get; set; }
        [DataMember]
        public bool ConfirmSmartFolderRemoval { get; set; }

        // --- misc ---
        [DataMember]
        public int RecentMax { get; set; }

        [DataMember]
        public bool EmptyTrashBoxOnExit { get; set; }

        [DataMember]
        public int MemoTextFrameVisiblePolicy { get; set; }

        [DataMember]
        public NotifyIconActionKind NotifyIconClickAction { get; set; }

        [DataMember]
        public NotifyIconActionKind NotifyIconDoubleClickAction { get; set; }

        [DataMember]
        public bool ConfirmDataConversionFromV1 { get; set; }

        [DataMember]
        public bool EditorCanvasImeOn { get; set; }

        [DataMember]
        public bool CheckLatestOnStart { get; set; }

        // ========================================
        // method
        // ========================================
        internal FontDescription GetDefaultMemoTextFont() {
            var fontName = DefaultMemoTextFontName;
            var fontSize = DefaultMemoTextFontSize;
            return new FontDescription(fontName, fontSize);
        }

        internal int GetDefaultMemoTextFontHeight() {
            if (_defaultMemoTextFontHeightCache == 0) {
                using (var font = GetDefaultMemoTextFont().CreateFont()) {
                    _defaultMemoTextFontHeightCache = (int) Math.Round(font.GetHeight());
                }
            }
            return _defaultMemoTextFontHeightCache;
        }

        internal FontDescription GetDefaultMemoContentFont() {
            var fontName = DefaultMemoContentFontName;
            var fontSize = DefaultMemoContentFontSize;
            return new FontDescription(fontName, fontSize);
        }

        internal FontDescription GetDefaultUmlFont() {
            var fontName = DefaultUmlFontName;
            var fontSize = DefaultUmlFontSize;
            return new FontDescription(fontName, fontSize);
        }

    }
}
