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
using Mkamo.Common.Forms.Descriptions;
using Mkamo.Common.Forms.Themes;

namespace Mkamo.Control.Configurator {
    [ToolboxItem(false)]
    public partial class BackgroundConfigurator: UserControl {

        // ========================================
        // field
        // ========================================
        private bool _isModified;

        private ITheme _theme;

        // ========================================
        // constructor
        // ========================================
        public BackgroundConfigurator() {
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
                _noPaintRadioButton.Font = captionFont;
                _solidPaintRadioButton.Font = captionFont;
                _gradientPaintRadioButton.Font = captionFont;
                
                _solidBackgroundConfigurator.Theme = value;
                _gradientBackgroundConfigurator.Theme = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsModified {
            get {
                return
                    _isModified ||
                    (_solidPaintRadioButton.Checked && _solidBackgroundConfigurator.IsModified) ||
                    (_gradientPaintRadioButton.Checked && _gradientBackgroundConfigurator.IsModified);
            }

            set {
                _isModified = value;
                _solidBackgroundConfigurator.IsModified = value;
                _gradientBackgroundConfigurator.IsModified = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IBrushDescription Background {
            get {
                if (_solidPaintRadioButton.Checked) {
                    return new SolidBrushDescription(
                        _solidBackgroundConfigurator.Color,
                        _solidBackgroundConfigurator.Opacity
                    );
                }
                if (_gradientPaintRadioButton.Checked) {
                    return new GradientBrushDescription(
                        _gradientBackgroundConfigurator.StartColor,
                        _gradientBackgroundConfigurator.EndColor,
                        _gradientBackgroundConfigurator.Angle
                    );
                }

                return null;
            }

            set {
                if (value == null) {
                    _noPaintRadioButton.Checked = true;

                } else if (value.Kind == BrushKind.Solid) {
                    var solid = value as SolidBrushDescription;
                    _solidPaintRadioButton.Checked = true;
                    _solidBackgroundConfigurator.Color = solid.Color;
                    _solidBackgroundConfigurator.Opacity = solid.Opacity;

                } else if (value.Kind == BrushKind.Gradient) {
                    var grad = value as GradientBrushDescription;
                    var blend = grad.ColorBlend;
                    _gradientPaintRadioButton.Checked = true;
                    _gradientBackgroundConfigurator.StartColor = grad.Color1;
                    _gradientBackgroundConfigurator.EndColor = grad.Color2;
                    _gradientBackgroundConfigurator.Angle = grad.Angle;

                } else {
                    _noPaintRadioButton.Checked = true;
                }
            }
        }

        // ========================================
        // method
        // ========================================
        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            Invalidate();
        }

        private void _noPaintRadioButton_CheckedChanged(object sender, EventArgs e) {
            if (_noPaintRadioButton.Checked) {
                _solidBackgroundConfigurator.Hide();
                _gradientBackgroundConfigurator.Hide();
            }
            _isModified = true;
        }

        private void _solidPaintRadioButton_CheckedChanged(object sender, EventArgs e) {
            if (_solidPaintRadioButton.Checked) {
                _solidBackgroundConfigurator.Show();
                _gradientBackgroundConfigurator.Hide();
            }
            _isModified = true;
        }

        private void _gradientPaintRadioButton_CheckedChanged(object sender, EventArgs e) {
            if (_gradientPaintRadioButton.Checked) {
                _solidBackgroundConfigurator.Hide();
                _gradientBackgroundConfigurator.Show();
            }
            _isModified = true;
        }
    }
}
