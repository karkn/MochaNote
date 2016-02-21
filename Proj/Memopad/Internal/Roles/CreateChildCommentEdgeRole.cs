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
using Mkamo.Editor.Commands;
using Mkamo.Editor.Requests;
using Mkamo.Figure.Figures;
using System.Drawing;
using Mkamo.Common.Command;
using Mkamo.Common.Forms.Drawing;
using Mkamo.Common.DataType;
using Mkamo.Figure.Utils;
using Mkamo.Figure.Routers;
using Mkamo.Editor.Roles;
using Mkamo.Memopad.Internal.Requests;
using Mkamo.Memopad.Internal.Commands;

namespace Mkamo.Memopad.Internal.Roles {
    internal class CreateChildCommentEdgeRole: AbstractRole {
        // ========================================
        // field
        // ========================================
        private IEdge _feedback;

        // ========================================
        // constructor
        // ========================================
        public CreateChildCommentEdgeRole() {

        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public override bool CanUnderstand(IRequest request) {
            if (request.Id != RequestIds.CreateEdge) {
                return false;
            }

            var req = request as CreateCommentRequest;
            if (req == null) {
                return false;
            }

            var container = _Host.Controller as IContainerController;
            return container != null && container.CanContainChild(req.ModelFactory.ModelDescriptor);
        }

        public override ICommand CreateCommand(IRequest request) {
            var req = request as CreateCommentRequest;
            if (req == null) {
                return null;
            }

            var edgePoints = default(Point[]);
            //if (req.EdgeSourceEditor != null && req.EdgeSourceEditor == req.EdgeTargetEditor) {
            //    /// 同じ図形に両端をつなごうとしている場合はループ
            //    edgePoints = EdgeUtil.GetLoopPoints(req.EdgeSourceEditor.Figure.Bounds);
            //} else if (req.EdgeSourceEditor != null && req.EdgeSourceEditor.Figure.ContainsPoint(req.EndPoint)) {
            //    /// StartもEndoも同じ図形だけどループにできない場合
            //    // todo: 実装
            //    return null;

            //} else {
                var edge = _feedback;
                edge.ClearBendPoints();

                if (req.EdgeSourceEditor == null) {
                    edge.SourceAnchor.Disconnect();
                    edge.SourceAnchor.Location = GetGridAdjustedPoint(req.StartPoint);
                } else {
                    var srcFigClone = req.EdgeSourceEditor.Figure.CloneFigure() as INode;
                    edge.SourceAnchor.Connect(srcFigClone);
                    edge.SourceAnchor.Location = edge.GetConnectionPoint(
                        edge.SourceAnchor, srcFigClone, GetGridAdjustedPoint(req.StartPoint)
                    );
                }

                if (req.EdgeTargetEditor == null) {
                    edge.TargetAnchor.Disconnect();
                    edge.TargetAnchor.Location = GetGridAdjustedPoint(req.EndPoint);
                } else {
                    var tgtFigClone = req.EdgeTargetEditor.Figure.CloneFigure() as INode;
                    edge.TargetAnchor.Disconnect();
                    edge.TargetAnchor.Location = req.EndPoint;

                    //edge.TargetAnchor.Connect(tgtFigClone);
                    //edge.TargetAnchor.Location = edge.GetConnectionPoint(
                    //    edge.TargetAnchor, tgtFigClone, GetGridAdjustedPoint(req.EndPoint)
                    //);
                }

                edgePoints = new Point[2];
                edgePoints[0] = edge.SourceAnchor.Location;
                edgePoints[1] = edge.TargetAnchor.Location;
            //}

            return new CreateCommentCommand(
                _Host,
                req.ModelFactory,
                edgePoints,
                req.EdgeSourceEditor,
                req.EdgeTargetEditor
            );
        }

        public override IFigure CreateFeedback(IRequest request) {
            if (!CanUnderstand(request)) {
                return null;
            }
            return _feedback;
        }

        public override void UpdateFeedback(IRequest request, IFigure feedback) {
            var req = request as CreateCommentRequest;
            if (req == null) {
                return;
            }

            var edge = feedback as IEdge;
            _feedback = edge;

            edge.IsVisible = true;
            //if (req.EdgeSourceEditor != null && req.EdgeSourceEditor == req.EdgeTargetEditor) {
            //    /// 同じ図形に両端をつなごうとしている場合はループする形にする
            //    var pts = EdgeUtil.GetLoopPoints(req.EdgeSourceEditor.Figure.Bounds);
            //    edge.SetEdgePoints(pts);

            //} else {
                // todo: is CentralRouterで調べるのではなく，ClearBendPoints()が必要かどうかをIRouterに聞く
                if ((edge.Router == null || edge.Router is CentralRouter) && edge.EdgePointCount > 2) {
                    edge.ClearBendPoints();
                }
                if (req.EdgeSourceEditor == null) {
                    edge.SourceAnchor.Disconnect();
                    edge.SourceAnchor.Location = GetGridAdjustedPoint(req.StartPoint);
                } else {
                    var srcFigClone = req.EdgeSourceEditor.Figure.CloneFigure() as INode;
                    edge.SourceAnchor.Connect(srcFigClone);
                    edge.SourceAnchor.Location = edge.GetConnectionPoint(
                        edge.SourceAnchor, srcFigClone, GetGridAdjustedPoint(req.StartPoint)
                    );
                }

                if (req.EdgeTargetEditor == null) {
                    edge.TargetAnchor.Disconnect();
                    edge.TargetAnchor.Location = GetGridAdjustedPoint(req.EndPoint);
                } else {
                    edge.TargetAnchor.Location = req.EndPoint;
                    //var tgtFigClone = req.EdgeTargetEditor.Figure.CloneFigure() as INode;
                    //edge.TargetAnchor.Connect(tgtFigClone);
                    //edge.TargetAnchor.Location = edge.GetConnectionPoint(
                    //    edge.TargetAnchor, tgtFigClone, GetGridAdjustedPoint(req.EndPoint)
                    //);
                }
            //}
        }

        public override void DisposeFeedback(IRequest request, IFigure feedback) {
        }

    }
}
