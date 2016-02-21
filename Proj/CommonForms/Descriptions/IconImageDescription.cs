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

namespace Mkamo.Common.Forms.Descriptions {
    /// <summary>
    /// Icon形式のImage情報を格納するクラス．Immutable．
    /// </summary>
    [Serializable]
    public class IconImageDescription: IImageDescription {
        // ========================================
        // field
        // ========================================
        private Icon _icon;

        // ========================================
        // constructor
        // ========================================
        public IconImageDescription(Icon icon) {
            _icon = icon.Clone() as Icon;
        }

        // ========================================
        // property
        // ========================================
        public ImageKind Kind {
            get { return ImageKind.Bytes; }
        }

        public Icon Icon {
            get { return _icon; }
        }

        // ========================================
        // method
        // ========================================
        /// <summary>
        /// Imageとは別にStreamもDispose()しないといけないのでImageUsingContextとしてまとめて返す．
        /// </summary>
        /// <returns></returns>
        public Image CreateImage() {
            return _icon.ToBitmap();
        }

        public object Clone() {
            var ret = new IconImageDescription(_icon == null? null: _icon.Clone() as Icon);
            return ret;
        }
    }
}
