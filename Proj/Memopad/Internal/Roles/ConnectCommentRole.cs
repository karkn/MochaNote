/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using Mkamo.Editor.Requests;
using Mkamo.Editor.Core;
using Mkamo.Figure.Figures;
using System.Drawing;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Editor.Commands;
using Mkamo.Common.Command;
using Mkamo.Common.Forms.Drawing;
using Mkamo.Figure.Routers;
using Mkamo.Figure.Utils;
using Mkamo.Editor.Roles;
using Mkamo.Memopad.Internal.Commands;
using Mkamo.Memopad.Internal.Requests;

namespace Mkamo.Memopad.Internal.Roles {
    internal class ConnectCommentRole: AbstractRole {
        // ========================================
        // field
        // ========================================


        // ========================================
        // constructor
        // ========================================
        public ConnectCommentRole() {
        }

        // ========================================
        // method
        // ========================================
        public override void Installed(IEditor host) {
            if (!(host.Figure is IConnection)) {
                throw new ArgumentException("host.Figure must be IConnection");
            }
            if (!(host.Controller is IConnectionController)) {
                throw new ArgumentException("host.Controller must be IConnectionController");
            }
            base.Installed(host);
        }

        public override bool CanUnderstand(IRequest request) {
            if (request.Id != RequestIds.Connect) {
                return false;
            }

            var req = request as ConnectRequest;
            if (req == null) {
                return false;
            }

            var decider = new ConnectCommentCommand(
                _Host,
                req.ConnectingAnchor,
                req.NewConnectableEditor,
                req.NewLocation
            );
            return decider.CanExecute;
        }

        public override ICommand CreateCommand(IRequest request) {
            var req = request as ConnectRequest;
            if (req == null) {
                return null;
            }

            return new ConnectCommentCommand(
                _Host,
                req.ConnectingAnchor,
                req.NewConnectableEditor,
                req.IsDisconnect?
                    GetGridAdjustedPoint(req.NewLocation):
                    //GetConnectionPoint(req)
                    req.NewLocation
            );
        }

        public override IFigure CreateFeedback(IRequest request) {
            var req = request as ConnectRequest;
            if (req == null) {
                return null;
            }

            var feedback = _Host.Figure.CloneFigure() as IConnection;
            feedback.Accept(
                elem => {
                    elem.MakeTransparent(0.6f);
                    return false;
                }
            );

            UpdateFeedback(req, feedback);
            return feedback;
        }

        public override void UpdateFeedback(IRequest request, IFigure feedback) {
            var req = request as ConnectRequest;
            var edgeFeedback = feedback as IEdge;
            if (req == null || edgeFeedback == null) {
                return;
            }

            var anchor = req.ConnectingAnchor;
            var edge = req.ConnectingAnchor.Owner as IEdge;

            var srcClone = edge.Source == null? null: edge.Source.CloneFigure() as INode;
            var tgtClone = edge.Target == null? null: edge.Target.CloneFigure() as INode;
            if (srcClone != null) {
                edgeFeedback.Source = srcClone;
            }
            if (tgtClone != null) {
                edgeFeedback.Target = tgtClone;
            }

            if (req.IsDisconnect) {
                if (anchor.Kind == ConnectionAnchorKind.Source) {
                    edgeFeedback.SourceAnchor.Disconnect();
                    edgeFeedback.SourceAnchor.Location = GetGridAdjustedPoint(req.NewLocation);
                } else {
                    edgeFeedback.TargetAnchor.Disconnect();
                    edgeFeedback.TargetAnchor.Location = GetGridAdjustedPoint(req.NewLocation);
                }
            } else {
                edgeFeedback.ClearBendPoints();
                if (edge.BendPoints.Count() > 0) {
                    edgeFeedback.SetEdgePoints(edge.EdgePoints);
                }
                if (anchor.Kind == ConnectionAnchorKind.Source) {
                    var srcFig = req.NewConnectableEditor.Figure.CloneFigureOnly() as IConnectable;
                    edgeFeedback.SourceAnchor.Connect(srcFig);
                    edgeFeedback.SourceAnchor.Location = GetConnectionPoint(req);
                } else {
                    edgeFeedback.TargetAnchor.Disconnect();
                    edgeFeedback.TargetAnchor.Location = req.NewLocation;
                }
            }
        }

        public override void DisposeFeedback(IRequest request, IFigure feedback) {

        }


        private Point GetConnectionPoint(ConnectRequest request) {
            var edge = _Host.Figure as IEdge;
            return edge.GetConnectionPoint(
                request.ConnectingAnchor,
                request.NewConnectableEditor.Figure as INode,
                //GetGridAdjustedPoint(request.NewLocation)
                request.NewLocation
            );
        }
    }
}
