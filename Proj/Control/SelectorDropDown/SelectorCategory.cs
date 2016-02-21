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

namespace Mkamo.Control.SelectorDropDown {
    [ToolboxItem(false)]
    public partial class SelectorCategory: Panel {
        // ========================================
        // field
        // ========================================
        private System.Windows.Forms.FlowLayoutPanel _panel;
        private TitleLabel _titleLabel;

        private List<Label> _labels;
        private ToolTip _toolTip;
        private Size _labelSize = new Size(20, 20);
        private int _maxCols = 8;

        private int _currentCol = 0;
        private int _rowCount = 1;

        // ========================================
        // constructor
        // ========================================
        public SelectorCategory(string title) {
            InitializeComponent();

            BackColor = Color.FromArgb(235, 235, 235);
            _panel.AutoSize = true;
            _toolTip = new ToolTip();

            _titleLabel.Text = title;

            _labels = new List<Label>();
        }

        // ========================================
        // property
        // ========================================
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Title {
            get { return _titleLabel.Text; }
            set {
                if (value == _titleLabel.Text) {
                    return;
                }
                _titleLabel.Text = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Size LabelSize {
            get { return _labelSize; }
            set { _labelSize = value; }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int MaxCols {
            get { return _maxCols; }
            set { _maxCols = value; }
        }

        public ToolTip ToolTip {
            get { return _toolTip; }
        }

        public override Font Font {
            get { return base.Font; }
            set {
                base.Font = value;
                _titleLabel.Font = new Font(value, FontStyle.Bold);
                foreach (var label in _labels) {
                    label.Font = value;
                }
            }
        }

        // ========================================
        // method
        // ========================================
        public override Size GetPreferredSize(Size proposedSize) {
            //return new Size(
            //    _labelSize.Width * _maxCols,
            //    _labelSize.Height * ((_labels.Count - 1) / _maxCols + 1) + _titleLabel.Height
            //);
            return new Size(
                _labelSize.Width * _maxCols,
                _labelSize.Height * (_rowCount) + _titleLabel.Height
            );
        }

        public void AddLabel(Image image, string text, Action action) {
            AddLabel(image, text, action, false);
        }

        public void AddLabel(Image image, string text, Action action, bool flowBreak) {
            var label = new ItemLabel() {
                Margin = Padding.Empty,
                Image = image,
                Size = _labelSize,
            };
            label.Click += (sender, e) => action();

            ++_currentCol;
            if (_currentCol > _maxCols) {
                _currentCol = 0;
                ++_rowCount;
            }

            _panel.Controls.Add(label);
            if (flowBreak && _currentCol != 0) {
                _panel.SetFlowBreak(label, true);
                _currentCol = 0;
                ++_rowCount;
            }
            _labels.Add(label);
            _toolTip.SetToolTip(label, text);
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
        }

        private void InitializeComponent() {
            _panel = new FlowLayoutPanel();
            _titleLabel = new TitleLabel();

            SuspendLayout();

            _panel.Dock = DockStyle.Bottom;
            _panel.Location = Point.Empty;
            _panel.Name = "panel";
            _panel.Size = new System.Drawing.Size(10, 10);
            _panel.TabIndex = 1;

            _titleLabel.Dock = DockStyle.Top;
            _titleLabel.Font = new Font("MS UI Gothic", 9F, FontStyle.Bold, GraphicsUnit.Point, ((byte) (128)));
            _titleLabel.Location = Point.Empty;
            _titleLabel.Name = "titleLabel";
            _titleLabel.Size = new Size(10, 22);
            _titleLabel.TabIndex = 2;
            _titleLabel.Text = "Title";

            AutoSize = true;

            Controls.Add(_titleLabel);
            Controls.Add(_panel);
            Name = "ToolCategory";

            ResumeLayout(false);
        }
    }

}
