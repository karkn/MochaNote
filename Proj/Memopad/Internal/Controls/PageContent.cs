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
using Mkamo.Editor.Core;
using Mkamo.Memopad.Internal.Utils;
using System.Drawing.Drawing2D;
using Mkamo.Memopad.Properties;
using Mkamo.Memopad.Core;
using Mkamo.Common.Forms.Drawing;
using Mkamo.Model.Memo;
using Mkamo.Common.String;
using Mkamo.Container.Core;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Model.Core;
using Mkamo.Common.Command;
using Mkamo.Common.Forms.Utils;
using Mkamo.Memopad.Internal.Forms;
using Mkamo.Common.Forms.Input;
using Mkamo.Memopad.Internal.Helpers;
using Mkamo.Common.Diagnostics;
using Mkamo.Common.Forms.Themes;
using ComponentFactory.Krypton.Toolkit;
using System.Diagnostics;
using Mkamo.Common.Win32.User32;
using Mkamo.Common.Forms.KeyMap;

namespace Mkamo.Memopad.Internal.Controls {
    [ToolboxItem(false)]
    internal partial class PageContent: UserControl {
        // ========================================
        // static field
        // ========================================
        /// <summary>
        /// control間のスペース．
        /// </summary>
        private const int ItemSpace = 8;

        private const int ItemVSpace = 12;

        /// <summary>
        /// borderを自分で描くコントロールの本当のboundsとborder間の距離
        /// </summary>
        private const int ItemPadding = 3;

        // ========================================
        // field
        // ========================================
        private MemopadApplication _facade;
        private Memo _memo;
        private MemoInfo _info;

        private bool _isModified;

        private bool _isSubInfoShown;
        private bool _isCompact;

        // --- theme ---
        private ITheme _theme;
        private Color _borderColor;

        private int _dateLabelWidth;

        private TagSelectorToolStripItem _tagSelectorToolStripItem;

        private bool _isInMemoSearcherShown;

        // --- key map ---
        private KeyMap<PageContent> _keyMap;
        private KeyMap<TextBox> _titleTextBoxKeyMap;
        private KeyMap<TextBox> _sourceTextBoxKeyMap;

        // ========================================
        // constructor
        // ========================================
        public PageContent(MemoInfo info) {
            InitializeComponent();
            DoubleBuffered = true;
            ResizeRedraw = true;

            _facade = MemopadApplication.Instance;
            _info = info;
            _titleTextBox.Text = _info.Title;

            Memo = _facade.Container.Find<Memo>(info.MemoId);
            _tagSelectorToolStripItem = new TagSelectorToolStripItem(_memo);
            _selectTagContextMenuStrip.AutoClose = false;

            _dateLabelWidth = _modifiedDateLabel.Width;

            _isInMemoSearcherShown = false;

            _keyMap = new KeyMap<PageContent>();
            if (!DesignMode) {
                DefineAdditionalKeyMap(_keyMap);
            }

            _titleTextBoxKeyMap = new KeyMap<TextBox>();
            _sourceTextBoxKeyMap = new KeyMap<TextBox>();
            if (!DesignMode) {
                _facade.KeySchema.PageContentTitleTextBoxKeyBinder.Bind(_titleTextBoxKeyMap);
                _facade.KeySchema.PageContentTitleTextBoxKeyBinder.Bind(_sourceTextBoxKeyMap);
            }

            _tagTextBox.GotFocus += (se, ev) => {
                User32PI.HideCaret(_tagTextBox.Handle);
            };
        }


        // ========================================
        // destructor
        // ========================================
        /// <summary>
        /// MemopadPageContent.Designer.csのDispose()で呼ばれる
        /// </summary>
        private void CleanUp() {
            if (_memo != null) {
                _memo.PropertyChanged -= HandleMemoPropChanged;
            }
            if (_tagSelectorToolStripItem != null) {
                _tagSelectorToolStripItem.Dispose();
            }

            _facade.Workspace.MemoTagChanged -= HandleMemoTagChanged;
            _facade.Workspace.MemoTagAdded -= HandleMemoTagAdded;
            _facade.Workspace.MemoTagRemoving -= HandleMemoTagRemoved;
        }
        
