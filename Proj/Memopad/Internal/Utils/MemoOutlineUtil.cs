/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Memopad.Core;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Editor.Core;
using System.Drawing;
using System.Drawing.Imaging;
using Mkamo.Model.Memo;
using System.IO;

namespace Mkamo.Memopad.Internal.Utils {
    internal static class MemoOutlineUtil {
        private const float PreviewRatio = 0.9f;
        private static readonly Size OutlineMargin = new Size(8, 8);

        public static Bitmap CreateOutline(MemoInfo info) {
            var facade = MemopadApplication.Instance;
            
            using (var canvas = new EditorCanvas()) {
                MemoSerializeUtil.LoadEditor(canvas, info.MementoId);
                return CreateOutline(canvas);
            }
        }

        public static Bitmap CreateOutline(EditorCanvas canvas) {
            return CreateOutline(canvas, PreviewRatio);
        }

        public static Bitmap CreateOutline(EditorCanvas canvas, float ratio) {
            var tmp = canvas.ImageCopyRightString;
            try {
                canvas.ImageCopyRightString = null;
                return canvas.CreateBitmap(ratio, OutlineMargin);
            } finally {
                canvas.ImageCopyRightString = tmp;
            }
        }

        public static void SaveOutline(EditorCanvas canvas) {
            var memo = canvas.EditorContent as Memo;
            var facade = MemopadApplication.Instance;

            using (var outline = CreateOutline(canvas)) {
                if (outline != null) {
                    var conv = new ImageConverter();
                    var bytes = (byte[]) conv.ConvertTo(outline, typeof(byte[]));
                    facade.Container.SaveExtendedBinaryData(memo, "Outline", bytes);
                }
            }
        }

        public static void SaveOutline(MemoInfo info) {
            var facade = MemopadApplication.Instance;
            var container = facade.Container;
            var memo = container.Find<Memo>(info.MemoId);

            using (var outline = CreateOutline(info)) {
                if (outline != null) {
                    var conv = new ImageConverter();
                    var bytes = (byte[]) conv.ConvertTo(outline, typeof(byte[]));
                    facade.Container.SaveExtendedBinaryData(memo, "Outline", bytes);
                }
            }
        }

        public static Bitmap LoadOutline(MemoInfo info) {
            var facade = MemopadApplication.Instance;
            var container = facade.Container;
            var memo = container.Find<Memo>(info.MemoId);

            var bytes = container.LoadExtendedBinaryData(memo, "Outline");
            if (bytes == null) {
                return null;
            }
            var conv = new ImageConverter();
            return (Bitmap) conv.ConvertFrom(bytes);
        }

        public static Bitmap LoadOrSaveAndLoadOutline(MemoInfo info) {
            var ret = LoadOutline(info);
            if (ret == null) {
                SaveOutline(info);
                return LoadOutline(info);
            } else {
                return ret;
            }
        }
    }
}
