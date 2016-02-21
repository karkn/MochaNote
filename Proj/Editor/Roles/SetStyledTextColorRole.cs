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
    using StyledText = Mkamo.StyledText.Core.StyledText;
    using StyledTextProvider = Func<Mkamo.StyledText.Core.StyledText>;

    public class SetStyledTextColorRole: AbstractRole {

        // ========================================
        // field
        // ========================================
        private StyledTextProvider _styledTextProvider;

        // ========================================
        // constructor
        // ========================================
        public SetStyledTextColorRole(StyledTextProvider styledTextProvider) {
            _styledTextProvider = styledTextProvider;
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public override bool CanUnderstand(IRequest request) {
            return request.Id == RequestIds.SetStyledTextColor;
        }

        public override ICommand CreateCommand(IRequest request) {
            var req = request as SetStyledTextColorRequest;
            if (req == null) {
                return null;
            }
            return new SetStyledTextColorCommand(_Host, _styledTextProvider, flow => req.Color);
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
