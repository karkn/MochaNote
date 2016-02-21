/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.String;
using Mkamo.Common.Forms.Descriptions;
using System.Drawing;
using Mkamo.StyledText.Util;
using Mkamo.Common.Collection;
using Mkamo.Common.DataType;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Mkamo.Common.Forms.Clipboard;
using Mkamo.StyledText.Commands;
using Mkamo.Common.Command;
using Mkamo.Common.Forms.Drawing;

namespace Mkamo.StyledText.Core {
    using ICommandExecutor = Mkamo.Common.Command.ICommandExecutor;
    using HorizontalAlignment = Mkamo.Common.DataType.HorizontalAlignment;
    using VerticalAlignment = Mkamo.Common.DataType.VerticalAlignment;
    using Mkamo.Common.Core;
    
    /// <summary>
    /// caret，mark，selectionの位置の管理．
    /// 
    /// Move: カーソル移動
    /// Insert: 文字列挿入
    /// Remove: 文字列削除
    /// Yank: 文字列移動
    /// Paste: copy bufから貼り付け
    /// Cut: copy bufに切り取り
    /// Copy: copy bufにコピー
    /// 
    /// Region: MarkとCaretの間
    /// Selection: 選択範囲
    /// </summary>
    public class StyledTextReferer {
        // ========================================
        // field
        // ========================================
        private StyledText _target;

        private CommandExecutor _executor = new CommandExecutor();

        private int _caretIndex = 0;
        private int? _mark = null;
        private StyledTextSelection _selection;

        /// <summary>
        /// 次に入力するFontやColorが設定されたときにそれらの情報を格納するためのinline
        /// </summary>
        private Run _nextInputInline = null;

        /// <summary>
        /// 現在のcharIndexから前の行のcharIndexを取得するための関数．
        /// </summary>
        private Func<int, int> _prevLineCharIndexProvider;

        /// <summary>
        /// 現在のcharIndexから次の行のcharIndexを取得するための関数．
        /// </summary>
        private Func<int, int> _nextLineCharIndexProvider;

        /// <summary>
        /// 現在のcharIndexから前のページのcharIndexを取得するための関数．
        /// </summary>
        private Func<int, int> _prevPageCharIndexProvider;

        /// <summary>
        /// 現在のcharIndexから次のページのcharIndexを取得するための関数．
        /// </summary>
        private Func<int, int> _nextPageCharIndexProvider;

        // ========================================
        // constructor
        // ========================================
        public StyledTextReferer(
            StyledText target,
            Func<int, int> prevLineCharIndexProvider,
            Func<int, int> nextLineCharIndexProvider,
            Func<int, int> prevPageCharIndexProvider,
            Func<int, int> nextPageCharIndexProvider
        ) {
            _target = target;
            _selection = new StyledTextSelection(this);

            _prevLineCharIndexProvider = prevLineCharIndexProvider;
            _nextLineCharIndexProvider = nextLineCharIndexProvider;
            _prevPageCharIndexProvider = prevPageCharIndexProvider;
            _nextPageCharIndexProvider = nextPageCharIndexProvider;
        }

        // ========================================
        // event
        // ========================================
        public event EventHandler<CaretMovedEventArgs> CaretMoved;
        public event EventHandler<EventArgs> MarkSet;

        // ========================================
        // property
        // ========================================
        public StyledText Target {
            get { return _target; }
            set {
                if (value == _target) {
                    return;
                }
                _target = value;
                _caretIndex = 0;
                _mark = null;
                _selection.Clear();
            }
        }

        public int CaretIndex {
            get { return _caretIndex; }
            set {
                var newIndex = value < _target.Length? value: _target.Length - 1;
                if (newIndex == _caretIndex) {
                    return;
                }
                _nextInputInline = null;

                var old = _caretIndex;
                _caretIndex = newIndex;
                OnCaretMoved(new CaretMovedEventArgs(old, newIndex));
            }
        }

        public int? Mark {
            get { return _mark == null? (int?) null: Math.Min(_mark.Value, _target.Length - 1); }
            set {
                if (value == _mark) {
                    return;
                }
                _mark = value;
                OnMarkSet();
            }
        }

        public bool IsMarkSet {
            get { return _mark != null; }
        }

        public StyledTextSelection Selection {
            get { return _selection; }
        }

        public ICommandExecutor CommandExecutor {
            get { return _executor; }
        }

        public Run NextInputRun {
            get { return _nextInputInline; }
            set { _nextInputInline = value; }
        }

        public bool IsModified {
            get { return _executor.CanUndo; }
        }

        // ========================================
        // method
        // ========================================
        public FontDescription GetNextInputFont() {
            if (_nextInputInline != null) {
                return _nextInputInline.Font;
            }

            if (_target.IsLineHead(_caretIndex)) {
                /// 行頭であれば
                var inline = _target.GetInlineAt(_caretIndex);
                return inline.Font;

            } else {
                /// それ以外であれば
                var inline = _target.GetInlineAt(_caretIndex - 1);
                return inline.Font;
            }
        }

