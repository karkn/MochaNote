/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Common.String;
using Mkamo.Common.Collection;
using Mkamo.Common.Core;
using System.Windows.Forms;
using Mkamo.StyledText.Core;
using Mkamo.Common.Win32.Gdi32;

namespace Mkamo.Figure.Internal.Figures {
    using HorizontalAlignment = Mkamo.Common.DataType.HorizontalAlignment;
    using Mkamo.Common.Forms.Drawing;
    using Mkamo.Common.Diagnostics;
using Mkamo.Figure.Figures;
    using Mkamo.Figure.Core;
    using System.Text.RegularExpressions;
    using Mkamo.Common.DataType;
    
    internal class BlockRenderer {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 行頭禁則文字
        /// </summary>
        private static readonly string ForbiddensForLineHead = @",，、.．。・:;：；!?！？-‐―~～)]}）］｝」』】〕〉》'""’”";

        /// <summary>
        /// 行末禁則文字
        /// </summary>
        private static readonly string ForbiddensForLineEnd = "([{（［｛「『【〔〈《'\"’”";


        // ========================================
        // field
        // ========================================
        private IFigure _owner;
        private FontCache _fontCache;
        private VisualLineCache _visualLineCache;
        private SizeCache _sizeCache;
        private BoundsCache _boundsCache;

        private int _maxWidth;

        // ========================================
        // constructor
        // ========================================
        public BlockRenderer(
            IFigure owner,
            FontCache fontCache,
            VisualLineCache visualLineCache,
            SizeCache sizeCache,
            BoundsCache boundsCache
        ) {
            _owner = owner;
            _fontCache = fontCache;
            _visualLineCache = visualLineCache;
            _sizeCache = sizeCache;
            _boundsCache = boundsCache;
            _maxWidth = int.MaxValue;
        }

        // ========================================
        // property
        // ========================================
        public int MaxWidth {
            get { return _maxWidth; }
            set { _maxWidth = value; }
        }

        // ========================================
        // method
        // ========================================
        /// <summary>
        /// blockのサイズを返す．
        /// </summary>
        public Size MeasureBlockSize(Graphics g, Block block) {
            var lines = block.LineSegments;

            var width = 0;
            var height = 0;
            foreach (var line in lines) {
                var lineSize = _sizeCache.GetSize(g, line);
                width = Math.Max(width, lineSize.Width);
                height += lineSize.Height;
            }

            return new Size(width, height) + block.Padding.Size;
        }

        /// <summary>
        /// lineのサイズを返す．
        /// </summary>
        public Size MeasureLineSegmentSize(Graphics g, LineSegment line) {
            var lineHeight = 0;
            var lineWidth = 0;

            var virLines = _visualLineCache.GetVisualLines(g, line);
            foreach (var virLine in virLines) {
                lineWidth = Math.Max(lineWidth, virLine.Bounds.Width);
                lineHeight += virLine.Bounds.Height;
            }

            return new Size(lineWidth, lineHeight);
        }

        /// <summary>
        /// inlineのサイズを返す．
        /// </summary>
        public Size MeasureInlineSize(Graphics g, Inline inline) {

            /// font.Heightだと
            /// MS Gothicだとそのままの値
            /// MeiryoKe_Gothicで-1しないとちゃんといけない値が返ってくるので
            /// " "のSizeを代わりに使う

            var font = _fontCache.GetFont(inline.Font);
            if (inline.IsAnchorCharacter) {
                var size = _owner.MeasureText(g, " ", font, int.MaxValue);
                return new Size(FigureConsts.AnchorCharWidth, size.Height);

            } else if (inline.IsLineEndCharacter) {
                var size = _owner.MeasureText(g, " ", font, int.MaxValue);
                return new Size(FigureConsts.LineEndCharWidth, size.Height);

            } else {
                var size = _owner.MeasureText(g, inline.Text, font, int.MaxValue);
                return size;
            }
        }

