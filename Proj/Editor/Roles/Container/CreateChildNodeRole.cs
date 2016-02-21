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
using Mkamo.Figure.Figures;
using System.Drawing;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Common.Command;
using Mkamo.Common.Forms.Drawing;

namespace Mkamo.Editor.Roles.Container {
    public class CreateChildNodeRole: AbstractRole {
        // ========================================
        // field
        // ========================================
        private IFigure _feedback;

        // ========================================
        // constructor
        // ========================================
        public CreateChildNodeRole() {
        }

        // ========================================
        // property
        // ========================================
        public IFigure Feedback {
            get {
                if (_feedback == null) {
                    var fig = new SimpleRect();
                    fig.Foreground = SystemColors.ButtonShadow;
                    fig.Background = new SolidBrushDescription(Color.FromArgb(16, SystemColors.ButtonFace));
                    _feedback = fig;
                }
                return _feedback;
            }
            set { _feedback = value; }
        }

        // ========================================
        // method
        // ========================================
        public override bool CanUnderstand(IRequest request) {
            if (request.Id != RequestIds.CreateNode) {
                return false;
            }

            var req = request as CreateNodeRequest;
            if (req == null) {
                return false;
            }

            var container = _Host.Controller as IContainerController;
            return container != null && container.CanContainChild(req.ModelFactory.ModelDescriptor);
        }

        public override ICommand CreateCommand(IRequest request) {
            var createReq = request as CreateNodeRequest;
            if (createReq == null) {
                return null;
            }

            return new CreateNodeCommand(
                _Host,
                createReq.ModelFactory,
                GetGridAdjustedRect(createReq.Bounds, createReq.AdjustSizeToGrid)
            );
        }

        public override IFigure CreateFeedback(IRequest request) {
            if (!CanUnderstand(request)) {
                return null;
            }

            return Feedback;
        }

        public override void UpdateFeedback(IRequest request, IFigure feedback) {
            var createReq = request as CreateNodeRequest;
            if (createReq == null) {
                return;
            }

            feedback.Bounds = GetGridAdjustedRect(createReq.Bounds, createReq.AdjustSizeToGrid);
        }

        public override void DisposeFeedback(IRequest request, IFigure feedback) {
            
        }
    }
}