        public Color GetNextInputColor() {
            if (_nextInputInline != null) {
                return _nextInputInline.Color;
            }

            if (_target.IsLineHead(_caretIndex)) {
                /// 行頭であれば
                var inline = _target.GetInlineAt(_caretIndex);
                return inline.Color;

            } else {
                /// それ以外であれば
                var inline = _target.GetInlineAt(_caretIndex - 1);
                return inline.Color;
            }
        }

        public ParagraphKind? GetParagraphKind() {
            var para = _target.GetBlockAt(_caretIndex) as Paragraph;
            if (para == null) {
                return null;
            } else {
                return para.ParagraphKind;
            }
        }

        public Range GetRegion() {
            if (IsMarkSet) {
                return new Range(
                    Math.Min(Mark.Value, CaretIndex),
                    Math.Abs(Mark.Value - CaretIndex)
                );
            } else {
                return Range.Empty;
            }
        }

        public string GetInputingWordPart() {
            if (!_selection.IsEmpty) {
                return null;
            }

            if (_caretIndex < 1) {
                return null;
            }

            var index = _caretIndex - 1;
            var text = _target.Text;

            var chKind = StringUtil.GetCharKind(text[index]);
            if (IsValidWordCharKind(chKind)) {
                var first = StringUtil.GetBackwardWordBound(text, index);
                return text.Substring(first, index - first + 1);
            } else {
                return null;
            }
        }

        // --- move ---
        public void MoveForwardChar() {
            if (CaretIndex < _target.Length - 1) {
                ClearSelection();
                ++CaretIndex;
            }
        }

        public void MoveBackwardChar() {
            if (CaretIndex > 0) {
                ClearSelection();
                --CaretIndex;
            }
        }

        public void MovePreviousLine() {
            ClearSelection();
            CaretIndex = _prevLineCharIndexProvider(CaretIndex);
        }

        public void MoveNextLine() {
            ClearSelection();
            CaretIndex = _nextLineCharIndexProvider(CaretIndex);
        }

        public void MovePreviousPage() {
            ClearSelection();
            CaretIndex = _prevPageCharIndexProvider(CaretIndex);
        }

        public void MoveNextPage() {
            ClearSelection();
            CaretIndex = _nextPageCharIndexProvider(CaretIndex);
        }

        public void MoveBeginningOfLine() {
            var curLineIndex = Target.GetLineIndex(CaretIndex);
            var lineStart = Target.GetLineStartCharIndex(curLineIndex);
            ClearSelection();
            CaretIndex = lineStart;
        }

        public void MoveEndOfLine() {
            var curLineIndex = Target.GetLineIndex(CaretIndex);
            var lineStart = Target.GetLineStartCharIndex(curLineIndex);
            var lineEnd = lineStart + Target.GetColumnCount(curLineIndex) - 1;
            ClearSelection();
            CaretIndex = lineEnd;
        }

        public void MoveBeginningOfText() {
            ClearSelection();
            CaretIndex = 0;
        }

        public void MoveEndOfText() {
            ClearSelection();
            CaretIndex = _target.Length - 1;
        }

        public void MoveNextWord() {
            ClearSelection();
            if (_caretIndex == _target.Length - 1) {
                return;
            }

            var text = _target.Text;
            var index = _caretIndex;
            while (index < _target.Length - 1) {
                var chKind = StringUtil.GetCharKind(text[index]);
                index = StringUtil.GetForwardWordBound(text, index) + 1;
                if (IsValidWordCharKind(chKind)) {
                    break;
                }
            }

            if (index > _target.Length - 1) {
                index = _target.Length - 1;
            }
            CaretIndex = index;
        }

        public void MovePreviousWord() {
            ClearSelection();
            if (_caretIndex == 0) {
                return;
            }

            var text = _target.Text;
            var index = _caretIndex;
            while (index > 0) {
                var first = StringUtil.GetBackwardWordBound(text, index);
                if (first == index) {
                    if (first - 1 < 0) {
                        index = 0;
                        break;
                    } else {
                        if (index != _caretIndex && first == StringUtil.GetForwardWordBound(text, first)) {
                            var chKind = StringUtil.GetCharKind(text[index]);
                            if (IsValidWordCharKind(chKind)) {
                                index = first;
                                break;
                            }
                        }
                        index = first - 1;
                    }
                    
                } else {
                    var chKind = StringUtil.GetCharKind(text[index]);
                    if (IsValidWordCharKind(chKind)) {
                        index = first;
                        break;
                    } else {
                        index = first - 1;
                    }
                }
            }

            if (index < 0) {
                index = 0;
            }
            CaretIndex = index;
        }


