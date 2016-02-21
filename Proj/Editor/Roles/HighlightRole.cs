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
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Common.Command;
using Mkamo.Common.Core;

namespace Mkamo.Editor.Roles {
    public class HighlightRole: AbstractRole {
        // ========================================
        // field
        // ========================================
        private Lazy<IFigure> _feedback;

        // ========================================
        // constructor
        // ========================================
        public HighlightRole(): base() {
            _feedback = new Lazy<IFigure>(
                () => {
                    var fig = new SimpleRect();
                    fig.Foreground = FigureConsts.HighlightColor;
                    fig.Background = FigureConsts.HighlightBrush;
                    return fig;
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
            return request.Id == RequestIds.Highlight;
        }

        public override ICommand CreateCommand(IRequest request) {
            return null;
        }

        public override IFigure CreateFeedback(IRequest request) {
            if (!CanUnderstand(request)) {
                return null;
            }
            UpdateFeedback(request, _feedback.Value);
            return _feedback.Value;
        }

        public override void UpdateFeedback(IRequest request, IFigure feedback) {
            if (!CanUnderstand(request)) {
                return;
            }
            feedback.Bounds = _Host.Figure.Bounds;
        }

        public override void DisposeFeedback(IRequest request, IFigure feedback) {

        }
    }
}
