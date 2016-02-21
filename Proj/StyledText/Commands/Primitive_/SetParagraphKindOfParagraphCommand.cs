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
using Mkamo.Common.DataType;
using Mkamo.Common.Forms.Descriptions;

namespace Mkamo.StyledText.Commands {
    public class SetParagraphKindOfParagraphCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private Paragraph _target;
        private ParagraphKind _paragraphKind;

        private ICommand _command;
        private ICommand _toNormalParaCommand;

        // ========================================
        // constructor
        // ========================================
        public SetParagraphKindOfParagraphCommand(Paragraph target, ParagraphKind paragraphKind) {
            _target = target;
            _paragraphKind = paragraphKind;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null && _target.ParagraphKind != _paragraphKind && _target.Root != null; }
        }

        public override bool CanUndo {
            get { return true; }
        }

        // ========================================
        // method
        // ========================================
        public override void Execute() {
            var styledText = _target.Root;
            var font = styledText.GetDefaultFont(_paragraphKind);
            var range = styledText.GetRange(_target);
            range = new Range(range.Offset, range.Length - 1);

            Func<Flow, FontDescription> fontProvider = flow => new FontDescription(
                flow.Font.Name,
                font.Size,
                font.Style
            );

            if (_target.ListKind != ListKind.None || _target.ListLevel > 0) {
                /// リストとインデントを解除
                _toNormalParaCommand = new CompositeCommand();
                if (_target.ListKind != ListKind.None) {
                    _toNormalParaCommand.Chain(new SetListKindOfParagraphCommand(_target, ListKind.None));
                }
                if (_target.ListLevel > 0) {
                    _toNormalParaCommand.Chain(new IndentParagraphCommand(_target, -_target.ListLevel));
                }
                _toNormalParaCommand.Execute();
            }

            _command = new CompositeCommand();
            if (!range.IsEmpty) {
                _command.Chain(new SetFontCommand(styledText, range, fontProvider));
            }
            _command.Chain(new SetParagraphPropertiesCommand(
                _target,
                _target.GetDefaultPadding(_paragraphKind),
                _target.LineSpace,
                _target.HorizontalAlignment,
                _paragraphKind,
                _target.ListKind,
                _target.ListLevel,
                _target.ListState
            ));

            _command.Execute();
        }

        public override void Undo() {
            if (_command != null) {
                _command.Undo();
            }
            if (_toNormalParaCommand != null) {
                _toNormalParaCommand.Undo();
            }
        }
    }
}