        // --- yank ---
        //public void YankSelectionContent(int charIndex) {
        //    //if (!Selection.IsEmpty) {
        //    //    if (Target.Move(Selection.Offset, Selection.Length, charIndex)) {
        //    //        if (charIndex < Selection.Offset) {
        //    //            Selection.Offset = charIndex;
        //    //            CaretPosition = Selection.Offset;
        //    //        } else {
        //    //            Selection.Offset = charIndex - Selection.Length;
        //    //            CaretPosition = Selection.Offset;
        //    //        }
        //    //    }
        //    //}
        //}


        // --- mark ---
        public void SetMark() {
            Mark = CaretIndex;
        }

        public void ClearMark() {
            Mark = null;
        }

        public void PopMark() {
            if (IsMarkSet) {
                ClearSelection();
                CaretIndex = Mark.Value;
                ClearMark();
            }
        }

        public void ExchangeCaretAndMark() {
            if (IsMarkSet) {
                ClearSelection();
                var old = CaretIndex;
                CaretIndex = Mark.Value;
                Mark = old;
            }
        }

        // --- select ---
        public void SelectRegion() {
            if (IsMarkSet) {
                Selection.Range = GetRegion();
            }
        }

        public void ClearSelection() {
            Selection.Clear();
        }

        public void SelectAll() {
            CaretIndex = _target.Length - 1;
            Selection.Range = new Range(0, _target.Length - 1);
        }

        public void SelectParagraph() {
            var block = _target.GetBlockAt(CaretIndex);
            var range = _target.GetRange(block);
            if (range.End >= _target.Length - 1) {
                range = new Range(range.Offset, range.Length - 1);
            }
            CaretIndex = range.End + 1;
            Selection.Range = range;
        }

        public void SelectForwardChar() {
            if (CaretIndex < _target.Length - 1) {
                if (Selection.IsEmpty) {
                    Selection.Range = new Range(CaretIndex, 1);
                    ++CaretIndex;
                } else {
                    if (Selection.Offset == CaretIndex) {
                        Selection.Range = new Range(Selection.Offset + 1, Selection.Length - 1);
                        ++CaretIndex;
                    } else {
                        ++Selection.Length;
                        ++CaretIndex;
                    }
                }
            }
        }

        public void SelectBackwardChar() {
            if (CaretIndex > 0) {
                if (Selection.IsEmpty) {
                    Selection.Range = new Range(CaretIndex - 1, 1);
                    --CaretIndex;
                } else {
                    if (Selection.Offset == CaretIndex) {
                        Selection.Range = new Range(Selection.Offset - 1, Selection.Length + 1);
                        --CaretIndex;
                    } else {
                        --Selection.Length;
                        --CaretIndex;
                    }
                }
            }
        }

        public void SelectNextLine() {
            var oldCaretIndex = CaretIndex;
            CaretIndex = _nextLineCharIndexProvider(CaretIndex);
            if (CaretIndex != oldCaretIndex) {
                /// Selectionの設定
                if (Selection.IsEmpty) {
                    Selection.Range = new Range(oldCaretIndex, CaretIndex - oldCaretIndex);

                } else  {
                    /// offsetとoldCaretIndexが等しいということは上方向・backward方向への範囲選択中
                    if (Selection.Offset == oldCaretIndex) {
                        if (CaretIndex < Selection.Offset + Selection.Length - 1) {
                            /// forward・backward選択の入れ替わる場合
                            Selection.Range = new Range(
                                CaretIndex, Selection.Length - (CaretIndex - Selection.Offset)
                            );
                        } else {
                            var offset = Selection.Offset + Selection.Length;
                            Selection.Range = new Range(offset, CaretIndex - offset);
                        }
                    } else {
                        Selection.Range = new Range(Selection.Offset, CaretIndex - Selection.Offset);
                    }
                }
            }
        }

        public void SelectPreviousLine() {
            var oldCaretIndex = CaretIndex;
            CaretIndex = _prevLineCharIndexProvider(CaretIndex);
            if (CaretIndex != oldCaretIndex) {
                /// Selectionの設定
                if (Selection.IsEmpty) {
                    Selection.Range = new Range(CaretIndex, oldCaretIndex - CaretIndex);

                } else  {
                    /// offsetとoldCaretIndexが等しいということは上方向・backward方向への範囲選択中
                    if (Selection.Offset == oldCaretIndex) {
                        Selection.Range = new Range(CaretIndex, Selection.Length + (oldCaretIndex - CaretIndex));

                    } else {
                        /// forward・backward選択の入れ替わりかどうか
                        if (CaretIndex > Selection.Offset) {
                            Selection.Range = new Range(Selection.Offset, CaretIndex - Selection.Offset);
                        } else {
                            Selection.Range = new Range(CaretIndex, Selection.Offset - CaretIndex);
                        }
                    }
                }
            }
        }

