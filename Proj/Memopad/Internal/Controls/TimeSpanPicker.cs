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
using Mkamo.Memopad.Properties;
using Mkamo.Memopad.Internal.Core;
using Mkamo.Memopad.Core;

namespace Mkamo.Memopad.Internal.Controls {
    [ToolboxItem(false)]
    internal partial class TimeSpanPicker: UserControl {
        // ========================================
        // field
        // ========================================
        /// <summary>
        /// control間のスペース．
        /// </summary>
        private const int ItemSpace = 5;

        /// <summary>
        /// borderを自分で描くコントロールの本当のboundsとborder間の距離
        /// </summary>
        private const int ItemPadding = 3;

        /// <summary>
        /// ラベルと入力コントロール間のスペース．
        /// </summary>
        private const int LabelSpace = 5;

        private Font _captionFont;
        private Font _inputFont;


        // ========================================
        // constructor
        // ========================================
        public TimeSpanPicker() {
            InitializeComponent();

            _targetComboBox.SelectedIndexChanged += HandleTargetComboBoxSelectedIndexChanged;

            _dayComboBox.SelectedIndexChanged += HandleSomeChanged;
            _weekComboBox.SelectedIndexChanged += HandleSomeChanged;
            _monthComboBox.SelectedIndexChanged += HandleSomeChanged;

            _toDateTimePicker.ValueChanged += HandleSomeChanged;
            _fromDateTimePicker.ValueChanged += HandleSomeChanged;

            _dayRadioButton.CheckedChanged += HandleRadioButtonCheckedChanged;
            _weekRadioButton.CheckedChanged += HandleRadioButtonCheckedChanged;
            _monthRadioButton.CheckedChanged += HandleRadioButtonCheckedChanged;
            _dateRadioButton.CheckedChanged += HandleRadioButtonCheckedChanged;

            InitUIState();
        }

        private void InitUIState() {
            _fromDateTimePicker.Value = DateTime.Today.AddMonths(-1);
            _dayRadioButton.Checked = true;
            UpdateUIState();
        }

        // ========================================
        // event
        // ========================================
        public event EventHandler ValueChanged;

