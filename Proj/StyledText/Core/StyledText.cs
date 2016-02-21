/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Common.Util;
using Mkamo.Common.Collection;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Mkamo.Common.DataType;
using Mkamo.StyledText.Internal.Core;
using Mkamo.StyledText.Util;
using Mkamo.Common.String;
using Mkamo.Common.Core;
using Mkamo.Common.Diagnostics;
using Mkamo.StyledText.Writer;
using System.Drawing;

/// <summary>
/// 実装の注意．
/// 各クラスのフィールド値の変更時には必ずFlow#NotifyContentsChange()を呼ぶ．
/// </summary>
namespace Mkamo.StyledText.Core {
    [Serializable]
    [DataContract]
    public class StyledText: Flow, ILineContainer {
        // ========================================
        // static field
        // ========================================
        private static readonly log4net.ILog Logger =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // ========================================
        // field
        // ========================================
        [DataMember]
        private List<Block> _blocks = new List<Block>();

        [DataMember]
        private VerticalAlignment _verticalAlignment = VerticalAlignment.Top;

        [NonSerialized]
        private FlowOffsetCache<Block> _offsetCache;

        [NonSerialized]
        private LineContainerSupport _lineContainer;

        // ========================================
        // constructor
        // ========================================
        public StyledText(): this(new Paragraph()) {
        }

        public StyledText(Paragraph para) {
            if (para == null) {
                para = new Paragraph();
            }

            _offsetCache = new FlowOffsetCache<Block>(_blocks);

            _lineContainer = new LineContainerSupport(CreateLineStrings, "\r");

            Font = DefaultFont;
            Add(para);
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) {
            _lineContainer = new LineContainerSupport(CreateLineStrings, "\r");
            _offsetCache = new FlowOffsetCache<Block>(_blocks);
        }
    
        // ========================================
        // event
        // ========================================
        [field:NonSerialized]
        public event EventHandler<ContentsChangedEventArgs> ContentsChanged;

        [field:NonSerialized]
        public event EventHandler<StyledTextModifiedEventArgs> TextModified;

        // ========================================
        // property
        // ========================================
        // === Flow ==========
        public override StyledText Root {
            get { return this; }
        }

        public override int Length {
            get { return _offsetCache.Length; }
        }

        public override string Text {
            get {
                var ret = new StringBuilder();
                foreach (var block in _blocks) {
                    ret.Append(block.Text);
                }
                return ret.ToString();
            }

            set {
                throw new NotImplementedException();
            }
        }

        public override IEnumerable<Flow> Children {
            get {
                foreach (var block in _blocks) {
                    yield return block;
                }
            }
        }

        // === StyledText ==========
        public string[] Lines {
            get { return _lineContainer.Lines; }
        }

        public int LineCount {
            get { return _blocks.Sum(block => block.LineCount); }
        }

        public IEnumerable<LineSegment> LineSegments {
            get { return new LineSegmentsEnumerable(this); }
        }

        public IEnumerable<Inline> Inlines {
            get { return new InlinesEnumerable(this); }
        }

        public IEnumerable<Block> Blocks {
            get { return _blocks; }
        }

        public int InlinesCount {
            get { return Inlines.Count(); }
        }

        public int BlocksCount {
            get { return _blocks.Count; }
        }

        public bool IsEmpty {
            get {
                return
                    _blocks.Count == 1 &&
                    _blocks[0].LineCount == 1 &&
                    _blocks[0]._Lines[0]._Inlines.Count == 1;
            }
        }

        public VerticalAlignment VerticalAlignment {
            get { return _verticalAlignment; }
            set {
                if (value == _verticalAlignment) {
                    return;
                }
                _verticalAlignment = value;
                OnContentsChanged(this);
            }
        }

        // ------------------------------
        // internal
        // ------------------------------
        internal List<Block> _Blocks {
            get { return _blocks; }
        }

        // ========================================
        // method
        // ========================================
        // ------------------------------
        // public
        // ------------------------------
        public override string ToPlainText(PlainTextWriterSettings settings) {
            var ret = new StringBuilder();
            foreach (var block in _blocks) {
                ret.Append(block.ToPlainText(settings));
            }
            return ret.ToString();
        }

        public override string ToHtmlText() {
            var writer = new HtmlWriter();
            return writer.ToHtmlBodyContent(_blocks.As<Block, Flow>());
        }

