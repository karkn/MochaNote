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
    using Mkamo.StyledText.Core;

    public class SetStyledTextListKindRole: AbstractRole {

        // ========================================
        // field
        // ========================================
        private StyledTextProvider _styledTextProvider;

        // ========================================
        // constructor
        // ========================================
        public SetStyledTextListKindRole(StyledTextProvider styledTextProvider) {
            _styledTextProvider = styledTextProvider;
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public override bool CanUnderstand(IRequest request) {
            var req = request as SetStyledTextListKindRequest;
            if (req == null) {
                return false;
            }
            return (request.Id == RequestIds.SetStyledTextListKind);
        }

        public override ICommand CreateCommand(IRequest request) {
            var req = request as SetStyledTextListKindRequest;
            if (req == null) {
                return null;
            }


            return new SetStyledTextListKindCommand(
                _Host,
                _styledTextProvider,
                req.ListKind,
                req.On
            );
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
