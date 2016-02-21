/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using Mkamo.Common.Command;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Common.Forms.Input;
using Mkamo.StyledText.Core;
using Mkamo.StyledText.Commands;
using Mkamo.StyledText.Util;
using Mkamo.Common.Externalize;
using Mkamo.Common.String;
using Mkamo.Editor.Core;
using Mkamo.Editor.Focuses;
using Mkamo.Figure.Core;
using Mkamo.Figure.Figures;
using Mkamo.Common.Diagnostics;
using Mkamo.Common.DataType;
using DataType = Mkamo.Common.DataType;
using Mkamo.Editor.Requests;
using Mkamo.Editor.Utils;
using Mkamo.Editor.Controllers;
using Mkamo.Common.Forms.Drawing;
using Mkamo.Common.Core;
using Mkamo.Common.Win32.Gdi32;
using Mkamo.Control.Core;
using Mkamo.Editor.Internal.Controls;

namespace Mkamo.Editor.Focuses {
    using ICommandExecutor = Mkamo.Common.Command.ICommandExecutor;
    using StyledText = Mkamo.StyledText.Core.StyledText;
    using HorizontalAlignment = Mkamo.Common.DataType.HorizontalAlignment;
    using VerticalAlignment = Mkamo.Common.DataType.VerticalAlignment;

