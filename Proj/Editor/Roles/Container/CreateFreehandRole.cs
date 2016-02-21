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
using Mkamo.Figure.Core;
using Mkamo.Figure.Figures;
using System.Drawing;
using Mkamo.Editor.Commands;

namespace Mkamo.Editor.Roles.Container {
    public class CreateFreehandRole: AbstractRole {
        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        public override bool CanUnderstand(IRequest request) {
            var req = request as FreehandRequest;
            if (req == null) {
                return false;
            }

            return req.Points != null;// && req.Points.Count > 1;
        }

        public override Mkamo.Common.Command.ICommand CreateCommand(Mkamo.Editor.Core.IRequest request) {
            var req = request as FreehandRequest;
            if (req == null) {
                return null;
            }

            return new CreateFreehandCommand(_Host, req.ModelFactory, req.Points, req.Width, req.Color);
        }

        public override IFigure CreateFeedback(IRequest request) {
            var req = request as FreehandRequest;
            if (req == null) {
                return null;
            }

            var ret = new FreehandFigure();
            ret.BorderWidth = req.Width;
            ret.Foreground = req.Color;
            ret.SetPoints(req.Points);

            return ret;
        }

        public override void UpdateFeedback(IRequest request, IFigure feedback) {
            var req = request as FreehandRequest;
            if (req == null) {
                return;
            }

            var fig = feedback as FreehandFigure;
            if (fig == null) {
                return;
            }

            fig.SetPoints(req.Points);
        }

        public override void DisposeFeedback(IRequest request, IFigure feedback) {
            
        }

    }
}
