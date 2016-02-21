/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;

namespace Mkamo.Common.Forms.Descriptions {
    /// <summary>
    /// Image情報を格納するクラス．Immutable．
    /// </summary>
    [Serializable]
    public class FileImageDescription: IImageDescription {
        // ========================================
        // static field
        // ========================================
        public static string RootPath = "";

        // ========================================
        // field
        // ========================================
        private string _filename;

        // ========================================
        // constructor
        // ========================================
        public FileImageDescription(string filename) {
            _filename = filename;
        }

        // ========================================
        // property
        // ========================================
        public ImageKind Kind {
            get { return ImageKind.File; }
        }

        public string Filename {
            get { return _filename; }
        }

        // ========================================
        // method
        // ========================================
        public Image CreateImage() {
            return Image.FromFile(GetFullPath());
        }

        public object Clone() {
            return MemberwiseClone();
        }

        public string GetFullPath() {
            if (File.Exists(_filename)) {
                return _filename;
            } else {
                return Path.Combine(RootPath, _filename);
            }
        }
    }
}
