/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Common.Externalize;
using Mkamo.Model.Core;
using System.Drawing;
using Mkamo.Common.Structure;
using System.Collections.ObjectModel;
using Mkamo.Container.Core;
using Mkamo.Common.Association;
using Mkamo.Common.Collection;
using Mkamo.Common.Event;
using Mkamo.Common.String;
using Mkamo.Common.Core;

namespace Mkamo.Model.Memo {
    [Externalizable(Type = typeof(MemoTag), FactoryMethodType = typeof(MemoFactory), FactoryMethod = "CreateTag")]
    [Entity(OnLoading = "OnLoading", OnLoaded = "OnLoaded")]
    public class MemoTag: MemoElement {
        // ========================================
        // field
        // ========================================
        private string _name;

        private MemoTag _superTag;
        private AssociationCollection<MemoTag> _subTags;

        private AssociationCollection<Memo> _memos;

        // ========================================
        // constructor
        // ========================================
        protected internal MemoTag() {
            _subTags = new AssociationCollection<MemoTag>(
                subTag => {
                    if (subTag.SuperTag != this) {
                        subTag.SuperTag = this;
                    }
                },
                subTag => subTag.SuperTag = null,
                this,
                "SubTags"
            );
            _subTags.DetailedPropertyChanged += (sender, e) => OnPropertyChanged(this, e);

            _memos = new AssociationCollection<Memo>(
                memo => {
                    if (!memo.Tags.Contains(this)) {
                        memo.AddTag(this);
                    }
                },
                memo => memo.RemoveTag(this),
                this,
                "Memos"
            );
            _memos.DetailedPropertyChanged += (sender, e) => OnPropertyChanged(this, e);
        }

        // ========================================
        // property
        // ========================================
        [Persist, External]
        public virtual string Name {
            get { return _name; }
            set {
                if (_name == value) {
                    return;
                }
                var old = _name;
                _name = value;
                OnPropertySet(this, "Name", old, value);
            }
        }

        [Persist, External]
        public virtual MemoTag SuperTag {
            get { return _superTag; }
            set {
                if (value == this) {
                    return;
                }

                var old = _superTag;
                var result = AssociationUtil.EnsureAssociation(
                    _superTag,
                    value,
                    tag => _superTag = tag,
                    tag => {
                        if (!tag.SubTags.Contains(this)) {
                            tag.AddSubTag(this);
                        }
                    },
                    tag => tag.RemoveSubTag(this)
                );
                if (result != AssociationUtil.EnsureResult.None) {
                    OnPropertySet(this, "SuperTag", old, value);
                }
            }
        }

        [Persist(Add = "AddSubTag"), External(Add = "AddSubTag")]
        public virtual IEnumerable<MemoTag> SubTags {
            get { return _subTags; }
        }

        [Persist(Add = "AddMemo"), External(Add = "AddMemo")]
        public virtual IEnumerable<Memo> Memos {
            get { return _memos; }
        }

        public virtual string FullName {
            get {
                var ret = new StringBuilder(_name);
                var super = _superTag;
                while (super != null) {
                    ret.Insert(0, super.Name + "/");
                    super = super.SuperTag;
                }
                return ret.ToString();
            }
        }

        // ========================================
        // method
        // ========================================
        public override string ToString() {
            return Name?? "";
        }

        public virtual string GetIndentedName(int indentLevel) {
            var indent = StringUtil.Repeat(" ", indentLevel);

            var ret = new StringBuilder();
            var super = _superTag;
            while (super != null) {
                ret.Append(indent);
                super = super.SuperTag;
            }
            ret.Append(_name);

            return ret.ToString();
        }

        /// <summary>
        /// thisがotherの祖先であるかどうかを返す．自分自身の場合もtrue．
        /// </summary>
        public virtual bool IsAscendant(MemoTag other) {
            var ite = new Iterator<MemoTag>(this, tag => tag.SubTags);
            return ite.Contains(other);
        }

        /// <summary>
        /// thisがotherの子孫であるかどうかを返す．自分自身の場合もtrue．
        /// </summary>
        public virtual bool IsDescendant(MemoTag other) {
            var ite = new Iterator<MemoTag>(this, tag => new[] { tag.SuperTag, });
            return ite.Contains(other);
        }

        // --- sub tag ---
        [Dirty]
        public virtual void AddSubTag(MemoTag sub) {
            if (IsAscendant(sub)) {
                return;
            }
            _subTags.Add(sub);
        }

        [Dirty]
        public virtual void RemoveSubTag(MemoTag sub) {
            _subTags.Remove(sub);
        }

        [Dirty]
        public virtual void ClearSubTags() {
            _subTags.Clear();
        }

        // --- memo ---
        [Dirty]
        public virtual void AddMemo(Memo memo) {
            _memos.Add(memo);
        }

        [Dirty]
        public virtual void RemoveMemo(Memo memo) {
            _memos.Remove(memo);
        }

        [Dirty]
        public virtual void ClearMemos() {
            _memos.Clear();
        }


        public virtual void OnLoading() {
            _memos.DisableAssociator = true;
        }

        public virtual void OnLoaded() {
            _memos.DisableAssociator = false;
        }
    }

}
