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
using Mkamo.Container.Core;

namespace Mkamo.Model.Memo {
    [Externalizable(Type = typeof(MemoQuery), FactoryMethodType = typeof(MemoFactory), FactoryMethod = "CreateQuery")]
    public class MemoQuery: MemoElement {
        // ========================================
        // field
        // ========================================
        private string _condition;

        private string _title;

        private bool _narrowByTagIds;
        private List<string> _tagIds;
        private bool _noTag;
        private MemoConditionCompoundKind _tagCompoundKind = MemoConditionCompoundKind.Any;

        private bool _narrowByMarkKinds;
        private List<MemoMarkKind> _markKinds;
        private bool _noMarkKind;
        private MemoConditionCompoundKind _markCompoundKind = MemoConditionCompoundKind.Any;

        private bool _narrowByTimeSpan;
        private MemoTimeSpan _timeSpan;

        private bool _narrowByRecentTimeSpan;
        private MemoRecentTimeSpan _recentTimeSpan;

        private bool _narrowByImportanceKind;
        private List<MemoImportanceKind> _importanceKinds;
        //private MemoConditionCompoundKind _importanceCompoundKind = MemoConditionCompoundKind.Any;

        // ========================================
        // constructor
        // ========================================
        protected internal MemoQuery() {
            _tagIds = new List<string>();
            _markKinds = new List<MemoMarkKind>();
            _importanceKinds = new List<MemoImportanceKind>();
        }

        // ========================================
        // property
        // ========================================
        [Persist, External]
        public virtual string Condition {
            get { return _condition; }
            set {
                if (_condition == value) {
                    return;
                }
                var old = _condition;
                _condition = value;
                OnPropertySet(this, "Condition", old, value);
            }
        }

        [Persist, External]
        public virtual string Title {
            get { return _title; }
            set {
                if (_title == value) {
                    return;
                }
                var old = _title;
                _title = value;
                OnPropertySet(this, "Title", old, value);
            }
        }

        [Persist, External]
        public virtual bool NarrowByTagIds {
            get { return _narrowByTagIds; }
            set {
                if (_narrowByTagIds == value) {
                    return;
                }
                var old = _narrowByTagIds;
                _narrowByTagIds = value;
                OnPropertySet(this, "NarrowByTagIds", old, value);
            }
        }

        [Persist(Add = "AddTagId"), External(Add = "AddTagId")]
        public virtual IEnumerable<string> TagIds {
            get { return _tagIds; }
        }

        [Persist, External]
        public virtual bool NoTag {
            get { return _noTag; }
            set {
                if (_noTag == value) {
                    return;
                }
                var old = _noTag;
                _noTag = value;
                OnPropertySet(this, "NoTag", old, value);
            }
        }

        [Persist, External]
        public virtual MemoConditionCompoundKind TagCompoundKind {
            get { return _tagCompoundKind; }
            set {
                if (_tagCompoundKind == value) {
                    return;
                }
                var old = _tagCompoundKind;
                _tagCompoundKind = value;
                OnPropertySet(this, "TagCompoundKind", old, value);
            }
        }

        [Persist, External]
        public virtual bool NarrowByMarkKinds {
            get { return _narrowByMarkKinds; }
            set {
                if (_narrowByMarkKinds == value) {
                    return;
                }
                var old = _narrowByMarkKinds;
                _narrowByMarkKinds = value;
                OnPropertySet(this, "NarrowByMarkKinds", old, value);
            }
        }

        [Persist(Add = "AddMarkKind"), External(Add = "AddMarkKind")]
        public virtual IEnumerable<MemoMarkKind> MarkKinds {
            get { return _markKinds; }
        }

        [Persist, External]
        public virtual bool NoMarkKind {
            get { return _noMarkKind; }
            set {
                if (_noMarkKind == value) {
                    return;
                }
                var old = _noMarkKind;
                _noMarkKind = value;
                OnPropertySet(this, "NoMarkKind", old, value);
            }
        }

        [Persist, External]
        public virtual MemoConditionCompoundKind MarkCompoundKind {
            get { return _markCompoundKind; }
            set {
                if (_markCompoundKind == value) {
                    return;
                }
                var old = _markCompoundKind;
                _markCompoundKind = value;
                OnPropertySet(this, "MarkCompoundKind", old, value);
            }
        }


        [Persist, External]
        public virtual bool NarrowByTimeSpan {
            get { return _narrowByTimeSpan; }
            set {
                if (_narrowByTimeSpan == value) {
                    return;
                }
                var old = _narrowByTimeSpan;
                _narrowByTimeSpan = value;
                OnPropertySet(this, "NarrowByTimeSpan", old, value);
            }
        }

