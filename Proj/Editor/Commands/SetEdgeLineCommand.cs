/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Command;
using Mkamo.Editor.Core;
using Mkamo.Figure.Core;
using Mkamo.Common.Forms.Descriptions;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Mkamo.Editor.Commands {
    public class SetEdgeLineCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private IEditor _target;
        private Color _newLineColor;
        private int _newLineWidth;
        private DashStyle _newLineDashStyle;

        private Color _oldLineColor;
        private int _oldLineWidth;
        private DashStyle _oldLineDashStyle;

        // ========================================
        // constructor
        // ========================================
        public SetEdgeLineCommand(
            IEditor target,
            Color newLineColor,
            int newLineWidth,
            DashStyle newLineDashStyle
        ) {
            _target = target;
            _newLineColor = newLineColor;
            _newLineWidth = newLineWidth;
            _newLineDashStyle = newLineDashStyle;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null && (_target.Figure is IEdge); }
        }

        public override bool CanUndo {
            get { return true; }
        }


        // ========================================
        // method
        // ========================================
        public override void Execute() {
            var edge = _target.Figure as IEdge;
            if (edge != null) {
                _oldLineColor = edge.LineColor;
                _oldLineWidth = edge.LineWidth;
                _oldLineDashStyle = edge.LineDashStyle;

                edge.LineColor = _newLineColor;
                edge.LineWidth = _newLineWidth;
                edge.LineDashStyle = _newLineDashStyle;
            }
        }

        public override void Undo() {
            var edge = _target.Figure as IEdge;
            if (edge != null) {
                edge.LineColor = _oldLineColor;
                edge.LineWidth = _oldLineWidth;
                edge.LineDashStyle = _oldLineDashStyle;
            }
        }
    }
}
