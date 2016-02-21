/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Memopad.Internal.Core;
using System.Windows.Forms;
using Mkamo.Editor.Core;
using Mkamo.Memopad.Internal.Controls;
using Mkamo.Common.Forms.KeyMap;
using Mkamo.Editor.Focuses;
using Mkamo.Memopad.Internal.KeyActions;
using Mkamo.Common.Forms.Core;

namespace Mkamo.Memopad.Internal.KeySchemes {
    internal abstract class AbstractKeyScheme: IKeyScheme {
        // ========================================
        // field
        // ========================================
        private KeyBinder<TextBox> _textBoxKeyBinder;
        private KeyBinder<TreeView> _treeViewKeyBinder;
        private KeyBinder<ComboBox> _comboBoxKeyBinder;
        private KeyBinder<MemoListView> _memoListViewKeyBinder;
        private KeyBinder<TextBox> _pageContentTitleTextBoxKeyBinder;

        private KeyBinder<EditorCanvas> _noSelectionKeyBinder;
        private IKeyMap<EditorCanvas> _noSelectionKeyMap;
        private KeyBinder<IFocus> _memoContentFocusKeyBinder;
        private IKeyMap<IFocus> _memoContentFocusKeyMap;
        private KeyBinder<IFocus> _memoContentSingleLineFocusKeyBinder;
        private IKeyMap<IFocus> _memoContentSingleLineFocusKeyMap;
        private KeyBinder<IEditor> _memoEditorKeyBinder;
        private IKeyMap<IEditor> _memoEditorKeyMap;
        private KeyBinder<IEditor> _memoContentEditorKeyBinder;
        private IKeyMap<IEditor> _memoContentEditorKeyMap;
        private KeyBinder<IEditor> _memoTableCellEditorKeyBinder;
        private IKeyMap<IEditor> _memoTableCellEditorKeyMap;
        private KeyBinder<IFocus> _memoTableCellFocusKeyBinder;
        private IKeyMap<IFocus> _memoTableCellFocusKeyMap;
        private KeyBinder<IEditor> _umlFeatureEditorKeyBinder;
        private IKeyMap<IEditor> _umlFeatureEditorKeyMap;

        
        // ========================================
        // constructor
        // ========================================
        public AbstractKeyScheme() {
        }

        // ========================================
        // property
        // ========================================
        public KeyBinder<TextBox> TextBoxKeyBinder {
            get {
                if (_textBoxKeyBinder == null) {
                    _textBoxKeyBinder = new KeyBinder<TextBox>(
                        new[] { typeof(CommonKeyActions<TextBox>), typeof(TextBoxKeyActions), }
                    );
                    ResetTextBoxKeyBind(_textBoxKeyBinder);
                }
                return _textBoxKeyBinder;
            }
        }

        public KeyBinder<TreeView> TreeViewKeyBinder {
            get {
                if (_treeViewKeyBinder == null) {
                    _treeViewKeyBinder = new KeyBinder<TreeView>(
                        new[] { typeof(CommonKeyActions<TreeView>), typeof(TreeViewKeyActions), }
                    );
                    ResetTreeViewKeyBind(_treeViewKeyBinder);
                }
                return _treeViewKeyBinder;
            }
        }

        public KeyBinder<ComboBox> ComboBoxKeyBinder {
            get {
                if (_comboBoxKeyBinder == null) {
                    _comboBoxKeyBinder = new KeyBinder<ComboBox>(
                        new[] { typeof(CommonKeyActions<ComboBox>), typeof(ComboBoxKeyActions), }
                    );
                    ResetComboBoxKeyBind(_comboBoxKeyBinder);
                }
                return _comboBoxKeyBinder;
            }
        }

        public KeyBinder<MemoListView> MemoListViewKeyBinder {
            get {
                if (_memoListViewKeyBinder == null) {
                    _memoListViewKeyBinder = new KeyBinder<MemoListView>(
                        new[] { typeof(CommonKeyActions<MemoListView>), typeof(MemoListViewKeyActions), }
                    );
                    ResetMemoListViewKeyBind(_memoListViewKeyBinder);
                }
                return _memoListViewKeyBinder;
            }
        }

        public KeyBinder<TextBox> PageContentTitleTextBoxKeyBinder {
            get {
                if (_pageContentTitleTextBoxKeyBinder == null) {
                    _pageContentTitleTextBoxKeyBinder = new KeyBinder<TextBox>(
                        new[] { typeof(CommonKeyActions<TextBox>), typeof(TextBoxKeyActions), typeof(PageContentTitleTextBoxKeyActions), }
                    );
                    ResetPageContentTitleTextBoxKeyBind(_pageContentTitleTextBoxKeyBinder);
                }
                return _pageContentTitleTextBoxKeyBinder;
            }
        }

        //public IKeyMap<TextBox> TextBoxKeyMap {
        //    get { return _textBoxKeyMap ?? (_textBoxKeyMap = CreateTextBoxKeyMap()); }
        //}

