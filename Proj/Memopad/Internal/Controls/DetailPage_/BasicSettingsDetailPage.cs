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
using Mkamo.Common.Forms.DetailSettings;
using Mkamo.Common.Forms.Themes;
using Mkamo.Memopad.Core;
using Mkamo.Common.Command;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Editor.Core;
using Mkamo.Figure.Figures;

namespace Mkamo.Memopad.Internal.Controls {
    [ToolboxItem(false)]
    internal partial class BasicSettingsDetailPage: UserControl, IDetailSettingsPage {
        // ========================================
        // field
        // ========================================
        private ITheme _theme;
        private bool _isModified;

        private MemopadSettings _settings;
        private MemopadWindowSettings _windowSettings;
        private IToolRegistry _toolRegistry;

        private string _previousFontName;

        // ========================================
        // constructor
        // ========================================
        public BasicSettingsDetailPage(
            MemopadSettings settings,
            MemopadWindowSettings windowSettings,
            IToolRegistry toolRegistry
        ) {
            InitializeComponent();

            _settings = settings;
            _windowSettings = windowSettings;
            _toolRegistry = toolRegistry;

            InitFontNameToolStripComboBox();
            InitFontSizeToolStripComboBox();
            _useClearTypeCheckBox.Checked = _settings.UseClearType;
            _editorCanvasImeOnCheckBox.Checked = _settings.EditorCanvasImeOn;

            InitMemoTextFrameVisiblePolicyComboBox();
            InitMemoTextDefaultMaxWidthComboBox();

            KeyScheme = _settings.KeyScheme;

            _showLineBreakCheckBox.Checked = _windowSettings.ShowLineBreak;
            _showBlockBreakCheckBox.Checked = _windowSettings.ShowBlockBreak;

            _memoDefaultFontNameComboBox.SelectedIndexChanged += HandleMemoDefaultFontNameComboBoxSelectedIndexChanged;
            _memoDefaultFontSizeComboBox.SelectedIndexChanged += HandleControlValueChanged;
            _useClearTypeCheckBox.CheckedChanged += HandleControlValueChanged;
            _keySchemeComboBox.SelectedIndexChanged += HandleControlValueChanged;
            _memoTextFrameVisiblePolicyComboBox.SelectedIndexChanged += HandleControlValueChanged;
            _memoTextDefaultMaxWidthcomboBox.SelectedIndexChanged += HandleControlValueChanged;
            _showLineBreakCheckBox.CheckedChanged += HandleControlValueChanged;
            _showBlockBreakCheckBox.CheckedChanged += HandleControlValueChanged;
            _editorCanvasImeOnCheckBox.CheckedChanged += HandleControlValueChanged;

            _isModified = false;
        }

        private void InitFontNameToolStripComboBox() {
            _memoDefaultFontNameComboBox.Items.AddRange(RegularFontNames.Instance.FontNames);
            _memoDefaultFontNameComboBox.Text = _settings.DefaultMemoTextFontName;
            _previousFontName = _settings.DefaultMemoTextFontName;
        }

        private void InitFontSizeToolStripComboBox() {
            _memoDefaultFontSizeComboBox.Items.AddRange(
                new[] {
                    "8",
                    "9",
                    "10",
                    "11",
                    "12",
                    "14",
                    "16",
                    "18",
                    "20",
                    "24",
                    "28",
                    "32",
                    "36",
                    "40",
                    "44",
                    "48",
                }
            );
            _memoDefaultFontSizeComboBox.Text = _settings.DefaultMemoTextFontSize.ToString();
        }

       //private void InitDefaultShapeComboBox() {
        //    var ids = _toolRegistry.CreateNodeToolIds;
        //    foreach (var id in ids) {
        //        var text = _toolRegistry.GetCreateNodeToolText(id);
        //        _defaultShapeComboBox.Items.Add(new DefaultShapeComboBoxItem(id, text));
        //    }
        //    _defaultShapeComboBox.Text = _toolRegistry.GetCreateNodeToolText(_settings.DefaultShapeId);
        //}

        private void InitMemoTextFrameVisiblePolicyComboBox() {
            var pol = (HandleStickyKind) _settings.MemoTextFrameVisiblePolicy;
            switch (pol) {
                case HandleStickyKind.Selected:
                    _memoTextFrameVisiblePolicyComboBox.SelectedIndex = 2;
                    break;
                case HandleStickyKind.MouseEnter:
                    _memoTextFrameVisiblePolicyComboBox.SelectedIndex = 0;
                    break;
                case HandleStickyKind.MouseOver:
                    _memoTextFrameVisiblePolicyComboBox.SelectedIndex = 1;
                    break;
                case HandleStickyKind.Always:
                case HandleStickyKind.SelectedIncludingChildren:
                default:
                    _memoTextFrameVisiblePolicyComboBox.SelectedIndex = 0;
                    break;
            }
        }

        private void InitMemoTextDefaultMaxWidthComboBox() {
            _memoTextDefaultMaxWidthcomboBox.Items.AddRange(
                new [] {
                    "自動",
                    "100",
                    "150",
                    "200",
                    "250",
                    "300",
                    "350",
                    "400",
                    "450",
                    "500",
                    "550",
                    "600",
                    "650",
                    "700",
                    "750",
                    "800",
                    "850",
                    "900",
                    "950",
                }
            );
            _memoTextDefaultMaxWidthcomboBox.Text = _windowSettings.MemoTextDefaultMaxWidth.ToString();
            if (_memoTextDefaultMaxWidthcomboBox.SelectedIndex == -1) {
                _memoTextDefaultMaxWidthcomboBox.SelectedIndex = 0;
            }
        }

