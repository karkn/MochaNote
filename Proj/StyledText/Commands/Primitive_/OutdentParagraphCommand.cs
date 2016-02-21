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
#if false
namespace Mkamo.StyledText.Commands {
    public class OutdentParagraphCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private Paragraph _target;

        private ICommand _command;

        // ========================================
        // constructor
        // ========================================
        public OutdentParagraphCommand(Paragraph target) {
            _target = target;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null && _target.ListLevel > 0; }
        }

        public override bool CanUndo {
            get { return true; }
        }


        // ========================================
        // method
        // ========================================
        public override void Execute() {
            var newPadding = _target.Padding;
            newPadding = new Insets(
                newPadding.Left - StyledTextConsts.IndentPaddingWidth,
                newPadding.Top,
                newPadding.Right,
                newPadding.Bottom
            );
            _command = new SetParagraphPropertiesCommand(
                _target,
                newPadding,
                _target.LineSpace,
                _target.HorizontalAlignment,
                _target.ParagraphKind,
                _target.ListKind,
                _target.ListLevel - 1
            );
            _command.Execute();
       }

        public override void Undo() {
            if (_command != null) {
                _command.Undo();
            }
        }
    }
}
#endif