        // --- search ---
        public bool SearchForwardFirst(string s) {
            if (string.IsNullOrEmpty(s)) {
                return false;
            }
            var baseIndex = _selection.IsEmpty? _caretIndex: _selection.Offset;
            if (baseIndex >= _target.Length - 1) {
                return false;
            }
            var text = _target.Text;
            var index = text.IndexOf(s, baseIndex, StringComparison.OrdinalIgnoreCase);
            if (index > -1) {
                var len = s.Length;
                CaretIndex = index + len;
                _selection.Range = new Range(index, len);
                return true;
            } else {
                _selection.Clear();
                return false;
            }
        }

        public bool SearchForwardNext(string s) {
            if (string.IsNullOrEmpty(s)) {
                return false;
            }
            var baseIndex = _selection.IsEmpty? _caretIndex: _selection.Range.End + 1;
            if (baseIndex >= _target.Length - 1) {
                return false;
            }
            var text = _target.Text;
            var index = text.IndexOf(s, baseIndex, StringComparison.OrdinalIgnoreCase);
            if (index > -1) {
                var len = s.Length;
                CaretIndex = index + len;
                _selection.Range = new Range(index, len);
                return true;
            } else {
                return false;
            }
        }

        public bool SearchBackwardFirst(string s) {
            if (string.IsNullOrEmpty(s)) {
                return false;
            }
            var baseIndex = _selection.IsEmpty? _caretIndex - 1: _selection.Range.End;
            if (baseIndex <= 0) {
                return false;
            }
            var text = _target.Text;
            var index = text.LastIndexOf(s, baseIndex, StringComparison.OrdinalIgnoreCase);
            if (index > -1) {
                CaretIndex = index;
                _selection.Range = new Range(index, s.Length);
                return true;
            } else {
                _selection.Clear();
                return false;
            }
        }

        public bool SearchBackwardNext(string s) {
            if (string.IsNullOrEmpty(s)) {
                return false;
            }
            var baseIndex = _selection.IsEmpty? _caretIndex - 1: _selection.Offset - 1;
            if (baseIndex < 0) {
                return false;
            }
            var text = _target.Text;
            var index = text.LastIndexOf(s, baseIndex, StringComparison.OrdinalIgnoreCase);
            if (index > -1) {
                CaretIndex = index;
                _selection.Range = new Range(index, s.Length);
                return true;
            } else {
                return false;
            }
        }

        // --- clipboard ---
        //public void Copy() {
        //    if (!_selection.IsEmpty) {
        //        // todo: PlainTextだと範囲が変わるのでそのままではSubstring()できない
        //        //var text = _target.ToPlainText().Substring(_selection.Offset, _selection.Length);
        //        var text = _target.Text.Substring(_selection.Offset, _selection.Length);
        //        text = text.Replace("\r", "\n");
        //        text = text.Replace("\n", Environment.NewLine);
        //        ClipboardUtil.SetText(text);
        //    }
        //}

        public void Copy() {
            if (!_selection.IsEmpty) {
                var cmd = new CopyCommand(_target, _selection.Range);
                _executor.Execute(cmd);
            }
        }

        public void Cut() {
            Copy();
            RemoveBackward();
        }

        public void CopyRegion() {
            SelectRegion();
            Copy();
        }

        public void CutRegion() {
            CopyRegion();
            RemoveBackward();
        }

        public void KillWord() {
            if (_caretIndex == _target.Length - 1) {
                return;
            }

            int col, lineOffset;
            var line = _target.GetLineSegmentAt(_caretIndex, out col, out lineOffset);
            var range = StringUtil.GetWordRange(line.Text, col);
            if (!range.IsEmpty) {
                Selection.Range = Range.FromStartAndEnd(_caretIndex, range.End + lineOffset);
                Cut();
            }
        }

        public void KillLineFirst() {
            KillLineBase(true);
        }

        public void KillLine() {
            KillLineBase(false);
        }

        public void InsertText(string text, bool inBlock) {
            _nextInputInline = null;

            if (StringUtil.IsNullOrWhitespace(text)) {
                return;
            }

            var cmds = new CompositeCommand();
            var hasLastLineBreak = text[text.Length - 1] == '\n' || text[text.Length - 1] == '\r';
            var lines = StyledTextUtil.SplitWithBlockBreak(text);
            if (lines.Any()) {
                if (!_selection.IsEmpty) {
                    /// 範囲選択されているならその範囲を削除
                    var index = _selection.Offset;
                    cmds.Children.Add(new RemoveCommand(_target, _selection.Offset, _selection.Length));
                    CaretIndex = index;
                    ClearSelection();
                }

                var firstLine = lines[0];
                var lastLine = lines[lines.Length - 1];
                var caretIndex = _caretIndex;
                var urlRanges = new List<Tuple<Range, string>>(); /// for link
                foreach (var line in lines) {
                    if (!string.IsNullOrEmpty(line)) {
                        _nextInputInline = null;
                        var sanitized = line.Replace("\t", "    ");

                        /// link取得
                        var ranges = StringUtil.GetUrlRanges(sanitized);
                        if (ranges.Any()) {
                            foreach (var r in ranges) {
                                var tuple = Tuple.Create(
                                    new Range(r.Offset + caretIndex, r.Length),
                                    sanitized.Substring(r.Offset, r.Length)
                                );
                                urlRanges.Add(tuple);
                            }
                        }

                        if (line == firstLine) {
                            cmds.Children.Add(new InsertStringCommand(_target, caretIndex, sanitized));
                        } else {
                            cmds.Children.Add(
                                new InsertInlineCommand(_target, caretIndex, new Run(sanitized))
                            );
                        }
                        caretIndex += sanitized.Length;
                    }
                    if (line != lastLine || hasLastLineBreak) {
                        if (inBlock) {
                            cmds.Children.Add(new InsertLineBreakCommand(_target, caretIndex));
                        } else {
                            cmds.Children.Add(new InsertBlockBreakCommand(_target, caretIndex));
                        }
                        ++caretIndex;
                    }
                }
                
                /// link設定
                foreach (var urlRange in urlRanges) {
                    var link = new SetLinkCommand(_target, urlRange.Item1, new Link(urlRange.Item2));
                    cmds.Children.Add(link);
                }

                _executor.Execute(cmds);
                CaretIndex = caretIndex;
            }
        }

