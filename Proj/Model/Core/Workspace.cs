/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Container.Core;
using Mkamo.Model.Core;
using System.ComponentModel;
using Mkamo.Model.Memo;
using Mkamo.Common.Event;

namespace Mkamo.Model.Core {
    public class Workspace {
        // ========================================
        // field
        // ========================================
        private IEntityContainer _container;

        private List<MemoTag> _tags;
        private List<MemoFolder> _rootFolders;
        private List<MemoSmartFolder> _smartFolders;

        // ========================================
        // constructor
        // ========================================
        public Workspace(IEntityContainer container) {
            _container = container;
            _container.EntityPersisted += HandleEntityPersisted;
            _container.EntityRemoving += HandleEntityRemoving;
        }

        private List<MemoTag> InitTags() {
            var ret = new List<MemoTag>();
            ret.AddRange(_container.FindAll<MemoTag>());
            foreach (var tag in ret) {
                tag.DetailedPropertyChanged += HandleTagPropChanged;
            }
            return ret;
        }

        private List<MemoFolder> InitFolders() {
            var ret = new List<MemoFolder>();
            var folders = _container.FindAll<MemoFolder>();
            foreach (var folder in folders) {
                if (folder.ParentFolder == null) {
                    ret.Add(folder);
                }
                folder.DetailedPropertyChanged += HandleFolderPropChanged;
            }
            return ret;
        }

        private List<MemoSmartFolder> InitSmartFolders() {
            var ret = new List<MemoSmartFolder>();
            ret.AddRange(_container.FindAll<MemoSmartFolder>());
            foreach (var folder in ret) {
                folder.DetailedPropertyChanged += HandleSmartFolderPropChanged;
            }
            return ret;
        }

        // ========================================
        // event
        // ========================================
        /// <summary>
        /// Tagsに何らかの変更が行われたときに発火するイベント．
        /// </summary>
        public event EventHandler<MemoTagChangedEventArgs> MemoTagChanged;
        public event EventHandler<MemoTagEventArgs> MemoTagAdded;
        public event EventHandler<MemoTagEventArgs> MemoTagRemoving;

        /// <summary>
        /// Foldersに何らかの変更が行われたときに発火するイベント．
        /// </summary>
        public event EventHandler<MemoFolderChangedEventArgs> MemoFolderChanged;
        public event EventHandler<MemoFolderEventArgs> MemoFolderAdded;
        public event EventHandler<MemoFolderEventArgs> MemoFolderRemoving;

        /// <summary>
        /// SmartFoldersに何らかの変更が行われたときに発火するイベント．
        /// </summary>
        public event EventHandler<MemoSmartFolderChangedEventArgs> MemoSmartFolderChanged;
        public event EventHandler<MemoSmartFolderEventArgs> MemoSmartFolderAdded;
        public event EventHandler<MemoSmartFolderEventArgs> MemoSmartFolderRemoving;


        public event EventHandler<MemoSmartFilterEventArgs> MemoSmartFilterAdded;
        public event EventHandler<MemoSmartFilterEventArgs> MemoSmartFilterRemoving;
        
        // ========================================
        // property
        // ========================================
        public IEntityContainer Container {
            get { return _container; }
        }

        public IEnumerable<MemoTag> Tags {
            get { return _Tags; }
        }

        public IEnumerable<MemoSmartFolder> SmartFolders {
            get { return _SmartFolders; }
        }

        public IEnumerable<MemoFolder> Folders {
            get { return _container.FindAll<MemoFolder>(); }
        }

