/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Mkamo.Common.Forms.Descriptions {
    /// <summary>
    /// Imageと関連してDispose()しなければならないリソースをまとめたクラス．
    /// </summary>
    public class ImageUsingContext: IDisposable {
        // ========================================
        // field
        // ========================================
        private Image _image;
        private IEnumerable<IDisposable> _disposables;

        // ========================================
        // constructor
        // ========================================
        public ImageUsingContext(Image image, IEnumerable<IDisposable> disposables) {
            _image = image;
            _disposables = disposables;
        }

        // ========================================
        // property
        // ========================================
        public Image Image {
            get { return _image; }
        }

        // ========================================
        // method
        // ========================================
        public void Dispose() {
            _image.Dispose();
            foreach (var disposable in _disposables) {
                disposable.Dispose();
            }
        }

    }
}
