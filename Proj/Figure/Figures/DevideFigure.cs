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
    public class DevideFigure: AbstractPathBoundingNode {
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
        public DevideFigure() {
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

            var leftTop = new Point(left, center.Y - widthHalf);
            var rightTop = new Point(right, center.Y - widthHalf);
            var leftBottom = new Point(left, center.Y + widthHalf);
            var rightBottom = new Point(right, center.Y + widthHalf);

            var pts = new[] {
                leftTop,
                rightTop,
                rightBottom,
                leftBottom,
            };

            ret.AddPolygon(pts);

            var northRect = new Rectangle(center.X - widthHalf, top, figWidth, figWidth);
            ret.AddEllipse(northRect);

            var southRect = new Rectangle(center.X - widthHalf, bottom - figWidth, figWidth, figWidth);
            ret.AddEllipse(southRect);

            return ret;
        }
    }
}