        public void InsertBlocksAndInlines(IEnumerable<Flow> blocksAndInlines) {
            _nextInputInline = null;
            var cmd = new InsertBlocksAndInlinesCommand(_target, _caretIndex, blocksAndInlines);
            _executor.Execute(cmd);
            if (cmd != null) {
                ReflectExecutedRange(cmd);
            }
        }


        public void Paste() {
            _nextInputInline = null;

            var cmds = new CompositeCommand();
            if (!_selection.IsEmpty) {
                /// 範囲選択されているならその範囲を削除
                var index = _selection.Offset;
                cmds.Children.Add(new RemoveCommand(_target, _selection.Offset, _selection.Length));
                CaretIndex = index;
                ClearSelection();
            }
            var paste = new PasteCommand(_target, _caretIndex);
            cmds.Children.Add(paste);
            _executor.Execute(cmds);
            if (paste != null) {
                ReflectExecutedRange(paste);
            }
        }

        public void PasteText(bool inBlock) {
            _nextInputInline = null;

            if (Clipboard.ContainsText()) {
                var data = Clipboard.GetText();
                InsertText(data, inBlock);
            }
        }

        public void PasteLastLine() {
            _nextInputInline = null;

            if (!Clipboard.ContainsText()) {
                return;
            }

            var data = Clipboard.GetText();

            if (data != null) {
                var strs = StyledTextUtil.SplitWithBlockBreak(data);
                if (strs.Any()) {
                    var last = strs.Last();
                    if (!string.IsNullOrEmpty(last)) {
                        Insert(last);
                    }
                }
            }
        }

        // --- undo redo ---
        public void Undo() {
            _nextInputInline = null;

            if (_executor.CanUndo) {
                var undo = _executor.Undo();
                var rangeCmd = undo as IRangeCommand;
                if (rangeCmd == null) {
                    var cmpcmd = undo as CompositeCommand;
                    if (cmpcmd != null && cmpcmd.Children.Any()) {
                        rangeCmd = cmpcmd.Children.First() as IRangeCommand;
                    }
                }
                if (rangeCmd != null) {
                    ReflectUndoneRange(rangeCmd);
                }
            }
        }

        public void Redo() {
            _nextInputInline = null;

            if (_executor.CanRedo) {
                var redo = _executor.Redo();
                var rangeCmd = redo as IRangeCommand;
                if (rangeCmd == null) {
                    var cmpcmd = redo as CompositeCommand;
                    if (cmpcmd != null && cmpcmd.Children.Any()) {
                        rangeCmd = cmpcmd.Children.Last() as IRangeCommand;
                    }
                }
                if (rangeCmd != null) {
                    ReflectExecutedRange(rangeCmd);
                }
            }
        }

        // --- insert ---
        public void Insert(string s) {
            if (!_selection.IsEmpty) {
                var index = _selection.Offset;
                var cmd = new RemoveCommand(_target, _selection.Offset, _selection.Length);
                _executor.Execute(cmd);
                ReflectExecutedRange(cmd);
            }

            if (_nextInputInline != null) {
                _nextInputInline.Text = s;
                var cmd = new InsertInlineCommand(_target, _caretIndex, _nextInputInline);
                _executor.Execute(cmd);
                ReflectExecutedRange(cmd);
                _nextInputInline = null;

            } else {
                var cmd = new InsertStringCommand(_target, _caretIndex, s);
                _executor.Execute(cmd);
                ReflectExecutedRange(cmd);
            }
        }