        public KeyBinder<EditorCanvas> NoSelectionKeyBinder {
            get {
                if (_noSelectionKeyBinder == null) {
                    _noSelectionKeyBinder = new KeyBinder<EditorCanvas>(
                        new[] { typeof(CommonKeyActions<EditorCanvas>), typeof(NoSelectionKeyActions), }
                    );

                    // todo: load from file if possible
                    ResetNoSelectionKeyBind(_noSelectionKeyBinder);
                }
                return _noSelectionKeyBinder;
            }
        }

        public IKeyMap<EditorCanvas> NoSelectionKeyMap {
            get { return _noSelectionKeyMap ?? (_noSelectionKeyMap = CreateNoSelectionKeyMap()); }
        }

        public KeyBinder<IFocus> MemoContentFocusKeyBinder {
            get {
                if (_memoContentFocusKeyBinder == null) {
                    _memoContentFocusKeyBinder = new KeyBinder<IFocus>(
                        new[] { typeof(CommonKeyActions<IFocus>), typeof(StyledTextFocusKeyActions), typeof(MemoContentFocusKeyActions), }
                    );
                    _memoContentFocusKeyBinder.RegisterBindAction("KillLine", BindKillLine);
                    // todo: load from file if possible
                    ResetMemoContentFocusKeyBind(_memoContentFocusKeyBinder);
                }
                return _memoContentFocusKeyBinder;
            }
        }

        public IKeyMap<IFocus> MemoContentFocusKeyMap {
            get { return _memoContentFocusKeyMap ?? (_memoContentFocusKeyMap = CreateMemoContentFocusKeyMap()); }
        }

        public KeyBinder<IFocus> MemoContentSingleLineFocusKeyBinder {
            get {
                if (_memoContentSingleLineFocusKeyBinder == null) {
                    _memoContentSingleLineFocusKeyBinder = new KeyBinder<IFocus>(
                        new[] { typeof(CommonKeyActions<IFocus>), typeof(StyledTextFocusKeyActions), typeof(MemoContentFocusKeyActions), }
                    );

                    // todo: load from file if possible
                    ResetMemoContentSingleLineFocusKeyBind(_memoContentSingleLineFocusKeyBinder);
                }
                return _memoContentSingleLineFocusKeyBinder;
            }
        }

        public IKeyMap<IFocus> MemoContentSingleLineFocusKeyMap {
            get { return _memoContentSingleLineFocusKeyMap ?? (_memoContentSingleLineFocusKeyMap = CreateMemoContentSingleLineFocusKeyMap()); }
        }

        public KeyBinder<IEditor> MemoEditorKeyBinder {
            get {
                if (_memoEditorKeyBinder == null) {
                    _memoEditorKeyBinder = new KeyBinder<IEditor>(
                        new[] { typeof(CommonKeyActions<IEditor>), typeof(MemoEditorKeyActions), }
                    );

                    // todo: load from file if possible
                    ResetMemoEditorKeyBind(_memoEditorKeyBinder);
                }
                return _memoEditorKeyBinder;
            }
        }

        public IKeyMap<IEditor> MemoEditorKeyMap {
            get { return _memoEditorKeyMap ?? (_memoEditorKeyMap = CreateMemoEditorKeyMap()); }
        }

        public KeyBinder<IEditor> MemoContentEditorKeyBinder {
            get {
                if (_memoContentEditorKeyBinder == null) {
                    _memoContentEditorKeyBinder = new KeyBinder<IEditor>(
                        new[] { typeof(CommonKeyActions<IEditor>), typeof(MemoContentEditorKeyActions), }
                    );

                    // todo: load from file if possible
                    ResetMemoContentEditorKeyBind(_memoContentEditorKeyBinder);
                }
                return _memoContentEditorKeyBinder;
            }
        }

        public IKeyMap<IEditor> MemoContentEditorKeyMap {
            get { return _memoContentEditorKeyMap ?? (_memoContentEditorKeyMap = CreateMemoContentEditorKeyMap()); }
        }

        public KeyBinder<IEditor> MemoTableCellEditorKeyBinder {
            get {
                if (_memoTableCellEditorKeyBinder == null) {
                    _memoTableCellEditorKeyBinder = new KeyBinder<IEditor>(
                        new[] { typeof(CommonKeyActions<IEditor>), typeof(MemoTableCellEditorKeyActions), }
                    );

                    // todo: load from file if possible
                    ResetMemoTableCellEditorKeyBind(_memoTableCellEditorKeyBinder);
                }
                return _memoTableCellEditorKeyBinder;
            }
        }

        public IKeyMap<IEditor> MemoTableCellEditorKeyMap {
            get { return _memoTableCellEditorKeyMap ?? (_memoTableCellEditorKeyMap = CreateMemoTableCellEditorKeyMap()); }
        }

        public KeyBinder<IFocus> MemoTableCellFocusKeyBinder {
            get {
                if (_memoTableCellFocusKeyBinder == null) {
                    _memoTableCellFocusKeyBinder = new KeyBinder<IFocus>(
                        new[] {
                            typeof(CommonKeyActions<IFocus>),
                            typeof(StyledTextFocusKeyActions),
                            typeof(MemoContentFocusKeyActions),
                            typeof(MemoTableCellFocusKeyActions),
                        }
                    );

                    // todo: load from file if possible
                    ResetMemoTableCellFocusKeyBind(_memoTableCellFocusKeyBinder);
                }
                return _memoTableCellFocusKeyBinder;
            }
        }

