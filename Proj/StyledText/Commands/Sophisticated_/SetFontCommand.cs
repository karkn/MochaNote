/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.DataType;
using Mkamo.StyledText.Core;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Common.Command;

namespace Mkamo.StyledText.Commands {
    using StyledText = Mkamo.StyledText.Core.StyledText;

    public class SetFontCommand: AbstractCommand, IRangeCommand {
        // ========================================
        // field
        // ========================================
        private StyledText _target;
        private Range _range;
        private Func<Flow, FontDescription> _fontProvider;

        private ICommand _command;

        // ========================================
        // constructor
        // ========================================
        public SetFontCommand(
            StyledText target,
            Range range,
            Func<Flow, FontDescription> fontProvider
        ) {
            _target = target;
            _range = range;
            _fontProvider = fontProvider;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null && !_range.IsEmpty && _fontProvider != null; }
        }

        public override bool CanUndo {
            get { return true; }
        }

        public Range ExecutedRange {
            get { return _range; }
        }

        public Range UndoneRange {
            get { return _range; }
        }


        // ========================================
        // method
        // ========================================
        public override void Execute() {
            var font = _fontProvider(_target.GetInlineAt(_range.Offset));
            _command = new ApplyCommandCommand(
                _target,
                _range,
                (flow) => new SetFontOfFlowCommand(flow, font)
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
