/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Mkamo.Common.Forms.Descriptions {
    /// <summary>
    /// Stream形式のImage情報を格納するクラス．Immutable．
    /// </summary>
    [Serializable]
    public class BytesImageDescription: IImageDescription {
        // ========================================
        // field
        // ========================================
        private byte[] _bytes;

        // ========================================
        // constructor
        // ========================================
        public BytesImageDescription(byte[] bytes) {
            _bytes = bytes;
        }

        public BytesImageDescription(Image image) {
            using (var stream = new MemoryStream()) {
                image.Save(stream, ImageFormat.Png);
                _bytes = stream.ToArray();
            }
        }

        // ========================================
        // property
        // ========================================
        public ImageKind Kind {
            get { return ImageKind.Bytes; }
        }

        public byte[] Bytes{
            get { return _bytes; }
        }

        // ========================================
        // method
        // ========================================
        /// <summary>
        /// Imageとは別にStreamもDispose()しないといけないのでImageUsingContextとしてまとめて返す．
        /// </summary>
        /// <returns></returns>
        //public ImageUsingContext CreateImage() {
        //    var stream = new BufferedStream(new MemoryStream(_bytes));
        //    return new ImageUsingContext(Image.FromStream(stream), new [] { stream });
        //}
        public Image CreateImage() {
            var conv = new ImageConverter();
            return (Image) conv.ConvertFrom(_bytes);
        }

        public object Clone() {
            return MemberwiseClone();
        }
    }
}
