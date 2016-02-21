/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Figures;
using Mkamo.Figure.Core;
using Mkamo.Common.DataType;
using Mkamo.Common.Forms.Descriptions;
using System.Drawing;
using Mkamo.StyledText.Core;
using Mkamo.Figure.Layouts;
using Mkamo.Figure.Layouts.Locators;
using Mkamo.Model.Memo;
using Mkamo.Model.Core;
using Mkamo.Common.Externalize;
using Mkamo.Editor.Handles;
using System.Windows.Forms;
using Mkamo.Editor.Roles;
using Mkamo.Editor.Focuses;
using Mkamo.Editor.Controllers;
using Mkamo.Editor.Core;
using Mkamo.Memopad.Internal.Utils;
using Mkamo.Editor.Utils;
using Mkamo.Editor.Forms;
using Mkamo.Common.Forms.DetailSettings;
using Mkamo.Memopad.Core;
using System.IO;
using Mkamo.Memopad.Internal.Helpers;
using Mkamo.Memopad.Internal.Figures;
using Mkamo.Memopad.Internal.Handles;
using Mkamo.Common.Win32.User32;
using System.Runtime.InteropServices;
using Mkamo.Memopad.Internal.Controllers.UIProviders;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Common.IO;
using Mkamo.Common.Core;

namespace Mkamo.Memopad.Internal.Controllers {
    internal class MemoFileController: AbstractMemoContentController<MemoFile, MemoFileFigure> {
        // ========================================
        // field
        // ========================================
        private Lazy<IUIProvider> _uiProvider;

        // ========================================
        // constructor
        // ========================================
        internal MemoFileController() {
            _uiProvider = new Lazy<IUIProvider>(() => new MemoFileUIProvider(this));
        }

        // ========================================
        // property
        // ========================================
        public override IUIProvider UIProvider {
            get { return _uiProvider.Value; }
        }

        // ========================================
        // method
        // ========================================
        public override void ConfigureEditor(IEditor editor) {
            var editorHandle = new MemoFileEditorHandle() {
                Cursor = Cursors.SizeAll
            };
            var facade = MemopadApplication.Instance;
            editorHandle.KeyMap = facade.KeySchema.MemoContentEditorKeyMap;
            editor.InstallEditorHandle(editorHandle);

            editor.InstallHandle(new SelectionIndicatingHandle());

            editor.InstallRole(new SelectRole());
            editor.InstallRole(new MoveRole());
            editor.InstallRole(new RemoveRole());
            editor.InstallRole(new ReorderRole());
            editor.InstallRole(new CopyRole());

            var export = new ExportRole();
            export.RegisterExporter("File", ExportFile);
            editor.InstallRole(export);
        }

        protected override MemoFileFigure CreateFigure(MemoFile model) {
            var ret = new MemoFileFigure();
            ret.IsForegroundEnabled = false;
            ret.Font = MemopadApplication.Instance.Settings.GetDefaultMemoContentFont();
            return ret;
        }

        protected override void RefreshEditor(RefreshContext context, MemoFileFigure figure, MemoFile model) {
            var path = model.Path;
            if (model.IsEmbedded) {
                path = Path.Combine(MemopadConsts.EmbeddedFileRoot, path);
            }

            var icon = default(Icon);
            var hIcon = IntPtr.Zero;
            if (File.Exists(path) || Directory.Exists(path)) {
                /// アイコン取得
                var shinfo = new SHFILEINFO();
                var flags = model.IsEmbedded?
                    ShellGetFileInfoFlags.Icon:
                    ShellGetFileInfoFlags.Icon | ShellGetFileInfoFlags.LinkOverlay;
                var hSuccess = Shell32PI.SHGetFileInfo(
                    path,
                    0,
                    ref shinfo,
                    (int) Marshal.SizeOf(shinfo),
                    flags
                );
                if (hSuccess != IntPtr.Zero) {
                    hIcon = shinfo.hIcon;
                    icon = Icon.FromHandle(hIcon);
                } else {
                    icon = SystemIcons.Error;
                }
            } else {
                icon = SystemIcons.Error;
            }

            var desc = new IconImageDescription(icon);
            if (hIcon != IntPtr.Zero) {
                User32PI.DestroyIcon(hIcon);
            }

            figure.Text = string.IsNullOrEmpty(model.Name)? model.Path: model.Name;
            figure.ImageDesc = desc;
            figure.AdjustSize();

            UpdateMemoMarkHandles(model);
        }

        public override IMemento GetModelMemento() {
            var externalizer = new Externalizer();
            return externalizer.Save(Model, (key, obj) => false);
        }

        public override object GetTranserInitArgs() {
            if (Model.IsEmbedded) {
                return MemoFileEditorHelper.GetFullPath(Model);
            } else {
                return null;
            }
        }

        public override TransferInitializer GetTransferInitializer() {
            if (Model.IsEmbedded) {
                return InitTransfered;
            } else {
                return null;
            }
        }

        /// <summary>
        /// クリップボードに格納するためにSerializable，すなわちstaticにしておく。
        /// </summary>
        public static void InitTransfered(IEditor transfered, object args) {
            var transferedModel = transfered.Model as MemoFile;
            if (transferedModel == null || !transferedModel.IsEmbedded) {
                return;
            }

            /// 埋め込みファイルをコピー
            var originalFilePath = (string) args;
            var id = Guid.NewGuid().ToString();
            var newDirPath = Path.Combine(MemopadConsts.EmbeddedFileRoot, id);
            var newFilePath = Path.Combine(newDirPath, Path.GetFileName(originalFilePath));

            PathUtil.EnsureDirectoryExists(newDirPath);
            File.Copy(originalFilePath, newFilePath);

            transferedModel.EmbeddedId = id;
            transferedModel.Path = Path.Combine(id, Path.GetFileName(newFilePath));
        }


        public override string GetText() {
            var ret = Model.Name;
            if (Model.Keywords != null) {
                ret = ret + Environment.NewLine + Model.Keywords;
            }
            return ret;
        }

        //public override void DisposeModel(object model) {
        //    /// .embededのファイルを削除予約
        //    var filePath = MemoFileEditorHelper.GetEmbeddedFileDirectoryPath(Model);
        //    var trashPath = MemoFileEditorHelper.GetEmbeddedTrashDirectoryPath(Model);
        //    if (Directory.Exists(trashPath)) {
        //        Directory.Delete(trashPath, true);
        //    }
        //    Directory.Move(filePath, trashPath);

        //    base.DisposeModel(model);
        //}

        //public override void RestoreModel(object model) {
        //    base.RestoreModel(model);

        //    /// .embeddedのファイルの削除予約解除
        //    var filePath = MemoFileEditorHelper.GetEmbeddedFileDirectoryPath(Model);
        //    var trashPath = MemoFileEditorHelper.GetEmbeddedTrashDirectoryPath(Model);
        //    if (Directory.Exists(filePath)) {
        //        Directory.Delete(filePath, true);
        //    }
        //    Directory.Move(trashPath, filePath);
        //}

        private void ExportFile(IEditor editor, string outputPath) {
            var memoFile = Model;
            var path = MemoFileEditorHelper.GetFullPath(memoFile);
            if (File.Exists(path)) {
                File.Copy(path, outputPath, true);
            }
        }
    }
}
