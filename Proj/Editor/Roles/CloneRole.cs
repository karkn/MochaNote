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
using System.Drawing;
using Mkamo.Editor.Commands;
using Mkamo.Common.Command;

namespace Mkamo.Editor.Roles {
    public class CloneRole: AbstractRole {
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
        public override bool CanUnderstand(IRequest request) {
            var req = request as CloneRequest;
            if (req == null) {
                return false;
            }
            var copyReq = new CopyRequest(req.TargetEditors);
            return req.TargetEditors.Any(editor => editor.CanUnderstand(copyReq));
        }

        public override ICommand CreateCommand(IRequest request) {
            var req = request as CloneRequest;
            if (req == null) {
                return null;
            }

            if (req.TargetEditors.Any()) {
                /// _HostはModel==MemoなEditorなのでGetGridAdjustedMoveDelta(req.MoveDelta)してはいけない。
                var cloning = req.TargetEditors.First();
                var moveDelta = req.MoveDelta + GetGridAdjustedDiff(cloning.Figure.Location + req.MoveDelta);
                return new CloneCommand(_Host, req.TargetEditors, moveDelta);
            } else {
                return null;
            }

        }

        public override IFigure CreateFeedback(IRequest request) {
            var req = request as CloneRequest;
            if (req == null) {
                return null;
            }

            var feedback = _Host.Figure.CloneFigure();
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
            var req = request as ChangeBoundsRequest;
            if (req == null) {
                return;
            }

            feedback.Bounds = GetNewBounds(_Host.Figure.Bounds, req);
        }

        public override void DisposeFeedback(IRequest request, IFigure feedback) {

        }

        // ------------------------------
        // protected
        // ------------------------------
        protected virtual Rectangle GetNewBounds(Rectangle oldBounds, ChangeBoundsRequest request) {
            return new Rectangle(
                GetGridAdjustedPoint(oldBounds.Location + request.MoveDelta),
                oldBounds.Size
            );
        }
    }
}
