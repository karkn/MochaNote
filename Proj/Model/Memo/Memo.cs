/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Container.Core;
using Mkamo.Common.Externalize;
using Mkamo.Model.Core;
using System.Collections.ObjectModel;
using Mkamo.Common.Association;
using System.Runtime.Serialization;

namespace Mkamo.Model.Memo {
    [Externalizable(Type = typeof(Memo), FactoryMethodType = typeof(MemoFactory), FactoryMethod = "CreateMemo")]
    [Entity(OnLoading = "OnLoading", OnLoaded = "OnLoaded")]
    [DataContract]
    [Serializable]
    public class Memo: MemoMarkableElement {
        // ========================================
        // field
        // ========================================
        private MemoElementCollection _contents;

        [NonSerialized]
        private AssociationCollection<MemoTag> _tags;
        [NonSerialized]
        private AssociationCollection<MemoFolder> _containedFolders;

        [DataMember(Name = "Title")]
        private string _title;
        [DataMember(Name = "Source")]
        private string _source;
        [DataMember(Name = "AccessedDate")]
        private DateTime _accessedDate;
        [DataMember(Name = "Importance")]
        private MemoImportanceKind _importance;

        // ========================================
        // constructor
        // ========================================
        protected internal Memo() {
            _tags = new AssociationCollection<MemoTag>(
                tag => {
                    if (!tag.Memos.Contains(this)) {
                        tag.AddMemo(this);
                    }
                },
                tag => tag.RemoveMemo(this),
                this,
                "Tags"
            );
            _tags.DetailedPropertyChanged += (sender, e) => OnPropertyChanged(this, e);

            _containedFolders = new AssociationCollection<MemoFolder>(
                folder => {
                    if (!folder.ContainingMemos.Contains(this)) {
                        folder.AddContainingMemo(this);
                    }
                },
                folder => folder.RemoveContainingMemo(this),
                this,
                "ContainedFolders"
            );
            _containedFolders.DetailedPropertyChanged += (sender, e) => OnPropertyChanged(this, e, false);

            _title = "";
            _source = "";
            _accessedDate = ModifiedDate;
        }

        // ========================================
        // property
        // ========================================
        public override bool IsMarkable {
            get { return true; }
        }

        [Persist(Cascade = true, Composite = typeof(MemoElementCollection)), External]
        public virtual MemoElementCollection Contents
        {
            get { return _contents; }
            set { _contents = value; }
        }

        [Persist(Add = "AddTag"), External(Add = "AddTag")]
        public virtual IEnumerable<MemoTag> Tags {
            get { return _tags; }
        }

        [Persist(Add = "AddContainedFolder"), External(Add = "AddContainedFolder")]
        public virtual IEnumerable<MemoFolder> ContainedFolders {
            get { return _containedFolders; }
        }
        
        [Persist, External]
        public virtual string Title {
            get { return _title; }
            set {
                if (value == null || value == _title) {
                    return;
                }
                var old = _title;
                _title = value;
                OnPropertySet(this, "Title", old, value);
            }
        }

        [Persist, External]
        public virtual string Source {
            get { return _source; }
            set {
                if (value == null || value == _source) {
                    return;
                }
                var old = _source;
                _source = value;
                OnPropertySet(this, "Source", old, value);
            }
        }

        [Persist, External]
        public virtual MemoImportanceKind Importance {
            get { return _importance; }
            set {
                if (_importance == value) {
                    return;
                }
                var old = _importance;
                _importance = value;
                OnPropertySet(this, "Importance", old, value);
            }
        }

        [Persist, External]
        public virtual DateTime AccessedDate {
            get { return _accessedDate; }
            set { _accessedDate = value; }
        }



        public virtual string TagsText {
            get {
                var tagNames = Tags.Select(tag => tag.Name).ToArray();
                Array.Sort(tagNames);
                return string.Join(", ", tagNames);
            }
        }

        public virtual string FullTagsText {
            get {
                var tagNames = Tags.Select(tag => tag.FullName).ToArray();
                Array.Sort(tagNames);
                return string.Join(", ", tagNames);
            }
        }

        // ========================================
        // method
        // ========================================
        [Dirty]
        public virtual void AddTag(MemoTag tag) {
            _tags.Add(tag);
        }

        [Dirty]
        public virtual void RemoveTag(MemoTag tag) {
            _tags.Remove(tag);
        }

        [Dirty]
        public virtual void ClearTags() {
            _tags.Clear();
        }

        [Dirty]
        public virtual void AddContainedFolder(MemoFolder folder) {
            _containedFolders.Add(folder);
        }

        [Dirty]
        public virtual void RemoveContainedFolder(MemoFolder folder) {
            _containedFolders.Remove(folder);
        }

        [Dirty]
        public virtual void ClearContainedFolders() {
            _containedFolders.Clear();
        }

        public virtual void OnLoading() {
            _tags.DisableAssociator = true;
            _containedFolders.DisableAssociator = true;
        }

        public virtual void OnLoaded() {
            _tags.DisableAssociator = false;
            _containedFolders.DisableAssociator = false;
        }
    }
}
