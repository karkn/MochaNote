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
    public class EqualFigure: AbstractPathBoundingNode {
        // ========================================
        // field
        // ========================================
        private bool _isNotEqual;

        /// <summary>
        /// 線の太さ
        /// </summary>
        private float _widthRatio;

        /// <summary>
        /// 上下余白部分
        /// </summary>
        private float _marginRatio;

        /// <summary>
        /// 打ち消し線
        /// </summary>
        private float _strikeMarginRatio;

        // ========================================
        // constructor
        // ========================================
        public EqualFigure(bool isNotEqual) {
            _isNotEqual = isNotEqual;
            _widthRatio = 0.25f;
            _marginRatio = 0.2f;
            _strikeMarginRatio = 0.1f;
        }

        public EqualFigure(): this(false) {
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public override void WriteExternal(IMemento memento, ExternalizeContext context) {
            base.WriteExternal(memento, context);

            memento.WriteBool("IsNotEqual", _isNotEqual);
            memento.WriteFloat("WidthRatio", _widthRatio);
            memento.WriteFloat("MarginRatio", _marginRatio);
            memento.WriteFloat("StrikeRatio", _strikeMarginRatio);
        }

        public override void ReadExternal(IMemento memento, ExternalizeContext context) {
            base.ReadExternal(memento, context);

            _isNotEqual = memento.ReadBool("IsNotEqual");
            _widthRatio = memento.ReadFloat("WidthRatio");
            _marginRatio = memento.ReadFloat("MarginRatio");
            _strikeMarginRatio = memento.ReadFloat("StrikeRatio");
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

            var figWidth = (int) (Height * _widthRatio);
            var margin = (int) (Height * _marginRatio);

            var northTop = top + margin;
            var northBottom = northTop + figWidth;
            var northTopLeft = new Point(left, northTop);
            var northTopRight = new Point(right, northTop);
            var northBottomLeft = new Point(left, northBottom);
            var northBottomRight = new Point(right, northBottom);

            var southBottom = bottom - margin;
            var southTop = southBottom - figWidth;
            var southTopLeft = new Point(left, southTop);
            var southTopRight = new Point(right, southTop);
            var southBottomLeft = new Point(left, southBottom);
            var southBottomRight = new Point(right, southBottom);

            if (_isNotEqual) {
                var sin = (height * _strikeMarginRatio) / figWidth;
                var r = Math.Asin(sin);
                var strikeMarginHeight = (int) (height * _strikeMarginRatio);
                var h = height - strikeMarginHeight;
                var strikeMarginWidth = (int) ((width - figWidth * Math.Cos(r) - (h * Math.Tan(r))) / 2);
                var cos = (int) (figWidth * Math.Cos(r));

                var strikeTopLeft = new Point(right - cos - strikeMarginWidth, top);
                var strikeTopRight = new Point(right - strikeMarginWidth, top + strikeMarginHeight);
                var strikeBottomLeft = new Point(left + strikeMarginWidth, bottom - strikeMarginHeight);
                var strikeBottomRight = new Point(left + cos + strikeMarginWidth, bottom);

                var strikeLeft = new Line(strikeBottomLeft, strikeTopLeft);
                var strikeRight = new Line(strikeBottomRight, strikeTopRight);
                var line1 = new Line(northTopLeft, northTopRight);
                var line2 = new Line(northBottomLeft, northBottomRight);
                var ptl1 = strikeLeft.GetIntersectionPoint(line1);
                var ptl2 = strikeLeft.GetIntersectionPoint(line2);
                var ptr1 = strikeRight.GetIntersectionPoint(line1);
                var ptr2 = strikeRight.GetIntersectionPoint(line2);

                var line3 = new Line(southTopLeft, southTopRight);
                var line4 = new Line(southBottomLeft, southBottomRight);
                var ptl3 = strikeLeft.GetIntersectionPoint(line3);
                var ptl4 = strikeLeft.GetIntersectionPoint(line4);
                var ptr3 = strikeRight.GetIntersectionPoint(line3);
                var ptr4 = strikeRight.GetIntersectionPoint(line4);

                path.AddPolygon(
                    new Point[] {
                        northTopLeft,
                        ptl1,
                        strikeTopLeft,
                        strikeTopRight,
                        ptr1,
                        northTopRight,
                        northBottomRight,
                        ptr2,
                        ptr3,
                        southTopRight,
                        southBottomRight,
                        ptr4,
                        strikeBottomRight,
                        strikeBottomLeft,
                        ptl4,
                        southBottomLeft,
                        southTopLeft,
                        ptl3,
                        ptl2,
                        northBottomLeft,
                        northTopLeft,
                    }
                );

            } else {
                path.AddPolygon(
                    new Point[] {
                        northTopLeft,
                        northTopRight,
                        northBottomRight,
                        northBottomLeft,
                        northTopLeft,
                    }
                );
    
                path.AddPolygon(
                    new Point[] {
                        southTopLeft,
                        southTopRight,
                        southBottomRight,
                        southBottomLeft,
                        southTopLeft,
                    }
                );
            }


            return path;
        }

    }
}
