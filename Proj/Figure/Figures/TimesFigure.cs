/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Externalize;
using System.Drawing;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Common.Forms.Drawing;

namespace Mkamo.Figure.Figures {
    public class TimesFigure: AbstractPathBoundingNode {
        // ========================================
        // field
        // ========================================
        /// <summary>
        /// 線の太さ
        /// </summary>
        private float _widthRatio;

        // ========================================
        // constructor
        // ========================================
        public TimesFigure() {
            _widthRatio = 0.25f;
        }

        // ========================================
        // method
        // ========================================
        public override void WriteExternal(IMemento memento, ExternalizeContext context) {
            base.WriteExternal(memento, context);

            memento.WriteFloat("WidthRatio", _widthRatio);
        }

        public override void ReadExternal(IMemento memento, ExternalizeContext context) {
            base.ReadExternal(memento, context);

            _widthRatio = memento.ReadFloat("WidthRatio");
        }

        protected override GraphicsPathDescription CreatePath(Rectangle bounds) {
            var ret = new GraphicsPathDescription();

            var center = RectUtil.GetCenter(bounds);
            var left = bounds.Left;
            var right = bounds.Right;
            var top = bounds.Top;
            var bottom = bounds.Bottom;
            var height = bounds.Height;
            var width = bounds.Width;

            var figWidth = (int) (Math.Min(width, height) * _widthRatio);
            var r = Math.Atan2(height, width);
            var sin = (int) (figWidth * Math.Sin(r));
            var cos = (int) (figWidth * Math.Cos(r));

            /// 左上
            var ptlt1 = new Point(left, top + cos);
            var ptlt2 = new Point(left + sin, top);

            /// 右上
            var ptrt1 = new Point(right - sin, top);
            var ptrt2 = new Point(right, top + cos);

            /// 左下
            var ptlb1 = new Point(left, bottom - cos);
            var ptlb2 = new Point(left + sin, bottom);

            /// 右下
            var ptrb1 = new Point(right - sin, bottom);
            var ptrb2 = new Point(right, bottom - cos);

            /// 左上から右下の上側
            var line1t = new Line(ptlt2, ptrb2);
            /// 左上から右下の下側
            var line1b = new Line(ptlt1, ptrb1);

            /// 右上から左下の上側
            var line2t = new Line(ptrt1, ptlb1);
            /// 右上から左下の下側
            var line2b = new Line(ptrt2, ptlb2);

            /// 上交点
            var ptt = line1t.GetIntersectionPoint(line2t);
            /// 下交点
            var ptb = line1b.GetIntersectionPoint(line2b);
            /// 左交点
            var ptl = line1b.GetIntersectionPoint(line2t);
            /// 右交点
            var ptr = line1t.GetIntersectionPoint(line2b);

            var pts = new[] {
                ptlt1,
                ptlt2,
                ptt,
                ptrt1,
                ptrt2,
                ptr,
                ptrb2,
                ptrb1,
                ptb,
                ptlb2,
                ptlb1,
                ptl,
            };

            ret.AddPolygon(pts);

            return ret;
        }
    }
}
