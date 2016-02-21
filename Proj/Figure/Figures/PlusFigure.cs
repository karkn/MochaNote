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
    public class PlusFigure: AbstractPathBoundingNode {
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
        public PlusFigure() {
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
            var widthHalf = (int) ((Math.Min(width, height) * _widthRatio) / 2);
            
            var northLeft = new Point(center.X - widthHalf, top);
            var northRight = new Point(center.X + widthHalf, top);

            var eastTop = new Point(right, center.Y - widthHalf);
            var eastBottom = new Point(right, center.Y + widthHalf);

            var southLeft = new Point(center.X - widthHalf, bottom);
            var southRight = new Point(center.X + widthHalf, bottom);

            var westTop = new Point(left, center.Y - widthHalf);
            var westBottom = new Point(left, center.Y + widthHalf);

            var pts = new[] {
                northLeft,
                northRight,
                new Point(northRight.X, westTop.Y),
                eastTop,
                eastBottom,
                new Point(northRight.X, westBottom.Y),
                southRight,
                southLeft,
                new Point(northLeft.X, westBottom.Y),
                westBottom,
                westTop,
                new Point(northLeft.X, westTop.Y),
            };

            ret.AddPolygon(pts);

            return ret;
        }
    }
}