    public partial class StyledTextFocus: AbstractFocus {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(global::System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // ========================================
        // field
        // ========================================
        private StyledTextFocusFigure _figure;
        private StyledTextReferer _referer;

        private bool _isConsiderImeWindowSize;
        private bool _inImeComposition;

        /// <summary>
        /// 次にCaretのY値を変更したときに望まれるX値．
        /// 以前にCaretのX値を変更したときの値が入っている．
        /// </summary>
        private int _expectedCaretPosX;

        private Range _dragStartCharRange;
        private int _dragEndCharIndex;

        //private bool _isEmacsEdit;

        //private bool _isSingleLine;

        /// <summary>
        /// Relocate()時にテキスト矩形だけでなくHostのBoundsも考慮するかどうか
        /// </summary>
        private bool _isConsiderHostBounds;

        private Lazy<StyledTextFocusContextMenuProvider> _contextMenuProvider;

        // ========================================
        // constructor
        // ========================================
        /// <summary>
        /// initializerはBegin()時にmodelの内容でStyledTextFocusを初期化するための処理．
        /// committerはCommit()時にStyledTextFocusの内容をmodelに反映させる処理．
        /// </summary>
        public StyledTextFocus(): this(false) {
        }

        public StyledTextFocus(bool isEmacsEdit) {
            _figure = new StyledTextFocusFigure(this) {
                Background = new SolidBrushDescription(Color.Ivory),
                Foreground = Color.Gray,
                AutoSizeKinds = AutoSizeKinds.FitBoth,
                CursorProvider = GetMouseCursor,
            };

            _contextMenuProvider = new Lazy<StyledTextFocusContextMenuProvider>(
                () => new StyledTextFocusContextMenuProvider(this)
            );

            _referer = new StyledTextReferer(
                _figure.StyledText,
                GetPreviousLineIndex, GetNextLineIndex, GetPreviousPageIndex, GetNextPageIndex
            );
            _referer.CaretMoved += HandleRefererCaretMove;
            _referer.Selection.SelectionChanged += HandleRefererSelectionChanged;

            //_isSingleLine = false;

            _isConsiderImeWindowSize = false;
            _inImeComposition = false;

            _isConsiderHostBounds = false;

            //_isEmacsEdit = isEmacsEdit;
            //_KeyMap.ValueCreated += (s, ev) => {
            //    if (_isSingleLine) {
            //        if (_isEmacsEdit) {
            //            ResetEmacsSingleLineKeyMap();
            //        } else {
            //            ResetDefaultSingleLineKeyMap();
            //        }
            //    } else {
            //        if (_isEmacsEdit) {
            //            ResetEmacsKeyMap();
            //        } else {
            //            ResetDefaultKeyMap();
            //        }
            //    }
            //};
        }

        private Cursor GetMouseCursor(MouseEventArgs e) {
            if (Host.IsFocused) {
                if (_figure.IsInBullet(e.Location, ListKind.CheckBox | ListKind.TriStateCheckBox)) {
                    return Cursors.Default;
                }

                if (_figure.IsInSelection(e.Location)) {
                    return Cursors.Default;
                }
            }
            return Cursors.IBeam;
        }

        // ========================================
        // event
        // ========================================
        public event EventHandler<CaretMovedEventArgs> CaretMoved {
            add { _referer.CaretMoved += value; }
            remove { _referer.CaretMoved -= value; }
        }

        public event EventHandler<EventArgs> MarkSet {
            add { _referer.MarkSet += value; }
            remove { _referer.MarkSet -= value; }
        }

        public event EventHandler<EventArgs> SelectionChanged {
            add { _referer.Selection.SelectionChanged += value; }
            remove { _referer.Selection.SelectionChanged -= value; }
        }

        public event EventHandler<Mkamo.Editor.Core.LinkClickedEventArgs> LinkClicked;

        // ========================================
        // property
        // ========================================
        public override object Value {
            get { return StyledText.CloneDeeply(); }
            set { StyledText = value as StyledText; }
        }

        public override bool IsModified {
            get { return _referer.IsModified; }
        }

        public override INode Figure {
            get { return _figure; }
        }

        public bool IsBegun {
            get { return _figure.StyledText != null; }
        }

        public StyledTextReferer StyledTextReferer {
            get { return _referer; }
        }

        public StyledTextSelection Selection {
            get { return _referer.Selection; }
        }

        public Run NextInputRun {
            get { return _referer.NextInputRun; }
            set { _referer.NextInputRun = value; }
        }

        //public bool IsSingleLine {
        //    get { return _isSingleLine; }
        //    set {
        //        if (value == _isSingleLine) {
        //            return;
        //        }
        //        _isSingleLine = value;
        //        //if (_isSingleLine) {
        //        //    _figure.AutoSizeKinds = AutoSizeKinds.FitWidth;
        //        //    if (_isEmacsEdit) {
        //        //        ResetEmacsSingleLineKeyMap();
        //        //    } else {
        //        //        ResetDefaultSingleLineKeyMap();
        //        //    }
        //        //} else {
        //        //    _figure.AutoSizeKinds = AutoSizeKinds.FitBoth;
        //        //    if (_isEmacsEdit) {
        //        //        ResetEmacsKeyMap();
        //        //    } else {
        //        //        ResetDefaultKeyMap();
        //        //    }
        //        //}
        //    }
        //}

        public bool IsConsiderImeWindowSize {
            get { return _isConsiderImeWindowSize; }
            set { _isConsiderImeWindowSize = value; }
        }

        public bool IsConsiderHostBounds {
            get { return _isConsiderHostBounds; }
            set { _isConsiderHostBounds = value; }
        }

        public string SingleLine {
            get {
                var strs = _referer.Target.Lines;
                return strs.Any()? strs[0]: "";
            }
            set {
                var node = Figure as INode;
                var para = new Paragraph();
                para.InsertBefore(new Run(value));
                node.StyledText = new StyledText(para);
            }
        }

        public StyledTextReferer Referer {
            get { return _referer; }
        }

        public StyledText StyledText {
            get { return _referer.Target; }
            set { _referer.Target = value; }
        }

        public ICommandExecutor CommandExecutor {
            get { return _referer.CommandExecutor; }
        }

        public bool CanCut {
            get { return !_referer.Selection.IsEmpty; }
        }

        public bool CanCopy {
            get { return !_referer.Selection.IsEmpty; }
        }

        public bool CanPaste {
            /// StyledTextを含んでいるときはかならずTextも含んでいるのでこれでよい
            get { return Clipboard.ContainsText(); }
        }

        //public bool IsEmacsEdit {
        //    get { return _isEmacsEdit; }
        //    set {
        //        if (value == _isEmacsEdit) {
        //            return;
        //        }
        //        _isEmacsEdit = value;
        //        if (_isSingleLine) {
        //            if (_isEmacsEdit) {
        //                ResetEmacsSingleLineKeyMap();
        //            } else {
        //                ResetDefaultSingleLineKeyMap();
        //            }
        //        } else {
        //            if (_isEmacsEdit) {
        //                ResetEmacsKeyMap();
        //            } else {
        //                ResetDefaultKeyMap();
        //            }
        //        }
        //    }
        //}

        public bool IsCurrentLineBackgroundEnable {
            get { return _figure.IsCurrentLineBackgroundEnable; }
            set { _figure.IsCurrentLineBackgroundEnable = value; }
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected Caret _Caret {
            get { return Host == null || Host.Root == null? null: Host.Site.EditorCanvas.Caret; }
        }

        // ========================================
        // method
        // ========================================
        public override void Install(IEditor host) {
            base.Install(host);
        }

        public override void Relocate(IFigure hostFigure) {
            if (Relocator != null) {
                Relocator(this, Figure, hostFigure);

            } else {
                var rect = default(Rectangle);
                var hostNode = hostFigure as INode;
                if (
                    hostNode != null &&
                    hostNode.Root != null &&
                    hostNode.Root.Canvas != null &&
                    !hostNode.Root.Canvas.IsDisposed &&
                    hostNode.StyledText != null
                ) {
                    var textBounds = hostNode.GetStyledTextBoundsFor(hostNode.ClientArea);
                    if (textBounds.IsEmpty) {
                        rect = hostNode.Bounds;
                    } else {
                        rect = hostNode.Padding.GetBounds(textBounds);
                    }
                    if (_isConsiderHostBounds) {
                        rect = Rectangle.Union(rect, hostFigure.Bounds);
                    }
                } else {
                    rect = hostFigure.Bounds;
                }

                _figure.MinSize = rect.Size;
                _figure.Bounds = rect;
                _figure.AdjustSize();
            }
        }


        public override void Begin(Point? location) {
            var hostNode = Host.Figure as INode;
            Contract.Requires(hostNode != null);

            /// set up figure
            _figure.StyledText = _referer.Target;

            /// set up appearance
            Relocate(hostNode);

            if (location != null) {
                /// RootFigureがないとArgumentNullException
                _referer.CaretIndex = _figure.GetCharIndexAt(location.Value);
                RecordExpectedCaretPosX();
            }

            /// IME ON時の_figureのサイズ調整
            Host.Site.EditorCanvas.ImeStartComposition += HandleEditorCanvasImeStartComposition;
            Host.Site.EditorCanvas.ImeEndComposition += HandleEditorCanvasImeEndComposition;
            Host.Site.EditorCanvas.ImeComposition += HandleEditorCanvasImeComposition;

            /// show caret
            RefreshCaret();
            _Caret.Show();
        }

        public override bool Commit() {
            Host.Site.EditorCanvas.ImeComposition -= HandleEditorCanvasImeComposition;
            Host.Site.EditorCanvas.ImeStartComposition -= HandleEditorCanvasImeStartComposition;
            Host.Site.EditorCanvas.ImeEndComposition -= HandleEditorCanvasImeEndComposition;

            _Caret.Hide();
            _figure.StyledText = null;
            _referer.CommandExecutor.Clear();
            return true;
        }

        public override void Rollback() {
            Host.Site.EditorCanvas.ImeComposition -= HandleEditorCanvasImeComposition;
            Host.Site.EditorCanvas.ImeStartComposition -= HandleEditorCanvasImeStartComposition;
            Host.Site.EditorCanvas.ImeEndComposition -= HandleEditorCanvasImeEndComposition;

            _Caret.Hide();
            _figure.StyledText = null;
            _referer.CommandExecutor.Clear();
        }

        public override IContextMenuProvider GetContextMenuProvider() {
            return _contextMenuProvider.Value;
        }

        // === StyledTextFocus ==========
        // --- info ---
        public Inline GetInlineAtCaretIndex() {
            return _referer.Target.GetInlineAt(_referer.CaretIndex);
        }

        public Block GetBlockAtCaretIndex() {
            return _referer.Target.GetBlockAt(_referer.CaretIndex);
        }

        public Inline GetInlineAtSelectionStart() {
            return _referer.Target.GetInlineAt(Selection.Offset);
        }

        public Block GetBlockAtSelectionStart() {
            return _referer.Target.GetBlockAt(Selection.Offset);
        }

        public Block GetBlockAtSelectionEnd() {
            return _referer.Target.GetBlockAt(Selection.Offset + Selection.Length + 1);
        }

        public Block GetBlockAtMark() {
            return _referer.IsMarkSet ? _referer.Target.GetBlockAt(_referer.Mark.Value) : null;
        }

        public override FontDescription GetNextInputFont() {
            return _referer.GetNextInputFont();
        }

        public Color GetNextInputColor() {
            return _referer.GetNextInputColor();
        }

        public ParagraphKind? GetParagraphKind() {
            return _referer.GetParagraphKind();
        }

        public string GetInputingWordPart() {
            return _referer.GetInputingWordPart();
        }


        // --- move ---
        public void MoveBackwardChar() {
            _referer.MoveBackwardChar();
            RecordExpectedCaretPosX();
        }

        public void MoveForwardChar() {
            _referer.MoveForwardChar();
            RecordExpectedCaretPosX();
        }

        public void MovePreviousLine() {
            _referer.MovePreviousLine();
        }

        public void MoveNextLine() {
            _referer.MoveNextLine();
        }

        public void MovePreviousPage() {
            _referer.MovePreviousPage();
        }

        public void MoveNextPage() {
            _referer.MoveNextPage();
        }

        public void MoveBeginningOfLine() {
            _referer.MoveBeginningOfLine();
            RecordExpectedCaretPosX();
        }

        public void MoveEndOfLine() {
            _referer.MoveEndOfLine();
            RecordExpectedCaretPosX();
        }

        public void MoveBeginningOfText() {
            _referer.MoveBeginningOfText();
            RecordExpectedCaretPosX();
        }

        public void MoveEndOfText() {
            _referer.MoveEndOfText();
            RecordExpectedCaretPosX();
        }

        public void MovePreviousWord() {
            _referer.MovePreviousWord();
            RecordExpectedCaretPosX();
        }

        public void MoveNextWord() {
            _referer.MoveNextWord();
            RecordExpectedCaretPosX();
        }

        // --- select ---
        public void SelectBackwardChar() {
            _referer.SelectBackwardChar();
            RecordExpectedCaretPosX();
        }

        public void SelectForwardChar() {
            _referer.SelectForwardChar();
            RecordExpectedCaretPosX();
        }

        public void SelectPreviousLine() {
            _referer.SelectPreviousLine();
        }

        public void SelectNextLine() {
            _referer.SelectNextLine();
        }

        public void SetMark() {
            _referer.SetMark();
        }

        public void PopMark() {
            _referer.PopMark();
            RecordExpectedCaretPosX();
        }

        public void ExchangeCaretAndMark() {
            _referer.ExchangeCaretAndMark();
            RecordExpectedCaretPosX();
        }

        public void SelectRegion() {
            _referer.SelectRegion();
        }

        public void SelectAll() {
            _referer.SelectAll();
        }

        public void SelectParagraph() {
            _referer.SelectParagraph();
        }

        // --- search ---
        public bool SearchForwardFirst(string s) {
            var ret = _referer.SearchForwardFirst(s);
            if (ret) {
                RecordExpectedCaretPosX();
            }
            return ret;
        }

        public bool SearchForwardNext(string s) {
            var ret = _referer.SearchForwardNext(s);
            if (ret) {
                RecordExpectedCaretPosX();
            }
            return ret;
        }

        public bool SearchBackwardFirst(string s) {
            var ret = _referer.SearchBackwardFirst(s);
            if (ret) {
                RecordExpectedCaretPosX();
            }
            return ret;
        }

        public bool SearchBackwardNext(string s) {
            var ret = _referer.SearchBackwardNext(s);
            if (ret) {
                RecordExpectedCaretPosX();
            }
            return ret;
        }

        // --- clipboard ---
        public void Copy() {
            _referer.Copy();
        }

        public void Cut() {
            if (_referer.Target.VerticalAlignment == VerticalAlignment.Top) {
                var range = _referer.Selection.Range;
                if (range.IsEmpty) {
                    return;
                }
                using (_figure.BeginUpdateStyledText(() => { }, GetCutRangePostAction(range))) {
                    _referer.CutRegion();
                }
            } else {
                using (_figure.BeginUpdateStyledText(DirtyForward)) {
                    _referer.Cut();
                }
            }
            AdjustFigureBounds();
            RefreshCaret();
            RecordExpectedCaretPosX();
        }

        public void CopyRegion() {
            _referer.CopyRegion();
        }

        public void CutRegion() {
            if (_referer.Target.VerticalAlignment == VerticalAlignment.Top) {
                var region = _referer.GetRegion();
                if (region.IsEmpty) {
                    return;
                }
                using (_figure.BeginUpdateStyledText(() => { }, GetCutRangePostAction(region))) {
                    _referer.CutRegion();
                }
            } else {
                using (_figure.BeginUpdateStyledText(DirtyRegion)) {
                    _referer.CutRegion();
                }
            }
            AdjustFigureBounds();
            RefreshCaret();
            RecordExpectedCaretPosX();
        }

        private Action GetCutRangePostAction(Range range) {
            var blocks = _referer.Target.GetBlocksInRange(range, true, true);
            var endInline = _referer.Target.GetInlineAt(range.End);
            if (endInline is BlockBreak) {
                var last = blocks.Last();
                 if (last.HasNextSibling) {
                     blocks = blocks.Concat(new [] { last.NextSibling as Block });
                 }
            }

            var blocksHeight = 0;
            blocksHeight = blocks.Sum(b => _figure.GetBlockSize(b).Height);


            return () => {
                var newBlock = GetBlockAtCaretIndex();
                _figure.DirtySizeAndVisLine(newBlock);
                _figure.DirtyBounds(newBlock);
                foreach (var b in blocks) {
                    _figure.DirtySizeAndVisLine(b);
                    _figure.DirtyBounds(b);
                }

                var newBlockSize = _figure.GetBlockSize(newBlock);
                var heightDelta = newBlockSize.Height - blocksHeight;
                if (newBlock.HasNextSibling) {
                    _figure.UpdateBoundsAfter(
                        newBlock.NextSibling as Block,
                        rect => new Rectangle(rect.Left, rect.Top + heightDelta, rect.Width, rect.Height),
                        rect => new Rectangle(rect.Left, rect.Top + heightDelta, rect.Width, rect.Height)
                    );
                }
            };
        }

        public void KillWord() {
            using (_figure.BeginUpdateStyledText(DirtyForward)) {
                _referer.KillWord();
            }
            AdjustFigureBounds();
            RefreshCaret();
            RecordExpectedCaretPosX();
        }

        public void KillLineFirst() {
            _referer.KillLineFirst();
            RecordExpectedCaretPosX();
            RefreshCaret();
            AdjustFigureBounds();
        }

        public void KillLine() {
            _referer.KillLine();
            RecordExpectedCaretPosX();
            RefreshCaret();
            AdjustFigureBounds();
        }
        
        // --- insert ---
        public void Insert(string s) {
            Insert(s, false);
        }

        public void Insert(string s, bool bulk) {
            if (!_referer.Selection.IsEmpty) {
                RemoveForward();
            }
            _referer.Insert(s);

            if (!bulk) {
                AdjustFigureBounds();
                RefreshCaret();
                RecordExpectedCaretPosX();
            }
        }

        public void InsertLink(string s, Link link) {
            _referer.InsertLink(s, link);
            AdjustFigureBounds();
            RefreshCaret();
            RecordExpectedCaretPosX();
        }

        public void InsertText(string text, bool inBlock) {
            using (_figure.DirtManager.BeginDirty()) {
                _Caret.Hide();

                using (_figure.BeginUpdateStyledText(DirtyForward)) {
                    _referer.InsertText(text, inBlock);
                }

                AdjustFigureBounds();
                RefreshCaret();
                RecordExpectedCaretPosX();
                _Caret.Show();
            }
        }

        public void InsertBlocksAndInlines(IEnumerable<Flow> blocksAndInlines) {
            using (_figure.DirtManager.BeginDirty()) {
                _Caret.Hide();

                using (_figure.BeginUpdateStyledText(DirtyForward)) {
                    _referer.InsertBlocksAndInlines(blocksAndInlines);
                }

                AdjustFigureBounds();
                RefreshCaret();
                RecordExpectedCaretPosX();
                _Caret.Show();
            }
        }

        public void InsertLineBreak() {
            using (_figure.BeginUpdateStyledText(DirtyForward)) {
                _referer.InsertLineBreak();
            }
            AdjustFigureBounds();
            RefreshCaret();
            RecordExpectedCaretPosX();
        }

        public void OpenLineBreak() {
            using (_figure.BeginUpdateStyledText(DirtyForward)) {
                _referer.OpenLineBreak();
            }
            AdjustFigureBounds();
            RefreshCaret();
            RecordExpectedCaretPosX();
        }

        public void InsertBlockBreak() {
            if ((_referer.Target.VerticalAlignment == VerticalAlignment.Top) && Selection.IsEmpty) {
                var post = GetInsertBlockBreakPostAction();
                using (_figure.BeginUpdateStyledText(() => {}, post)) {
                    _referer.InsertBlockBreak();
                }
            } else {
                using (_figure.BeginUpdateStyledText(DirtyForward)) {
                    _referer.InsertBlockBreak();
                }
            }
            AdjustFigureBounds();
            RefreshCaret();
            RecordExpectedCaretPosX();
        }

        private Action GetInsertBlockBreakPostAction() {
            var blockSize = Size.Empty;
            var block = GetBlockAtCaretIndex();
            blockSize = _figure.GetBlockSize(block);
            Action post = () => {
                var newBlock = GetBlockAtCaretIndex();
                var oldBlock = newBlock.PrevSibling as Block;
                _figure.DirtySizeAndVisLine(newBlock);
                _figure.DirtySizeAndVisLine(oldBlock);
                _figure.DirtyBounds(newBlock);
                _figure.DirtyBounds(oldBlock);

                var newBlockSize = _figure.GetBlockSize(newBlock);
                var oldBlockSize = _figure.GetBlockSize(oldBlock);
                var heightDelta = (oldBlockSize.Height + newBlockSize.Height) - blockSize.Height;
                if (newBlock.HasNextSibling) {
                    _figure.UpdateBoundsAfter(
                        newBlock.NextSibling as Block,
                        rect => new Rectangle(rect.Left, rect.Top + heightDelta, rect.Width, rect.Height),
                        rect => new Rectangle(rect.Left, rect.Top + heightDelta, rect.Width, rect.Height)
                    );
                }
            };
            return post;
        }
        
        public void InsertDynamicAbbrev() {
            var dyn = Host.Site.EditorCanvas.AbbrevWordProvider;
            var inputting = GetInputingWordPart();
            if (StringUtil.IsNullOrWhitespace(inputting)) {
                return;
            }

            var candidates = dyn.GetCandidates(inputting);
            if (candidates.Count() < 1) {
                return;
            }

            var selected = false;
            var completion = new Completion();
            completion._completionListBox.Width = 200;
            completion._completionListBox.Height = completion._completionListBox.ItemHeight * Math.Min(20, candidates.Count());
            completion.Font = Host.Site.EditorCanvas.Font;

            var popup = new Popup(completion);
            completion.SetItems(candidates.ToArray());
            completion._completionListBox.KeyDown += (se, ev) => {
                if (ev.KeyCode == Keys.Enter || ev.KeyData == (Keys.M | Keys.Control)) {
                    selected = true;
                    popup.Close();
                } else if (ev.KeyData == (Keys.G | Keys.Control)) {
                    popup.Close();
                }
            };
            completion._completionListBox.MouseDoubleClick += (se, ev) => {
                selected = true;
                popup.Close();
            };

            popup.AutoClose = true;
            popup.Closed += (se, ev) => {
                if (!selected) {
                    return;
                }

                var inserting = completion.SelectedString;
                if (!string.IsNullOrEmpty(inserting)) {
                    InsertText(inserting.Remove(0, inputting.Length), false);
                }
            };


            var inputtingFirstCharRect = _figure.GetCharRect(_referer.CaretIndex - inputting.Length);
            var popupLoc = Host.Site.EditorCanvas.TranslateToControlPoint(new Point(inputtingFirstCharRect.Left, inputtingFirstCharRect.Bottom + 2));
            popup.Show(Host.Site.EditorCanvas, popupLoc);
            completion.Focus();

        }
        
        // --- remove ---
        public void RemoveForward() {
            using (_figure.DirtManager.BeginDirty()) {
                /// cache更新範囲を狭くするための分岐
                if (_referer.Selection.IsEmpty) {
                    var inline = _referer.Target.GetInlineAt(_referer.CaretIndex);
                    if (inline.IsLineEndCharacter) {
                        if (_referer.Target.VerticalAlignment == VerticalAlignment.Top && inline is BlockBreak) {
                            var post = GetRemoveForwardPostAction();
                            if (post == null) {
                                return;
                            }
                            using (_figure.BeginUpdateStyledText(() => {}, post)) {
                                _referer.RemoveForward();
                            }
                        } else {
                            using (_figure.BeginUpdateStyledText(DirtyForward)) {
                                _referer.RemoveForward();
                            }
                        }
                    } else {
                        /// cache更新はAbstractNode.HandleStyledTextContentsChanged()に任せる
                        /// ここでcache更新するようにBeginUpdateStyledText()した方がいいかも
                        _referer.RemoveForward();
                    }
                } else {
                    using (_figure.BeginUpdateStyledText(DirtyForward)) {
                        _referer.RemoveForward();
                    }
                }

                AdjustFigureBounds();
                RefreshCaret();
                RecordExpectedCaretPosX();
            }
        }

        private Action GetRemoveForwardPostAction() {
            var curBlock = default(Block);
            var curBlockSize = Size.Empty;
            var nextBlock = default(Block);
            var nextBlockSize = Size.Empty;
            curBlock = GetBlockAtCaretIndex();
            curBlockSize = _figure.GetBlockSize(curBlock);
            if (!curBlock.HasNextSibling) {
                return null;
            }
            nextBlock = curBlock.NextSibling as Block;
            nextBlockSize = _figure.GetBlockSize(nextBlock);

            Action post = () => {
                var newBlock = GetBlockAtCaretIndex();
                _figure.DirtySizeAndVisLine(curBlock);
                _figure.DirtySizeAndVisLine(nextBlock);
                _figure.DirtySizeAndVisLine(newBlock);
                _figure.DirtyBounds(curBlock);
                _figure.DirtyBounds(nextBlock);
                _figure.DirtyBounds(newBlock);

                var newBlockSize = _figure.GetBlockSize(newBlock);
                var heightDelta = newBlockSize.Height - (curBlockSize.Height + nextBlockSize.Height);
                if (newBlock.HasNextSibling) {
                    _figure.UpdateBoundsAfter(
                        newBlock.NextSibling as Block,
                        rect => new Rectangle(rect.Left, rect.Top + heightDelta, rect.Width, rect.Height),
                        rect => new Rectangle(rect.Left, rect.Top + heightDelta, rect.Width, rect.Height)
                    );
                }

            };
            return post;
        }

        public void RemoveBackward() {
            using (_figure.DirtManager.BeginDirty()) {
                /// cache更新範囲を狭くするための分岐
                if (_referer.Selection.IsEmpty) {
                    if (_referer.CaretIndex > 0) {
                        var inline = _referer.Target.GetInlineAt(_referer.CaretIndex - 1);
                        if (inline.IsLineEndCharacter) {
                            if (_referer.Target.VerticalAlignment == VerticalAlignment.Top && inline is BlockBreak) {
                                var post = GetRemoveBackwardPostAction();
                                using (_figure.BeginUpdateStyledText(() => { }, post)) {
                                    _referer.RemoveBackward();
                                }
                            } else {
                                using (_figure.BeginUpdateStyledText(DirtyBackword)) {
                                    _referer.RemoveBackward();
                                }
                            }
                        } else {
                            /// cache更新はAbstractNode.HandleStyledTextContentsChanged()に任せる
                            /// ここでcache更新するようにBeginUpdateStyledText()した方がいいかも
                            _referer.RemoveBackward();
                        }
                    }
                } else {
                    using (_figure.BeginUpdateStyledText(DirtyBackword)) {
                        _referer.RemoveBackward();
                    }
                }
                AdjustFigureBounds();
                RefreshCaret();
                RecordExpectedCaretPosX();
            }
        }

        private Action GetRemoveBackwardPostAction() {
            var curBlock = default(Block);
            var nextBlock = default(Block);
            var curBlockSize = Size.Empty;
            var nextBlockSize = Size.Empty;
            curBlock = _referer.Target.GetBlockAt(_referer.CaretIndex - 1);
            curBlockSize = _figure.GetBlockSize(curBlock);
            if (curBlock.HasNextSibling) {
                nextBlock = curBlock.NextSibling as Block;
                nextBlockSize = _figure.GetBlockSize(nextBlock);
            }

            Action post = () => {
                var newBlock = GetBlockAtCaretIndex();
                _figure.DirtySizeAndVisLine(curBlock);
                _figure.DirtySizeAndVisLine(newBlock);
                _figure.DirtyBounds(curBlock);
                _figure.DirtyBounds(newBlock);
                if (nextBlock != null) {
                    _figure.DirtySizeAndVisLine(nextBlock);
                    _figure.DirtyBounds(nextBlock);
                }

                var newBlockSize = _figure.GetBlockSize(newBlock);
                var heightDelta = newBlockSize.Height - (curBlockSize.Height + nextBlockSize.Height);
                if (newBlock.HasNextSibling) {
                    _figure.UpdateBoundsAfter(
                        newBlock.NextSibling as Block,
                        rect => new Rectangle(rect.Left, rect.Top + heightDelta, rect.Width, rect.Height),
                        rect => new Rectangle(rect.Left, rect.Top + heightDelta, rect.Width, rect.Height)
                    );
                }

            };
            return post;
        }

        // --- style ---
        /// <summary>
        /// Selectionの範囲の文字列のParagraphを設定する．
        /// </summary>
        public void SetParagraphKind(ParagraphKind paragraphKind) {
            _referer.SetParagraphKind(paragraphKind);
            AdjustFigureBounds();
            RefreshCaret();
        }

        /// <summary>
        /// Selectionの範囲の文字列のFontを設定する．
        /// </summary>
        public void SetFont(Func<Flow, FontDescription> fontProvider) {
            using (_figure.BeginUpdateStyledText()) {
                _referer.SetFont(fontProvider);
            }
            AdjustFigureBounds();
            RefreshCaret();
        }

        public void SetFont(Func<FontDescription, FontDescription> fontConverter) {
            SetFont(flow => fontConverter(flow.Font));
        }

        /// <summary>
        /// Selectionの範囲の文字列のFontを設定する．
        /// </summary>
        public void SetFont(FontDescription font) {
            SetFont((Flow flow) => font);
        }

        /// <summary>
        /// Selectionの範囲の文字列のFontNameを設定する．
        /// </summary>
        public void SetFontName(string fontName) {
            SetFont(flow => new FontDescription(flow.Font, fontName));
        }

        /// <summary>
        /// Selectionの範囲の文字列のFontSizeを設定する．
        /// </summary>
        public void SetFontSize(float fontSize) {
            SetFont(flow => new FontDescription(flow.Font, fontSize));
        }

        /// <summary>
        /// Selectionの範囲の文字列のFontStyleを設定する．
        /// </summary>
        public void SetFontStyle(FontStyle fontStyle) {
            SetFont(flow => new FontDescription(flow.Font, fontStyle));
        }

        /// <summary>
        /// Selectionの範囲の文字列のColorを設定する．
        /// </summary>
        public void SetColor(Color color) {
            _referer.SetColor(color);
            AdjustFigureBounds();
            RefreshCaret();
        }

        public void SetHorizontalAlignment(HorizontalAlignment hAlign) {
            _referer.SetHorizontalAlignment(hAlign);
            AdjustFigureBounds();
            RefreshCaret();
            RecordExpectedCaretPosX();
        }

        public void SetVerticalAlignment(VerticalAlignment vAlign) {
            _referer.SetVerticalAlignment(vAlign);
            AdjustFigureBounds();
            RefreshCaret();
        }

        public void ToggleList(ListKind listKind) {
            _referer.ToggleList(listKind);
            AdjustFigureBounds();
            RefreshCaret();
            RecordExpectedCaretPosX();
        }

        public void ToggleUnorderedList() {
            _referer.ToggleUnorderedList();
            AdjustFigureBounds();
            RefreshCaret();
            RecordExpectedCaretPosX();
        }

        public void ToggleOrderedList() {
            _referer.ToggleOrderedList();
            AdjustFigureBounds();
            RefreshCaret();
            RecordExpectedCaretPosX();
        }

        public void ToggleCheckBoxList() {
            _referer.ToggleCheckBoxList();
            AdjustFigureBounds();
            RefreshCaret();
            RecordExpectedCaretPosX();
        }

        public void ToggleTriStateCheckBoxList() {
            _referer.ToggleTriStateCheckBoxList();
            AdjustFigureBounds();
            RefreshCaret();
            RecordExpectedCaretPosX();
        }

        public void ToggleStarList() {
            _referer.ToggleStarList();
            AdjustFigureBounds();
            RefreshCaret();
            RecordExpectedCaretPosX();
        }

        public void ToggleLeftArrowList() {
            _referer.ToggleLeftArrowList();
            AdjustFigureBounds();
            RefreshCaret();
            RecordExpectedCaretPosX();
        }

        public void ToggleRightArrowList() {
            _referer.ToggleRightArrowList();
            AdjustFigureBounds();
            RefreshCaret();
            RecordExpectedCaretPosX();
        }

        public void Indent() {
            _referer.Indent();
            AdjustFigureBounds();
            RefreshCaret();
            RecordExpectedCaretPosX();
        }

        public void Outdent() {
            _referer.Outdent();
            AdjustFigureBounds();
            RefreshCaret();
            RecordExpectedCaretPosX();
        }

        public void ChangeToNextListState() {
            _referer.ChangeToNextListState();
        }

        // --- link ---
        public void SetLink(string uri, string relationship) {
            _referer.SetLink(uri, relationship);
        }

        public void UnsetLink() {
            _referer.UnsetLink();
        }

        // --- util ---
        public void SetRunText(Run run, string text) {
            _referer.SetRunText(run, text);
            AdjustFigureBounds();
        }

        // --- clipboard ---
        public void Paste() {
            if (ClipboardUtil.ContainsBlocksAndInlines()) {
                using (_figure.DirtManager.BeginDirty()) {
                    _Caret.Hide();

                    using (_figure.BeginUpdateStyledText(DirtyForward)) {
                        _referer.Paste();
                    }

                    AdjustFigureBounds();
                    RefreshCaret();
                    RecordExpectedCaretPosX();
                    _Caret.Show();
                }
            }
        }

        public void PasteText(bool inBlock) {
            if (ClipboardUtil.ContainsText()) {
                using (_figure.DirtManager.BeginDirty()) {
                    _Caret.Hide();

                    using (_figure.BeginUpdateStyledText(DirtyForward)) {
                        _referer.PasteText(inBlock);
                    }

                    AdjustFigureBounds();
                    RefreshCaret();
                    RecordExpectedCaretPosX();
                    _Caret.Show();
                }
            }
        }

        public void PasteInlinesOrText(bool inBlock) {
            if (inBlock) {
                PasteText(inBlock);
            } else {
                if (ClipboardUtil.ContainsBlocksAndInlines()) {
                    Paste();
                } else if (ClipboardUtil.ContainsText()) {
                    PasteText(inBlock);
                }
            }
                
        }

        public void PasteLastLine() {
            _referer.PasteLastLine();
            RecordExpectedCaretPosX();
            AdjustFigureBounds();
        }

        // --- undo redo ---
        public void Undo() {
            using (_figure.BeginUpdateStyledText()) {
                _referer.Undo();
            }
            AdjustFigureBounds();
            RefreshCaret();
            RecordExpectedCaretPosX();
        }

        public void Redo() {
            using (_figure.BeginUpdateStyledText()) {
                _referer.Redo();
            }
            AdjustFigureBounds();
            RefreshCaret();
            RecordExpectedCaretPosX();
        }

        // --- misc ---
        public void RefreshCaret() {
            if (!IsBegun || Host == null || _referer == null || _referer.Target == null) {
                return;
            }

            if (_referer.Target.IsLineHead(_referer.CaretIndex)) {
                /// 行頭であれば
                var charRect = _figure.GetCharRect(_referer.CaretIndex);
                _Caret.Position = charRect.Location;
                _Caret.Height = charRect.Height;

            } else if (_figure.IsVisualLineHead(_referer.CaretIndex)) {
                /// 表示行の最初
                var charRect = _figure.GetCharRect(_referer.CaretIndex);
                _Caret.Position = charRect.Location;
                _Caret.Height = charRect.Height;

            } else {
                /// それ以外であれば
                var prevCharRect = _figure.GetCharRect(_referer.CaretIndex - 1);
                _Caret.Position = new Point(prevCharRect.Right, prevCharRect.Top);
                _Caret.Height = prevCharRect.Height;
            }
        }

        public void AdjustFigureBounds() {
            if (Relocator != null) {
                Relocator(this, Figure, Host.Figure);

            } else {
                var hostFigure = Host.Figure;
                var rect = default(Rectangle);
                var hostNode = Host.Figure as INode;
                var textBounds = Rectangle.Empty;
                if (
                    hostNode != null &&
                    hostNode.Root != null &&
                    hostNode.Root.Canvas != null &&
                    !hostNode.Root.Canvas.IsDisposed &&
                    hostNode.StyledText != null
                ) {
                    textBounds = _figure.GetStyledTextBoundsFor(hostNode.ClientArea);
                    if (textBounds.IsEmpty) {
                        rect = _figure.Bounds;
                    } else {
                        rect = _figure.Padding.GetBounds(textBounds);
                    }

                    if (_isConsiderHostBounds) {
                        rect = Rectangle.Union(rect, hostFigure.Bounds);
                    }

                } else {
                    rect = _figure.Bounds;
                }

                var hostSTBounds = hostNode.Padding.GetBounds(hostNode.StyledTextBounds);
                var bounds = Rectangle.Union(rect, hostSTBounds);
                if (_isConsiderHostBounds && bounds.Left < hostFigure.Left) {
                    bounds = new Rectangle(hostFigure.Left, bounds.Top, bounds.Width, bounds.Height);
                }
                _figure.Bounds = bounds;

                //if (_isConsiderImeWindowSize) {
                //    _figure.AdjustSize();
                //}
            }
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected override void OnFigureKeyPress(KeyPressEventArgs e) {
            /// 文字の入力
            if (!Char.IsControl(e.KeyChar)) {
                Insert(e.KeyChar.ToString(), _inImeComposition);
                if (_inImeComposition) {
                    AdjustImeBounds();
                }
            }

            base.OnFigureKeyPress(e);
        }

        protected override void OnFigureKeyDown(KeyEventArgs e) {
            if (e.KeyData == Keys.Escape) {
                /// focus解除
                e.SuppressKeyPress = true;
                Host.RequestFocus(FocusKind.Rollback, null);
            }

            base.OnFigureKeyDown(e);
        }

        private bool _figureMouseDownHandled = false;

        protected override void OnFigureMouseDown(MouseEventArgs e) {
            base.OnFigureMouseDown(e);

            _figureMouseDownHandled = false;

            if (e.Button == MouseButtons.Left) {
                var bulcmd = _figure.GetProcessCheckBoxBulletCommand(e.Location);
                if (bulcmd != null) {
                    CommandExecutor.Execute(bulcmd);
                    _figureMouseDownHandled = true;
                }
            }

            if (!_figureMouseDownHandled) {
                if (e.Clicks == 2) {
                    /// 単語選択
                    var index = _figure.GetCharIndexAt(e.Location);
                    int col, lineOffset;
                    var line = _referer.Target.GetLineSegmentAt(index, out col, out lineOffset);
                    var range = StringUtil.GetWordRange(line.Text, col);
                    _referer.Selection.Range = new Range(range.Offset + lineOffset, range.Length);
                } else if (e.Clicks == 3) {
                    /// 行選択
                    var index = _figure.GetCharIndexAt(e.Location);
                    int col, lineOffset;
                    var line = _referer.Target.GetLineSegmentAt(index, out col, out lineOffset);
                    var range = _referer.Target.GetRange(line);
                    _referer.Selection.Range = range;
                } else if (e.Clicks == 4) {
                    /// 段落選択
                    var index = _figure.GetCharIndexAt(e.Location);
                    var block = _referer.Target.GetBlockAt(index);
                    var range = _referer.Target.GetRange(block);
                    _referer.Selection.Range = range;
                }
            }
        }

        protected override void OnFigureMouseClick(MouseEventArgs e) {
            /// 右クリックしてe.Locationが選択領域内のとき以外は選択領域をクリアしてcaret移動
            if (e.Button == MouseButtons.Right && !_referer.Selection.IsEmpty) {
                var rects = _figure.GetStringRect(Selection.Range.Offset, Selection.Range.Length);

                var inSelection = false;
                foreach (var rect in rects) {
                    if (rect.Contains(e.Location)) {
                        inSelection = true;
                        break;
                    }
                }
                if (!inSelection) {
                    _referer.Selection.Clear();
                    _referer.CaretIndex = _figure.GetCharIndexAt(e.Location);
                    RecordExpectedCaretPosX();
                }

            } else if (!_figureMouseDownHandled) {
                if (KeyUtil.IsShiftPressed()) {
                    /// 範囲選択
                    if (_referer.Selection.IsEmpty) {
                        var charIndex = _figure.GetCharIndexAt(e.Location);
                        var selStart = Math.Min(charIndex, _referer.CaretIndex);
                        var selEnd = Math.Max(charIndex, _referer.CaretIndex);
                        _referer.CaretIndex = charIndex;
                        _referer.Selection.Range = new Range(selStart, selEnd - selStart);
                    } else {
                        var sel = _referer.Selection;
                        var start = sel.Offset == _referer.CaretIndex ? sel.Range.End : sel.Offset;
                        var charIndex = _figure.GetCharIndexAt(e.Location);
                        var selStart = Math.Min(charIndex, start);
                        var selEnd = Math.Max(charIndex, start);
                        _referer.CaretIndex = charIndex;
                        _referer.Selection.Range = new Range(selStart, selEnd - selStart);
                    }
                    RecordExpectedCaretPosX();

                } else {

                    if (!_referer.Selection.IsEmpty) {
                        _referer.Selection.Clear();
                    }
                    _referer.CaretIndex = _figure.GetCharIndexAt(e.Location);
                    RecordExpectedCaretPosX();

                    if (e.Button == MouseButtons.Middle || (KeyUtil.IsControlPressed() && e.Button == MouseButtons.Left)) {
                        /// 真中クリックまたはCtrl+左クリックならlinkクリック
                        var run = GetInlineAtCaretIndex() as Run;
                        if (run != null && run.HasLink) {
                            OnLinkClicked(run.Link);
                        }
                    }
                }
            }
            base.OnFigureMouseClick(e);
        }

        protected override void OnFigureDragStart(MouseEventArgs e) {
            if (_figureMouseDownHandled) {
                return;
            }

            if (e.Clicks == 1) {
                _referer.Selection.Clear();
            }

            if (_referer.Selection.IsEmpty) {
                _dragStartCharRange = new Range(_figure.GetCharIndexAt(e.Location), 0);
            } else {
                _dragStartCharRange = _referer.Selection.Range;
            }

            base.OnFigureDragStart(e);
        }

        protected override void OnFigureDragMove(MouseEventArgs e) {
            if (_figureMouseDownHandled) {
                return;
            }

            /// 範囲選択終了
            using (Host.Site.DirtManager.BeginDirty()) {
                var i = _figure.GetCharIndexAt(e.Location);
                if (i != _dragEndCharIndex) {
                    _dragEndCharIndex = i;
                    if (_dragStartCharRange.Contains(_dragEndCharIndex)) {
                        _referer.Selection.Range = _dragStartCharRange;
                    } else {
                        if (_dragStartCharRange.Length == 0) {
                            var selStart = Math.Min(_dragStartCharRange.Start, _dragEndCharIndex);
                            var selEnd = Math.Max(_dragStartCharRange.Start, _dragEndCharIndex);
                            _referer.CaretIndex = _dragEndCharIndex;
                            _referer.Selection.Range = new Range(selStart, selEnd - selStart);
                        } else {
                            var selStart = Math.Min(_dragStartCharRange.Start, _dragEndCharIndex);
                            var selEnd = Math.Max(_dragStartCharRange.End, Math.Max(0, _dragEndCharIndex - 1));
                            _referer.CaretIndex = _dragEndCharIndex;
                            _referer.Selection.Range = Range.FromStartAndEnd(selStart, selEnd);
                        }
                    }

                    RecordExpectedCaretPosX();
                }
            }
            base.OnFigureDragMove(e);
        }

        protected override void OnFigureDragFinish(MouseEventArgs e) {
            if (_figureMouseDownHandled) {
                return;
            }

            _dragEndCharIndex = -1;
            base.OnFigureDragFinish(e);
        }

        protected override void OnFigureDragCancel() {
            if (_figureMouseDownHandled) {
                return;
            }

            _dragEndCharIndex = -1;
            base.OnFigureDragCancel();
        }

        protected virtual void OnLinkClicked(Link link) {
            var handler = LinkClicked;
            if (handler != null) {
                handler(this, new Mkamo.Editor.Core.LinkClickedEventArgs(link));
            }
        }

        // ------------------------------
        // private
        // ------------------------------
        private void RecordExpectedCaretPosX() {
            _expectedCaretPosX = _figure.GetCharRect(_referer.CaretIndex).X;
        }

        private void DirtyForward() {
            var block = default(Block);
            switch (_referer.Target.VerticalAlignment) {
                case VerticalAlignment.Top:
                    block = Selection.IsEmpty ? GetBlockAtCaretIndex() : GetBlockAtSelectionStart();
                    _figure.DirtySizeAndVisLine(block);
                    _figure.DirtyAllBoundsAfter(block);
                    break;
                case VerticalAlignment.Center:
                    block = Selection.IsEmpty ? GetBlockAtCaretIndex() : GetBlockAtSelectionStart();
                    _figure.DirtySizeAndVisLine(block);
                    _figure.DirtyAllBounds();
                    break;
                case VerticalAlignment.Bottom:
                    block = Selection.IsEmpty ? GetBlockAtCaretIndex() : GetBlockAtSelectionStart();
                    _figure.DirtySizeAndVisLine(block);
 
                    block = Selection.IsEmpty ? GetBlockAtCaretIndex() : GetBlockAtSelectionEnd();
                    block = block.HasNextSibling ? block.NextSibling as Block : block;
                    _figure.DirtyAllBoundsBefore(block);
                    break;
            }
        }

        private void DirtyBackword() {
            var block = default(Block);
            switch (_referer.Target.VerticalAlignment) {
                case VerticalAlignment.Top:
                    block = Selection.IsEmpty ? GetBlockAtCaretIndex() : GetBlockAtSelectionStart();
                    _figure.DirtySizeAndVisLine(block);
                    block = block.HasPrevSibling ? block.PrevSibling as Block : block;
                    _figure.DirtySizeAndVisLine(block);

                    _figure.DirtyAllBoundsAfter(block);
                    break;
                case VerticalAlignment.Center:
                    block = Selection.IsEmpty ? GetBlockAtCaretIndex() : GetBlockAtSelectionStart();
                    _figure.DirtySizeAndVisLine(block);
                    block = block.HasPrevSibling ? block.PrevSibling as Block : block;
                    _figure.DirtySizeAndVisLine(block);

                    _figure.DirtyAllBounds();
                    break;
                case VerticalAlignment.Bottom:
                    block = Selection.IsEmpty ? GetBlockAtCaretIndex() : GetBlockAtSelectionStart();
                    _figure.DirtySizeAndVisLine(block);
                    block = block.HasPrevSibling ? block.PrevSibling as Block : block;
                    _figure.DirtySizeAndVisLine(block);

                    block = Selection.IsEmpty ? GetBlockAtCaretIndex() : GetBlockAtSelectionEnd();
                    block = block.HasNextSibling ? block.NextSibling as Block : block;
                    _figure.DirtyAllBoundsBefore(block);
                    break;
            }
        }

        private void DirtyRegion() {
            var start = _referer.IsMarkSet ? Math.Min(_referer.CaretIndex, _referer.Mark.Value) : _referer.CaretIndex;
            var end = _referer.IsMarkSet ? Math.Max(_referer.CaretIndex, _referer.Mark.Value) : _referer.CaretIndex;

            var block = default(Block);
            switch (_referer.Target.VerticalAlignment) {
                case VerticalAlignment.Top:
                    block = _referer.Target.GetBlockAt(start);
                    _figure.DirtySizeAndVisLine(block);
                    _figure.DirtyAllBoundsAfter(block);
                    break;
                case VerticalAlignment.Center:
                    block = _referer.Target.GetBlockAt(start);
                    _figure.DirtySizeAndVisLine(block);
                    _figure.DirtyAllBounds();
                    break;
                case VerticalAlignment.Bottom:
                    block = _referer.Target.GetBlockAt(start);
                    _figure.DirtySizeAndVisLine(block);

                    block = _referer.Target.GetBlockAt(end);
                    block = block.HasNextSibling ? block.NextSibling as Block : block;
                    _figure.DirtyAllBoundsBefore(block);
                    break;
            }
        }


        private int GetPreviousLineIndex(int charIndex) {
            var range = _figure.GetVisualLineRange(_referer.CaretIndex);
            if (range.Offset <= 0) {
                return charIndex;
            }
            var prevVisLineBounds = _figure.GetVisualLineBounds(range.Offset - 1);
            var pt = new Point(_expectedCaretPosX, prevVisLineBounds.Top + 1);
            return _figure.GetCharIndexAt(pt);
        }

        private int GetNextLineIndex(int charIndex) {
            var range = _figure.GetVisualLineRange(_referer.CaretIndex);
            if (range.End >= _referer.Target.Length - 1) {
                return charIndex;
            }
            var nextVisLineBounds = _figure.GetVisualLineBounds(range.End + 1);
            var pt = new Point(_expectedCaretPosX, nextVisLineBounds.Top + 1);
            return _figure.GetCharIndexAt(pt);
        }

        private int GetPreviousPageIndex(int charIndex) {
            var canvas = Host.Site.EditorCanvas;
            var canvasSize = canvas.ClientSize;
            var pos = _Caret.Position;
            return _figure.GetCharIndexAt(new Point(pos.X, pos.Y - (int) (canvasSize.Height * 0.9)));
        }

        private int GetNextPageIndex(int charIndex) {
            var canvas = Host.Site.EditorCanvas;
            var canvasSize = canvas.ClientSize;
            var pos = _Caret.Position;
            return _figure.GetCharIndexAt(new Point(pos.X, pos.Y + (int) (canvasSize.Height * 0.9)));
        }

        private void HandleRefererCaretMove(object sender, CaretMovedEventArgs e) {
            if (_figure.InUpdatingStyledText) {
                /// BeginUpdateStyledText()中は_figureのcacheが更新されないようにする
                return;
            }

            if (_Caret.IsVisible) {
                RefreshCaret();
            }
            _figure.OnOwnerCaretMoved(e);
        }

        private void HandleRefererSelectionChanged(object sender, EventArgs e) {
            using (_figure.DirtManager.BeginDirty()) {
                _figure.Selection = _referer.Selection.Range;
                _figure.OnOwnerSelectionChanged();
            }
        }

        private void HandleEditorCanvasImeStartComposition(object sender, EventArgs e) {
            if (_isConsiderImeWindowSize) {
                _inImeComposition = true;
            }
        //    if (_isConsiderImeWindowSize) {
        //        _figure.IsImeOpened = true;
        //        HandleEditorCanvasImeComposition(sender, e);
        //        //AdjustFigureBounds();
        //    }
        }

        private void HandleEditorCanvasImeEndComposition(object sender, EventArgs e) {
            if (_isConsiderImeWindowSize) {
                _inImeComposition = false;
                _figure.IsImeOpened = false;
                _figure.AdjustSize();
                RefreshCaret();
                RecordExpectedCaretPosX();
                //AdjustFigureBounds();
            }
        }

        private void HandleEditorCanvasImeComposition(object sender, EventArgs e) {
            AdjustImeBounds();
        }

        private void AdjustImeBounds() {
            if (_isConsiderImeWindowSize) {
                _figure.IsImeOpened = true;

                var canvas = Host.Site.EditorCanvas;
                var str = canvas.GetImeString();
                /// Enterキーを押して確定した直後にstr == ""でImeCompositionがくるので無視する
                if (string.IsNullOrEmpty(str)) {
                    return;
                }
                var loc = canvas.GetImeWindowLocation();
                using (var g = canvas.CreateGraphics())
                using (var font = GetNextInputFont().CreateFont()) {
                    //var renderer = new Gdi32TextRenderer(g, font);
                    //var drawable = -1;
                    //var size = renderer.MeasureText(str, 200, out drawable);
                    //size += new Size(6, 0);
                    //renderer.Dispose();

                    //var size = TextRenderer.MeasureText(
                    //    str, font, new Size(int.MaxValue, int.MaxValue), _figure.GetFormatFlags()
                    //);
                    //size -= new Size(3, 0); /// 調整，なぜこの値なのかは不明
                    var size = _figure.MeasureText(g, str, font, int.MaxValue);
                    size += new Size(0, -2); /// 調整，なぜこの値なのかは不明

                    _figure.ImeWindowBounds = new Rectangle(loc, size);
                    _figure.AdjustSize();
                    //AdjustFigureBounds();
                }
            }
        }

    }
}