        public void InsertLink(string s, Link link) {
            _nextInputInline = null;

            if (!_selection.IsEmpty) {
                var index = _selection.Offset;
                var remove = new RemoveCommand(_target, _selection.Offset, _selection.Length);
                _executor.Execute(remove);
                ReflectExecutedRange(remove);
            }

            var inserting = default(Run);
            if (_nextInputInline != null) {
                inserting = _nextInputInline;
                inserting.Text = s;
                inserting.Link = link;
                _nextInputInline = null;
            } else {
                inserting = new Run(s, link);
            }

            var insert = new InsertInlineCommand(_target, _caretIndex, inserting);
            _executor.Execute(insert);
            ReflectExecutedRange(insert);
        }

        public void InsertLineBreak() {
            _nextInputInline = null;

            if (!_selection.IsEmpty) {
                var index = _selection.Offset;
                var cmd = new RemoveCommand(_target, _selection.Offset, _selection.Length);
                _executor.Execute(cmd);
                ReflectExecutedRange(cmd);
            }

            var insert = new InsertLineBreakCommand(_target, _caretIndex);
            _executor.Execute(insert);
            ReflectExecutedRange(insert);
        }

        public void OpenLineBreak() {
            _nextInputInline = null;

            if (!_selection.IsEmpty) {
                var index = _selection.Offset;
                var remove = new RemoveCommand(_target, _selection.Offset, _selection.Length);
                _executor.Execute(remove);
                ReflectExecutedRange(remove);
            }

            var insert = new InsertLineBreakCommand(_target, _caretIndex, true);
            _executor.Execute(insert);
            ReflectExecutedRange(insert);
        }

        public void InsertBlockBreak() {
            _nextInputInline = null;

            if (!_selection.IsEmpty) {
                var index = _selection.Offset;
                var remove = new RemoveCommand(_target, _selection.Offset, _selection.Length);
                _executor.Execute(remove);
                ReflectExecutedRange(remove);
            }

            var insert = new InsertBlockBreakCommand(_target, _caretIndex);
            _executor.Execute(insert);
            ReflectExecutedRange(insert);
        }

        // --- remove ---
        public void RemoveForward() {
            _nextInputInline = null;

            if (_selection.IsEmpty) {
                /// 最後のBlockBreakが消せてはいけない
                if (_caretIndex < _target.Length - 1) {
                    var remove = new RemoveCommand(_target, _caretIndex, 1);
                    _executor.Execute(remove);
                    ReflectExecutedRange(remove);
                }
            } else {
                var offset = _selection.Offset;
                var remove = new RemoveCommand(_target, _selection.Offset, _selection.Length);
                _executor.Execute(remove);
                ReflectExecutedRange(remove);
            }
        }

        public void RemoveBackward() {
            _nextInputInline = null;

            if (_selection.IsEmpty) {
                if (_caretIndex > 0) {
                    var remove = new RemoveCommand(_target, _caretIndex - 1, 1);
                    _executor.Execute(remove);
                    ReflectExecutedRange(remove);
                }
            } else {
                var offset = _selection.Offset;
                var remove = new RemoveCommand(_target, _selection.Offset, _selection.Length);
                _executor.Execute(remove);
                ReflectExecutedRange(remove);
            }
        }

        // --- font style ---
        /// <summary>
        /// Selectionの範囲の文字列のParagraphKindを設定する．
        /// </summary>
        public void SetParagraphKind(ParagraphKind paragraphKind) {
            if (_selection.IsEmpty) {
                var target = _target.GetBlockAt(_caretIndex) as Paragraph;
                if (target != null) {
                    _executor.Execute(new SetParagraphKindOfParagraphCommand(target, paragraphKind));
                    _nextInputInline = new Run();
                    _nextInputInline.Font = _target.GetDefaultFont(paragraphKind);
                }

            } else {
                var targets = GetParagraphsInSelection();

                var cmd = new CompositeCommand();
                foreach (var target in targets) {
                    cmd.Chain(new SetParagraphKindOfParagraphCommand(target, paragraphKind));
                }
                if (cmd.Children.Count > 0) {
                    _executor.Execute(cmd);
                }
            }
        }

        /// <summary>
        /// Selectionの範囲の文字列のFontを設定する．
        /// </summary>
        public void SetFont(Func<Flow, FontDescription> fontProvider) {
            if (_selection.IsEmpty) {
                if (_nextInputInline == null) {
                    var inline = default(Inline);
                    if (_target.IsLineHead(_caretIndex)) {
                        inline = _target.GetInlineAt(_caretIndex);
                    } else {
                        inline = _target.GetInlineAt(_caretIndex - 1);
                    }

                    if (inline.IsLineEndCharacter) {
                        _nextInputInline = new Run();
                        _nextInputInline.Font = inline.Font;
                    } else {
                        _nextInputInline = inline.Clone() as Run;
                        _nextInputInline.Font = inline.Font;
                    }
                }
                _nextInputInline.Font = fontProvider(_nextInputInline);

            } else {
                var cmd = new SetFontCommand(_target, _selection.Range, fontProvider);
                _executor.Execute(cmd);
                ReflectExecutedRange(cmd);
            }
        }