        // === ILineContainer ==========
        public int GetLineIndex(int charIndex) {
            return _lineContainer.GetLineIndex(charIndex);
        }

        public int GetColumnIndex(int charIndex) {
            return _lineContainer.GetColumnIndex(charIndex);
        }

        public int GetLineStartCharIndex(int lineIndex) {
            return _lineContainer.GetLineStartCharIndex(lineIndex);
        }

        public int GetCharIndex(int lineIndex, int columnIndex) {
            return _lineContainer.GetCharIndex(lineIndex, columnIndex);
        }

        public int GetColumnCount(int lineIndex) {
            return _lineContainer.GetColumnCount(lineIndex);
        }

        // === StyledText ==========
        // --- read ---
        public override void Transfer(Flow flow) {
            base.Transfer(flow);

            var st = flow as StyledText;
            if (st == null) {
                return;
            }
            st._verticalAlignment = _verticalAlignment;
        }


        /// <summary>
        /// ディープコピーした複製を返す．
        /// </summary>
        public override object CloneDeeply() {
            var ret = (StyledText) Clone();
            ret.RemoveAt(0);
            foreach (var block in Blocks) {
                var blockClone = (Block) block.CloneDeeply();
                ret.Add(blockClone);
            }
            return ret;
        }

        public StyledText CopyRange(Range copyRange) {
            Contract.Requires(copyRange.Start >= 0 && copyRange.End < Length);

            var ret = (StyledText) Clone();
            ret.RemoveAt(0);

            foreach (var block in _blocks) {
                var blockRange = new Range(_offsetCache.GetOffset(block), block.Length);

                if (copyRange.Contains(blockRange)) {
                    var childClone = (Block) block.CloneDeeply();
                    ret.InsertAfter(childClone);

                } else if (blockRange.Contains(copyRange)) {
                    var offset = copyRange.Start - blockRange.Start;
                    var childCopy = (Block) block.CopyRange(new Range(offset, copyRange.Length));
                    ret.InsertAfter(childCopy);

                } else if (copyRange.Intersects(blockRange)) {

                    if (blockRange.Start < copyRange.Start) {
                        var offset = copyRange.Start - blockRange.Start;
                        var childCopy = (Block) block.CopyRange(new Range(offset, blockRange.End - copyRange.Start + 1));
                        ret.InsertAfter(childCopy);

                    } else {
                        var childCopy = (Block) block.CopyRange(new Range(0, copyRange.End - blockRange.Start + 1));
                        ret.InsertAfter(childCopy);
                    }
                
                }
            }

            return ret;
        }

        public IEnumerable<Inline> CopyInlines(Range copyRange) {
            Contract.Requires(copyRange.Start >= 0 && copyRange.End < Length);

            var ret = new List<Inline>();

            foreach (var block in _blocks) {
                var blockRange = new Range(_offsetCache.GetOffset(block), block.Length);

                if (copyRange.Contains(blockRange)) {
                    var childCopy = block.CopyInlines(new Range(0, block.Length));
                    ret.AddRange(childCopy);

                } else if (blockRange.Contains(copyRange)) {
                    var offset = copyRange.Start - blockRange.Start;
                    var childCopy = block.CopyInlines(new Range(offset, copyRange.Length));
                    ret.AddRange(childCopy);

                } else if (copyRange.Intersects(blockRange)) {

                    if (blockRange.Start < copyRange.Start) {
                        var offset = copyRange.Start - blockRange.Start;
                        var childCopy = block.CopyInlines(new Range(offset, blockRange.End - copyRange.Start + 1));
                        ret.AddRange(childCopy);

                    } else {
                        var childCopy = block.CopyInlines(new Range(0, copyRange.End - blockRange.Start + 1));
                        ret.AddRange(childCopy);
                    }
                
                }
            }

            return ret;
        }

        public IEnumerable<Flow> CopyBlocksAndInlines(int copyStartOffset){
            return CopyBlocksAndInlines(Range.FromStartAndEnd(copyStartOffset, Length - 1));
        }

