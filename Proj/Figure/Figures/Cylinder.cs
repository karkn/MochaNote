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
    public class Cylinder: AbstractPathBoundingNode {
        // ========================================
        // field
        // ========================================
        private float _ovalRatio;


        // ========================================
        // constructor
        // ========================================
        public Cylinder() {
            _ovalRatio = 0.2f;
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

            memento.WriteFloat("OvalRatio", _ovalRatio);
        }

        public override void ReadExternal(IMemento memento, ExternalizeContext context) {
            base.ReadExternal(memento, context);

            _ovalRatio = memento.ReadFloat("OvalRatio");
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

            var ovalHeight = (int) (height * _ovalRatio);
            var ovalHeightHalf = ovalHeight / 2;
            path.AddArc(new Rectangle(left, top, width, ovalHeight), 180, 180);
            path.AddLine(new Point(right, top + ovalHeightHalf), new Point(right, bottom - ovalHeightHalf));
            path.AddArc(new Rectangle(left, bottom - ovalHeight, width, ovalHeight), 0, 180);
            path.AddLine(new Point(left, bottom - ovalHeightHalf), new Point(left, top + ovalHeightHalf));

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

            var ovalHeight = (int) (height * _ovalRatio);
            var ovalHeightHalf = ovalHeight / 2;
            path.AddArc(new Rectangle(left, top, width, ovalHeight), 180, 180);
            path.AddLine(new Point(right, top + ovalHeightHalf), new Point(right, bottom - ovalHeightHalf));
            path.AddArc(new Rectangle(left, bottom - ovalHeight, width, ovalHeight), 0, 180);
            path.AddLine(new Point(left, bottom - ovalHeightHalf), new Point(left, top + ovalHeightHalf));

            path.AddArc(new Rectangle(left, top, width, ovalHeight), 180, 180);
            path.AddArc(new Rectangle(left, top, width, ovalHeight), 0, 180);

            path.FillMode = System.Drawing.Drawing2D.FillMode.Winding;
            return path;
        }
    }
}
