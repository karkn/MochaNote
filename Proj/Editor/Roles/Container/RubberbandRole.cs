/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Figures;
using Mkamo.Editor.Core;
using System.Drawing;
using Mkamo.Figure.Core;
using Mkamo.Editor.Requests;
using Mkamo.Editor.Commands;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Common.Command;

namespace Mkamo.Editor.Roles.Container {
    public class RubberbandRole: AbstractRole {
        // ========================================
        // field
        // ========================================
        private SimpleRect _feedback;

        // ========================================
        // constructor
        // ========================================
        public RubberbandRole() {
            _feedback = new SimpleRect();
            _feedback.Foreground = FigureConsts.HighlightColor;
            _feedback.Background = FigureConsts.HighlightBrush;
        }

        // ========================================
        // method
        // ========================================
        public override bool CanUnderstand(IRequest request) {
            return request.Id == RequestIds.Rubberband;
        }

        public override ICommand CreateCommand(IRequest request) {
            var rubberbandReq = request as RubberbandRequest;
            if (rubberbandReq == null) {
                return null;
            }

            var selecteds = new List<IEditor>();
            foreach (var child in _Host.Children) {
                foreach (var handle in child.AuxiliaryHandles) {
                    if (handle.Figure.IntersectsWith(rubberbandReq.Bounds)) {
                        selecteds.Add(child);
                    }
                }
                if (child.EditorHandles.Count > 0) {
                    if (child.Figure.IntersectsWith(rubberbandReq.Bounds)) {
                        selecteds.Add(child);
                    }
                }
            }

            return new SelectMultiCommand(selecteds, SelectKind.True, true);
        }


        public override IFigure CreateFeedback(IRequest request) {
            var req = request as RubberbandRequest;
            if (req == null) {
                return null;
            }
            UpdateFeedback(req, _feedback);
            return _feedback;
        }

        public override void UpdateFeedback(IRequest request, IFigure feedback) {
            var rubberReq = request as RubberbandRequest;
            if (rubberReq == null) {
                return;
            }
            feedback.Bounds = rubberReq.Bounds;
        }

        public override void DisposeFeedback(IRequest request, IFigure feedback) {

        }
    }
}