        // ========================================
        // event
        // ========================================
        public event EventHandler TitleChanged;
        public event EventHandler ContentModified;
        
        public event EventHandler InMemoSearcherShown;
        public event EventHandler InMemoSearcherHidden;

        // ========================================
        // property
        // ========================================
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Memo Memo {
            get { return _memo; }
            private set {
                if (_memo != null) {
                    _memo.PropertyChanged -= HandleMemoPropChanged;
                }
                _memo = value;
                if (_memo != null) {
                    _memo.PropertyChanged += HandleMemoPropChanged;
                    UpdateTagTextBox();
                    UpdateSourceTextBox();
                    UpdateDateLabel();
                }
            }
        }

        public MemoInfo MemoInfo {
            get { return _info; }
        }

        public EditorCanvas EditorCanvas {
            get { return _editorCanvas; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Title {
            get { return _titleTextBox.Text; }
            set {
                if (value == _titleTextBox.Text) {
                    return;
                }
                _titleTextBox.Text = value;
                ReflectTitle();
            }
        }

        public InMemoSearcher InMemoSearcher {
            get { return _inMemoSearcher; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsModified {
            get { return _isModified; }
            set { _isModified = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ITheme Theme {
            get { return _theme; }
            set {
                if (value == _theme) {
                    return;
                }
                _theme = value;

                _editorCanvas.Theme = value;

                var captionFont = value.CaptionFont;
                Font = captionFont;
                //_titleLabel.Font = captionFont;
                //_tagLabel.Font = captionFont;
                //_modifiedDateLabel.Font = captionFont;
                _tagSelectorToolStripItem.TagSelector.CaptionFont = captionFont;
                _inMemoSearcher.CaptionFont = captionFont;
                if (captionFont.SizeInPoints > 9.0f) {
                    _selectTagDropButton.StateCommon.Content.ShortText.Font = new Font(captionFont.Name, 9);
                    if (FontUtil.IsMeiryo(captionFont)) {
                        _selectTagDropButton.Height = 26;
                    }
                }

                var inputFont = value.InputFont;
                _titleTextBox.Font = inputFont;
                _tagTextBox.Font = inputFont;
                _tagSelectorToolStripItem.TagSelector.InputFont = inputFont;
                _inMemoSearcher.InputFont = inputFont;

                using (var g = CreateGraphics()) {
                    _dateLabelWidth = TextRenderer.MeasureText(
                        g, "作成日時: 9999/99/99 99:99", Font
                    ).Width;
                }

                _borderColor = KryptonManager.CurrentGlobalPalette.GetBorderColor1(
                    PaletteBorderStyle.ControlClient,
                    PaletteState.Normal
                );
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsCompact {
            get { return _isCompact; }
            set { SetCompact(value); }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsInMemoSearcherShown {
            get { return _isInMemoSearcherShown; }
        }

        // ========================================
        // method
        // ========================================
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (_keyMap.IsDefined(keyData)) {
                var action = _keyMap.GetAction(keyData);
                if (action != null) {
                    if (action(this)) {
                        return true;
                    }
                }
            }

            if (_titleTextBox.Focused) {
                if (_titleTextBoxKeyMap.IsDefined(keyData)) {
                    var action = _titleTextBoxKeyMap.GetAction(keyData);
                    if (action != null) {
                        if (action(_titleTextBox)) {
                            return true;
                        }
                    }
                }
            }

            if (_sourceTextBox.Focused) {
                if (_sourceTextBoxKeyMap.IsDefined(keyData)) {
                    var action = _sourceTextBoxKeyMap.GetAction(keyData);
                    if (action != null) {
                        if (action(_sourceTextBox)) {
                            return true;
                        }
                    }
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }


        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            //_tagTextBox.AutoCompleteMode = AutoCompleteMode.Suggest;
            //_tagTextBox.AutoCompleteSource = AutoCompleteSource.CustomSource;

            _inMemoSearcher.Hide();

            _editorCanvas.BackColor = Color.White;

            _selectTagContextMenuStrip.Margin = Padding.Empty;
            _selectTagContextMenuStrip.Padding = Padding.Empty;
            _selectTagContextMenuStrip.Items.Add(_tagSelectorToolStripItem);

            //_editorCanvas.GotFocus += HandleEditorCanvasGotFocus;
            _editorCanvas.CommandExecutor.CommandExecuted += HandleCommandExecutorCommandExecuted;
            _editorCanvas.CommandExecutor.CommandUndone += HandleCommandExecutorCommandUndone;
            _editorCanvas.CommandExecutor.CommandRedone += HandleCommandExecutorCommandRedone;
            _titleTextBox.LostFocus += HandleTitleTextBoxLostFocus;
            _sourceTextBox.LostFocus += HandleSourceTextBoxLostFocus;
            _modifiedDateLabel.Resize += HandleDateLabelResize;
            _selectTagContextMenuStrip.Opening += HandleSelectTagContextMenuStripOpening;
            _selectTagContextMenuStrip.Opened += HandleSelectTagContextMenuStripOpened;
            _selectTagContextMenuStrip.Closing += HandleSelectTagContextMenuStripClosing;

            _facade.Workspace.MemoTagChanged += HandleMemoTagChanged;
            _facade.Workspace.MemoTagAdded += HandleMemoTagAdded;
            _facade.Workspace.MemoTagRemoving += HandleMemoTagRemoved;

            SetSubInfoVisible(_facade.WindowSettings.SubInfoShown, true);

            if (_info != null) {
                _sourceTextBox.Text = _facade.Container.Find<Memo>(_info.MemoId).Source;
            }
        }


        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            Arrange();
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);

            var g = e.Graphics;
            var borderColor = SystemColors.ControlDark;
            var backColor = SystemColors.Window;

            if (_isCompact) {
                ControlPaint.DrawBorder(g, ClientRectangle, borderColor, ButtonBorderStyle.Solid);
                return;
            }

            var y = _editorCanvas.Top - 1;
            var y2 = _titleTextBox.Top - ItemPadding;
            var r = new Rectangle(0, 0, Width, y);

            using (var brush = new SolidBrush(_theme.DarkBackColor))
            using (var pen = new Pen(Color.FromArgb(160, 170, 200), 1)) {
                //g.FillRectangle(brush1, r1);
                //g.FillRectangle(brush2, r2);
                g.FillRectangle(brush, r);
                g.DrawLine(pen, new Point(0, y), new Point(Width, y));
            }
            if (_inMemoSearcher.Visible) {
                using (var pen = new Pen(_borderColor)) {
                    g.DrawLine(pen, new Point(0, _inMemoSearcher.Top - 1), new Point(Width, _inMemoSearcher.Top - 1));
                }
            }
            
            ControlPaintUtil.DrawBorder(g, _titleTextBox, borderColor, backColor, ItemPadding);
            ControlPaintUtil.DrawBorder(g, _tagTextBox, borderColor, backColor, ItemPadding);

            if (_isSubInfoShown) {
                using (var pen = new Pen(Color.Silver)) {
                    pen.DashStyle = DashStyle.Dash;
                    var lineY = (_tagTextBox.Top + _sourceTextBox.Bottom) / 2;
                    g.DrawLine(pen, new Point(ItemSpace, lineY), new Point(Width - ItemSpace - 1, lineY));
                }
                ControlPaintUtil.DrawBorder(g, _sourceTextBox, borderColor, backColor, ItemPadding);

                /// mark
                if (_memo != null) {
                    var marks = _memo.Marks;
                    var markCount = marks.Count;
                    var markImages = new List<Image>();
                    for (int i = 0; i < markCount; ++i) {
                        var def = MemoMarkUtil.GetMarkDefinition(marks[i]);
                        markImages.Add(def.Image);
                    }

                    if (markImages.Count > 0) {
                        var top = _sourceLabel.Bottom +ItemPadding + ((y - (_sourceLabel.Bottom + ItemPadding)) - markImages[0].Height) / 2;
                        var cLeft = _titleTextBox.Left - ItemPadding;
                        for (int i = 0; i < markCount; ++i) {
                            var image = markImages[i];
                            g.DrawImage(image, new Point(cLeft, top));
                            cLeft += image.Width + ItemPadding;
                        }
                    }
                }
            }

            ControlPaint.DrawBorder(g, ClientRectangle, _borderColor, ButtonBorderStyle.Solid);

        }

        // ------------------------------
        // public
        // ------------------------------
        public void StartSearch(bool isForward) {
            _inMemoSearcher.BeginSession(isForward);
        }

        public void FinishSearch(bool canceled) {
            _inMemoSearcher.EndSession(canceled);
        }

        public void SetCompact(bool value) {
            if (value == _isCompact) {
                return;
            }

            _isCompact = value;
            foreach (System.Windows.Forms.Control control in Controls) {
                if (control != _editorCanvas && control != _inMemoSearcher) {
                    control.Visible = !_isCompact;
                }
            }

            var visible = !_isCompact && _isSubInfoShown;
            _showSubInfoButton.Values.Image =
                visible ? Resources._2arrow_up : Resources._2arrow_down;

            _sourceLabel.Visible = visible;
            _sourceTextBox.Visible = visible;
            _showSourceButton.Visible = visible;
            _createdDateLabel.Visible = visible;
            _modifiedDateLabel.Visible = visible;

            Arrange();
            Invalidate();
        }

        // ------------------------------
        // protected
        // ------------------------------
        protected virtual void OnContentModified() {
            _isModified = true;

            var handler = ContentModified;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnTitleChanged() {
            _isModified = true;

            var handler = TitleChanged;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnInMemoSearcherShowed() {
            var handler = InMemoSearcherShown;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnInMemoSearcherHidden() {
            var handler = InMemoSearcherHidden;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        // ------------------------------
        // internal
        // ------------------------------
        internal void ShowInMemoSearcher() {
            // todo: 検索中はToolStripの更新はいらない
            _isInMemoSearcherShown = true;
            Arrange();
            _inMemoSearcher.Show();
            _inMemoSearcher.SearchTextBox.Focus();
            _inMemoSearcher.SearchTextBox.SelectAll();
            OnInMemoSearcherShowed();
        }

        internal void HideInMemoSearcher() {
            // todo: 検索が終わったらToolStripの更新を元に戻す
            // editor canvas のgotfocusでupdateしてもいいかも
            _isInMemoSearcherShown = false;
            _editorCanvas.Select();
            _inMemoSearcher.Hide();
            Arrange();
            OnInMemoSearcherHidden();
        }


        // ------------------------------
        // private
        // ------------------------------
        // --- key map ---
        private void DefineAdditionalKeyMap(IKeyMap<PageContent> keyMap) {
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

        private void DefineAdditionalKeyMapToDefault(IKeyMap<PageContent> keyMap) {
            keyMap.SetAction(Keys.F | Keys.Control, searcher => StartSearch(true));
        }

        private void DefineAdditionalKeyMapToEmacs(IKeyMap<PageContent> keyMap) {
            keyMap.SetAction(Keys.S | Keys.Control, searcher => StartSearch(true));
            keyMap.SetAction(Keys.R | Keys.Control, searcher => StartSearch(false));
        }

        //private void FixTags() {
        //    var tagsText = _tagTextBox.Text;
        //    if (StringUtil.IsNullOrWhitespace(tagsText)) {
        //        _tagTextBox.Text = "";
        //        return;
        //    }

        //    var facade = MemopadAppFacade.Instance;
        //    var tags = facade.Tags;

        //    var founds = new List<MemoTag>();
        //    var addings = new List<string>();
        //    var strs = tagsText.Split(',');
        //    foreach (var str in strs) {
        //        var tagStr = str.Trim();
        //        var found = tags.FirstOrDefault(
        //            tag => string.Equals(tag.Name, tagStr, StringComparison.CurrentCultureIgnoreCase)
        //        );
        //        if (found == null) {
        //            addings.Add(tagStr);
        //        } else {
        //            founds.Add(found);
        //        }
        //    }

        //    foreach (var found in founds) {
        //        _memo.AddTag(found);
        //    }
        //    foreach (var adding in addings) {
        //        var added = MemoFactory.CreateTag();
        //        added.Name = adding;
        //        _memo.AddTag(added);
        //    }

        //    _tagTextBox.Text = "";
        //}

        //private void UpdateTagTextAutoCompleteSource() {
        //    var facade = MemopadAppFacade.Instance;
        //    var tags = new List<string>();
        //    foreach (var tag in facade.Tags) {
        //        tags.Add(tag.Name);
        //    }
        //    var col = new AutoCompleteStringCollection();
        //    col.AddRange(tags.ToArray());
        //    _tagTextBox.AutoCompleteSource = AutoCompleteSource.None;
        //    _tagTextBox.AutoCompleteCustomSource = col;
        //    _tagTextBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
        //}

        private void Arrange() {
            SuspendLayout();

            if (_isCompact) {
                var rect = ClientRectangle;
                rect.Inflate(-1, -1);
                if (_isInMemoSearcherShown) {
                    _inMemoSearcher.Left = 1;
                    _inMemoSearcher.Top = Height - _inMemoSearcher.Height - 1;
                    _inMemoSearcher.Width = Width - 2;

                    _editorCanvas.Left = 1;
                    _editorCanvas.Width = Width - 2;
                    _editorCanvas.Top = 1;
                    _editorCanvas.Height = _inMemoSearcher.Top - _editorCanvas.Top - 1;
                } else {
                    _editorCanvas.Bounds = rect;
                }
                return;
            }

            _titleLabel.Left = ItemSpace;
            _titleLabel.Top = ItemVSpace;

            _tagLabel.Left = ItemSpace;
            _tagLabel.Top =  _titleLabel.Bottom + ItemSpace + ItemPadding * 2;

            var inputLeft = _titleLabel.Right + ItemSpace;
            _titleTextBox.Left = inputLeft + ItemPadding;
            _titleTextBox.Top = _titleLabel.Top;
            _titleTextBox.Width = Width - inputLeft - (ItemPadding + ItemSpace + ItemPadding);

            _tagTextBox.Width =
                (Width - inputLeft -
                (ItemPadding + /* _tagTextBox */ ItemPadding + ItemSpace + _selectTagDropButton.Width + ItemSpace +
                    _showSubInfoButton.Width + ItemSpace));
            _tagTextBox.Height = _titleTextBox.Height;

            _tagTextBox.Left = inputLeft + ItemPadding;
            _tagTextBox.Top = _titleTextBox.Bottom + (ItemSpace + ItemPadding * 2);

            _showSubInfoButton.Left = Width - _showSubInfoButton.Width - ItemSpace;
            _showSubInfoButton.Top = _tagLabel.Top;

            _selectTagDropButton.Left = _tagTextBox.Right + ItemPadding + ItemSpace;
            _selectTagDropButton.Top = (_tagTextBox.Top + _tagTextBox.Bottom) / 2 - (_selectTagDropButton.Height / 2);

            if (_isSubInfoShown) {
                _sourceLabel.Left = ItemSpace;
                _sourceLabel.Top = _tagTextBox.Bottom + ItemVSpace + ItemVSpace - 1;
                _sourceTextBox.Left = inputLeft + ItemPadding;
                _sourceTextBox.Top = _sourceLabel.Top;

                _showSourceButton.Left = Width - _showSourceButton.Width - ItemSpace;
                _showSourceButton.Top = (_sourceTextBox.Top + _sourceTextBox.Bottom) / 2 - (_showSourceButton.Height / 2);

                _sourceTextBox.Width = Width - inputLeft - (ItemPadding + ItemSpace + ItemPadding + _showSourceButton.Width);

                _modifiedDateLabel.Left = Width - _dateLabelWidth - ItemSpace;
                _modifiedDateLabel.Top = _sourceLabel.Bottom + ItemSpace + ItemPadding;

                _createdDateLabel.Left = _modifiedDateLabel.Left - ItemSpace - _dateLabelWidth;
                _createdDateLabel.Top = _modifiedDateLabel.Top;

                _editorCanvas.Top = _createdDateLabel.Bottom + ItemSpace;
            } else {
                _editorCanvas.Top = _tagTextBox.Bottom + ItemVSpace;
            }

            _editorCanvas.Left = 1;
            _editorCanvas.Width = Width - 2;

            if (_isInMemoSearcherShown) {
                _inMemoSearcher.Left = 1;
                _inMemoSearcher.Top = Height - _inMemoSearcher.Height - 1;
                _inMemoSearcher.Width = Width - 2;

                _editorCanvas.Height = _inMemoSearcher.Top - _editorCanvas.Top - 1;
            } else {
                _editorCanvas.Height = Height - _editorCanvas.Top - 1;
            }

            ResumeLayout();
        }

        private void UpdateTagTextBox() {
            _tagTextBox.Text = _memo.FullTagsText;
        }

        private void UpdateSourceTextBox() {
            _sourceTextBox.Text = _memo.Source;
            _showSourceButton.Enabled = UriUtil.IsHttpUri(_sourceTextBox.Text) || UriUtil.IsFileUri(_sourceTextBox.Text);
        }

        private void UpdateDateLabel() {
            _createdDateLabel.Text = "作成日時: " + _memo.CreatedDate.ToString("yyyy/MM/dd HH:mm");
            _modifiedDateLabel.Text = "更新日時: " + _memo.ModifiedDate.ToString("yyyy/MM/dd HH:mm");
        }

        private void ReflectTitle() {
            var title = _titleTextBox.Text;
            _memo.Title = title;
            _info.Title = title;
            OnContentModified();
            OnTitleChanged();
        }

        // --- event handler ---
        private void HandleMemoPropChanged(object sender, EventArgs e) {
            UpdateTagTextBox();
            UpdateSourceTextBox();
            UpdateDateLabel();
            Invalidate();
        }

        //private void HandleFixTagButtonClick(object sender, EventArgs e) {
        //    FixTags();
        //}

        //private void HandleTagTextBoxKeyDown(object sender, KeyEventArgs e) {
        //    if (e.KeyData == Keys.Enter) {
        //        FixTags();
        //        UpdateTagTextAutoCompleteSource();
        //        e.SuppressKeyPress = true;
        //    }
        //}

        //private void HandleTitleTextBoxEnter(object sender, EventArgs e) {
        //    UpdateTagTextAutoCompleteSource();
        //}

        private void HandleCommandExecutorCommandExecuted(object sender, CommandEventArgs e) {
            if (_memo != null && e.Command != null && e.Command.CanUndo) {
                _memo.ModifiedDate = DateTime.Now;
                UpdateDateLabel();
                OnContentModified();
            }
        }

        private void HandleCommandExecutorCommandUndone(object sender, CommandEventArgs e) {
            if (_memo != null && e.Command != null && e.Command.CanUndo) {
                _memo.ModifiedDate = DateTime.Now;
                UpdateDateLabel();
                OnContentModified();
            }
        }

        private void HandleCommandExecutorCommandRedone(object sender, CommandEventArgs e) {
            if (_memo != null && e.Command != null && e.Command.CanUndo) {
                _memo.ModifiedDate = DateTime.Now;
                UpdateDateLabel();
                OnContentModified();
            }
        }

        private void HandleTitleTextBoxLostFocus(object sender, EventArgs e) {
            ReflectTitle();
        }

        private void HandleSourceTextBoxLostFocus(object sender, EventArgs e) {
            _memo.Source = _sourceTextBox.Text;
            OnContentModified();
        }

        private void HandleDateLabelResize(object sender, EventArgs e) {
            _modifiedDateLabel.Left = Right - ItemSpace - _modifiedDateLabel.Width;
        }

        private void HandleSelectTagContextMenuStripOpening(object sender, CancelEventArgs e) {
            var oldCursor = Cursor;
            try {
                Cursor = Cursors.WaitCursor;
                _tagSelectorToolStripItem.TagSelector.InitUI();
                _tagSelectorToolStripItem.TagSelector.RequireClose += HandleTagSelectorRequireClose;
            } finally {
                Cursor = oldCursor;
            }
        }

        private void HandleSelectTagContextMenuStripOpened(object sender, EventArgs e) {
            _tagSelectorToolStripItem.TagSelector.TagTextBox.Focus();
        }

        private void HandleSelectTagContextMenuStripClosing(object sender, CancelEventArgs e) {
            _tagSelectorToolStripItem.TagSelector.ReflectChecksToMemo(_memo);
            _tagSelectorToolStripItem.TagSelector.RequireClose -= HandleTagSelectorRequireClose;
        }

        private void HandleTagSelectorRequireClose(object sender, EventArgs e) {
            _selectTagContextMenuStrip.Close(ToolStripDropDownCloseReason.CloseCalled);
            _editorCanvas.Select();
        }

        //private void HandleEditorCanvasGotFocus(object sender, EventArgs e) {
        //    if (_isInMemoSearcherShown) {
        //        _inMemoSearcher.EndSession(false);
        //    }
        //}

        private void HandleMemoTagChanged(object sender, MemoTagEventArgs e) {
            UpdateTagTextBox();
        }

        private void HandleMemoTagAdded(object sender, MemoTagEventArgs e) {
            UpdateTagTextBox();
        }

        private void HandleMemoTagRemoved(object sender, MemoTagEventArgs e) {
            UpdateTagTextBox();
        }

        private void SetSubInfoVisible(bool visible, bool force) {
            if (!force && visible == _isSubInfoShown) {
                return;
            }

            _isSubInfoShown = visible;
            _facade.WindowSettings.SubInfoShown = _isSubInfoShown;

            _showSubInfoButton.Values.Image =
                visible ? Resources._2arrow_up : Resources._2arrow_down;

            _sourceLabel.Visible = visible;
            _sourceTextBox.Visible = visible;
            _showSourceButton.Visible = visible;
            _createdDateLabel.Visible = visible;
            _modifiedDateLabel.Visible = visible;
        }

        // --- generated handler ---
        private void _showSubInfoButton_Click(object sender, EventArgs e) {
            _editorCanvas.Invalidate();
            SetSubInfoVisible(!_isSubInfoShown, false);
            Arrange();
            Invalidate();
        }

        private void _showSourceButton_Click(object sender, EventArgs e) {
            if (UriUtil.IsHttpUri(_sourceTextBox.Text) || UriUtil.IsFileUri(_sourceTextBox.Text)) {
                Process.Start(_sourceTextBox.Text);
            }
        }

        private void _tagTextBox_Click(object sender, EventArgs e) {
            _selectTagDropButton.Focus();
            _selectTagDropButton.PerformDropDown();
        }

    }
}