        public IEnumerable<Flow> CopyBlocksAndInlines(Range copyRange) {
            Contract.Requires(copyRange.Start >= 0 && copyRange.End < Length);

            var ret = new List<Flow>();

            foreach (var block in _blocks) {
                var blockRange = new Range(_offsetCache.GetOffset(block), block.Length);

                if (copyRange.Contains(blockRange)) {
                    var childClone = (Block) block.CloneDeeply();
                    ret.Add(childClone);

                } else if (blockRange.Contains(copyRange)) {
                    var offset = copyRange.Start - blockRange.Start;
                    var childCopy = block.CopyInlines(new Range(offset, copyRange.Length));
                    foreach (var child in childCopy) {
                        ret.Add(child);
                    }

                } else if (copyRange.Intersects(blockRange)) {

                    if (blockRange.Start < copyRange.Start) {
                        var offset = copyRange.Start - blockRange.Start;
                        var childCopy = block.CopyInlines(new Range(offset, blockRange.End - copyRange.Start + 1));
                        foreach (var child in childCopy) {
                            ret.Add(child);
                        }

                    } else {
                        var childCopy = (Block) block.CopyRange(new Range(0, copyRange.End - blockRange.Start + 1));
                        ret.Add(childCopy);
                    }
                }
            }

            return ret;
        }

        // --- util ---
        public bool IsLineHead(int charIndex) {
            return GetColumnIndex(charIndex) == 0;
        }
        
        // --- range ---
        public Range GetRange(Flow flow) {
            if (flow is Inline) {
                return new Range(GetInlineOffset(flow as Inline), flow.Length);
            } else if (flow is LineSegment) {
                return new Range(GetLineSegmentOffset(flow as LineSegment), flow.Length);
            } else if (flow is Block) {
                return new Range(GetBlockOffset(flow as Block), flow.Length);
            } else if (flow == this) {
                return new Range(0, Length);
            }

            throw new ArgumentException("flow");
        }



        // --- style ---
        public FontDescription GetDefaultFont(ParagraphKind paraKind) {
            var style = paraKind == ParagraphKind.Normal ? FontStyle.Regular : FontStyle.Bold;
            return new FontDescription(Font.Name, GetDefaultFontSize(paraKind), style);
        }

        private float GetDefaultFontSize(ParagraphKind paraKind) {
            var fontSize = Font.Size;

            switch (paraKind) {
                case ParagraphKind.Heading1:
                    fontSize += 8;
                    break;
                case ParagraphKind.Heading2:
                    fontSize += 4;
                    break;
                case ParagraphKind.Heading3:
                    fontSize += 2;
                    break;
                case ParagraphKind.Heading4:
                    //fontSize += 1;
                    ++fontSize;
                    break;
                case ParagraphKind.Heading5:
                    break;
                case ParagraphKind.Heading6:
                    //fontSize -= 1;
                    --fontSize;
                    break;
                case ParagraphKind.Normal:
                default:
                    break;
            }

            return fontSize;
        }


        // --- block ---
        public int GetBlockOffset(Block block) {
            return _offsetCache.GetOffset(block);
        }

        public int GetBlockOffset(int blockIndex) {
            return _offsetCache.GetOffset(blockIndex);
        }

        /// <summary>
        /// charIndexの位置にあるBlockを返す．
        /// </summary>
        public Block GetBlockAtLocal(int charIndex, out int blockIndex, out int charIndexInBlock) {
            if (charIndex < 0) {
                throw new ArgumentOutOfRangeException("charIndex");
            }

            var cIndex = 0;
            var items = _offsetCache.Items;
            for (int i = 0; i < items.Count; ++i) {
                var item = items[i];
                var len = item.Length;
                if (charIndex >= cIndex && charIndex < cIndex + len) {
                    blockIndex = i;
                    charIndexInBlock = charIndex - cIndex;
                    return _blocks[i];
                }

                cIndex += len;
            }

            throw new ArgumentOutOfRangeException("charIndex");
        }

        public Block GetBlockAt(int charIndex, out int charIndexInBlock, out int blockOffset) {
            int blockIndex;
            var ret = GetBlockAtLocal(charIndex, out blockIndex, out charIndexInBlock);
            blockOffset = charIndex - charIndexInBlock;
            return ret;
        }

        public Block GetBlockAt(int charIndex) {
            int blockIndex, charIndexInBlock;
            return GetBlockAtLocal(charIndex, out blockIndex, out charIndexInBlock);
        }

