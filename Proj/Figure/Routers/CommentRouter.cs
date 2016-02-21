/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Externalize;
using Mkamo.Figure.Core;
using Mkamo.Common.Forms.Drawing;
using System.Drawing;

namespace Mkamo.Figure.Routers {
    [Externalizable, Serializable]
    public class CommentRouter: IRouter {
        // ========================================
        // static field
        // ========================================
        private const int RightMargin = 8;
        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public void Route(IEdge edge) {
            if (edge.EdgePointCount != 3) {
                var pts = new Point[3];
                pts[0] = edge.First;
                pts[1] = GetMiddlePoint(edge);
                pts[2] = edge.Last;
                edge.SetEdgePoints(pts);
            }

            if (edge.IsTargetConnected) {
                var tgt = edge.Target as INode;
                var pt = tgt.GetConnectionPoint(edge.TargetConnectionOption);
                if (pt.HasValue) {
                    edge.Last = pt.Value;
                }
            }

            edge.EdgePointRefs.ElementAt(1).EdgePoint = GetMiddlePoint(edge);

            if (edge.IsSourceConnected) {
                /// SourceがつながっているのでFirstを調整

                var src = edge.Source as INode;
                var y = Math.Min(src.Top + 8, src.Bottom - 1);
                edge.First = src.OuterFrame.GetNearestPoint(new Point(src.Left, y));
            }
        }

        private Point GetMiddlePoint(IEdge edge) {
            if (edge.IsTargetConnected) {
                var targetBounds = edge.Target.Bounds;
                var pt = edge.EdgePointRefs.ElementAt(1);
                return new Point(targetBounds.Right + RightMargin, edge.Last.Y);
            } else {
                return new Point(edge.Last.X + RightMargin * 2, edge.Last.Y);
            }
        }

    }
}
