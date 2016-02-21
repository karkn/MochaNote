/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Commands;
using Mkamo.Editor.Core;
using Mkamo.Editor.Requests;
using Mkamo.Figure.Core;
using Mkamo.Common.Command;
using Mkamo.Editor.Focuses;
using STCore = Mkamo.StyledText.Core;

namespace Mkamo.Editor.Roles {
    public class FocusRole: AbstractRole {

        // ========================================
        // field
        // ========================================
        private FocusInitializer _initializer;
        private FocusCommiter _committer;

        // ========================================
        // constructor
        // ========================================
        public FocusRole(FocusInitializer initializer, FocusCommiter commiter) {
            _initializer = initializer;
            _committer = commiter;
        }

        // ========================================
        // method
        // ========================================
        public override bool CanUnderstand(IRequest request) {
            return request.Id == RequestIds.Focus;

            //if (request.Id != RequestIds.Focus) {
            //    return false;
            //}

            //var req = request as FocusRequest;
            //if (req == null) {
            //    return false;
            //}

            //return
            //    (req.Value == FocusKind.Begin && req.Location != null) ||
            //    (req.Value == FocusKind.Commit) ||
            //    (req.Value == FocusKind.Rollback);
        }

        public override ICommand CreateCommand(IRequest request) {
            var req = request as FocusRequest;
            if (req == null) {
                return null;
            }

            switch (req.Value) {
                case FocusKind.Begin: {
                    return new FocusCommand(_Host, req.Location, _initializer);
                }
                case FocusKind.Commit: {
                    return new FocusCommand(_Host, _committer);
                }
                case FocusKind.Rollback: {
                    return new FocusCommand(_Host);
                }
                default: {
                    throw new ArgumentException("req.Value");
                }
            }
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
