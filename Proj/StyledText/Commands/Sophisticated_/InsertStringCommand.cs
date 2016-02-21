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
using Mkamo.StyledText.Core;

namespace Mkamo.StyledText.Commands {
    using StyledText = Mkamo.StyledText.Core.StyledText;

    public class InsertStringCommand: AbstractCommand, IRangeCommand {
        // ========================================
        // field
        // ========================================
        private StyledText _target;
        private int _index;
        private string _text;

        private ICommand _command;

        // ========================================
        // constructor
        // ========================================
        public InsertStringCommand(StyledText target, int index, string text) {
            _target = target;
            _index = index;
            _text = text;
            
            _command = null;
        }

        // ========================================
        // property
        // ========================================
        public override bool CanExecute {
            get { return _target != null && _text != null; }
        }

        public override bool CanUndo {
            get { return true; }
        }

        public Range ExecutedRange {
            get { return new Range(_index + _text.Length, 0); }
        }

        public Range UndoneRange {
            get { return new Range(_index, 0); }
        }


        // ========================================
        // method
        // ========================================
        public override void Execute() {
            _command = null;

            int inlineOffset = 0;
            int charIndexInInline = 0;
            var inline = _target.GetInlineAt(_index, out charIndexInInline, out inlineOffset);

            if (inline.IsLineEndCharacter || inline.IsAnchorCharacter) {
                /// _indexのinlineがControlCharならば

                if (_target.HasPrevInline(inline)) {
                    /// 文頭でなければ

                    var prev = _target.GetPrevInline(inline);
                    if (prev.IsLineEndCharacter || prev.IsAnchorCharacter) {
                        /// inlineの前がControlCharacterなら直前にrunを追加
                        var parent = inline.Parent as LineSegment;
                        var index = parent._Inlines.IndexOf(inline);

                        var newRun = new Run(_text);
                        if (prev.IsLineEndCharacter && prev.HasPrevSibling) {
                            /// inlineの前のLineEndCharacterの前に
                            /// ControlCharacterでないinlineがあればtransfer

                            var para = parent.Parent as Paragraph;
                            var pp = prev.PrevSibling as Inline;
                            var ppLine = pp.Parent;
                            var ppPara = ppLine == null ? null : ppLine.Parent as Paragraph;
                            var ppParaIsNormal = ppPara != null && ppPara.ParagraphKind == ParagraphKind.Normal;
                            var ppParaIsEqual = ppPara != null && ppPara.ParagraphKind == para.ParagraphKind;
                            if (pp != null && !pp.IsLineEndCharacter && !pp.IsAnchorCharacter && ppParaIsNormal && ppParaIsEqual) {
                                var ppRun = pp as Run;
                                if (ppRun != null) {
                                    /// Linkは写さない
                                    ppRun.TransferWithoutLink(newRun);
                                } else {
                                    pp.Transfer(newRun);
                                }
                            } else if (para.ParagraphKind != ParagraphKind.Normal) {
                                newRun.Font = _target.GetDefaultFont(para.ParagraphKind);
                            }
                        }
                        _command = new InsertInlineToLineSegmentCommand(parent, newRun, index);
                    
                    } else {

                        var run = prev as Run;
                        if (run != null && run.HasLink) {
                            /// prevにlinkが設定されている場合はlinkを引き継がないように直前にrunを追加
                            var parent = inline.Parent as LineSegment;
                            var index = parent._Inlines.IndexOf(inline);
                            var newRun = new Run(_text);
                            run.TransferWithoutLink(newRun);
                            _command = new InsertInlineToLineSegmentCommand(parent, newRun, index);
                        } else {
                            _command = new AppendStringToInlineCommand(prev, _text);
                        }
                    }

                } else {
                    /// 文頭の場合prevがないので挿入しておく
                    var first = _target.Blocks.First();
                    var newRun = new Run(_text);
                    _target.Transfer(newRun);
                    _command = new InsertInlineBeforeToBlockCommand(first, newRun);
                }
            } else {
                /// _indexのinlineがControlCharでなければ

                if (_index == inlineOffset) {
                    /// inlineの最初の文字を指している場合，

                    /// StyledText中で一番最初のinlineだったり，
                    /// ControlCharacter直後のinlineの場合はinlineにInsert，
                    /// そうでなければ直前のinlineにAppend
                    if (_target.HasPrevInline(inline)) {
                        var prev = _target.GetPrevInline(inline);
                        if (prev.IsLineEndCharacter || prev.IsAnchorCharacter) {
                            _command = new InsertStringToInlineCommand(inline, _text, charIndexInInline);
                        } else {
                            var run = prev as Run;
                            if (run != null && run.HasLink) {
                                /// prevにlinkが設定されている場合はlinkを引き継がないように直前にrunを追加
                                var parent = inline.Parent as LineSegment;
                                var index = parent._Inlines.IndexOf(inline);
                                var newRun = new Run(_text);
                                run.TransferWithoutLink(newRun);
                                _command = new InsertInlineToLineSegmentCommand(parent, newRun, index);

                            } else {
                                _command = new AppendStringToInlineCommand(prev, _text);
                            }
                        }

                    } else {
                        var run = inline as Run;
                        if (run != null && run.HasLink) {
                            /// inlineにlinkを設定されている場合はlinkにならないように直前にrunを挿入
                            var parent = inline.Parent as LineSegment;
                            var index = parent._Inlines.IndexOf(inline);
                            var newRun = new Run(_text);
                            run.TransferWithoutLink(newRun);
                            _command = new InsertInlineToLineSegmentCommand(parent, newRun, index);
                        } else {
                            /// inlineにlinkが設定されていなければそのまま文字列をinsert
                            _command = new InsertStringToInlineCommand(inline, _text, charIndexInInline);
                        }
                    }
                } else {

                    /// inlineの最初でなければ普通にInsert
                    _command = new InsertStringToInlineCommand(inline, _text, charIndexInInline);
                }
            }

            _command.Execute();
        }

        public override void Undo() {
            if (_command != null) {
                _command.Undo();
            }
        }
    }
}
