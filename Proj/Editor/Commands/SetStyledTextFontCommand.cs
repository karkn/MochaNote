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

    public class SetStyledTextFontCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private IEditor _target;
        private Func<StyledText> _styledTextProvider;
        private Func<Flow, FontDescription> _fontProvider;

        private ICommand _command;
        private FontDescription _oldStyledTextFont;

        // ========================================
        // constructor
        // ========================================
        public SetStyledTextFontCommand(
            IEditor target,
            Func<StyledText> styledTextProvider,
            Func<Flow, FontDescription> fontProvider
        ){ 
            _target = target;
            _styledTextProvider = styledTextProvider;
            _fontProvider = fontProvider;

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
                _oldStyledTextFont = stext.Font;
                stext.Font = _fontProvider(stext);

                var range = new Range(0, stext.Length - 1);
                _command = new SetFontCommand(stext, range, _fontProvider);
                _command.Execute();
            }
        }

        public override void Undo() {
            if (_command != null) {
                _command.Undo();
            }

            var stext = _styledTextProvider();
            if (stext != null) {
                stext.Font = _oldStyledTextFont;
            }
        }
    }
}