        public IEnumerable<Block> GetBlocksInRange(Range range, bool containsFirstIntersected, bool containsLastIntersected) {
            var ret = new List<Block>();
            foreach (var block in _blocks) {
                var blockRange = new Range(_offsetCache.GetOffset(block), block.Length);

                if (range.Contains(blockRange)) {
                    ret.Add(block);

                } else if (blockRange.Contains(range)) {
                    if (containsFirstIntersected && containsLastIntersected) {
                        ret.Add(block);
                        break;
                    }

                } else if (range.Intersects(blockRange)) {

                    if (blockRange.Start < range.Start) {
                        if (containsFirstIntersected) {
                            ret.Add(block);
                        }

                    } else {
                        if (containsLastIntersected) {
                            ret.Add(block);
                        }
                    }
                }
            }
            return ret;
        }

        // --- line segment ---
        public LineSegment GetLineSegmentAt(int charIndex, out int charIndexInLine, out int lineSegOffset) {
            int blockIndex, charIndexInBlock;
            var block = GetBlockAtLocal(charIndex, out blockIndex, out charIndexInBlock);

            int lineIndex;
            var ret = block.GetLineSegmentAtLocal(charIndexInBlock, out lineIndex, out charIndexInLine);
            lineSegOffset = charIndex - charIndexInLine;
            return ret;
        }

        public int GetLineSegmentOffset(LineSegment line) {
            for (int bi = 0, blen = _blocks.Count; bi < blen; ++bi) {
                var block = _blocks[bi];

                for (int li = 0, llen = block._Lines.Count; li < llen; ++li) {
                    var l = block._Lines[li];

                    if (l == line) {
                        return
                            _offsetCache.GetOffset(bi) +
                            block.GetLineSegmentLocalOffset(li);
                    }
                }
            }

            throw new ArgumentException("line");
        }

        // --- inline ---
        public Inline GetInlineAt(int charIndex, out int charIndexInInline, out int inlineOffset) {
            int blockIndex, charIndexInBlock;
            var block = GetBlockAtLocal(charIndex, out blockIndex, out charIndexInBlock);

            int lineIndex, charIndexInLine;
            var line = block.GetLineSegmentAtLocal(charIndexInBlock, out lineIndex, out charIndexInLine);

            int inlineIndex;
            var ret = line.GetInlineAtLocal(charIndexInLine, out inlineIndex, out charIndexInInline);
            inlineOffset = charIndex - charIndexInInline;
            return ret;
        }

        public Inline GetInlineAt(int charIndex) {
            int charIndexInInline, inlineOffset;
            return GetInlineAt(charIndex, out charIndexInInline, out inlineOffset);
        }

        public int GetInlineOffset(Inline inline) {
            for (int bIndex = 0, bLen = _blocks.Count; bIndex < bLen; ++bIndex) {
                var block = _blocks[bIndex];

                for (int lIndex = 0, lLen = block._Lines.Count; lIndex < lLen; ++lIndex) {
                    var line = block._Lines[lIndex];

                    for (int iIndex = 0, ilen = line._Inlines.Count; iIndex < ilen; ++iIndex) {
                        var il = line._Inlines[iIndex];
                        if (il == inline) {
                            return
                                _offsetCache.GetOffset(bIndex) +
                                block.GetLineSegmentLocalOffset(lIndex) +
                                line.GetInlineLocalOffset(iIndex);
                        }
                    }
                }
            }

            throw new ArgumentException("inline");
        }

        // --- inline ---
        public bool HasNextInline(Inline inline) {
            var found = false;
            foreach (var i in Inlines) {
                if (found) {
                    return true;
                }
                if (i == inline) {
                    found = true;
                }
            }
            return false;
        }

        public bool HasPrevInline(Inline inline) {
            var first = true;
            foreach (var i in Inlines) {
                if (i == inline) {
                    return !first;
                }
                first = false;
            }

            return false;
        }

        public Inline GetNextInline(Inline inline) {
            var found = false;
            foreach (var il in Inlines) {
                if (found) {
                    return il;
                }

                if (il == inline) {
                    found = true;
                }
            }

            throw new ArgumentException("inline");
        }

        public Inline GetPrevInline(Inline inline) {
            Inline prev = null;
            foreach (var il in Inlines) {
                if (il == inline) {
                    if (prev == null) {
                        throw new ArgumentException("inline");
                    }
                    return prev;
                }
                prev = il;
            }

            throw new ArgumentException("inline");
        }

        
        // --- edit ---
        public void Add(Block block) {
            if (_blocks.IndexOf(block) > -1) {
                throw new ArgumentException("block already exists");
            }
            var offset = Length - 1;
            _blocks.Add(block);
            block.Parent = this;

            _offsetCache.Invalidate(_blocks.Count - 1);
            OnTextModified(StyledTextModificationKind.Insert, offset, block.Length);
            OnContentsChanged(this);
        }

