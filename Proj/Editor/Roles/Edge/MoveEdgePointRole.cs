/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Editor.Requests;
using Mkamo.Editor.Commands;
using Mkamo.Figure.Core;
using System.Drawing;
using Mkamo.Common.Forms.Drawing;
using Mkamo.Common.Command;

namespace Mkamo.Editor.Roles.Edge {
    public class MoveEdgePointRole: AbstractRole {
        // ========================================
        // field
        // ========================================
        private int _flattenWidth;

        // ========================================
        // constructor
        // ========================================
        public MoveEdgePointRole() {
            _flattenWidth = 8;
        }

        // ========================================
        // property
        // ========================================
        public int FlattenWidth {
            get { return _flattenWidth; }
            set { _flattenWidth = value < 1? 1: value; }
        }

        // ========================================
        // method
        // ========================================
        public override void Installed(IEditor host) {
            if (!(host.Figure is IEdge)) {
                throw new ArgumentException("host.Figure must be IEdge");
            }
            base.Installed(host);
        }

        public override bool CanUnderstand(IRequest request) {
            return request.Id == RequestIds.MoveEdgePoint;
        }

        public override ICommand CreateCommand(IRequest request) {
            var req = request as MoveEdgePointRequest;
            if (req == null) {
                return null;
            }

            var hostEdge = _Host.Figure as IEdge;
            var edgePointRef = req.EdgePointRef;
            var location = req.AdjustGrid ? GetGridAdjustedPoint(req.Location) : req.Location;

            if (!req.DontFlatten) {
                var flatten = GetEdgeFlattenCommand(hostEdge, edgePointRef, location);
                if (flatten != null) {
                    return flatten;
                }
            }

            return new MoveEdgePointCommand(hostEdge, edgePointRef, location);
        }

        public override IFigure CreateFeedback(IRequest request) {
            var req = request as MoveEdgePointRequest;
            if (req == null) {
                return null;
            }

            var feedback = _Host.Figure.CloneFigure() as IEdge;
            feedback.Accept(
                elem => {
                    elem.MakeTransparent(0.5f);
                    return false;
                }
            );

            UpdateFeedback(req, feedback);
            return feedback;
        }

        public override void UpdateFeedback(IRequest request, IFigure feedback) {
            var req = request as MoveEdgePointRequest;
            if (req == null) {
                return;
            }

            var edge = feedback as IEdge;
            if (edge == null) {
                throw new ArgumentException("feedback must be IEdge");
            }
            var index = req.EdgePointRef.Index;
            var loc = req.AdjustGrid ? GetGridAdjustedPoint(req.Location) : req.Location;
            edge[index] = loc;
        }

        public override void DisposeFeedback(IRequest request, IFigure feedback) {

        }

        // ------------------------------
        // protected
        // ------------------------------
        protected ICommand GetEdgeFlattenCommand(IEdge edge, EdgePointRef edgePointRef, Point location) {
            if (edgePointRef.HasPrevEdgePointReference && edgePointRef.HasNextEdgePointReference) {
                Point prev = edgePointRef.Prev.EdgePoint;
                Point next = edgePointRef.Next.EdgePoint;
                double dist = Vector2D.GetDistanceLineSegAndPoint(
                    (Vector2D) prev, (Vector2D) next, (Vector2D) location
                );
                if (dist < _flattenWidth) {
                    return new RemoveEdgePointCommand(edge, edgePointRef.Index);
                }
            } 
            if (edgePointRef.HasPrevEdgePointReference && edgePointRef.Prev.HasPrevEdgePointReference) {
                Point prev = edgePointRef.Prev.EdgePoint;
                Point prevprev = edgePointRef.Prev.Prev.EdgePoint;
                double dist = Vector2D.GetDistanceLineSegAndPoint(
                    (Vector2D) prevprev, (Vector2D) location, (Vector2D) prev
                );
                if (dist < _flattenWidth) {
                    return new RemoveEdgePointCommand(edge, edgePointRef.Index - 1);
                }
            }
            if (edgePointRef.HasNextEdgePointReference && edgePointRef.Next.HasNextEdgePointReference) {
                Point next = edgePointRef.Next.EdgePoint;
                Point nextnext = edgePointRef.Next.Next.EdgePoint;
                double dist = Vector2D.GetDistanceLineSegAndPoint(
                    (Vector2D) nextnext, (Vector2D) location, (Vector2D) next
                );
                if (dist < _flattenWidth) {
                    return new RemoveEdgePointCommand(edge, edgePointRef.Index + 1);
                }
            }

            return null;
        }

    }
}
