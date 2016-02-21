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

    public class SetStyledTextAlignmentRole: AbstractRole {

        // ========================================
        // field
        // ========================================
        private StyledTextProvider _styledTextProvider;
        private AlignmentModificationKinds _supportKinds;

        // ========================================
        // constructor
        // ========================================
        public SetStyledTextAlignmentRole(StyledTextProvider styledTextProvider, AlignmentModificationKinds supportKinds) {
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
            var req = request as SetStyledTextAlignmentRequest;
            if (req == null) {
                return false;
            }
            return (request.Id == RequestIds.SetStyledTextAlignment) && ((_supportKinds & req.Kinds) == req.Kinds);
        }

        public override ICommand CreateCommand(IRequest request) {
            var req = request as SetStyledTextAlignmentRequest;
            if (req == null) {
                return null;
            }


            return new SetStyledTextAlignmentCommand(
                _Host,
                _styledTextProvider,
                req.Kinds,
                req.HorizontalAlignment,
                req.VerticalAlignment
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