        // ========================================
        // property
        // ========================================
        public System.Windows.Forms.Control PageControl {
            get { return this; }
        }

        public bool NeedBorder {
            get { return true; }
        }

        public ITheme Theme {
            get { return _theme; }
            set {
                if (value == _theme) {
                    return;
                }
                _theme = value;

                var captionFont = value.CaptionFont;
                Font = captionFont;
            }
        }

        public bool IsModified {
            get { return _isModified; }
            set { _isModified = value; }
        }


        public string MemoDefaultFontName {
            get { return _memoDefaultFontNameComboBox.Text; }
            set {
                if (value == _memoDefaultFontNameComboBox.Text) {
                    return;
                }
                _memoDefaultFontNameComboBox.Text = value;
                _isModified = true;
            }
        }

        public int MemoDefaultFontSize {
            get { return int.Parse(_memoDefaultFontSizeComboBox.Text); }
            set {
                if (value == int.Parse(_memoDefaultFontSizeComboBox.Text)) {
                    return;
                }
                _memoDefaultFontSizeComboBox.Text = value.ToString();
                _isModified = true;
            }
        }

        public KeySchemeKind KeyScheme {
            get {
                switch (_keySchemeComboBox.SelectedIndex) {
                    case 1: {
                        return KeySchemeKind.Emacs;
                    }
                    case 0:
                    default: {
                        return KeySchemeKind.Default;
                    }
                }
            }

            set {
                switch (value) {
                    case KeySchemeKind.Default: {
                        if (_keySchemeComboBox.SelectedIndex == 0) {
                            return;
                        }
                        _keySchemeComboBox.SelectedIndex = 0;
                        _isModified = true;
                        break;
                    }
                    case KeySchemeKind.Emacs: {
                        if (_keySchemeComboBox.SelectedIndex == 1) {
                            return;
                        }
                        _keySchemeComboBox.SelectedIndex = 1;
                        _isModified = true;
                        break;
                    }
                }
            }
        }

        //public string DefaultShapeId {
        //    get {
        //        var item = (DefaultShapeComboBoxItem) _defaultShapeComboBox.SelectedItem;
        //        return item.Id;
        //    }
        //}

        public HandleStickyKind MemoTextFrameVisiblePolicy {
            get {
                switch (_memoTextFrameVisiblePolicyComboBox.SelectedIndex) {
                    case 0:
                        return HandleStickyKind.MouseEnter;
                    case 1:
                        return HandleStickyKind.MouseOver;
                    case 2:
                        return HandleStickyKind.Selected;
                    default:
                        return HandleStickyKind.MouseEnter;
                }
            }
        }

        public int MemoTextDefaultMaxWidth {
            get {
                if (_memoTextDefaultMaxWidthcomboBox.SelectedIndex == 0) {
                    return -1;
                } else {
                    return int.Parse(_memoTextDefaultMaxWidthcomboBox.Text);
                }
            }
        }

        // ========================================
        // method
        // ========================================
        public ICommand GetUpdateCommand() {
            return new DelegatingCommand(
                () => {
                    _settings.DefaultMemoTextFontName = MemoDefaultFontName;
                    _settings.DefaultMemoContentFontName = MemoDefaultFontName;
                    _settings.DefaultUmlFontName = MemoDefaultFontName;

                    _settings.DefaultMemoTextFontSize = MemoDefaultFontSize;
                    _settings.DefaultMemoContentFontSize = MemoDefaultFontSize;
                    _settings.DefaultUmlFontSize = MemoDefaultFontSize;

                    _settings.UseClearType = _useClearTypeCheckBox.Checked;

                    _settings.KeyScheme = KeyScheme;

                    //_settings.DefaultShapeId = DefaultShapeId;

                    _settings.MemoTextFrameVisiblePolicy = (int) MemoTextFrameVisiblePolicy;
                    _windowSettings.MemoTextDefaultMaxWidth = MemoTextDefaultMaxWidth;

                    _windowSettings.ShowLineBreak = _showLineBreakCheckBox.Checked;
                    _windowSettings.ShowBlockBreak = _showBlockBreakCheckBox.Checked;

                    _settings.EditorCanvasImeOn = _editorCanvasImeOnCheckBox.Checked;
                }
            );
        }

        private void HandleControlValueChanged(object sender, EventArgs e) {
            _isModified = true;
        }

        private void HandleMemoDefaultFontNameComboBoxSelectedIndexChanged(object sender, EventArgs e) {
            var fontName = _memoDefaultFontNameComboBox.Text;
            try {
                if (string.IsNullOrEmpty(fontName)) {
                    _memoDefaultFontNameComboBox.Text = _previousFontName;
                    return;
                }

                var family = new FontFamily(fontName);
                if (!family.IsStyleAvailable(FontStyle.Regular)) {
                    _memoDefaultFontNameComboBox.Text = _previousFontName;
                    return;
                }
            } catch (ArgumentException) {
                _memoDefaultFontNameComboBox.Text = _previousFontName;
                return;
            }

            _previousFontName = _memoDefaultFontNameComboBox.Text;
            _isModified = true;
        }

        // ========================================
        // type
        // ========================================
        //private struct DefaultShapeComboBoxItem {
        //    public string Id;
        //    public string Text;
        //    public DefaultShapeComboBoxItem(string id, string text) {
        //        Id = id;
        //        Text = text;
        //    }
        //    public override string ToString() {
        //        return Text;
        //    }
        //}
    }
}
