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
using Mkamo.Common.Command;

namespace Mkamo.Editor.Roles {
    public class ReorderRole: AbstractRole {
        // ========================================
        // field
        // ========================================
        private bool _isModelTarget;

        // ========================================
        // constructor
        // ========================================
        public ReorderRole(bool isModelTarget) {
            _isModelTarget = isModelTarget;
        }

        public ReorderRole(): this(false) {
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public override bool CanUnderstand(IRequest request) {
            return request.Id == RequestIds.Reorder;
        }

        public override ICommand CreateCommand(IRequest request) {
            var req = request as ReorderRequest;
            if (req == null) {
                throw new ArgumentException("request");
            }

            if (_isModelTarget) {
                return new ReorderModelCommand(_Host, req.Kind);
            } else {
                return new ReorderCommand(_Host, req.Kind);
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
