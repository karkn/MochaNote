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
using Mkamo.StyledText.Core;

namespace Mkamo.StyledText.Commands {
    using StyledText = Mkamo.StyledText.Core.StyledText;

    public class CompressInlinesCommand: AbstractCommand {
        // ========================================
        // field
        // ========================================
        private StyledText _target;
        private Inline _firstInline;
        private Inline _lastInline;

        private List<ICommand> _commands;

        // ========================================
        // constructor
        // ========================================
        public CompressInlinesCommand(StyledText target, Inline firstInline, Inline lastInline) {
            _target = target;
            _firstInline = firstInline;
            _lastInline = lastInline;

            _commands = new List<ICommand>();
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null && _firstInline != null && _lastInline != null; }
        }

        public override bool CanUndo {
            get { return true; }
        }

        //public override Range UndoneRange {
        //    get {
        //        var offset = _target.GetInlineOffset(_firstInline);
        //        var length = (_target.GetInlineOffset(_lastInline) + _lastInline.Length - 1) - offset;
        //        return new Range(offset, length);
        //    }
        //}

        // ========================================
        // method
        // ========================================
        public override void Execute() {
            _commands.Clear();

            var mergingsList = new List<List<Inline>>();

            /// 統合対象となるinlineの収集
            var mergings = default(List<Inline>);
            var prev = default(Inline);
            var isStarted = false;
            var isDone = false;
            foreach (var inline in _target.Inlines) {
                if (isStarted) {
                    if (inline.CanMerge(prev) && inline.Parent == prev.Parent) {
                        if (mergings == null) {
                            mergings = new List<Inline>();
                            mergings.Add(prev);
                            mergings.Add(inline);
                        } else {
                            mergings.Add(inline);
                        }
                    } else {
                        if (mergings != null) {
                            mergingsList.Add(mergings);
                            mergings = null;
                        }
                    }
                }

                if (inline == _firstInline) {
                    isStarted = true;
                }

                if (inline == _lastInline) {
                    if (mergings != null) {
                        mergingsList.Add(mergings);
                        mergings = null;
                    }
                    isDone = true;
                    break;
                }

                prev = inline;
            }
            if (!isDone) {
                throw new ArgumentException("first and last");
            }

            /// 統合処理
            foreach (var inlines in mergingsList) {
                var first = inlines.First();
                var parent = first.Parent as LineSegment;

                var buf = new StringBuilder();
                inlines.ForEach(inline => buf.Append(inline.Text));

                _commands.Add(new SetTextOfFlowCommand(first, buf.ToString()));

                for (int i = inlines.Count - 1; i >= 0; --i) {
                    var inline = inlines[i];
                    if (inline != first) {
                        _commands.Add(new RemoveInlineFromLineSegmentCommand(parent, inline));
                    }
                }
            }

            /// 実行
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
