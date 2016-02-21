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
using Mkamo.Common.Association;
using System.Collections.ObjectModel;
using Mkamo.Model.Core;
using System.Runtime.Serialization;

namespace Mkamo.Model.Memo {
    [Externalizable(
        Type = typeof(MemoFolder),
        FactoryMethodType = typeof(MemoFactory), FactoryMethod = "CreateFolder"
    )]
    [Entity(OnLoading = "OnLoading", OnLoaded = "OnLoaded")]
    [DataContract, Serializable]
    public class MemoFolder: MemoElement {
        // ========================================
        // field
        // ========================================
        [DataMember(Name = "Name")]
        private string _name;

        [DataMember(Name = "ParentFolder")]
        private MemoFolder _parentFolder;

        [DataMember(Name = "SubFolders")]
        private AssociationCollection<MemoFolder> _subFolders;

        [DataMember(Name = "ContainingMemos")]
        private AssociationCollection<Memo> _containingMemos;

        // ========================================
        // constructor
        // ========================================
        protected internal MemoFolder() {
            _subFolders = new AssociationCollection<MemoFolder>(
                folder => {
                    if (folder.ParentFolder != this) {
                        folder.ParentFolder = this;
                    }
                },
                folder => folder.ParentFolder = null,
                this,
                "SubFolders"
            );
            _subFolders.DetailedPropertyChanged += (sender, e) => OnPropertyChanged(this, e);

            _containingMemos = new AssociationCollection<Memo>(
                memo => {
                    if (!memo.ContainedFolders.Contains(this)) {
                        memo.AddContainedFolder(this);
                    }
                },
                memo => memo.RemoveContainedFolder(this),
                this,
                "ContainingMemos"
            );
            _containingMemos.DetailedPropertyChanged += (sender, e) => OnPropertyChanged(this, e);
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
        public virtual MemoFolder ParentFolder {
            get { return _parentFolder; }
            set {
                if (value == this) {
                    return;
                }

                var old = _parentFolder;
                var result = AssociationUtil.EnsureAssociation(
                    _parentFolder,
                    value,
                    folder => _parentFolder = folder,
                    folder => {
                        if (!folder.SubFolders.Contains(this)) {
                            folder.AddSubFolder(this);
                        }
                    },
                    folder => folder.RemoveSubFolder(this)
                );
                if (result != AssociationUtil.EnsureResult.None) {
                    OnPropertySet(this, "Parent", old, value);
                }
            }
        }

        [Persist(Add = "AddSubFolder"), External(Add = "AddSubFolder")]
        public virtual IEnumerable<MemoFolder> SubFolders {
            get { return _subFolders; }
        }

        [Persist(Add = "AddContainingMemo"), External(Add = "AddContainingMemo")]
        public virtual IEnumerable<Memo> ContainingMemos {
            get { return _containingMemos; }
        }

        public virtual string FullName {
            get {
                var ret = new StringBuilder(_name);
                var parent = _parentFolder;
                while (parent != null) {
                    ret.Insert(0, parent.Name + "/");
                    parent = parent.ParentFolder;
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

        [Dirty]
        public virtual void AddSubFolder(MemoFolder folder) {
            if (folder == this) {
                return;
            }
            _subFolders.Add(folder);
        }

        [Dirty]
        public virtual void RemoveSubFolder(MemoFolder folder) {
            _subFolders.Remove(folder);
        }

        [Dirty]
        public virtual void ClearSubFolders() {
            _subFolders.Clear();
        }

        [Dirty]
        public virtual void AddContainingMemo(Memo memo) {
            _containingMemos.Add(memo);
        }

        [Dirty]
        public virtual void RemoveContainingMemo(Memo memo) {
            _containingMemos.Remove(memo);
        }

        [Dirty]
        public virtual void ClearContainingMemos() {
            _containingMemos.Clear();
        }

        public virtual void OnLoading() {
            _containingMemos.DisableAssociator = true;
        }

        public virtual void OnLoaded() {
            _containingMemos.DisableAssociator = false;
        }
    }
}
