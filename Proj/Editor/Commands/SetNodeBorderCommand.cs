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
    public class SetNodeBorderCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private IEditor _target;
        private bool _newIsBorderEnabled;
        private Color _newBorderColor;
        private int _newBorderWidth;
        private DashStyle _newBorderDashStyle;

        private bool _oldIsBorderEnabled;
        private Color _oldBorderColor;
        private int _oldBorderWidth;
        private DashStyle _oldBorderDashStyle;

        // ========================================
        // constructor
        // ========================================
        public SetNodeBorderCommand(
            IEditor target,
            bool newIsBorderEnabled,
            Color newBorderColor,
            int newBorderWidth,
            DashStyle newBorderDashStyle
        ) {
            _target = target;
            _newIsBorderEnabled = newIsBorderEnabled;
            _newBorderColor = newBorderColor;
            _newBorderWidth = newBorderWidth;
            _newBorderDashStyle = newBorderDashStyle;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null && (_target.Figure is INode); }
        }

        public override bool CanUndo {
            get { return true; }
        }


        // ========================================
        // method
        // ========================================
        public override void Execute() {
            var node = _target.Figure as INode;
            if (node != null) {
                _oldIsBorderEnabled = node.IsForegroundEnabled;
                _oldBorderColor = node.Foreground;
                _oldBorderWidth = node.BorderWidth;
                _oldBorderDashStyle = node.BorderDashStyle;

                node.IsForegroundEnabled = _newIsBorderEnabled;
                node.Foreground = _newBorderColor;
                node.BorderWidth = _newBorderWidth;
                node.BorderDashStyle = _newBorderDashStyle;
            }
        }

        public override void Undo() {
            var node = _target.Figure as INode;
            if (node != null) {
                node.IsForegroundEnabled = _oldIsBorderEnabled;
                node.Foreground = _oldBorderColor;
                node.BorderWidth = _oldBorderWidth;
                node.BorderDashStyle = _oldBorderDashStyle;
            }
        }
    }
}