        public IKeyMap<IFocus> MemoTableCellFocusKeyMap {
            get { return _memoTableCellFocusKeyMap ?? (_memoTableCellFocusKeyMap = CreateMemoTableCellFocusKeyMap()); }
        }

        public KeyBinder<IEditor> UmlFeatureEditorKeyBinder {
            get {
                if (_umlFeatureEditorKeyBinder == null) {
                    _umlFeatureEditorKeyBinder = new KeyBinder<IEditor>(
                        new[] { typeof(CommonKeyActions<IEditor>), typeof(UmlFeatureEditorKeyActions), }
                    );

                    // todo: load from file if possible
                    ResetUmlFeatureEditorKeyBind(_umlFeatureEditorKeyBinder);
                }
                return _umlFeatureEditorKeyBinder;
            }
        }

        public IKeyMap<IEditor> UmlFeatureEditorKeyMap {
            get { return _umlFeatureEditorKeyMap ?? (_umlFeatureEditorKeyMap = CreateUmlFeatureEditorKeyMap()); }
        }


        // ========================================
        // method
        // ========================================
        protected abstract void ResetTextBoxKeyBind(KeyBinder<TextBox> binder);
        protected abstract void ResetTreeViewKeyBind(KeyBinder<TreeView> binder);
        protected abstract void ResetComboBoxKeyBind(KeyBinder<ComboBox> binder);
        protected abstract void ResetMemoListViewKeyBind(KeyBinder<MemoListView> binder);
        protected abstract void ResetPageContentTitleTextBoxKeyBind(KeyBinder<TextBox> binder);

        protected abstract void ResetNoSelectionKeyBind(KeyBinder<EditorCanvas> binder);
        private IKeyMap<EditorCanvas> CreateNoSelectionKeyMap() {
            var ret = new KeyMap<EditorCanvas>();
            NoSelectionKeyBinder.Bind(ret);
            return ret;
        }

        protected abstract void ResetMemoEditorKeyBind(KeyBinder<IEditor> binder);
        private IKeyMap<IEditor> CreateMemoEditorKeyMap() {
            var ret = new KeyMap<IEditor>();
            MemoEditorKeyBinder.Bind(ret);
            return ret;
        }

        protected abstract void ResetMemoContentFocusKeyBind(KeyBinder<IFocus> binder);
        private IKeyMap<IFocus> CreateMemoContentFocusKeyMap() {
            var ret = new KeyMap<IFocus>();
            MemoContentFocusKeyBinder.Bind(ret);
            return ret;
        }

        protected abstract void ResetMemoContentSingleLineFocusKeyBind(KeyBinder<IFocus> binder);
        private IKeyMap<IFocus> CreateMemoContentSingleLineFocusKeyMap() {
            var ret = new KeyMap<IFocus>();
            MemoContentSingleLineFocusKeyBinder.Bind(ret);
            return ret;
        }

        protected abstract void ResetMemoContentEditorKeyBind(KeyBinder<IEditor> binder);
        private IKeyMap<IEditor> CreateMemoContentEditorKeyMap() {
            var ret = new KeyMap<IEditor>();
            MemoContentEditorKeyBinder.Bind(ret);
            return ret;
        }

        protected abstract void ResetMemoTableCellEditorKeyBind(KeyBinder<IEditor> binder);
        private IKeyMap<IEditor> CreateMemoTableCellEditorKeyMap() {
            var ret = new KeyMap<IEditor>();
            MemoTableCellEditorKeyBinder.Bind(ret);
            return ret;
        }

        protected abstract void ResetMemoTableCellFocusKeyBind(KeyBinder<IFocus> binder);
        private IKeyMap<IFocus> CreateMemoTableCellFocusKeyMap() {
            var ret = new KeyMap<IFocus>();
            MemoTableCellFocusKeyBinder.Bind(ret);
            return ret;
        }

        protected abstract void ResetUmlFeatureEditorKeyBind(KeyBinder<IEditor> binder);
        private IKeyMap<IEditor> CreateUmlFeatureEditorKeyMap() {
            var ret = new KeyMap<IEditor>();
            UmlFeatureEditorKeyBinder.Bind(ret);
            return ret;
        }

        // ------------------------------
        // private
        // ------------------------------
        private void BindKillLine(Keys shortcutKey, IKeyMap<IFocus> keyMap) {
            var killLinePrefix = keyMap.SetPrefix(
                shortcutKey,
                (key, focus) => ((StyledTextFocus) focus).KillLineFirst(),
                (key, focus) => {
                    if (keyMap.IsDefined(key)) {
                        var action = keyMap.GetAction(key);
                        if (action != null) {
                            action(focus);
                        }
                    }
                },
                (key, focus) => key != shortcutKey,
                false
            );
            killLinePrefix.SetAction(shortcutKey, (focus) => ((StyledTextFocus) focus).KillLine());
        }

    }
}