        /// <summary>
        /// line内に含まれるVisualLineの配列を返す．
        /// 戻り値のVisualLineに格納されるOffsetやBoundsはlineで0起点の値．
        /// </summary>
        public VisualLine[] MeasureVisualLines(Graphics g, LineSegment line) {
            var ret = new List<VisualLine>();

            var block = line.Parent as Block;
            var visLineMaxWidth = _maxWidth - block.Padding.Width;

            var cTop = 0;
            var cWidth = 0;
            var cHeight = 0;
            var maxWidth = 0;

            var visLine = new VisualLine();
            visLine.Line = line;
            visLine.LineIndex = 0;
            visLine.CharOffset = 0;
            visLine.CharLength = 0;

            /// lineの最後なので必ずLine/BlockBreak
            var lastInline = line._Inlines[line._Inlines.Count - 1];
            var lastInlineSize = _sizeCache.GetSize(g, lastInline);

            for(int i = 0, len = line._Inlines.Count; i < len; ++i) {
                var inline = line._Inlines[i];
                var inlineSize = _sizeCache.GetSize(g, inline);

                var isJustBeforeBreak = (i == len - 2); /// Breakの直前かどうか
                if (
                    (!isJustBeforeBreak && (cWidth + inlineSize.Width < visLineMaxWidth)) || /// inlineが表示行をまたがない
                    (isJustBeforeBreak && cWidth + inlineSize.Width + lastInlineSize.Width < visLineMaxWidth)
                    /// Breakの直前だがBreakを含めても行をまたがない
                ) {
                    /// inlineが表示行をまたがない，かつ次がanchorでないまたはanchrだが行をまたがない
                    /// ただしi == len - 2，すなわちLine/BlockBreakの一つ手前のinlineの場合は
                    /// 最後のinlineのwidthも考慮する(禁則処理のため)

                    /// visLineにinlineを追加
                    visLine.CharLength += inline.Length;
                    cWidth += inlineSize.Width;
                    cHeight = Math.Max(cHeight, inlineSize.Height);
                    visLine.VisualInlines.Add(new VisualInline(inline, inlineSize));

                    visLine.Bounds = new Rectangle(
                        0,
                        cTop,
                        cWidth,
                        cHeight + line.LineSpace
                    );

                } else {
                    /// inlineが表示行をまたぐ

                    if (inline.IsLineEndCharacter) {
                        ret.Add(visLine);
                        maxWidth = Math.Max(maxWidth, visLine.Bounds.Width);

                        var newVisLine = new VisualLine();
                        newVisLine.Line = line;
                        newVisLine.LineIndex = visLine.LineIndex + 1;
                        newVisLine.CharOffset = visLine.CharOffset + visLine.CharLength;
                        newVisLine.CharLength = 0;

                        cTop += cHeight + line.LineSpace;

                        visLine = newVisLine;

                        visLine.CharLength += inline.Length;
                        visLine.Bounds = new Rectangle(0, cTop, inlineSize.Width, inlineSize.Height + line.LineSpace);
                        visLine.VisualInlines.Add(new VisualInline(inline, inlineSize));
                        continue;

                    } else if (inline.IsAnchorCharacter) {
                        ret.Add(visLine);
                        maxWidth = Math.Max(maxWidth, visLine.Bounds.Width);

                        var newVisLine = new VisualLine();
                        newVisLine.Line = line;
                        newVisLine.LineIndex = visLine.LineIndex + 1;
                        newVisLine.CharOffset = visLine.CharOffset + visLine.CharLength;
                        newVisLine.CharLength = 0;

                        cTop += cHeight + line.LineSpace;

                        visLine = newVisLine;

                        visLine.CharLength += inline.Length;
                        visLine.Bounds = new Rectangle(0, cTop, inlineSize.Width, inlineSize.Height + line.LineSpace);
                        visLine.VisualInlines.Add(new VisualInline(inline, inlineSize));

                        continue;
                    }

                    /// cacheしたfontだと例外が起きる．DeleteObject()してしまうのがまずいのだろうか?
                    using (var font = inline.Font.CreateFont()) {

                        var cOffsetInInline = 0;
                        var drawableLen = 0;
                        var first = true;
                        var partSize = _owner.MeasureText(g, inline.Text, font, visLineMaxWidth - cWidth, out drawableLen);

                        /// Break直前のinline (i == len - 2)，かつ，
                        /// Breakを考慮しなければ行に収まる，かつ
                        /// Breakを考慮すると行にに収まらない，
                        /// すなわちbreakの禁則処理が必要である
                        var needHyphenationForBreak =
                            i == len - 2 &&
                            (cOffsetInInline + drawableLen == inline.Length) &&
                            !(cWidth + inlineSize.Width + lastInlineSize.Width < visLineMaxWidth);

                        while (
                            (cOffsetInInline + drawableLen < inline.Length || needHyphenationForBreak) &&
                            (first || drawableLen > 0)
                        ) {
                            if (first) {
                                first = false;
                            }

                            /// 禁則処理 (送り出し処理のみ対応，最大5文字まで)
                            var hyphenationCount = 0;
                            while (
                                (
                                    needHyphenationForBreak ||
                                    NeedHyphenationForLineHead(inline.Text, cOffsetInInline + drawableLen) ||
                                    NeedHyphenationForLineEnd(inline.Text, cOffsetInInline + drawableLen - 1)
                                ) &&
                                drawableLen > 1 &&
                                hyphenationCount < 5 /// 禁則処理は5文字までにしておく
                            ) {
                                needHyphenationForBreak = false;
                                partSize = _owner.MeasureText(
                                    g, inline.Text.Substring(cOffsetInInline, drawableLen - 1), font, visLineMaxWidth, out drawableLen
                                );
                                ++hyphenationCount;
                            }

                            visLine.CharLength += drawableLen;
                            cWidth += partSize.Width;
                            cHeight = Math.Max(cHeight, partSize.Height);
                            /// 後で合わせるのでここでは真中寄せ，右寄せを考慮しなくていい
                            visLine.Bounds = new Rectangle(0, cTop, cWidth, cHeight + line.LineSpace);
                            visLine.VisualInlines.Add(
                                new VisualInline(
                                    inline,
                                    inline.Text.Substring(cOffsetInInline, drawableLen),
                                    partSize
                                )
                            );
                            ret.Add(visLine);
                            maxWidth = Math.Max(maxWidth, visLine.Bounds.Width);

                            cOffsetInInline += drawableLen;
                            cTop += cHeight + line.LineSpace;
                            cWidth = 0;
                            cHeight = 0;

                            var newVisLine = new VisualLine();
                            newVisLine.Line = line;
                            newVisLine.LineIndex = visLine.LineIndex + 1;
                            newVisLine.CharOffset = visLine.CharOffset + visLine.CharLength;
                            newVisLine.CharLength = 0;

                            visLine = newVisLine;

                            partSize = _owner.MeasureText(
                                g, inline.Text.Substring(cOffsetInInline), font, visLineMaxWidth, out drawableLen
                            );

                            /// 2行目以降のBreak用禁則処理判定
                            if (i == len - 2) {
                                needHyphenationForBreak =
                                    (cOffsetInInline + drawableLen == inline.Length) &&
                                    !(cWidth + partSize.Width + lastInlineSize.Width < visLineMaxWidth);
                            }

                            /// 2行目以降のAnchor用禁則処理判定
                            //if (nextInlineIsAnchor) {
                            //    needHyphenationForAnchor =
                            //        (cOffsetInInline + drawableLen == inline.Length) &&
                            //        !(cWidth + inlineSize.Width + nextAnchorSize.Width < visLineMaxWidth);
                            //}
                        }

                        visLine.CharLength += drawableLen;
                        cWidth += partSize.Width;
                        cHeight = Math.Max(cHeight, partSize.Height);
                        /// 後で合わせるのでここでは真中寄せ，右寄せを考慮しなくていい
                        visLine.Bounds = new Rectangle(0, cTop, cWidth, cHeight + line.LineSpace);
                        visLine.VisualInlines.Add(
                            new VisualInline(
                                inline,
                                inline.Text.Substring(cOffsetInInline),
                                partSize
                            )
                        );
                    }
                }
            }


            ret.Add(visLine);
            maxWidth = Math.Max(maxWidth, visLine.Bounds.Width);


            /// 真中寄せ，右寄せ
            if (line.HorizontalAlignment != HorizontalAlignment.Left) {
                /// 最後の表示行の水平位置の調整
                var lastVisLine = ret[ret.Count - 1];
                var visLineLeft = 0;
                switch (line.HorizontalAlignment) {
                    case HorizontalAlignment.Center: {
                        visLineLeft = (maxWidth - lastVisLine.Bounds.Width + FigureConsts.LineEndCharWidth) / 2;
                        break;
                    }
                    case HorizontalAlignment.Right: {
                        visLineLeft = maxWidth - lastVisLine.Bounds.Width + FigureConsts.LineEndCharWidth - 1; /// -1は本当はいらないと思うが見栄えのため
                        break;
                    }
                }
                lastVisLine.Bounds = new Rectangle(
                    new Point(visLineLeft, lastVisLine.Bounds.Top),
                    lastVisLine.Bounds.Size
                );
            }

            Contract.Ensures(
                line.Length == ret.Sum(vl => vl.VisualInlines.Sum(vi => vi.Text.Length)),
                "VisualInline length"
            );
            Contract.Ensures(
                line.Length == ret.Sum(vl => vl.CharLength),
                "VisualLine length"
            );

            if (_owner.Root != null) {
                /// ハイライト処理
                var registry = _owner.Root.Canvas.HighlightRegistry;
                {
                    var hls = registry.GetHighlights(block.Class);
                    if (hls != null) {
                        foreach (var hl in hls) {
                            HighlightVisualLines(g, line, ret, hl);
                        }
                    }
                }
    
                /// グローバルハイライト (主に検索キーワード強調)
                {
                    var ghls = registry.GlobalHighlights;
                    if (ghls != null) {
                        foreach (var hl in ghls) {
                            HighlightVisualLines(g, line, ret, hl);
                        }
                    }
                }
            }

            return ret.ToArray();
        }

