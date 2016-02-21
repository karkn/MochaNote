/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Collection;
using Mkamo.Common.Command;
using Mkamo.Common.DataType;
using Mkamo.Common.Core;
using Mkamo.StyledText.Core;

namespace Mkamo.StyledText.Commands {
    using StyledText = Mkamo.StyledText.Core.StyledText;

    public class InsertInlineCommand: AbstractCommand, IRangeCommand {
        // ========================================
        // field
        // ========================================
        private StyledText _target;
        private int _index;
        private Inline _inserted;

        private List<ICommand> _commands;

        // ========================================
        // constructor
        // ========================================
        public InsertInlineCommand(StyledText target, int index, Inline inserted) {
            _target = target;
            _index = index;
            _inserted = inserted;

            _commands = new List<ICommand>();
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null; }
        }

        public override bool CanUndo {
            get { return true; }
        }

        public Range ExecutedRange {
            get { return new Range(_index + _inserted.Length, 0); }
        }

        public Range UndoneRange {
            get { return new Range(_index, 0); }
        }


        // ========================================
        // method
        // ========================================
        public override void Execute() {
            _commands.Clear();

            int inlineOffset, charIndexInInline;
            var inline = _target.GetInlineAt(_index, out charIndexInInline, out inlineOffset);

            if (charIndexInInline == 0) {
                /// inlineの最初の文字を指している場合，inlineの直前に挿入する
                var line = inline.Parent as LineSegment;
                var index = line._Inlines.IndexOf(inline);
                _commands.Add(new InsertInlineToLineSegmentCommand(line, _inserted, index));

            } else {
                /// inlineの最初文字でない場合，inlineを分割して間に挿入する
                var line = inline.Parent as LineSegment;
                var inlineClone = inline.Clone() as Inline;
                var index = line._Inlines.IndexOf(inline);
                _commands.Add(new RemoveStringFromInlineCommand(inline, charIndexInInline));
                _commands.Add(new RemoveStringFromInlineCommand(inlineClone, 0, charIndexInInline));
                _commands.Add(new InsertInlineToLineSegmentCommand(line, _inserted, index + 1));
                _commands.Add(new InsertInlineToLineSegmentCommand(line, inlineClone, index + 2));
            }

            foreach (var cmd in _commands) {
                cmd.Execute();
            }
        }

        public override void Undo() {
            for (int i = _commands.Count - 1; i >= 0; --i) {
                _commands[i].Undo();
            }
        }
    }
}
