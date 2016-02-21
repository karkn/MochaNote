/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.StyledText.Util;
using Mkamo.Common.Command;
using Mkamo.StyledText.Core;
using Mkamo.Common.DataType;

namespace Mkamo.StyledText.Commands {
    using StyledText = Mkamo.StyledText.Core.StyledText;

    public class InsertBlocksAndInlinesCommand: AbstractCommand, IRangeCommand {

        // ========================================
        // field
        // ========================================
        private StyledText _target;
        private int _index;
        private IEnumerable<Flow> _blocksAndInlines;

        private int _insertedIndex;
        private List<ICommand> _commands;

        // ========================================
        // constructor
        // ========================================
        public InsertBlocksAndInlinesCommand(StyledText target, int index, IEnumerable<Flow> blocksAndInlines) {
            _target = target;
            _index = index;
            _blocksAndInlines = blocksAndInlines;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null && _blocksAndInlines != null && _blocksAndInlines.Any(); }
        }

        public override bool CanUndo {
            get { return true; }
        }

        public Range ExecutedRange {
            get { return new Range(_insertedIndex, 0); }
        }

        public Range UndoneRange {
            get { return new Range(_index, 0); }
        }

        // ========================================
        // method
        // ========================================
        public override void Execute() {
            _insertedIndex = InsertFlows(_blocksAndInlines);
        }

        public override void Undo() {
            if (_commands != null && _commands.Any()) {
                foreach (var cmd in _commands.Reverse<ICommand>()) {
                    cmd.Undo();
                }
            }
        }

        public override void Redo() {
            if (_blocksAndInlines != null) {
                _insertedIndex = InsertFlows(_blocksAndInlines);
            }
        }

        private int InsertFlows(IEnumerable<Flow> flows) {
            _commands = new List<ICommand>();
            {
                var cmd = new EnsureInlineBreakCommand(_target, _index);
                cmd.Execute();
                _commands.Add(cmd);
            }

            var firstBlock = _target.GetBlockAt(_index);
            var bIndex = _target._Blocks.IndexOf(firstBlock);
            var cIndex = _index;
            foreach (var flow in flows) {
                if (flow is Block) {
                    var block = flow as Block;

                    int charIndexInLine, lineSegOffset;
                    _target.GetLineSegmentAt(cIndex, out charIndexInLine, out lineSegOffset);

                    if (charIndexInLine == 0) {
                        /// Line中でなければそのまま挿入
                        var cmd = new InsertBlockToStyledTextCommand(_target, block, bIndex);
                        cmd.Execute();
                        _commands.Add(cmd);
                        cIndex += flow.Length;

                    } else {
                        /// Line中ならInlineだけ挿入
                        foreach (var line in block.LineSegments) {
                            foreach (var inline in line.Inlines) {
                                if (inline.IsLineEndCharacter) {
                                    if (inline is BlockBreak) {
                                        var cmd = new InsertBlockBreakCommand(_target, cIndex);
                                        cmd.Execute();
                                        _commands.Add(cmd);

                                    } else if (inline is LineBreak) {
                                        var cmd = new InsertLineBreakCommand(_target, cIndex);
                                        cmd.Execute();
                                        _commands.Add(cmd);
                                    }

                                } else {
                                    var clone = inline.CloneDeeply() as Inline;
                                    var cmd = new InsertInlineCommand(_target, cIndex, clone);
                                    cmd.Execute();
                                    _commands.Add(cmd);
                                }
                                cIndex += inline.Length;
                            }
                        }
                    
                    }
                    ++bIndex;

                } else if (flow is Inline) {
                    var inline = flow as Inline;
                    if (inline.IsLineEndCharacter) {
                        if (inline is BlockBreak) {
                            var cmd = new InsertBlockBreakCommand(_target, cIndex);
                            cmd.Execute();
                            _commands.Add(cmd);
                            ++bIndex;

                        } else if (inline is LineBreak) {
                            var cmd = new InsertLineBreakCommand(_target, cIndex);
                            cmd.Execute();
                            _commands.Add(cmd);
                        }

                    } else {
                        var clone = inline.CloneDeeply() as Inline;
                        var cmd = new InsertInlineCommand(_target, cIndex, clone);
                        cmd.Execute();
                        _commands.Add(cmd);
                    }
                    cIndex += flow.Length;
                }
            }

            /// 最初のinlineがmergeできるならmerge
            var firstInline = _target.GetInlineAt(_index);
            if (_target.HasPrevInline(firstInline)) {
                var prev = _target.GetPrevInline(firstInline);
                if (prev.CanMerge(firstInline)) {
                    var cmd = new MergeInlineCommand(prev, firstInline);
                    cmd.Execute();
                    _commands.Add(cmd);
                }
            }

            /// 最後のinlineがmergeできるならmerge
            var lastInline = _target.GetInlineAt(cIndex - 1);
            if (_target.HasNextInline(lastInline)) {
                var next = _target.GetNextInline(lastInline);
                if (lastInline.CanMerge(next)) {
                    var cmd = new MergeInlineCommand(lastInline, next);
                    cmd.Execute();
                    _commands.Add(cmd);
                }
            }

            return cIndex;
        }
    }
}
