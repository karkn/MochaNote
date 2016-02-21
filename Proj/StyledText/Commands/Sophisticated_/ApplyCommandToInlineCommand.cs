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
using Mkamo.Common.Diagnostics;
using Mkamo.StyledText.Core;

namespace Mkamo.StyledText.Commands {
    /// inlineのrangeにapplicationActionをする．
    /// inlineはrangeの前方部分，range部分，rangeの後方部分に分割される．
    /// rangeはinline内での範囲(0～inline.length - 1)
    public class ApplyCommandToInlineCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private Inline _target;
        private Func<Flow, ICommand> _commandProvider;
        private Range _range;

        private List<ICommand> _commands;

        // ========================================
        // constructor
        // ========================================
        public ApplyCommandToInlineCommand(Inline target, Range range, Func<Flow, ICommand> commandProvider) {
            _target = target;
            _commandProvider = commandProvider;
            _range = range;

            _commands = new List<ICommand>();
        }

        public ApplyCommandToInlineCommand(Inline target, int index, int length, Func<Flow, ICommand> commandProvider):
            this(target, new Range(index, length), commandProvider) {

        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null && _target.Parent != null && _commandProvider != null; }
        }

        public override bool CanUndo {
            get { return true; }
        }

        //public override Range UndoneRange {
        //    get { return _range; }
        //}

        // ========================================
        // method
        // ========================================
        public override void Execute() {
            _commands.Clear();

            var line = _target.Parent as LineSegment;
            Contract.Ensures(line != null, "Parent of inline must not be null");

            var inlineIndex = line._Inlines.IndexOf(_target);

            if (_range.End < _target.Length - 1) {
                /// rangeより後ろの部分があればそこで分割

                var split = new SplitInlineCommand(_target, _range.End);
                split.Execute();
                _commands.Add(split);
            }

            if (_range.Offset > 0) {
                /// rangeより前の部分があれば分割してrange内のinlineにコマンド適用

                var split = new SplitInlineCommand(_target, _range.Offset);
                split.Execute();
                _commands.Add(split);

                var provided = _commandProvider(split.Splitted);
                provided.Execute();
                _commands.Add(provided);

            } else {
                var provided = _commandProvider(_target);
                provided.Execute();
                _commands.Add(provided);
            }
        }

        public override void Undo() {
            for (int i = _commands.Count - 1; i >= 0; --i) {
                _commands[i].Undo();
            }
        }
    }
}
