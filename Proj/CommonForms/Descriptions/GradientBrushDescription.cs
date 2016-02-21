/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using Mkamo.Common.Forms.Util;

namespace Mkamo.Common.Forms.Descriptions {
    /// <summary>
    /// GradientBrush情報を格納するクラス．Immutable．
    /// </summary>
    [Serializable]
    public class GradientBrushDescription: IBrushDescription {
        // ========================================
        // field
        // ========================================
        private Color _color1;
        private Color _color2;

        private float _angle;
        private BlendDescription _blend;
        private ColorBlendDescription _colorBlend;

        private float _opacity;

        // ========================================
        // constructor
        // ========================================
        public GradientBrushDescription(
            Color color1, Color color2, BlendDescription blend, float angle, float opacity
        ) {
            _color1 = color1;
            _color2 = color2;

            _blend = blend;
            _colorBlend = null;

            _angle = angle;

            _opacity = opacity > 1? 1: opacity;
        }

        public GradientBrushDescription(
            Color color1, Color color2, BlendDescription blend, float angle
        )
            : this(color1, color2, blend, angle, 1)
        {
        }        

        public GradientBrushDescription(
            Color color1, Color color2, float angle
        )
            : this(color1, color2, null, angle, 1)
        {
        }        

        public GradientBrushDescription(
            Color color1, Color color2, BlendDescription blend
        )
            : this(color1, color2, blend, 90, 1)
        {
        }        

        public GradientBrushDescription(Color color1, Color color2)
            : this(color1, color2, null, 90, 1)
        {
        }

        public GradientBrushDescription(
            ColorBlendDescription colorBlend, float angle, float opacity
        ) {
            _blend = null;
            _colorBlend = colorBlend;

            _angle = angle;

            _opacity = opacity > 1? 1: opacity;
            _colorBlend.Opacity = _opacity;
        }

        public GradientBrushDescription(ColorBlendDescription colorBlend, float angle):
            this(colorBlend, angle, 1)
        {

        }

        public GradientBrushDescription(ColorBlendDescription colorBlend):
            this(colorBlend, 90, 1)
        {

        }

        // for Clone()
        private GradientBrushDescription() {
        }
        
        // ========================================
        // property
        // ========================================
        public BrushKind Kind {
            get { return BrushKind.Gradient; }
        }

        public bool IsDark {
            get { return ColorUtil.IsDark(_color1) && ColorUtil.IsDark(_color2); }
        }

        public Color Color1 {
            get { return _color1; }
        }

        public Color Color2 {
            get { return _color2; }
        }

        public float Angle {
            get { return _angle; }
        }

        public BlendDescription Blend {
            get { return _blend; }
        }

        public ColorBlendDescription ColorBlend {
            get { return _colorBlend; }
        }

        public float Opacity {
            get { return _opacity; }
        }

        // ========================================
        // method
        // ========================================
        public Brush CreateBrush(Rectangle bounds) {
            if (_colorBlend != null) {
                var ret = new LinearGradientBrush(bounds, _color1, _color2, _angle);
                ret.InterpolationColors = _colorBlend.CreateColorBlend();
                return ret;
            } else {
                Color opcolor1 = Math.Abs(_opacity - 1) <= float.Epsilon * _opacity?
                    _color1: Color.FromArgb((int) (_opacity * 255), _color1);
                Color opcolor2 = Math.Abs(_opacity - 1) <= float.Epsilon * _opacity?
                    _color2: Color.FromArgb((int) (_opacity * 255), _color2);
                if (_blend != null) {
                    var ret = new LinearGradientBrush(bounds, opcolor1, opcolor2, _angle);
                    ret.Blend = _blend.CreateBlend();
                    return ret;
                } else {
                    return new LinearGradientBrush(bounds, opcolor1, opcolor2, _angle);
                }
            }
        }

        public object Clone() {
            var ret = new GradientBrushDescription();
            ret._color1 = _color1;
            ret._color2 = _color2;
            ret._angle = _angle;
            ret._blend = _blend == null? null: _blend.Clone() as BlendDescription;
            ret._colorBlend = _colorBlend == null? null: _colorBlend.Clone() as ColorBlendDescription;
            ret._opacity = _opacity;
            return ret;
        }


    }

    [Serializable]
    public class BlendDescription: ICloneable {
        public float[] Factors;
        public float[] Positions;
        public Blend CreateBlend() {
            if (Factors == null || Positions == null || Factors.Length != Positions.Length) {
                throw new InvalidOperationException();
            }
            var ret = new Blend(Factors.Length);
            ret.Factors = Factors;
            ret.Positions = Positions;
            return ret;
        }

        public object Clone() {
            var ret = new BlendDescription();

            ret.Factors = new float[Factors.Length];
            Array.Copy(Factors, ret.Factors, Factors.Length);

            ret.Positions = new float[Positions.Length];
            Array.Copy(Positions, ret.Positions, Positions.Length);

            return ret;
        }
    }

    [Serializable]
    public class ColorBlendDescription: ICloneable {
        public Color[] Colors;
        public float[] Positions;
        public float Opacity;

        public ColorBlend CreateColorBlend() {
            if (Colors == null || Positions == null || Colors.Length != Positions.Length) {
                throw new InvalidOperationException();
            }
            var ret = new ColorBlend(Colors.Length);
            if (Math.Abs(Opacity - 1) <= float.Epsilon * Opacity) {
                ret.Colors = Colors;
            } else {
                var opcolors = new Color[Colors.Length];
                for (int i = 0, len = opcolors.Length; i < len; ++i) {
                    opcolors[i] = Color.FromArgb((int) (Opacity * 255), Colors[i]);
                }
                ret.Colors = opcolors;
            }
            ret.Positions = Positions;
            return ret;
        }

        public object Clone() {
            var ret = new ColorBlendDescription();

            ret.Colors = new Color[Colors.Length];
            Array.Copy(Colors, ret.Colors, Colors.Length);

            ret.Positions = new float[Positions.Length];
            Array.Copy(Positions, ret.Positions, Positions.Length);

            ret.Opacity = Opacity;

            return ret;
        }
    }
}
