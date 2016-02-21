/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Command;
using Mkamo.StyledText.Core;
using Mkamo.Common.Forms.Drawing;

namespace Mkamo.StyledText.Commands {
    public class IndentParagraphCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private Paragraph _target;
        private int _delta;

        private ICommand _command;

        // ========================================
        // constructor
        // ========================================
        public IndentParagraphCommand(Paragraph target): this(target, 1) {
        }

        public IndentParagraphCommand(Paragraph target, int delta) {
            _target = target;
            _delta = delta;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get {
                return
                    _target != null &&
                    _target.ListLevel + _delta >= 0 &&
                    _target.ListLevel + _delta < 10;
            }
        }

        public override bool CanUndo {
            get { return true; }
        }


        // ========================================
        // method
        // ========================================
        public override void Execute() {
            var newPadding = GetNewPadding(_target.Padding, _delta);
            //newPadding = new Insets(
            //    newPadding.Left + StyledTextConsts.IndentPaddingWidth,
            //    newPadding.Top,
            //    newPadding.Right,
            //    newPadding.Bottom
            //);
            _command = new SetParagraphPropertiesCommand(
                _target,
                newPadding,
                _target.LineSpace,
                _target.HorizontalAlignment,
                _target.ParagraphKind,
                _target.ListKind,
                _target.ListLevel + _delta,
                _target.ListState
            );
            _command.Execute();
        }

        public override void Undo() {
            if (_command != null) {
                _command.Undo();
            }
        }

        private static Insets GetNewPadding(Insets oldPadding, int delta) {
            return new Insets(
                oldPadding.Left + (StyledTextConsts.IndentPaddingWidth * delta),
                oldPadding.Top,
                oldPadding.Right,
                oldPadding.Bottom
            );
        }
    }
}
