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
using Mkamo.Common.Forms.Drawing;
using Mkamo.Control.Core;

namespace Mkamo.Editor.Internal.Controls {
    internal partial class Completion: UserControl {
        public Completion() {
            InitializeComponent();

            _completionListBox.KeyDown += (se, e) => {
                if (e.KeyData == (Keys.N | Keys.Control)) {
                    ListBoxUtil.SelectNextItem(_completionListBox);
                    e.SuppressKeyPress = true;
                } else if (e.KeyData == (Keys.P | Keys.Control)) {
                    ListBoxUtil.SelectPreviousItem(_completionListBox);
                    e.SuppressKeyPress = true;
                }
            };
        }
        
        public string SelectedString {
            get { return _completionListBox.SelectedItem as string; }
        }

        public void SetItems(string[] items) {
            _completionListBox.BeginUpdate();
            _completionListBox.Items.AddRange(items);
            if (_completionListBox.Items.Count > 0) {
                _completionListBox.SelectedIndex = 0;
            }
            _completionListBox.EndUpdate();
        }

        public override Size GetPreferredSize(Size constrainingSize) {
            return new Size(_completionListBox.Width + 2, _completionListBox.Height + 2);
        }
        
        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);

            GraphicsUtil.SetupGraphics(e.Graphics, GraphicQuality.MaxQuality);
            var rect = new Rectangle(Bounds.Location, new Size(Bounds.Width - 1, Bounds.Height - 1));
            e.Graphics.DrawRectangle(Pens.Gray, rect);
        }
    }
}
