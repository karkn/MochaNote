/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Command;
using Mkamo.Common.DataType;
using Mkamo.StyledText.Util;
using Mkamo.StyledText.Core;

namespace Mkamo.StyledText.Commands {
    using StyledText = Mkamo.StyledText.Core.StyledText;

    public class ApplyCommandCommand: AbstractCommand, IRangeCommand {
        // ========================================
        // field
        // ========================================
        private StyledText _target;
        private Range _range;
        private Func<Flow, ICommand> _commandProvider;

        private List<ICommand> _commands;
        private ICommand _ensureRangeStartBreak;
        private ICommand _ensureRangeEndBreak;
        private ICommand _compress;

        // ========================================
        // constructor
        // ========================================
        public ApplyCommandCommand(StyledText target, Range range, Func<Flow, ICommand> commandProvider) {
            _target = target;
            _range = range;
            _commandProvider = commandProvider;

            _commands = new List<ICommand>();
            _compress = null;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null && !_range.IsEmpty && _commandProvider != null; }
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
            _commands.Clear();
            _compress = null;
            _ensureRangeStartBreak = null;
            _ensureRangeEndBreak = null;

            _ensureRangeStartBreak = new EnsureInlineBreakCommand(_target, _range.Start);
            _ensureRangeStartBreak.Execute();
            
            _ensureRangeEndBreak = new EnsureInlineBreakCommand(_target, _range.End + 1);
            _ensureRangeEndBreak.Execute();

            var cOffset = 0;
            var firstInline = default(Inline);
            var lastInline = default(Inline);
            foreach (var inline in _target.Inlines) {
                var inlineRange = new Range(cOffset, inline.Length);
                if (_range.Contains(inlineRange)) {
                    if (cOffset == _range.Offset) {
                        firstInline = inline;
                    }
                    if (cOffset + inline.Length - 1 == _range.End) {
                        lastInline = inline;
                    }

                    var cmd = _commandProvider(inline);
                    if (cmd != null) {
                        cmd.Execute();
                        _commands.Add(cmd);
                    }

                    if (lastInline != null) {
                        break;
                    }
                }

                cOffset += inline.Length;
            }

            if (firstInline != null && lastInline != null) {
                firstInline = firstInline.HasPrevSibling? firstInline.PrevSibling as Inline: firstInline;
                lastInline = lastInline.HasNextSibling? lastInline.NextSibling as Inline: lastInline;

                _compress = new CompressInlinesCommand(_target, firstInline, lastInline);
                _compress.Execute();
            }
        }

        public override void Undo() {
            if (_compress != null) {
                _compress.Undo();
            }
            for (int i = _commands.Count - 1; i >= 0; --i) {
                _commands[i].Undo();
            }
            if (_ensureRangeEndBreak != null) {
                _ensureRangeEndBreak.Undo();
            }
            if (_ensureRangeStartBreak != null) {
                _ensureRangeStartBreak.Undo();
            }
        }

    }
}