        // ========================================
        // property
        // ========================================
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TimeSpanTargetKind TargetKind {
            get {
                switch (_targetComboBox.SelectedIndex) {
                    case 0: {
                        return TimeSpanTargetKind.None;
                    }
                    case 1: {
                        return TimeSpanTargetKind.Modified;
                    }
                    case 2: {
                        return TimeSpanTargetKind.Created;
                    }
                    case 3: {
                        return TimeSpanTargetKind.Accessed;
                    }
                }
                return TimeSpanTargetKind.None;
            }
            set {
                switch (value) {
                    case TimeSpanTargetKind.None: {
                        _targetComboBox.SelectedIndex = 0;
                        break;
                    }
                    case TimeSpanTargetKind.Modified: {
                        _targetComboBox.SelectedIndex = 1;
                        break;
                    }
                    case TimeSpanTargetKind.Created: {
                        _targetComboBox.SelectedIndex = 2;
                        break;
                    }
                    case TimeSpanTargetKind.Accessed: {
                        _targetComboBox.SelectedIndex = 3;
                        break;
                    }
                }
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsRecentDayChecked {
            get { return _dayRadioButton.Checked; }
            set { _dayRadioButton.Checked = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsRecentWeekChecked {
            get { return _weekRadioButton.Checked; }
            set { _weekRadioButton.Checked = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsRecentMonthChecked {
            get { return _monthRadioButton.Checked; }
            set { _monthRadioButton.Checked = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsDateChecked {
            get { return _dateRadioButton.Checked; }
            set { _dateRadioButton.Checked = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int RecentDay {
            get { return int.Parse(_dayComboBox.Text); }
            set { _dayComboBox.Text = value.ToString(); }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int RecentWeek {
            get { return int.Parse(_weekComboBox.Text); }
            set { _weekComboBox.Text = value.ToString(); }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int RecentMonth {
            get { return int.Parse(_monthComboBox.Text); }
            set { _monthComboBox.Text = value.ToString(); }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DateTime FromDate {
            get { return _fromDateTimePicker.Value.Date; }
            set { _fromDateTimePicker.Value = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DateTime ToDate {
            get {
                var ret = default(DateTime);
                ret = _toDateTimePicker.Value.Date;
                ret = ret.AddDays(1);
                return ret;
            }
            set {
                var date = value;
                date = date.AddDays(-1);
                _toDateTimePicker.Value = date;
            }
        }

        //public DateTime FromDate {
        //    get {
        //        var ret = default(DateTime);

        //        if (_dayRadioButton.Checked) {
        //            ret = DateTime.Today;
        //            ret = ret.AddDays(-(int.Parse(_dayComboBox.Text)));
        //        }

        //        if (_weekRadioButton.Checked) {
        //            ret = DateTime.Today;
        //            ret = ret.AddDays(-(int.Parse(_weekComboBox.Text) * 7));
        //        }

        //        if (_monthRadioButton.Checked) {
        //            ret = DateTime.Today;
        //            ret = ret.AddMonths(-(int.Parse(_monthComboBox.Text)));
        //        }

        //        if (_dateRadioButton.Checked) {
        //            ret = _fromDateTimePicker.Value.Date;
        //        }

        //        return ret;
        //    }
        //}

        //public DateTime ToDate {
        //    get {
        //        var ret = default(DateTime);

        //        if (_dateRadioButton.Checked) {
        //            ret = _toDateTimePicker.Value.Date;
        //            ret = ret.AddDays(1);
        //        } else {
        //            ret = DateTime.Today;
        //            ret = ret.AddDays(1);
        //        }

        //        return ret;
        //    }
        //}

        public Font CaptionFont {
            set {
                _captionFont = value;
                _dayRadioButton.Font = _captionFont;
                _dayLabel.Font = _captionFont;
                _weekRadioButton.Font = _captionFont;
                _weekLabel.Font = _captionFont;
                _monthRadioButton.Font = _captionFont;
                _monthLabel.Font = _captionFont;
                _dateRadioButton.Font = _captionFont;
            }
        }

        public Font InputFont {
            set {
                _inputFont = value;
                _targetComboBox.Font = _inputFont;
                _dayComboBox.Font = _inputFont;
                _weekComboBox.Font = _inputFont;
                _monthComboBox.Font = _inputFont;
                _fromDateTimePicker.Font = _inputFont;
                _toDateTimePicker.Font = _inputFont;
            }
        }

        // ========================================
        // method
        // ========================================
        protected override void OnResize(EventArgs e) {
            base.OnResize(e);

            //Arrange();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);

            var g = e.Graphics;
            var r = new Rectangle(Point.Empty, Size);
            ControlPaint.DrawBorder(g, r, SystemColors.ControlDark, ButtonBorderStyle.Solid);
        }

        //protected virtual void Arrange() {
        //    SuspendLayout();

        //    _targetComboBox.Left = ItemSpace;
        //    _targetComboBox.Width = Width - ItemSpace * 2;
        //    _targetComboBox.DropDownWidth = _targetComboBox.Width > 0? _targetComboBox.Width: 10;

        //    ResumeLayout();
        //}

        private void HandleRadioButtonCheckedChanged(object sender, EventArgs e) {
            UpdateUIState();
            OnValueChanged();
        }

        private void HandleTargetComboBoxSelectedIndexChanged(object sender, EventArgs e) {
            UpdateUIState();
            OnValueChanged();
        }

        private void HandleSomeChanged(object sender, EventArgs e) {
            OnValueChanged();
        }

        private void UpdateUIState() {
            _dayRadioButton.Enabled = _targetComboBox.SelectedIndex != 0;
            _weekRadioButton.Enabled = _targetComboBox.SelectedIndex != 0;
            _monthRadioButton.Enabled = _targetComboBox.SelectedIndex != 0;
            _dateRadioButton.Enabled = _targetComboBox.SelectedIndex != 0;

            _dayComboBox.Enabled =  _dayRadioButton.Enabled && _dayRadioButton.Checked;
            _weekComboBox.Enabled = _weekRadioButton.Enabled && _weekRadioButton.Checked;
            _monthComboBox.Enabled = _monthRadioButton.Enabled && _monthRadioButton.Checked;
            _fromDateTimePicker.Enabled = _dateRadioButton.Enabled && _dateRadioButton.Checked;
            _toDateTimePicker.Enabled = _dateRadioButton.Enabled && _dateRadioButton.Checked;
        }

        protected virtual void OnValueChanged() {
            var handler = ValueChanged;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
