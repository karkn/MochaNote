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
    public class SetListKindOfParagraphCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private Paragraph _target;
        private ListKind _listKind;

        private ICommand _command;

        // ========================================
        // constructor
        // ========================================
        public SetListKindOfParagraphCommand(Paragraph target, ListKind listKind) {
            _target = target;
            _listKind = listKind;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null && _target.ListKind != _listKind; }
        }

        public override bool CanUndo {
            get { return true; }
        }

        // ========================================
        // method
        // ========================================
        public override void Execute() {
            var newPadding = _target.Padding;
            if (_listKind != ListKind.None && _target.ListKind == ListKind.None) {
                newPadding = new Insets(
                    newPadding.Left + StyledTextConsts.IndentPaddingWidth,
                    newPadding.Top,
                    newPadding.Right,
                    newPadding.Bottom
                );
            } else if (_listKind == ListKind.None && _target.ListKind != ListKind.None) {
                newPadding = new Insets(
                    newPadding.Left - StyledTextConsts.IndentPaddingWidth,
                    newPadding.Top,
                    newPadding.Right,
                    newPadding.Bottom
                );
            }

            _command = new SetParagraphPropertiesCommand(
                _target,
                newPadding,
                _target.LineSpace,
                _target.HorizontalAlignment,
                _target.ParagraphKind,
                _listKind,
                _target.ListLevel,
                ListStateKind.Unchecked
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
