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
using Mkamo.Common.Forms.Themes;

namespace Mkamo.Control.Configurator {
    [ToolboxItem(false)]
    public partial class SolidBackgroundConfigurator: UserControl {
        // ========================================
        // field
        // ========================================
        private bool _isModified;

        private ITheme _theme;

        // ========================================
        // constructor
        // ========================================
        public SolidBackgroundConfigurator() {
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
                _opacityLabel.Font = captionFont;
                _showColorDialogButton.Font = captionFont;
            }
        }


        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsModified {
            get { return _isModified; }
            set { _isModified = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color Color {
            get { return _colorPanel.BackColor; }
            set {
                if (value == _colorPanel.BackColor) {
                    return;
                }
                _colorPanel.BackColor = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float Opacity {
            get { return (float) _opacityTrackBar.Value / 100; }
            set {
                var v = (int) (value * 100);
                if (v == _opacityTrackBar.Value) {
                    return;
                }
                _opacityTrackBar.Value = v;
            }
        }

        // ========================================
        // method
        // ========================================
        private void _showColorDialogButton_Click(object sender, EventArgs e) {
            var dialog = new ColorDialog();
            dialog.Color = Color;
            if (dialog.ShowDialog() == DialogResult.OK) {
                Color = dialog.Color;
                _isModified = true;
            }
        }

        private void _opacityTrackBar_ValueChanged(object sender, EventArgs e) {
            _isModified = true;
        }

    }
}
