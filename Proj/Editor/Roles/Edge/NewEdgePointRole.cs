/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Figure.Core;
using Mkamo.Editor.Requests;
using Mkamo.Editor.Commands;
using System.Drawing;
using Mkamo.Common.Command;
using Mkamo.Common.Forms.Drawing;

namespace Mkamo.Editor.Roles.Edge {
    public class NewEdgePointRole: AbstractRole {
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
        public override void Installed(IEditor host) {
            if (!(host.Figure is IEdge)) {
                throw new ArgumentException("host.Figure must be IEdge");
            }
            base.Installed(host);
        }

        public override bool CanUnderstand(IRequest request) {
            return request.Id == RequestIds.NewEdgePoint;
        }

        public override ICommand CreateCommand(IRequest request) {
            var req = request as NewEdgePointRequest;
            if (req == null) {
                return null;
            }

            return new NewEdgePointCommand(
                _Host.Figure as IEdge,
                req.PrevEdgePointRef,
                GetGridAdjustedPoint(req.Location)
            );
        }

        public override IFigure CreateFeedback(IRequest request) {
            var req = request as NewEdgePointRequest;
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

            var newIndex = req.PrevEdgePointRef.Index + 1;
            var loc = GetGridAdjustedPoint(req.Location);
            feedback.InsertBendPoint(newIndex, loc);
            return feedback;
        }

        public override void UpdateFeedback(IRequest request, IFigure feedback) {
            var req = request as NewEdgePointRequest;
            if (req == null) {
                return;
            }

            var edge = feedback as IEdge;
            if (edge == null) {
                throw new ArgumentException("feedback must be IEdge");
            }
            var prev = req.PrevEdgePointRef;
            var loc = GetGridAdjustedPoint(req.Location);
            edge[prev.Index + 1] = loc;
        }

        public override void DisposeFeedback(IRequest request, IFigure feedback) {
            
        }
    }
}
