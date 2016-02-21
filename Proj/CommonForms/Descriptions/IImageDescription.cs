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
    [Serializable]
    public enum ImageKind {
        File,
        Bytes,
        Icon,
    }

    public interface IImageDescription: ICloneable {
        // ========================================
        // property
        // ========================================
        ImageKind Kind { get; }

        // ========================================
        // method
        // ========================================
        //ImageUsingContext CreateImage();
        Image CreateImage();
    }

}
