/*
 * Copyright (c) 2007-2012, Masahiko Kamo (mkamo@mkamo.com).
 * All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Permissions;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.VisualStyles;
using System.Security;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;

namespace Mkamo.Control.Core {
    [ToolboxItem(false)]
    public class Popup: ToolStripDropDown {
        // ========================================
        // static field
        // ========================================
        private const int WS_EX_NOACTIVATE = 0x08000000;

        // ========================================
        // field
        // ========================================
        private ToolStripControlHost _host;
        private System.Windows.Forms.Control _content;

        // ========================================
        // constructor
        // ========================================
        public Popup(System.Windows.Forms.Control content) {
            DoubleBuffered = true;
            Margin = Padding.Empty;
            Padding = Padding.Empty;
            TabStop = false;

            _content = content;
            _content.TabStop = false;

            _host = new ToolStripControlHost(_content);
            _host.Margin = Padding.Empty;
            _host.Padding = Padding.Empty;
            Items.Add(_host);
        }

        // ========================================
        // property
        // ========================================

        // ========================================
        // method
        // ========================================
        protected override CreateParams CreateParams {
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_NOACTIVATE;
                return cp;
            }
        }

        //protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
        //    if (keyData == Keys.Space) {
        //        Hide();
        //        return true;
        //    }
        //    return base.ProcessCmdKey(ref msg, keyData);
        //}
    }
}