        [Persist, External]
        public virtual MemoTimeSpan TimeSpan {
            get { return _timeSpan; }
            set {
                if (_timeSpan.Equals(value)) {
                    return;
                }
                var old = _timeSpan;
                _timeSpan = value;
                OnPropertySet(this, "TimeSpan", old, value);
            }
        }

        [Persist, External]
        public virtual bool NarrowByRecentTimeSpan {
            get { return _narrowByRecentTimeSpan; }
            set {
                if (_narrowByRecentTimeSpan == value) {
                    return;
                }
                var old = _narrowByRecentTimeSpan;
                _narrowByRecentTimeSpan = value;
                OnPropertySet(this, "NarrowByRecentTimeSpan", old, value);
            }
        }

        [Persist, External]
        public virtual MemoRecentTimeSpan RecentTimeSpan {
            get { return _recentTimeSpan; }
            set {
                if (_recentTimeSpan.Equals(value)) {
                    return;
                }
                var old = _recentTimeSpan;
                _recentTimeSpan = value;
                OnPropertySet(this, "RecentTimeSpan", old, value);
            }
        }


        [Persist, External]
        public virtual bool NarrowByImportanceKind {
            get { return _narrowByImportanceKind; }
            set {
                if (value == _narrowByImportanceKind) {
                    return;
                }
                var old = _narrowByImportanceKind;
                _narrowByImportanceKind = value;
                OnPropertySet(this, "NarrowByImportanceKind", old, value);
            }
        }

        [Persist(Add = "AddImportanceKind"), External(Add = "AddImportanceKind")]
        public virtual IEnumerable<MemoImportanceKind> ImportanceKinds {
            get { return _importanceKinds; }
        }


        //[Persist, External]
        //public virtual MemoConditionCompoundKind ImportanceCompoundKind {
        //    get { return _importanceCompoundKind; }
        //    set {
        //        if (_importanceCompoundKind == value) {
        //            return;
        //        }
        //        var old = _importanceCompoundKind;
        //        _importanceCompoundKind = value;
        //        OnPropertySet(this, "ImportanceCompoundKind", old, value);
        //    }
        //}



        public virtual bool IsEmptyQuery {
            get {
                return
                    !NarrowByKeywords &&
                    !NarrowByTitle &&
                    !_narrowByTagIds &&
                    !_narrowByMarkKinds &&
                    !_narrowByTimeSpan &&
                    !_narrowByRecentTimeSpan &&
                    !_narrowByImportanceKind;
            }
        }

        public virtual bool NarrowByKeywords {
            get { return !string.IsNullOrEmpty(_condition); }
        }

        public virtual bool NarrowByTitle {
            get { return !string.IsNullOrEmpty(_title); }
        }

        // ========================================
        // method
        // ========================================
        [Dirty]
        public virtual void Clear() {
            _condition = "";
            _title = "";

            _narrowByTagIds = false;
            _noTag = false;
            ClearTagIds();

            _narrowByMarkKinds = false;
            _noMarkKind = false;
            ClearMarkKinds();

            _narrowByTimeSpan = false;
            _narrowByRecentTimeSpan = false;

            _narrowByImportanceKind = false;
            ClearImportanceKinds();
        }

        [Dirty]
        public virtual void AddTagId(string tagId) {
            _tagIds.Add(tagId);
        }

        [Dirty]
        public virtual void RemoveTagId(string tagId) {
            _tagIds.Remove(tagId);
        }

        [Dirty]
        public virtual void ClearTagIds() {
            _tagIds.Clear();
        }

        [Dirty]
        public virtual void AddMarkKind(MemoMarkKind kind) {
            _markKinds.Add(kind);
        }

        [Dirty]
        public virtual void RemoveMarkKind(MemoMarkKind kind) {
            _markKinds.Remove(kind);
        }

        [Dirty]
        public virtual void ClearMarkKinds() {
            _markKinds.Clear();
        }


        [Dirty]
        public virtual void AddImportanceKind(MemoImportanceKind kind) {
            _importanceKinds.Add(kind);
        }

        [Dirty]
        public virtual void RemoveImportanceKind(MemoImportanceKind kind) {
            _importanceKinds.Remove(kind);
        }

        [Dirty]
        public virtual void ClearImportanceKinds() {
            _importanceKinds.Clear();
        }

    }
}
