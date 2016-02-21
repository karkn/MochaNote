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
using System.Drawing.Drawing2D;

namespace Mkamo.Figure.Figures {
    public class Paper: AbstractPathBoundingNode {
        // ========================================
        // field
        // ========================================
        /// <summary>
        /// 折り返し
        /// </summary>
        private float _foldRatio;


        // ========================================
        // constructor
        // ========================================
        public Paper() {
            _foldRatio = 0.2f;
        }


        // ========================================
        // property
        // ========================================
        protected override bool _UseDrawPath {
            get { return true; }
        }

        // ========================================
        // method
        // ========================================
        public override void WriteExternal(IMemento memento, ExternalizeContext context) {
            base.WriteExternal(memento, context);

            memento.WriteFloat("FoldRatio", _foldRatio);
        }

        public override void ReadExternal(IMemento memento, ExternalizeContext context) {
            base.ReadExternal(memento, context);

            _foldRatio = memento.ReadFloat("FoldRatio");
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

            var fold = (int) (Math.Min(width, height) * _foldRatio);
            var pts = new[] {
                new Point(left, top),
                new Point(right, top),
                new Point(right, bottom - fold),
                new Point(right - fold, bottom),
                new Point(left, bottom),
            };
            path.AddPolygon(pts);

            return path;
        }

        protected override GraphicsPathDescription CreateDrawPath(Rectangle bounds) {
            var path = new GraphicsPathDescription();

            var center = RectUtil.GetCenter(bounds);
            var left = bounds.Left;
            var right = bounds.Right;
            var top = bounds.Top;
            var bottom = bounds.Bottom;
            var height = bounds.Height;
            var width = bounds.Width;

            var fold = (int) (Math.Min(width, height) * _foldRatio);
            var pts = new[] {
                new Point(left, top),
                new Point(right, top),
                new Point(right, bottom - fold),
                new Point(right - fold, bottom),
                new Point(left, bottom),
            };
            path.AddPolygon(pts);

            var foldTop = (int) (fold * 3 / 4);
            path.AddPolygon(
                new[] {
                    new Point(right, bottom - fold),
                    new Point(right - foldTop, bottom - foldTop),
                    new Point(right - fold, bottom),
                }
            );

            return path;
        }
    }
}
