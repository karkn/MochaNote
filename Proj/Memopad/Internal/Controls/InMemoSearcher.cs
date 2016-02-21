/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using Mkamo.Common.Forms.Utils;
using Mkamo.Editor.Core;
using Mkamo.Editor.Focuses;
using Mkamo.Model.Memo;
using Mkamo.Common.Forms.Input;
using Mkamo.Memopad.Internal.Helpers;
using Mkamo.Common.Diagnostics;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Memopad.Core;
using ComponentFactory.Krypton.Toolkit;
using Mkamo.Figure.Core;
using Mkamo.Common.Forms.KeyMap;

namespace Mkamo.Memopad.Internal.Controls {
    [ToolboxItem(false)]
    internal partial class InMemoSearcher: UserControl {
        // ========================================
        // static field
        // ========================================
        /// <summary>
        /// control間のスペース．
        /// </summary>
        private const int ItemSpace = 10;

        /// <summary>
        /// borderを自分で描くコントロールの本当のboundsとborder間の距離
        /// </summary>
        private const int ItemPadding = 3;

        // ========================================
        // field
        // ========================================
        private State _sessionState;

        // --- keymap ---
        private KeyMap<InMemoSearcher> _keyMap;
        private KeyMap<TextBox> _searchTextBoxKeyMap;
        private KeyMap<TextBox> _replaceTextBoxKeyMap;

        // --- font ---
        private Font _captionFont;
        private Font _inputFont;

        // ========================================
        // constructor
        // ========================================
        public InMemoSearcher() {
            InitializeComponent();

            DoubleBuffered = true;
        }

        // ========================================
        // event
        // ========================================
        public event EventHandler Searching;
        public event EventHandler Searched;

        // ========================================
        // property
        // ========================================
        public string SearchText {
            get { return _searchTextBox.Text; }
        }

        public string ReplaceText {
            get { return _replaceTextBox.Text; }
        }

        public Font CaptionFont {
            set {
                _captionFont = value;
                _searchLabel.Font = value;
                _replaceLabel.Font = value;
            }
        }

        public Font InputFont {
            set {
                _inputFont = value;
                _searchTextBox.Font = value;
                _replaceTextBox.Font = value;
            }
        }


        internal PageContent PageContent {
            get { return Parent as PageContent; }
        }

        internal EditorCanvas EditorCanvas {
            get { return PageContent.EditorCanvas; }
        }

        internal TextBox SearchTextBox {
            get { return _searchTextBox; }
        }

        internal TextBox ReplaceTextBox {
            get { return _replaceTextBox; }
        }

        // ========================================
        // method
        // ========================================
        // === Control ==========
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (_keyMap.IsDefined(keyData)) {
                var action = _keyMap.GetAction(keyData);
                if (action != null) {
                    if (action(this)) {
                        return true;
                    }
                }
            }

            if (_searchTextBox.Focused) {
                if (_searchTextBoxKeyMap.IsDefined(keyData)) {
                    var action = _searchTextBoxKeyMap.GetAction(keyData);
                    if (action != null) {
                        if (action(_searchTextBox)) {
                            return true;
                        }
                    }
                }
            }

            if (_replaceTextBox.Focused) {
                if (_replaceTextBoxKeyMap.IsDefined(keyData)) {
                    var action = _replaceTextBoxKeyMap.GetAction(keyData);
                    if (action != null) {
                        if (action(_replaceTextBox)) {
                            return true;
                        }
                    }
                }
            }
            
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);

            var g = e.Graphics;
            var r = new Rectangle(0, 0, Width - 1, Height - 1);
            using (
                var brush = new LinearGradientBrush(
                    r, SystemColors.ControlLightLight, SystemColors.Control, LinearGradientMode.Vertical
                )
            ) {
                g.FillRectangle(brush, r);
            }
            
