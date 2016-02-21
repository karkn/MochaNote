/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Text;
using Mkamo.Common.Core;
using Mkamo.Common.Forms.Descriptions;
using System.Drawing;
using System.Runtime.Serialization;
using Mkamo.Common.DataType;
using Mkamo.StyledText.Util;
using Mkamo.Common.Diagnostics;
using Mkamo.Common.String;
using Mkamo.StyledText.Writer;

namespace Mkamo.StyledText.Core {
    /// <summary>
    /// インライン要素．
    /// 必ずLength > 0．
    /// </summary>
    [Serializable]
    [DataContract]
    public abstract class Inline: Flow {
        // ========================================
        // property
        // ========================================
        public override IEnumerable<Flow> Children {
            get { yield break; }
        }

        public bool IsControlCharacter {
            get { return IsLineEndCharacter || IsAnchorCharacter; }
        }

        public bool IsLineEndCharacter {
            get { return IsLineBreakCharacter || IsBlockBreakCharacter; }
        }

        public virtual bool IsLineBreakCharacter {
            get { return false; }
        }
        public virtual bool IsBlockBreakCharacter {
            get { return false; }
        }

        public virtual bool IsAnchorCharacter {
            get { return false; }
        }

        // ========================================
        // method
        // ========================================
        public override object CloneDeeply() {
            return Clone();
        }

        public virtual object CopyRange(Range range) {
            Contract.Requires(range.Start >= 0 && range.End < Length);
            var ret = Clone() as Inline;
            return ret;
        }

        public override string ToPlainText(PlainTextWriterSettings settings) {
            return Text;
        }

        public abstract void Append(string s);
        public abstract void Insert(int index, string s);
        public abstract void Remove(int index, int length);

        public void Remove(int index) {
            var range = new Range(0, Length);
            var removingRange = range.Latter(index);
            Remove(removingRange.Offset, removingRange.Length);
        }

        public virtual bool CanMerge(Inline other) {
            if (other == null) {
                return false;
            }
            if (GetType() != other.GetType()) {
                return false;
            }
            return Font.Equals(other.Font) && Color == other.Color && Class == other.Class;
        }


        protected internal override void OnChildrenTextModified(
            Flow sender, StyledTextModificationKind kind, int offset, int length
        ) {
            throw new InvalidOperationException("Inline must not have child");
        }
    }

    /// <summary>
    /// 汎用インライン要素
    /// </summary>
    [Serializable]
    [DataContract]
    public class Run: Inline {
        // ========================================
        // field
        // ========================================
        [DataMember]
        private StringBuilder _buffer = new StringBuilder();
        [DataMember]
        private Link _link;

        // ========================================
        // constructor
        // ========================================
        public Run() {
        }

        public Run(string s): this() {
            Text = s;
        }

        public Run(string s, Link link): this(s) {
            _link = link;
        }
        
        // ========================================
        // property
        // ========================================
        // === Flow ==========
        public override int Length {
           get { return _buffer.Length; }
        }

        // === Inline ==========
        public override string Text {
            get { return _buffer.ToString(); }
            set {
                if (value == null) {
                    value = "";
                }
                
                _buffer.Clear();
                _buffer.Append(Sanitize(value));

                OnTextModified(StyledTextModificationKind.Replace, 0, Length);
                OnContentsChanged(this);
            }
        }

        // === Run ==========
        public Link Link {
            get { return _link; }
            set {
                if (value == _link) {
                    return;
                }
                _link = value;
                OnContentsChanged(this);
            }
        }

        public bool HasLink {
            get { return _link != null; }
        }

        // ========================================
        // method
        // ========================================
        public override object Clone() {
            var ret = base.Clone() as Run;
            ret._buffer.Append(_buffer.ToString());
            return ret;
        }

        public override object CopyRange(Range range) {
            Contract.Requires(range.Start >= 0 && range.End < Length);

            var ret = Clone() as Inline;

            if (range.End < Length - 1) {
                ret.Remove(range.End + 1);
            }
            if (range.Start > 0) {
                ret.Remove(0, range.Start);
            }

            return ret;
        }


        // === Flow ==========
        public override void Transfer(Flow flow) {
            base.Transfer(flow);

            var run = flow as Run;
            if (run == null) {
                return;
            }
            run._link = _link == null? null: _link.Clone() as Link;
        }

        public void TransferWithoutLink(Flow flow) {
            base.Transfer(flow);
        }

        public override void Append(string s) {
            var offset = _buffer.Length - 1;
            var sanitized = Sanitize(s);
            _buffer.Append(sanitized);

            OnTextModified(StyledTextModificationKind.Insert, offset, sanitized.Length);
            OnContentsChanged(this);
        }

        public override void Insert(int index, string s) {
            var sanitized = Sanitize(s);
            _buffer.Insert(index, sanitized);

            OnTextModified(StyledTextModificationKind.Insert, index, sanitized.Length);
            OnContentsChanged(this);
        }

