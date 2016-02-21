/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Common.DataType;
using DataType = Mkamo.Common.DataType;
using Mkamo.Common.Collection;
using Mkamo.Common.Util;
using System.Runtime.Serialization;
using Mkamo.StyledText.Internal.Core;
using Mkamo.Common.String;
using System.Collections.ObjectModel;
using Mkamo.Common.Forms.Drawing;
using System.Drawing;
using Mkamo.Common.Diagnostics;
using Mkamo.StyledText.Writer;

namespace Mkamo.StyledText.Core {
    /// <summary>
    /// Block．最後にBlockBreakが必ず付く．
    /// </summary>
    [Serializable]
    [DataContract]
    public abstract class Block: Flow, ILineContainer {
        // ========================================
        // field
        // ========================================
        [DataMember]
        private HorizontalAlignment _horizontalAlign = HorizontalAlignment.Left;
        [DataMember]
        private List<LineSegment> _lines = new List<LineSegment>();
        [DataMember]
        private int _lineSpace = 2;
        [DataMember]
        private Insets _padding = new Insets(2);

        [NonSerialized]
        private LineContainerSupport _lineContainer;

        [NonSerialized]
        private FlowOffsetCache<LineSegment> _offsetCache;

        // ========================================
        // constructor
        // ========================================
        public Block() {
            _offsetCache = new FlowOffsetCache<LineSegment>(_lines);
            _lineContainer = new LineContainerSupport(CreateLineStrings, "\r");

            var lastLine = new LineSegment(new BlockBreak());
            _lines.Add(lastLine);
            lastLine.Parent = this;
        }

        public Block(Run run): this() {
            InsertAfter(run);
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) {
            _offsetCache = new FlowOffsetCache<LineSegment>(_lines);
            _lineContainer = new LineContainerSupport(CreateLineStrings, "\r");

            /// 最初行間を0にしていたのを変更したための措置
            // todo: 行間を設定可能にしたときに削除する
            _lineSpace = 2;
        }

        // ========================================
        // property
        // ========================================
        // === Flow ==========
        public override string Text {
            get {
                var ret = new StringBuilder();
                foreach (var line in LineSegments) {
                    ret.Append(line.Text);
                }
                return ret.ToString();
            }

            set {
                throw new NotImplementedException();
            }
        }
        public override int Length {
            get { return _offsetCache.Length; }
        }

        public override IEnumerable<Flow> Children {
            get {
                foreach (var line in _lines) {
                    yield return line;
                }
            }
        }

        //public override Color Color {
        //    get { return Parent.Color; }
        //    set { base.Color = value; }
        //}


        // === Block ==========
        /// <summary>
        /// 各行の文字列を格納した配列を返す．
        /// 行末の改行，改ブロック記号は含まれない．
        /// </summary>
        public virtual string[] Lines {
            get { return _lineContainer.Lines; }
        }

        /// <summary>
        /// 行数を返す．
        /// </summary>
        public virtual int LineCount {
            get { return _lines.Count; }
        }

        public virtual IEnumerable<LineSegment> LineSegments {
            get { return _lines; }
        }

        public HorizontalAlignment HorizontalAlignment {
            get { return _horizontalAlign; }
            set {
                if (value == _horizontalAlign) {
                    return;
                }
                _horizontalAlign = value;
                OnContentsChanged(this);
            }
        }

        public int LineSpace {
            get { return _lineSpace; }
            set {
                if (value == _lineSpace) {
                    return;
                }
                _lineSpace = value;
                OnContentsChanged(this);
            }
        }

        public Insets Padding {
            get { return _padding; }
            set {
                _padding = value;
                if (value == _padding) {
                    return;
                }
                _padding = value;
                OnContentsChanged(this);
            }
        }

        public bool IsEmpty {
            get {
                return
                    LineCount == 1 &&
                    _Lines[0]._Inlines.Count == 1;
            }
        }

        // ------------------------------
        // internal
        // ------------------------------
        internal List<LineSegment> _Lines {
            get { return _lines; }
        }

        // ========================================
        // method
        // ========================================
        public override object Clone() {
            var ret = (Block) base.Clone();
            ret.RemoveAt(0);
            return ret;
        }

        public override void Transfer(Flow flow) {
            base.Transfer(flow);

            var block = flow as Block;
            if (block == null) {
                return;
            }
            block._horizontalAlign = _horizontalAlign;
            block._lineSpace = _lineSpace;
            block._padding = _padding;
        }

