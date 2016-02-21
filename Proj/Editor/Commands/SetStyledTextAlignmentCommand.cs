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
using Mkamo.StyledText.Commands;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.StyledText.Core;
using Mkamo.Common.DataType;

namespace Mkamo.Editor.Commands {
    using StyledText = Mkamo.StyledText.Core.StyledText;

    public class SetStyledTextListKindCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private IEditor _target;
        private Func<StyledText> _styledTextProvider;
        private ListKind _listKind;
        private bool _on;

        private CompositeCommand _command;

        // ========================================
        // constructor
        // ========================================
        public SetStyledTextListKindCommand(
            IEditor target,
            Func<StyledText> styledTextProvider,
            ListKind listKind,
            bool on
        ) {
            _target = target;
            _styledTextProvider = styledTextProvider;
            _listKind = listKind;
            _on = on;

            _command = null;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null && _target.Figure is INode; }
        }

        public override bool CanUndo {
            get { return true; }
        }


        // ========================================
        // method
        // ========================================
        public override void Execute() {
            _command = null;

            var stext = _styledTextProvider();
            if (stext != null) {

                _command = new CompositeCommand();

                foreach (var block in stext.Blocks) {
                    var para = block as Paragraph;
                    if (para != null) {
                        if (_on) {
                            if (para.ListKind != _listKind) {
                                _command.Chain(new SetListKindOfParagraphCommand(para, _listKind));
                            }
                        } else {
                            if (para.ListKind != ListKind.None) {
                                _command.Chain(new SetListKindOfParagraphCommand(para, ListKind.None));
                            }
                        }
                    }
                }

                if (_command.Children.Count > 0) {
                    _command.Execute();
                }
            }
        }

        public override void Undo() {
            if (_command != null && _command.Children.Count > 0) {
                _command.Undo();
            }
        }
    }
}
