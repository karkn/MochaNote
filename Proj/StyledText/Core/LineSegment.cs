/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Diagnostics;
using Mkamo.StyledText.Internal.Core;
using System.Runtime.Serialization;
using Mkamo.Common.DataType;
using System.Drawing;
using Mkamo.StyledText.Writer;

namespace Mkamo.StyledText.Core {
    /// <summary>
    /// LineBreak/BlockBreakは削除できないが，変更はできる．
    /// </summary>
    [Serializable]
    [DataContract]
    public class LineSegment: Flow {
        // ========================================
        // field
        // ========================================
        [DataMember]
        private List<Inline> _inlines = new List<Inline>();
        [DataMember]
        private Inline _lineBreak;

        [NonSerialized]
        private FlowOffsetCache<Inline> _offsetCache;

        // ========================================
        // constructor
        // ========================================
        public LineSegment(): this(new LineBreak()) {
        }

        public LineSegment(Run run): this(new LineBreak()) {
            InsertBefore(run);
        }

        public LineSegment(Inline lineBreak) {
            Contract.Requires(lineBreak.IsLineEndCharacter);

            _lineBreak = lineBreak;
            _inlines.Add(lineBreak);
            _lineBreak.Parent = this;

            _offsetCache = new FlowOffsetCache<Inline>(_inlines);
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) {
            _offsetCache = new FlowOffsetCache<Inline>(_inlines);
        }

        // ========================================
        // property
        // ========================================
        public override IEnumerable<Flow> Children {
            get {
                foreach (var inline in _inlines) {
                    yield return inline;
                }
            }
        }

        public override string Text {
            get {
                var ret = new StringBuilder();
                foreach (var content in _inlines) {
                    ret.Append(content.Text);
                }
                return ret.ToString();

            }
            set {
                _inlines.Clear();
                var run = new Run(value);
                _inlines.Add(run);
                run.Parent = this;

                _inlines.Add(_lineBreak);
                _lineBreak.Parent = this;

                _offsetCache.Clear();
                OnTextModified(StyledTextModificationKind.Replace, 0, Length);
                OnContentsChanged(this);
            }
        }

        public override int Length {
            get { return _offsetCache.Length; }
        }

        //public override Color Color {
        //    get { return Parent.Color; }
        //    set { base.Color = value; }
        //}

        public IEnumerable<Inline> Inlines {
            get { return _inlines; }
        }

        public int LineSpace {
            get {
                var block = Parent as Block;
                return block == null? 0: block.LineSpace;
            }
        }

        public HorizontalAlignment HorizontalAlignment {
            get {
                var block = Parent as Block;
                return block == null?
                    HorizontalAlignment.Left:
                    block.HorizontalAlignment;
            }
        }

        // ------------------------------
        // internal
        // ------------------------------
        internal List<Inline> _Inlines {
            get { return _inlines; }
        }

        internal Inline _LineBreak {
            get { return _lineBreak; }
            set {
                if (value == _lineBreak) {
                    return;
                }
                _inlines.RemoveAt(_inlines.Count - 1);
                _lineBreak.Parent = null;

                _lineBreak = value;
                _inlines.Add(value);
                _lineBreak.Parent = this;

                _offsetCache.Invalidate(_inlines.Count - 1);
                OnTextModified(
                    StyledTextModificationKind.Replace,
                    _offsetCache.GetOffset(_inlines.Count - 1),
                    _lineBreak.Length
                );
                OnContentsChanged(this);
            }
        }

        // ========================================
        // method
        // ========================================
        public override object Clone() {
            var ret = (LineSegment) base.Clone();
            ret._LineBreak = (Inline) _LineBreak.CloneDeeply();
            return ret;
        }

        public override object CloneDeeply() {
            var ret = (LineSegment) Clone();
            foreach (var inline in Inlines) {
                if (!(inline.IsLineEndCharacter)) {
                    var childClone = (Inline) inline.CloneDeeply();
                    ret.InsertAfter(childClone);
                }
            }
            return ret;
        }


        /// <summary>
        /// 最後はかならずLineBreak。
        /// </summary>
        public LineSegment CopyRange(Range copyRange) {
            Contract.Requires(copyRange.Start >= 0 && copyRange.End < Length);

            var ret = (LineSegment) Clone();
            foreach (var inline in Inlines) {
                var inlineRange = new Range(_offsetCache.GetOffset(inline), inline.Length);

                if (inline.IsLineEndCharacter) {
                    /// 最後のLine/BlockBreakはもともとretに入っているので無視
                    break;
                } else if (inline.IsAnchorCharacter) {
                    /// 同じIdのanchorが存在しないように無視
                    continue;
                }

                if (copyRange.Contains(inlineRange)) {
                    var childClone = (Inline) inline.CloneDeeply();
                    ret.InsertAfter(childClone);

                } else if (inlineRange.Contains(copyRange)) {
                    var offset = copyRange.Start - inlineRange.Start;
                    var childCopy = (Inline) inline.CopyRange(new Range(offset, copyRange.Length));
                    ret.InsertAfter(childCopy);

                } else if (copyRange.Intersects(inlineRange)) {
                    if (inlineRange.Start < copyRange.Start) {
                        var offset = copyRange.Start - inlineRange.Start;
                        var childCopy = (Inline) inline.CopyRange(new Range(offset, inlineRange.End - copyRange.Start + 1));
                        ret.InsertAfter(childCopy);

                    } else {
                        var childCopy = (Inline) inline.CopyRange(new Range(0, copyRange.End - inlineRange.Start + 1));
                        ret.InsertAfter(childCopy);
                    }
                }
            }
            return ret;
        }