        public void Insert(int index, Block block) {
            if (_blocks.IndexOf(block) > -1) {
                throw new ArgumentException("block already exists");
            }
            _blocks.Insert(index, block);
            block.Parent = this;

            _offsetCache.Invalidate(index);
            OnTextModified(StyledTextModificationKind.Insert, _offsetCache.GetOffset(index), block.Length);
            OnContentsChanged(this);
        }

        public void InsertBefore(Block block) {
            Insert(0, block);
        }

        public void InsertAfter(Block block) {
            Insert(_blocks.Count, block);
        }

        public void Remove(Block block) {
            var blockIndex = _blocks.IndexOf(block);
            if (blockIndex == -1) {
                throw new ArgumentException("block not exists");
            }
            var offset = _offsetCache.GetOffset(blockIndex);

            _blocks.Remove(block);
            block.Parent = null;

            _offsetCache.Invalidate(blockIndex);
            OnTextModified(StyledTextModificationKind.Remove, offset, block.Length);
            OnContentsChanged(this);
        }

        public void RemoveAt(int index) {
            if (index < 0 || index >= _blocks.Count) {
                throw new ArgumentOutOfRangeException("index");
            }
            var block = _blocks[index];
            var offset = _offsetCache.GetOffset(index);

            _blocks.RemoveAt(index);
            block.Parent = null;

            _offsetCache.Invalidate(index);
            OnTextModified(StyledTextModificationKind.Remove, offset, block.Length);
            OnContentsChanged(this);
        }

        // --- split and merge ---
        public Inline SplitInline(Inline inline, int charIndexInInline) {
            var splitted = (Inline) inline.Clone();
            splitted.Remove(0, charIndexInInline);
            return SplitInline(inline, charIndexInInline, splitted);
        }

        /// <summary>
        /// splittedには以前MergeInlines()に渡したmergedを渡す．
        /// undoでのmergeとの対称性のため．
        /// </summary>
        public Inline SplitInline(Inline inline, int charIndexInInline, Inline splitted) {
            Contract.Requires(inline != null && !inline.IsLineEndCharacter);

            var parent = (LineSegment) inline.Parent;
            var inlineIndex = parent._Inlines.IndexOf(inline);
            inline.Remove(charIndexInInline);
            //splitted.Remove(0, charIndexInInline);//
            parent.Insert(inlineIndex + 1, splitted);
            return splitted;
        }

        /// <summary>
        /// targetにmergedの内容を追加してmergedを削除する．
        /// </summary>
        public void MergeInlines(Inline target, Inline merged) {
            Contract.Requires(
                target != null && merged != null &&
                !target.IsLineEndCharacter && !merged.IsLineEndCharacter
            );

            target.Append(merged.Text);
            var parent = merged.Parent as LineSegment;
            if (parent != null) {
                parent.Remove(merged);
            }
        }

        public LineSegment SplitLineSegment(LineSegment line, int inlineIndexInLine) {
            return SplitLineSegment(line, inlineIndexInLine, line.Clone() as LineSegment);
        }

        /// <summary>
        /// splittedには以前MergeLineSegments()に渡したmergedを渡す．
        /// undoでのmergeとの対称性のため．
        /// </summary>
        public LineSegment SplitLineSegment(LineSegment line, int inlineIndexInLine, LineSegment splitted) {
            Contract.Requires(line != null);

            var parent = line.Parent as Block;
            var lineIndex = parent._Lines.IndexOf(line);

            var moveds = new List<Inline>();
            for (int i = inlineIndexInLine, len = line._Inlines.Count; i < len - 1; ++i) {
                /// 最後のline breakは移動しなくてよい
                moveds.Add(line._Inlines[i]);
            }

            moveds.ForEach(
                moved => {
                    line.Remove(moved);
                    splitted.InsertAfter(moved);
                }
            );

            parent.Insert(lineIndex + 1, splitted);

            return splitted;
        }

