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
    using Mkamo.Common.Forms.Drawing;

    public class InsertBlockBreakCommand: AbstractCommand, IRangeCommand {
        // ========================================
        // field
        // ========================================
        private StyledText _target;
        private int _index;

        private ICommand _command;

        // ========================================
        // constructor
        // ========================================
        public InsertBlockBreakCommand(StyledText target, int index) {
            _target = target;
            _index = index;

            _command = null;
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
            get { return new Range(_index + 1, 0); }
        }

        public Range UndoneRange {
            get { return new Range(_index, 0); }
        }


        // ========================================
        // method
        // ========================================
        public override void Execute() {
            _command = null;
 
            int blockIndex, charIndexInBlock;
            var block = _target.GetBlockAtLocal(_index, out blockIndex, out charIndexInBlock);
            var isLastIndexInLine = false;

            if (charIndexInBlock == 0) {
                /// blockの最初の文字を指している場合，blockの直前に空のBlockを作成して挿入する
                var para = new Paragraph();
                block.Transfer(para);
                _command = new InsertBlockToStyledTextCommand(_target, para, blockIndex);

            } else {
                /// blockの途中の文字を指している場合，blockをそこで分割する
                
                int lineIndex, charIndexInLine;
                var line = block.GetLineSegmentAtLocal(charIndexInBlock, out lineIndex, out charIndexInLine);

                isLastIndexInLine = charIndexInLine == line.Length - 1;

                if (charIndexInLine == 0) {
                    /// LineSegmentの先頭を指している場合
                    _command = new InsertLineSegmentToBlockCommand(block, new LineSegment(), lineIndex);
                    _command = _command.Chain(new SplitBlockCommand(block, lineIndex));

                } else {
                    _command = new EnsureBlockBreakCommand(_target, _index);
                }
            }

            _command.Execute();

            if (charIndexInBlock != 0 && isLastIndexInLine && block.HasNextSibling) {
                var nextPara = block.NextSibling as Paragraph;
                var setPara = new SetParagraphPropertiesCommand(
                        nextPara,
                        GetNewPadding(nextPara.GetDefaultPadding(ParagraphKind.Normal), nextPara.ListKind, nextPara.ListLevel),
                        //nextPara.Padding,
                        nextPara.LineSpace,
                        nextPara.HorizontalAlignment,
                        ParagraphKind.Normal,
                        nextPara.ListKind,
                        nextPara.ListLevel,
                        nextPara.ListState
                );
                setPara.Execute();
                _command.Chain(setPara);
            }
        }

        public override void Undo() {
            if (_command != null) {
                _command.Undo();
            }
        }

        private static Insets GetNewPadding(Insets oldPadding, ListKind listKind, int listLevel) {
            var indent = listKind == ListKind.None ? listLevel : listLevel + 1;
            return new Insets(
                oldPadding.Left + (StyledTextConsts.IndentPaddingWidth * indent),
                oldPadding.Top,
                oldPadding.Right,
                oldPadding.Bottom
            );
        }
    }
}
