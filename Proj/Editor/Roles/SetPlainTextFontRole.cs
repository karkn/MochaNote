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
using STCore = Mkamo.StyledText.Core;

namespace Mkamo.Editor.Roles {
    public class SetPlainTextFontRole: AbstractRole {

        // ========================================
        // field
        // ========================================
        private FontModificationKinds _supportKinds;

        // ========================================
        // constructor
        // ========================================
        public SetPlainTextFontRole(FontModificationKinds supportKinds) {
            _supportKinds = supportKinds;
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public override bool CanUnderstand(IRequest request) {
            var req = request as SetPlainTextFontRequest;
            if (req == null) {
                return false;
            }
            return (request.Id == RequestIds.SetPlainTextFont) && ((_supportKinds & req.Kinds) == req.Kinds);
        }

        public override ICommand CreateCommand(IRequest request) {
            var req = request as SetPlainTextFontRequest;
            if (req == null) {
                return null;
            }
            return new SetPlainTextFontCommand(_Host, req.Font);
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
