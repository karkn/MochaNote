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
    public class ArrowFigure: AbstractPathBoundingNode {
        // ========================================
        // field
        // ========================================
        private Directions _direction;
        private float _headLengthRatio;
        private float _shaftWidthRatio;

        // ========================================
        // constructor
        // ========================================
        public ArrowFigure(Directions direction) {
            _direction = direction;
            _headLengthRatio = 0.4f;
            _shaftWidthRatio = 0.5f;
        }

        public ArrowFigure(): this(Directions.Right) {
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public override void WriteExternal(IMemento memento, ExternalizeContext context) {
            base.WriteExternal(memento, context);

            memento.WriteSerializable("Direction", _direction);
            memento.WriteFloat("HeadLengthRatio", _headLengthRatio);
            memento.WriteFloat("ShaftWidthRatio", _shaftWidthRatio);
        }

        public override void ReadExternal(IMemento memento, ExternalizeContext context) {
            base.ReadExternal(memento, context);

            _direction = (Directions) memento.ReadSerializable("Direction");
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

            /// 上向き矢印で考えたときに横幅をshapeWidth，高さをshapeHeight，上下左右をtop，bottom，left，bottomとする
            var shapeWidth = _direction == Directions.Up || _direction == Directions.Down?
                width:
                height;
            var shapeHeight =  _direction == Directions.Up || _direction == Directions.Down?
                height:
                width;

            var headLen = (int) (shapeHeight * _headLengthRatio);
            var shaftLen = shapeHeight - headLen;
            var shaftWidth = (int) (shapeWidth * _shaftWidthRatio);
            var headWidth = shapeWidth;
            var widthDiff = headWidth - shaftWidth;

            Point headTop, headRightside, headLeftside, shaftTopRight, shaftTopLeft, shaftBottomRight, shaftBottomLeft;
            switch (_direction) {
                case Directions.Left: {
                    headTop = new Point(left, center.Y);
                    headRightside = new Point(left + headLen, top);
                    headLeftside = new Point(left + headLen, bottom);
                    shaftTopRight = new Point(left + headLen, top + widthDiff / 2);
                    shaftTopLeft = new Point(left + headLen, bottom - widthDiff / 2);
                    shaftBottomRight = new Point(right, top + widthDiff / 2);
                    shaftBottomLeft = new Point(right, bottom - widthDiff / 2);
                    break;
                }
                case Directions.Right: {
                    headTop = new Point(right, center.Y);
                    headRightside = new Point(right - headLen, bottom);
                    headLeftside = new Point(right - headLen, top);
                    shaftTopRight = new Point(right - headLen, bottom - widthDiff / 2);
                    shaftTopLeft = new Point(right - headLen, top + widthDiff / 2);
                    shaftBottomRight = new Point(left, bottom - widthDiff / 2);
                    shaftBottomLeft = new Point(left, top + widthDiff / 2);
                    break;
                }
                case Directions.Up: {
                    headTop = new Point(center.X, top);
                    headRightside = new Point(right, top + headLen);
                    headLeftside = new Point(left, top + headLen);
                    shaftTopRight = new Point(right - widthDiff / 2, top + headLen);
                    shaftTopLeft = new Point(left + widthDiff / 2, top + headLen);
                    shaftBottomRight = new Point(right - widthDiff / 2, bottom);
                    shaftBottomLeft = new Point(left + widthDiff / 2, bottom);
                    break;
                }
                case Directions.Down: {
                    headTop = new Point(center.X, bottom);
                    headRightside = new Point(left, bottom - headLen);
                    headLeftside = new Point(right, bottom - headLen);
                    shaftTopRight = new Point(left + widthDiff / 2, bottom - headLen);
                    shaftTopLeft = new Point(right - widthDiff / 2, bottom - headLen);
                    shaftBottomRight = new Point(left + widthDiff / 2, top);
                    shaftBottomLeft = new Point(right - widthDiff / 2, top);
                    break;
                }
                default: {
                    throw new InvalidOperationException();
                }
            }

            path.AddPolygon(
                new Point[] {
                    headTop,
                    headRightside,
                    shaftTopRight,
                    shaftBottomRight,
                    shaftBottomLeft,
                    shaftTopLeft,
                    headLeftside,
                    headTop,
                }
            );

            return path;
        }

    }
}
