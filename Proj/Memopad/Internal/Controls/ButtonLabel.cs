/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.ComponentModel;

namespace Mkamo.Memopad.Internal.Controls {
    /// <summary>
    /// ツールボタンのように見えるラベル．
    /// </summary>
    [ToolboxItem(false)]
    internal class ButtonLabel: Label {
        private Color _originalBackColor;

        public ButtonLabel() {
            AutoSize = false;
            Size = new Size(24, 24);
            //BorderStyle = BorderStyle.FixedSingle;
        }

        protected override void OnMouseEnter(EventArgs e) {
            base.OnMouseEnter(e);
            _originalBackColor = BackColor;
            BackColor = Color.FromArgb(32, SystemColors.Highlight);
        }

        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);
            BackColor = _originalBackColor;
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            base.OnMouseDown(e);
            BackColor = Color.FromArgb(128, SystemColors.Highlight);
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);
            BackColor = Color.FromArgb(32, SystemColors.Highlight);
        }
    }

}