        /// <summary>
        /// targetにmergedの内容を追加してmergedを削除する．
        /// </summary>
        public void MergeLineSegments(LineSegment target, LineSegment merged) {
            Contract.Requires(target != null && merged != null);

            var mergedInlines = new List<Inline>();
            foreach (var inline in merged._Inlines) {
                if (!inline.IsLineEndCharacter) {
                    mergedInlines.Add(inline);
                }
            }

            mergedInlines.ForEach(
                inline => {
                    merged.Remove(inline);
                    target.InsertAfter(inline);
                }
            );

            var parent = merged.Parent as Block;
            if (parent != null) {
                parent.Remove(merged);
            }
        }

        public Block SplitBlock(Block block, int lineIndexInBlock) {
            return SplitBlock(block, lineIndexInBlock, block.Clone() as Block);
        }

        /// <summary>
        /// splittedには以前MergeBlocks()に渡したmergedを渡す．
        /// undoでのmergeとの対称性のため．
        /// </summary>
        public Block SplitBlock(Block block, int lineIndexInBlock, Block splitted) {
            Contract.Requires(block != null);

            var parent = block.Parent as StyledText;
            var blockIndex = parent._Blocks.IndexOf(block);

            var moveds = new List<LineSegment>();
            for (int i = lineIndexInBlock, len = block._Lines.Count; i < len; ++i) {
                moveds.Add(block._Lines[i]);
            }

            moveds.ForEach(
                moved => {
                    block.Remove(moved);
                    splitted.InsertAfter(moved);
                }
            );

            parent.Insert(blockIndex + 1, splitted);

            return splitted;
        }

        public void MergeBlocks(Block target, Block merged) {
            Contract.Requires(target != null && merged != null);

            var mergedLines = new List<LineSegment>();
            foreach (var line in merged._Lines) {
                mergedLines.Add(line);
            }

            mergedLines.ForEach(
                line => {
                    merged.Remove(line);
                    target.InsertAfter(line);
                }
            );

            var parent = merged.Parent as StyledText;
            if (parent != null) {
                parent.Remove(merged);
            }
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected internal override void OnChildrenTextModified(
            Flow sender, StyledTextModificationKind kind, int offset, int length
        ) {
            var block = sender as Block;
            if (block != null) {
                _offsetCache.Invalidate(block);
                OnTextModified(kind, offset + _offsetCache.GetOffset(block), length);
            }
        }
        
        protected override void OnContentsChanged(Flow changed) {
            //Logger.Debug("StyledText Contents Changed " + changed);
            var handler = ContentsChanged;
            if (handler != null) {
                handler(this, new ContentsChangedEventArgs(changed));
            }
        }

        protected override void OnTextModified(StyledTextModificationKind kind, int offset, int length) {
            _lineContainer.Dirty();

            var handler = TextModified;
            if (handler != null) {
                handler(this, new StyledTextModifiedEventArgs(kind, offset, length));
            }
        }


        // ------------------------------
        // private
        // ------------------------------
        private string[] CreateLineStrings() {
            var ret = new List<string>();
            foreach (var block in _blocks) {
                foreach (var line in block.Lines) {
                    ret.Add(line);
                }
            }
            return ret.ToArray();
        }


        // ========================================
        // constructor
        // ========================================
        /// <summary>
        /// _contensの内容を返す．
        /// </summary>
        private class InlinesEnumerable: IEnumerable<Inline> {
            // ========================================
            // field
            // ========================================
            private StyledText _owner;

            // ========================================
            // constructor
            // ========================================
            public InlinesEnumerable(StyledText owner) {
                _owner = owner;
            }

            // ========================================
            // method
            // ========================================
            public IEnumerator<Inline> GetEnumerator() {
                foreach (var block in _owner._blocks) {
                    foreach (var lineseg in block.LineSegments) {
                        foreach (var inline in lineseg.Inlines) {
                            yield return inline;
                        }
                    }
                }
            }

            global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }
        }

        private class LineSegmentsEnumerable: IEnumerable<LineSegment> {
            // ========================================
            // field
            // ========================================
            private StyledText _owner;

            // ========================================
            // constructor
            // ========================================
            public LineSegmentsEnumerable(StyledText owner) {
                _owner = owner;
            }

            // ========================================
            // method
            // ========================================
            public IEnumerator<LineSegment> GetEnumerator() {
                foreach (var block in _owner._blocks) {
                    foreach (var lineseg in block.LineSegments) {
                        yield return lineseg;
                    }
                }
            }

            global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator() {
                return GetEnumerator();
            }
        }
   }
}
