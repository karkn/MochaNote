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
    [ToolboxItem(false)]
    internal class MarkSelectTextBox: KryptonTextBox {
        // ========================================
        // field
        // ========================================
        private IEnumerable<MemoMarkDefinition> _checkedMarkDefs = null;
        private bool _isAnyChecked = true;

        // ========================================
        // constructor
        // ========================================
        public MarkSelectTextBox() {
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
                ShowMarkSelectForm();
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
        public IEnumerable<MemoMarkKind> CheckedMarkKinds {
            get {
                return
                    _checkedMarkDefs == null?
                        new MemoMarkKind[0]:
                        _checkedMarkDefs.Select(def => def.Kind).ToArray();
            }
            set {
                if (value == null) {
                    _checkedMarkDefs = null;
                } else {
                    var newDefs = new List<MemoMarkDefinition>();
                    var allDefs = MemoMarkUtil.GetMemoMarkDefinitions();
                    foreach (var def in allDefs) {
                        if (value.Any(v => v == def.Kind)) {
                            newDefs.Add(def);
                        }
                    }
                    _checkedMarkDefs = newDefs;
                }
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
            _checkedMarkDefs = null;
            _isAnyChecked = true;
            UpdateText();
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);
            if (e.Control && e.KeyCode == Keys.Enter) {
                ClearCondition();
                OnValueChanged();
            } else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space) {
                ShowMarkSelectForm();
            }
        }

        protected virtual void OnValueChanged() {
            var handler = ValueChanged;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        private void UpdateText() {
            if (_checkedMarkDefs == null || !_checkedMarkDefs.Any()) {
                Text = "条件なし";
                StateCommon.Content.Color1 = SystemColors.GrayText;
            } else {
                var text = new StringBuilder();

                if (_checkedMarkDefs != null) {
                    foreach (var markDef in _checkedMarkDefs.OrderBy(d => d.Name)) {
                        if (text.Length > 0) {
                            text.Append(", ");
                        }
                        text.Append(markDef.Name);
                    }
                }

                Text = text.ToString();
                StateCommon.Content.Color1 = Color.Black;
            }
        }


        private void ShowMarkSelectForm() {
            using (var dialog = new MarkSelectForm()) {
                dialog.CheckedMarkDefinitions = _checkedMarkDefs;
                dialog.IsAnyChecked = _isAnyChecked;
                
                if (dialog.ShowDialog(MemopadApplication.Instance.MainForm) == DialogResult.OK) {
                    var oldCursor = Cursor.Current;
                    try {
                        Cursor.Current = Cursors.WaitCursor;

                        _checkedMarkDefs = dialog.CheckedMarkDefinitions;
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
            ShowMarkSelectForm();
        }

        private void HandleCloseButtonSpecClick(object sender, EventArgs e) {
            ClearCondition();
            OnValueChanged();
        }
    }
}
