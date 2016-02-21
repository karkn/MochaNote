/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using Mkamo.Common.Forms.Drawing;
using System.Drawing;
using Mkamo.Common.Core;

namespace Mkamo.Figure.Routers {
    public class OrthogonalMidpointRouter: OrthogonalRouter {
        protected override bool _NeedMoveSourcePoint {
            get { return true; }
        }

        protected override bool _NeedMoveTargetPoint {
            get { return true; }
        }


        protected override System.Drawing.Point GetSourceLocation(Mkamo.Figure.Core.IEdge edge) {
            var node = edge.Source as INode;
            if (node == null) {
                return base.GetSourceLocation(edge);
            }

            var loc = edge.SourceAnchor.Location;
            return GetSideMidpoint(loc, node);
        }

        protected override System.Drawing.Point GetTargetLocation(Mkamo.Figure.Core.IEdge edge) {
            var node = edge.Target as INode;
            if (node == null) {
                return base.GetTargetLocation(edge);
            }

            var loc = edge.TargetAnchor.Location;
            return GetSideMidpoint(loc, node);
        }

        private Point GetSideMidpoint(Point loc, INode node) {
            var outerFrame = node.OuterFrame;
            var bounds = node.Bounds;
            var dir = RectUtil.GetNearestLineDirection(bounds, loc);
            var center = RectUtil.GetCenter(bounds);
            if (EnumUtil.HasAllFlags((int) dir, (int) Mkamo.Common.DataType.Directions.Left)) {
                return outerFrame.GetNearestPoint(new Point(bounds.Left, center.Y));
            } else if (EnumUtil.HasAllFlags((int) dir, (int) Mkamo.Common.DataType.Directions.Right)) {
                return outerFrame.GetNearestPoint(new Point(bounds.Right, center.Y));
            } else if (EnumUtil.HasAllFlags((int) dir, (int) Mkamo.Common.DataType.Directions.Up)) {
                return outerFrame.GetNearestPoint(new Point(center.X, bounds.Top));
            } else {
                /// } else if (EnumUtil.HasFlag((int) dir, (int) Mkamo.Common.DataType.Directions.Left)) {
                return outerFrame.GetNearestPoint(new Point(center.X, bounds.Bottom));
            } 
        }
    }
}
