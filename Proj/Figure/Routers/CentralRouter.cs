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
using Mkamo.Common.Externalize;
using Mkamo.Figure.Utils;

namespace Mkamo.Figure.Routers {
    /// <summary>
    /// 中心点を結ぶように接点を設定するルータ．
    /// </summary>
    [Externalizable, Serializable]
    public class CentralRouter: IRouter {
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
            if (edge.Source == edge.Target) {
                if (edge.Source != null) {
                    /// 同じ図形に両端がつながっている場合はループ
                    var pts = EdgeUtil.GetLoopPoints(edge.Source.Bounds);
                    edge.SetEdgePoints(pts);
                }
                return;
            }

            if (edge.EdgePointCount == 2) {
                /// first，lastしか点がない
                
                if (edge.IsSourceConnected && edge.IsTargetConnected) {
                    /// Sourceの中心点とTargetの中心点を結ぶ

                    var src = edge.Source as INode;
                    var tgt = edge.Target as INode;
                    var srcCenter = RectUtil.GetCenter(src.Bounds);
                    var tgtCenter = RectUtil.GetCenter(tgt.Bounds);
                    
                    var line = new Line(tgtCenter, srcCenter);
                    if (src.OuterFrame.IntersectsWith(line)) {
                        edge.First = src.OuterFrame.GetIntersectionPoint(line);
                    } else {
                        edge.First = srcCenter;
                    }

                    line = new Line(srcCenter, tgtCenter);
                    if (tgt.OuterFrame.IntersectsWith(line)) {
                        edge.Last = tgt.OuterFrame.GetIntersectionPoint(line);
                    } else {
                        edge.Last = tgtCenter;
                    }


                } else if (edge.IsSourceConnected) {
                    /// Sourceの中心点とLastを結ぶ
                    
                    var src = edge.Source as INode;
                    var srcCenter = RectUtil.GetCenter(src.Bounds);

                    var line = new Line(edge.Last, srcCenter);
                    if (src.OuterFrame.IntersectsWith(line)) {
                        edge.First = src.OuterFrame.GetIntersectionPoint(line);
                    } else {
                        edge.First = srcCenter;
                    }


                } else if (edge.IsTargetConnected) {
                    /// Targetの中心点とFirstを結ぶ

                    var tgt = edge.Target as INode;
                    var tgtCenter = RectUtil.GetCenter(tgt.Bounds);

                    var line = new Line(edge.First, tgtCenter);
                    if (tgt.OuterFrame.IntersectsWith(line)) {
                        edge.Last = tgt.OuterFrame.GetIntersectionPoint(line);
                    } else {
                        edge.Last = tgtCenter;
                    }

                } else {
                    /// 何もしない

                }

            } else if (edge.EdgePointCount > 2) {

                if (edge.IsSourceConnected) {
                    /// SourceがつながっているのでFirstを調整

                    var src = edge.Source as INode;
                    var srcCenter = RectUtil.GetCenter(src.Bounds);

                    var line = new Line(edge.FirstRef.Next.EdgePoint, srcCenter);
                    if (src.OuterFrame.IntersectsWith(line)) {
                        edge.First = src.OuterFrame.GetIntersectionPoint(line);
                    } else {
                        edge.First = srcCenter;
                    }
                }

                if (edge.IsTargetConnected) {
                    /// TargetがつながっているのでLastを調整

                    var tgt = edge.Target as INode;
                    var tgtCenter = RectUtil.GetCenter(tgt.Bounds);

                    var line = new Line(edge.LastRef.Prev.EdgePoint, tgtCenter);
                    if (tgt.OuterFrame.IntersectsWith(line)) {
                        edge.Last = tgt.OuterFrame.GetIntersectionPoint(line);
                    } else {
                        edge.Last = tgtCenter;
                    }
                }

            }
        }

    }
}