        public IEnumerable<Inline> CopyInlines(Range copyRange) {
            Contract.Requires(copyRange.Start >= 0 && copyRange.End < Length);

            var ret = new List<Inline>();

            foreach (var inline in Inlines) {
                if (inline.IsAnchorCharacter) {
                    /// 同じIdのanchorが存在しないように無視
                    continue;
                }

                var inlineRange = new Range(_offsetCache.GetOffset(inline), inline.Length);

                if (copyRange.Contains(inlineRange)) {
                    var childClone = (Inline) inline.CloneDeeply();
                    ret.Add(childClone);

                } else if (inlineRange.Contains(copyRange)) {
                    var offset = copyRange.Start - inlineRange.Start;
                    var childCopy = (Inline) inline.CopyRange(new Range(offset, copyRange.Length));
                    ret.Add(childCopy);

                } else if (copyRange.Intersects(inlineRange)) {
                    if (inlineRange.Start < copyRange.Start) {
                        var offset = copyRange.Start - inlineRange.Start;
                        var childCopy = (Inline) inline.CopyRange(new Range(offset, inlineRange.End - copyRange.Start + 1));
                        ret.Add(childCopy);

                    } else {
                        var childCopy = (Inline) inline.CopyRange(new Range(0, copyRange.End - inlineRange.Start + 1));
                        ret.Add(childCopy);
                    }
                }
            }
            return ret;

        }


        //public override void Transfer(Flow flow) {
        //    base.Transfer(flow);
        //}

        public override string ToPlainText(PlainTextWriterSettings settings) {
            var ret = new StringBuilder();
            foreach (var content in _inlines) {
                ret.Append(content.ToPlainText(settings));
            }
            return ret.ToString();
        }

        public override string ToHtmlText() {
            var ret = new StringBuilder();
            foreach (var content in _inlines) {
                ret.Append(content.ToHtmlText());
            }
            return ret.ToString();
        }

        // === LineSegment ==========
        public Inline GetInlineAtLocal(int charIndex, out int inlineIndex, out int charIndexInInine) {
            if (charIndex < 0) {
                throw new ArgumentOutOfRangeException("charIndex");
            }

            var cIndex = 0;
            var items = _offsetCache.Items;
            for (int i = 0; i < items.Count; ++i) {
                var item = items[i];
                var len = item.Length;

                if (charIndex >= cIndex && charIndex < cIndex + len) {
                    inlineIndex = i;
                    charIndexInInine = charIndex - cIndex;
                    return _inlines[i];
                }

                cIndex += len;
            }

            throw new ArgumentOutOfRangeException("charIndex");
        }

        public int GetInlineLocalOffset(Inline inline) {
            return _offsetCache.GetOffset(inline);
        }

        public int GetInlineLocalOffset(int inlineIndex) {
            return _offsetCache.GetOffset(inlineIndex);
        }

        // --- edit ---
        public void Insert(int index, Inline inline) {
            if (inline is LineBreak || inline is BlockBreak) {
                throw new ArgumentException("inline");
            }
            if (index < 0 || index > _inlines.Count - 1) {
                throw new ArgumentOutOfRangeException("index");
            }
            if (_inlines.IndexOf(inline) > -1) {
                throw new ArgumentException("content already exists");
            }

            var prevInlineOffset = 0;
            var prevInlineLength = 0;
            if (index > 0 && _inlines.Count > 1) {
                var item = _offsetCache.GetItem(_inlines[index - 1]);
                prevInlineOffset = item.Offset;
                prevInlineLength = item.Length;
            }

            _inlines.Insert(index, inline);
            inline.Parent = this;

            _offsetCache.Invalidate(index);
            OnTextModified(
                StyledTextModificationKind.Insert, prevInlineOffset + prevInlineLength, inline.Length
            );
            OnContentsChanged(this);
        }

        /// <summary>
        /// 最初の位置にcontentを挿入する．
        /// </summary>
        public void InsertBefore(Inline inline) {
            Insert(0, inline);
        }

        /// <summary>
        /// Line/BlockBreakの直前にcontentを挿入する．
        /// </summary>
        public void InsertAfter(Inline inline) {
            Insert(_inlines.Count - 1, inline);
        }

        /// <summary>
        /// inlineを削除する．
        /// </summary>
        public void Remove(Inline inline) {
            if (inline == _lineBreak) {
                throw new ArgumentException("inline");
            }
            var inlineIndex = _inlines.IndexOf(inline);
            if (inlineIndex == -1) {
                throw new ArgumentException("content not exists");
            }

            var inlineOffset = _offsetCache.GetOffset(inlineIndex);

            _inlines.Remove(inline);
            inline.Parent = null;

            _offsetCache.Invalidate(inlineIndex);
            OnTextModified(StyledTextModificationKind.Remove, inlineOffset, inline.Length);
            OnContentsChanged(this);
        }

        /// <summary>
        /// inlineを削除する．
        /// </summary>
        public void RemoveAt(int index) {
            if (index < 0 || index > _inlines.Count - 2) {
                throw new ArgumentOutOfRangeException("index");
            }

            var inline = _inlines[index];
            var inlineOffset = _offsetCache.GetOffset(inline);

            _inlines.RemoveAt(index);
            inline.Parent = null;

            _offsetCache.Invalidate(index);
            OnTextModified(
                StyledTextModificationKind.Remove, inlineOffset, inline.Length
            );
            OnContentsChanged(this);
        }

        // ------------------------------
        // protected internal
        // ------------------------------
        protected internal override void OnChildrenTextModified(
            Flow sender, StyledTextModificationKind kind, int offset, int length
        ) {
            var inline = sender as Inline;
            if (inline != null) {
                _offsetCache.Invalidate(inline);
                OnTextModified(kind, _offsetCache.GetOffset(inline), length);
            }
        }

    }
}
