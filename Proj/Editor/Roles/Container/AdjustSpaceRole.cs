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
using Mkamo.Figure.Figures;
using System.Drawing;
using Mkamo.Common.Core;
using Mkamo.Common.Command;
using Mkamo.Editor.Requests;
using Mkamo.Editor.Internal.Figures;

namespace Mkamo.Editor.Roles.Container {
    public class AdjustSpaceRole: AbstractRole {
        // ========================================
        // field
        // ========================================
        private Lazy<AdjustSpaceFeedbackFigure> _feedback;

        // ========================================
        // constructor
        // ========================================
        public AdjustSpaceRole() {
            _feedback = new Lazy<AdjustSpaceFeedbackFigure>(
                () => {
                    var ret = new AdjustSpaceFeedbackFigure();
                    ret.Foreground = FigureConsts.HighlightColor;
                    return ret;
                }
            );
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================

        public override bool CanUnderstand(IRequest request) {
            return request.Id == RequestIds.AdjustSpace;
        }

        public override ICommand CreateCommand(IRequest request) {
            return null;
        }

        public override IFigure CreateFeedback(IRequest request) {
            return _feedback.Value;
        }

        public override void UpdateFeedback(IRequest request, IFigure feedback) {
            var req = request as AdjustSpaceRequest;
            if (req == null) {
                return;
            }

            _feedback.Value.Bounds = req.Bounds;
            _feedback.Value.Horizontal = req.Horizontal;
        }

        public override void DisposeFeedback(IRequest request, IFigure feedback) {
            
        }
    }
}