        public override object CloneDeeply() {
            var ret = (Block) Clone();
            foreach (var line in LineSegments) {
                var childClone = (LineSegment) line.CloneDeeply();
                ret.InsertAfter(childClone);
            }
            return ret;
        }


        public virtual Block CopyRange(Range copyRange) {
            Contract.Requires(copyRange.Start >= 0 && copyRange.End < Length);

            var ret = (Block) Clone();

            foreach (var line in LineSegments) {
                var lineRange = new Range(_offsetCache.GetOffset(line), line.Length);

                if (copyRange.Contains(lineRange)) {
                    var childClone = (LineSegment) line.CloneDeeply();
                    ret.InsertAfter(childClone);

                } else if (lineRange.Contains(copyRange)) {
                    var offset = copyRange.Start - lineRange.Start;
                    var childCopy = (LineSegment) line.CopyRange(new Range(offset, copyRange.Length));
                    ret.InsertAfter(childCopy);

                } else if (copyRange.Intersects(lineRange)) {

                    if (lineRange.Start < copyRange.Start) {
                        var offset = copyRange.Start - lineRange.Start;
                        var childCopy = (LineSegment) line.CopyRange(new Range(offset, lineRange.End - copyRange.Start + 1));
                        ret.InsertAfter(childCopy);

                    } else {
                        var childCopy = (LineSegment) line.CopyRange(new Range(0, copyRange.End - lineRange.Start + 1));
                        ret.InsertAfter(childCopy);
                    }
                
                }
            }

            return ret;
        }

        public virtual IEnumerable<Inline> CopyInlines(Range copyRange) {
            Contract.Requires(copyRange.Start >= 0 && copyRange.End < Length);

            var ret = new List<Inline>();

            foreach (var line in LineSegments) {
                var lineRange = new Range(_offsetCache.GetOffset(line), line.Length);

                if (copyRange.Contains(lineRange)) {
                    var childClone = (LineSegment) line.CloneDeeply();
                    ret.AddRange(childClone.Inlines);

                } else if (lineRange.Contains(copyRange)) {
                    var offset = copyRange.Start - lineRange.Start;
                    var childCopy = line.CopyInlines(new Range(offset, copyRange.Length));
                    ret.AddRange(childCopy);

                } else if (copyRange.Intersects(lineRange)) {

                    if (lineRange.Start < copyRange.Start) {
                        var offset = copyRange.Start - lineRange.Start;
                        var childCopy = line.CopyInlines(new Range(offset, lineRange.End - copyRange.Start + 1));
                        ret.AddRange(childCopy);

                    } else {
                        var childCopy = line.CopyInlines(new Range(0, copyRange.End - lineRange.Start + 1));
                        ret.AddRange(childCopy);
                    }
                }
            }

            return ret;
        }