            var borderColor = KryptonManager.CurrentGlobalPalette.GetBorderColor1(
                PaletteBorderStyle.ControlClient,
                PaletteState.Normal
            );
            var backColor = SystemColors.Window;
            ControlPaintUtil.DrawBorder(g, _searchTextBox, borderColor, backColor, ItemPadding);
            ControlPaintUtil.DrawBorder(g, _replaceTextBox, borderColor, _replaceTextBox.Enabled ? backColor : SystemColors.Control, ItemPadding);
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            _searchTextBox.TextChanged += HandleSearchTextBoxTextChanged;
            _replaceTextBox.TextChanged += HandleReplaceTextBoxTextChanged;

            _keyMap = new KeyMap<InMemoSearcher>();
            _searchTextBoxKeyMap = new KeyMap<TextBox>();
            _replaceTextBoxKeyMap = new KeyMap<TextBox>();


            var facade = MemopadApplication.Instance;
            if (!DesignMode) {
                DefineAdditionalKeyMap(_keyMap);
                facade.KeySchema.TextBoxKeyBinder.Bind(_searchTextBoxKeyMap);
                facade.KeySchema.TextBoxKeyBinder.Bind(_replaceTextBoxKeyMap);
            }
        }

        // === InMemoSearcher ==========
        // ------------------------------
        // public
        // ------------------------------
        public void BeginSession(bool isForward) {
            PageContent.ShowInMemoSearcher();
            StoreState(isForward);
            UpdateUIState();

            _highlightCheckBox.Checked = false;
        }

        public void EndSession(bool canceled) {
            if (canceled) {
                RestoreState();
            }
            PageContent.HideInMemoSearcher();

            if (_highlightCheckBox.Checked) {
                DisableHighlight();
            }
        }

        public void SearchForwardFirst(string s) {
            SearchBase(s, true, true);
            UpdateUIState();
        }

        public void SearchForwardNext(string s) {
            SearchBase(s, false, true);
            UpdateUIState();
        }

        public void SearchBackwardFirst(string s) {
            SearchBase(s, true, false);
            UpdateUIState();
        }

        public void SearchBackwardNext(string s) {
            SearchBase(s, false, false);
            UpdateUIState();
        }

        public void ReplaceForward(string s, string replace) {
            if (_replaceForwardButton.Enabled) {
                var canvas = EditorCanvas;
                if (canvas.FocusManager.IsEditorFocused) {
                    var focus = canvas.FocusManager.Focus as StyledTextFocus;
                    if (focus != null) {
                        if (_replaceTextBox.Text == "") {
                            focus.RemoveForward();
                        } else {
                            focus.Insert(replace);
                        }
                        StoreState(true);
                        SearchForwardNext(s);
                        UpdateUIState();
                    }
                }
            }

        }
        
        public void ReplaceBackward(string s, string replace) {
            if (_replaceBackwardButton.Enabled) {
                var canvas = EditorCanvas;
                if (canvas.FocusManager.IsEditorFocused) {
                    var focus = canvas.FocusManager.Focus as StyledTextFocus;
                    if (focus != null) {
                        if (_replaceTextBox.Text == "") {
                            focus.RemoveForward();
                        } else {
                            focus.Insert(replace);
                        }
                        StoreState(false);
                        SearchBackwardNext(s);
                        UpdateUIState();
                    }
                }
            }
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected virtual void OnSearching() {
            var handler = Searching;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnSearched() {
            var handler = Searched;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        // ------------------------------
        // private
        // ------------------------------
        private void SearchBase(string s, bool isFirst, bool isForward) {
            OnSearching();

            try {
                if (string.IsNullOrEmpty(s)) {
                    return;
                }
    
                if (isFirst) {
                    RestoreState();
                }
    
                var canvas = EditorCanvas;
    
                if (canvas.FocusManager.IsEditorFocused) {
                    /// focusされていれば
    
                    var focus = canvas.FocusManager.Focus as StyledTextFocus;
                    if (focus != null) {
    
                        if (SearchInFocus(focus, s, isFirst, isForward)) {
                            /// focus内検索して範囲選択
    
                        } else {
                            /// 見つからなければfocusされたeditorを起点に他のeditorを検索
    
                            var targets = GetTargets(isForward);
                            var found = false;
                            var searched = SearchEditor(
                                s,
                                targets,
                                e => {
                                    if (isFirst) {
                                        return e == canvas.FocusManager.FocusedEditor;
                                    } else {
                                        /// focusされているeditorの次のeditor
                                        if (found) {
                                            return true;
                                        }
                                        if (e == canvas.FocusManager.FocusedEditor) {
                                            found = true;
                                        }
                                    return false;
                                    }
                                }
                            );
                            if (searched != null) {
                                canvas.FocusManager.FocusedEditor.RequestFocusCommit(true);
                                SetSearched(searched, s, isFirst, isForward);
                            }
                        }
                    }
    
                } else {
                    var selected = canvas.SelectionManager.SelectedEditors;
                    if (selected.Contains(canvas.RootEditor.Content)) {
                        /// Memoが選択されているので最初から検索
                        var targets = GetTargets(isForward);
                        var searched = SearchEditor(
                            s,
                            targets,
                            e => IsTargetBounds(canvas.Caret.Position, e.Figure.Bounds, isForward)
                        );
                        if (searched != null) {
                            SetSearched(searched, s, isFirst, isForward);
                        }
    
                    } else {
                        /// 選択されているeditorを起点に検索
                        var targets = GetTargets(isForward);
                        var found = false;
                        var searched = SearchEditor(
                            s,
                            targets,
                            e => {
                                if (isFirst) {
                                    return e == canvas.FocusManager.FocusedEditor;
                                } else {
                                    /// 選択されているeditorの次のeditor
                                    if (found) {
                                        return true;
                                    }
                                    if (canvas.SelectionManager.SelectedEditors.Contains(e)) {
                                        found = true;
                                    }
                                    return false;
                                }
                            }
                        );
                        if (searched != null) {
                            SetSearched(searched, s, isFirst, isForward);
                        }
                    }
                }
            } finally {
                OnSearched();
            }
        }

        private IEditor SearchEditor(string s, IEnumerable<IEditor> targets, Func<IEditor, bool> baseJudge) {
            var baseFound = false;
            foreach (var child in targets) {
                if (baseJudge == null || baseJudge(child)) {
                    baseFound = true;
                }
                if (baseFound) {
                    /// selected内の初めて見つけた図形を起点に検索
                    var found = false;
                    child.Accept(
                        e => {
                            var text = e.Controller.GetText();
                            if (!string.IsNullOrEmpty(text) && text.IndexOf(s, StringComparison.OrdinalIgnoreCase) > -1) {
                                found = true;
                                return true;
                            }
                            return false;
                        }
                    );
                    if (found) {
                        return child;
                    }
                }
            }

            return null;
        }

        private void SetSearched(IEditor editor, string s, bool isFirst, bool isForward) {
            if (editor.Model is MemoText) {
                /// MemoTextならフォーカスして検索
                editor.RequestSelect(SelectKind.True, true);
                editor.RequestFocus(FocusKind.Begin, null);
                var focus = editor.Focus as StyledTextFocus;
                if (focus != null) {
                    if (!isForward) {
                        /// 最初のcaret位置を一番最後の文字に
                        focus.MoveEndOfText();
                    }
                    SearchInFocus(focus, s, isFirst, isForward);
                }
            } else {
                /// MemoTextでなければ選択
                editor.RequestSelect(SelectKind.True, true);
                EditorCanvas.EnsureVisible(editor);
            }
        }

        private bool IsTargetBounds(Point pos, Rectangle bounds, bool isForward) {
            return
                isForward?
                    pos.X <= bounds.Right && pos.Y <= bounds.Bottom :
                    pos.X >= bounds.Left && pos.Y >= bounds.Top;
        }

        private IEnumerable<IEditor> GetTargets(bool isForward) {
            var children = EditorCanvas.RootEditor.Content.GetChildrenByPosition();
            if (isForward) {
                return children.ToArray();
            } else {
                return children.Reverse().ToArray();
            }
        }

        private bool SearchInFocus(StyledTextFocus focus, string s, bool isFirst, bool isForward) {
            if (isForward) {
                if (isFirst) {
                    return focus.SearchForwardFirst(s);
                } else {
                    return focus.SearchForwardNext(s);
                }
            } else {
                if (isFirst) {
                    return focus.SearchBackwardFirst(s);
                } else {
                    return focus.SearchBackwardNext(s);
                }
            }
        }

        // --- state ---
        private void StoreState(bool isForward) {
            var canvas = EditorCanvas;

            _sessionState = new State();
            _sessionState.IsForward = isForward;
            if (canvas.FocusManager.IsEditorFocused) {
                _sessionState.Kind = StateKind.Focused;
                _sessionState.FocusedEditor = canvas.FocusManager.FocusedEditor;
                var focus = canvas.FocusManager.Focus as StyledTextFocus;
                if (focus != null) {
                    _sessionState.CaretIndex = focus.Referer.CaretIndex;
                }
            } else {
                _sessionState.Kind = StateKind.Selected;
                _sessionState.SelectedEditors = canvas.SelectionManager.SelectedEditors.ToArray();
                _sessionState.CaretPosition = canvas.Caret.Position;
            }
        }

        private void RestoreState() {
            var canvas = EditorCanvas;
            var fman = canvas.FocusManager;

            if (_sessionState.Kind == StateKind.Focused) {
                if (fman.IsEditorFocused) {
                    if (_sessionState.FocusedEditor != fman.FocusedEditor) {
                        fman.FocusedEditor.RequestFocusCommit(true);
                        _sessionState.FocusedEditor.RequestFocus(FocusKind.Begin, null);
                    }
                }
                var focus = fman.Focus as StyledTextFocus;
                if (focus != null) {
                    focus.Referer.CaretIndex = Math.Min(_sessionState.CaretIndex, focus.Referer.Target.Length);
                    focus.Selection.Clear();
                }
            } else if (_sessionState.Kind == StateKind.Selected) {
                if (fman.IsEditorFocused) {
                    fman.FocusedEditor.RequestFocusCommit(true);
                }
                canvas.SelectionManager.DeselectAll();
                foreach (var selected in _sessionState.SelectedEditors.ToArray()) {
                    selected.RequestSelect(SelectKind.True, false);
                }
                canvas.Caret.Position = _sessionState.CaretPosition;
            }
        }

        // --- ui ---
        private void UpdateUIState() {
            _searchForwardButton.Enabled = !string.IsNullOrEmpty(_searchTextBox.Text);
            _searchBackwardButton.Enabled = !string.IsNullOrEmpty(_searchTextBox.Text);
            _highlightCheckBox.Enabled = !string.IsNullOrEmpty(_searchTextBox.Text);

            var replaceEnabled = false;
            //if (!string.IsNullOrEmpty(_replaceTextBox.Text)) {
            var canvas = EditorCanvas;
            if (canvas.FocusManager.IsEditorFocused) {
                var focus = canvas.FocusManager.Focus as StyledTextFocus;
                if (focus != null) {
                    replaceEnabled = !focus.Referer.Selection.IsEmpty;
                }
            }
            //}
            var needInvalidate = replaceEnabled != _replaceTextBox.Enabled;
            _replaceTextBox.Enabled = replaceEnabled;
            _replaceForwardButton.Enabled = replaceEnabled;
            _replaceBackwardButton.Enabled = replaceEnabled;
            if (needInvalidate) {
                Invalidate();
            }
        }

        // --- key map ---
        private static void DefineAdditionalKeyMap(IKeyMap<InMemoSearcher> keyMap) {
            Contract.Requires(keyMap != null);

            var facade = MemopadApplication.Instance;
            switch (facade.Settings.KeyScheme) {
                case KeySchemeKind.Emacs: {
                    DefineAdditionalKeyMapToEmacs(keyMap);
                    break;
                }
                default: {
                    DefineAdditionalKeyMapToDefault(keyMap);
                    break;
                }
            }
        }

        private static void DefineAdditionalKeyMapToDefault(IKeyMap<InMemoSearcher> keyMap) {
            keyMap.SetAction(Keys.F | Keys.Control, searcher => searcher.SearchForwardNext(searcher.SearchText));

            keyMap.SetAction(Keys.Enter, searcher => searcher.EndSession(false));
            keyMap.SetAction(Keys.Escape, searcher => searcher.EndSession(true));
        }

        private static void DefineAdditionalKeyMapToEmacs(IKeyMap<InMemoSearcher> keyMap) {
            keyMap.SetAction(Keys.S | Keys.Control, searcher => searcher.SearchForwardNext(searcher.SearchText));
            keyMap.SetAction(Keys.R | Keys.Control, searcher => searcher.SearchBackwardNext(searcher.SearchText));

            keyMap.SetAction(
                Keys.S | Keys.Control | Keys.Alt,
                searcher => searcher.ReplaceForward(searcher.SearchText, searcher.ReplaceText)
            );
            keyMap.SetAction(
                Keys.R | Keys.Control | Keys.Alt,
                searcher => searcher.ReplaceBackward(searcher.SearchText, searcher.ReplaceText)
            );

            keyMap.SetAction(Keys.Enter, searcher => searcher.EndSession(false));
            keyMap.SetAction(Keys.Escape, searcher => searcher.EndSession(true));
            keyMap.SetAction(Keys.G | Keys.Control, searcher => searcher.EndSession(true));
        }

        // --- highlight ---
        private void EnableHighlight() {
            var hls = Highlight.CreateHighlights(_searchTextBox.Text);
            EditorCanvas.HighlightRegistry.GlobalHighlights = hls;
            EditorCanvas.DirtyAllVisualLines();
        }

        private void DisableHighlight() {
            var hls = Highlight.CreateHighlights(null);
            EditorCanvas.HighlightRegistry.GlobalHighlights = hls;
            EditorCanvas.DirtyAllVisualLines();
        }

        // --- handler ---
        private void HandleSearchTextBoxTextChanged(object sender, EventArgs e) {
            if (string.IsNullOrEmpty(SearchText)) {
                RestoreState();
            } else {
                if (_sessionState.IsForward) {
                    SearchForwardFirst(_searchTextBox.Text);
                } else {
                    SearchBackwardFirst(_searchTextBox.Text);
                }
            }
            UpdateUIState();
        }

        private void HandleReplaceTextBoxTextChanged(object sender, EventArgs e) {
            UpdateUIState();
        }

        private void _searchForwardButton_Click(object sender, EventArgs e) {
            SearchForwardNext(_searchTextBox.Text);
        }

        private void _searchBackwordButton_Click(object sender, EventArgs e) {
            SearchBackwardNext(_searchTextBox.Text);
        }

        private void _replaceForwardButton_Click(object sender, EventArgs e) {
            ReplaceForward(_searchTextBox.Text, _replaceTextBox.Text);
        }

        private void _replaceBackwardButton_Click(object sender, EventArgs e) {
            ReplaceBackward(_searchTextBox.Text, _replaceTextBox.Text);
        }

        private void _closeButton_Click(object sender, EventArgs e) {
            EndSession(false);
        }

        private void _highlightCheckBox_CheckedChanged(object sender, EventArgs e) {
            if (_highlightCheckBox.Checked) {
                EnableHighlight();
            } else {
                DisableHighlight();
            }
        }

        private void _searchTextBox_TextChanged(object sender, EventArgs e) {
            if (_highlightCheckBox.Checked) {
                EnableHighlight();
            }
        }

        // ========================================
        // type
        // ========================================
        private enum StateKind {
            None,
            Focused,
            Selected,
        }

        private struct State {
            public bool IsForward;
            public StateKind Kind;
            public IEditor FocusedEditor;
            public int CaretIndex;
            public IEnumerable<IEditor> SelectedEditors;
            public Point CaretPosition;
        }

    }
}