        /// <summary>
        /// Selectionの範囲の文字列のColorを設定する．
        /// </summary>
        public void SetColor(Color color) {
            if (_selection.IsEmpty) {
                if (_nextInputInline == null) {
                    var inline = _target.GetInlineAt(_caretIndex - 1);
                    if (inline.IsLineEndCharacter) {
                        _nextInputInline = new Run();
                    } else {
                        _nextInputInline = inline.Clone() as Run;
                    }
                }
                _nextInputInline.Color = color;

            } else {
                var cmd = new ApplyCommandCommand(
                    _target,
                    _selection.Range,
                    (flow) => new SetColorOfFlowCommand(flow, color)
                );
                _executor.Execute(cmd);
                ReflectExecutedRange(cmd);
            }
        }

        public void SetHorizontalAlignment(HorizontalAlignment hAlign) {
            var block = _target.GetBlockAt(_caretIndex);
            var cmd = new SetHorizontalAlignmentOfBlock(block, hAlign);
            _executor.Execute(cmd);
        }

        public void SetVerticalAlignment(VerticalAlignment vAlign) {
            var cmd = new SetVerticalAlignmentOfStyledTextCommand(_target, vAlign);
            _executor.Execute(cmd);
        }


        // --- list ---
        public void ToggleList(ListKind listKind) {
            ToggleListBase(listKind);
        }

        public void ToggleUnorderedList() {
            ToggleListBase(ListKind.Unordered);
        }

        public void ToggleCheckBoxList() {
            ToggleListBase(ListKind.CheckBox);
        }

        public void ToggleTriStateCheckBoxList() {
            ToggleListBase(ListKind.TriStateCheckBox);
        }

        public void ToggleStarList() {
            ToggleListBase(ListKind.Star);
        }

        public void ToggleLeftArrowList() {
            ToggleListBase(ListKind.LeftArrow);
        }

        public void ToggleRightArrowList() {
            ToggleListBase(ListKind.RightArrow);
        }

        public void ToggleOrderedList() {
            ToggleListBase(ListKind.Ordered);
        }

        public void Indent() {
            if (_selection.IsEmpty) {
                var target = _target.GetBlockAt(_caretIndex) as Paragraph;
                var cmd = new IndentParagraphCommand(target);
                _executor.Execute(cmd);
            } else {
                var targets = GetParagraphsInSelection();
                var cmds = new CompositeCommand();
                foreach (var target in targets) {
                    var cmd = new IndentParagraphCommand(target);
                    if (cmd.CanExecute) {
                        cmds.Chain(cmd);
                    }
                }
                if (cmds.Children.Count > 0) {
                    _executor.Execute(cmds);
                }
            }
        }

        public void Outdent() {
            if (_selection.IsEmpty) {
                var target = _target.GetBlockAt(_caretIndex) as Paragraph;
                var cmd = new IndentParagraphCommand(target, -1);
                _executor.Execute(cmd);
            } else {
                var targets = GetParagraphsInSelection();
                var cmds = new CompositeCommand();
                foreach (var target in targets) {
                    var cmd = new IndentParagraphCommand(target, -1);
                    if (cmd.CanExecute) {
                        cmds.Chain(cmd);
                    }
                }
                if (cmds.Children.Count > 0) {
                    _executor.Execute(cmds);
                }
            }
        }

        public void ChangeToNextListState() {
            if (_selection.IsEmpty) {
                var para = _target.GetBlockAt(_caretIndex) as Paragraph;
                if (para.ListKind == ListKind.CheckBox || para.ListKind == ListKind.TriStateCheckBox) {
                    var cmd = new SetParagraphPropertiesCommand(
                        para,
                        para.Padding,
                        para.LineSpace,
                        para.HorizontalAlignment,
                        para.ParagraphKind,
                        para.ListKind,
                        para.ListLevel,
                        para.GetNextListState()
                    );
                    _executor.Execute(cmd);
                }
            } else {
                var targets = GetParagraphsInSelection();
                var cmds = new CompositeCommand();
                foreach (var para in targets) {
                    if (para.ListKind == ListKind.CheckBox || para.ListKind == ListKind.TriStateCheckBox) {
                        var cmd = new SetParagraphPropertiesCommand(
                            para,
                            para.Padding,
                            para.LineSpace,
                            para.HorizontalAlignment,
                            para.ParagraphKind,
                            para.ListKind,
                            para.ListLevel,
                            para.GetNextListState()
                        );
                        cmds.Chain(cmd);
                    }
                }
                if (cmds.Children.Count > 0) {
                    _executor.Execute(cmds);
                }
            }
        }

        // --- link ---
        public void SetLink(string uri, string relationship) {
            var link = new Link(uri, relationship);
            if (_selection.IsEmpty) {
                var run = _target.GetInlineAt(_caretIndex) as Run;
                if (run != null) {
                    var cmd = new SetLinkOfRunCommand(run, link);
                    _executor.Execute(cmd);
                }

            } else {

                var cmd = new SetLinkCommand(_target, _selection.Range, link);
                _executor.Execute(cmd);
            }
        }

