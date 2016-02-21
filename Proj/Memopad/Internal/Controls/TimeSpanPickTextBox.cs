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
using Mkamo.Memopad.Core;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Common.Win32.User32;

namespace Mkamo.Memopad.Internal.Controls {
    [ToolboxItem(false)]
    internal class TimeSpanPickTextBox: KryptonTextBox {
        // ========================================
        // field
        // ========================================
        private TimeSpanTargetKind _timeSpanTargetKind;
        private bool _isRecentTimeSpanSelected;
        private MemoRecentTimeSpan _recentTimeSpan;
        private bool _isTimeSpanSelected;
        private MemoTimeSpan _timeSpan;

        // ========================================
        // constructor
        // ========================================
        public TimeSpanPickTextBox() {
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
                ShowTimeSpanPickForm();
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
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TimeSpanTargetKind TimeSpanTargetKind {
            get { return _timeSpanTargetKind; }
            set {
                if (value == _timeSpanTargetKind) {
                    return;
                }
                _timeSpanTargetKind = value;
                UpdateText();
                OnValueChanged();
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsRecentTimeSpanSelected {
            get { return _isRecentTimeSpanSelected; }
            set{
                if (value == _isRecentTimeSpanSelected) {
                    return;
                }
                _isRecentTimeSpanSelected = value;
                UpdateText();
                OnValueChanged();
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MemoRecentTimeSpan RecentTimeSpan {
            get { return _recentTimeSpan; }
            set {
                if (value.Equals(_recentTimeSpan)) {
                    return;
                }
                _recentTimeSpan = value;
                UpdateText();
                OnValueChanged();
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsTimeSpanSelected {
            get { return _isTimeSpanSelected; }
            set {
                if (value == _isTimeSpanSelected) {
                    return;
                }
                _isTimeSpanSelected = value;
                UpdateText();
                OnValueChanged();
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public MemoTimeSpan TimeSpan {
            get { return _timeSpan; }
            set {
                if (value.Equals(_timeSpan)) {
                    return;
                }
                _timeSpan = value;
                UpdateText();
                OnValueChanged();
            }
        }


        // ========================================
        // method
        // ========================================
        public void ClearCondition() {
            _timeSpanTargetKind = TimeSpanTargetKind.None;
            _isRecentTimeSpanSelected = false;
            _isTimeSpanSelected = false;
            UpdateText();
        }

        protected virtual void OnValueChanged() {
            var handler = ValueChanged;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyUp(e);
            if (e.Control && e.KeyCode == Keys.Enter) {
                ClearCondition();
                OnValueChanged();
            } else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space) {
                ShowTimeSpanPickForm();
            }
        }

        private void UpdateText() {
            if (_isRecentTimeSpanSelected) {
                
                if (_recentTimeSpan.Day > 0) {
                    Text =
                        "過去" + _recentTimeSpan.Day + "日間に" + ToString(_timeSpanTargetKind);
                } else if (_recentTimeSpan.Week > 0) {
                    Text =
                        "過去" + _recentTimeSpan.Week + "週間に" + ToString(_timeSpanTargetKind);
                } else if (_recentTimeSpan.Month > 0) {
                    Text =
                        "過去" + _recentTimeSpan.Month + "か月間に" + ToString(_timeSpanTargetKind);
                }
                StateCommon.Content.Color1 = Color.Black;

            } else if (_isTimeSpanSelected) {
                var toDate = _timeSpan.ToDate.AddDays(-1);
                Text =
                    _timeSpan.FromDate.ToShortDateString() + "～" + toDate.ToShortDateString() +
                    "に" + ToString(_timeSpanTargetKind);
                StateCommon.Content.Color1 = Color.Black;

            } else {
                Text = "条件なし";
                StateCommon.Content.Color1 = SystemColors.GrayText;
            }
        }

        private void ShowTimeSpanPickForm() {
            using (var dialog = new TimeSpanPickForm()) {
                var picker = dialog.TimeSpanPicker;
                picker.TargetKind = _timeSpanTargetKind;
                if (_isRecentTimeSpanSelected) {
                    if (_recentTimeSpan.Day > 0) {
                        picker.IsRecentDayChecked = true;
                        picker.RecentDay = _recentTimeSpan.Day;
                    } else if (_recentTimeSpan.Week > 0) {
                        picker.IsRecentWeekChecked = true;
                        picker.RecentWeek = _recentTimeSpan.Week;
                    } else if (_recentTimeSpan.Month > 0) {
                        picker.IsRecentMonthChecked = true;
                        picker.RecentMonth = _recentTimeSpan.Month;
                    }

                } else if (_isTimeSpanSelected) {
                    picker.IsDateChecked = true;
                    picker.FromDate = _timeSpan.FromDate;
                    picker.ToDate = _timeSpan.ToDate;
                }
                
                if (dialog.ShowDialog(MemopadApplication.Instance.MainForm) == DialogResult.OK) {
                    var oldCursor = Cursor.Current;
                    try {
                        Cursor.Current = Cursors.WaitCursor;

                        _isRecentTimeSpanSelected = false;
                        _isTimeSpanSelected = false;

                        _timeSpanTargetKind = picker.TargetKind;
                        if (picker.TargetKind != TimeSpanTargetKind.None) {
                            if (picker.IsRecentDayChecked) {
                                _isRecentTimeSpanSelected = true;
                                _recentTimeSpan = new MemoRecentTimeSpan();
                                _recentTimeSpan.DateKind = ToMemoDateKind(picker.TargetKind);
                                _recentTimeSpan.Day = picker.RecentDay;

                            } else if (picker.IsRecentWeekChecked) {
                                _isRecentTimeSpanSelected = true;
                                _recentTimeSpan = new MemoRecentTimeSpan();
                                _recentTimeSpan.DateKind = ToMemoDateKind(picker.TargetKind);
                                _recentTimeSpan.Week = picker.RecentWeek;

                            } else if (picker.IsRecentMonthChecked) {
                                _isRecentTimeSpanSelected = true;
                                _recentTimeSpan = new MemoRecentTimeSpan();
                                _recentTimeSpan.DateKind = ToMemoDateKind(picker.TargetKind);
                                _recentTimeSpan.Month = picker.RecentMonth;

                            } else if (picker.IsDateChecked) {
                                _isTimeSpanSelected = true;
                                _timeSpan = new MemoTimeSpan();
                                _timeSpan.DateKind = ToMemoDateKind(picker.TargetKind);
                                _timeSpan.FromDate = picker.FromDate;
                                _timeSpan.ToDate = picker.ToDate;
                            }
                        }

                        OnValueChanged();

                    } finally {
                        Cursor.Current = oldCursor;
                    }
                    UpdateText();
                }
            }
        }

        private void HandleSelectButtonSpecClick(object sender, EventArgs e) {
            ShowTimeSpanPickForm();
        }

        private void HandleCloseButtonSpecClick(object sender, EventArgs e) {
            ClearCondition();
            OnValueChanged();
        }

        private MemoDateKind ToMemoDateKind(TimeSpanTargetKind kind) {
            switch (kind) {
                case TimeSpanTargetKind.Modified: {
                    return MemoDateKind.Modified;
                }
                case TimeSpanTargetKind.Created: {
                    return MemoDateKind.Created;
                }
                case TimeSpanTargetKind.Accessed: {
                    return MemoDateKind.Accessed;
                }
            }

            throw new ArgumentException("kind");
        }

        private string ToString(TimeSpanTargetKind kind) {
            switch (kind) {
                case TimeSpanTargetKind.None: {
                    return "";
                }
                case TimeSpanTargetKind.Modified: {
                    return "更新";
                }
                case TimeSpanTargetKind.Created: {
                    return "作成";
                }
                case TimeSpanTargetKind.Accessed: {
                    return "アクセス";
                }
            }

            throw new ArgumentException("kind");
        }
    }
}
