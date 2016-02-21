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
using Mkamo.Common.DataType;
using System.Drawing;
using Mkamo.Common.Externalize;
using Mkamo.Common.Core;

namespace Mkamo.Figure.Routers {
    /// <summary>
    /// カギ線にBendPointを設定するルータ．
    /// </summary>
    [Externalizable, Serializable]
    public class OrthogonalRouter: IRouter {
        // ========================================
        // static field
        // ========================================
        private const int DefaultLength = 24;

        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================

        // ========================================
        // property
        // ========================================
        protected virtual bool _NeedMoveSourcePoint {
            get { return false; }
        }

        protected virtual bool _NeedMoveTargetPoint {
            get { return false; }
        }

        // ========================================
        // method
        // ========================================
        public void Route(IEdge edge) {
            var bendPts = default(Point[]);

            if (!edge.IsSourceConnected && !edge.IsTargetConnected) {
                bendPts = BothDisconnected(edge);
            } else if (edge.IsSourceConnected && !edge.IsTargetConnected) {
                bendPts = SourceConnected(edge);
                if (_NeedMoveSourcePoint) {
                    edge.SourcePoint = GetSourceLocation(edge);
                }
            } else if (!edge.IsSourceConnected && edge.IsTargetConnected) {
                bendPts = TargetConnected(edge);
                if (_NeedMoveTargetPoint) {
                    edge.TargetPoint = GetTargetLocation(edge);
                }
            } else if (edge.IsSourceConnected && edge.IsTargetConnected) {
                bendPts = BothConnected(edge);
                if (_NeedMoveSourcePoint) {
                    edge.SourcePoint = GetSourceLocation(edge);
                }
                if (_NeedMoveTargetPoint) {
                    edge.TargetPoint = GetTargetLocation(edge);
                }
            }

            edge.ClearBendPoints();
            foreach (var pt in bendPts) {
                // todo: constraintでptをずらせるようにする
                edge.AddBendPoint(pt);
            }
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected virtual Point GetSourceLocation(IEdge edge) {
            return edge.SourceAnchor.Location;
        }

        protected virtual Point GetTargetLocation(IEdge edge) {
            return edge.TargetAnchor.Location;
        }


        // ------------------------------
        // private
        // ------------------------------
        private Point[] BothDisconnected(IEdge edge) {
            var srcLoc = GetSourceLocation(edge); //edge.SourceAnchor.Location;
            var tgtLoc = GetTargetLocation(edge); // edge.TargetAnchor.Location;

            var bendPts = default(Point[]);

            if (Math.Abs(srcLoc.X - tgtLoc.X) < Math.Abs(srcLoc.Y - tgtLoc.Y)) {
                /// 縦長
                bendPts = new[] {
                    new Point(srcLoc.X, (srcLoc.Y + tgtLoc.Y) / 2),
                    new Point(tgtLoc.X, (srcLoc.Y + tgtLoc.Y) / 2),
                };
            } else {
                /// 横長
                bendPts = new[] {
                    new Point((srcLoc.X + tgtLoc.X) / 2, srcLoc.Y),
                    new Point((srcLoc.X + tgtLoc.X) / 2, tgtLoc.Y),
                };
            }

            return bendPts;
        }

        private Point[] SourceConnected(IEdge edge) {
            var src = edge.Source;
            var srcLoc = GetSourceLocation(edge); //edge.SourceAnchor.Location;
            var tgtLoc = GetTargetLocation(edge); // edge.TargetAnchor.Location;

            var srcDir = RectUtil.GetNearestLineDirection(src.Bounds, srcLoc);

            var bendPts = default(Point[]);

            if (EnumUtil.HasAllFlags((int) srcDir, (int) Directions.Left)) {
                var angle = 180;
                var srcTopLeft = new Point(src.Right, src.Bottom);
                var srcBottomRight = new Point(src.Left, src.Top);
                bendPts = SourceConnectedBase(srcLoc, tgtLoc, srcTopLeft, srcBottomRight, angle);

            } else if (EnumUtil.HasAllFlags((int) srcDir, (int) Directions.Right)) {
                var angle = 0;
                var srcTopLeft = src.Location;
                var srcBottomRight = new Point(src.Right, src.Bottom);
                bendPts = SourceConnectedBase(srcLoc, tgtLoc, srcTopLeft, srcBottomRight, angle);

            } else if (EnumUtil.HasAllFlags((int) srcDir, (int) Directions.Up)) {
                var angle = 90;
                var srcTopLeft = new Point(src.Left, src.Bottom);
                var srcBottomRight = new Point(src.Right, src.Top);
                bendPts = SourceConnectedBase(srcLoc, tgtLoc, srcTopLeft, srcBottomRight, angle);

            } else if (EnumUtil.HasAllFlags((int) srcDir, (int) Directions.Down)) {
                var angle = 270;
                var srcTopLeft = new Point(src.Right, src.Top);
                var srcBottomRight = new Point(src.Left, src.Bottom);
                bendPts = SourceConnectedBase(srcLoc, tgtLoc, srcTopLeft, srcBottomRight, angle);

            }

            return bendPts;
        }

        private Point[] TargetConnected(IEdge edge) {
            var tgt = edge.Target;
            var srcLoc = GetSourceLocation(edge); //edge.SourceAnchor.Location;
            var tgtLoc = GetTargetLocation(edge); // edge.TargetAnchor.Location;

            var tgtDir = RectUtil.GetNearestLineDirection(tgt.Bounds, tgtLoc);

            var bendPts = default(Point[]);

            if (EnumUtil.HasAllFlags((int) tgtDir, (int) Directions.Left)) {
                var angle = 180;
                var tgtTopLeft = new Point(tgt.Right, tgt.Bottom);
                var tgtBottomRight = new Point(tgt.Left, tgt.Top);
                bendPts = SourceConnectedBase(tgtLoc, srcLoc, tgtTopLeft, tgtBottomRight, angle);

            } else if (EnumUtil.HasAllFlags((int) tgtDir, (int) Directions.Right)) {
                var angle = 0;
                var tgtTopLeft = tgt.Location;
                var tgtBottomRight = new Point(tgt.Right, tgt.Bottom);
                bendPts = SourceConnectedBase(tgtLoc, srcLoc, tgtTopLeft, tgtBottomRight, angle);

            } else if (EnumUtil.HasAllFlags((int) tgtDir, (int) Directions.Up)) {
                var angle = 90;
                var tgtTopLeft = new Point(tgt.Left, tgt.Bottom);
                var tgtBottomRight = new Point(tgt.Right, tgt.Top);
                bendPts = SourceConnectedBase(tgtLoc, srcLoc, tgtTopLeft, tgtBottomRight, angle);

            } else if (EnumUtil.HasAllFlags((int) tgtDir, (int) Directions.Down)) {
                var angle = 270;
                var tgtTopLeft = new Point(tgt.Right, tgt.Top);
                var tgtBottomRight = new Point(tgt.Left, tgt.Bottom);
                bendPts = SourceConnectedBase(tgtLoc, srcLoc, tgtTopLeft, tgtBottomRight, angle);

            }

            return bendPts.Reverse().ToArray();
        }


        /// <summary>
        /// srcLocを原点，srcから出る線分を右方向にtransformしてbendponitを計算．
        /// </summary>
        private Point[] SourceConnectedBase(
            Point srcLoc, Point tgtLoc, Point srcTopLeft, Point srcBottomRight, float angle
        ) {
            var ret = default(Point[]);

            var delta = new Size(-srcLoc.X, -srcLoc.Y);

            /// srcLocが原点になるように平行移動してから回転
            var trTgtLoc = ForwardTransform(tgtLoc, delta, angle);

            var trSrcTopLeft = ForwardTransform(srcTopLeft, delta, angle);
            var trSrcBottomRight = ForwardTransform(srcBottomRight, delta, angle);

            /// 平行移動・回転後のSourceとTargetのBoundsの各値
            var trSrcLeft = trSrcTopLeft.X;
            var trSrcTop = trSrcTopLeft.Y;
            var trSrcRight = trSrcBottomRight.X;
            var trSrcBottom = trSrcBottomRight.Y;


            if (trTgtLoc.X >= 0) {
                if (Math.Abs(trTgtLoc.X) < Math.Abs(trTgtLoc.Y)) {
                    /// 縦長
                    ret = new[] {
                        new Point(trTgtLoc.X, 0),
                    };
                } else {
                    /// 横長
                    ret = new[] {
                        new Point((0 + trTgtLoc.X) / 2, 0),
                        new Point((0 + trTgtLoc.X) / 2, trTgtLoc.Y),
                    };
                }
            } else if (trTgtLoc.X >= trSrcLeft) {
                ret = new[] {
                    new Point(DefaultLength, 0),
                    new Point(DefaultLength, trTgtLoc.Y),
                };

            } else {
                if (trTgtLoc.Y < trSrcTop) {
                    var w = Math.Abs(trTgtLoc.X) + DefaultLength;
                    var h = Math.Abs(trTgtLoc.Y);
                    if (w < h) {
                        ret = new[] {
                            new Point(DefaultLength, 0),
                            new Point(DefaultLength, (trSrcTop + trTgtLoc.Y) / 2),
                            new Point(trTgtLoc.X, (trSrcTop + trTgtLoc.Y) / 2),
                        };
                    } else {
                        ret = new[] {
                            new Point(DefaultLength, 0),
                            new Point(DefaultLength, trTgtLoc.Y),
                        };
                    }
                } else if (trTgtLoc.Y > trSrcBottom) {
                    var w = Math.Abs(trTgtLoc.X) + DefaultLength;
                    var h = Math.Abs(trTgtLoc.Y);
                    if (w < h) {
                        ret = new[] {
                            new Point(DefaultLength, 0),
                            new Point(DefaultLength, (trSrcBottom+ trTgtLoc.Y) / 2),
                            new Point(trTgtLoc.X, (trSrcBottom + trTgtLoc.Y) / 2),
                        };
                    } else {
                        ret = new[] {
                            new Point(DefaultLength, 0),
                            new Point(DefaultLength, trTgtLoc.Y),
                        };
                    }
                } else {
                    ret = new[] {
                        new Point(DefaultLength, 0),
                        new Point(DefaultLength, trSrcBottom + DefaultLength),
                        new Point((trTgtLoc.X + trSrcLeft) / 2, trSrcBottom + DefaultLength),
                        new Point((trTgtLoc.X + trSrcLeft) / 2, trTgtLoc.Y),
                    };
                }
            }

            for (int i = 0, len = ret.Length; i < len; ++i) {
                ret[i] = ReverseTransform(ret[i], -angle, (Size) srcLoc);
            }

            return ret;
        }


        private Point[] BothConnected(IEdge edge) {
            var src = edge.Source;
            var srcLoc = GetSourceLocation(edge); //edge.SourceAnchor.Location;

            var tgt = edge.Target;
            var tgtLoc = GetTargetLocation(edge); // edge.TargetAnchor.Location;

            var srcDir = RectUtil.GetNearestLineDirection(src.Bounds, srcLoc);
            var tgtDir = RectUtil.GetNearestLineDirection(tgt.Bounds, tgtLoc);

            var bendPts = default(Point[]);

            if (EnumUtil.HasAllFlags((int) srcDir, (int) Directions.Left)) {
                var angle = 180;
                var srcTopLeft = new Point(src.Right, src.Bottom);
                var srcBottomRight = new Point(src.Left, src.Top);
                var tgtTopLeft = new Point(tgt.Right, tgt.Bottom);
                var tgtBottomRight = new Point(tgt.Left, tgt.Top);

                if (EnumUtil.HasAllFlags((int) tgtDir, (int) Directions.Right)) {
                    /// 横同士，向き合う方向
                    bendPts = FacedDirection(
                        srcLoc,
                        tgtLoc,
                        srcTopLeft,
                        srcBottomRight,
                        tgtTopLeft,
                        tgtBottomRight,
                        angle
                    );

                } else if (EnumUtil.HasAllFlags((int) tgtDir, (int) Directions.Left)) {
                    /// 横同士，同じ方向
                    bendPts = SameDirection(
                        srcLoc,
                        tgtLoc,
                        srcTopLeft,
                        srcBottomRight,
                        tgtTopLeft,
                        tgtBottomRight,
                        angle
                    );

                } else if (EnumUtil.HasAllFlags((int) tgtDir, (int) Directions.Up)) {
                    bendPts = LeftBendingDirection(
                        srcLoc,
                        tgtLoc,
                        srcTopLeft,
                        srcBottomRight,
                        tgtTopLeft,
                        tgtBottomRight,
                        angle
                    );
                } else if (EnumUtil.HasAllFlags((int) tgtDir, (int) Directions.Down)) {
                    bendPts = RightBendingDirection(
                        srcLoc,
                        tgtLoc,
                        srcTopLeft,
                        srcBottomRight,
                        tgtTopLeft,
                        tgtBottomRight,
                        angle
                    );
                }

            } else if (EnumUtil.HasAllFlags((int) srcDir, (int) Directions.Right)) {
                var angle = 0;
                var srcTopLeft = src.Location;
                var srcBottomRight = new Point(src.Right, src.Bottom);
                var tgtTopLeft = tgt.Location;
                var tgtBottomRight = new Point(tgt.Right, tgt.Bottom);

                if (EnumUtil.HasAllFlags((int) tgtDir, (int) Directions.Left)) {
                    /// 横同士，向き合う方向
                    bendPts = FacedDirection(
                        srcLoc,
                        tgtLoc,
                        srcTopLeft,
                        srcBottomRight,
                        tgtTopLeft,
                        tgtBottomRight,
                        angle
                    );
                } else if (EnumUtil.HasAllFlags((int) tgtDir, (int) Directions.Right)) {
                    /// 横同士，同じ方向
                    bendPts = SameDirection(
                        srcLoc,
                        tgtLoc,
                        srcTopLeft,
                        srcBottomRight,
                        tgtTopLeft,
                        tgtBottomRight,
                        angle
                    );

                } else if (EnumUtil.HasAllFlags((int) tgtDir, (int) Directions.Up)) {
                    bendPts = RightBendingDirection(
                        srcLoc,
                        tgtLoc,
                        srcTopLeft,
                        srcBottomRight,
                        tgtTopLeft,
                        tgtBottomRight,
                        angle
                    );
                } else if (EnumUtil.HasAllFlags((int) tgtDir, (int) Directions.Down)) {
                    bendPts = LeftBendingDirection(
                        srcLoc,
                        tgtLoc,
                        srcTopLeft,
                        srcBottomRight,
                        tgtTopLeft,
                        tgtBottomRight,
                        angle
                    );
                }

            } else if (EnumUtil.HasAllFlags((int) srcDir, (int) Directions.Up)) {
                var angle = 90;
                var srcTopLeft = new Point(src.Left, src.Bottom);
                var srcBottomRight = new Point(src.Right, src.Top);
                var tgtTopLeft = new Point(tgt.Left, tgt.Bottom);
                var tgtBottomRight = new Point(tgt.Right, tgt.Top);

                if (EnumUtil.HasAllFlags((int) tgtDir, (int) Directions.Down)) {
                    /// 縦同士，向き合う方向
                    bendPts = FacedDirection(
                        srcLoc,
                        tgtLoc,
                        srcTopLeft,
                        srcBottomRight,
                        tgtTopLeft,
                        tgtBottomRight,
                        angle
                    );
                } else if (EnumUtil.HasAllFlags((int) tgtDir, (int) Directions.Up)) {
                    /// 縦同士，同じ方向
                    bendPts = SameDirection(
                        srcLoc,
                        tgtLoc,
                        srcTopLeft,
                        srcBottomRight,
                        tgtTopLeft,
                        tgtBottomRight,
                        angle
                    );
                } else if (EnumUtil.HasAllFlags((int) tgtDir, (int) Directions.Left)) {
                    bendPts = RightBendingDirection(
                        srcLoc,
                        tgtLoc,
                        srcTopLeft,
                        srcBottomRight,
                        tgtTopLeft,
                        tgtBottomRight,
                        angle
                    );
                } else if (EnumUtil.HasAllFlags((int) tgtDir, (int) Directions.Right)) {
                    bendPts = LeftBendingDirection(
                        srcLoc,
                        tgtLoc,
                        srcTopLeft,
                        srcBottomRight,
                        tgtTopLeft,
                        tgtBottomRight,
                        angle
                    );
                }

            } else if (EnumUtil.HasAllFlags((int) srcDir, (int) Directions.Down)) {
                var angle = 270;
                var srcTopLeft = new Point(src.Right, src.Top);
                var srcBottomRight = new Point(src.Left, src.Bottom);
                var tgtTopLeft = new Point(tgt.Right, tgt.Top);
                var tgtBottomRight = new Point(tgt.Left, tgt.Bottom);

                if (EnumUtil.HasAllFlags((int) tgtDir, (int) Directions.Up)) {
                    /// 縦同士，向き合う方向
                    bendPts = FacedDirection(
                        srcLoc,
                        tgtLoc,
                        srcTopLeft,
                        srcBottomRight,
                        tgtTopLeft,
                        tgtBottomRight,
                        angle
                    );
                } else if (EnumUtil.HasAllFlags((int) tgtDir, (int) Directions.Down)) {
                    /// 縦同士，同じ方向
                    bendPts = SameDirection(
                        srcLoc,
                        tgtLoc,
                        srcTopLeft,
                        srcBottomRight,
                        tgtTopLeft,
                        tgtBottomRight,
                        angle
                    );
                } else if (EnumUtil.HasAllFlags((int) tgtDir, (int) Directions.Left)) {
                    bendPts = LeftBendingDirection(
                        srcLoc,
                        tgtLoc,
                        srcTopLeft,
                        srcBottomRight,
                        tgtTopLeft,
                        tgtBottomRight,
                        angle
                    );
                } else if (EnumUtil.HasAllFlags((int) tgtDir, (int) Directions.Right)) {
                    bendPts = RightBendingDirection(
                        srcLoc,
                        tgtLoc,
                        srcTopLeft,
                        srcBottomRight,
                        tgtTopLeft,
                        tgtBottomRight,
                        angle
                    );
                }

            }

            return bendPts;
        }

        /// <summary>
        /// srcLocが原点で右方向に線が出ているように平行移動，回転して折れ点を計算．
        /// </summary>
        private Point[] FacedDirection(
            Point srcLoc, Point tgtLoc,
            Point srcTopLeft, Point srcBottomRight, Point tgtTopLeft, Point tgtBottomRight,
            Single angle
        ) {
            var delta = new Size(-srcLoc.X, -srcLoc.Y);

            /// srcLocが原点になるように平行移動してから回転
            var trTgtLoc = ForwardTransform(tgtLoc, delta, angle);

            var trSrcTopLeft = ForwardTransform(srcTopLeft, delta, angle);
            var trSrcBottomRight = ForwardTransform(srcBottomRight, delta, angle);

            var trTgtTopLeft = ForwardTransform(tgtTopLeft, delta, angle);
            var trTgtBottomRight = ForwardTransform(tgtBottomRight, delta, angle);
            
            /// 平行移動・回転後のSourceとTargetのBoundsの各値
            var trSrcLeft = trSrcTopLeft.X;
            var trSrcTop = trSrcTopLeft.Y;
            var trSrcRight = trSrcBottomRight.X;
            var trSrcBottom = trSrcBottomRight.Y;
            var trTgtLeft = trTgtTopLeft.X;
            var trTgtTop = trTgtTopLeft.Y;
            var trTgtRight = trTgtBottomRight.X;
            var trTgtBottom = trTgtBottomRight.Y;
            
            var midLoc = PointUtil.MiddlePoint(Point.Empty, trTgtLoc);

            var ret = default(Point[]);

            if (trTgtLoc.X >= DefaultLength) {
                /// 両方connectedでなければ必ずこの条件
                ret = new [] { 
                    new Point(midLoc.X, 0),
                    new Point(midLoc.X, trTgtLoc.Y),
                };
            } else {
                var y = default(int);
                if (trTgtTop > trSrcBottom) {
                    y = (trTgtTop + trSrcBottom) / 2;
                } else if (trTgtBottom < trSrcTop) {
                    y = (trSrcTop + trTgtBottom) / 2;
                } else {
                    var top = Math.Min(trSrcTop, trTgtTop);
                    var btm = Math.Max(trSrcBottom, trTgtBottom);

                    /// 原点(元srcLoc)とtopの距離 + trTgtLocとtopの距離
                    var distanceToTop = Math.Abs(0 - top) + Math.Abs(trTgtLoc.Y - top);
                    /// 原点(元srcLoc)とbtmの距離 + trTgtLocとbtmの距離
                    var distanceToBtm = Math.Abs(btm - 0) + Math.Abs(btm - trTgtLoc.Y);

                    if (distanceToTop < distanceToBtm) {
                        y = top - DefaultLength;
                    } else {
                        y = btm + DefaultLength;
                    }
                }
                ret = new[] {
                    new Point(DefaultLength, 0),
                    new Point(DefaultLength, y),
                    new Point(trTgtLoc.X - DefaultLength, y),
                    new Point(trTgtLoc.X - DefaultLength, trTgtLoc.Y),
                };
            }

            for (int i = 0, len = ret.Length; i < len; ++i) {
                ret[i] = ReverseTransform(ret[i], -angle, (Size) srcLoc);
            }
            return ret;
        }


        private Point[] SameDirection(
            Point srcLoc, Point tgtLoc,
            Point srcTopLeft, Point srcBottomRight, Point tgtTopLeft, Point tgtBottomRight,
            Single angle
        ) {
            var delta = new Size(-srcLoc.X, -srcLoc.Y);

            /// srcLocを原点にして回転
            var trTgtLoc = ForwardTransform(tgtLoc, delta, angle);

            var trSrcTopLeft = ForwardTransform(srcTopLeft, delta, angle);
            var trSrcBottomRight = ForwardTransform(srcBottomRight, delta, angle);

            var trTgtTopLeft = ForwardTransform(tgtTopLeft, delta, angle);
            var trTgtBottomRight = ForwardTransform(tgtBottomRight, delta, angle);
            
            var trSrcLeft = trSrcTopLeft.X;
            var trSrcTop = trSrcTopLeft.Y;
            var trSrcRight = trSrcBottomRight.X;
            var trSrcBottom = trSrcBottomRight.Y;
            var trTgtLeft = trTgtTopLeft.X;
            var trTgtTop = trTgtTopLeft.Y;
            var trTgtRight = trTgtBottomRight.X;
            var trTgtBottom = trTgtBottomRight.Y;

            var ret = default(Point[]);

            if (trTgtLoc.X < trSrcLeft) {
                /// 横軸がtgtが左
                if (trTgtLoc.Y < trSrcTop) {
                    ret = new[] {
                        new Point(DefaultLength, 0),
                        new Point(DefaultLength, trTgtLoc.Y),
                    };
                } else if (trTgtLoc.Y > trSrcBottom) {
                    ret = new[] {
                        new Point(DefaultLength, 0),
                        new Point(DefaultLength, trTgtLoc.Y),
                    };
                } else {
                    ret = new[] {
                        new Point(DefaultLength, 0),
                        new Point(DefaultLength, trSrcTop - DefaultLength),
                        new Point((trTgtLoc.X + trSrcLeft) / 2, trSrcTop - DefaultLength),
                        new Point((trTgtLoc.X + trSrcLeft) / 2, trTgtLoc.Y),
                    };
                }

            } else if (trTgtLeft < trSrcRight) {
                /// 横軸が重なる
                if (trTgtRight > 0) {
                    ret = new[] {
                        new Point(trTgtRight + DefaultLength, 0),
                        new Point(trTgtRight + DefaultLength, trTgtLoc.Y),
                    };
                } else {
                    ret = new[] {
                        new Point(DefaultLength, 0),
                        new Point(DefaultLength, trTgtLoc.Y),
                    };
                }

            } else {
                /// 横軸がtgtが右
                if (trTgtTop > 0) {
                    ret = new[] {
                        new Point(trTgtRight + DefaultLength, 0),
                        new Point(trTgtRight + DefaultLength, trTgtLoc.Y),
                    };
                } else if (trTgtBottom < 0) {
                    ret = new[] {
                        new Point(trTgtRight + DefaultLength, 0),
                        new Point(trTgtRight + DefaultLength, trTgtLoc.Y),
                    };
                } else {
                    ret = new[] {
                        new Point(DefaultLength, 0),
                        new Point(DefaultLength, trTgtBottom + DefaultLength),
                        new Point(trTgtRight + DefaultLength, trTgtBottom + DefaultLength),
                        new Point(trTgtRight + DefaultLength, trTgtLoc.Y),
                    };
                }
            }

            for (int i = 0, len = ret.Length; i < len; ++i) {
                ret[i] = ReverseTransform(ret[i], -angle, (Size) srcLoc);
            }
            return ret;
        }        

        private Point[] RightBendingDirection(
            Point srcLoc, Point tgtLoc,
            Point srcTopLeft, Point srcBottomRight, Point tgtTopLeft, Point tgtBottomRight,
            Single angle
        ) {
            var delta = new Size(-srcLoc.X, -srcLoc.Y);

            /// srcLocを原点にして回転
            var trTgtLoc = ForwardTransform(tgtLoc, delta, angle);

            var trSrcTopLeft = ForwardTransform(srcTopLeft, delta, angle);
            var trSrcBottomRight = ForwardTransform(srcBottomRight, delta, angle);

            var trTgtTopLeft = ForwardTransform(tgtTopLeft, delta, angle);
            var trTgtBottomRight = ForwardTransform(tgtBottomRight, delta, angle);
            
            var trSrcLeft = trSrcTopLeft.X;
            var trSrcTop = trSrcTopLeft.Y;
            var trSrcRight = trSrcBottomRight.X;
            var trSrcBottom = trSrcBottomRight.Y;
            var trTgtLeft = trTgtTopLeft.X;
            var trTgtTop = trTgtTopLeft.Y;
            var trTgtRight = trTgtBottomRight.X;
            var trTgtBottom = trTgtBottomRight.Y;
            
            var ret = default(Point[]);

            if (trTgtLoc.X < 0) {
                if (trTgtLoc.Y > trSrcBottom) {
                    ret = new[] {
                        new Point(DefaultLength, 0),
                        new Point(DefaultLength, (trSrcBottom + trTgtLoc.Y) / 2),
                        new Point(trTgtLoc.X, (trSrcBottom + trTgtLoc.Y) / 2),
                    };
                } else {
                    if (trTgtLoc.Y < trSrcTop) {
                        ret = new[] {
                            new Point(DefaultLength, 0),
                            new Point(DefaultLength, trTgtLoc.Y - DefaultLength),
                            new Point(trTgtLoc.X, trTgtLoc.Y - DefaultLength),
                        };
                    } else {
                        ret = new[] {
                            new Point(DefaultLength, 0),
                            new Point(DefaultLength, trSrcTop - DefaultLength),
                            new Point(trTgtLoc.X, trSrcTop - DefaultLength),
                        };
                    }
                }
            } else {
                if (trTgtLoc.Y > 0) {
                    ret = new[] {
                        new Point(trTgtLoc.X, 0),
                    };
                } else {
                    ret = new[] {
                        new Point((0 + trTgtLeft) / 2, 0),
                        new Point((0 + trTgtLeft) / 2, trTgtTop - DefaultLength),
                        new Point(trTgtLoc.X, trTgtTop - DefaultLength),
                    };
                }
            }

            for (int i = 0, len = ret.Length; i < len; ++i) {
                ret[i] = ReverseTransform(ret[i], -angle, (Size) srcLoc);
            }
            return ret;
        }

        private Point[] LeftBendingDirection(
            Point srcLoc, Point tgtLoc,
            Point srcTopLeft, Point srcBottomRight, Point tgtTopLeft, Point tgtBottomRight,
            Single angle
        ) {
            var delta = new Size(-srcLoc.X, -srcLoc.Y);

            /// srcLocを原点にして回転
            var trTgtLoc = ForwardTransform(tgtLoc, delta, angle);

            var trSrcTopLeft = ForwardTransform(srcTopLeft, delta, angle);
            var trSrcBottomRight = ForwardTransform(srcBottomRight, delta, angle);

            var trTgtTopLeft = ForwardTransform(tgtTopLeft, delta, angle);
            var trTgtBottomRight = ForwardTransform(tgtBottomRight, delta, angle);
            
            var trSrcLeft = trSrcTopLeft.X;
            var trSrcTop = trSrcTopLeft.Y;
            var trSrcRight = trSrcBottomRight.X;
            var trSrcBottom = trSrcBottomRight.Y;
            var trTgtLeft = trTgtTopLeft.X;
            var trTgtTop = trTgtTopLeft.Y;
            var trTgtRight = trTgtBottomRight.X;
            var trTgtBottom = trTgtBottomRight.Y;
            
            var ret = default(Point[]);

            if (trTgtLoc.X < 0) {
                if (trTgtBottom < trSrcTop) {
                    ret = new[] {
                        new Point(DefaultLength, 0),
                        new Point(DefaultLength, (trTgtBottom + trSrcTop) / 2),
                        new Point(trTgtLoc.X, (trTgtBottom + trSrcTop) / 2),
                    };
                } else {
                    if (trTgtBottom > trSrcBottom) {
                        ret = new[] {
                            new Point(DefaultLength, 0),
                            new Point(DefaultLength, trTgtBottom + DefaultLength),
                            new Point(trTgtLoc.X, trTgtBottom + DefaultLength),
                        };
                    } else {
                        ret = new[] {
                            new Point(DefaultLength, 0),
                            new Point(DefaultLength, trSrcBottom + DefaultLength),
                            new Point(trTgtLoc.X, trSrcBottom + DefaultLength),
                        };
                    }
                }
            } else {
                if (trTgtLoc.Y < 0) {
                    ret = new[] {
                        new Point(trTgtLoc.X, 0),
                    };
                } else {
                    ret = new[] {
                        new Point((0 + trTgtLeft) / 2, 0),
                        new Point((0 + trTgtLeft) / 2, trTgtBottom + DefaultLength),
                        new Point(trTgtLoc.X, trTgtBottom + DefaultLength),
                    };
                }
            }

            for (int i = 0, len = ret.Length; i < len; ++i) {
                ret[i] = ReverseTransform(ret[i], -angle, (Size) srcLoc);
            }
            return ret;
        }

        
        private Point ForwardTransform(Point pt, Size delta, Single angle) {
            return PointUtil.RotateAtOrigin(PointUtil.Translate(pt, delta), angle);
        }

        private Point ReverseTransform(Point pt, Single angle, Size delta) {
            return PointUtil.Translate(PointUtil.RotateAtOrigin(pt, angle), delta);
        }
    }
}
