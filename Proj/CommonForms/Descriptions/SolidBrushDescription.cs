/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Mkamo.Common.Forms.Util;

namespace Mkamo.Common.Forms.Descriptions {
    /// <summary>
    /// SolidBrushの情報を格納するクラス．Immutable．
    /// </summary>
    [Serializable]
    public class SolidBrushDescription: IBrushDescription {
        // ========================================
        // field
        // ========================================
        private Color _color;
        private float _opacity;

        // ========================================
        // constructor
        // ========================================
        public SolidBrushDescription(Color color, float opacity) {
            _color = color;
            _opacity = opacity > 1? 1: opacity;
        }

        public SolidBrushDescription(): this(Color.Black, 1) {
        }

        public SolidBrushDescription(Color color): this(color, 1) {
        }

        // ========================================
        // property
        // ========================================
        public BrushKind Kind {
            get { return BrushKind.Solid; }
        }

        public bool IsDark {
            get { return ColorUtil.IsDark(_color); }
        }

        public Color Color {
            get { return _color; }
        }

        public float Opacity {
            get { return _opacity; }
        }

        // ========================================
        // method
        // ========================================
        public Brush CreateBrush(Rectangle bounds) {
            if (Math.Abs(_opacity - 1) <= float.Epsilon * _opacity) {
                return new SolidBrush(_color);
            } else {
                return new SolidBrush(Color.FromArgb((int) (_opacity * 255), _color));
            }
        }

        public object Clone() {
            return MemberwiseClone();
        }

    }
}
