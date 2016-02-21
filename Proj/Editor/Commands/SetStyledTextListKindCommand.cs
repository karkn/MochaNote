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
    using Mkamo.Common.Core;

    public class SetStyledTextAlignmentCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private IEditor _target;
        private Func<StyledText> _styledTextProvider;
        private AlignmentModificationKinds _kinds;
        private HorizontalAlignment _newHorizontalAlignment;
        private VerticalAlignment _newVerticalAlignment;

        private ICommand _command;

        // ========================================
        // constructor
        // ========================================
        public SetStyledTextAlignmentCommand(
            IEditor target,
            Func<StyledText> styledTextProvider,
            AlignmentModificationKinds kinds,
            HorizontalAlignment newHorizontalAlignment,
            VerticalAlignment newVerticalAlignment
        ) {
            _target = target;
            _styledTextProvider = styledTextProvider;
            _kinds = kinds;
            _newVerticalAlignment = newVerticalAlignment;
            _newHorizontalAlignment = newHorizontalAlignment;

            _command = null;
        }

        public SetStyledTextAlignmentCommand(
            IEditor target,
            Func<StyledText> styledTextProvider,
            HorizontalAlignment newHorizontalAlignment
        ){ 
            _target = target;
            _styledTextProvider = styledTextProvider;
            _kinds = AlignmentModificationKinds.Horizontal;
            _newHorizontalAlignment = newHorizontalAlignment;

            _command = null;
        }

        public SetStyledTextAlignmentCommand(
            IEditor target,
            Func<StyledText> styledTextProvider,
            VerticalAlignment newVerticalAlignment
        ){ 
            _target = target;
            _styledTextProvider = styledTextProvider;
            _kinds = AlignmentModificationKinds.Vertical;
            _newVerticalAlignment = newVerticalAlignment;

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
                if (EnumUtil.HasAllFlags((int) _kinds, (int) AlignmentModificationKinds.Horizontal)) {
                    _command = new CompositeCommand();
                    foreach (var block in stext.Blocks) {
                        _command.Chain(new SetHorizontalAlignmentOfBlock(block, _newHorizontalAlignment));
                    }
                }
                if (EnumUtil.HasAllFlags((int) _kinds, (int) AlignmentModificationKinds.Vertical)) {
                    var cmd =  new SetVerticalAlignmentOfStyledTextCommand(stext, _newVerticalAlignment);
                    _command = _command == null? cmd: _command.Chain(cmd);
                }

                _command.Execute();
            }
        }

        public override void Undo() {
            if (_command != null) {
                _command.Undo();
            }
        }
    }
}
