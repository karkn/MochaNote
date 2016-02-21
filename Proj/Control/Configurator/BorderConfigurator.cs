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
    public partial class BorderConfigurator: UserControl {
        // ========================================
        // field
        // ========================================
        private bool _isModified;

        private ITheme _theme;

        // ========================================
        // constructor
        // ========================================
        public BorderConfigurator() {
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
                _noBorderRadioButton.Font = captionFont;
                _borderRadioButton.Font = captionFont;

                _lineConfigurator.Theme = value;
            }
        }


        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsModified {
            get {
                return
                    _isModified ||
                    (_borderRadioButton.Checked && _lineConfigurator.IsModified);
            }
            set {
                _isModified = value;
                _lineConfigurator.IsModified = value;
            }
        }


        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsBorderEnabled {
            get { return _borderRadioButton.Checked; }
            set {
                _borderRadioButton.Checked = value;
                _noBorderRadioButton.Checked = !value;
                if (value) {
                    _lineConfigurator.Show();
                } else {
                    _lineConfigurator.Hide();
                }
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color LineColor {
            get { return _lineConfigurator.LineColor; }
            set { _lineConfigurator.LineColor= value; }
        }

        /// <summary>
        /// _widthComboBoxの項目は1pt, 2pt, 3pt, 4pt
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int LineWidth {
            get { return _lineConfigurator.LineWidth; }
            set { _lineConfigurator.LineWidth = value; }
        }


        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DashStyle LineDashStyle {
            get { return _lineConfigurator.LineDashStyle; }
            set { _lineConfigurator.LineDashStyle = value; }
        }

        public LineConfigurator LineConfigurator {
            get { return _lineConfigurator; }
        }

        // ========================================
        // method
        // ========================================
        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            Invalidate();
        }

        private void _noBorderRadioButton_CheckedChanged(object sender, EventArgs e) {
            if (_noBorderRadioButton.Checked) {
                _lineConfigurator.Hide();
            }
            _isModified = true;
        }

        private void _borderRadioButton_CheckedChanged(object sender, EventArgs e) {
            if (_borderRadioButton.Checked) {
                _lineConfigurator.Show();
            }
            _isModified = true;
        }
    }
}
