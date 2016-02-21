/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Figure.Core;
using System.Drawing;
using Mkamo.Common.DataType;
using System.Windows.Forms;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Figure.Internal.Figures;
using Mkamo.Common.Core;
using Mkamo.Common.Diagnostics;
using Mkamo.StyledText.Core;
using Mkamo.Common.Win32.Gdi32;
using DataType = Mkamo.Common.DataType;
using Mkamo.Common.Forms.Drawing;
using Mkamo.Figure.Properties;
using Mkamo.StyledText.Commands;
using Mkamo.Common.Command;

namespace Mkamo.Figure.Figures {
    using StyledText = Mkamo.StyledText.Core.StyledText;

    partial class AbstractNode {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // ========================================
        // field
        // ========================================

        // ========================================
        // constructor
        // ========================================

        // ========================================
        // property
        // ========================================
        public virtual Size MaxSize {
            get { return _maxSize; }
            set {
                if (value == _maxSize) {
                    return;
                }
                if (value.Width < _minSize.Width || value.Height < _minSize.Height) {
                    return;
                }

                _maxSize = value;

                if (_blockRenderer != null) {
                    _blockRenderer.MaxWidth = _maxSize.Width - Padding.Width;
                }
                if (_visualLineCache != null) {
                    _visualLineCache.DirtyAll();
                }
                if (_sizeCache != null) {
                    _sizeCache.DirtyAll();
                }
                if (_boundsCache != null) {
                    _boundsCache.DirtyAll();
                }

                if (Bounds.Width > _maxSize.Width || Bounds.Height > _maxSize.Height) {
                    Bounds = new Rectangle(
                        Bounds.Left,
                        Bounds.Top,
                        Math.Min(Bounds.Width, _maxSize.Width),
                        Math.Min(Bounds.Height, _maxSize.Height)
                    );
                }

                OnMaxSizeChanged();
            }
        }

        public virtual StyledText StyledText {
            get { return _styledText; }
            set {
                if (value == _styledText) {
                    return;
                }

                if (_styledText != null) {
                    _styledText.ContentsChanged -= HandleStyledTextContentsChanged;
                    _visualLineCache = null;
                    _sizeCache = null;
                    _boundsCache = null;
                    _blockRenderer = null;
                    _styledTextSizeCache = Size.Empty;
                }
                _styledText = value;
                if (_styledText != null) {
                    _visualLineCache = new VisualLineCache(GetVisualLinesCore);
                    _sizeCache = new SizeCache(_styledText, GetInlineSizeCore, GetLineSegmentSizeCore, GetBlockSizeCore);
                    _boundsCache = new BoundsCache(_styledText, GetBlockRectCore, GetLineBoundsCore);
                    _blockRenderer = new BlockRenderer(
                        this, _fontCache, _visualLineCache, _sizeCache, _boundsCache
                    );
                    _blockRenderer.MaxWidth = _maxSize.Width - Padding.Width;
                    _selection = Range.Empty;
                    _styledText.ContentsChanged += HandleStyledTextContentsChanged;
                }
            }
        }

        public Size StyledTextSize {
            get {
                if (Root == null || Root.Canvas == null || Root.Canvas.IsDisposed || _styledText == null) {
                    return Size.Empty;
                }
                if (_styledTextSizeCache.IsEmpty) {
                    using (var context = GetGraphicsUsingContext()) {
                        if (context == null) {
                            return Size.Empty;
                        }

                        var g = context.Graphics;
                        var rect = default(Rectangle);
                        var first = true;
                        foreach (var block in _styledText.Blocks) {
                            if (first) {
                                rect = GetBlockBounds(g, block);
                                first = false;
                            } else {
                                rect = Rectangle.Union(rect, GetBlockBounds(g, block));
                            }
                        }
                        _styledTextSizeCache = rect.Size;
                    }
                }
                return _styledTextSizeCache;
            }
        }

        public Rectangle StyledTextBounds {
            get { return GetStyledTextBoundsFor(ClientArea); }
        }

        public bool InUpdatingStyledText {
            get { return _updateStyledTextDepth > 0; }
        }

        public Range Selection {
            get { return _selection; }
            set {
                if (value == _selection) {
                    return;
                }

                var oldSelection = _selection;

                _selection = value;

                /// 再描画領域の計算
                if (!InUpdatingStyledText) {
                    /// EndUpdateStyledText()でInvalidatePaint()されるので呼ばなくてよい。
                    /// 呼んでしまうと中途半端にcacheが作成されてしまって
                    /// CutRegion()時にうまく更新されなくなる
                    InvalidatePaintSelection(oldSelection);
                }
            }
        }

        private Rectangle GetBoundsForInvalidation(Graphics g, int startCharIndex, int endCharIndex) {
            int dummy1, dummy2;
            var firstLine = _styledText.GetLineSegmentAt(
                startCharIndex, out dummy1, out dummy2
            );
            var lastLine = _styledText.GetLineSegmentAt(
                endCharIndex, out dummy1, out dummy2
            );
            var firstLineBounds = _boundsCache.GetBounds(g, firstLine);
            var lastLineBounds = _boundsCache.GetBounds(g, lastLine);
            return new Rectangle(Left, firstLineBounds.Top, Width, lastLineBounds.Bottom - firstLineBounds.Top);
        }

        private void InvalidatePaintSelection(Range oldSelection) {
            if (Root != null) {
                using (var context = GetGraphicsUsingContext()) {
                    if (context == null) {
                        return;
                    }
                    var g = context.Graphics;

                    var r = Rectangle.Empty;
                    if (!_selection.IsEmpty) {
                        r = GetBoundsForInvalidation(g, _selection.Offset, _selection.End);
                    }
                    if (!oldSelection.IsEmpty) {
                        var oldSelectionBounds = GetBoundsForInvalidation(g, oldSelection.Offset, oldSelection.End);
                        if (r.IsEmpty) {
                            r = oldSelectionBounds;
                        } else {
                            r = Rectangle.Union(r, oldSelectionBounds);
                        }
                    }
                    if (!r.IsEmpty) {
                        r = new Rectangle(Left, r.Top, Width, r.Height);
                        InvalidatePaint(r);
                    }
                }
            }
        }

        public Color SelectionBorderColor {
            get { return _selectionBorderColor; }
            set {
                if (value == _selectionBorderColor) {
                    return;
                }
                _selectionBorderColor = value;
                _ResourceCache.DisposeResource(SelectionPenResourceKey);
                InvalidatePaint();
            }
        }
        public IBrushDescription SelectionBrush {
            get { return _selectionBrush; }
            set {
                if (value == _selectionBrush) {
                    return;
                }
                _selectionBrush = value;
                _ResourceCache.DisposeResource(SelectionBrushResourceKey);
                InvalidatePaint();
            }
        }