        private void HighlightVisualLines(Graphics g, LineSegment line, List<VisualLine> visLines, Highlight highlight) {

            var lineText = line.Text;
            var matches = highlight.Regex.Matches(lineText);

            /// マッチ個所をハイライト
            if (matches.Count > 0) {
                foreach (Match match in matches) {
                    var matchIndex = match.Index;
                    var matchLen = match.Length;

                    SplitVisualLines(g, visLines, matchIndex);
                    SplitVisualLines(g, visLines, matchIndex + matchLen);

                    var cIndex = 0;
                    var inRange = false;
                    for (int i = 0, ilen = visLines.Count; i < ilen; ++i) {
                        var visLine = visLines[i];

                        for (int j = 0, jlen = visLine.VisualInlines.Count; j < jlen; ++j) {
                            var visInline = visLine.VisualInlines[j];

                            if (!(visInline.Inline.IsControlCharacter)) {
                                if (cIndex == matchIndex) {
                                    inRange = true;
                                }

                                if (inRange) {
                                    /// ハイライトの適用
                                    if (highlight.ForeColor.HasValue) {
                                        visInline.ForeColor = highlight.ForeColor.Value;
                                    }
                                    if (highlight.BackColor.HasValue) {
                                        visInline.BackColor = highlight.BackColor.Value;
                                    }
                                    if (highlight.IsUnderline.HasValue) {
                                        if (highlight.IsUnderline.Value) {
                                            visInline.Font = new FontDescription(visInline.Font, visInline.Font.Style | FontStyle.Underline);
                                        } else {
                                            visInline.Font = new FontDescription(visInline.Font, visInline.Font.Style ^ FontStyle.Underline);
                                        }
                                    }
                                    visLine.VisualInlines[j] = visInline;
                                }

                                if (cIndex + visInline.Text.Length == matchIndex + matchLen) {
                                    inRange = false;
                                }

                                cIndex += visInline.Text.Length;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// visLines内のvisInlineをindex番目で分割する。
        /// </summary>
        private void SplitVisualLines(Graphics g, List<VisualLine> visLines, int index) {
            var cIndex = 0;
            for (int i = 0, ilen = visLines.Count; i < ilen; ++i) {
                var visLine = visLines[i];

                for (int j = 0, jlen = visLine.VisualInlines.Count; j < jlen; ++j) {
                    var visInline = visLine.VisualInlines[j];

                    if (!(visInline.Inline.IsControlCharacter)) {
                        var visInlineRange = new Range(cIndex, visInline.Text.Length);

                        if (visInlineRange.Contains(index)) {
                            if (visInlineRange.Start == index) {
                                return;
                            }

                            var first = visInline;
                            var second = visInline;
                            using (var font = visInline.Font.CreateFont()) {
                                first.Text = visInline.Text.Substring(0, index - cIndex);
                                first.Size = _owner.MeasureText(g, first.Text, font, int.MaxValue);
                                second.Text = visInline.Text.Substring(index - cIndex);
                                second.Size = _owner.MeasureText(g, second.Text, font, int.MaxValue);
                            }

                            visLine.VisualInlines[j] = first;
                            visLine.VisualInlines.Insert(j + 1, second);
                        }

                        cIndex += visInline.Text.Length;
                    }
                }
            }
        }


        /// <summary>
        /// 戻り値はlineを含むBlockで0起点の値．
        /// </summary>
        public Rectangle MeasureLineBounds(Graphics g, LineSegment line) {
            var block = line.Parent as Block;

            var cTop = block.Padding.Top;
            foreach (var cLine in block.LineSegments) {
                var lineSize = _sizeCache.GetSize(g, cLine);
                if (cLine == line) {
                    switch (block.HorizontalAlignment) {
                        case HorizontalAlignment.Left: {
                            return new Rectangle(
                                block.Padding.Left, cTop, lineSize.Width, lineSize.Height
                            );
                        }
                        case HorizontalAlignment.Center: {
                            var blockSize = _sizeCache.GetSize(g, block);
                            return new Rectangle(
                                (blockSize.Width - lineSize.Width) / 2, cTop,
                                lineSize.Width, lineSize.Height
                            );
                        }
                        case HorizontalAlignment.Right: {
                            var blockSize = _sizeCache.GetSize(g, block);
                            return new Rectangle(
                                blockSize.Width - lineSize.Width, cTop,
                                lineSize.Width, lineSize.Height
                            );
                        }
                    }
                }

                cTop += lineSize.Height;
            }

            throw new ArgumentOutOfRangeException("line");
        }

        /// <summary>
        /// charIndex，戻り値はblockで0起点の値．
        /// </summary>
        public Rectangle[] GetStringRect(Graphics g, Block block, int charIndex, int length) {

            int startLineIndex, startColumnIndex;
            var startLine = block.GetLineSegmentAtLocal(charIndex, out startLineIndex, out startColumnIndex);
            var startVisLines = _visualLineCache.GetVisualLines(g, startLine);
            var startVisLine = GetVisualLineAt(g, startLine, startColumnIndex);

            var endCharIndex = charIndex + length - 1;
            int endLineIndex, endColumnIndex;
            var endLine = block.GetLineSegmentAtLocal(endCharIndex, out endLineIndex, out endColumnIndex);
            var endVisLines = _visualLineCache.GetVisualLines(g, endLine);
            var endVisLine = GetVisualLineAt(g, endLine, endColumnIndex);

            var blockBounds = _boundsCache.GetBounds(g, block);

            if (startLineIndex == endLineIndex) {
                /// 最初のLineと最後のLineが同じ場合
                var startLineBounds = _boundsCache.GetBounds(g, startLine);
                var rects = GetStringRectsInLine(g, startLine, startColumnIndex, length);

                return RectUtil.Translate(rects, (Size) startLineBounds.Location - (Size) blockBounds.Location);

            } else {
                /// 最初の行と最後の行が異なる場合

                var ret = new List<Rectangle>();

                /// 最初の行
                var startLineBounds = _boundsCache.GetBounds(g, startLine);
                var startLineStringRects = GetStringRectsInLine(
                    g, startLine, startColumnIndex, startLine.Length - startColumnIndex
                );
                ret.AddRange(
                    RectUtil.Translate(
                        startLineStringRects,
                        (Size) startLineBounds.Location - (Size) blockBounds.Location
                    )
                );

                /// 間の行
                for (int i = startLineIndex + 1; i < endLineIndex; ++i) {
                    var line = block.LineSegments.ElementAt(i);
                    var lineBounds = _boundsCache.GetBounds(g, line);
                    var visLines = _visualLineCache.GetVisualLines(g, line);
                    var rects = visLines.Select(
                        visLine => RectUtil.Translate(
                            visLine.Bounds, (Size) lineBounds.Location - (Size) blockBounds.Location
                        )
                    );
                    ret.AddRange(rects);
                }

                /// 最後の行
                var endLineBounds = _boundsCache.GetBounds(g, endLine);
                var endLineStringRects = GetStringRectsInLine(
                    g, endLine, 0, endColumnIndex + 1
                );
                ret.AddRange(
                    RectUtil.Translate(
                        endLineStringRects,
                        (Size) endLineBounds.Location - (Size) blockBounds.Location
                    )
                );

                return ret.ToArray();
            }
        }

        /// <summary>
        /// line全体の矩形内におけるindex番目の文字の描画矩形を返す．
        /// charIndex，戻り値はlineで0起点の値．
        /// </summary>
        public Rectangle GetCharRectInLine(Graphics g, LineSegment line, int charIndex) {
            var lineLength = line.Length;
            if (charIndex > lineLength) {
                throw new ArgumentOutOfRangeException("index");
            }

            var visLines = _visualLineCache.GetVisualLines(g, line);
            var cCharIndex = 0;
            foreach (var visLine in visLines) {
                if (charIndex >= cCharIndex && charIndex < cCharIndex + visLine.CharLength) {
                    return GetCharRectInVisualLine(g, visLine, charIndex);
                }
                cCharIndex += visLine.CharLength;
            }

            throw new ArgumentOutOfRangeException("index");
        }
    
        /// <summary>
        /// line全体の矩形内におけるindex番目から長さlengthの文字列の描画矩形を返す．
        /// charIndex，戻り値はlineで0起点の値．
        /// </summary>
        public Rectangle[] GetStringRectsInLine(
            Graphics g, LineSegment line, int charIndex, int length
        ) {
            var visLines = _visualLineCache.GetVisualLines(g, line);
            var startVisLine = GetVisualLineAt(g, line, charIndex);
            var endCharIndex = charIndex + length - 1;
            var endVisLine = GetVisualLineAt(g, line, endCharIndex);

            if (startVisLine.LineIndex == endVisLine.LineIndex) {
                /// 最初の行と最後の行が同じ場合
                var rect = GetStringRectInVisualLine(g, startVisLine, charIndex, length);
                return new[] { rect };

            } else {
                /// 最初の行と最後の行が異なる場合
                var ret = new List<Rectangle>();

                /// 最初の行
                var startVisLineRect = GetStringRectInVisualLine(
                    g,
                    startVisLine,
                    charIndex,
                    startVisLine.CharLength - (charIndex - startVisLine.CharOffset + 1) + 1
                );
                ret.Add(startVisLineRect);

                /// 間の行
                for (int i = startVisLine.LineIndex + 1; i < endVisLine.LineIndex; ++i) {
                    ret.Add(visLines[i].Bounds);
                }

                /// 最後の行
                var endVisLineRect = GetStringRectInVisualLine(
                    g,
                    endVisLine,
                    endVisLine.CharOffset,
                    endCharIndex - endVisLine.CharOffset + 1
                );
                ret.Add(endVisLineRect);

                return ret.ToArray();
            }
        }


        /// <summary>
        /// charIndex，戻り値はvisLineが含まれるLineで0起点の値．
        /// </summary>
        public Rectangle GetStringRectInVisualLine(
            Graphics g, VisualLine visLine, int charIndex, int length
        ) {
            if (charIndex < visLine.CharOffset) {
                throw new ArgumentOutOfRangeException("index");
            }
            /// lengthが長すぎる場合は行末までの長さに
            var realLen =
                charIndex + length < visLine.CharOffset + visLine.CharLength?
                    length:
                    visLine.CharOffset + visLine.CharLength - charIndex;
                    /// (visLine.CharOffset + visLine.CharLength - 1) - charIndex + 1の+1-1を省略

            var startCharLeft = GetCharLeftInVisualLine(g, visLine, charIndex);
            var endCharRight = GetCharRightInVisualLine(g, visLine, charIndex + realLen - 1);

            return new Rectangle(
                startCharLeft,
                visLine.Bounds.Top,
                endCharRight - startCharLeft,
                visLine.Bounds.Height
            );
        }

        /// <summary>
        /// charIndex，戻り値はvisLineが含まれるLineで0起点の値．
        /// </summary>
        public Rectangle GetCharRectInVisualLine(Graphics g, VisualLine visLine, int charIndex) {
            var cCharIndex = visLine.CharOffset;
            var cLeft = visLine.Bounds.Left;

            foreach (var visInline in visLine.VisualInlines) {
                /// charIndexがvisInline内の文字を指すなら
                if (charIndex >= cCharIndex && charIndex < cCharIndex + visInline.Text.Length) {
                    var cFont = _fontCache.GetFont(visInline.Font);

                    var top = visLine.Bounds.Bottom - visInline.Size.Height;
                    var left = (charIndex == cCharIndex)?
                        cLeft:
                        cLeft + _owner.MeasureText(
                            g,
                            visInline.Text.Substring(0, charIndex - cCharIndex),
                            cFont,
                            int.MaxValue
                        ).Width;
                    var width = 0;
                    if (visInline.Inline.IsLineEndCharacter) {
                        width = FigureConsts.LineEndCharWidth;
                    } else if (visInline.Inline.IsAnchorCharacter) {
                        width = FigureConsts.AnchorCharWidth;
                    } else {
                        width = _owner.MeasureText(
                            g,
                            visInline.Text.Substring(charIndex - cCharIndex, 1),
                            cFont,
                            int.MaxValue
                        ).Width;
                    }
                    var height = visInline.Size.Height;
                    return new Rectangle(left, top, width, height);
                }
                cLeft += visInline.Size.Width;
                cCharIndex += visInline.Text.Length;
            }

            //if (charIndex == visLine.CharLength) {
            //    return new Rectangle(cLeft, 0, ControlCharWidth, visLine.Bounds.Height);
            //}

            throw new ArgumentOutOfRangeException("index");
        }

        // ------------------------------
        // private
        // ------------------------------
        /// <summary>
        /// charIndex，戻り値はvisLineが含まれるLineで0起点の値．
        /// </summary>
        private int GetCharLeftInVisualLine(Graphics g, VisualLine visLine, int charIndex) {
            var cCharIndex = visLine.CharOffset;
            var cLeft = visLine.Bounds.Left;

            foreach (var visInline in visLine.VisualInlines) {
                /// charIndexがvisInline内の文字を指すなら
                if (charIndex >= cCharIndex && charIndex < cCharIndex + visInline.Text.Length) {
                    var cFont = _fontCache.GetFont(visInline.Font);

                    return (charIndex == cCharIndex)?
                        cLeft:
                        cLeft + _owner.MeasureText(
                            g,
                            visInline.Text.Substring(0, charIndex - cCharIndex),
                            cFont,
                            int.MaxValue
                        ).Width;
                }
                cLeft += visInline.Size.Width;
                cCharIndex += visInline.Text.Length;
            }

            throw new ArgumentOutOfRangeException("index");
        }

        /// <summary>
        /// charIndex，戻り値はvisLineが含まれるLineで0起点の値．
        /// </summary>
        private int GetCharRightInVisualLine(Graphics g, VisualLine visLine, int charIndex) {
            var cCharIndex = visLine.CharOffset;
            var cLeft = visLine.Bounds.Left;

            foreach (var visInline in visLine.VisualInlines) {
                /// charIndexがvisInline内の文字を指すなら
                if (charIndex >= cCharIndex && charIndex < cCharIndex + visInline.Text.Length) {
                    var cFont = _fontCache.GetFont(visInline.Font);

                    return (charIndex == cCharIndex + visInline.Text.Length - 1)?
                        cLeft + visInline.Size.Width:
                        cLeft + _owner.MeasureText(
                            g,
                            visInline.Text.Substring(0, charIndex - cCharIndex + 1),
                            cFont,
                            int.MaxValue
                        ).Width;
                }
                cLeft += visInline.Size.Width;
                cCharIndex += visInline.Text.Length;
            }

            throw new ArgumentOutOfRangeException("index");
        }

        /// <summary>
        /// charIndexはlineで0起点の値．
        /// </summary>
        private VisualLine GetVisualLineAt(Graphics g, LineSegment line, int charIndex) {
            var visLines = _visualLineCache.GetVisualLines(g, line);
            foreach (var visLine in visLines) {
                if (charIndex >= visLine.CharOffset && charIndex < visLine.CharOffset + visLine.CharLength) {
                    return visLine;
                }
            }

            throw new ArgumentOutOfRangeException("charIndex");
        }

        /// <summary>
        /// sのindex番目の文字が行頭禁則処理が必要かどうかを返す。
        /// </summary>
        private bool NeedHyphenationForLineHead(string s, int index) {
            if (index < 0 || index > s.Length - 1) {
                return false;
            }
            var ch = s[index];
            return ForbiddensForLineHead.IndexOf(ch) != -1;
        }

        /// <summary>
        /// sのindex番目の文字が行末禁則処理が必要かどうかを返す。
        /// </summary>
        private bool NeedHyphenationForLineEnd(string s, int index) {
            if (index < 0 || index > s.Length - 1) {
                return false;
            }
            var ch = s[index];
            return ForbiddensForLineEnd.IndexOf(ch) != -1;
        }

        // 使われていないがもったいないので消さないでコメントアウト
        //public Rectangle GetCharRect(Graphics g, Block block, int charIndex) {
        //    if (charIndex < 0) {
        //        throw new ArgumentOutOfRangeException("index");
        //    }

        //    var lines = block.Lines;
        //    var cLineIndex = 0;
        //    var cCharIndex = 0;
        //    foreach (var line in lines) {
        //        var lineLength = line.Length;

        //        /// index番目の文字がlineに含まれているなら
        //        if (charIndex >= cCharIndex && charIndex < cCharIndex + lineLength) {
        //            var colIndex = charIndex - cCharIndex;
        //            var lineRect = GetLineRect(g, block, cLineIndex);
        //            var charRectInLine = GetCharRectInLine(g, line, colIndex);

        //            return new Rectangle(
        //                lineRect.Left + charRectInLine.Left,
        //                lineRect.Top + charRectInLine.Top,
        //                charRectInLine.Width,
        //                charRectInLine.Height
        //            );
        //        }
                
        //        cCharIndex += lineLength;
        //        ++cLineIndex;
        //    }

        //    throw new ArgumentOutOfRangeException("index");
        //}

    }
}
