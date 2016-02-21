/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mkamo.Editor.Core;
using Mkamo.Editor.Internal.Editors;

namespace Mkamo.Editor.Internal.Core {
    /// <summary>
    /// Editorの選択状態を管理する．
    ///   - 同じ親を持つEditorだけが選択状態にする．
    /// </summary>
    internal class SelectionManager: ISelectionManager {
        // ========================================
        // field
        // ========================================
        private RootEditor _root;

        private List<IEditor> _selectedEditors;
        private IEditor _currentParent;
        private bool _multiSelect;

        private bool _inDeselectAll;

        // ========================================
        // constructor
        // ========================================
        public SelectionManager(RootEditor root) {
            _root = root;
            _selectedEditors = new List<IEditor>();
            _currentParent = null;
            _multiSelect = true;

            _inDeselectAll = false;
        }

        // ========================================
        // property
        // ========================================
        /// <summary>
        /// 現在選択されているeditorのリスト．
        /// 格納されているすべてのeditorが同じ親を持つことを保証．
        /// 格納されているeditorの順序は親に参照されている順序と同じ順序であることを保証．
        /// </summary>
        public IEnumerable<IEditor> SelectedEditors {
            get { return _selectedEditors; }
        }

        public bool MultiSelect {
            get { return _multiSelect; }
            set {
                if (value == _multiSelect) {
                    return;
                }
                _multiSelect = value;
                if (!_multiSelect) {
                    DeselectAllSelectedEditors();
                }
            }
        }

        public event EventHandler<EventArgs> SelectionChanged;

        // ========================================
        // method
        // ========================================
        public void Select(IEditor selected) {
            if (selected == null) {
                return;
            }

            if (!MultiSelect && _selectedEditors.Contains(selected)) {
                return;
            }

            if (selected.Parent != _currentParent || !MultiSelect) {
                DeselectAllSelectedEditors();
                _currentParent = selected.Parent;
            }

            if (_selectedEditors.Contains(selected)) {
                return;
            }

            _selectedEditors.Add(selected);
            selected.IsSelected = true;

            _root.Site.UpdateHandleLayer();
            OnSelectionChanged();
        }

        // todo: 複数選択の高速化，RubberbandからのSelectCommand時にこれを呼ぶようにするだけで高速化できる
        public void SelectMulti(IEnumerable<IEditor> selecteds, bool toggle) {
            if (selecteds == null || !MultiSelect) {
                return;
            }

            if (!selecteds.Any()) {
                return;
            }

            var parent = selecteds.First().Parent;
            if (parent != _currentParent) {
                DeselectAllSelectedEditors();
            }
            _currentParent = parent;

            foreach (var selected in selecteds) {
                if (selected.Parent == _currentParent) {
                    if (toggle) {
                        if (_selectedEditors.Contains(selected)) {
                            _selectedEditors.Remove(selected);
                            selected.IsSelected = false;
                        } else {
                            _selectedEditors.Add(selected);
                            selected.IsSelected = true;
                        }
                    } else {
                        if (!_selectedEditors.Contains(selected)) {
                            _selectedEditors.Add(selected);
                            selected.IsSelected = true;
                        }
                    }
                } else {
                    _selectedEditors.Remove(selected);
                    selected.IsSelected = false;
                }
            }

            _root.Site.UpdateHandleLayer();
            OnSelectionChanged();
        }

        public void Deselect(IEditor deselected) {
            if (deselected == null || !_selectedEditors.Contains(deselected)) {
                return;
            }

            _selectedEditors.Remove(deselected);
            deselected.IsSelected = false;

            if (!_inDeselectAll) {
                _root.Site.UpdateHandleLayer();
                OnSelectionChanged();
            }
        }

        public void DeselectAll() {
            DeselectAllSelectedEditors();
            _root.Site.UpdateHandleLayer();
            OnSelectionChanged();
        }

        // ------------------------------
        // protected
        // ------------------------------
        internal void DeselectInternal(IEditor deselected) {
            _selectedEditors.Remove(deselected);
        }

        protected void DeselectAllSelectedEditors() {
            _inDeselectAll = true;
            try {
                foreach (var editor in _selectedEditors.ToArray()) {
                    editor.IsSelected = false;
                }
            } finally {
                _inDeselectAll = false;
            }
        }

        protected void OnSelectionChanged() {
            var handler = SelectionChanged;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
