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
using Mkamo.Common.Forms.Themes;

namespace Mkamo.Control.Configurator {
    [ToolboxItem(false)]
    public partial class LineConfigurator: UserControl {
        // ========================================
        // field
        // ========================================
        private bool _isModified;

        private ITheme _theme;

        // ========================================
        // constructor
        // ========================================
        public LineConfigurator() {
            InitializeComponent();
            _isModified = false;
        }

        // ========================================
        // property
        // ========================================
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ITheme Theme {
            get { return _theme; }
            set {
                if (value == _theme) {
                    return;
                }
                _theme = value;

                var captionFont = value.CaptionFont;
                _colorLabel.Font = captionFont;
                _widthLabel.Font = captionFont;
                _styleLabel.Font = captionFont;
                _showColorDialogButton.Font = captionFont;
                _widthComboBox.Font = captionFont;
                _styleComboBox.Font = captionFont;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsModified {
            get { return _isModified; }
            set { _isModified = value; }
        }


        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color LineColor {
            get { return _colorPanel.BackColor; }
            set {
                if (value == _colorPanel.BackColor) {
                    return;
                }
                _colorPanel.BackColor = value;
            }
        }

        /// <summary>
        /// _widthComboBoxの項目は1pt, 2pt, 3pt, 4pt
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int LineWidth {
            get { return _widthComboBox.SelectedIndex + 1; }
            set {
                var v = value < 0 || value > 4? 1: value;
                _widthComboBox.SelectedIndex = v - 1;
            }
        }


        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DashStyle LineDashStyle {
            get { return IndexToDashStyle(_styleComboBox.SelectedIndex); }
            set { _styleComboBox.SelectedIndex = DashStyleToIndex(value); }
        }

        // ========================================
        // method
        // ========================================
        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            Invalidate();
        }

        private DashStyle IndexToDashStyle(int index) {
            switch (index) {
                case 0: {
                    return DashStyle.Solid;
                }
                case 1: {
                    return DashStyle.Dash;
                }
                case 2: {
                    return DashStyle.Dot;
                }
                case 3: {
                    return DashStyle.DashDot;
                }
                case 4: {
                    return DashStyle.DashDotDot;
                }
                default: {
                    return DashStyle.Solid;
                }
            }
        }

        private int DashStyleToIndex(DashStyle style) {
            switch (style) {
                case DashStyle.Solid: {
                    return 0;
                }
                case DashStyle.Dash: {
                    return 1;
                }
                case DashStyle.Dot: {
                    return 2;
                }
                case DashStyle.DashDot: {
                    return 3;
                }
                case DashStyle.DashDotDot: {
                    return 4;
                }
                default: {
                    return 0;
                }
            }
        }


        private void _showColorDialogButton_Click(object sender, EventArgs e) {
            var dialog = new ColorDialog();
            dialog.Color = LineColor;
            if (dialog.ShowDialog() == DialogResult.OK) {
                LineColor = dialog.Color;
                _isModified = true;
            }
        }

        private void _widthComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            _isModified = true;
        }

        private void _styleComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            _isModified = true;
        }

    }
}
