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
using Mkamo.Common.DataType;
using Mkamo.Common.Forms.Themes;

namespace Mkamo.Control.Configurator {
    [ToolboxItem(false)]
    public partial class GradientBackgroundConfigurator: UserControl {
        // ========================================
        // field
        // ========================================
        private bool _isModified;

        private ITheme _theme;

        // ========================================
        // constructor
        // ========================================
        public GradientBackgroundConfigurator() {
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
                _directionLabel.Font = captionFont;
                _startColorLabel.Font = captionFont;
                _endColorLabel.Font = captionFont;
                _directionComboBox.Font = captionFont;
                _showStartColorDialogButton.Font = captionFont;
                _showEndColorDialogButton.Font = captionFont;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsModified {
            get { return _isModified; }
            set { _isModified = value; }
        }

       [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
         public float Angle {
            get { return IndexToAngle(_directionComboBox.SelectedIndex); }
            set { _directionComboBox.SelectedIndex = AngleToIndex(value); }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color StartColor {
            get { return _startColorPanel.BackColor; }
            set {
                if (value == _startColorPanel.BackColor) {
                    return;
                }
                _startColorPanel.BackColor = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color EndColor {
            get { return _endColorPanel.BackColor; }
            set {
                if (value == _endColorPanel.BackColor) {
                    return;
                }
                _endColorPanel.BackColor = value;
            }
        }


        // ========================================
        // method
        // ========================================
        /// <summary>
        /// Indexは上，下，左，右，左上，右上，左下，右下の順．
        /// </summary>
        private float IndexToAngle(int index) {
            switch (index) {
                case 0: {
                    return 270;
                }
                case 1: {
                    return 90;
                }
                case 2: {
                    return 180;
                }
                case 3: {
                    return 0;
                }
                case 4: {
                    return 225;
                }
                case 5: {
                    return 315;
                }
                case 6: {
                    return 135;
                }
                case 7: {
                    return 45;
                }
                default: {
                    return 0;
                }
            }
        }

        private int AngleToIndex(float angle) {
            var angleInt = (int) angle;
            switch (angleInt) {
                case 0: {
                    return 3;
                }
                case 45: {
                    return 7;
                }
                case 90: {
                    return 1;
                }
                case 135: {
                    return 6;
                }
                case 180: {
                    return 2;
                }
                case 225: {
                    return 4;
                }
                case 270: {
                    return 0;
                }
                case 315: {
                    return 5;
                }
                default: {
                    return 0;
                }
            }
        }

        private void _directionComboBox_SelectedIndexChanged(object sender, EventArgs e) {
            _isModified = true;            
        }

        private void _showStartColorDialogButton_Click(object sender, EventArgs e) {
            var dialog = new ColorDialog();
            dialog.Color = StartColor;
            if (dialog.ShowDialog() == DialogResult.OK) {
                StartColor = dialog.Color;
                _isModified = true;
            }
        }

        private void _showEndColorDialogButton_Click(object sender, EventArgs e) {
            var dialog = new ColorDialog();
            dialog.Color = EndColor;
            if (dialog.ShowDialog() == DialogResult.OK) {
                EndColor = dialog.Color;
                _isModified = true;
            }
        }
    }
}
