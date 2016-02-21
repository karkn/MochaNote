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

    public class SetStyledTextFontRole: AbstractRole {

        // ========================================
        // field
        // ========================================
        private StyledTextProvider _styledTextProvider;
        private FontModificationKinds _supportKinds;

        // ========================================
        // constructor
        // ========================================
        public SetStyledTextFontRole(StyledTextProvider styledTextProvider, FontModificationKinds supportKinds) {
            _styledTextProvider = styledTextProvider;
            _supportKinds = supportKinds;
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public override bool CanUnderstand(IRequest request) {
            var req = request as SetStyledTextFontRequest;
            if (req == null) {
                return false;
            }
            return (request.Id == RequestIds.SetStyledTextFont) && ((_supportKinds & req.Kinds) == req.Kinds);
        }

        public override ICommand CreateCommand(IRequest request) {
            var req = request as SetStyledTextFontRequest;
            if (req == null) {
                return null;
            }
            return new SetStyledTextFontCommand(_Host, _styledTextProvider, req.FontProvider);
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