        public override void Remove(int index, int length) {
            var len = (index + length > Length)? Length - index: length;
            _buffer.Remove(index, len);

            OnTextModified(StyledTextModificationKind.Remove, index, len);
            OnContentsChanged(this);
        }

        public override string ToHtmlText() {
            var ret = new StringBuilder(Text);
            if (HasLink) {
                if (StringUtil.IsUrl(_link.Uri)) {
                    ret.Insert(0, "<a href=" + _link.Uri + ">");
                    ret.Append("</a>");
                }
            }
            if (Font != null) {
                if (Font.IsBold) {
                    ret.Insert(0, "<strong>");
                    ret.Append("</strong>");
                }
                if (Font.IsItalic) {
                    ret.Insert(0, "<em>");
                    ret.Append("</em>");
                }
                if (Font.IsStrikeout) {
                    ret.Insert(0, "<del>");
                    ret.Append("</del>");
                }
                if (Font.IsUnderline) {
                    ret.Insert(0, "<u>");
                    ret.Append("</u>");
                }
            }
            return ret.ToString();
        }

        // --- Inline ---
        public override bool CanMerge(Inline other) {
            var ret = base.CanMerge(other);
            if (ret) {
                var run = other as Run;
                return Link == run.Link;
            } else {
                return false;
            }
        }


        // ------------------------------
        // protected
        // ------------------------------
        /// <summary>
        /// 改行はスペースに変換．
        /// '\n'はBlockBreak，'\r'はLineBreakであらわすため．
        /// </summary>
        protected string Sanitize(string s) {
            return StyledTextUtil.NormalizeLineBreak(s, " ");
        }

    }

    [Serializable]
    [DataContract]
    public abstract class ControlCharacterInline: Inline {

        public override FontDescription Font {
            get {
                if (HasPrevSibling) {
                    var inline = PrevSibling as Inline;
                    return inline.Font;
                }
                return base.Font;
            }
            set {
                // base.Font = value;
                // do nothing
            }
        }

        public override void Append(string s) {
            throw new NotSupportedException();
        }

        public override void Insert(int index, string s) {
            throw new NotSupportedException();
        }

        public override void Remove(int index, int length) {
            throw new NotSupportedException();
        }

        public override bool CanMerge(Inline other) {
            return false;
        }
    }


    /// <summary>
    /// 改行．
    /// (改行，\n，LF，0x0A)
    /// </summary>
    [Serializable]
    [DataContract]
    public class LineBreak: ControlCharacterInline {
        public override int Length {
            get { return 1; }
        }

        public override string Text {
            get { return "\r"; }
            set {
                if (value != "\r") {
                    throw new ArgumentException("value");
                }
            }
        }

        public override bool IsLineBreakCharacter {
            get { return true; }
        }

        // ========================================
        // method
        // ========================================
        public override string ToPlainText(PlainTextWriterSettings settings) {
            return Environment.NewLine;
        }

        public override string ToHtmlText() {
            return "<br />";
        }
    }

    /// <summary>
    /// 改ブロック．
    /// (復帰，\r，CR，0x0D)
    /// </summary>
    [Serializable]
    [DataContract]
    public class BlockBreak: ControlCharacterInline {
        public override int Length {
            get { return 1; }
        }

        public override string Text {
            get { return "\n"; }
            set {
                if (value != "\n") {
                    throw new ArgumentException("value");
                }
            }
        }

        public override bool IsBlockBreakCharacter {
            get { return true; }
        }

        // ========================================
        // method
        // ========================================
        public override string ToPlainText(PlainTextWriterSettings settings) {
            return Environment.NewLine;
        }

        public override string ToHtmlText() {
            return "";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    [DataContract]
    public class Anchor: ControlCharacterInline {
        // ========================================
        // field
        // ========================================
        [DataMember]
        private string _id;

        // ========================================
        // constructor
        // ========================================
        public Anchor() {
            _id = Guid.NewGuid().ToString();
        }

        // ========================================
        // property
        // ========================================
        public override int Length {
            get { return 1; }
        }

        public override string Text {
            get { return "|"; }
            set {
                if (value != "|") {
                    throw new ArgumentException("value");
                }
            }
        }

        public override bool IsAnchorCharacter {
            get { return true; }
        }

        public string Id {
            get { return _id; }
        }

        // ========================================
        // method
        // ========================================
        public override void Transfer(Flow flow) {
            base.Transfer(flow);

            var anchor = flow as Anchor;
            if (anchor == null) {
                return;
            }
            anchor._id = _id;
        }

        public override string ToPlainText(PlainTextWriterSettings settings) {
            return "";
        }

        public override string ToHtmlText() {
            return "";
        }

        public void RenewId() {
            _id = Guid.NewGuid().ToString();
        }
    }

}
