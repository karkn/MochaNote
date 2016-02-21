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

namespace Mkamo.Editor.Roles {
    public class DropTextRole: AbstractRole {
        // ========================================
        // field
        // ========================================
        private Lazy<IFigure> _feedback;

        // ========================================
        // constructor
        // ========================================
        public DropTextRole() {
            _feedback = new Lazy<IFigure>(
                () => {
                    var ret = new SimpleRect();
                    ret.MinSize = new Size(2, 10);
                    ret.Size = new Size(2, 10);
                    ret.Foreground = Color.Red;
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
            return request.Id == RequestIds.DropText;
        }

        public override ICommand CreateCommand(IRequest request) {
            return null;
        }

        public override IFigure CreateFeedback(IRequest request) {
            return _feedback.Value;
        }

        public override void UpdateFeedback(IRequest request, IFigure feedback) {
            var req = request as DropTextRequest;
            if (req == null) {
                return;
            }

            _feedback.Value.Bounds = req.Bounds;
        }

        public override void DisposeFeedback(IRequest request, IFigure feedback) {
            
        }
    }
}