        public void UnsetLink() {
            if (_selection.IsEmpty) {
                var run = _target.GetInlineAt(_caretIndex) as Run;
                if (run != null) {
                    var cmd = new SetLinkOfRunCommand(run, null);
                    _executor.Execute(cmd);
                }
            } else {
                var cmd = new SetLinkCommand(_target, _selection.Range, null);
                _executor.Execute(cmd);
            }
        }


        // --- util ---
        public void SetRunText(Run run, string text) {
            var cmd = new SetTextOfFlowCommand(run, text);
            _executor.Execute(cmd);
            CaretIndex = CaretIndex;
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected void OnCaretMoved(CaretMovedEventArgs e) {
            var handler = CaretMoved;
            if (handler != null) {
                CaretMoved(this, e);
            }
        }

        protected void OnMarkSet() {
            var handler = MarkSet;
            if (handler != null) {
                MarkSet(this, EventArgs.Empty);
            }
        }

        // ------------------------------
        // private
        // ------------------------------
        private IEnumerable<Paragraph> GetParagraphsInSelection() {
            var first = _target.GetBlockAt(_selection.Offset);
            var last = _target.GetBlockAt(_selection.Range.End);

            /// 範囲内のParagraphを取得
            var ret = new List<Paragraph>();
            var isTarget = false;
            foreach (var block in _target.Blocks) {
                if (block == first) {
                    isTarget = true;
                }

                if (isTarget) {
                    var para = block as Paragraph;
                    if (para != null) {
                        ret.Add(para);
                    }
                }

                if (block == last) {
                    isTarget = false;
                }
            }

            return ret;
        }

        private void ToggleListBase(ListKind listKind) {
            if (_selection.IsEmpty) {
                var target = _target.GetBlockAt(_caretIndex) as Paragraph;
                if (target != null) {
                    if (target.ListKind == listKind) {
                        /// listからnoneに戻す
                        _executor.Execute(new SetListKindOfParagraphCommand(target, ListKind.None));

                    } else {
                        /// listにする
                        _executor.Execute(new SetListKindOfParagraphCommand(target, listKind));
                    }
                }

            } else {
                var targets = GetParagraphsInSelection();

                /// すべてのParagraphがlistならlist解除，そうでなければすべてlistにする
                var cmd = new CompositeCommand();
                var cancel = !targets.Any(target => target.ListKind != listKind);
                foreach (var target in targets) {
                    if (cancel) {
                        if (target.ListKind != ListKind.None) {
                            cmd.Chain(new SetListKindOfParagraphCommand(target, ListKind.None));
                        }
                    } else {
                        if (target.ListKind != listKind) {
                            cmd.Chain(new SetListKindOfParagraphCommand(target, listKind));
                        }
                    }
                }
                if (cmd.Children.Count > 0) {
                    _executor.Execute(cmd);
                }
            }
        }

        private void KillLineBase(bool isFirst) {
            if (_caretIndex == _target.Length - 1) {
                return;
            }

            int col, lineSegOffset;
            var line = _target.GetLineSegmentAt(_caretIndex, out col, out lineSegOffset);
            if (col == line.Length - 1) {
                if (isFirst) {
                    Mkamo.Common.Forms.Clipboard.ClipboardUtil.SetText(Environment.NewLine);
                } else {
                    var text = Clipboard.GetText();
                    Mkamo.Common.Forms.Clipboard.ClipboardUtil.SetText(text + Environment.NewLine);
                }
                RemoveForward();
            } else {
                var len = line.Length - col - 1;
                if (isFirst) {
                    Mkamo.Common.Forms.Clipboard.ClipboardUtil.SetText(line.Text.Substring(col, len));
                } else {
                    var text = Clipboard.GetText();
                    Mkamo.Common.Forms.Clipboard.ClipboardUtil.SetText(text + line.Text.Substring(col, len));
                }
                var remove = new RemoveCommand(_target, _caretIndex, len);
                _executor.Execute(remove);
                ReflectExecutedRange(remove);
            }
        }

        private void ReflectExecutedRange(IRangeCommand command) {
            var range = command.ExecutedRange;
            CaretIndex = range.Offset;
            if (range.IsEmpty) {
                ClearSelection();
            } else {
                _selection.Range = range;
            }
        }

        private void ReflectUndoneRange(IRangeCommand command) {
            var range = command.UndoneRange;
            CaretIndex = range.Offset;
            if (range.IsEmpty) {
                ClearSelection();
            } else {
                _selection.Range = range;
            }
        }

        private bool IsValidWordCharKind(CharKind chKind) {
            return !(chKind == CharKind.Control || chKind == CharKind.Whitespace || chKind == CharKind.Punctuation || chKind == CharKind.Others);
        }


    }

}
