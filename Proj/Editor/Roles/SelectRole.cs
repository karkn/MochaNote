/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Editor.Requests;
using Mkamo.Editor.Commands;
using Mkamo.Figure.Core;
using Mkamo.Common.Command;

namespace Mkamo.Editor.Roles {
    public class SelectRole: AbstractRole {
        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================
        public SelectRole() {
        }

        // ========================================
        // method
        // ========================================
        public override bool CanUnderstand(IRequest request) {
            return request.Id == RequestIds.Select;
        }

        public override ICommand CreateCommand(IRequest request) {
            var req = request as SelectRequest;
            if (req == null) {
                return null;
            }
            return new SelectCommand(_Host, req.Value, req.DeselectOthers);
        }

        public override IFigure CreateFeedback(IRequest request) {
            return null;
        }

        public override void UpdateFeedback(IRequest request, IFigure feedback) {

        }

        public override void DisposeFeedback(IRequest request, IFigure feedback) {

        }
    }
}
