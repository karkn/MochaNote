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

    public class RemoveCommand: AbstractCommand, IRangeCommand {
        // ========================================
        // field
        // ========================================
        private StyledText _target;
        private int _index;
        private int _length;

        private List<ICommand> _commands;

        // ========================================
        // constructor
        // ========================================
        public RemoveCommand(StyledText target, int index, int length) {
            _target = target;
            _index = index;
            _length = length;

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
            get { return new Range(_index, 0); }
        }

        public Range UndoneRange {
            get { return new Range(_index, _length); }
        }

        // ========================================
        // method
        // ========================================
        public override void Execute() {
            _commands.Clear();

            if (_length == 1) {
                /// 削除する長さが1のとき
                
                int inlineOffset, charIndexInInline;
                var inline = _target.GetInlineAt(_index, out charIndexInInline, out inlineOffset);

                if (inline.IsLineEndCharacter) {
                    var line = inline.Parent as LineSegment;

                    if (inline is LineBreak) {
                        /// line breakならlineをマージ
                        var nextLine = line.NextSibling as LineSegment;
                        var cmd = new MergeLineSegmentsCommand(line, nextLine);
                        cmd.Execute();
                        _commands.Add(cmd);

                    } else if (inline is BlockBreak) {
                        /// block breakならblockをマージ
                        var block = line.Parent as Block;
                        if (block.HasNextSibling) {
                            var nextBlock = block.NextSibling as Block;
                            var lastLine = block.LineSegments.Last();
                            var nextFirstLine = nextBlock.LineSegments.First();

                            var mbcmd = new MergeBlocksCommand(block, nextBlock);
                            mbcmd.Execute();
                            _commands.Add(mbcmd);

                            var mlcmd = new MergeLineSegmentsCommand(lastLine, nextFirstLine);
                            mlcmd.Execute();
                            _commands.Add(mlcmd);
                        }
                    }
                } else {
                    if (inline.Length == 1) {
                        var line = inline.Parent as LineSegment;
                        var cmd = new RemoveInlineFromLineSegmentCommand(line, inline);
                        cmd.Execute();
                        _commands.Add(cmd);
                    } else {
                        var cmd = new RemoveStringFromInlineCommand(inline, charIndexInInline, 1);
                        cmd.Execute();
                        _commands.Add(cmd);
                    }
                }
            } else if (_length > 1) {
                var range = new Range(_index, _length);

                /// rangeの最初と最後でblockを分ける
                var endInline = _target.GetInlineAt(range.End);
                var endcmd = new EnsureBlockBreakCommand(_target, range.End + 1);
                endcmd.Execute();
                _commands.Add(endcmd);
                range = endcmd.IsExecuted && !endInline.IsLineEndCharacter?
                    new Range(_index, _length + 1):
                    range;

                var prevInline = range.Offset > 0? _target.GetInlineAt(range.Offset - 1): null;
                var startcmd = new EnsureBlockBreakCommand(_target, range.Offset);
                startcmd.Execute();
                _commands.Add(startcmd);
                range = startcmd.IsExecuted && !prevInline.IsLineEndCharacter?
                    new Range(range.Offset + 1, range.Length):
                    range;

                /// 範囲内のblockをすべて削除
                var removeds = new List<Block>();
                foreach (var block in _target.Blocks) {
                    var blockRange = _target.GetRange(block);
                    if (range.Contains(blockRange)) {
                        removeds.Add(block);
                    }
                }

                foreach (var block in removeds) {
                    var cmd = new RemoveBlockFromStyledTextCommand(_target, block);
                    cmd.Execute();
                    _commands.Add(cmd);
                }

                /// rangeの最初でblockを分けた場合はmergeしておく
                if (startcmd.IsExecuted && !prevInline.IsLineEndCharacter) {
                    /// 直前のinlineがLineBreakではなかったならば
                    /// blockをつないでlineもつなげる
                    /// (BlockBreakをLineBreakにして，さらにLineBreakを削除)

                    var block = _target.GetBlockAt(_index);
                    var nextBlock = block.NextSibling as Block;
                    var lastLine = block.LineSegments.Last();
                    var nextFirstLine = nextBlock.LineSegments.First();

                    var mbcmd = new MergeBlocksCommand(block, nextBlock);
                    mbcmd.Execute();
                    _commands.Add(mbcmd);

                    var mlcmd = new MergeLineSegmentsCommand(lastLine, nextFirstLine);
                    mlcmd.Execute();
                    _commands.Add(mlcmd);

                } else if (startcmd.IsExecuted && prevInline.IsLineEndCharacter) {
                    /// 直前のinlineがLineBreakだったならば
                    /// blockをつなぐ
                    /// (BlockBreakをLineBreakにするだけ)

                    var block = _target.GetBlockAt(_index - 1);
                    var nextBlock = block.NextSibling as Block;

                    var mbcmd = new MergeBlocksCommand(block, nextBlock);
                    mbcmd.Execute();
                    _commands.Add(mbcmd);
                }

            }

        }

        public override void Undo() {
            for (int i = _commands.Count - 1; i >= 0; --i) {
                _commands[i].Undo();
            }
        }
    }
}
