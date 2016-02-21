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
using Mkamo.Common.Command;
using Mkamo.Common.Core;
using Mkamo.Common.Forms.Drawing;
using System.Drawing;

namespace Mkamo.Editor.Roles.Edge {
    public class MoveEdgeRole: AbstractRole {
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
                throw new ArgumentException("host.Figure must be IConnectionEdge");
            }
            if (!(host.Controller is IConnectionController)) {
                throw new ArgumentException("host.Controller must be IConnectionController");
            }
            base.Installed(host);
        }

        public override bool CanUnderstand(IRequest request) {
            return request.Id == RequestIds.ChangeBounds;
        }

        public override ICommand CreateCommand(IRequest request) {
            var req = request as ChangeBoundsRequest;
            if (req == null) {
                return null;
            }

            var moveDelta = req.MoveDelta +
                GetGridAdjustedDiff(_Host.Figure.Location + req.MoveDelta);
            var sizeDelta = req.SizeDelta +
                GetGridAdjustedDiff(_Host.Figure.Location + moveDelta + _Host.Figure.Size + req.SizeDelta);

            var hostConn = _Host.Figure as IEdge;
            if (hostConn.IsSourceConnected) {
                moveDelta = GetSourceConnectableMoveDelta(hostConn, req);
            } else if (hostConn.IsTargetConnected) {
                moveDelta = GetTargetConnectableMoveDelta(hostConn, req);
            }

            return new ChangeBoundsCommand(
                _Host,
                moveDelta,
                sizeDelta,
                req.ResizeDirection,
                req.MovingEditors
            );
        }

        public override IFigure CreateFeedback(IRequest request) {
            var req = request as ChangeBoundsRequest;
            if (req == null) {
                return null;
            }

            var connFeedback = _Host.Figure.CloneFigure() as IEdge;
            connFeedback.Accept(
                elem => {
                    elem.MakeTransparent(0.5f);
                    return false;
                }
            );

            UpdateFeedback(req, connFeedback);

            return connFeedback;
        }

        public override void UpdateFeedback(IRequest request, IFigure feedback) {
            var req = request as ChangeBoundsRequest;
            if (req == null) {
                return;
            }


            var connFeedback = feedback as IEdge;
            var hostConn = _Host.Figure as IEdge;
            var hostConnCtrl = _Host.Controller as IConnectionController;

            var adjustedMoveDelta = req.MoveDelta +
                GetGridAdjustedDiff(_Host.Figure.Location + req.MoveDelta);

            var connectedMoveDelta = Size.Empty;

            if (hostConn.IsSourceConnected) {
                if (req.MovingEditors.Any(editor => editor.Figure == hostConn.Source)) {
                    /// 接続点はグリッドに合わせてはいけないのでreq.MoveDeltaを使う
                    var srcMoveDelta = GetSourceConnectableMoveDelta(hostConn, req);
                    connectedMoveDelta = srcMoveDelta;
                    connFeedback.First = hostConn.First + srcMoveDelta;
                } else {
                    /// sourceのeditorはmoveの対象ではないときは動かさない
                    connFeedback.First = hostConn.First;
                }
            } else {
                /// 接続されてなければ普通に移動
                connFeedback.First = hostConn.First + adjustedMoveDelta;
            }

            if (hostConn.IsTargetConnected) {
                if (req.MovingEditors.Any(editor => editor.Figure == hostConn.Target)) {
                    /// 接続点はグリッドに合わせてはいけないのでreq.MoveDeltaを使う
                    var tgtMoveDelta = GetTargetConnectableMoveDelta(hostConn, req);
                    connectedMoveDelta = tgtMoveDelta;
                    connFeedback.Last = hostConn.Last + tgtMoveDelta;
                } else {
                    /// targetのeditorはmoveの対象ではないときは動かさない
                    connFeedback.Last = hostConn.Last;
                }
            } else {
                /// 接続されてなければ普通に移動
                connFeedback.Last = hostConn.Last + adjustedMoveDelta;
            }

            for (int i = 1, len = connFeedback.EdgePointCount; i < len - 1; ++i) {
                if (hostConn.IsSourceConnected || hostConn.IsTargetConnected) {
                    connFeedback[i] = hostConn[i] + connectedMoveDelta;
                } else {
                    connFeedback[i] = hostConn[i] + adjustedMoveDelta;
                }
            }
        }

        public override void DisposeFeedback(IRequest request, IFigure feedback) {

        }

        private Size GetSourceConnectableMoveDelta(IEdge edge, ChangeBoundsRequest req) {
            var srcLoc = edge.Source.Location;
            return (Size) GetGridAdjustedPoint(srcLoc + req.MoveDelta) - (Size) srcLoc;
        }

        private Size GetTargetConnectableMoveDelta(IEdge edge, ChangeBoundsRequest req) {
            var tgtLoc = edge.Target.Location;
            return (Size) GetGridAdjustedPoint(tgtLoc + req.MoveDelta) - (Size) tgtLoc;
        }

    }
}
