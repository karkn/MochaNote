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
    public class Parallelogram: AbstractPathBoundingNode {
        // ========================================
        // field
        // ========================================
        /// <summary>
        /// 上下余白部分
        /// </summary>
        private float _marginRatio;


        // ========================================
        // constructor
        // ========================================
        public Parallelogram() {
            _marginRatio = 0.2f;
        }


        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public override void WriteExternal(IMemento memento, ExternalizeContext context) {
            base.WriteExternal(memento, context);

            memento.WriteFloat("MarginRatio", _marginRatio);
        }

        public override void ReadExternal(IMemento memento, ExternalizeContext context) {
            base.ReadExternal(memento, context);

            _marginRatio = memento.ReadFloat("MarginRatio");
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

            var margin = (int) (width * _marginRatio);
            var pts = new [] {
                new Point(left + margin, top),
                new Point(right, top),
                new Point(right - margin, bottom),
                new Point(left, bottom),
            };
            path.AddPolygon(pts);

            return path;
        }

    }
}
