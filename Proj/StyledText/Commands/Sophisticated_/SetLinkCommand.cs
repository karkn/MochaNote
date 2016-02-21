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

    public class SetLinkCommand: AbstractCommand, IRangeCommand {
        // ========================================
        // field
        // ========================================
        private StyledText _target;
        private Range _range;
        private Link _link;

        private ICommand _command;

        // ========================================
        // constructor
        // ========================================
        public SetLinkCommand(
            StyledText target,
            Range range,
            Link link
        ) {
            _target = target;
            _range = range;
            _link = link;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null && !_range.IsEmpty; }
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
            _command = new ApplyCommandCommand(
                _target,
                _range,
                (flow) => {
                    var run = flow as Run;
                    if (run == null) {
                        return new NoopCommand();
                    } else {
                        return new SetLinkOfRunCommand(run, _link);
                    }
                }
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
