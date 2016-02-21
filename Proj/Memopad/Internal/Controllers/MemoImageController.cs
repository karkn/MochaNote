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
using Mkamo.Memopad.Internal.Helpers;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Common.Core;
using Mkamo.Memopad.Internal.Controllers.UIProviders;
using System.Drawing.Imaging;
using Mkamo.Memopad.Core;
using System.IO;
using Mkamo.Common.IO;
using Mkamo.Common.Forms.Drawing;

namespace Mkamo.Memopad.Internal.Controllers {
    internal class MemoImageController: AbstractMemoContentController<MemoImage, ImageFigure> {
        // ========================================
        // field
        // ========================================
        private Lazy<IUIProvider> _uiProvider;

        // ========================================
        // constructor
        // ========================================
        internal MemoImageController() {
            _uiProvider = new Lazy<IUIProvider>(() => new MemoImageUIProvider(this));
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
            var editorHandle = new MoveEditorHandle() {
                Cursor = Cursors.SizeAll
            };
            var facade = MemopadApplication.Instance;
            editorHandle.KeyMap = facade.KeySchema.MemoContentEditorKeyMap;
            editor.InstallEditorHandle(editorHandle);

            editor.InstallHandle(
                new ResizeHandle(Directions.Left) {
                    Cursor = Cursors.SizeWE
                }
            );
            editor.InstallHandle(
                new ResizeHandle(Directions.Up) {
                    Cursor = Cursors.SizeNS
                }
            );
            editor.InstallHandle(
                new ResizeHandle(Directions.Right) {
                    Cursor = Cursors.SizeWE
                }
            );
            editor.InstallHandle(
                new ResizeHandle(Directions.Down) {
                    Cursor = Cursors.SizeNS
                }
            );
            editor.InstallHandle(
                new ResizeHandle(Directions.UpLeft) {
                    Cursor = Cursors.SizeNWSE
                }
            );
            editor.InstallHandle(
                new ResizeHandle(Directions.UpRight) {
                    Cursor = Cursors.SizeNESW
                }
            );
            editor.InstallHandle(
                new ResizeHandle(Directions.DownLeft) {
                    Cursor = Cursors.SizeNESW
                }
            );
            editor.InstallHandle(
                new ResizeHandle(Directions.DownRight) {
                    Cursor = Cursors.SizeNWSE
                }
            );

            editor.InstallRole(new SelectRole());
            editor.InstallRole(new ResizeRole());
            editor.InstallRole(new RemoveRole());
            editor.InstallRole(new CopyRole());
            editor.InstallRole(new ReorderRole());

            var export = new ExportRole();
            export.RegisterExporter("PNG", ExportPngFile);
            export.RegisterExporter("JPEG", ExportJpegFile);
            editor.InstallRole(export);
        }

        protected override ImageFigure CreateFigure(MemoImage model) {
            var ret = new ImageFigure();
            ret.IsForegroundEnabled = false;
            return ret;
        }

        protected override void RefreshEditor(RefreshContext context, ImageFigure figure, MemoImage model) {
            figure.ImageDesc = model.Image;
            figure.AdjustSize();

            UpdateMemoMarkHandles(model);
        }

        public override IMemento GetModelMemento() {
            var externalizer = new Externalizer();
            return externalizer.Save(Model, (key, obj) => false);
        }

        public override string GetText() {
            var ret = "";
            if (Model.Keywords != null) {
                ret = Model.Keywords;
            }
            return ret;
        }

        public override object GetTranserInitArgs() {
            var desc = Model.Image as FileImageDescription;
            if (desc == null) {
                return null;
            } else {
                return desc.GetFullPath();
            }
        }

        public override TransferInitializer GetTransferInitializer() {
            return InitTransfered;
        }

        /// <summary>
        /// クリップボードに格納するためにSerializable，すなわちstaticにしておく。
        /// </summary>
        public static void InitTransfered(IEditor transfered, object args) {
            var transferedModel = transfered.Model as MemoImage;
            if (transferedModel == null) {
                return;
            }

            /// 埋め込み画像ファイルをコピー
            var originalFilePath = (string) args;
            var id = Guid.NewGuid().ToString();
            var newFilePath = Path.Combine(MemopadConsts.EmbeddedImageRoot, id);

            PathUtil.EnsureDirectoryExists(MemopadConsts.EmbeddedImageRoot);
            File.Copy(originalFilePath, newFilePath);

            transferedModel.Image = new FileImageDescription(id);
        }

        // ------------------------------
        // private
        // ------------------------------
        // --- export ---
        private void ExportPngFile(IEditor editor, string outputPath) {
            var desc = Model.Image;
            using (var image = desc.CreateImage()) {
                image.Save(outputPath, ImageFormat.Png);
            }
        }

        private void ExportJpegFile(IEditor editor, string outputPath) {
            var desc = Model.Image;
            using (var image = desc.CreateImage()) {
                var codec = ImageUtil.GetCodec(ImageFormat.Jpeg);
                var para = ImageUtil.CreateEncoderParameters(100);
                image.Save(outputPath, codec, para);
            }
        }
    }
}