        // ========================================
        // method
        // ========================================
        protected virtual void OnMaxSizeChanged() {
            var handler = MaxSizeChanged;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        protected internal override void DisposeResourceCache() {
            base.DisposeResourceCache();
            if (_fontCache != null) {
                _fontCache.Dispose();
            }
        }

        protected override void OnBoundsChangedAfter(
            Rectangle oldBounds, Rectangle newBounds, IEnumerable<IFigure> movingFigures
        ) {
            _styledTextSizeCache = Size.Empty;
            if (oldBounds.Width != newBounds.Width) {
                if (_visualLineCache != null) {
                    _visualLineCache.DirtyAll();
                }
                if (_sizeCache != null) {
                    _sizeCache.DirtyAllLineSeguments();
                    _sizeCache.DirtyAllBlocks();
                }
            }
            if (_boundsCache != null) {
                var isSameLoc = oldBounds.Location == newBounds.Location;
                var isSameWidth = oldBounds.Width == newBounds.Width;
                if (!(isSameLoc && isSameWidth && _styledText.VerticalAlignment == VerticalAlignment.Top)) {
                    _boundsCache.DirtyAll(); /// 遅いのでできるだけ呼ばないようにする
                }
            }

            base.OnBoundsChangedAfter(oldBounds, newBounds, movingFigures);
        }

        // --- bulk update ---
        public UpdateStyledTextContext BeginUpdateStyledText() {
            return BeginUpdateStyledText(null, null);
        }

        public UpdateStyledTextContext BeginUpdateStyledText(Action pre) {
            return BeginUpdateStyledText(pre, null);
        }

        public UpdateStyledTextContext BeginUpdateStyledText(Action pre, Action post) {
            if (_updateStyledTextDepth == 0) {
                if (_styledText != null) {
                    _styledTextSizeCache = Size.Empty;
                    if (pre == null) {
                        _visualLineCache.DirtyAll();
                        _sizeCache.DirtyAll();
                        _boundsCache.DirtyAll();
                    } else {
                        pre();
                    }
                }
            }

            ++_updateStyledTextDepth;
            return new UpdateStyledTextContext(this, post);
        }

        public void EndUpdateStyledText(UpdateStyledTextContext context) {
            if (_updateStyledTextDepth > 0) {
                --_updateStyledTextDepth;
                if (_updateStyledTextDepth == 0) {
                    if (_styledText != null) {
                        if (context.Action != null) {
                            context.Action();
                        }
                    }
                    InvalidatePaint();
                }
            }
        }

        // --- dirty notification ---
        /// <summary>
        /// すべてのLineに対するVisualLineのキャッシュをdirtyする。
        /// </summary>
        public void DirtyAllVisLines() {
            if (_visualLineCache != null) {
                _visualLineCache.DirtyAll();
            }
        }

        /// <summary>
        /// block以降に含まれるすべてのLineに対するVisualLineのキャッシュをdirtyする。
        /// </summary>
        public void DirtyAllVisLinesAfter(Block block) {
            if (_visualLineCache != null) {
                var stext = block.Parent as StyledText;
                var found = false;
                foreach (var bl in stext.Blocks) {
                    if (bl == block) {
                        found = true;
                    }
                    if (found) {
                        foreach (var l in bl.LineSegments) {
                            _visualLineCache.Dirty(l);
                        }
                    }
                }
            }
        }

        public void DirtySizeAndVisLine(Block block) {
            foreach (var line in block.LineSegments) {
                _visualLineCache.Dirty(line);
                _sizeCache.Dirty(line);
                foreach (var inline in line._Inlines) {
                    _sizeCache.Dirty(inline);
                }
            }
            _sizeCache.Dirty(block);
        }

        public void DirtyAllBounds() {
            _boundsCache.DirtyAll();
        }

        /// <summary>
        /// block以降のすべてのblockと配下のlineのbounds cacheをdirtyする．
        /// </summary>
        public void DirtyAllBoundsAfter(Block block) {
            var stext = block.Parent as StyledText;
            var found = false;
            foreach (var bl in stext.Blocks) {
                if (bl == block) {
                    found = true;
                }
                if (found) {
                    _boundsCache.Dirty(bl);
                    foreach (var l in bl.LineSegments) {
                        _boundsCache.Dirty(l);
                    }
                }
            }
        }

        public void DirtyAllBoundsBefore(Block block) {
            var stext = block.Parent as StyledText;
            foreach (var bl in stext.Blocks) {
                _boundsCache.Dirty(bl);
                foreach (var l in bl.LineSegments) {
                    _boundsCache.Dirty(l);
                }
                if (bl == block) {
                    break;
                }
            }
        }

        public void DirtyBounds(Block block) {
            _boundsCache.Dirty(block);
            foreach (var l in block.LineSegments) {
                _boundsCache.Dirty(l);
            }
        }

        public void UpdateBoundsAfter(Block block, Func<Rectangle, Rectangle> blockUpdater, Func<Rectangle, Rectangle> lineUpdater) {
            var stext = block.Parent as StyledText;
            var found = false;
            foreach (var bl in stext.Blocks) {
                if (bl == block) {
                    found = true;
                }
                if (found) {
                    _boundsCache.Update(bl, blockUpdater);
                    foreach (var l in bl.LineSegments) {
                        _boundsCache.Update(l, lineUpdater);
                    }
                }
            }
        }

        // --- info ---
        public Block GetBlockAt(Graphics g, Point location) {
            foreach (var block in _styledText.Blocks) {
                var bounds = _boundsCache.GetBounds(g, block);
                if (bounds.Contains(location)) {
                    return block;
                }
            }
            return null;
        }

        public LineSegment GetLineSegmentAt(Graphics g, Point location) {
            var block = GetBlockAt(g, location);
            if (block == null) {
                return null;
            }

            var lines = block.LineSegments;
            foreach (var line in lines) {
                var bounds = _boundsCache.GetBounds(g, line);
                if (bounds.Contains(location)) {
                    return line;
                }
            }
            return null;
        }

        public Inline GetInlineAt(Point location) {
            using (var con = GetGraphicsUsingContext()) {
                var g = con.Graphics;
                var line = GetLineSegmentAt(g, location);
                if (line == null) {
                    return null;
                }
    
                var lineBounds = _boundsCache.GetBounds(g, line);
                var visLines = _visualLineCache.GetVisualLines(g, line);
                foreach (var visLine in visLines) {
                    var r = new Rectangle(
                        lineBounds.Left + visLine.Bounds.Left,
                        lineBounds.Top + visLine.Bounds.Top,
                        visLine.Bounds.Width,
                        visLine.Bounds.Height
                    );
                    if (r.Contains(location)) {
                        var cLeft = r.Left;
                        foreach (var visInline in visLine.VisualInlines) {
                            var ir = new Rectangle(
                                cLeft,
                                r.Top,
                                visInline.Size.Width,
                                r.Height
                            );
                            if (ir.Contains(location)) {
                                return visInline.Inline;
                            }
                            cLeft += visInline.Size.Width;
                        }
                    }
                }
            }

            return null;
        }

        public int GetCharIndexAt(Point location) {
            using (var con = GetGraphicsUsingContext()) {
                var g = con.Graphics;

                /// blockの検索
                var blockRect = Rectangle.Empty;
                var block = default(Block);
                {
                    var firstBlock = _styledText.Blocks.First();
                    var firstBlockRect = GetBlockBounds(g, firstBlock);
                    if (location.Y < firstBlockRect.Top) {
                        block = firstBlock;
                        blockRect = firstBlockRect;
                    } else {
                        var lastBlock = _styledText.Blocks.Last();
                        var lastBlockRect = GetBlockBounds(g, lastBlock);
                        if (location.Y > lastBlockRect.Bottom) {
                            block = lastBlock;
                            blockRect = lastBlockRect;
                        } else {
                            /// block間にMarginがある場合(Figure内にblockに含まれない点がある場合)，
                            /// このコードでは見つけられない
                            /// 今はPaddingだけなので大丈夫
                            block = _styledText.Blocks.Find(
                                b => {
                                    blockRect = GetBlockBounds(g, b);
                                    return location.Y >= blockRect.Top && location.Y <= blockRect.Bottom;
                                }
                            );
                        }
                    }
                    Contract.Ensures(block != null);
                }
    
                /// line segmentの検索
                var lines = block.LineSegments;
                var line = default(LineSegment);
                {
                    var firstLineRect = _boundsCache.GetBounds(g, lines.First());
                    if (location.Y < firstLineRect.Top) {
                        line = lines.First();
                    } else {
                        var lastLineIndex = lines.Count() - 1;
                        var lastLineRect = _boundsCache.GetBounds(g, lines.ElementAt(lastLineIndex));
                        if (location.Y > lastLineRect.Bottom) {
                            line = lines.Last();
                        } else {
                            /// 行の間にはスペースはないので行の間を指してしまうことを考えなくてよい
                            /// LineSegment.LineSpaceは行そのもの余白の大きさを変える
                            line = lines.Find(
                                l => {
                                    var rect = _boundsCache.GetBounds(g, l);
                                    return
                                        location.Y >= rect.Top &&
                                        location.Y <= rect.Bottom;
                                }
                            );
                        }
                    }
                    Contract.Ensures(line != null);
                }
    
                var lineIndex = lines.IndexOf(line);
                var lineBounds = _boundsCache.GetBounds(g, line);
    
                /// visual lineの検索
                var visLines = _visualLineCache.GetVisualLines(g, line);
                var visLine = default(VisualLine);
                {
                    var firstVisLine = visLines[0];
                    if (location.Y < firstVisLine.Bounds.Top + lineBounds.Top) {
                        visLine = firstVisLine;
                    } else {
                        var lastVisLine = visLines[visLines.Length - 1];
                        if (location.Y > lastVisLine.Bounds.Bottom + lineBounds.Top) {
                            visLine = lastVisLine;
                        } else {
                            visLine = visLines.Find(
                                vl => {
                                    return
                                        location.Y >= vl.Bounds.Top + lineBounds.Top &&
                                        location.Y <= vl.Bounds.Bottom + lineBounds.Top;
                                }
                            );
                        }
                    }
                    Contract.Ensures(visLine != null);
                }
    
                var blockOffset = _styledText.GetBlockOffset(block);
                var lineOffset = block.GetLineSegmentLocalOffset(lineIndex) + blockOffset;
    
                /// charの検索
                if (location.X < visLine.Bounds.Left + lineBounds.Left) {
                    return lineOffset + visLine.CharOffset;
                } else {
                    if (location.X > visLine.Bounds.Right + lineBounds.Left) {
                        if (visLine.LineIndex == visLines.Length - 1) {
                            return lineOffset + visLine.CharOffset + visLine.CharLength - 1;
                        } else {
                            return lineOffset + visLine.CharOffset + visLine.CharLength;
                        }
                    } else {
    
                        var cLeft = lineBounds.Left;
                        var cIndexInLine = visLine.CharOffset;
                        foreach (var visInline in visLine.VisualInlines) {
                            if (location.X >= cLeft && location.X <= cLeft + visInline.Size.Width) {
                                int drawable;
                                using (var font = visInline.Font.CreateFont()) {
                                    MeasureText(g, visInline.Text, font, location.X - cLeft, out drawable);
                                }
                                if (drawable == visInline.Text.Length) {
                                    var indexInLine = cIndexInLine + visInline.Text.Length;
                                    if (line.Length == indexInLine) {
                                        /// ちょうどBlockBreak/LineBreakをポイントしたときは一文字前の位置を返す
                                        return lineOffset + indexInLine - 1;
                                    } else {
                                        return lineOffset + indexInLine;
                                    }
                                } else {
                                    var rect = _blockRenderer.GetCharRectInVisualLine(
                                        g, visLine, cIndexInLine + drawable
                                    );
                                    var center = lineBounds.Left + rect.Left + rect.Width / 2;
                                    //if (location.X >= lineBounds.Left + rect.Left && location.X <= center) {
                                    if (location.X <= center) {
                                        return lineOffset + cIndexInLine + drawable;
                                    //} else if (location.X >= center && location.X <= lineBounds.Left + rect.Right) {
                                    } else {
                                        /// 行末よりうしろに行かないようにminを取る
                                        return Math.Min(
                                            lineOffset + cIndexInLine + drawable + 1,
                                            lineOffset + visLine.CharOffset + visLine.CharLength - 1
                                        );
                                    }
                                }
                            }
                            cIndexInLine += visInline.Text.Length;
                            cLeft += visInline.Size.Width;
                        }
                    }
                }
                return _styledText.Length - 1;
            }

            //throw new InvalidOperationException("char index not found");
        }

        public Rectangle GetStyledTextBoundsFor(Rectangle bounds) {
            if (Root == null || Root.Canvas == null || Root.Canvas.IsDisposed || _styledText == null) {
                return Rectangle.Empty;
            }
            using (var context = GetGraphicsUsingContext()) {
                if (context == null) {
                    return Rectangle.Empty;
                }
                var g = context.Graphics;
                return GetStyledTextBoundsFor(g, bounds);
            }
        }

        public Rectangle GetStyledTextBoundsFor(Graphics g, Rectangle bounds) {
            /// 与えられたboundsに対しての処理なので_boundsCacheは使えない．
            /// _sizeCacheは使ってもよい．
            var ret = Rectangle.Empty;
            var first = true;
            var bTop = 0;
            foreach (var block in _styledText.Blocks) {
                var blockSize = _sizeCache.GetSize(g, block);

                var bLeft = 0;
                switch (block.HorizontalAlignment) {
                    case DataType::HorizontalAlignment.Left: {
                        bLeft = bounds.Left;
                        break;
                    }
                    case DataType::HorizontalAlignment.Center: {
                        bLeft = bounds.Left + (bounds.Width - blockSize.Width) / 2;
                        break;
                    }
                    case DataType::HorizontalAlignment.Right: {
                        bLeft = bounds.Left + (bounds.Width - blockSize.Width);
                        break;
                    }
                };
                var blockBounds = new Rectangle(
                    bLeft,
                    bTop,
                    blockSize.Width,
                    blockSize.Height
                );

                if (first) {
                    first = false;
                    ret = blockBounds;
                } else {
                    ret = Rectangle.Union(ret, blockBounds);
                }

                bTop += blockSize.Height;
            }

            var top = 0;
            switch (_styledText.VerticalAlignment) {
                case VerticalAlignment.Top: {
                    top = bounds.Top;
                    break;
                }
                case VerticalAlignment.Center: {
                    top = (int) (RectUtil.GetCenterF(bounds).Y - (double) ret.Height / 2);
                    break;
                }
                case VerticalAlignment.Bottom: {
                    top = bounds.Bottom - ret.Height;
                    break;
                }
            }

            return new Rectangle(ret.Left, top, ret.Width, ret.Height);
        }

        public Rectangle GetBoundsWithText() {
            return GetBoundsWithTextFor(Bounds);
        }

        public Rectangle GetBlockBounds(Graphics g, int blockIndex) {
            var block = _styledText.Blocks.ElementAt(blockIndex);
            return GetBlockBounds(g, block);
        }

        public Rectangle GetBlockBounds(Graphics g, Block block) {
            return _boundsCache.GetBounds(g, block);
        }

        public Rectangle GetLineSegmentBounds(Graphics g, LineSegment line) {
            return _boundsCache.GetBounds(g, line);
        }

        public Rectangle GetCharRect(int charIndex) {
            using (var con = GetGraphicsUsingContext()) {
                var g = con.Graphics;
                int charIndexInLine, lineSegOffset;
                var line = _styledText.GetLineSegmentAt(charIndex, out charIndexInLine, out lineSegOffset);
                var lineRect = _boundsCache.GetBounds(g, line);
                var charRectInLine = _blockRenderer.GetCharRectInLine(g, line, charIndexInLine);
                return new Rectangle(
                    charRectInLine.Left + lineRect.Left,
                    charRectInLine.Top + lineRect.Top,
                    charRectInLine.Width,
                    charRectInLine.Height
                );
            }
        }

        /// <summary>
        /// charIndex番目から長さlength文の文字列を囲む矩形の配列を返す．
        /// </summary>
        public Rectangle[] GetStringRect(int charIndex, int length) {
            using (var con = GetGraphicsUsingContext()) {
                var g = con.Graphics;
                var endCharIndex = charIndex + length - 1;
    
                int startBlockIndex;
                int charIndexInStartBlock;
                var startBlock = _styledText.GetBlockAtLocal(charIndex, out startBlockIndex, out charIndexInStartBlock);
                
                int endBlockIndex;
                int charIndexInEndBlock;
                var endBlock = _styledText.GetBlockAtLocal(endCharIndex, out endBlockIndex, out charIndexInEndBlock);
    
                if (startBlock == endBlock) {
                    /// 最初のblockと最後のblockが同じなら
    
                    var firstBlockRect = GetBlockBounds(g, startBlock);
                    var rects = _blockRenderer.GetStringRect(
                        g, startBlock, charIndexInStartBlock, length
                    );
    
                    return TranslateRectsInBlockToRoot(rects, firstBlockRect);
    
                } else {
                    /// 最初のblockと最後のblockが同じでないなら
    
                    var ret = new List<Rectangle>();
    
                    /// 最初のblock
                    var firstBlockRect = GetBlockBounds(g, startBlock);
                    var firstBlock = startBlock;
                    var rectsInFirstBlock = _blockRenderer.GetStringRect(
                        g, firstBlock, charIndexInStartBlock, firstBlock.Length - charIndexInStartBlock
                    );
                    ret.AddRange(TranslateRectsInBlockToRoot(rectsInFirstBlock, firstBlockRect));
    
                    /// 間のblock
                    var len = endBlockIndex - startBlockIndex - 1;
                    foreach (var block in _styledText.Blocks.Range(startBlockIndex + 1, len)) {
                        var blockRect = GetBlockBounds(g, block);
                        var rectsInBlock = _blockRenderer.GetStringRect(g, block, 0, block.Length);
                        ret.AddRange(TranslateRectsInBlockToRoot(rectsInBlock, blockRect));
                    }
    
                    /// 最後のblock
                    var lastBlockRect = GetBlockBounds(g, endBlockIndex);
                    var lastBlock = endBlock;
                    var rectsInLastBlock = _blockRenderer.GetStringRect(g, lastBlock, 0, charIndexInEndBlock + 1);
                    ret.AddRange(TranslateRectsInBlockToRoot(rectsInLastBlock, lastBlockRect));
    
                    return ret.ToArray();
    
                }
            }
        }

        public Size GetBlockSize(Block block) {
            using (var con = GetGraphicsUsingContext()) {
                return _sizeCache.GetSize(con.Graphics, block);
            }
        }

        public Size GetLineSegmentSize(Graphics g, LineSegment line) {
            return _sizeCache.GetSize(g, line);
        }

        public Size GetInlineSize(Graphics g, Inline inline) {
            return _sizeCache.GetSize(g, inline);
        }

        /// <summary>
        /// charIndexを含むVisualLineの表示領域を返す．
        /// </summary>
        public Rectangle GetVisualLineBounds(int charIndex) {
            using (var con = GetGraphicsUsingContext()) {
                var g = con.Graphics;
                int charIndexInLine, lineOffset;
                var line = _styledText.GetLineSegmentAt(charIndex, out charIndexInLine, out lineOffset);
                var lineBounds = _boundsCache.GetBounds(g, line);
                var visLines = _visualLineCache.GetVisualLines(g, line);

                foreach (var visLine in visLines) {
                    if (charIndex >= lineOffset + visLine.CharOffset && charIndex < lineOffset + visLine.CharOffset + visLine.CharLength) {
                        return new Rectangle(
                            visLine.Bounds.Location + (Size) lineBounds.Location,
                            visLine.Bounds.Size
                        );
                    }
                }
            }

            throw new ArgumentException("charIndex");
        }

        /// <summary>
        /// charIndexを含むVisualLineの文字範囲を返す．
        /// </summary>
        public Range GetVisualLineRange(int charIndex) {
            using (var con = GetGraphicsUsingContext()) {
                var g = con.Graphics;
                int charIndexInLine, lineOffset;
                var line = _styledText.GetLineSegmentAt(charIndex, out charIndexInLine, out lineOffset);
                var visLines = _visualLineCache.GetVisualLines(g, line);

                foreach (var visLine in visLines) {
                    if (charIndex >= lineOffset + visLine.CharOffset && charIndex < lineOffset + visLine.CharOffset + visLine.CharLength) {
                        return new Range(lineOffset + visLine.CharOffset, visLine.CharLength);
                    }
                }
            }

            throw new ArgumentException("charIndex");
        }

        public bool IsVisualLineHead(int charIndex) {
            using (var con = GetGraphicsUsingContext()) {
                var g = con.Graphics;
                int charIndexInLine, lineOffset;
                var line = _styledText.GetLineSegmentAt(charIndex, out charIndexInLine, out lineOffset);
                var visLines = _visualLineCache.GetVisualLines(g, line);

                foreach (var visLine in visLines) {
                    if (charIndex >= lineOffset + visLine.CharOffset && charIndex < lineOffset + visLine.CharOffset + visLine.CharLength) {
                        return charIndex == lineOffset + visLine.CharOffset;
                    }
                }

                return false;
            }
            //throw new ArgumentException("charIndex");
        }

        public bool IsVisualLineEnd(Graphics g, int charIndex) {
            int charIndexInLine, lineOffset;
            var line = _styledText.GetLineSegmentAt(charIndex, out charIndexInLine, out lineOffset);
            var visLines = _visualLineCache.GetVisualLines(g, line);

            foreach (var visLine in visLines) {
                if (charIndex >= lineOffset + visLine.CharOffset && charIndex < lineOffset + visLine.CharOffset + visLine.CharLength) {
                    return charIndex == lineOffset + visLine.CharOffset + visLine.CharLength - 1;
                }
            }

            throw new ArgumentException("charIndex");
        }

        public bool IsFirstVisualLine(int charIndex) {
            using (var con = GetGraphicsUsingContext()) {
                var g = con.Graphics;
                var line = _styledText.LineSegments.First();
                var lineOffset = 0;
                if (charIndex >= lineOffset && charIndex < lineOffset + line.Length) {
                    var visLines = _visualLineCache.GetVisualLines(g, line);
                    var firstVisLine = visLines.First();
                    return
                        charIndex >= lineOffset + firstVisLine.CharOffset &&
                        charIndex < lineOffset + firstVisLine.CharOffset + firstVisLine.CharLength;
                } else {
                    return false;
                }
            }

        }

        public bool IsLastVisualLine(int charIndex) {
            using (var con = GetGraphicsUsingContext()) {
                var g = con.Graphics;
                var line = _styledText.LineSegments.Last();
                var lineOffset = _styledText.GetLineSegmentOffset(line);
                if (charIndex >= lineOffset && charIndex < lineOffset + line.Length) {
                    var visLines = _visualLineCache.GetVisualLines(g, line);
                    var lastVisLine = visLines.Last();
                    return
                        charIndex >= lineOffset + lastVisLine.CharOffset &&
                        charIndex < lineOffset + lastVisLine.CharOffset + lastVisLine.CharLength;
                } else {
                    return false;
                }
            }
        }

        public void GetFirstVisibleVisualLine(Graphics g) {
            var bounds = GetVisibleBounds();
            if (bounds.IsEmpty) {
                return;
            }
        }

        public Point? GetConnectionPoint(object option) {
            var anchorId = option as string;
            if (anchorId == null) {
                return null;
            }

            var found = _styledText.Inlines.Find(
                inline => {
                    if (inline.IsAnchorCharacter) {
                        var anchor = inline as Anchor;
                        if (string.Equals(anchorId, anchor.Id, StringComparison.OrdinalIgnoreCase)) {
                            return true;
                        }
                    }
                    return false;
                }
            );
            if (found == null) {
                return null;
            }

            var range = _styledText.GetRange(found);
            var rect = GetCharRect(range.Offset);
            return new Point(rect.Left + rect.Width / 2, rect.Bottom);
        }

        public Rectangle GetBulletRect(Graphics g, Block block) {
            Contract.Requires(block != null);

            var firstLine = block.LineSegments.First();
            var firstLineBounds = _boundsCache.GetBounds(g, firstLine);
            var visLines = _visualLineCache.GetVisualLines(g, firstLine);
            var firstVisLine = visLines[0];
            return new Rectangle(
                firstLineBounds.Left + firstVisLine.Bounds.Left - 20,
                firstLineBounds.Top + firstVisLine.Bounds.Top + block.LineSpace,
                20,
                firstVisLine.Bounds.Height - block.LineSpace
            );
        }

        public bool IsInSelection(Point loc) {
            if (_selection.IsEmpty) {
                return false;
            }

            var rects = GetStringRect(_selection.Offset, _selection.Length);
            foreach (var rect in rects) {
                if (rect.Contains(loc)) {
                    return true;
                }
            }

            return false;
        }

        public bool IsInBullet(Point loc) {
            using (var context = GetGraphicsUsingContext()) {
                var g = context.Graphics;

                var para = GetBlockAt(g, loc) as Paragraph;
                if (para != null && para.ListKind != ListKind.None) {
                    var bulRect = GetBulletRect(g, para);
                    if (bulRect.Contains(loc)) {
                        return true;
                    }
                }
                return false;
            }
        }

        public bool IsInBullet(Point loc, ListKind listKind) {
            using (var context = GetGraphicsUsingContext()) {
                var g = context.Graphics;

                var para = GetBlockAt(g, loc) as Paragraph;
                if (para != null && EnumUtil.HasAnyFlags((int) para.ListKind, (int) listKind)) {
                    var bulRect = GetBulletRect(g, para);
                    if (bulRect.Contains(loc)) {
                        return true;
                    }
                }
                return false;
            }
        }

        public ICommand GetProcessCheckBoxBulletCommand(Point location) {
            using (var context = GetGraphicsUsingContext()) {
                var g = context.Graphics;
             
                var para = GetBlockAt(g, location) as Paragraph;
                if (para != null && (para.ListKind == ListKind.CheckBox || para.ListKind == ListKind.TriStateCheckBox)) {
                    var bulRect = GetBulletRect(g, para);
                    if (bulRect.Contains(location)) {
                        return new SetParagraphPropertiesCommand(
                            para,
                            para.Padding,
                            para.LineSpace,
                            para.HorizontalAlignment,
                            para.ParagraphKind,
                            para.ListKind,
                            para.ListLevel,
                            para.GetNextListState()
                        );
                    }
                }

                return null;
            }
        }
        //public bool IsOnBullet(Graphics g, Point location, out Block block) {
        //    block = GetBlockAt(g, location);
        //    if (block == null) {
        //        return false;
        //    }

        //    var r = GetBulletRect(g, block);
        //    return r.Contains(location);
        //}

        // ------------------------------
        // protected
        // ------------------------------
        public override void InvalidatePaint() {
            var tSize = TextSize;
            var stSize = StyledTextSize;
            var textSize = new Size(
                Math.Max(tSize.Width, stSize.Width),
                Math.Max(tSize.Height, stSize.Height)
            );

            var bounds = PaintBounds;

            if (bounds.Size.Width >= textSize.Width && bounds.Size.Height >= textSize.Height) {
                base.InvalidatePaint();
            } else {
                var r = GetBoundsWithTextFor(bounds);
                r.Inflate(Padding.Size);
                DirtManager.DirtyPaint(r);
            }
        }

        protected Rectangle GetBoundsWithTextFor(Rectangle bounds) {
            var ret = bounds;

            var textSize = TextSize;
            var stSize = StyledTextSize;
            var size = new Size(
                Math.Max(textSize.Width, stSize.Width),
                Math.Max(textSize.Height, stSize.Height)
            );
            var diff = size - ret.Size;
            var isize = new Size(Math.Max(diff.Width / 2, 0), Math.Max(diff.Height / 2, 0));
            ret.Inflate(isize);

            return ret;
        }

        protected virtual void PaintSelection(Graphics g) {
            if (_styledText != null) {
                if (!_selection.IsEmpty) {
                    var rects = GetStringRect(_selection.Offset, _selection.Length);
                    foreach (var rect in rects) {
                        if (g.ClipBounds.IntersectsWith(rect)) {
                            var r = rect;
                            r.Inflate(-1, -1);
                            g.FillRectangle(
                                _SelectionBrushResource,
                                rect.Left, rect.Top, rect.Width - 1, rect.Height - 1
                            );
                            g.DrawRectangle(
                                Pens.White,
                                r.Left, r.Top, r.Width - 1, r.Height - 1
                            );
                            g.DrawRectangle(
                                _SelectionPenResource,
                                rect.Left, rect.Top, rect.Width - 1, rect.Height - 1
                            );
                        }
                    }
                }
            }
        }

        protected virtual void PaintStyledText(Graphics g) {
            if (_styledText != null) {
                foreach (var block in _styledText.Blocks) {
                    var blockRect = _boundsCache.GetBounds(g, block);
                    if (g.ClipBounds.IntersectsWith(blockRect)) {
                        PaintBlock(g, block);
                    }
                }
            }
        }

        protected override Size MeasureSelf(SizeConstraint constraint) {
            if (_styledText == null && string.IsNullOrEmpty(_text)) {
                /// サイズを持つものが何もないのでEmpty
                return Size.Empty;
            }

            if (Root == null || Root.Canvas == null || Root.Canvas.IsDisposed) {
                /// 測れないので現状維持
                return constraint.MeasureConstrainedSize(Size);
                //return Size.Empty;
            }

            if (!IsVisible) {
                /// 現状維持
                return constraint.MeasureConstrainedSize(Size);
                //return Size.Empty;
            }

            using (_ResourceCache.UseResource()) {
                var cliRect = Padding.GetClientArea(Bounds);
                int? width = null;
                int? height = null;

                if (_styledText != null || _text != null) {
                    using (var context = GetGraphicsUsingContext()) {
                        if (context == null) {
                            return constraint.MeasureConstrainedSize(Size);
                        }
                        var g = context.Graphics;

                        /// StyledTextのサイズ
                        if (_styledText != null) {
                            /// BlockのサイズをUnion
                            var stRect = default(Rectangle);
                            var first = true;
                            foreach (var block in _styledText.Blocks) {
                                if (first) {
                                    stRect = GetBlockBounds(g, block);
                                    first = false;
                                } else {
                                    stRect = Rectangle.Union(stRect, GetBlockBounds(g, block));
                                }
                            }
                            if (!first) {
                                width = stRect.Width + Padding.Width;
                                height = stRect.Height + Padding.Height;
                            }
                        }

                        /// PlainTextのサイズ
                        if (string.IsNullOrEmpty(_text)) {
                            var size = TextSize + Padding.Size;
                            width = width.HasValue ? Math.Max(width.Value, size.Width) : size.Width;
                            height = height.HasValue ? Math.Max(height.Value, size.Height) : size.Height;
                        }
    
                    }
                }

                if (width.HasValue && height.HasValue) {
                    return constraint.MeasureConstrainedSize(new Size(width.Value, height.Value));
                } else {
                    return base.MeasureSelf(constraint);
                }
            }
        }


        // ------------------------------
        // private
        // -----------------------------
        // === StyledTextFigure ==========
        // --- paint ---
        /// <summary>
        /// blockを表示し，描画した領域を返す．
        /// </summary>
        private void PaintBlock(Graphics g, Block block) {
            if (block is Paragraph) {
                var para = block as Paragraph;
                if (para.ListKind != ListKind.None && !para.IsEmpty) {
                    switch (para.ListKind) {
                        case ListKind.Ordered:
                            PaintNumberBullet(g, para);
                            break;
                        case ListKind.Unordered:
                            PaintBullet(g, para);
                            break;
                        case ListKind.CheckBox:
                        case ListKind.TriStateCheckBox:
                            PaintCheckBoxBullet(g, para);
                            break;
                        case ListKind.Star:
                            PaintStarBullet(g, para);
                            break;
                        case ListKind.LeftArrow:
                            PaintLeftArrowBullet(g, para);
                            break;
                        case ListKind.RightArrow:
                            PaintRightArrowBullet(g, para);
                            break;
                    }
                }
            }
            foreach (var line in block.LineSegments) {
                var lineBounds = _boundsCache.GetBounds(g, line);
                if (g.ClipBounds.IntersectsWith(lineBounds)) {
                    PaintLine(g, line);
                }
            }
        }

        /// <summary>
        /// paraの行頭文字を描画する．
        /// </summary>
        private void PaintBullet(Graphics g, Paragraph para) {
            var r = GetBulletRect(g, para);
            var font = default(Font);
            var bullet = "";
            switch (para.ListLevel % 4) {
                case 0: {
                    font = _fontCache.GetFont(new FontDescription("Wingdings", 6));
                    bullet = "l";
                    break;
                }
                case 1: {
                    font = _fontCache.GetFont(new FontDescription("Wingdings", 6));
                    bullet = "¡";
                    break;
                }
                case 2: {
                    font = _fontCache.GetFont(new FontDescription("Wingdings", 6));
                    bullet = "n";
                    break;
                }
                case 3: {
                    font = _fontCache.GetFont(new FontDescription("Wingdings", 6));
                    bullet = "o";
                    break;
                }
            }
            if (font != null && !string.IsNullOrEmpty(bullet)) {
                DrawTextCenter(g, bullet, font, r, para.Color);
            }
        }

        /// <summary>
        /// paraの行頭文字を描画する．
        /// </summary>
        private void PaintCheckBoxBullet(Graphics g, Paragraph para) {
            if (para.ListState == ListStateKind.Checked) {
                PaintImageBullet(g, para, Resources.checkbox_checked);
            } else if (para.ListState == ListStateKind.Indeterminate) {
                PaintImageBullet(g, para, Resources.checkbox_indeterminate);
            //} else {
            } else if (para.ListState == ListStateKind.Unchecked) {
                PaintImageBullet(g, para, Resources.checkbox_unchecked);
            }
        }

        private void PaintStarBullet(Graphics g, Paragraph para) {
            PaintImageBullet(g, para, Resources.star);
        }

        private void PaintLeftArrowBullet(Graphics g, Paragraph para) {
            PaintImageBullet(g, para, Resources.left_arrow);
        }

        private void PaintRightArrowBullet(Graphics g, Paragraph para) {
            PaintImageBullet(g, para, Resources.right_arrow);
        }

        /// <summary>
        /// paraの行頭文字を描画する．
        /// </summary>
        private void PaintImageBullet(Graphics g, Paragraph para, Image image) {
            var r = GetBulletRect(g, para);
            var imageSize = image.Size;
            var imageRect = new Rectangle(
                r.Left + (r.Width - imageSize.Width) / 2,
                r.Top + (r.Height - imageSize.Height) / 2,
                imageSize.Width,
                imageSize.Height
            );
            g.DrawImage(image, imageRect);
        }



        /// <summary>
        /// paraの行頭文字を描画する．
        /// </summary>
        private void PaintNumberBullet(Graphics g, Paragraph para) {
            var firstLine = para.LineSegments.First();
            var firstLineBounds = _boundsCache.GetBounds(g, firstLine);
            var visLines = _visualLineCache.GetVisualLines(g, firstLine);
            var firstVisLine = visLines[0];
            var r = new Rectangle(
                firstLineBounds.Left + firstVisLine.Bounds.Left - 20,
                firstLineBounds.Top + firstVisLine.Bounds.Top + para.LineSpace,
                20,
                firstVisLine.Bounds.Height - para.LineSpace
            );
            var font = _fontCache.GetFont(para.Font);
            var bullet = (para.ListIndex + 1).ToString() + ".";
            DrawTextCenter(g, bullet, font, r, para.Color);
        }

        /// <summary>
        ///  lineをlineBoundsに描画し，描画した領域を返す．
        /// </summary>
        private void PaintLine(Graphics g, LineSegment line) {
            var lineBounds = _boundsCache.GetBounds(g, line);
            var visLines = _visualLineCache.GetVisualLines(g, line);
            foreach (var visLine in visLines) {
                var visLineBounds = visLine.Bounds;
                var cLeft = lineBounds.Left + visLineBounds.Left;
                foreach (var visInline in visLine.VisualInlines) {
                    var cTop = lineBounds.Top + visLineBounds.Top + (visLineBounds.Height - visInline.Size.Height);
                    var cRect = new Rectangle(cLeft, cTop, visInline.Size.Width, visInline.Size.Height);
                    if (visInline.Inline.IsAnchorCharacter) {
                        var anc = Resources.anchor;
                        //g.DrawImage(anc, new Point(cRect.Left, cRect.Top + (cRect.Height - anc.Height)));
                        g.DrawImage(anc, new Rectangle(cRect.Left, cRect.Top + (cRect.Height - anc.Height), anc.Width, anc.Height));

                    } else if (!PreventPaintingLineBreakAndBlockBreak && _showLineBreak && visInline.Inline.IsLineBreakCharacter) {
                        var lb = Resources.line_break;
                        //g.DrawImage(lb, new Point(cRect.Left, cRect.Top + (cRect.Height - lb.Height) / 2));
                        g.DrawImage(lb, new Rectangle(cRect.Left, cRect.Top + (cRect.Height - lb.Height) / 2, lb.Width, lb.Height));

                    } else if (!PreventPaintingLineBreakAndBlockBreak && _showBlockBreak && visInline.Inline.IsBlockBreakCharacter) {
                        var bb = Resources.block_break;
                        //g.DrawImage(bb, new Point(cRect.Left, cRect.Top + (cRect.Height - bb.Height) / 2));
                        g.DrawImage(bb, new Rectangle(cRect.Left, cRect.Top + (cRect.Height - bb.Height) / 2, bb.Width, bb.Height));

                    } else if (!visInline.Inline.IsControlCharacter && !string.IsNullOrEmpty(visInline.Text)) {
                        if (visInline.BackColor.HasValue) {
                            /// selectionと被る場合は描画しない
                            using (var brush = new SolidBrush(visInline.BackColor.Value)) {
                                var r = new Rectangle(cRect.Left, cRect.Top - 1, cRect.Width - 1, cRect.Height - 1);
                                g.FillRectangle(brush, r);
                            }
                        }
                        DrawText(
                            g, visInline.Text, _fontCache.GetFont(visInline.Font),
                            cRect, visInline.ForeColor
                        );
                    }
                    cLeft += visInline.Size.Width;
                }
            }
        }

        // ------------------------------
        // protected
        // ------------------------------
        /// <summary>
        /// ClientRect内でblockRectの矩形であるBlock中のrectsをRootFigure内の座標に変換する．
        /// </summary>
        private Rectangle[] TranslateRectsInBlockToRoot(Rectangle[] rects, Rectangle blockRect) {
            var ret = new Rectangle[rects.Length];

            for (int i = 0, len = rects.Length; i < len; ++i) {
                var rect = rects[i];
                ret[i] = new Rectangle(
                    rect.Left + blockRect.Left,
                    rect.Top + blockRect.Top,
                    rect.Width,
                    rect.Height
                );
            }

            return ret;
        }

        // --- handler ---
        private void HandleStyledTextContentsChanged(object sender, ContentsChangedEventArgs e) {
            if (Root == null || Root.Canvas == null || Root.Canvas.IsDisposed) {
                return;
            }

            if (InUpdatingStyledText) {
                /// 自動Dirtyをしない
                return;
            }

            _styledTextSizeCache = Size.Empty;

            // todo: Dirty()，InvalidatePaint()の範囲をできるだけ狭くする
            var changed = e.ChangedContent;
            if (changed is Inline) {
                var inline = changed as Inline;
                var line = inline.Parent as LineSegment;
                var block = line.Parent as Block;

                _sizeCache.Dirty(inline);
                _visualLineCache.Dirty(line);

                using (var context = GetGraphicsUsingContext()) {
                    if (context == null) {
                        return;
                    }
                    var g = context.Graphics;

                    var oldLineBounds = _boundsCache.GetBounds(g, line);

                    var oldLineSize = _sizeCache.GetSize(g, line);
                    var oldBlockSize = _sizeCache.GetSize(g, block);
                    _sizeCache.Dirty(line);
                    _sizeCache.Dirty(block);
                    var newLineSize = _sizeCache.GetSize(g, line);
                    var newBlockSize = _sizeCache.GetSize(g, block);

                    switch (_styledText.VerticalAlignment) {
                        case VerticalAlignment.Top: {
                            if (oldLineSize.Height != newLineSize.Height) {
                                DirtyAllBoundsAfter(block);
                            } else {
                                if (oldLineSize.Width != newLineSize.Width) {
                                    _boundsCache.Dirty(line);
                                }
                                if (oldBlockSize.Height != newBlockSize.Height) {
                                    DirtyAllBoundsAfter(block);
                                } else if (oldBlockSize.Width != newBlockSize.Width) {
                                    _boundsCache.Dirty(block);
                                }
                            }
                            break;
                        }
                        case VerticalAlignment.Center: {
                            if (oldLineSize.Height != newLineSize.Height) {
                                _boundsCache.DirtyAll();
                            } else {
                                if (oldLineSize.Width != newLineSize.Width) {
                                    _boundsCache.Dirty(line);
                                }
                                if (oldBlockSize.Height != newBlockSize.Height) {
                                    _boundsCache.DirtyAll();
                                } else if (oldBlockSize.Width != newBlockSize.Width) {
                                    _boundsCache.Dirty(block);
                                }
                            }
                            break;
                        }
                        case VerticalAlignment.Bottom: {
                            if (oldLineSize.Height != newLineSize.Height) {
                                DirtyAllBoundsBefore(block);
                            } else {
                                if (oldLineSize.Width != newLineSize.Width) {
                                    _boundsCache.Dirty(line);
                                }
                                if (oldBlockSize.Height != newBlockSize.Height) {
                                    DirtyAllBoundsBefore(block);
                                } else if (oldBlockSize.Width != newBlockSize.Width) {
                                    _boundsCache.Dirty(block);
                                }
                            }
                            break;
                        }
                    }

                    if (inline.IsLineEndCharacter) {
                        InvalidatePaint();
                    } else {
                        if (oldLineSize.Height != newLineSize.Height) {
                            InvalidatePaint();
                        } else {
                            var bounds = Bounds;
                            var lineBounds = _boundsCache.GetBounds(g, line);
                            var invRect = new Rectangle(bounds.Left, lineBounds.Top, bounds.Width, lineBounds.Height);
                            InvalidatePaint(invRect);
                        }
                    }
                }


            } else if (changed is LineSegment) {
                var line = changed as LineSegment;
                var block = line.Parent as Block;
                var stext = block.Parent as StyledText;

                _visualLineCache.Dirty(line);
                _sizeCache.Dirty(line);
                _sizeCache.Dirty(block);

                switch (_styledText.VerticalAlignment) {
                    case VerticalAlignment.Top: {
                        DirtyAllBoundsAfter(block);
                        break;
                    }
                    case VerticalAlignment.Center: {
                        _boundsCache.DirtyAll();
                        break;
                    }
                    case VerticalAlignment.Bottom: {
                        DirtyAllBoundsBefore(block);
                        break;
                    }
                }

                InvalidatePaint();

            } else if (changed is Block) {
                var block = changed as Block;
                var stext = block.Parent as StyledText;

                foreach (var line in block.LineSegments) {
                    _visualLineCache.Dirty(line);
                    _sizeCache.Dirty(line);
                }
                _sizeCache.Dirty(block);

                
                switch (_styledText.VerticalAlignment) {
                    case VerticalAlignment.Top: {
                        DirtyAllBoundsAfter(block);
                        break;
                    }
                    case VerticalAlignment.Center: {
                        _boundsCache.DirtyAll();
                        break;
                    }
                    case VerticalAlignment.Bottom: {
                        DirtyAllBoundsBefore(block);
                        break;
                    }
                }

                InvalidatePaint();

            } else {
                _visualLineCache.DirtyAll();
                _sizeCache.DirtyAll();
                _boundsCache.DirtyAll();

                InvalidatePaint();
            }
        }

        /// <summary>
        /// _boundsCache以外から呼ばれてはならない．
        /// </summary>
        private Rectangle GetBlockRectCore(Graphics g, Block block) {
            var cliArea = ClientArea;
            var blockSize = _sizeCache.GetSize(g, block);

            /// Blocks内でのblockのtop計算
            var cTop = 0;
            foreach (var b in _styledText.Blocks) {
                var size = _sizeCache.GetSize(g, b);
                if (b == block) {
                    break;
                }
                cTop += size.Height;
            }
            var blockTop = cTop;


            /// blockのtop計算
            var top = 0;
            switch (_styledText.VerticalAlignment) {
                case VerticalAlignment.Top: {
                    top = blockTop + cliArea.Top;
                    break;
                }
                case VerticalAlignment.Center: {
                    var blocksHeight = _styledText.Blocks.Sum(b => _sizeCache.GetSize(g, b).Height);
                    top = blockTop + cliArea.Top + (cliArea.Height - blocksHeight) / 2;
                    break;
                }
                case VerticalAlignment.Bottom: {
                    var blocksHeight = _styledText.Blocks.Sum(b => _sizeCache.GetSize(g, b).Height);
                    top = blockTop + cliArea.Top + (cliArea.Height - blocksHeight);
                    break;
                }
                default: {
                    throw new InvalidOperationException("unknown vertical alignment");
                }
            }

            /// blockのleft計算
            var left = 0;
            switch (block.HorizontalAlignment) {
                case DataType::HorizontalAlignment.Left: {
                    left = cliArea.Left;
                    break;
                }
                case DataType::HorizontalAlignment.Center: {
                    left = (int) Math.Round(cliArea.Left + (double) (cliArea.Width - blockSize.Width) / 2);
                    break;
                }
                case DataType::HorizontalAlignment.Right: {
                    left = cliArea.Left + (cliArea.Width - blockSize.Width);
                    break;
                }
                default: {
                    throw new InvalidOperationException("unknown horizontal alignment");
                }
            };

            return new Rectangle(left, top, blockSize.Width, blockSize.Height);
        }

        /// <summary>
        /// _boundsCache以外から呼ばれてはならない．
        /// </summary>
        private Rectangle GetLineBoundsCore(Graphics g, LineSegment line) {
            var block = line.Parent as Block;
            var blockBounds = _boundsCache.GetBounds(g, block);

            var lineBounds = _blockRenderer.MeasureLineBounds(g, line);

            return new Rectangle(
                lineBounds.Left + blockBounds.Left,
                lineBounds.Top + blockBounds.Top,
                lineBounds.Width,
                lineBounds.Height
            );
        }

        /// <summary>
        /// _sizeCache以外から呼ばれてはならない．
        /// </summary>
        private Size GetBlockSizeCore(Graphics g, Block block) {
            return _blockRenderer.MeasureBlockSize(g, block);
        }

        /// <summary>
        /// _sizeCache以外から呼ばれてはならない．
        /// </summary>
        private Size GetLineSegmentSizeCore(Graphics g, LineSegment line) {
            return _blockRenderer.MeasureLineSegmentSize(g, line);
        }

        /// <summary>
        /// _sizeCache以外から呼ばれてはならない．
        /// </summary>
        private Size GetInlineSizeCore(Graphics g, Inline inline) {
            return _blockRenderer.MeasureInlineSize(g, inline);
        }


        /// <summary>
        /// _visualLineCache以外から呼ばれてはならない．
        /// </summary>
        private VisualLine[] GetVisualLinesCore(Graphics g, LineSegment line) {
            return _blockRenderer.MeasureVisualLines(g, line);
        }


    }
}
