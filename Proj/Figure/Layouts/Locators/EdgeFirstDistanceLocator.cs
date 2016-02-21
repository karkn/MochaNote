/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using System.Drawing;
using Mkamo.Common.Forms.Drawing;
using Mkamo.Common.DataType;

namespace Mkamo.Figure.Layouts.Locators {
    [Serializable]
    public class EdgeFirstDistanceLocator: ILocator {
        // ========================================
        // field
        // ========================================
        /// <summary>
        /// 矩形からの距離
        /// </summary>
        private int _boundsDistance;

        /// <summary>
        /// 交点からの距離
        /// </summary>
        private int _intersectionDistance;

        private LocateDirectionKind _direction;

        // ========================================
        // constructor
        // ========================================
        public EdgeFirstDistanceLocator(int boundsDistance, int intersectionDistance, LocateDirectionKind direction) {
            _boundsDistance = boundsDistance;
            _intersectionDistance = intersectionDistance;
            _direction = direction;
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public void Relocate(IFigure figure, IFigure parent) {
            var edge = (IEdge) parent;
            var srcPt = edge.SourcePoint;
            var nextPt = edge.FirstRef.Next.EdgePoint;
            var bounds = figure.Bounds;

            var srcBounds = Rectangle.Empty;
            if (edge.Source == null) {
                var xDiff = Math.Abs(srcPt.X - nextPt.X);
                var yDiff = Math.Abs(srcPt.Y - nextPt.Y);
                if (xDiff < yDiff) {
                    if (srcPt.Y < nextPt.Y) {
                        srcBounds = Rectangle.FromLTRB(
                            srcPt.X - _boundsDistance / 2,
                            srcPt.Y - _boundsDistance,
                            srcPt.X + _boundsDistance / 2,
                            srcPt.Y
                        );
                    } else {
                        srcBounds = Rectangle.FromLTRB(
                            srcPt.X - _boundsDistance / 2,
                            srcPt.Y,
                            srcPt.X + _boundsDistance / 2,
                            srcPt.Y + _boundsDistance
                        );
                    }
                } else {
                    if (srcPt.X < nextPt.X) {
                        srcBounds = Rectangle.FromLTRB(
                            srcPt.X - _boundsDistance,
                            srcPt.Y - _boundsDistance / 2,
                            srcPt.X,
                            srcPt.Y + _boundsDistance / 2
                        );
                    } else {
                        srcBounds = Rectangle.FromLTRB(
                            srcPt.X,
                            srcPt.Y - _boundsDistance / 2,
                            srcPt.X + _boundsDistance,
                            srcPt.Y + _boundsDistance / 2
                        );
                    }
                }
            } else {
                srcBounds = edge.Source.Bounds;
            }

            var inflatedBounds = srcBounds;
            inflatedBounds.Inflate(_boundsDistance, _boundsDistance);

            var isPt = RectUtil.GetIntersectionPointOfRectAndExtendedLineSeg(inflatedBounds, edge.SourcePoint, nextPt);
            var dir = RectUtil.GetOuterDirection(srcBounds, isPt);

            if (DirectionUtil.ContainsUp(dir)) {
                /// up
                if (_direction == LocateDirectionKind.Left) {
                    figure.Location = new Point(isPt.X - _intersectionDistance - bounds.Width + FigureConsts.LineEndCharWidth, isPt.Y - bounds.Height);
                } else {
                    figure.Location = new Point(isPt.X + _intersectionDistance, isPt.Y - bounds.Height);
                }
    
            } else if (DirectionUtil.ContainsDown(dir)) {
                /// down
                if (_direction == LocateDirectionKind.Left) {
                    figure.Location = new Point(isPt.X - _intersectionDistance - bounds.Width + FigureConsts.LineEndCharWidth, isPt.Y);
                } else {
                    figure.Location = new Point(isPt.X + _intersectionDistance, isPt.Y);
                }
    
            } else if (DirectionUtil.ContainsLeft(dir)) {
                /// left
                if (_direction == LocateDirectionKind.Left) {
                    figure.Location = new Point(isPt.X - bounds.Width + FigureConsts.LineEndCharWidth, isPt.Y - _intersectionDistance - bounds.Height);
                } else {
                    figure.Location = new Point(isPt.X - bounds.Width + FigureConsts.LineEndCharWidth, isPt.Y + _intersectionDistance);
                }
    
            } else {
                /// right
                if (_direction == LocateDirectionKind.Left) {
                    figure.Location = new Point(isPt.X, isPt.Y - _intersectionDistance - bounds.Height);
                } else {
                    figure.Location = new Point(isPt.X, isPt.Y + _intersectionDistance);
                }
            }
        }


    }

}
