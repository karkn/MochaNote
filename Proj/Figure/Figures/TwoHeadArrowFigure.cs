/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Forms.Descriptions;
using System.Drawing;
using Mkamo.Common.Externalize;
using Mkamo.Common.DataType;
using Mkamo.Common.Forms.Drawing;

namespace Mkamo.Figure.Figures {
    public class TwoHeadArrowFigure: AbstractPathBoundingNode {
        // ========================================
        // field
        // ========================================
        private bool _isVertical;
        //private Directions _direction;
        private float _headLengthRatio;
        private float _shaftWidthRatio;

        // ========================================
        // constructor
        // ========================================
        public TwoHeadArrowFigure(bool isVertical) {
            _isVertical = isVertical;
            _headLengthRatio = 0.3f;
            _shaftWidthRatio = 0.5f;
        }

        public TwoHeadArrowFigure(): this(true) {
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public override void WriteExternal(IMemento memento, ExternalizeContext context) {
            base.WriteExternal(memento, context);

            memento.WriteBool("IsVertical", _isVertical);
            memento.WriteFloat("HeadLengthRatio", _headLengthRatio);
            memento.WriteFloat("ShaftWidthRatio", _shaftWidthRatio);
        }

        public override void ReadExternal(IMemento memento, ExternalizeContext context) {
            base.ReadExternal(memento, context);

            _isVertical = memento.ReadBool("IsVertical");
            _headLengthRatio = memento.ReadFloat("HeadLengthRatio");
            _shaftWidthRatio = memento.ReadFloat("ShaftWidthRatio");
        }

        protected override GraphicsPathDescription CreatePath(Rectangle bounds) {
            var path = new GraphicsPathDescription();

            var center = RectUtil.GetCenter(bounds);
            var left = bounds.Left;
            var right = bounds.Right;
            var top = bounds.Top;
            var bottom = bounds.Bottom;
            var height = bounds.Height;
            var width = bounds.Width;

            /// 上下矢印で考えたときに横幅をwidth，高さをlength，上下左右をtop，bottom，left，bottomとする

            var shapeWidth = _isVertical? width: height;
            var shapeHeight =  _isVertical? height: width;

            var headLen = (int) (shapeHeight * _headLengthRatio);
            var shaftLen = shapeHeight - headLen;
            var shaftWidth = (int) (shapeWidth * _shaftWidthRatio);
            var headWidth = shapeWidth;
            var widthDiff = headWidth - shaftWidth;

            Point northHeadTop, northHeadRightside, northHeadLeftside;
            Point shaftTopRight, shaftTopLeft, shaftBottomRight, shaftBottomLeft;
            Point southHeadTop, southHeadRightside, southHeadLeftside;
            if (_isVertical) {
                northHeadTop = new Point(center.X, top);
                northHeadRightside = new Point(right, top + headLen);
                northHeadLeftside = new Point(left, top + headLen);

                southHeadTop = new Point(center.X, bottom);
                southHeadRightside = new Point(right, bottom - headLen);
                southHeadLeftside = new Point(left, bottom - headLen);

                shaftTopRight = new Point(right - widthDiff / 2, top + headLen);
                shaftTopLeft = new Point(left + widthDiff / 2, top + headLen);
                shaftBottomRight = new Point(right - widthDiff / 2, bottom - headLen);
                shaftBottomLeft = new Point(left + widthDiff / 2, bottom- headLen);

            } else {
                northHeadTop = new Point(left, center.Y);
                northHeadRightside = new Point(left + headLen, top);
                northHeadLeftside = new Point(left + headLen, bottom);

                shaftTopRight = new Point(left + headLen, top + widthDiff / 2);
                shaftTopLeft = new Point(left + headLen, bottom - widthDiff / 2);
                shaftBottomRight = new Point(right- headLen, top + widthDiff / 2);
                shaftBottomLeft = new Point(right- headLen, bottom - widthDiff / 2);

                southHeadTop = new Point(right, center.Y);
                southHeadRightside = new Point(right - headLen, top);
                southHeadLeftside = new Point(right - headLen, bottom);
            }

            path.AddPolygon(
                new Point[] {
                    northHeadTop,
                    northHeadRightside,

                    shaftTopRight,
                    shaftBottomRight,

                    southHeadRightside,
                    southHeadTop,
                    southHeadLeftside,

                    shaftBottomLeft,
                    shaftTopLeft,

                    northHeadLeftside,
                    northHeadTop,
                }
            );

            return path;
        }

    }
}
