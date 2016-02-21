/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Requests;
using Mkamo.Editor.Core;
using Mkamo.Common.Command;
using Mkamo.Figure.Core;
using Mkamo.Editor.Commands;
using Mkamo.Figure.Figures;
using System.Drawing;

namespace Mkamo.Editor.Roles {
    public class CombineRole: AbstractRole {
        // ========================================
        // field
        // ========================================
        private EditorCombinationJudge _judge;
        private EditorCombinator _combinator;

        // ========================================
        // constructor
        // ========================================
        public CombineRole(EditorCombinationJudge judge, EditorCombinator combinator) {
            _judge = judge;
            _combinator = combinator;
        }

        // ========================================
        // method
        // ========================================
        public override bool CanUnderstand(Mkamo.Editor.Core.IRequest request) {
            var req = request as CombineRequest;
            if (req == null) {
                return false;
            }

            return _judge(_Host, req.Combineds);
        }

        public override ICommand CreateCommand(IRequest request) {
            var req = request as CombineRequest;
            if (req == null) {
                return null;
            }

            return new CombineCommand(_Host, req.Combineds, _combinator);
        }

        public override IFigure CreateFeedback(IRequest request) {
            var feedback = new SimpleRect();
            feedback.Foreground = Color.Red;
            feedback.IsForegroundEnabled = true;
            feedback.IsBackgroundEnabled = false;
            feedback.BorderWidth = 2;
            feedback.Bounds = _Host.Figure.Bounds;
            return feedback;
        }

        public override void UpdateFeedback(IRequest request, IFigure feedback) {
            feedback.Bounds = _Host.Figure.Bounds;
        }

        public override void DisposeFeedback(IRequest request, IFigure feedback) {

        }
    }
}