        public override string ToPlainText(PlainTextWriterSettings settings) {
            var ret = new StringBuilder();
            foreach (var line in LineSegments) {
                ret.Append(line.ToPlainText(settings));
            }
            return ret.ToString();
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

        // === Block ==========
        /// <summary>
        /// charIndexはBlockで0起点．
        /// </summary>
        public LineSegment GetLineSegmentAtLocal(int charIndex, out int lineIndex, out int charIndexInLine) {
            if (charIndex < 0) {
                throw new ArgumentOutOfRangeException("charIndex");
            }

            var cIndex = 0;
            var items = _offsetCache.Items;
            for (int i = 0; i < items.Count; ++i) {
                var item = items[i];
                var len = item.Length;

                if (charIndex >= cIndex && charIndex < cIndex + len) {
                    lineIndex = i;
                    charIndexInLine = charIndex - cIndex;
                    return _lines[i];
                }

                cIndex += len;
            }

            throw new ArgumentOutOfRangeException("charIndex");
        }

        public int GetLineSegmentLocalOffset(LineSegment line) {
            return _offsetCache.GetOffset(line);
        }

        public int GetLineSegmentLocalOffset(int lineIndex) {
            return _offsetCache.GetOffset(lineIndex);
        }


        // --- edit ---
        /// <summary>
        /// indexの位置にlineを挿入する．
        /// </summary>
        public void Insert(int index, LineSegment line) {
            if (index < 0 || index > _lines.Count) {
                throw new ArgumentOutOfRangeException("index");
            }
            var lineIndex = _lines.IndexOf(line);
            if (lineIndex > -1) {
                throw new ArgumentException("line already exists");
            }

            var prevLineOffset = 0;
            var prevLineLength = 0;
            if (index > 0 && _lines.Count > 1) {
                var item = _offsetCache.GetItem(_lines[index - 1]);
                prevLineOffset = item.Offset;
                prevLineLength = item.Length;
            }

            if (index == _lines.Count) {
                /// 最後の行の追加の場合はblock breakを付け変える
                if (_lines.Count > 0) {
                    _lines[_lines.Count - 1]._LineBreak = new LineBreak();
                }
                line._LineBreak = new BlockBreak();
            }
            _lines.Insert(index, line);
            line.Parent = this;

            _offsetCache.Invalidate(index);

            OnTextModified(
                StyledTextModificationKind.Insert, prevLineOffset + prevLineLength, line.Length
            );
            OnContentsChanged(this);
        }

        /// <summary>
        /// 最初の位置にlineを挿入する．
        /// </summary>
        public void InsertBefore(LineSegment line) {
            Insert(0, line);
        }

        /// <summary>
        /// 最後の行としてlineを挿入する．
        /// </summary>
        public void InsertAfter(LineSegment line) {
            Insert(_lines.Count, line);
        }

        /// <summary>
        /// 最初の位置にinlineを挿入する．
        /// </summary>
        public void InsertBefore(Inline inline) {
            _lines[0].InsertBefore(inline);
        }

        /// <summary>
        /// BlockBreakの直前にinlineを挿入する．
        /// </summary>
        public void InsertAfter(Inline inline) {
            _lines[_lines.Count - 1].InsertAfter(inline);
        }
        
        /// <summary>
        /// contentを削除する．
        /// </summary>
        public void Remove(LineSegment line) {
            var lineIndex = _lines.IndexOf(line);
            if (lineIndex == -1) {
                throw new ArgumentException("line not exists");
            }

            var lineOffset = _offsetCache.GetOffset(line);

            if (lineIndex == _lines.Count - 1) {
                /// 最後の行の削除の場合，直前の行にblock breakを設定
                if (_lines.Count > 1) {
                    _lines[_lines.Count - 2]._LineBreak = new BlockBreak();
                }
            }
            _lines.Remove(line);
            line.Parent = null;

            _offsetCache.Invalidate(lineIndex);
            OnTextModified(StyledTextModificationKind.Remove, lineOffset, line.Length);
            OnContentsChanged(this);
        }
        

        /// <summary>
        /// index番目のcontentを削除する．
        /// </summary>
        public void RemoveAt(int index) {
            if (index < 0 || index > _lines.Count - 1) {
                throw new ArgumentOutOfRangeException("index");
            }

            var line = _lines[index];
            var lineOffset = _offsetCache.GetOffset(line);

            if (index == _lines.Count - 1) {
                /// 最後の行の削除の場合，直前の行にblock breakを設定
                if (_lines.Count > 1) {
                    _lines[_lines.Count - 2]._LineBreak = new BlockBreak();
                }
            }
            _lines.RemoveAt(index);
            line.Parent = null;

            _offsetCache.Invalidate(index);
            OnTextModified(
                StyledTextModificationKind.Remove, lineOffset, line.Length
            );
            OnContentsChanged(this);
        }

        public void Remove(Inline inline) {
            foreach (var line in _lines) {
                var index = line._Inlines.IndexOf(inline);
                if (index > -1) {
                    line.RemoveAt(index);
                    return;
                }
            }
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected internal override void OnChildrenTextModified(
            Flow sender, StyledTextModificationKind kind, int offset, int length
        ) {
            var line = sender as LineSegment;
            if (line != null) {
                _offsetCache.Invalidate(line);
                OnTextModified(kind, _offsetCache.GetOffset(line) + offset, length);
            }
        }

        protected override void OnTextModified(StyledTextModificationKind kind, int offset, int length) {
            _lineContainer.Dirty();
            base.OnTextModified(kind, offset, length);
        }

        // ------------------------------
        // private
        // ------------------------------
        private string[] CreateLineStrings() {
            var ret = new List<string>();

            var line = new StringBuilder();
            foreach (var lineseg in _lines) {
                foreach (var inline in lineseg.Inlines) {
                    if (inline is BlockBreak) {
                        ret.Add(line.ToString());
                        line = null;
                        break;
                    } else if (inline is LineBreak) {
                        ret.Add(line.ToString());
                        line = new StringBuilder();
                    } else {
                        line.Append(inline.Text);
                    }
                }
            }

            return ret.ToArray();
        }

    }

}
