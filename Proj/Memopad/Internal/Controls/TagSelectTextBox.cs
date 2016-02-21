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
using ComponentFactory.Krypton.Toolkit;
using Mkamo.Memopad.Properties;
using Mkamo.Model.Memo;
using Mkamo.Memopad.Internal.Forms;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Common.Win32.User32;

namespace Mkamo.Memopad.Internal.Controls {
    [ToolboxItem(false)]
    internal class TagSelectTextBox: KryptonTextBox {
        // ========================================
        // field
        // ========================================
        private bool _isUntaggedChecked = false;
        private IEnumerable<MemoTag> _checkedTags = null;
        private bool _isAnyChecked = true;

        // ========================================
        // constructor
        // ========================================
        public TagSelectTextBox() {
            ReadOnly = true;

            var bspec = new ButtonSpecAny();
            bspec.Image = Resources.chevron_expand;
            bspec.Click += HandleSelectButtonSpecClick;
            ButtonSpecs.Add(bspec);

            bspec = new ButtonSpecAny();
            bspec.Type = PaletteButtonSpecStyle.Close;
            bspec.Click += HandleCloseButtonSpecClick;
            ButtonSpecs.Add(bspec);

            TextBox.MouseClick += (se, ev) => {
                ShowTagSelectForm();
            };
            TextBox.GotFocus += (se, ev) => {
                User32PI.HideCaret(TextBox.Handle);
            };

            UpdateText();
        }

        // ========================================
        // event
        // ========================================
        public event EventHandler ValueChanged;

        // ========================================
        // property
        // ========================================
        public bool IsUntaggedChecked {
            get { return _isUntaggedChecked; }
            set {
                if (value == _isUntaggedChecked) {
                    return;
                }
                _isUntaggedChecked = value;
                UpdateText();
                OnValueChanged();
            }
        }

        public IEnumerable<MemoTag> CheckedTags {
            get { return _checkedTags?? new MemoTag[0]; }
            set {
                if (value == _checkedTags) {
                    return;
                }
                _checkedTags = value;
                UpdateText();
                OnValueChanged();
            }
        }

        public bool IsAnyChecked {
            get { return _isAnyChecked; }
            set {
                if (value == _isAnyChecked) {
                    return;
                }
                _isAnyChecked = value;
                UpdateText();
                OnValueChanged();
            }
        }

        public bool IsAllChecked {
            get { return !_isAnyChecked; }
        }

        // ========================================
        // method
        // ========================================
        public void ClearCondition() {
            _isUntaggedChecked = false;
            _checkedTags = null;
            _isAnyChecked = true;
            UpdateText();
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);
            if (e.Control && e.KeyCode == Keys.Enter) {
                ClearCondition();
                OnValueChanged();
            } else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space) {
                ShowTagSelectForm();
            }
        }

        protected virtual void OnValueChanged() {
            var handler = ValueChanged;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        private void UpdateText() {
            if (!_isUntaggedChecked && (_checkedTags == null || !_checkedTags.Any())) {
                Text = "条件なし";
                StateCommon.Content.Color1 = SystemColors.GrayText;
            } else {
                var text = new StringBuilder();

                if (_isUntaggedChecked) {
                    text.Append("未整理");
                }

                if (_checkedTags != null) {
                    foreach (var tag in _checkedTags.OrderBy(tag => tag.Name)) {
                        if (text.Length > 0) {
                            text.Append(", ");
                        }
                        text.Append(tag.Name);
                    }
                }

                Text = text.ToString();
                StateCommon.Content.Color1 = Color.Black;
            }
        }

        private void ShowTagSelectForm() {
            using (var dialog = new TagSelectForm()) {
                dialog.IsUntaggedChecked = _isUntaggedChecked;
                dialog.CheckedTags = _checkedTags;
                dialog.IsAnyChecked = _isAnyChecked;

                if (dialog.ShowDialog(MemopadApplication.Instance.MainForm) == DialogResult.OK) {
                    var oldCursor = Cursor.Current;
                    try {
                        Cursor.Current = Cursors.WaitCursor;

                        _isUntaggedChecked = dialog.IsUntaggedChecked;
                        _checkedTags = dialog.CheckedTags;
                        _isAnyChecked = dialog.IsAnyChecked;

                        OnValueChanged();

                    } finally {
                        Cursor.Current = oldCursor;
                    }
                    UpdateText();
                }
            }
        }

        private void HandleSelectButtonSpecClick(object sender, EventArgs e) {
            ShowTagSelectForm();
        }

        private void HandleCloseButtonSpecClick(object sender, EventArgs e) {
            ClearCondition();
            OnValueChanged();
        }
    }
}
