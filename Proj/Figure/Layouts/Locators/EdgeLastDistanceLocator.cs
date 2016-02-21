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
    public class EdgeLastDistanceLocator: ILocator {
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
        public EdgeLastDistanceLocator(int boundsDistance, int intersectionDistance, LocateDirectionKind direction) {
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
            var tgtPt = edge.TargetPoint;
            var nextPt = edge.LastRef.Prev.EdgePoint;
            var bounds = figure.Bounds;

            var tgtBounds = Rectangle.Empty;
            if (edge.Target == null) {
                var xDiff = Math.Abs(tgtPt.X - nextPt.X);
                var yDiff = Math.Abs(tgtPt.Y - nextPt.Y);
                if (xDiff < yDiff) {
                    if (tgtPt.Y < nextPt.Y) {
                        tgtBounds = Rectangle.FromLTRB(
                            tgtPt.X - _boundsDistance / 2,
                            tgtPt.Y - _boundsDistance,
                            tgtPt.X + _boundsDistance / 2,
                            tgtPt.Y
                        );
                    } else {
                        tgtBounds = Rectangle.FromLTRB(
                            tgtPt.X - _boundsDistance / 2,
                            tgtPt.Y,
                            tgtPt.X + _boundsDistance / 2,
                            tgtPt.Y + _boundsDistance
                        );
                    }
                } else {
                    if (tgtPt.X < nextPt.X) {
                        tgtBounds = Rectangle.FromLTRB(
                            tgtPt.X - _boundsDistance,
                            tgtPt.Y - _boundsDistance / 2,
                            tgtPt.X,
                            tgtPt.Y + _boundsDistance / 2
                        );
                    } else {
                        tgtBounds = Rectangle.FromLTRB(
                            tgtPt.X,
                            tgtPt.Y - _boundsDistance / 2,
                            tgtPt.X + _boundsDistance,
                            tgtPt.Y + _boundsDistance / 2
                        );
                    }
                }
            } else {
                tgtBounds = edge.Target.Bounds;
            }

            var inflatedBounds = tgtBounds;
            inflatedBounds.Inflate(_boundsDistance, _boundsDistance);
    
            var isPt = RectUtil.GetIntersectionPointOfRectAndExtendedLineSeg(inflatedBounds, edge.TargetPoint, nextPt);
            var dir = RectUtil.GetOuterDirection(tgtBounds, isPt);

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
