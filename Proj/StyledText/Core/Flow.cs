/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Forms.Descriptions;
using System.Drawing;
using Mkamo.Common.Collection;
using Mkamo.Common.Visitor;
using Mkamo.Common.Core;
using System.Runtime.Serialization;
using Mkamo.StyledText.Writer;
using Mkamo.StyledText.Internal.Core;

namespace Mkamo.StyledText.Core {
    [Serializable]
    [DataContract]
    [KnownType(typeof(StyledText))]
    [KnownType(typeof(Paragraph))]
    [KnownType(typeof(LineSegment))]
    [KnownType(typeof(Run))]
    [KnownType(typeof(LineBreak))]
    [KnownType(typeof(BlockBreak))]
    [KnownType(typeof(Anchor))]
    public abstract class Flow: IVisitable<Flow> {
        // ========================================
        // static field
        // ========================================
        public static FontDescription DefaultFont = SystemFontDescriptions.DefaultFont;
        private static Color DefaultColor = SystemColors.WindowText;

        // ========================================
        // field
        // ========================================
        [DataMember]
        private Flow _parent;

        [DataMember]
        private FontDescription _font;
        [DataMember]
        private Color _color = DefaultColor;
        [DataMember]
        private string _class;

        [NonSerialized]
        private VisitableSupport<Flow> _visitableSupport;

        // ========================================
        // constructor
        // ========================================
        protected Flow() {
            _visitableSupport = new VisitableSupport<Flow>(
                this,
                GetNextVisitable
            );
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) {
            _visitableSupport = new VisitableSupport<Flow>(
                this,
                GetNextVisitable
            );
        }

        // ========================================
        // property
        // ========================================
        // === Flow ==========
        public virtual StyledText Root {
            get { return _parent == null? null: Parent.Root; }
        }

        public virtual Flow Parent {
            get { return _parent; }
            internal set {
                if (value == _parent) {
                    return;
                }
                _parent = value;
                /// Parentの変更は自分の変更ではないものとして変更通知はしない
            }
        }

        public bool HasNextSibling {
            get {
                if (Parent == null) {
                    return false;
                }
                var children = Parent.Children;
                var index = children.IndexOf(this);
                if (index > -1) {
                    return (index < children.Count() - 1);
                }

                throw new InvalidOperationException();
            }
        }

        public bool HasPrevSibling {
            get {
                if (Parent == null) {
                    return false;
                }
                var children = Parent.Children;
                var index = children.IndexOf(this);
                if (index > -1) {
                    return (index > 0);
                }

                throw new InvalidOperationException();
            }
        }

        public Flow NextSibling {
            get {
                if (Parent == null) {
                    return null;
                }

                var foundThis = false;
                foreach (var child in Parent.Children) {
                    if (foundThis) {
                        return child;
                    } else {
                        if (child == this) {
                            foundThis = true;
                        }
                    }
                }

                throw new InvalidOperationException();
            }
        }

        public Flow PrevSibling {
            get {
                if (Parent == null) {
                    return null;
                }

                Flow prev = null;
                foreach (var child in Parent.Children) {
                    if (child == this) {
                        if (prev == null) {
                            throw new InvalidOperationException();
                        }
                        return prev;
                    }
                    prev = child;
                }

                throw new InvalidOperationException();
            }
        }


        public virtual FontDescription Font {
            get { return _font?? (Parent == null? null: Parent.Font); }
            set {
                if (value == _font) {
                    return;
                }
                _font = value;
                OnContentsChanged(this);
            }
        }

        public virtual Color Color {
            get { return _color; }
            set {
                if (value == _color) {
                    return;
                }
                _color = value;
                OnContentsChanged(this);
            }
        }

        public string Class {
            get { return _class ?? ""; }
            set{
                if (value == _class) {
                    return;
                }
                _class = value;
                OnContentsChanged(this);
            }
        }

        public abstract IEnumerable<Flow> Children { get; }

        /// <summary>
        /// 文字列としての表現。
        /// 文字数などもcaretの位置制御に使われる。
        /// </summary>
        public abstract string Text { get; set; }

        public abstract int Length { get; }

        // ------------------------------
        // internal
        // ------------------------------
        internal FontDescription _OwnFont {
            get { return _font; }
        }

        // ========================================
        // method
        // ========================================
        public override string ToString() {
            var ret = new StringBuilder();
            Accept(
                (flow) => {
                    ret.Append("<" + flow.GetType().Name + ">");
                    if (flow is Run) {
                        ret.Append(flow.Text);
                    }
                    return false;
                },

                (flow) => {
                    ret.Append("</" + flow.GetType().Name + ">");
                },

                NextVisitOrder.PositiveOrder
            );
             return ret.ToString();
        }

        public virtual object Clone() {
            var act = TypeService.Instance.GetDefaultActivator(GetType());
            var ret = (Flow) act();

            Transfer(ret);
            return ret;
        }
        
        public abstract object CloneDeeply();
        
        // === IAcceptable ==========
        public void Accept(IVisitor<Flow> visitor) {
            _visitableSupport.Accept(visitor);
        }

        public void Accept(IVisitor<Flow> visitor, NextVisitOrder order) {
            _visitableSupport.Accept(visitor, order);
        }

        public void Accept(Predicate<Flow> visitPred) {
            _visitableSupport.Accept(visitPred);
        }

        public void Accept(Predicate<Flow> visitPred, Action<Flow> endVisitAction, NextVisitOrder order) {
            _visitableSupport.Accept(visitPred, endVisitAction, order);
        }

        // === Flow ==========
        public virtual void Transfer(Flow flow) {
            flow._color = _color;
            flow._font = _font;
            flow._class = _class;
        }
        
        /// <summary>
        /// PlainTextとしてクリップボードに貼る付けるときなどに使用。
        /// caretの制御などには使われない。
        /// </summary>
        public abstract string ToPlainText(PlainTextWriterSettings settings);
        public abstract string ToHtmlText();

        // ------------------------------
        // protected
        // ------------------------------
        protected virtual void OnContentsChanged(Flow changed) {
            if (Parent != null) {
                Parent.OnContentsChanged(changed);
            }
        }

        protected virtual void OnTextModified(StyledTextModificationKind kind, int offset, int length) {
            if (Parent != null) {
                Parent.OnChildrenTextModified(this, kind, offset, length);
            }
        }

        /// <summary>
        /// 子からの変更通知を受ける．offsetはsender内での文字インデクス．
        /// </summary>
        protected internal abstract void OnChildrenTextModified(
            Flow sender, StyledTextModificationKind kind, int offset, int length
        );

        // ------------------------------
        // private
        // ------------------------------
        private IEnumerable<Flow> GetNextVisitable(Flow flow) {
            return flow.Children;
        }

    }
}