        public IEnumerable<MemoFolder> RootFolders {
            get { return _RootFolders; }
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected List<MemoTag> _Tags {
            get {
                if (_tags == null) {
                    _tags = InitTags();
                }
                return _tags;
            }
        }

        protected List<MemoSmartFolder> _SmartFolders {
            get {
                if (_smartFolders == null) {
                    _smartFolders = InitSmartFolders();
                }
                return _smartFolders;
            }
        }

        protected List<MemoFolder> _RootFolders {
            get {
                if (_rootFolders == null) {
                    _rootFolders = InitFolders();
                }
                return _rootFolders;
            }
        }

        // ========================================
        // method
        // ========================================
        public MemoTag CreateTag() {
            return MemoFactory.CreateTag();
        }

        public void RemoveTag(MemoTag tag) {
            var subs = tag.SubTags.ToArray();
            foreach (var sub in subs) {
                RemoveTag(sub);
            }
            RemoveTagOnly(tag);
        }

        public MemoFolder CreateFolder() {
            return MemoFactory.CreateFolder();
        }

        public void RemoveFolder(MemoFolder folder) {
            var subs = folder.SubFolders.ToArray();
            foreach (var sub in subs) {
                RemoveFolder(sub);
            }
            RemoveFolderOnly(folder);
        }

        public Memo.Memo CreateMemo() {
            return MemoFactory.CreateMemo();
        }

        public void RemoveMemo(Memo.Memo memo) {
            memo.ClearTags();
            memo.ClearContainedFolders();
            _container.Remove(memo);
        }

        public MemoSmartFolder CreateSmartFolder() {
            var ret = MemoFactory.CreateSmartFolder();
            ret.Query = MemoFactory.CreateQuery();
            return ret;
        }

        public MemoSmartFolder CreateTransientSmartFolder() {
            var ret = MemoFactory.CreateTransientSmartFolder();
            ret.Query = MemoFactory.CreateTransientQuery();
            return ret;
        }

        public void RemoveSmartFolder(MemoSmartFolder smartFolder) {
            _container.Remove(smartFolder);
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected virtual void OnMemoTagChanged(MemoTag changed, DetailedPropertyChangedEventArgs cause) {
            var handler = MemoTagChanged;
            if (handler != null) {
                handler(this, new MemoTagChangedEventArgs(changed, cause));
            }
        }

        protected virtual void OnMemoTagAdded(MemoTag added) {
            var handler = MemoTagAdded;
            if (handler != null) {
                handler(this, new MemoTagEventArgs(added));
            }
        }

        protected virtual void OnMemoTagRemoving(MemoTag removing) {
            var handler = MemoTagRemoving;
            if (handler != null) {
                handler(this, new MemoTagEventArgs(removing));
            }
        }

        protected virtual void OnMemoFolderChanged(MemoFolder changed, DetailedPropertyChangedEventArgs cause) {
            var handler = MemoFolderChanged;
            if (handler != null) {
                handler(this, new MemoFolderChangedEventArgs(changed, cause));
            }
        }

        protected virtual void OnMemoFolderAdded(MemoFolder added) {
            var handler = MemoFolderAdded;
            if (handler != null) {
                handler(this, new MemoFolderEventArgs(added));
            }
        }

        protected virtual void OnMemoFolderRemoving(MemoFolder removing) {
            var handler = MemoFolderRemoving;
            if (handler != null) {
                handler(this, new MemoFolderEventArgs(removing));
            }
        }

        protected virtual void OnMemoSmartFolderChanged(MemoSmartFolder changed, DetailedPropertyChangedEventArgs cause) {
            var handler = MemoSmartFolderChanged;
            if (handler != null) {
                handler(this, new MemoSmartFolderChangedEventArgs(changed, cause));
            }
        }

        protected virtual void OnMemoSmartFolderAdded(MemoSmartFolder added) {
            var handler = MemoSmartFolderAdded;
            if (handler != null) {
                handler(this, new MemoSmartFolderEventArgs(added));
            }
        }

        protected virtual void OnMemoSmartFolderRemoving(MemoSmartFolder removing) {
            var handler = MemoSmartFolderRemoving;
            if (handler != null) {
                handler(this, new MemoSmartFolderEventArgs(removing));
            }
        }

        protected virtual void OnMemoSmartFilterAdded(MemoSmartFilter added) {
            var handler = MemoSmartFilterAdded;
            if (handler != null) {
                handler(this, new MemoSmartFilterEventArgs(added));
            }
        }

        protected virtual void OnMemoSmartFilterRemoving(MemoSmartFilter removing) {
            var handler = MemoSmartFilterRemoving;
            if (handler != null) {
                handler(this, new MemoSmartFilterEventArgs(removing));
            }
        }

        // ------------------------------
        // private
        // ------------------------------
        private void RemoveTagOnly(MemoTag tag) {
            tag.ClearMemos();
            tag.SuperTag = null;
            tag.ClearSubTags();
            _container.Remove(tag);
        }

        private void RemoveFolderOnly(MemoFolder folder) {
            folder.ClearContainingMemos();
            folder.ClearSubFolders();
            folder.ParentFolder = null;
            _container.Remove(folder);
        }


        private void HandleTagPropChanged(object sender, DetailedPropertyChangedEventArgs e) {
            OnMemoTagChanged(sender as MemoTag, e);
        }

        private void HandleFolderPropChanged(object sender, DetailedPropertyChangedEventArgs e) {
            OnMemoFolderChanged(sender as MemoFolder, e);
        }

        private void HandleSmartFolderPropChanged(object sender, DetailedPropertyChangedEventArgs e) {
            OnMemoSmartFolderChanged(sender as MemoSmartFolder, e);
        }

        private void HandleEntityPersisted(object sender, EntityEventArgs e) {
            if (e.EntityType == typeof(MemoTag)) {
                var tag = e.Entity as MemoTag;
                _Tags.Add(tag);
                tag.DetailedPropertyChanged += HandleTagPropChanged;
                OnMemoTagAdded(tag);
            } else if (e.EntityType == typeof(MemoFolder)) {
                var folder = e.Entity as MemoFolder;
                if (folder.ParentFolder == null) {
                    _RootFolders.Add(folder);
                }
                folder.DetailedPropertyChanged += HandleFolderPropChanged;
                OnMemoFolderAdded(folder);
            } else if (e.EntityType == typeof(MemoSmartFolder)) {
                var smartFolder = e.Entity as MemoSmartFolder;
                _SmartFolders.Add(smartFolder);
                smartFolder.DetailedPropertyChanged += HandleSmartFolderPropChanged;
                OnMemoSmartFolderAdded(smartFolder);
            } else if (e.EntityType == typeof(MemoSmartFilter)) {
                var smartFilter = e.Entity as MemoSmartFilter;
                OnMemoSmartFilterAdded(smartFilter);
            }
        }

        private void HandleEntityRemoving(object sender, EntityEventArgs e) {
            if (e.EntityType == typeof(MemoTag)) {
                var tag = e.Entity as MemoTag;
                tag.DetailedPropertyChanged -= HandleTagPropChanged;
                _Tags.Remove(tag);
                OnMemoTagRemoving(tag);
            } else if (e.EntityType == typeof(MemoFolder)) {
                var folder = e.Entity as MemoFolder;
                folder.DetailedPropertyChanged -= HandleFolderPropChanged;
                if (_RootFolders.Contains(folder)) {
                    _RootFolders.Remove(folder);
                }
                OnMemoFolderRemoving(folder);
            } else if (e.EntityType == typeof(MemoSmartFolder)) {
                var smartFolder = e.Entity as MemoSmartFolder;
                smartFolder.DetailedPropertyChanged -= HandleSmartFolderPropChanged;
                _SmartFolders.Remove(smartFolder);
                OnMemoSmartFolderRemoving(smartFolder);
            } else if (e.EntityType == typeof(MemoSmartFilter)) {
                var smartFilter = e.Entity as MemoSmartFilter;
                OnMemoSmartFilterRemoving(smartFilter);
            }
        }
    }
}
