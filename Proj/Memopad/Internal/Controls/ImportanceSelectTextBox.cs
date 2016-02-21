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
using Mkamo.Memopad.Internal.Utils;
using Mkamo.Common.Win32.User32;

namespace Mkamo.Memopad.Internal.Controls {
    [ToolboxItem(true)]
    internal class ImportanceSelectTextBox: KryptonTextBox {
        // ========================================
        // field
        // ========================================
        private IEnumerable<MemoImportanceKind> _checkedImportanceKinds= null;

        // ========================================
        // constructor
        // ========================================
        public ImportanceSelectTextBox() {
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
                ShowImportanceSelectForm();
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
        public IEnumerable<MemoImportanceKind> CheckedImportanceKinds {
            get {
                return _checkedImportanceKinds == null ?
                    new MemoImportanceKind[0]:
                    _checkedImportanceKinds;
            }
            set {
                if (value == _checkedImportanceKinds) {
                    return;
                }
                _checkedImportanceKinds = value == null ? null : value.ToArray();
                UpdateText();
                OnValueChanged();
            }

        }


        // ========================================
        // method
        // ========================================
        public void ClearCondition() {
            _checkedImportanceKinds = null;
            UpdateText();
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);
            if (e.Control && e.KeyCode == Keys.Enter) {
                ClearCondition();
                OnValueChanged();
            } else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space) {
                ShowImportanceSelectForm();
            }
        }

        protected virtual void OnValueChanged() {
            var handler = ValueChanged;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        private void UpdateText() {
            if (_checkedImportanceKinds == null || !_checkedImportanceKinds.Any()) {
                Text = "条件なし";
                StateCommon.Content.Color1 = SystemColors.GrayText;

            } else {
                var text = new StringBuilder();

                if (_checkedImportanceKinds!= null) {
                    if (_checkedImportanceKinds.Contains(MemoImportanceKind.High)) {
                        if (text.Length > 0) {
                            text.Append(", ");
                        }
                        text.Append("重要度高");
                    }
                    if (_checkedImportanceKinds.Contains(MemoImportanceKind.Normal)) {
                        if (text.Length > 0) {
                            text.Append(", ");
                        }
                        text.Append("重要度普通");
                    }
                    if (_checkedImportanceKinds.Contains(MemoImportanceKind.Low)) {
                        if (text.Length > 0) {
                            text.Append(", ");
                        }
                        text.Append("重要度低");
                    }
                }

                Text = text.ToString();
                StateCommon.Content.Color1 = Color.Black;
            }
        }


        private void ShowImportanceSelectForm() {
            using (var dialog = new ImportanceSelectForm()) {
                dialog.CheckedImportanceKinds = _checkedImportanceKinds;
                
                if (dialog.ShowDialog(MemopadApplication.Instance.MainForm) == DialogResult.OK) {
                    var oldCursor = Cursor.Current;
                    try {
                        Cursor.Current = Cursors.WaitCursor;

                        _checkedImportanceKinds = dialog.CheckedImportanceKinds;

                        OnValueChanged();

                    } finally {
                        Cursor.Current = oldCursor;
                    }
                    UpdateText();
                }
            }
        }

        private void HandleSelectButtonSpecClick(object sender, EventArgs e) {
            ShowImportanceSelectForm();
        }

        private void HandleCloseButtonSpecClick(object sender, EventArgs e) {
            ClearCondition();
            OnValueChanged();
        }
    }
}
